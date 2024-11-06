using CreolytixECommerce.Application.Interfaces.Messaging;
using CreolytixECommerce.Application.Wrappers;
using CreolytixECommerce.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MongoDB.Driver.WriteConcern;

namespace CreolytixECommerce.Infrastructure.Messaging
{
    public class RabbitMqPublisher : IMessagePublisher, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger<RabbitMqPublisher> _logger;

        public RabbitMqPublisher(RabbitMqSettings settings, ILogger<RabbitMqPublisher> logger)
        {
            _logger = logger;

            var factory = new ConnectionFactory
            {
                HostName = settings.HostName,
                UserName = settings.UserName,
                Password = settings.Password,
                Port = settings.Port,
            };

            _connection = factory.CreateConnection();
            
            _channel = _connection.CreateModel();
        }

        public async Task PublishAsync<T>(string queueName, MessageWrapper<T> messageWrapper, string replyToQueue = null)
        {
            try
            {
                // Declare the queue in case it doesn't exist
                _channel.QueueDeclare(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageWrapper));

                // Create message properties and set persistent delivery mode
                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;

                // Set correlation ID from MessageWrapper
                properties.CorrelationId = messageWrapper.CorrelationId;

                // Set reply-to queue if provided
                if (!string.IsNullOrEmpty(replyToQueue))
                {
                    properties.ReplyTo = replyToQueue;
                }

                // Publish the message
                _channel.BasicPublish(
                    exchange: "",
                    routingKey: queueName,
                    basicProperties: properties,
                    body: body);

                _logger.LogInformation("Message published to queue {QueueName} with CorrelationId: {CorrelationId}", queueName, messageWrapper.CorrelationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing message to queue {QueueName}", queueName);
            }
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
