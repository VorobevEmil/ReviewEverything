using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using ReviewEverything.Client.Components;
using ReviewEverything.Shared.Contracts.Requests;

namespace ReviewEverything.Client.Pages
{
    public partial class ReviewEditor
    {
        [Inject] private ISnackbar Snackbar { get; set; } = default!;
        [Parameter] public int? Id { get; set; }

        private ReviewRequest _review = null!;
        private SelectOrCreateComposition _composition = null!;
        protected override async Task OnInitializedAsync()
        {
            await GetReviewAsync();
        }

        private async Task GetReviewAsync()
        {
            if (Id != null)
            {
                var httpResponseMessage = await HttpClient.GetAsync($"api/Review/{Id}?typeReview=2");
                if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                {
                    _review = (await httpResponseMessage.Content.ReadFromJsonAsync<ReviewRequest>())!;
                }
                else if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    Snackbar.Add("Обзор был не найден", Severity.Error);
                }
                else
                {
                    Snackbar.Add("Не удалось загрузить обзор", Severity.Error);
                }
            }
            else
            {
                _review = new();
            }
        }

        private async Task OnValidSubmitAsync(EditContext context)
        {
            await _composition.CreateCompositionAsync();
            HttpResponseMessage httpMessageResponse;
            if (Id != null)
            {
                httpMessageResponse = await HttpClient.PutAsJsonAsync($"api/Review/{Id}", _review);
                if (httpMessageResponse.StatusCode == HttpStatusCode.OK)
                {
                    Snackbar.Add("Обзор успешно обновлен", Severity.Success);
                    NavigationManager.NavigateTo("./");
                }
                else
                {
                    Snackbar.Add("Не удалось обновить обзор", Severity.Error);
                }
            }
            else
            {
                httpMessageResponse = await HttpClient.PostAsJsonAsync("api/Review", _review);
                if (httpMessageResponse.StatusCode == HttpStatusCode.Created)
                {
                    Snackbar.Add("Обзор успешно создан", Severity.Success);
                    NavigationManager.NavigateTo("./");
                }
                else
                {
                    Snackbar.Add("Не удалось создать обзор", Severity.Error);
                }
            }
            
        }
    }
}