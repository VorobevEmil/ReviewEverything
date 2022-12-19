using ReviewEverything.Server.Models;

namespace ReviewEverything.Server.Services.CommentService
{
    public interface ICommentService
    {
        Task<List<Comment>> GetCommentsByReviewIdAsync(int reviewId, int pageNumber, int pageSize, int elementSkip);
        Task<bool> CreateCommentAsync(Comment comment);
    }
}