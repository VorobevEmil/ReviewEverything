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

        public async Task<List<Composition>> GetCompositionsAsync(string? search, int? categoryId, string? userId)
        {
            var compositions = _context.Compositions
                .Include(x => x.Reviews)
                .AsQueryable();
            compositions = FilterCompositionsBySearch(compositions, search);
            compositions = FilterCompositionsByCategoryId(compositions, categoryId);
            compositions = FilterCompositionsByUserId(compositions, userId);

            return await compositions.ToListAsync();
        }

        private IQueryable<Composition> FilterCompositionsBySearch(IQueryable<Composition> compositions, string? search)
        {
            if (!string.IsNullOrWhiteSpace(search))
                compositions = compositions
                    .Where(x => EF.Functions.Like(x.Title.ToLower(), $"%{search.ToLower()}%"));

            return compositions;
        }

        private IQueryable<Composition> FilterCompositionsByCategoryId(IQueryable<Composition> compositions, int? categoryId)
        {
            if (categoryId is not null)
                compositions = compositions
                    .Where(x => x.CategoryId == categoryId);

            return compositions;
        }

        private IQueryable<Composition> FilterCompositionsByUserId(IQueryable<Composition> compositions, string? userId)
        {
            if (!string.IsNullOrWhiteSpace(userId))
                compositions = compositions
                    .Where(x => x.Reviews.Any(x => x.AuthorId == userId));

            return compositions;
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
    }
}
