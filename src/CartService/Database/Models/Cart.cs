using System;
using System.Collections.Generic;

namespace CartService.Database.Models
{
    public class Cart
    {
        public Guid Id { get; set; }

        public ICollection<CartPosition>? CartPositions { get; set; }

        public Cart(Guid id)
        {
            Id = id;
        }
    }
}