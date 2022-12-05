using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.Net.Http.Json;
using System.Net;
using ReviewEverything.Shared.Models.Account;

namespace ReviewEverything.Client.Pages.Account
{
    public partial class SignUp
    {
        [Inject] private ISnackbar Snackbar { get; set; } = default!;

        private SignUpModel _model = new();
        private bool _sendRequest = false;
        private async Task OnValidSubmitAsync(EditContext context)
        {
            _sendRequest = true;

            var httpResponseMessage = await HttpClient.PostAsJsonAsync("api/Account/SignUp", _model);
            if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                Snackbar.Add("Пользователь успешно зарегистрирован", Severity.Success);
                NavigationManager.NavigateTo("/Account/SingIn");
            }
            else
            {
                Snackbar.Add(await httpResponseMessage.Content.ReadAsStringAsync(), Severity.Error);
            }

            _sendRequest = false;
        }
    }
}
