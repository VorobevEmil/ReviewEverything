using System.Net;
using Microsoft.AspNetCore.Components;
using ReviewEverything.Shared.Contracts.Responses;
using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using ReviewEverything.Shared.Contracts.Requests;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System.Data.Common;

namespace ReviewEverything.Client.Pages
{
    public partial class Article
    {
        [Parameter] public int Id { get; set; }
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
        [Inject] private ISnackbar Snackbar { get; set; } = default!;
        private ClaimsPrincipal User { get; set; } = default!;
        private string? _userId = default!;
        private ArticleReviewResponse ArticleReview { get; set; } = default!;
        private List<CommentResponse> Comments { get; set; } = default!;
        private HubConnection _hubConnection = default!;

        private string _bodyComment = default!;
        private int _userRatingComposition = default!;
        private bool _userLike = default!;

        protected override async Task OnInitializedAsync()
        {
            await GetUserAsync();
            await GetArticleAsync();
            await GetCommentsAsync();
            _userLike = CheckUserSetLike();
            await ConfigureHubConnectionAsync();
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


        private async Task GetCommentsAsync()
        {
            var httpResponseMessage = await HttpClient.GetAsync($"api/Comment/GetByReviewId/{Id}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                Comments = (await httpResponseMessage.Content.ReadFromJsonAsync<List<CommentResponse>>())!;
            }
        }

        private bool CheckUserSetLike()
        {
            return User.Identity!.IsAuthenticated
                   && ArticleReview.LikeUsers.Contains(GetUserId());
        }

        private async Task ConfigureHubConnectionAsync()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri("/commentHub"))
                .Build();

            _hubConnection.On<CommentResponse>("ReceiveComment", (comment) =>
            {
                Comments.Insert(0, comment);
                StateHasChanged();;
            });

            await _hubConnection.StartAsync();

            await _hubConnection.SendAsync("EnterToArticle", Id);

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


        private async Task SetUserLikeAsync(bool like)
        {
            if (!User.Identity!.IsAuthenticated)
                return;

            _userLike = like;
            if (like)
            {
                ArticleReview.LikeUsers.Add(GetUserId());
                await HttpClient.PostAsJsonAsync("api/UserLike", ArticleReview.Id);
            }
            else
            {
                ArticleReview.LikeUsers.Remove(GetUserId());
                await HttpClient.DeleteAsync($"api/UserLike/{ArticleReview.Id}");
            }
        }

        private string GetUserId()
        {
            return User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        }


        private async Task SendCommentAsync()
        {
            if (User.Identity!.IsAuthenticated)
            {
                CommentRequest newComment = new()
                {
                    Body = _bodyComment,
                    ReviewId = Id
                };
                var httpResponseMessage = await HttpClient.PostAsJsonAsync("api/Comment", newComment);
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    Snackbar.Add("Не удалось отправить комментарии", Severity.Error);
                }

                _bodyComment = default!;
            }
        }
    }
}
