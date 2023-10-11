using CartService.Contracts;
using CartService.Database.Repositories.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace CartService.Consumers
{
    public class RemoveCartPositionConsumer : IConsumer<RemoveCartPosition>
    {
        private readonly ILogger<RemoveCartPositionConsumer> _logger;
        private readonly ICartRepository _cartRepository;
        private readonly ICartPositionRepository _cartPositionRepository;

        public RemoveCartPositionConsumer(ILogger<RemoveCartPositionConsumer> logger,
            ICartRepository cartRepository,
            IGoodRepository goodRepository,
            ICartPositionRepository cartPositionRepository)
        {
            _logger = logger;
            _cartRepository = cartRepository;
            _cartPositionRepository = cartPositionRepository;
        }

        public async Task Consume(ConsumeContext<RemoveCartPosition> context)
        {
            var cartPositionForRemove = context.Message;

            var cart = await _cartRepository.GetCartWithCartPositionsAsync(cartPositionForRemove.OrderId);

            var cartPosition = cart.CartPositions!.FirstOrDefault(cp => cp.Good!.Name == cartPositionForRemove.Name);

            if (cartPosition != null)
            {
                if (cartPosition.Amount > cartPositionForRemove.Amount)
                {
                    await _cartPositionRepository.UpdateCartPositionAsync(cartPosition.Id, cartPosition.CartId, cartPosition.GoodId, cartPosition.Amount - cartPositionForRemove.Amount);
                }
                else
                {
                    await _cartPositionRepository.RemoveCartPositionAsync(cartPosition.Id);
                }
            }

            //to-do: respond
        }
    }
}