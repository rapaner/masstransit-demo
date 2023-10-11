using MassTransit;
using System;

namespace CartService.Consumers
{
    public class RemoveCartPositionConsumerDefinition : ConsumerDefinition<RemoveCartPositionConsumer>
    {
        public RemoveCartPositionConsumerDefinition()
        {
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<RemoveCartPositionConsumer> consumerConfigurator, IRegistrationContext context)
        {
            consumerConfigurator.UseDelayedRedelivery(r => r.Incremental(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)));
            consumerConfigurator.UseMessageRetry(r => r.Immediate(5));
            consumerConfigurator.UseInMemoryOutbox(context);
        }
    }
}