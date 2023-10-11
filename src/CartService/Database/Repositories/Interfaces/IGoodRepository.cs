using CartService.Database.Models;
using System;
using System.Threading.Tasks;

namespace CartService.Database.Repositories.Interfaces
{
    public interface IGoodRepository
    {
        public Task<bool> GoodExistsAsync(string name);

        public Task<Good> GetGoodByNameAsync(string name);

        public Task AddGoodAsync(Guid id,
            string? name,
            int price);
    }
}