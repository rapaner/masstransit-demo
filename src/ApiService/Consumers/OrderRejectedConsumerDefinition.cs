using MassTransit;

namespace ApiService.Consumers
{
    public class OrderRejectedConsumerDefinition : ConsumerDefinition<OrderRejectedConsumer>
    {
        public OrderRejectedConsumerDefinition()
        {
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<OrderRejectedConsumer> consumerConfigurator, IRegistrationContext context)
        {
            endpointConfigurator.UseDelayedRedelivery(r => r.Interval(5, 1000));
            endpointConfigurator.UseMessageRetry(r => r.Interval(5, 5000));
            endpointConfigurator.UseInMemoryOutbox(context);
        }
    }
}