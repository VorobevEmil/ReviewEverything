using ReviewEverything.Server.Models;

namespace ReviewEverything.Server.Services.UserService
{
    public interface IUserService
    {
        Task<ApplicationUser?> GetUserByIdAsync(string id);
    }
}