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
        [Parameter] public int? Id { get; set; }

        [Inject] private ISnackbar Snackbar { get; set; } = default!;
        [Inject] private IStringLocalizer<ReviewEditor> Localizer { get; set; } = default!;

        private bool _savingReview = default!;

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
                else
                {
                    Snackbar.Add(await httpResponseMessage.Content.ReadAsStringAsync(), Severity.Error);
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
                _savingReview = true;

                await _composition.CreateCompositionAsync();
                await _tags.CreateTagsAsync();
                if (Id == default)
                    await CreateReviewAsync();
                else
                    await UpdateReviewAsync();

                _savingReview = false;
            }
            else
            {
                ShowErrorsMessageInMessageBox(context);
            }
        }

        private async Task CreateReviewAsync()
        {
            var httpMessageResponse = await HttpClient.PostAsJsonAsync("api/Review", _review);

            await ShowResultRequestAsync(httpMessageResponse);
        }

        private async Task UpdateReviewAsync()
        {
            var httpMessageResponse = await HttpClient.PutAsJsonAsync($"api/Review/{Id}", _review);

            await ShowResultRequestAsync(httpMessageResponse);
        }

        private async Task ShowResultRequestAsync(HttpResponseMessage httpMessageResponse)
        {
            Snackbar.Add(await httpMessageResponse.Content.ReadAsStringAsync(), httpMessageResponse.StatusCode == HttpStatusCode.OK ? Severity.Success : Severity.Error);
            if (httpMessageResponse.StatusCode == HttpStatusCode.OK)
            {
                NavigationManager.NavigateTo("./");
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