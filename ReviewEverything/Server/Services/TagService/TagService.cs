using Microsoft.EntityFrameworkCore;
using ReviewEverything.Client.Pages.Admin;
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

        public async Task<Tag?> GetTagByIdAsync(int id)
        {
            return await _context.Tags.FirstOrDefaultAsync(tag => tag.Id == id);
        }

        public async Task<List<Tag>> GetTagsAsync(string? search)
        {
            var tags = _context.Tags.AsQueryable();

            if (search != null)
            {
                tags = tags
                    .Where(p => EF.Functions.Like(p.Title.ToLower(), $"%{search.ToLower()}%"));
            }

            return await tags.ToListAsync();
        }

        public async Task<bool> CreateTagAsync(Tag tag)
        {
            await _context.Tags.AddAsync(tag);
            var created = await _context.SaveChangesAsync();

            return created > 0;
        }

        public async Task<bool> UpdateTagAsync(Tag tag)
        {
            _context.Tags.Update(tag);
            var updated = await _context.SaveChangesAsync();
            return updated > 0;
        }

        public async Task<bool> DeleteTagAsync(int id)
        {
            var tag = await GetTagByIdAsync(id);
            if (tag == null)
                return false;

            _context.Tags.Remove(tag);
            var deleted = await _context.SaveChangesAsync();

            return deleted > 0;
        }
    }
}
