using ApiService.Contracts.UserApi;
using MassTransit;
using System.Threading.Tasks;

namespace ApiService.Consumers
{
    public class FeedbackRequestedConsumer : IConsumer<FeedbackRequested>
    {
        public Task Consume(ConsumeContext<FeedbackRequested> context)
        {
            return Task.CompletedTask;
        }
    }
}