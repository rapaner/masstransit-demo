using ApiService.Contracts.ManagerApi;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ApiService.Consumers
{
    public class NewOrderConfirmationRequestedConsumer : IConsumer<NewOrderConfirmationRequested>
    {
        private readonly ILogger<NewOrderConfirmationRequestedConsumer> _logger;

        public NewOrderConfirmationRequestedConsumer(ILogger<NewOrderConfirmationRequestedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<NewOrderConfirmationRequested> context)
        {
            _logger.LogInformation($"NewOrderConfirmationRequested {context.Message.OrderId}");
            return Task.CompletedTask;
        }
    }
}