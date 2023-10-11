using MassTransit;

namespace FeedbackService.Consumers
{
    public class GetOrderFeedbackConsumerDefinition : ConsumerDefinition<GetOrderFeedbackConsumer>
    {
        public GetOrderFeedbackConsumerDefinition()
        {
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<GetOrderFeedbackConsumer> consumerConfigurator, IRegistrationContext context)
        {
            consumerConfigurator.UseDelayedRedelivery(r => r.Intervals(1000, 2000, 5000, 10000, 10000));
            consumerConfigurator.UseMessageRetry((r => r.Intervals(1000, 2000, 5000, 10000, 10000)));
            consumerConfigurator.UseInMemoryOutbox(context);
        }
    }
}