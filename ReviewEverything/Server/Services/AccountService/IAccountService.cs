using System.Security.Claims;
using ReviewEverything.Shared.Models.Account;

namespace ReviewEverything.Server.Services.AccountService
{
    public interface IAccountService
    {
        Task LogoutAsync();
        Task SignInAsync(SignInModel model);
        Task SignUpAsync(SignUpModel model);
        UserInfo GetCurrentUserData(ClaimsPrincipal user);
    }
}