using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.Net.Http.Json;
using System.Net;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using ReviewEverything.Client.Resources;
using ReviewEverything.Shared.Models.Account;

namespace ReviewEverything.Client.Pages.Account
{
    public partial class SignUp
    {
        [Inject] private ISnackbar Snackbar { get; set; } = default!;
        private IStringLocalizer<AccountShared> Localizer { get; set; } = AccountShared.CreateStringLocalizer();

        private readonly SignUpModel _model = new();
        private bool _sendRequest = false;

        protected override void OnInitialized()
        {
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomEnd;
        }

        private async Task OnValidSubmitAsync(EditContext context)
        {
            _sendRequest = true;

            var httpResponseMessage = await HttpClient.PostAsJsonAsync("api/Account/SignUp", _model);
            if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
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
