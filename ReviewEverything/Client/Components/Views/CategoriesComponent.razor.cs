using ReviewEverything.Shared.Contracts.Responses;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;

namespace ReviewEverything.Client.Components.Views
{
    public partial class CategoriesComponent
    {
        [Parameter] public EventCallback<int?> GetReviewsFromCategoryId { get; set; }
        private List<CategoryResponse> Categories { get; set; } = default!;
        private string _titleCategory = default!;
        protected override async Task OnInitializedAsync()
        {
            await GetCategoriesFromApi();
            StateHasChanged();
            await SelectedCategoryAsync();
        }


        private async Task GetCategoriesFromApi()
        {
            Categories = (await HttpClient.GetFromJsonAsync<List<CategoryResponse>>("api/Category"))!;
        }

        private async Task SelectedCategoryAsync(int? categoryId = null)
        {
            _titleCategory = categoryId == null ? "Все Обзоры" : $"Обзоры на {Categories.First(x => x.Id == categoryId.Value).Title}";
            
            await GetReviewsFromCategoryId.InvokeAsync(categoryId);
        }
    }
}