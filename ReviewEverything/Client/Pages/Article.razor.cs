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
        private string? _userId = default!;
        private ArticleReviewResponse ArticleReview { get; set; } = default!;

        private int _userRatingComposition = default!;

        protected override async Task OnInitializedAsync()
        {
            await GetUserAsync();
            await GetArticleAsync();
        }

        private async Task GetUserAsync()
        {
            User = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
            _userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? default;
        }

        private async Task GetArticleAsync()
        {
            var httpResponseMessage = await HttpClient.GetAsync($"api/Review/{Id}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                ArticleReview = (await httpResponseMessage.Content.ReadFromJsonAsync<ArticleReviewResponse>())!;
                GetUserRating();
            }
        }

        private void GetUserRating()
        {
            _userRatingComposition = ArticleReview.UserScores.FirstOrDefault(x => x.UserId == _userId)?.Score ?? 0;
        }



        private async Task SetUserRatingAsync(int rating)
        {
            _userRatingComposition = rating;
            var userScore = ArticleReview.UserScores.FirstOrDefault(x => x.UserId == _userId);

            if (rating != 0)
            {
                await CreateOrUpdateUserRatingAsync(userScore);
            }
            else
            {
                await DeleteUserRatingAsync(userScore);
            }
        }

        private async Task CreateOrUpdateUserRatingAsync(UserScoreResponse? userScore)
        {
            var request = new UserScoreRequest()
            {
                CompositionId = ArticleReview.CompositionId,
                Score = _userRatingComposition
            };
            var httpResponseMessage = await HttpClient.PostAsJsonAsync("api/UserScore", request);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                if (userScore == null)
                {
                    userScore ??= new UserScoreResponse() { Score = _userRatingComposition, UserId = _userId! };
                    ArticleReview.UserScores.Add(userScore);
                }
                userScore.Score = _userRatingComposition;
            }
            else _userRatingComposition = 0;
        }

        private async Task DeleteUserRatingAsync(UserScoreResponse? userScore)
        {
            var httpMessageResponse = await HttpClient.DeleteAsync($"api/UserScore/{ArticleReview.CompositionId}");
            if (httpMessageResponse.StatusCode == HttpStatusCode.NoContent)
            {
                if (userScore != null) ArticleReview.UserScores.Remove(userScore);
            }
        }
    }
}
