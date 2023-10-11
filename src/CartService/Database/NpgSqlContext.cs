using CartService.Database.Configurations;
using CartService.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace CartService.Database
{
    public class NpgSqlContext : DbContext
    {
        public DbSet<Cart>? Carts { get; set; }
        public DbSet<CartPosition>? CartPositions { get; set; }
        public DbSet<Good>? Goods { get; set; }

        public NpgSqlContext()
        {
        }

        public NpgSqlContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CartConfiguration());
            modelBuilder.ApplyConfiguration(new CartPositionConfiguration());
            modelBuilder.ApplyConfiguration(new GoodConfiguration());
        }
    }
}