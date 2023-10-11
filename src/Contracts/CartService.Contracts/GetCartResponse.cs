using Contracts.Shared;
using System;
using System.Collections.Generic;

namespace CartService.Contracts
{
    public interface GetCartResponse
    {
        public Guid OrderId { get; set; }

        public List<CartPosition> CartContent { get; set; }

        public int TotalPrice { get; set; }
    }
}