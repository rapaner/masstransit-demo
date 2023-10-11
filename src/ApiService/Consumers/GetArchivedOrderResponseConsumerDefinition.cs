﻿using MassTransit;

namespace ApiService.Consumers
{
    public class GetArchivedOrderResponseConsumerDefinition : ConsumerDefinition<GetArchivedOrderResponseConsumer>
    {
        public GetArchivedOrderResponseConsumerDefinition()
        {
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<GetArchivedOrderResponseConsumer> consumerConfigurator, IRegistrationContext context)
        {
            endpointConfigurator.UseDelayedRedelivery(r => r.Interval(5, 1000));
            endpointConfigurator.UseMessageRetry(r => r.Interval(5, 5000));
            endpointConfigurator.UseInMemoryOutbox(context);
        }
    }
}