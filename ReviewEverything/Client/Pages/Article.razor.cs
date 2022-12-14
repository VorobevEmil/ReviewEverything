using System.Net;
using Microsoft.AspNetCore.Components;
using ReviewEverything.Shared.Contracts.Responses;
using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using ReviewEverything.Shared.Contracts.Requests;

namespace ReviewEverything.Client.Pages
{
    public partial class Article
    {
        [Parameter] public int Id { get; set; }
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
        private ClaimsPrincipal User { get; set; } = default!;
        public ArticleReviewResponse ArticleReview { get; set; } = default!;
        private string _bodyComment = default!;
        protected override async Task OnInitializedAsync()
        {
            User = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;

            var httpResponseMessage = await HttpClient.GetAsync($"api/Review/{Id}?typeReview=1");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                ArticleReview = (await httpResponseMessage.Content.ReadFromJsonAsync<ArticleReviewResponse>())!;
            }
        }

        private async Task SendCommentAsync()
        {
            if (User.Identity!.IsAuthenticated)
            {
                CommentRequest newComment = new CommentRequest()
                {
                    Body = _bodyComment,
                    ReviewId = Id
                };
                var httpResponseMessage = await HttpClient.PostAsJsonAsync("api/Comment", newComment);
                if (httpResponseMessage.StatusCode == HttpStatusCode.Created)
                {
                    ArticleReview.Comments.Add((await httpResponseMessage.Content.ReadFromJsonAsync<CommentResponse>())!);
                }

                _bodyComment = default!;
            }
        }
    }
}
