using MassTransit;

namespace DeliveryService.Consumers
{
    public class DeliveryOrderConsumerDefinition : ConsumerDefinition<DeliveryOrderConsumer>
    {
        public DeliveryOrderConsumerDefinition()
        {
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<DeliveryOrderConsumer> consumerConfigurator, IRegistrationContext context)
        {
            consumerConfigurator.UseDelayedRedelivery(r => r.Intervals(1000, 2000, 5000, 10000, 10000));
            consumerConfigurator.UseMessageRetry((r => r.Intervals(1000, 2000, 5000, 10000, 10000)));
            consumerConfigurator.UseInMemoryOutbox(context);
        }
    }
}