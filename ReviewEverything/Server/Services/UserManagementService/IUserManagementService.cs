using ReviewEverything.Server.Models;
using ReviewEverything.Shared.Models.Enums;

namespace ReviewEverything.Server.Services.UserManagementService
{
    public interface IUserManagementService
    {
        Task<(int count, List<ApplicationUser> users)> GetUsersAsync(int page, int pageSize, FilterUserByProperty filterUserByProperty, string? search, CancellationToken token);
        Task<bool> CheckUserContainsAdminRole(string userId);
        Task<bool> DeleteUserAsync(string userId, CancellationToken token);
        Task<bool> RefreshStatusBlockAsync(string userId, bool statusBlock, CancellationToken token);
        Task ChangeUserRoleAsync(string userId, bool statusRole, CancellationToken token);
    }
}