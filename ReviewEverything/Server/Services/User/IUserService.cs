using ReviewEverything.Server.Models;

namespace ReviewEverything.Server.Services
{
    public interface IUserService
    {
        Task<ApplicationUser?> GetUserByIdAsync(string id);
        Task<List<ApplicationUser>> GetUsersAsync();
    }
}