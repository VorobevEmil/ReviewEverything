using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using ReviewEverything.Client.Helpers;
using ReviewEverything.Shared.Contracts.Requests;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Client.Pages.Admin
{
    public partial class Category
    {
        [Inject] private IStringLocalizer<Category> Localizer { get; set; } = default!;
        [Inject] private ISnackbar Snackbar { get; set; } = default!;
        [Inject] private DisplayHelper DisplayHelper { get; set; } = default!;
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
                if (categoryId == default)
                    await CreateCategoryAsync();
                else
                    await UpdateCategoryAsync(categoryId);
            }

            CategoryRequest.Title = string.Empty;
        }

        private bool CheckMinSymbolsCategoryTitle()
        {
            if (CategoryRequest.Title.Length <= 3)
            {
                Snackbar.Add(Localizer["Название категории должно не менее 3 символов"], Severity.Warning);
                return false;
            }

            return true;
        }

        private async Task CreateCategoryAsync()
        {
            var httpResponseMessage = await HttpClient.PostAsJsonAsync("api/Category", CategoryRequest);
            if (httpResponseMessage.StatusCode == HttpStatusCode.Created)
                Categories.Add((await httpResponseMessage.Content.ReadFromJsonAsync<CategoryResponse>())!);
            else
                Snackbar.Add(await httpResponseMessage.Content.ReadAsStringAsync(), Severity.Error);
        }

        private async Task UpdateCategoryAsync(int categoryId)
        {
            var httpResponseMessage = await HttpClient.PutAsJsonAsync($"api/Category/{categoryId}", CategoryRequest);
            if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                var category = (await httpResponseMessage.Content.ReadFromJsonAsync<CategoryResponse>())!;
                Categories[Categories.FindIndex(x => x.Id == category.Id)] = category;
            }
            else
                Snackbar.Add(await httpResponseMessage.Content.ReadAsStringAsync(), Severity.Error);

        }

        private async Task DeleteCategoryAsync(int categoryId)
        {
            if (await DisplayHelper.ShowDeleteMessageBoxAsync() != true)
                return;

            var httpResponseMessage = await HttpClient.DeleteAsync($"api/Category/{categoryId}");

            if (httpResponseMessage.StatusCode == HttpStatusCode.NoContent)
                Categories.RemoveAll(category => category.Id == categoryId);
            else
                Snackbar.Add(await httpResponseMessage.Content.ReadAsStringAsync(), Severity.Error);

        }
    }
}
