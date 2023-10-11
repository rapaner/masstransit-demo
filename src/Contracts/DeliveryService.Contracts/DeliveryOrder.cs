using Contracts.Shared;
using System;
using System.Collections.Generic;

namespace DeliveryService.Contracts
{
    public interface DeliveryOrder
    {
        public Guid OrderId { get; set; }

        public List<CartPosition> Cart { get; set; }
    }
}