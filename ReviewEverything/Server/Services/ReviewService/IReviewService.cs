using ReviewEverything.Server.Models;

namespace ReviewEverything.Server.Services.ReviewService
{
    public interface IReviewService
    {
        Task<Review?> GetReviewByIdAsync(int id);
        Task<List<Review>> GetReviewsAsync(int? categoryId, string? userId, List<int>? tags, CancellationToken token);
        Task<List<Review>> SearchReviewsAsync(string search);
        Task<bool> CreateReviewAsync(Review review);
        Task<bool> UpdateReviewAsync(Review review);
        Task<bool> DeleteReviewAsync(int id);
    }
}
