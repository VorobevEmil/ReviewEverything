using ReviewEverything.Shared.Contracts.Responses;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;

namespace ReviewEverything.Client.Components.Views
{
    public partial class CategoriesComponent
    {

        [Parameter] public EventCallback GetReviewsFromApi { get; set; }
        private List<CategoryResponse> Categories { get; set; } = default!;
        private int? _categoryId;
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
            _categoryId = categoryId;

            await GetReviewsFromApi.InvokeAsync();
        }

        public string? GetCategoryParameterUrl() => _categoryId != null ? $"categoryId={_categoryId}&" : null;
    }
}