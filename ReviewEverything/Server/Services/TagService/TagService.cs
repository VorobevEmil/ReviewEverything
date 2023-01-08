using System.Net;
using Microsoft.EntityFrameworkCore;
using ReviewEverything.Server.Common.Exceptions;
using ReviewEverything.Server.Data;
using ReviewEverything.Server.Models;

namespace ReviewEverything.Server.Services.TagService
{
    public class TagService : ITagService
    {
        private readonly AppDbContext _context;

        public TagService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Tag> GetTagByIdAsync(int id)
        {
            var tag = await _context.Tags.FirstOrDefaultAsync(tag => tag.Id == id);
            if (tag == null)
                throw new HttpStatusRequestException(HttpStatusCode.NotFound, "Тег не найден");

            return tag;
        }

        public async Task<List<Tag>> GetTagsAsync(int page, int pageSize, string? search)
        {
            var tags = _context.Tags.AsQueryable();

            if (search != null)
            {
                tags = tags
                    .Where(p => EF.Functions.Like(p.Title.ToLower(), $"%{search.ToLower()}%"));
            }

            return await tags
                .OrderBy(x => x.Title)
                .ThenBy(x => x.Title.Length)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<bool> CreateTagAsync(Tag tag)
        {
            var existsTag = await _context.Tags
                .FirstOrDefaultAsync(x => x.Title.ToLower() == tag.Title.ToLower());
            if (existsTag != null)
                throw new HttpStatusRequestException(HttpStatusCode.Conflict);

            await _context.Tags.AddAsync(tag);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ExistTagByNameAsync(string title)
        {
            var existsTag = await _context.Tags
                .FirstOrDefaultAsync(x => x.Title.ToLower() == title.ToLower());
            if (existsTag == null)
                return false;

            return true;
        }
    }
}
