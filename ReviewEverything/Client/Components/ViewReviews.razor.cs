using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Client.Components
{
    public partial class ViewReviews
    {
        [Parameter] public bool Editor { get; set; }
        [Parameter] public List<ReviewResponse> Reviews { get; set; } = default!;
        [Parameter] public EventCallback<int?> GetReviewsFromApiAsync { get; set; }
        private List<CategoryResponse> Categories { get; set; } = default!;

        private string _titleCategory = default!;


        protected override async Task OnInitializedAsync()
        {
            Categories = (await HttpClient.GetFromJsonAsync<List<CategoryResponse>>("api/Category"))!;
            await SelectedCategoryAsync();
        }

        private async Task SelectedCategoryAsync(int? categoryId = null)
        {
            _titleCategory = categoryId == null ? "Все Обзоры" : $"Обзоры на {Categories.First(x => x.Id == categoryId.Value).Title}";

            await GetReviewsFromApiAsync.InvokeAsync(categoryId);
        }

        private void NavigateToReviewEditor(int? reviewId = null)
        {
            NavigationManager.NavigateTo($"review-editor/{reviewId}");
        }
    }
}