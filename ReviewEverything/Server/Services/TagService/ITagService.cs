using ReviewEverything.Server.Models;

namespace ReviewEverything.Server.Services.TagService
{
    public interface ITagService
    {
        Task<Tag?> GetTagByIdAsync(int id);
        Task<List<Tag>> GetTagsAsync(int page, int pageSize, string? search);
        Task<bool> CreateTagAsync(Tag tag);
        Task<bool> ExistTagByNameAsync(string title);
    }
}
