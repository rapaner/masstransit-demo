using ApiService.Contracts.ManagerApi;
using CartService.Contracts;
using FeedbackService.Contracts;
using HistoryService.Contracts;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using OrderOrchestratorService.Consumers;
using OrderOrchestratorService.Tests.ConsumerStubs;
using System;
using System.Threading.Tasks;
using Xunit;

namespace OrderOrchestratorService.Tests
{
    public class GetArchivedOrderConsumerTests
    {
        [Fact]
        public async Task Test()
        {
            var serviceCollection = new ServiceCollection()
                .AddLogging()
                .AddMassTransitTestHarness(cfg =>
                {
                    cfg.AddRequestClient<GetCart>();
                    cfg.AddRequestClient<GetOrderFromArchive>();
                    cfg.AddRequestClient<GetOrderFeedback>();

                    cfg.AddConsumer<GetCartConsumer>();
                    cfg.AddConsumer<GetOrderFeedbackConsumer>();
                    cfg.AddConsumer<GetOrderFromArchiveConsumer>();

                    cfg.AddConsumer<GetArchivedOrderConsumer>();
                });

            var provider = serviceCollection.BuildServiceProvider(true);

            var harness = provider.GetRequiredService<ITestHarness>();

            await harness.Start();

            try
            {
                var orderId = Guid.NewGuid();

                var bus = provider.GetRequiredService<IBus>();
                var client = bus.CreateRequestClient<GetArchivedOrder>();

                var response = await client.GetResponse<GetArchivedOrderResponse>(new
                {
                    OrderId = orderId
                });

                var consumerTestHarness = provider
                    .GetRequiredService<IConsumerTestHarness<GetArchivedOrderConsumer>>();

                Assert.True(await harness.Consumed.Any<GetArchivedOrder>());
                Assert.True(await consumerTestHarness.Consumed.Any<GetArchivedOrder>());

                Assert.True(await harness.Sent.Any<GetArchivedOrderResponse>());

                Assert.NotNull(response);
                Assert.Equal(orderId, response.Message.OrderId);
            }
            finally
            {
                await provider.DisposeAsync();
            }
        }
    }
}