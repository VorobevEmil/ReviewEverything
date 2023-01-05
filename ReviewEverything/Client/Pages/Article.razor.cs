using System.Net;
using Microsoft.AspNetCore.Components;
using ReviewEverything.Shared.Contracts.Responses;
using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using ReviewEverything.Shared.Contracts.Requests;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using MudBlazor;
using ReviewEverything.Client.Components.Views;
using ReviewEverything.Client.Resources;
using ReviewEverything.Client.Helpers;
using ReviewEverything.Shared.Models;

namespace ReviewEverything.Client.Pages
{
    public partial class Article
    {
        [Parameter] public int Id { get; set; }
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] private ISnackbar Snackbar { get; set; } = default!;
        [Inject] private DisplayHelper DisplayHelper { get; set; } = default!;
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
        [Inject] private IStringLocalizer<Article> Localizer { get; set; } = default!;
        private IStringLocalizer<ResourcesShared> SharedLocalizer { get; set; } = ResourcesShared.CreateStringLocalizer();
        private SimilarArticleView? _similarArticle;
        private ClaimsPrincipal User { get; set; } = default!;
        private string? _userId = default!;
        private ArticleReviewResponse ArticleReview { get; set; } = default!;

        private int _userRatingComposition = default!;
        private bool _convertedToPdf = false;

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
            else
            {
                await DisplayHelper.ShowErrorResponseMessage();
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

        private async Task ConvertToPDFAsync()
        {
            _convertedToPdf = true;
            var httpMessageResponse = await HttpClient.GetAsync($"api/PdfConverter/{Id}");
            if (httpMessageResponse.IsSuccessStatusCode)
            {
                var fileData = (await httpMessageResponse.Content.ReadFromJsonAsync<FileData>())!;
                await JSRuntime.InvokeVoidAsync("download", fileData.Data, fileData.FileName, fileData.ContentType);
            }
            else
            {
                Snackbar.Add(Localizer["Не удалось преобразовать в PDF"], Severity.Error);
            }
            _convertedToPdf = false;
        }
    }
}
