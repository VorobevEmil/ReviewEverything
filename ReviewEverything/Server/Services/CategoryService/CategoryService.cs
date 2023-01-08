using System.Net;
using Microsoft.EntityFrameworkCore;
using ReviewEverything.Server.Common.Exceptions;
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

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(category => category.Id == id);
            if (category == null)
                throw new HttpStatusRequestException(HttpStatusCode.NotFound, "Категория не найдена");

            return category;
        }

        public async Task<bool> CreateCategoryAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            var created = await _context.SaveChangesAsync();

            return created > 0;
        }

        public async Task<bool> UpdateCategoryAsync(Category category)
        {
            var categoryOld = await _context.Categories.FirstOrDefaultAsync(x => x.Id == category.Id);
            if (categoryOld == null)
                throw new HttpStatusRequestException(HttpStatusCode.NotFound, "Категория для обновления не найдена");
            categoryOld.Title = category.Title;
            var updated = await _context.SaveChangesAsync();
            return updated > 0;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
                throw new HttpStatusRequestException(HttpStatusCode.NotFound, "Категория для удаления не найдена");

            _context.Categories.Remove(category);
            var deleted = await _context.SaveChangesAsync();

            return deleted > 0;
        }
    }
}
