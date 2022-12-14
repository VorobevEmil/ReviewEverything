using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using ReviewEverything.Server.Data;
using ReviewEverything.Server.Models;
using System.Linq;

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

        public async Task<List<Review>> GetReviewsAsync(int? categoryId, string? userId, List<int>? tags)
        {
            var reviews = GetReviewIncludeAll();

            if (userId != null)
            {
                reviews = reviews
                    .Where(x => x.AuthorId == userId);
            }

            if (categoryId != null)
            {
                reviews = reviews
                    .Where(x => x.Composition.CategoryId == categoryId);
            }

            if (tags != null)
            {
                var tagList = await reviews
                    .SelectMany(x => x.Tags)
                    .Where(x => tags.Contains(x.Id))
                    .ToListAsync();
                var result = (await reviews.ToListAsync())
                    .Where(x => tagList
                        .All(tag => x.Tags.Select(x => x.Id).Contains(tag.Id)))
                    .ToList();
                return result;
            }

            return await reviews.ToListAsync();

            //return await _context.Reviews
            //    .Where(p => search != null! && EF.Functions.Like(p.Title.ToLower(), $"%{search.ToLower()}%"))
            //    .ToListAsync();
        }

        public async Task<bool> CreateReviewAsync(Review review)
        {
            var tags = review.Tags;
            review.Tags = null!;
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();



            var created = await _context.SaveChangesAsync();

            return created > 0;
        }

        public async Task<bool> UpdateReviewAsync(Review review)
        {
            var oldReview = await GetReviewByIdAsync(review.Id);
            if (oldReview == null)
                return false;

            oldReview.UpdateDate = DateTime.UtcNow;
            oldReview.Title = review.Title;
            oldReview.Subtitle = review.Subtitle;
            oldReview.Body = review.Body;
            oldReview.CompositionId = review.CompositionId;

            var newTags = review.Tags.Where(x => !oldReview.Tags.Select(x => x.Id).Contains(x.Id)).ToList();
            oldReview.Tags.RemoveAll(x => !review.Tags.Select(x => x.Id).Contains(x.Id));
            oldReview.Tags.AddRange(newTags);
            _context.Reviews.Update(oldReview);
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
    }
}
