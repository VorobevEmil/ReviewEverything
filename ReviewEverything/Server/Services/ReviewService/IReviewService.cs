using ReviewEverything.Server.Models;
using ReviewEverything.Shared.Models.Enums;

namespace ReviewEverything.Server.Services.ReviewService
{
    public interface IReviewService
    {
        Task<Review> GetReviewByIdAsync(int id);
        Task<List<Review>> GetReviewsAsync(int page, int pageSize, SortReviewByProperty sortByProperty, int? filterByAuthorScore, int? filterByCompositionId, int? categoryId, string? userId, List<int>? tags, CancellationToken token);
        Task<List<Review>> SearchReviewsAsync(string search);
        Task<bool> CreateReviewAsync(Review review);
        Task<bool> UpdateReviewAsync(Review updateReview);
        Task<bool> DeleteReviewAsync(int id);
        Task<List<Review>> GetSimilarArticleAsync(int reviewId);
    }
}
