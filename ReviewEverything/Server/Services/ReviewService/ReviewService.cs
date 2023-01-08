using System.Net;
using Microsoft.EntityFrameworkCore;
using ReviewEverything.Server.Common.Exceptions;
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

        public async Task<Review> GetReviewByIdAsync(int id)
        {
            var review = await GetIncludesReview()
                .FirstOrDefaultAsync(review => review.Id == id);

            if (review == null)
                throw new HttpStatusRequestException(HttpStatusCode.NotFound, "Обзор не найден");

            return review;
        }

        public async Task<List<Review>> GetReviewsAsync(int page, int pageSize, SortReviewByProperty sortByProperty, int? filterByAuthorScore, int? filterByCompositionId, int? categoryId, string? userId, List<int>? tags, CancellationToken token)
        {
            var reviews = GetIncludesReview()
                .Include(x => x.Comments)
                .AsQueryable();

            reviews = GetReviewsByAuthorId(reviews, userId);
            reviews = GetReviewsByCategoryId(reviews, categoryId);
            reviews = GetReviewsByTags(reviews, tags);
            reviews = FilterReviewsByAuthorScore(reviews, filterByAuthorScore);
            reviews = FilterReviewsByCompositionId(reviews, filterByCompositionId);
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

        private IQueryable<Review> FilterReviewsByCompositionId(IQueryable<Review> reviews, int? compositionId)
        {
            if (compositionId == null)
                return reviews;
            return reviews
                .Where(x => x.CompositionId == compositionId);
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
            var createdReview = await _context.SaveChangesAsync() > 0;
            review.Tags = tags;
            await _context.SaveChangesAsync();

            return createdReview;
        }

        public async Task<bool> UpdateReviewAsync(Review updateReview)
        {
            var oldReview = await GetReviewByIdAsync(updateReview.Id);
            if (oldReview == null)
                throw new HttpStatusRequestException(HttpStatusCode.NotFound, "Обзор для обновления не найден");

            UpdatePropertyOldReview(oldReview, updateReview);
            AddNewAndRemoveOldTags(oldReview, updateReview);
            AddNewAndRemoveOldImages(oldReview, updateReview);
            var updated = await _context.SaveChangesAsync();
            return updated > 0;
        }

        private void UpdatePropertyOldReview(Review oldReview, Review updateReview)
        {
            oldReview.UpdateDate = DateTime.UtcNow;
            oldReview.Title = updateReview.Title;
            oldReview.Subtitle = updateReview.Subtitle;
            oldReview.Body = updateReview.Body;
            oldReview.CompositionId = updateReview.CompositionId;
        }

        private void AddNewAndRemoveOldTags(Review oldReview, Review updateReview)
        {
            var newTags = updateReview.Tags
                .Where(x => !oldReview.Tags
                    .Select(x => x.Id)
                    .Contains(x.Id))
                .ToList();

            oldReview.Tags.RemoveAll(x => !updateReview.Tags.Select(x => x.Id).Contains(x.Id));
            oldReview.Tags.AddRange(newTags);
        }

        private void AddNewAndRemoveOldImages(Review oldReview, Review updateReview)
        {
            var newImages = updateReview.CloudImages.Where(x => !oldReview.CloudImages.Select(x => x.Id).Contains(x.Id)).ToList();
            oldReview.CloudImages.RemoveAll(x => !updateReview.CloudImages.Select(x => x.Id).Contains(x.Id));
            oldReview.CloudImages.AddRange(newImages);

        }

        public async Task<bool> DeleteReviewAsync(int id)
        {
            var review = await _context.Reviews.FirstOrDefaultAsync(x => x.Id == id);
            if (review == null)
                throw new HttpStatusRequestException(HttpStatusCode.NotFound, "Обзор для удаления не найден");

            _context.Reviews.Remove(review);
            var deleted = await _context.SaveChangesAsync();

            return deleted > 0;
        }

        public async Task<List<Review>> GetSimilarArticleAsync(int reviewId)
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
                throw new HttpStatusRequestException(HttpStatusCode.NotFound, "Обзор по которому нужно искать другие похожие обзоры не найден");
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
