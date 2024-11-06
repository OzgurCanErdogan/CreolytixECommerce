using CreolytixECommerce.Application.Interfaces.Messaging;
using CreolytixECommerce.Application.Queries.Stores;
using CreolytixECommerce.Application.Wrappers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreolytixECommerce.Infrastructure.Messaging.Consumers.Stores
{
    public class GetStoreByIdConsumer : BackgroundService
    {
        private readonly IMessageListener _messageListener;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMessagePublisher _messagePublisher;

        public GetStoreByIdConsumer(IMessageListener messageListener, IServiceProvider serviceProvider, IMessagePublisher messagePublisher)
        {
            _messageListener = messageListener;
            _serviceProvider = serviceProvider;
            _messagePublisher = messagePublisher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Listen for MessageWrapper<GetStoreByIdQuery>
            await _messageListener.StartListeningAsync<MessageWrapper<GetStoreByIdQuery>>("get_store_by_id_queue", async wrappedMessage =>
            {
                // Check if the cancellation token is requested to stop
                if (stoppingToken.IsCancellationRequested)
                    return;

                using var scope = _serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                // Extract the query from the wrapper
                var query = wrappedMessage.Payload;

                // Process the query using Mediator to get the store details
                var store = await mediator.Send(query, stoppingToken);

                // Wrap the response in MessageWrapper and reuse the original CorrelationId
                var responseWrapper = new MessageWrapper<object>(store)
                {
                    CorrelationId = wrappedMessage.CorrelationId  // Use the same CorrelationId
                };

                // Publish the wrapped response to the store_response_queue
                await _messagePublisher.PublishAsync("store_response_queue", responseWrapper);
            });
        }
    }
}
