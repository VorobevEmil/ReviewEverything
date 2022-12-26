using Microsoft.EntityFrameworkCore;
using ReviewEverything.Server.Data;
using ReviewEverything.Server.Models;

namespace ReviewEverything.Server.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _context.Categories
                .OrderBy(x => x.Title)
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(category => category.Id == id);
        }

        public async Task<bool> CreateCategoryAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            var created = await _context.SaveChangesAsync();

            return created > 0;
        }

        public async Task<bool> UpdateCategoryAsync(Category category)
        {
            _context.Categories.Update(category);
            var updated = await _context.SaveChangesAsync();
            return updated > 0;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await GetCategoryByIdAsync(id);
            if (category == null)
                return false;

            _context.Categories.Remove(category);
            var deleted = await _context.SaveChangesAsync();

            return deleted > 0;
        }
    }
}
