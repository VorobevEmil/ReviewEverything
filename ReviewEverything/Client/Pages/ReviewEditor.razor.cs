using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using ReviewEverything.Client.Components;
using ReviewEverything.Shared.Contracts.Requests;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Client.Pages
{
    public partial class ReviewEditor
    {
        [Inject] private ISnackbar Snackbar { get; set; } = default!;
        [Parameter] public int? Id { get; set; }

        private ReviewRequest _review = null!;
        private SelectOrCreateComposition _composition = null!;
        private SelectOrCreateTags _tags = null!;
        protected override async Task OnInitializedAsync()
        {
            await GetReviewAsync();
        }

        private async Task GetReviewAsync()
        {
            if (Id != null)
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

        private async Task OnValidSubmitAsync(EditContext context)
        {
            await _composition.CreateCompositionAsync();
            await CreateTagsAsync();

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

        private async Task CreateTagsAsync()
        {
            foreach (var tag in _tags.CreateTags)
            {
                var httpResponseMessage = await HttpClient.PostAsJsonAsync("api/Tag", tag);


                if (httpResponseMessage.StatusCode == HttpStatusCode.Created)
                {
                    var responseTag = (await httpResponseMessage.Content.ReadFromJsonAsync<TagResponse>())!;
                    _review.Tags[_review.Tags.FindIndex(x => x.Title == tag.Title)] = responseTag;
                }
                else
                {
                    _review.Tags.RemoveAll(x => x.Title == tag.Title);
                    Snackbar.Add(await httpResponseMessage.Content.ReadAsStringAsync(), Severity.Error);
                }
            }
        }
    }
}