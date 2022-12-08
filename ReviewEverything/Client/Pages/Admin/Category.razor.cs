using System.Net;
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
        private List<CategoryResponse> Categories { get; set; } = default!;
        private MudMessageBox Mbox { get; set; } = default!;
        private CategoryRequest _categoryRequest = new();

        protected override async Task OnInitializedAsync()
        {
            Categories = (await HttpClient.GetFromJsonAsync<List<CategoryResponse>>("api/Category"))!;
        }

        private async Task CreateCategoryAsync()
        {
            bool? result = await Mbox.Show();
            if (result == true && CheckMinSymbolsCategoryTitle())
            {
                var httpResponseMessage = await HttpClient.PostAsJsonAsync("api/Category", _categoryRequest);
                if (httpResponseMessage.StatusCode == HttpStatusCode.Created)
                {
                    Categories.Add((await httpResponseMessage.Content.ReadFromJsonAsync<CategoryResponse>())!);
                }
            }

            _categoryRequest.Title = string.Empty;
        }

        private bool CheckMinSymbolsCategoryTitle()
        {
            if (_categoryRequest.Title.Length <= 4)
            {
                Snackbar.Add("Название категории должно быть больше 4 символов", Severity.Warning);
                return false;
            }

            return true;
        }
    }
}
