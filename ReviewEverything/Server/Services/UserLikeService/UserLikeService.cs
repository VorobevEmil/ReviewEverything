using System.Net;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ReviewEverything.Server.Common.Exceptions;
using ReviewEverything.Server.Data;

namespace ReviewEverything.Server.Services.UserLikeService
{
    public class UserLikeService : IUserLikeService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public UserLikeService(AppDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        public async Task<bool> AddLikeToUserAsync(int reviewId)
        {
            var review = await _context.Reviews
                .Include(x => x.LikeUsers)
                .FirstOrDefaultAsync(x => x.Id == reviewId);
            if (review == null)
                throw new HttpStatusRequestException(HttpStatusCode.NotFound);

            if (review.LikeUsers.Any(x => x.Id == GetUserId()))
                return true;

            review.LikeUsers.Add((await _context.Users.FindAsync(GetUserId()))!);

            return await _context.SaveChangesAsync() > 0;

        }

        public async Task<bool> RemoveLikeFromUserAsync(int reviewId)
        {
            var review = await _context.Reviews
                .Include(x => x.LikeUsers)
                .FirstOrDefaultAsync(x => x.Id == reviewId);
            if (review == null)
                throw new HttpStatusRequestException(HttpStatusCode.NotFound);

            if (review.LikeUsers.All(x => x.Id != GetUserId()))
                return true;

            review.LikeUsers.Remove((await _context.Users.FindAsync(GetUserId()))!);
            return await _context.SaveChangesAsync() > 0;
        }

        private string GetUserId()
        {
            return _contextAccessor.HttpContext!.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        }
    }
}