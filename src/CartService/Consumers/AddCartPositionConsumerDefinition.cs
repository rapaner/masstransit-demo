using MassTransit;
using System;

namespace CartService.Consumers
{
    public class AddCartPositionConsumerDefinition : ConsumerDefinition<AddCartPositionConsumer>
    {
        public AddCartPositionConsumerDefinition()
        {
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<AddCartPositionConsumer> consumerConfigurator, IRegistrationContext context)
        {
            consumerConfigurator.UseDelayedRedelivery(r => r.Immediate(5));
            consumerConfigurator.UseMessageRetry(r => r.Incremental(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)));
            consumerConfigurator.UseInMemoryOutbox(context);
        }
    }
}