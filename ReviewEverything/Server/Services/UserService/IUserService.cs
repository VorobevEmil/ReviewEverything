using ReviewEverything.Server.Models;

namespace ReviewEverything.Server.Services.UserService
{
    public interface IUserService
    {
        Task<ApplicationUser?> GetUserByIdAsync(string id);
        Task<bool> EditAboutMeAsync(string userId, string aboutMe);
    }
}