﻿using Microsoft.EntityFrameworkCore;
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

        private async Task<Comment?> GetCommentByIdAsync(int id)
        {
            return await _context.Comments
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Comment>> GetCommentsByReviewIdAsync(int reviewId, int pageNumber, int pageSize, int elementSkip)
        {
            return await _context.Comments
                .Include(x => x.User)
                .Where(x => x.ReviewId == reviewId)
                .OrderByDescending(x => x.CreationDate)
                .Skip((pageNumber - 1) * pageSize).Take(pageSize)
                .Skip(elementSkip)
                .ToListAsync();
        }

        public async Task<bool> CreateCommentAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            var created = await _context.SaveChangesAsync();
            comment.User = (await GetCommentByIdAsync(comment.Id))!.User;
            return created > 0;
        }
    }
}