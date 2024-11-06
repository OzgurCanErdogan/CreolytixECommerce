using CreolytixECommerce.Application.Interfaces.Messaging;
using CreolytixECommerce.Application.Queries.Products;
using CreolytixECommerce.Application.Wrappers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreolytixECommerce.Infrastructure.Messaging.Consumers.Product
{
    public class GetProductByIdConsumer : BackgroundService
    {
        private readonly IMessageListener _messageListener;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMessagePublisher _messagePublisher;

        public GetProductByIdConsumer(IMessageListener messageListener, IServiceProvider serviceProvider, IMessagePublisher messagePublisher)
        {
            _messageListener = messageListener;
            _serviceProvider = serviceProvider;
            _messagePublisher = messagePublisher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Listen for MessageWrapper<GetProductByIdQuery>
            await _messageListener.StartListeningAsync<MessageWrapper<GetProductByIdQuery>>("get_product_by_id_queue", async wrappedMessage =>
            {
                // Check if the cancellation token is requested to stop
                if (stoppingToken.IsCancellationRequested)
                    return;

                using var scope = _serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                // Extract the query from the wrapper
                var query = wrappedMessage.Payload;

                // Process the query using Mediator
                var product = await mediator.Send(query, stoppingToken);

                // Wrap the response in MessageWrapper and set the CorrelationId
                var responseWrapper = new MessageWrapper<object>(product)
                {
                    CorrelationId = wrappedMessage.CorrelationId  // Reuse the original CorrelationId
                };

                // Publish the wrapped response to the response queue
                await _messagePublisher.PublishAsync("product_response_queue", responseWrapper);
            });
        }
    }
}
