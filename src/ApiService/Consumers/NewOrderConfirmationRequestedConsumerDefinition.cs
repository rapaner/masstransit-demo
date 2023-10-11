using MassTransit;

namespace ApiService.Consumers
{
    public class NewOrderConfirmationRequestedConsumerDefinition : ConsumerDefinition<NewOrderConfirmationRequestedConsumer>
    {
        public NewOrderConfirmationRequestedConsumerDefinition()
        {
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<NewOrderConfirmationRequestedConsumer> consumerConfigurator, IRegistrationContext context)
        {
            endpointConfigurator.UseDelayedRedelivery(r => r.Interval(5, 1000));
            endpointConfigurator.UseMessageRetry(r => r.Interval(5, 5000));
            endpointConfigurator.UseInMemoryOutbox(context);
        }
    }
}