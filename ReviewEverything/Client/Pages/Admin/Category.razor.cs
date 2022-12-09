using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using ReviewEverything.Shared.Contracts.Requests;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Client.Pages.Admin
{
    public partial class Category
    {
        [Inject] private ISnackbar Snackbar { get; set; } = default!;
        [Inject] private IDialogService DialogService { get; set; } = default!;
        private MudMessageBox MessageBox { get; set; } = default!;
        private List<CategoryResponse> Categories { get; set; } = default!;
        private CategoryRequest CategoryRequest { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            Categories = (await HttpClient.GetFromJsonAsync<List<CategoryResponse>>("api/Category"))!;
        }

        private async Task CreateOrEditCategoryAsync(int categoryId = 0, string title = "")
        {
            CategoryRequest.Title = title;

            bool? result = await MessageBox.Show();
            if (result == true && CheckMinSymbolsCategoryTitle())
            {
                HttpResponseMessage httpResponseMessage;
                if (categoryId == default)
                {
                    httpResponseMessage = await HttpClient.PostAsJsonAsync("api/Category", CategoryRequest);
                }
                else
                {
                    httpResponseMessage = await HttpClient.PutAsJsonAsync($"api/Category/{categoryId}", CategoryRequest);
                }

                await CheckStatusCodeCreateOrEditCategoryResponseAsync(httpResponseMessage);
            }

            CategoryRequest.Title = string.Empty;
        }

        private bool CheckMinSymbolsCategoryTitle()
        {
            if (CategoryRequest.Title.Length <= 4)
            {
                Snackbar.Add("Название категории должно быть больше 4 символов", Severity.Warning);
                return false;
            }

            return true;
        }

        private async Task CheckStatusCodeCreateOrEditCategoryResponseAsync(HttpResponseMessage httpResponseMessage)
        {
            switch (httpResponseMessage.StatusCode)
            {
                case HttpStatusCode.Created:
                    Categories.Add((await httpResponseMessage.Content.ReadFromJsonAsync<CategoryResponse>())!);
                    break;
                case HttpStatusCode.OK:
                    var category = (await httpResponseMessage.Content.ReadFromJsonAsync<CategoryResponse>())!;
                    Categories[Categories.FindIndex(x => x.Id == category.Id)] = category;
                    break;
                case HttpStatusCode.NotFound:
                    Snackbar.Add("Категория не найдена", Severity.Error);
                    break;
                default:
                    Snackbar.Add("Во время сохранения произошла ошибка", Severity.Error);
                    break;
            }
        }

        private async Task DeleteCategoryAsync(int categoryId)
        {
            if (await ShowDeleteCategoryMessageBoxAsync() != true)
                return;

            var httpResponseMessage = await HttpClient.DeleteAsync($"api/Category/{categoryId}");

            switch (httpResponseMessage.StatusCode)
            {
                case HttpStatusCode.NoContent:
                    Categories.RemoveAll(category => category.Id == categoryId);
                    break;
                case HttpStatusCode.NotFound:
                    Snackbar.Add("Категория не найдена", Severity.Error);
                    break;
                default:
                    Snackbar.Add("Во время удаления произошла ошибка", Severity.Error);
                    break;
            }
        }

        private async Task<bool?> ShowDeleteCategoryMessageBoxAsync()
        {
            return await DialogService.ShowMessageBox(
                "Внимание",
                "Удаление не может быть отменено!",
                yesText: "Удалить!", cancelText: "Отмена");
        }
    }
}
