using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using ReviewEverything.Shared.Contracts.Requests;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Client.Components.ReviewEditor
{
    public partial class SelectOrCreateComposition
    {
        [Inject] private ISnackbar Snackbar { get; set; } = default!;
        [Inject] private IStringLocalizer<Pages.ReviewEditor> Localizer { get; set; } = default!;
        [Parameter] public ReviewRequest Review { get; set; } = default!;
        private List<CategoryResponse>? Categories { get; set; }

        private CompositionResponse _selectedComposition = new();

        protected override async Task OnInitializedAsync()
        {
            if (Review.CompositionId != default)
            {
                _selectedComposition.Id = Review.CompositionId!.Value;
                _selectedComposition.Title = Review.Composition!.Title;
            }

            await GetCategoriesAsync();
        }
        private async Task GetCategoriesAsync()
        {
            Categories = (await HttpClient.GetFromJsonAsync<List<CategoryResponse>>("api/Category"))!;
        }

        private async Task<IEnumerable<CompositionResponse>> SearchCompositionAsync(string search)
        {
            return (await HttpClient.GetFromJsonAsync<List<CompositionResponse>>($"api/Composition{(!string.IsNullOrWhiteSpace(search) ? $"?search={search}" : null)}"))!;
        }

        public async Task CreateCompositionAsync()
        {
            if (Review.CompositionId == 0)
            {
                var httpResponseMessage = await HttpClient.PostAsJsonAsync("api/Composition", Review.Composition!);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    _selectedComposition = (await httpResponseMessage.Content.ReadFromJsonAsync<CompositionResponse>())!;
                    Review.Composition = null;
                    Review.CompositionId = _selectedComposition.Id;
                }
                else
                {
                    Snackbar.Add("Не удалось создать произведение, повторите попытку", Severity.Error);
                }
            }
        }

        private void SelectComposition(CompositionResponse composition)
        {
            Review.CompositionId = composition.Id;
            _selectedComposition = composition;
        }

        private void SetPropertyForCreateComposition()
        {
            Review.Composition = new CompositionRequest();
            Review.CompositionId = 0;
        }

        private void SetPropertyForSelectComposition()
        {
            Review.Composition = null;
            Review.CompositionId = null;
        }
    }
}
