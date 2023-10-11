using ApiService.Contracts.ManagerApi;
using CartService.Contracts;
using FeedbackService.Contracts;
using HistoryService.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrderOrchestratorService.Configurations;
using System;
using System.Threading.Tasks;

namespace OrderOrchestratorService.StateMachines.ArchivedOrderStateMachine
{
    public class ArchivedOrderStateMachine : MassTransitStateMachine<ArchivedOrderState>
    {
#nullable disable
        private readonly ILogger<ArchivedOrderStateMachine> _logger;
        private readonly EndpointsConfiguration _endpointsConfiguration;

        public Event<GetArchivedOrder> ArchivedOrderRequested { get; set; }
        public Event InformationCollected { get; set; }

        public State AwaitingInformation { get; set; }

        public Request<ArchivedOrderState, GetOrderFromArchive, GetOrderFromArchiveResponse> ArchiveRequest { get; set; }
        public Request<ArchivedOrderState, GetOrderFeedback, GetOrderFeedbackResponse> OrderFeedbackRequest { get; set; }
        public Request<ArchivedOrderState, GetCart, GetCartResponse> CartRequest { get; set; }

        public ArchivedOrderStateMachine(IOptions<EndpointsConfiguration> settings,
            ILogger<ArchivedOrderStateMachine> logger)
        {
            _logger = logger;
            _endpointsConfiguration = settings.Value;

            InstanceState(x => x.CurrentState);

            BuildStateMachine();

            OnUnhandledEvent(HandleUnhandledEvent);

            Initially(WhenArchivedOrderRequested());

            During(AwaitingInformation,
                WhenCartRequestCompleted(),
                WhenArchiveRequestCompleted(),
                WhenOrderFeedbackRequestCompleted(),
                Ignore(ArchivedOrderRequested));

            DuringAny(
                When(InformationCollected)
                    .IfElse(context => context.ShouldBeResponded(),
                        responded => responded
                            .SendAsync(x => x.Saga.ResponseAddress,
                                    x => x.CreateArchivedOrderResponse(),
                                    (consumeContext, sendContext) =>
                                        sendContext.RequestId = consumeContext.Saga.RequestId),
                        published => published
                            .PublishAsync(x => x.CreateArchivedOrderResponse()))
                    .Finalize());

            CompositeEvent(() => InformationCollected,
                           x => x.InformationStatus,
                           CartRequest.Completed,
                           ArchiveRequest.Completed,
                           OrderFeedbackRequest.Completed);

            SetCompletedWhenFinalized();
        }

        private Task HandleUnhandledEvent(UnhandledEventContext<ArchivedOrderState> context)
        {
            _logger.LogDebug($"[{DateTime.Now}][SAGA] Ignored unhandled event: {context.Event.Name}");

            return Task.CompletedTask;
        }

        private void BuildStateMachine()
        {
            Event(() => ArchivedOrderRequested, x => x.CorrelateById(x => x.Message.OrderId));

            Request(() => ArchiveRequest, r =>
            {
                r.ServiceAddress = new Uri(_endpointsConfiguration.HistoryServiceAddress!);
            });

            Request(() => OrderFeedbackRequest, r =>
            {
                r.ServiceAddress = new Uri(_endpointsConfiguration.FeedbackServiceAddress!);
            });

            Request(() => CartRequest, r =>
            {
                r.ServiceAddress = new Uri(_endpointsConfiguration.CartServiceAddress!);
            });
        }

        private EventActivities<ArchivedOrderState> WhenArchivedOrderRequested()
        {
            return When(ArchivedOrderRequested)
                .TransitionTo(AwaitingInformation)
                .Then(x =>
                {
                    if (x.TryGetPayload(out SagaConsumeContext<ArchivedOrderState, GetArchivedOrder> payload))
                    {
                        x.Saga.RequestId = payload.RequestId;
                        x.Saga.ResponseAddress = payload.ResponseAddress;
                    }
                })
                .Request(CartRequest, x => x.Init<GetCart>(new
                {
                    OrderId = x.Saga.CorrelationId
                }))
                .Request(ArchiveRequest, x => x.Init<GetOrderFromArchive>(new
                {
                    OrderId = x.Saga.CorrelationId
                }))
                .Request(OrderFeedbackRequest, x => x.Init<GetOrderFeedback>(new
                {
                    OrderId = x.Saga.CorrelationId
                }));
        }

        private EventActivities<ArchivedOrderState> WhenCartRequestCompleted()
        {
            return When(CartRequest.Completed)
                .Then(x =>
                {
                    _logger.LogInformation("WhenOrderCartRequestCompleted");
                    x.Saga.Cart = x.Message.CartContent;
                    x.Saga.TotalPrice = x.Message.TotalPrice;
                });
        }

        private EventActivities<ArchivedOrderState> WhenArchiveRequestCompleted()
        {
            return When(ArchiveRequest.Completed)
                .Then(x =>
                {
                    _logger.LogInformation("WhenArchiveRequestCompleted");
                    x.Saga.ConfirmDate = x.Message.ConfirmDate;
                    x.Saga.SubmitDate = x.Message.SubmitDate;
                    x.Saga.IsConfirmed = x.Message.IsConfirmed;
                    x.Saga.Manager = x.Message.Manager;
                    x.Saga.DeliveredDate = x.Message.DeliveredDate;
                });
        }

        private EventActivities<ArchivedOrderState> WhenOrderFeedbackRequestCompleted()
        {
            return When(OrderFeedbackRequest.Completed)
                .Then(x =>
                {
                    _logger.LogInformation("WhenOrderFeedbackRequestCompleted");
                    x.Saga.FeedbackText = x.Message.Text;
                    x.Saga.FeedbackStars = x.Message.StarsAmount;
                });
        }
    }

    public static class ArchivedOrderStateMachineExtensions
    {
        public static Task<SendTuple<GetArchivedOrderResponse>> CreateArchivedOrderResponse(this BehaviorContext<ArchivedOrderState> context)
        {
            return context.Init<GetArchivedOrderResponse>(new
            {
                OrderId = context.Saga.CorrelationId,
                Cart = context.Saga.Cart,
                TotalPrice = context.Saga.TotalPrice,
                IsConfirmed = context.Saga.IsConfirmed,
                SubmitDate = context.Saga.SubmitDate,
                Manager = context.Saga.Manager,
                ConfirmDate = context.Saga.ConfirmDate,
                DeliveredDate = context.Saga.DeliveredDate,
                FeedbackText = context.Saga.FeedbackText,
                FeedbackStars = context.Saga.FeedbackStars,
            });
        }

        public static bool ShouldBeResponded(this BehaviorContext<ArchivedOrderState> context)
        {
            return context.Saga.RequestId.HasValue && context.Saga.ResponseAddress != null;
        }
    }

#nullable restore
}