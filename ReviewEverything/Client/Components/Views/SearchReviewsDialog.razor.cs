using Microsoft.Extensions.Localization;
using MudBlazor;
using ReviewEverything.Shared.Contracts.Responses;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;

namespace ReviewEverything.Client.Components.Views
{
    public partial class SearchReviewsDialog
    {
        [Inject] private IStringLocalizer<SearchReviewsDialog> Localizer { get; set; } = default!;
        private bool _searchDialogOpen;
        private void OpenSearchDialog() => _searchDialogOpen = true;
        private readonly DialogOptions _dialogOptions = new() { Position = DialogPosition.TopCenter, NoHeader = true };

        private MudAutocomplete<ReviewSearchResponse> _searchAutocomplete = default!;


        private async Task<IEnumerable<ReviewSearchResponse>> SearchAsync(string search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return new List<ReviewSearchResponse>();

            return (await HttpClient.GetFromJsonAsync<IEnumerable<ReviewSearchResponse>>($"api/Review/Search/{search}"))!;
        }

        private async Task OnSearchResult(ReviewSearchResponse? entry)
        {
            NavigationManager.NavigateTo($"/redirect-page");
            NavigationManager.NavigateTo($"/Article/{entry.Id}");
            await _searchAutocomplete.Clear();
        }
    }
}