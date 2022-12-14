using Microsoft.AspNetCore.Components;
using MudBlazor;
using ReviewEverything.Shared.Contracts.Requests;
using ReviewEverything.Shared.Contracts.Responses;
using System.Net.Http.Json;

namespace ReviewEverything.Client.Components
{
    public partial class SelectOrCreateComposition
    {
        [Inject] private ISnackbar Snackbar { get; set; } = default!;
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

        private async Task<IEnumerable<CompositionResponse>> SearchCompositionAsync(string search)
        {
            return (await HttpClient.GetFromJsonAsync<List<CompositionResponse>>($"api/Composition{(!string.IsNullOrWhiteSpace(search) ? $"?search={search}" : null)}"))!;
        }


        private async Task GetCategoriesAsync()
        {
            Categories = (await HttpClient.GetFromJsonAsync<List<CategoryResponse>>("api/Category"))!;
        }

        public async Task CreateCompositionAsync()
        {
            if (Review.CompositionId == 0)
            {
                var httpResponseMessage = await HttpClient.PostAsJsonAsync<CompositionRequest>("api/Composition", Review.Composition!);
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
    }
}
