using CreolytixECommerce.Application.Interfaces.Messaging;
using CreolytixECommerce.Application.Wrappers;
using CreolytixECommerce.Infrastructure.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MongoDB.Driver.WriteConcern;

namespace CreolytixECommerce.Infrastructure.Messaging
{
    public class RabbitMqListener : IMessageListener, IHostedService, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger<RabbitMqListener> _logger;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<object>> _pendingResponses;
        private readonly ConcurrentDictionary<string, Type> _expectedResponseTypes;
        private readonly ConcurrentDictionary<string, EventingBasicConsumer> _responseQueueConsumers;

        public RabbitMqListener(RabbitMqSettings settings, ILogger<RabbitMqListener> logger)
        {
            _logger = logger;
            _pendingResponses = new ConcurrentDictionary<string, TaskCompletionSource<object>>();
            _expectedResponseTypes = new ConcurrentDictionary<string, Type>();
            _responseQueueConsumers = new ConcurrentDictionary<string, EventingBasicConsumer>();

            var factory = new ConnectionFactory
            {
                HostName = settings.HostName,
                UserName = settings.UserName,
                Password = settings.Password,
                Port = settings.Port
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        // General-purpose listener for messages
        public async Task StartListeningAsync<T>(string queueName, Func<T, Task> handleMessage)
        {
            _channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(body));

                try
                {
                    await handleMessage(message);
                    _logger.LogInformation("Processed message from queue {QueueName}: {Message}", queueName, message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message from queue {QueueName}", queueName);
                }
            };

            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
            _logger.LogInformation("Started listening to queue {QueueName}", queueName);
        }

        // Await a response with a specific correlation ID
        public async Task<T> WaitForResponseAsync<T>(string queueName, string correlationId)
        {
            // Ensure a consumer exists for this specific response queue
            if (!_responseQueueConsumers.ContainsKey(queueName))
            {
                StartListeningForResponses(queueName);
            }

            // Register the TaskCompletionSource and expected type for this correlation ID
            var tcs = new TaskCompletionSource<object>();
            _pendingResponses[correlationId] = tcs;
            _expectedResponseTypes[correlationId] = typeof(T);

            try
            {
                // Wait for the response with the matching correlation ID
                var result = await tcs.Task.ConfigureAwait(false);

                // Clean up completed task
                _pendingResponses.TryRemove(correlationId, out _);
                _expectedResponseTypes.TryRemove(correlationId, out _);
                return (T)result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error waiting for response with correlation ID {CorrelationId}", correlationId);
                throw;
            }
        }

        // Start listening for responses on a specific queue
        private void StartListeningForResponses(string queueName)
        {
            // Ensure the queue is declared before consuming
            _channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            if (_responseQueueConsumers.ContainsKey(queueName))
                return;

            var consumer = new EventingBasicConsumer(_channel);

            // Event handler for messages on the response queue
            consumer.Received += (model, ea) =>
            {
                var correlationId = ea.BasicProperties.CorrelationId;

                if (_pendingResponses.TryGetValue(correlationId, out var tcs) &&
                    _expectedResponseTypes.TryGetValue(correlationId, out var expectedType))
                {
                    var body = ea.Body.ToArray();
                    var messageJson = Encoding.UTF8.GetString(body);

                    try
                    {
                        var wrapper = JsonConvert.DeserializeObject<MessageWrapper<object>>(messageJson);
                        var payloadJson = JsonConvert.SerializeObject(wrapper.Payload);
                        var payload = JsonConvert.DeserializeObject(payloadJson, expectedType);

                        tcs.SetResult(payload);
                        _logger.LogInformation("Received response for CorrelationId: {CorrelationId} on queue {QueueName}", wrapper.CorrelationId, queueName);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error deserializing response for CorrelationId: {CorrelationId} on queue {QueueName}", correlationId, queueName);
                        tcs.SetException(ex);
                    }
                    finally
                    {
                        _pendingResponses.TryRemove(correlationId, out _);
                        _expectedResponseTypes.TryRemove(correlationId, out _);
                    }
                }
                else
                {
                    _logger.LogWarning("No pending request found for CorrelationId: {CorrelationId} on queue {QueueName}", correlationId, queueName);
                }
            };

            // Start consuming messages from the specified response queue
            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
            _responseQueueConsumers[queueName] = consumer;
            _logger.LogInformation("Listening for responses on queue: {QueueName}", queueName);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("RabbitMQ Listener starting...");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("RabbitMQ Listener stopping...");
            Dispose();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
