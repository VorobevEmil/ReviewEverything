using Microsoft.AspNetCore.Components;
using ReviewEverything.Shared.Contracts.Responses;
using System.Net.Http.Json;

namespace ReviewEverything.Client.Pages
{
    public partial class Article
    {
        [Parameter] public int Id { get; set; }
        public ArticleReviewResponse ArticleReview { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            var httpResponseMessage = await HttpClient.GetAsync($"api/Review/{Id}?typeReview=1");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                ArticleReview = (await httpResponseMessage.Content.ReadFromJsonAsync<ArticleReviewResponse>())!;
            }
        }
    }
}
