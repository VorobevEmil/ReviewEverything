using ReviewEverything.Shared.Contracts.Responses;
using System.Net.Http.Json;

namespace ReviewEverything.Client.Pages
{
    public partial class Index
    {
        private List<ReviewResponse> Reviews { get; set; } = default!;

        private async Task GetReviewsFromApiAsync(int? categoryId = null)
        {
            Reviews = null!;

            var httpResponseMessage = await HttpClient.GetAsync($"api/Review");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                Reviews = (await httpResponseMessage.Content.ReadFromJsonAsync<List<ReviewResponse>>())!;
            }
        }
    }
}
