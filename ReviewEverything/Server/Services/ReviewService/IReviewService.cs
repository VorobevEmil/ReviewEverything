using ReviewEverything.Server.Models;

namespace ReviewEverything.Server.Services.ReviewService
{
    public interface IReviewService
    {
        Task<Review?> GetReviewByIdAsync(int id);
        Task<List<Review>> GetReviewsAsync();
        Task<bool> CreateReviewAsync(Review review);
        Task<bool> UpdateReviewAsync(Review review);
        Task<bool> DeleteReviewAsync(int id);
    }
}
