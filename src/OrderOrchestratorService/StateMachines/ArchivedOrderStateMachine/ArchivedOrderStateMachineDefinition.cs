using MassTransit;

namespace OrderOrchestratorService.StateMachines.ArchivedOrderStateMachine;

public class ArchivedOrderStateMachineDefinition : SagaDefinition<ArchivedOrderState>
{
    public ArchivedOrderStateMachineDefinition()
    {
    }

    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<ArchivedOrderState> sagaConfigurator, IRegistrationContext context)
    {
        endpointConfigurator.UseMessageRetry(r => r.Intervals(50, 100, 500, 1000));
        endpointConfigurator.UseInMemoryOutbox(context);
    }
}