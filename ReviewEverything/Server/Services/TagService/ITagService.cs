using ReviewEverything.Server.Models;

namespace ReviewEverything.Server.Services.TagService
{
    public interface ITagService
    {
        Task<Tag?> GetTagByIdAsync(int id);
        Task<List<Tag>> GetTagsAsync(string? search);
        Task<bool> CreateTagAsync(Tag tag);
        Task<bool> UpdateTagAsync(Tag tag);
        Task<bool> DeleteTagAsync(int id);
    }
}
