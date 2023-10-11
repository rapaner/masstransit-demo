using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using PaymentService.Consumers;
using PaymentService.Contracts;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PaymentService.Tests
{
    public class ReserveMoneyConsumerTests
    {
        [Fact]
        public async Task MoneyReservationTest()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<ReserveMoneyConsumer>();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetRequiredService<ITestHarness>(); 
            var consumerHarness = harness.GetConsumerHarness<ReserveMoneyConsumer>();

            await harness.Start();

            try
            {
                var orderId = Guid.NewGuid();
                var amount = 100;

                var client = harness.GetRequestClient<ReserveMoney>();

                var response = await client.GetResponse<MoneyReserved>(new
                {
                    OrderId = orderId,
                    Amount = amount
                });

                Assert.Equal(orderId, response.Message.OrderId);

                Assert.True(consumerHarness.Consumed.Select<ReserveMoney>().Any());
                Assert.True(harness.Sent.Select<MoneyReserved>().Any());
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-20)]
        [InlineData(-1000)]
        public async Task ShouldNotReserveMoneyWhenAmointIsLessThanZero(int amount)
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<ReserveMoneyConsumer>();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetRequiredService<ITestHarness>();
            var consumerHarness = harness.GetConsumerHarness<ReserveMoneyConsumer>();

            await harness.Start();

            try
            {
                var orderId = Guid.NewGuid();

                var client = harness.GetRequestClient<ReserveMoney>();

                var response = await client.GetResponse<ErrorReservingMoney>(new
                {
                    OrderId = orderId,
                    Amount = amount
                });

                Assert.Equal(orderId, response.Message.OrderId);

                Assert.True(consumerHarness.Consumed.Select<ReserveMoney>().Any());

                Assert.True(harness.Sent.Select<ErrorReservingMoney>().Any());
                Assert.False(harness.Sent.Select<MoneyReserved>().Any());
            }
            finally
            {
                await harness.Stop();
            }
        }
    }
}