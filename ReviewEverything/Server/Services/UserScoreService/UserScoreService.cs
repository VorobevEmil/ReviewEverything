using System.Net;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ReviewEverything.Server.Common.Exceptions;
using ReviewEverything.Server.Data;
using ReviewEverything.Server.Models;

namespace ReviewEverything.Server.Services.UserScoreService
{
    public class UserScoreService : IUserScoreService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public UserScoreService(AppDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        public async Task CreateOrUpdateScopeAsync(UserScore request)
        {
            var userId = _contextAccessor.HttpContext!.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var scope = await _context.UserScores.FirstOrDefaultAsync(x => x.CompositionId == request.CompositionId && x.UserId == userId);
            if (scope == null)
            {
                request.UserId = userId;
                await _context.UserScores.AddAsync(request);
            }
            else
            {
                scope.Score = request.Score;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteScopeAsync(int compositionId)
        {
            var userId = _contextAccessor.HttpContext!.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var scope = await _context.UserScores.FirstOrDefaultAsync(x => x.CompositionId == compositionId && x.UserId == userId);
            if (scope == null)
                throw new HttpStatusRequestException(HttpStatusCode.NotFound, "Произведение не найдено");
            _context.UserScores.Remove(scope);

            return await _context.SaveChangesAsync() > 0;
        }
    }
}