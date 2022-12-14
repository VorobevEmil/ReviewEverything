using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Client.Components
{
    public partial class ViewReviews
    {
        [Inject] private DisplayHelper DisplayHelper { get; set; } = default!;
        [Parameter] public bool Editor { get; set; }
        [Parameter] public string? UserId { get; set; }

        public List<ReviewResponse> Reviews { get; set; } = default!;
        private List<CategoryResponse> Categories { get; set; } = default!;
        private List<TagResponse> Tags { get; set; } = default!;
        private List<TagResponse> SelectedTags { get; set; } = new();

        private string _titleCategory = default!;
        private int? _categoryId = default!;
        private string _tagSearch = default!;

        protected override async Task OnInitializedAsync()
        {
            await GetCategoriesFromApi();
            await GetTagsFromApiAsync();
            await SelectedCategoryAsync();
        }

        private async Task GetCategoriesFromApi()
        {
            Categories = (await HttpClient.GetFromJsonAsync<List<CategoryResponse>>("api/Category"))!;
        }

        private async Task GetTagsFromApiAsync(string search = null!)
        {
            _tagSearch = search;
            Tags = null!;
            Tags = (await HttpClient.GetFromJsonAsync<List<TagResponse>>($"api/Tag?{(!string.IsNullOrWhiteSpace(search) ? $"search={search}" : null)}"))!;
        }

        private async Task SelectedCategoryAsync(int? categoryId = null)
        {
            _titleCategory = categoryId == null ? "Все Обзоры" : $"Обзоры на {Categories.First(x => x.Id == categoryId.Value).Title}";
            _categoryId = categoryId;
            await GetReviewsFromApiAsync();
        }

        private async Task AddTagAsync(TagResponse tag)
        {
            SelectedTags.Add(tag);
            await GetReviewsFromApiAsync();
        }

        private async Task RemoveTagAsync(TagResponse tag)
        {
            SelectedTags.Remove(tag);
            await GetReviewsFromApiAsync();
        }

        private async Task  GetReviewsFromApiAsync()
        {
            Reviews = null!;
            var idTags = SelectedTags.Select(x => x.Id).ToList();
            string tags = "idTags=";
            for (int i = 0; i < idTags.Count; i++)
            {
                tags += $"{idTags[i]}.";
            }
            tags = tags.Remove(tags.Length - 1);

            var httpResponseMessage = await HttpClient.GetAsync($"api/Review?{(_categoryId != null ? $"categoryId={_categoryId}&" : null)}{(UserId != null ? $"userId={UserId}&" : null)}{tags}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                Reviews = (await httpResponseMessage.Content.ReadFromJsonAsync<List<ReviewResponse>>())!;
            }
        }

        private void NavigateToReviewEditor(int? reviewId = null)
        {
            NavigationManager.NavigateTo($"review-editor/{reviewId}");
        }

        private async Task DeleteReviewAsync(int reviewId)
        {
            if (await DisplayHelper.ShowDeleteMessageBoxAsync() != true)
                return;

            await HttpClient.DeleteAsync($"api/Review/{reviewId}");
            Reviews.RemoveAll(x => x.Id == reviewId);
        }
    }
}