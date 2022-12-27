using Microsoft.AspNetCore.Components;
using ReviewEverything.Shared.Models.Enums;

namespace ReviewEverything.Client.Components.ReviewsView
{
    public partial class FilterOptionView
    {
        private SortReviewByProperty _sortByProperty;
        private int _filterByAuthorScore = 0;

        [Parameter] public EventCallback GetReviewsFromApi { get; set; }

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

        public string GetFilterParameterUrl()
        {
            var sortByProperty = $"sortByProperty={_sortByProperty}&";
            var filterByAuthorScore = _filterByAuthorScore != 0 ? $"filterByAuthorScore={_filterByAuthorScore}&" : null;
            return sortByProperty + filterByAuthorScore;
        }
    }
}