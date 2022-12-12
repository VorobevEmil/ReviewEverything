using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using ReviewEverything.Server.Data;
using ReviewEverything.Server.Models;

namespace ReviewEverything.Server.Services.ReviewService
{
    public class ReviewService : IReviewService
    {
        private readonly AppDbContext _context;

        public ReviewService(AppDbContext context)
        {
            _context = context;
        }

        private IIncludableQueryable<Review, Category> GetReviewIncludeAll()
        {
            return _context.Reviews
                .Include(x => x.Author)
                .Include(x => x.CloudImages)
                .Include(x => x.Tags)
                .Include(x => x.Comments)
                .Include(x => x.LikeUsers)
                .Include(x => x.Composition)
                .ThenInclude(x => x.Category);
        }

        public async Task<Review?> GetReviewByIdAsync(int id)
        {
            return await GetReviewIncludeAll()
                .FirstOrDefaultAsync(review => review.Id == id);
        }

        public async Task<List<Review>> GetReviewsAsync()
        {
            return await GetReviewIncludeAll()
                .ToListAsync();

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
