using Microsoft.EntityFrameworkCore;
using ReviewEverything.Server.Data;
using ReviewEverything.Server.Models;

namespace ReviewEverything.Server.Services.CompositionService
{
    public class CompositionService : ICompositionService
    {
        private readonly AppDbContext _context;

        public CompositionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Composition>> GetCompositionsAsync(string? search)
        {
            return await _context.Compositions
                .Where(p => search != null && EF.Functions.Like(p.Title.ToLower(), $"%{search.ToLower()}%"))
                .ToListAsync();
        }

        public async Task<Composition?> GetCompositionByIdAsync(int id)
        {
            return await _context.Compositions.FirstOrDefaultAsync(composition => composition.Id == id);
        }

        public async Task<bool> CreateCompositionAsync(Composition composition)
        {
            await _context.Compositions.AddAsync(composition);
            var created = await _context.SaveChangesAsync();

            return created > 0;
        }

        public async Task<bool> UpdateCompositionAsync(Composition composition)
        {
            _context.Compositions.Update(composition);
            var updated = await _context.SaveChangesAsync();
            return updated > 0;
        }

        public async Task<bool> DeleteCompositionAsync(int id)
        {
            var composition = await GetCompositionByIdAsync(id);
            if (composition == null)
                return false;

            _context.Compositions.Remove(composition);
            var deleted = await _context.SaveChangesAsync();

            return deleted > 0;
        }
    }
}
