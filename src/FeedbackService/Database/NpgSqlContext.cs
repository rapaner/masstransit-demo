using FeedbackService.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace FeedbackService.Database
{
    public class NpgSqlContext : DbContext
    {
        public DbSet<Feedback>? Feedbacks { get; set; }

        public NpgSqlContext()
        {
        }

        public NpgSqlContext(DbContextOptions options) : base(options)
        {
        }
    }
}