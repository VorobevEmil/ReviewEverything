using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using MudBlazor;
using ReviewEverything.Client.Components.ReviewEditor;
using ReviewEverything.Shared.Contracts.Requests;

namespace ReviewEverything.Client.Pages
{
    public partial class ReviewEditor
    {
        [Inject] private ISnackbar Snackbar { get; set; } = default!;
        [Inject] private IStringLocalizer<ReviewEditor> Localizer { get; set; } = default!;
        [Parameter] public int? Id { get; set; }

        private ReviewRequest _review = default!;
        private SelectOrCreateComposition _composition = default!;
        private SelectOrCreateTags _tags = default!;
        protected override async Task OnInitializedAsync()
        {
            await GetReviewAsync();
        }

        private async Task GetReviewAsync()
        {
            if (Id is not null)
            {
                var httpResponseMessage = await HttpClient.GetAsync($"api/Review/Edit/{Id}");
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

        private async Task CheckValidSubmitAsync(EditContext context)
        {
            if (context.Validate())
            {
                await _composition.CreateCompositionAsync();
                await _tags.CreateTagsAsync();
                await CreateOrUpdateReview();
            }
            else
            {
                ShowErrorsMessageInMessageBox(context);
            }
        }

        private async Task CreateOrUpdateReview()
        {
            HttpResponseMessage httpMessageResponse;
            if (Id != null)
                httpMessageResponse = await HttpClient.PutAsJsonAsync($"api/Review/{Id}", _review);
            else
                httpMessageResponse = await HttpClient.PostAsJsonAsync("api/Review", _review);

            if (httpMessageResponse.StatusCode is HttpStatusCode.OK or HttpStatusCode.Created)
            {
                Snackbar.Add($"Обзор успешно {(Id != null ? "обновлен" : "создан")}", Severity.Success);
                NavigationManager.NavigateTo("./");
            }
            else
            {
                Snackbar.Add($"Не удалось {(Id != null ? "обновить" : "создать")} обзор", Severity.Error);
            }
        }

        private void ShowErrorsMessageInMessageBox(EditContext context)
        {
            Snackbar.Clear();
            foreach (var errorMessage in context.GetValidationMessages())
            {
                Snackbar.Add(errorMessage, Severity.Warning);
            }
        }
    }
}