using Microsoft.EntityFrameworkCore;
using ReviewEverything.Server.Data;
using ReviewEverything.Server.Models;

namespace ReviewEverything.Server.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string id)
        {
            return await _context.Users
                .Include(x => x.LikeReviews)
                .FirstOrDefaultAsync(user => user.Id == id);
        }

        public async Task<bool> EditAboutMeAsync(string userId, string aboutMe)
        {
            var user = await _context.Users.FirstAsync(x => x.Id == userId);
            user.AboutMe = aboutMe;
            return await _context.SaveChangesAsync() > 0;
        }
    }
}