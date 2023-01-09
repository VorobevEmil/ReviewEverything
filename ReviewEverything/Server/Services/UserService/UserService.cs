using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReviewEverything.Server.Common.Exceptions;
using ReviewEverything.Server.Data;
using ReviewEverything.Server.Models;

namespace ReviewEverything.Server.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string id)
        {
            var user = await _context.Users
                .Include(x => x.LikeReviews)
                .FirstOrDefaultAsync(user => user.Id == id);

            if (user == null)
                throw new HttpStatusRequestException(HttpStatusCode.NotFound);

            return user;
        }

        public async Task<bool> EditAboutMeAsync(string userId, string aboutMe)
        {
            var user = await _context.Users.FirstAsync(x => x.Id == userId);
            user.AboutMe = aboutMe;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> CheckAccountAsync(ClaimsPrincipal userPrincipal)
        {
            var userId = userPrincipal.Claims.First(t => ClaimTypes.NameIdentifier == t.Type).Value;
           
            var user = await _userManager.FindByIdAsync(userId);

            bool addedNewRoleInUser = false;
            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var userRolesCookie = userPrincipal.Claims
                    .Where(x => x.Type == ClaimTypes.Role)
                    .Select(x => x.Value)
                    .ToArray();

                addedNewRoleInUser = userRoles.Except(userRolesCookie).Any() || userRolesCookie.Except(userRoles).Any();
            }

            return user is { Block: true } or null || addedNewRoleInUser;
        }
    }
}