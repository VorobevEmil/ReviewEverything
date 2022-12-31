using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Client.Components.ReviewsView
{
    public partial class TagsComponent
    {
        [Parameter] public EventCallback GetReviewsFromApi { get; set; }
        [Inject] private IStringLocalizer<ReviewsView> Localizer { get; set; } = default!;

        private List<TagResponse> Tags { get; set; } = default!;
        private List<TagResponse> SelectedTags { get; set; } = new();
        private string _tagSearch = default!;

        private int _page = 1;
        private int _pageSize = 10;
        private bool _hiddenButtonLoadMore = false;
        private bool _sendRequest = false;

        protected override async Task OnInitializedAsync()
        {
            var tags = await GetTagsFromApiAsync();
            Tags = tags;
        }

        private async Task<List<TagResponse>> GetTagsFromApiAsync()
        {
            _sendRequest = true;
            var tags = (await HttpClient.GetFromJsonAsync<List<TagResponse>>($"api/Tag?page={_page}&pageSize={_pageSize}{(!string.IsNullOrWhiteSpace(_tagSearch) ? $"&search={_tagSearch}" : null)}"))!;
            _hiddenButtonLoadMore = tags.Count < _pageSize;
            _page++;
            _sendRequest = false;
            return tags;
        }

        private async Task LoadMoreTagsAsync()
        {
            var tags = await GetTagsFromApiAsync();
            Tags.AddRange(tags);
        }

        private async Task SearchTagsAsync(string search)
        {
            _page = 1;
            _tagSearch = search;
            Tags = await GetTagsFromApiAsync();
        }

        private async Task AddTagAsync(TagResponse tag)
        {
            SelectedTags.Add(tag);

            await GetReviewsFromApi.InvokeAsync();
        }

        private async Task RemoveTagAsync(TagResponse tag)
        {
            SelectedTags.Remove(tag);

            await GetReviewsFromApi.InvokeAsync();
        }

        public string GetSelectedTags()
        {
            var idTags = SelectedTags.Select(x => x.Id).ToList();
            string tags = "idTags=";
            foreach (var tag in idTags)
            {
                tags += $"{tag}.";
            }
            return tags.Remove(tags.Length - 1);
        }
    }
}