using ReviewEverything.Server.Models;

namespace ReviewEverything.Server.Services.CompositionService
{
    public interface ICompositionService
    {
        Task<Composition?> GetCompositionByIdAsync(int id);
        Task<List<Composition>> GetCompositionsAsync(string? search);
        Task<bool> CreateCompositionAsync(Composition category);
        Task<bool> UpdateCompositionAsync(Composition category);
        Task<bool> DeleteCompositionAsync(int id);

    }
}