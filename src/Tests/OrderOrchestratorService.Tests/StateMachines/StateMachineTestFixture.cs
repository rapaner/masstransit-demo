﻿using MassTransit;
using MassTransit.Saga;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using OrderOrchestratorService.Configurations;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OrderOrchestratorService.Tests.StateMachines
{
#nullable disable

    public class StateMachineTestFixture<TStateMachine, TInstance> : IAsyncLifetime
        where TStateMachine : class, SagaStateMachine<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        public TStateMachine Machine;
        public ServiceProvider ServiceProvider;
        public ISagaStateMachineTestHarness<TStateMachine, TInstance> SagaHarness;
        public ITestHarness TestHarness;
        public IndexedSagaDictionary<TInstance> Repository;

        public Action<IBusRegistrationConfigurator> ConfigureMassTransit = null;
        public Action<IServiceCollection> ConfigureServices = null;

        private readonly TimeSpan _testTimeout = TimeSpan.FromSeconds(5);

        private IServiceCollection _сollection;
        private Task<IScheduler> _scheduler;

        public StateMachineTestFixture()
        {
        }

        public async Task InitializeAsync()
        {
            _сollection = new ServiceCollection()
                .AddTransient(sp => GetMockedEndpoints())
                .AddTransient(sp => GetMockedLogger())
                .AddMassTransitTestHarness(cfg =>
                {
                    cfg.AddSagaStateMachine<TStateMachine, TInstance>()
                        .InMemoryRepository();

                    cfg.AddPublishMessageScheduler();

                    if (ConfigureMassTransit != null)
                        ConfigureMassTransit(cfg);

                    cfg.AddQuartz();
                    cfg.AddQuartzConsumers();
                });

            if (ConfigureServices != null)
                ConfigureServices(_сollection);

            ServiceProvider = _сollection.BuildServiceProvider(true);

            TestHarness = ServiceProvider.GetTestHarness();
            TestHarness.TestTimeout = _testTimeout;

            var schedulerFactory = ServiceProvider.GetRequiredService<ISchedulerFactory>();
            _scheduler = schedulerFactory.GetScheduler();

            await TestHarness.Start();

            SagaHarness = ServiceProvider.GetRequiredService<ISagaStateMachineTestHarness<TStateMachine, TInstance>>();
            Machine = ServiceProvider.GetRequiredService<TStateMachine>();
            Repository = ServiceProvider.GetRequiredService<IndexedSagaDictionary<TInstance>>();
        }

        public async Task DisposeAsync()
        {
            try
            {
                await TestHarness.Stop();
            }
            finally
            {
                await ServiceProvider.DisposeAsync();
                RestoreDefaultQuartzSystemTime();
            }
        }

        private IOptions<EndpointsConfiguration> GetMockedEndpoints()
        {
            var endpointsConfigMock = new Mock<IOptions<EndpointsConfiguration>>();

            endpointsConfigMock.Setup(f => f.Value).Returns(new EndpointsConfiguration()
            {
                ArchiveOrderStateMachineAddress = "exchange:test",
                CartServiceAddress = "exchange:cart-service",
                DeliveryServiceAddress = "exchange:test",
                FeedbackServiceAddress = "exchange:test",
                HistoryServiceAddress = "exchange:test",
                MessageSchedulerAddress = "exchange:test",
                OrderStateMachineAddress = "exchange:test",
                PaymentServiceAddress = "exchange:payment-service"
            });

            return endpointsConfigMock.Object;
        }

        private ILogger<TStateMachine> GetMockedLogger()
        {
            return new Mock<ILogger<TStateMachine>>().Object;
        }

        public int ConvertStateToInt(MassTransit.State state)
        {
            return Machine.States.ToList().IndexOf(Machine.GetState(state.Name)) + 1;
        }

        public int ConvertStateToInt(Func<TStateMachine, MassTransit.State> stateSelector)
        {
            var state = stateSelector(Machine);
            return Machine.States.ToList().IndexOf(Machine.GetState(state.Name)) + 1;
        }

        public async Task AdvanceSystemTime(TimeSpan duration)
        {
            var scheduler = await _scheduler.ConfigureAwait(false);

            await scheduler.Standby().ConfigureAwait(false);

            SystemTime.UtcNow = () => DateTimeOffset.UtcNow + duration;
            SystemTime.Now = () => DateTimeOffset.Now + duration;

            await scheduler.Start().ConfigureAwait(false);
        }

        public void RestoreDefaultQuartzSystemTime()
        {
            SystemTime.UtcNow = () => DateTimeOffset.UtcNow;
            SystemTime.Now = () => DateTimeOffset.Now;
        }
    }

#nullable restore
}