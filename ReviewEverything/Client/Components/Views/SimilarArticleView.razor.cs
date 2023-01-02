using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using ReviewEverything.Client.Pages;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Client.Components.Views
{
    public partial class SimilarArticleView
    {
        [Parameter] public int ReviewId { get; set; }
        [Inject] private IStringLocalizer<Article> Localizer { get; set; } = default!;

        public List<SimilarArticleReviewResponse>? SimilarArticles { get; private set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            var httpResponseMessage = await HttpClient.GetAsync($"api/Review/GetSimilarArticles/{ReviewId}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                SimilarArticles = await httpResponseMessage.Content.ReadFromJsonAsync<List<SimilarArticleReviewResponse>>();
            }
        }

        public void NavigateArticle(int id)
        {
            NavigationManager.NavigateTo($"./redirect-page");
            NavigationManager.NavigateTo($"./article/{id}");
        }
    }
}
