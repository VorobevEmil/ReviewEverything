using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ReviewEverything.Server.Common.Exceptions;
using ReviewEverything.Server.Data;
using ReviewEverything.Server.Hubs;
using ReviewEverything.Server.Models;
using ReviewEverything.Shared.Models.Enums;

namespace ReviewEverything.Server.Services.UserManagementService
{
    public class UserManagementService : IUserManagementService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<UserManagerHub> _hubContext;

        public UserManagementService(AppDbContext context, UserManager<ApplicationUser> userManager, IHubContext<UserManagerHub> hubContext)
        {
            _context = context;
            _userManager = userManager;
            _hubContext = hubContext;
        }
        public async Task<(int count, List<ApplicationUser> users)> GetUsersAsync(int page, int pageSize, FilterUserByProperty filterUserByProperty, string? search, CancellationToken token)
        {
            var users = _context.Users.AsQueryable();
            if (search != null)
                users = filterUserByProperty switch
                {
                    FilterUserByProperty.FullName => users.Where(x => EF.Functions.Like(x.FullName.ToLower(), $"%{search.ToLower()}%")),
                    FilterUserByProperty.UserName => users.Where(x => EF.Functions.Like(x.UserName!.ToLower(), $"%{search.ToLower()}%")),
                    _ => users
                };

            var userCount = await users.CountAsync(cancellationToken: token);
            var usersResult = await users
                .OrderBy(x => x.FullName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken: token);

            return (userCount, usersResult);
        }

        public async Task<bool> CheckUserContainsAdminRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new HttpStatusRequestException(HttpStatusCode.NotFound);

            return await _userManager.IsInRoleAsync(user, "Admin");
        }

        public async Task<bool> DeleteUserAsync(string userId, CancellationToken token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken: token);
            if (user == null)
                throw new HttpStatusRequestException(HttpStatusCode.NotFound);

            _context.Users.Remove(user);

            await _hubContext.Clients.User(userId).SendAsync("LogoutAccount", "Ваш аккаунт удален!", cancellationToken: token);
            return await _context.SaveChangesAsync(cancellationToken: token) > 0;
        }

        public async Task<bool> RefreshStatusBlockAsync(string userId, bool statusBlock, CancellationToken token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken: token);

            if (user == null)
                throw new HttpStatusRequestException(HttpStatusCode.NotFound);

            user.Block = statusBlock;

            if (statusBlock)
                await _hubContext.Clients.User(userId).SendAsync("LogoutAccount", "Ваш аккаунт заблокирован!", cancellationToken: token);
            return await _context.SaveChangesAsync(cancellationToken: token) > 0;
        }

        public async Task ChangeUserRoleAsync(string userId, bool statusRole, CancellationToken token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new HttpStatusRequestException(HttpStatusCode.NotFound);

            var isRoleInUser = await _userManager.IsInRoleAsync(user, "Admin");
            if ((statusRole && isRoleInUser) || (!statusRole && !isRoleInUser))
                return;

            if (statusRole)
                await _userManager.AddToRoleAsync(user, "Admin");
            else
                await _userManager.RemoveFromRoleAsync(user, "Admin");

            var message = statusRole ? "Вам выданы права администратора!" : "У вас отняты права администратора!";
            await _hubContext.Clients.User(userId).SendAsync("LogoutAccount", message, cancellationToken: token);
        }
    }
}