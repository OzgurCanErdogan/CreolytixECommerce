using CreolytixECommerce.Application.Commands.Reservations;
using CreolytixECommerce.Application.Interfaces.Messaging;
using CreolytixECommerce.Application.Wrappers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreolytixECommerce.Infrastructure.Messaging.Consumers.Reservations
{
    public class CancelReservationConsumer : BackgroundService
    {
        private readonly IMessageListener _messageListener;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMessagePublisher _messagePublisher;

        public CancelReservationConsumer(IMessageListener messageListener, IServiceProvider serviceProvider, IMessagePublisher messagePublisher)
        {
            _messageListener = messageListener;
            _serviceProvider = serviceProvider;
            _messagePublisher = messagePublisher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Listen for MessageWrapper<CancelReservationCommand>
            await _messageListener.StartListeningAsync<MessageWrapper<CancelReservationCommand>>("cancel_reservation_queue", async wrappedMessage =>
            {
                // Check if the cancellation token is requested to stop
                if (stoppingToken.IsCancellationRequested)
                    return;

                // Create a new scope to resolve scoped services
                using var scope = _serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                // Extract the command from the wrapper
                var command = wrappedMessage.Payload;

                // Process the command using Mediator to cancel the reservation
                var result = await mediator.Send(command, stoppingToken);

                // Wrap the response (result) in a new MessageWrapper, using the original CorrelationId
                var responseWrapper = new MessageWrapper<object>(result)
                {
                    CorrelationId = wrappedMessage.CorrelationId  // Reuse the original CorrelationId
                };

                // Publish the wrapped response to the reservation_response_queue
                await _messagePublisher.PublishAsync("reservation_response_queue", responseWrapper);
            });
        }
    }
}
