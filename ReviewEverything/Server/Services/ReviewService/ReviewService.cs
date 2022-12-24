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

        private IQueryable<Review> GetReviewIncludeAll()
        {
            return _context.Reviews
                .Include(x => x.Author)
                .Include(x => x.CloudImages)
                .Include(x => x.Tags)
                .Include(x => x.Comments)
                .Include(x => x.LikeUsers)
                .Include(x => x.Composition)
                .ThenInclude(x => x.Category)
                .Include(x => x.Composition)
                .ThenInclude(x => x.UserScores)
                .AsQueryable();
        }

        public async Task<Review?> GetReviewByIdAsync(int id)
        {
            return await GetReviewIncludeAll()
                .FirstOrDefaultAsync(review => review.Id == id);
        }

        public async Task<List<Review>> GetReviewsAsync(int page, int pageSize, SortByProperty sortByProperty, int? filterByAuthorScore, int? categoryId, string? userId, List<int>? tags, CancellationToken token)
        {
            var reviews = GetReviewIncludeAll();
            reviews = GetReviewsByAuthorId(reviews, userId);
            reviews = GetReviewsByCategoryId(reviews, categoryId);
            reviews = FilterReviewsByAuthorScore(reviews, filterByAuthorScore);
            reviews = SortReviewsByProperty(reviews, sortByProperty);

            if (tags != null)
            {
                var tagList = await reviews
                    .SelectMany(x => x.Tags)
                    .Where(x => tags.Contains(x.Id))
                    .ToListAsync(token);
                if (tagList.Count == 0)
                    return new List<Review>();

                return (await reviews.ToListAsync(token))
                    .Where(x => tagList.All(tag => x.Tags.Select(x => x.Id).Contains(tag.Id)))
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }

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

        private IQueryable<Review> FilterReviewsByAuthorScore(IQueryable<Review> reviews, int? authorScore)
        {
            if (authorScore == null)
                return reviews;
            return reviews
                .Where(x => x.AuthorScore == authorScore);
        }

        private IQueryable<Review> SortReviewsByProperty(IQueryable<Review> reviews, SortByProperty sortByProperty)
        {
            return sortByProperty switch
            {
                SortByProperty.Latest => reviews.OrderByDescending(x => x.CreationDate),
                SortByProperty.Oldest => reviews.OrderBy(x => x.CreationDate),
                SortByProperty.Score => reviews.OrderByDescending(x => x.AuthorScore),
                SortByProperty.Title => reviews.OrderBy(x => x.Title),
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
            var review = await GetReviewByIdAsync(id);
            if (review == null)
                return false;

            _context.Reviews.Remove(review);
            var deleted = await _context.SaveChangesAsync();

            return deleted > 0;
        }

        public async Task<List<Review>> SearchReviewsAsync(string search)
        {
            var reviews = await _context.Reviews
                            .Where(p => EF.Functions.Like(p.Title.ToLower(), $"%{search.ToLower()}%"))
                            .ToListAsync();

            var comments = await _context.Comments
                            .Include(x => x.Review)
                            .Where(x => EF.Functions.Like(x.Body.ToLower(), $"%{search.ToLower()}%"))
                            .Where(x => !reviews.Select(x => x.Id).Contains(x.ReviewId))
                            .GroupBy(x => x.ReviewId)
                            .Select(x => x.First())
                            .ToListAsync();

            reviews.AddRange(comments.Select(x => x.Review).ToList());

            return reviews;
        }
    }
}
