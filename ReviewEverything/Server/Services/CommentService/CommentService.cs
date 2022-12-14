using Microsoft.EntityFrameworkCore;
using ReviewEverything.Server.Data;
using ReviewEverything.Server.Models;

namespace ReviewEverything.Server.Services.CommentService
{
    public class CommentService : ICommentService
    {
        private readonly AppDbContext _context;

        public CommentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Comment?> GetCommentByIdAsync(int id)
        {
            return await _context.Comments
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public  async Task<List<Comment>> GetCommentsAsync()
        {
            return await _context.Comments.ToListAsync();
        }

        public async Task<bool> CreateCommentAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            var created = await _context.SaveChangesAsync();
            comment.User = (await GetCommentByIdAsync(comment.Id))!.User;
            return created > 0;
        }

        public async Task<bool> UpdateCommentAsync(Comment comment)
        {
            _context.Comments.Update(comment);
            var updated = await _context.SaveChangesAsync();
            return updated > 0;
        }

        public async Task<bool> DeleteCommentAsync(int id)
        {
            var comment = await GetCommentByIdAsync(id);
            if (comment == null)
                return false;

            _context.Comments.Remove(comment);
            var deleted = await _context.SaveChangesAsync();

            return deleted > 0;
        }
    }
}