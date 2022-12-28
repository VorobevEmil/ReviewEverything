using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Client.Components.Views
{
    public partial class SimilarArticleView
    {
        [Parameter] public int ReviewId { get; set; }
        private List<SimilarArticleReviewResponse>? SimilarArticles { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            SimilarArticles = await HttpClient.GetFromJsonAsync<List<SimilarArticleReviewResponse>>($"api/Review/GetSimilarArticles/{ReviewId}");
        }

        public void NavigateArticle(int id)
        {
            NavigationManager.NavigateTo($"./redirect-page");
            NavigationManager.NavigateTo($"./article/{id}");
        }
    }
}
