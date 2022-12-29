using Microsoft.EntityFrameworkCore;
using ReviewEverything.Server.Data;
using ReviewEverything.Server.Models;
using ReviewEverything.Shared.Models.Enums;

namespace ReviewEverything.Server.Services.ReviewService
{
    public class ReviewService : IReviewService
    {
        private readonly AppDbContext _context;

        public ReviewService(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<Review> GetIncludesReview()
        {
            return _context.Reviews
                .Include(x => x.CloudImages)
                .Include(x => x.Author)
                .Include(x => x.Composition)
                .ThenInclude(x => x.Category)
                .Include(x => x.Composition)
                .ThenInclude(x => x.UserScores)
                .Include(x => x.Tags)
                .Include(x => x.LikeUsers)
                .AsQueryable();
        }

        public async Task<Review?> GetReviewByIdAsync(int id)
        {
            return await GetIncludesReview()
                .FirstOrDefaultAsync(review => review.Id == id);
        }

        public async Task<List<Review>> GetReviewsAsync(int page, int pageSize, SortReviewByProperty sortByProperty, int? filterByAuthorScore, int? categoryId, string? userId, List<int>? tags, CancellationToken token)
        {
            var reviews = GetIncludesReview()
                .Include(x => x.Comments)
                .AsQueryable();

            reviews = GetReviewsByAuthorId(reviews, userId);
            reviews = GetReviewsByCategoryId(reviews, categoryId);
            reviews = GetReviewsByTags(reviews, tags);
            reviews = FilterReviewsByAuthorScore(reviews, filterByAuthorScore);
            reviews = SortReviewsByProperty(reviews, sortByProperty);

            return await reviews
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(token);
        }

        private IQueryable<Review> GetReviewsByAuthorId(IQueryable<Review> reviews, string? authorId)
        {
            if (authorId == null)
                return reviews;

            return reviews
                .Where(x => x.AuthorId == authorId);
        }

        private IQueryable<Review> GetReviewsByCategoryId(IQueryable<Review> reviews, int? categoryId)
        {
            if (categoryId == null)
                return reviews;

            return reviews
                .Where(x => x.Composition.CategoryId == categoryId);
        }

        private IQueryable<Review> GetReviewsByTags(IQueryable<Review> reviews, List<int>? tags)
        {
            if (tags == null)
                return reviews;

            return reviews
                    .Where(x => _context.Tags
                        .Where(tag => tags.Contains(tag.Id))
                        .All(tag => x.Tags.Contains(tag)));
        }

        private IQueryable<Review> FilterReviewsByAuthorScore(IQueryable<Review> reviews, int? authorScore)
        {
            if (authorScore == null)
                return reviews;
            return reviews
                .Where(x => x.AuthorScore == authorScore);
        }

        private IQueryable<Review> SortReviewsByProperty(IQueryable<Review> reviews, SortReviewByProperty sortByProperty)
        {
            return sortByProperty switch
            {
                SortReviewByProperty.Latest => reviews.OrderByDescending(x => x.CreationDate),
                SortReviewByProperty.Oldest => reviews.OrderBy(x => x.CreationDate),
                SortReviewByProperty.Score => reviews.OrderByDescending(x => x.AuthorScore),
                SortReviewByProperty.Title => reviews.OrderBy(x => x.Title),
                _ => reviews
            };
        }

        public async Task<bool> CreateReviewAsync(Review review)
        {
            var tags = review.Tags;
            review.Tags = null!;
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
            review.Tags = tags;
            await _context.SaveChangesAsync();

            var created = await _context.SaveChangesAsync();

            return created > 0;
        }

        public async Task<bool> UpdateReviewAsync(Review review)
        {
            var updateReview = await GetReviewByIdAsync(review.Id);
            if (updateReview == null)
                return false;

            updateReview.UpdateDate = DateTime.UtcNow;
            updateReview.Title = review.Title;
            updateReview.Subtitle = review.Subtitle;
            updateReview.Body = review.Body;
            updateReview.CompositionId = review.CompositionId;

            var newTags = review.Tags.Where(x => !updateReview.Tags.Select(x => x.Id).Contains(x.Id)).ToList();
            updateReview.Tags.RemoveAll(x => !review.Tags.Select(x => x.Id).Contains(x.Id));
            updateReview.Tags.AddRange(newTags);

            var newImages = review.CloudImages.Where(x => !updateReview.CloudImages.Select(x => x.Id).Contains(x.Id)).ToList();
            updateReview.CloudImages.RemoveAll(x => !review.CloudImages.Select(x => x.Id).Contains(x.Id));
            updateReview.CloudImages.AddRange(newImages);
            _context.Reviews.Update(updateReview);
            var updated = await _context.SaveChangesAsync();
            return updated > 0;
        }

        public async Task<bool> DeleteReviewAsync(int id)
        {
            var review = await _context.Reviews.FirstOrDefaultAsync(x => x.Id == id);
            if (review == null)
                return false;

            _context.Reviews.Remove(review);
            var deleted = await _context.SaveChangesAsync();

            return deleted > 0;
        }

        public async Task<List<Review>?> GetSimilarArticleAsync(int reviewId)
        {
            var review = await _context.Reviews
                .Include(x => x.Author)
                .Include(x => x.Composition)
                .ThenInclude(x => x.Reviews)
                .ThenInclude(x => x.CloudImages)
                .Include(x => x.Composition)
                .ThenInclude(x => x.Reviews)
                .ThenInclude(x => x.Comments)
                .FirstOrDefaultAsync(x => x.Id == reviewId);
            if (review == null)
                return null!;
            review.Composition.Reviews.Remove(review);

            return review.Composition.Reviews;
        }

        public async Task<List<Review>> SearchReviewsAsync(string search)
        {
            var reviews = await _context.Reviews
                .Include(x => x.Comments)
                            .Where(x => EF.Functions.Like(x.Title.ToLower(), $"%{search.ToLower()}%")
                                        || x.Comments.Any(comment => EF.Functions.Like(comment.Body.ToLower(), $"%{search.ToLower()}%")))
                            .ToListAsync();
            return reviews;
        }
    }
}
