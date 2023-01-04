using Microsoft.AspNetCore.Components;
using ReviewEverything.Shared.Contracts.Responses;
using ReviewEverything.Shared.Models.Enums;
using System.Net.Http.Json;

namespace ReviewEverything.Client.Components.ReviewsView
{
    public partial class FilterOptionView
    {
        [Parameter] public EventCallback GetReviewsFromApi { get; set; }
        [Parameter] public string? UrlCategoryId { get; set; } = default!;
        [Parameter] public string? UserId { get; set; } = default!;
        private SortReviewByProperty _sortByProperty;
        private int _filterByAuthorScore = 0;
        private int? _filterByCompositionId = default!;
        private CompositionResponse _selectedComposition = default!;

        private async Task ChangeSortByProperty(SortReviewByProperty sortByProperty)
        {
            _sortByProperty = sortByProperty;
            await GetReviewsFromApi.InvokeAsync();
        }

        private async Task ChangeFilterByAuthorScore(int filterByAuthorScore)
        {
            _filterByAuthorScore = filterByAuthorScore;
            await GetReviewsFromApi.InvokeAsync();
        }

        private async Task<IEnumerable<CompositionResponse>> SearchCompositionAsync(string search)
        {
            var urlSearch = (!string.IsNullOrWhiteSpace(search) ? $"search={search}&" : null);
            var urlUserId = (!string.IsNullOrWhiteSpace(UserId) ? $"userId={UserId}&" : null);
            return (await HttpClient.GetFromJsonAsync<List<CompositionResponse>>($"api/Composition?{urlSearch + UrlCategoryId + urlUserId}"))!;
        }

        private async Task SelectCompositionAsync(CompositionResponse composition)
        {
            _filterByCompositionId = composition.Id;
            _selectedComposition = composition;
            await GetReviewsFromApi.InvokeAsync();
        }

        private async Task ClearCompositionSearchAsync()
        {
            _filterByCompositionId = default!;
            _selectedComposition = default!;
            await GetReviewsFromApi.InvokeAsync();
        }

        public string GetFilterParameterUrl()
        {
            var sortByProperty = $"sortByProperty={_sortByProperty}&";
            var filterByAuthorScore = _filterByAuthorScore != 0 ? $"filterByAuthorScore={_filterByAuthorScore}&" : null;
            var filterByCompositionId = _filterByCompositionId != 0 ? $"filterByCompositionId={_filterByCompositionId}&" : null;
            return sortByProperty + filterByAuthorScore + filterByCompositionId;
        }
    }
}