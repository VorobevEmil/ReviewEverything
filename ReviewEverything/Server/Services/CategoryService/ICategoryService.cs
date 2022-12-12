using ReviewEverything.Server.Models;

namespace ReviewEverything.Server.Services.CategoryService
{
    public interface ICategoryService
    {
        Task<Category?> GetCategoryByIdAsync(int id);
        Task<List<Category>> GetCategoriesAsync();
        Task<bool> CreateCategoryAsync(Category category);
        Task<bool> UpdateCategoryAsync(Category category);
        Task<bool> DeleteCategoryAsync(int id);

    }
}
