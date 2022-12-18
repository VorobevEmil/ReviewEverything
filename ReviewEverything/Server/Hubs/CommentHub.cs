using Microsoft.AspNetCore.SignalR;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Server.Hubs
{
    public class CommentHub : Hub
    {
        public async Task EnterToArticle(int reviewId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, reviewId.ToString());
        }
    }
}