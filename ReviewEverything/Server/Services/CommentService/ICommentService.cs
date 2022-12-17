using ReviewEverything.Server.Models;

namespace ReviewEverything.Server.Services.CommentService
{
    public interface ICommentService
    {
        Task<Comment?> GetCommentByIdAsync(int id);
        Task<List<Comment>> GetCommentsByReviewIdAsync(int reviewId);
        Task<bool> CreateCommentAsync(Comment comment);
        Task<bool> UpdateCommentAsync(Comment comment);
        Task<bool> DeleteCommentAsync(int id);
    }
}