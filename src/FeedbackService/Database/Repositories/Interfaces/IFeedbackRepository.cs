using FeedbackService.Database.Models;
using System;
using System.Threading.Tasks;

namespace FeedbackService.Database.Repositories.Interfaces
{
    public interface IFeedbackRepository
    {
        public Task AddFeedbackAsync(Guid id,
            string text,
            int starsAmount);

        public Task<Feedback> GetFeedbackAsync(Guid id);

        public Task<Feedback?> FindFeedbackAsync(Guid id);
    }
}