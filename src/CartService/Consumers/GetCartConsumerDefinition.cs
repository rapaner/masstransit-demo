using MassTransit;
using System;

namespace CartService.Consumers
{
    public class GetCartConsumerDefinition : ConsumerDefinition<GetCartConsumer>
    {
        public GetCartConsumerDefinition()
        {
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<GetCartConsumer> consumerConfigurator, IRegistrationContext context)
        {
            consumerConfigurator.UseDelayedRedelivery(r => r.Exponential(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(12), TimeSpan.FromSeconds(1.5)));
            consumerConfigurator.UseMessageRetry(r => r.Intervals(1000, 2000, 5000, 10000, 10000));
            consumerConfigurator.UseInMemoryOutbox(context);
        }
    }
}