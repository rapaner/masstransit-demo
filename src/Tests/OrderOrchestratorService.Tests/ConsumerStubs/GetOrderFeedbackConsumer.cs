using FeedbackService.Contracts;
using MassTransit;
using System.Threading.Tasks;

namespace OrderOrchestratorService.Tests.ConsumerStubs;

public class GetOrderFeedbackConsumer : IConsumer<GetOrderFeedback>
{
    public async Task Consume(ConsumeContext<GetOrderFeedback> context)
    {
        await context.RespondAsync<GetOrderFeedbackResponse>(new
        {
            OrderId = context.Message.OrderId,
            Text = default(string),
            StarsAmount = default(int)
        });
    }
}