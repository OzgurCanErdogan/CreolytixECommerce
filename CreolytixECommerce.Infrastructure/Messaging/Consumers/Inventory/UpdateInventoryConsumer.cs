using CreolytixECommerce.Application.Commands.Inventory;
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

namespace CreolytixECommerce.Infrastructure.Messaging.Consumers.Inventory
{
    public class UpdateInventoryConsumer : BackgroundService
    {
        private readonly IMessageListener _messageListener;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMessagePublisher _messagePublisher;

        public UpdateInventoryConsumer(IMessageListener messageListener, IServiceProvider serviceProvider, IMessagePublisher messagePublisher)
        {
            _messageListener = messageListener;
            _serviceProvider = serviceProvider;
            _messagePublisher = messagePublisher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Listen for MessageWrapper<UpdateInventoryCommand>
            await _messageListener.StartListeningAsync<MessageWrapper<UpdateInventoryCommand>>("update_inventory_queue", async wrappedMessage =>
            {
                // Check if the cancellation token is requested to stop
                if (stoppingToken.IsCancellationRequested)
                    return;

                // Create a new scope to resolve scoped services
                using var scope = _serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                // Extract the query from the wrapper
                var query = wrappedMessage.Payload;

                var inventory = await mediator.Send(query, stoppingToken);

                // Wrap the response in MessageWrapper and set the CorrelationId
                var responseWrapper = new MessageWrapper<object>(inventory)
                {
                    CorrelationId = wrappedMessage.CorrelationId  // Use the same CorrelationId
                };

                // Publish the wrapped response to the store_response_queue
                await _messagePublisher.PublishAsync("inventory_response_queue", responseWrapper);
            });
        }
    }
}
