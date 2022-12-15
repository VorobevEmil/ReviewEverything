namespace ReviewEverything.Server.Services.UserLikeService
{
    public interface IUserLikeService
    {
        Task<bool> AddLikeToUserAsync(int reviewId);
        Task<bool> RemoveLikeFromUserAsync(int reviewId);
    }
}