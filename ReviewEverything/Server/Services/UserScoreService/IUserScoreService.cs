using ReviewEverything.Server.Models;

namespace ReviewEverything.Server.Services.UserScoreService
{
    public interface IUserScoreService
    {
        Task CreateOrUpdateScopeAsync(UserScore request);
        Task<bool> DeleteScopeAsync(int compositionId);
    }
}