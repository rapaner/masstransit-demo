using MassTransit;

namespace HistoryService.Consumers
{
    public class ArchivedOrderConsumerDefinition : ConsumerDefinition<ArchivedOrderConsumer>
    {
        public ArchivedOrderConsumerDefinition()
        {
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<ArchivedOrderConsumer> consumerConfigurator, IRegistrationContext context)
        {
            consumerConfigurator.UseDelayedRedelivery(r => r.Intervals(1000, 2000, 5000, 10000, 10000));
            consumerConfigurator.UseMessageRetry((r => r.Intervals(1000, 2000, 5000, 10000, 10000)));
            consumerConfigurator.UseInMemoryOutbox(context);
        }
    }
}