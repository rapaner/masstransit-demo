using ApiService.Contracts.UserApi;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ApiService.Consumers
{
    public class OrderRejectedConsumer : IConsumer<OrderRejected>
    {
        private readonly ILogger<OrderRejectedConsumer> _logger;

        public OrderRejectedConsumer(ILogger<OrderRejectedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<OrderRejected> context)
        {
            _logger.LogInformation($"OrderRejected {context.Message.OrderId}");
            return Task.CompletedTask;
        }
    }
}