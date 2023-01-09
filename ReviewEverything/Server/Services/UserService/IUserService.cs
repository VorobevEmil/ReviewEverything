using ReviewEverything.Server.Models;
using System.Security.Claims;

namespace ReviewEverything.Server.Services.UserService
{
    public interface IUserService
    {
        Task<ApplicationUser?> GetUserByIdAsync(string id);
        Task<bool> EditAboutMeAsync(string userId, string aboutMe);
        Task<bool> CheckAccountAsync(ClaimsPrincipal user);
    }
}