using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using ReviewEverything.Shared.Models.Account;
using System.Net.Http.Json;
using System.Net;
using ReviewEverything.Client.Services.Authorization;
using ReviewEverything.Client.Shared;
using Microsoft.Extensions.Localization;
using ReviewEverything.Client.Resources;

namespace ReviewEverything.Client.Pages.Account
{
    public partial class SignUpProvider
    {
        [Inject] private ISnackbar Snackbar { get; set; } = default!;
        [Parameter] public string Provider { get; set; } = default!;
        [Inject] private HostAuthenticationStateProvider HostAuthenticationStateProvider { get; set; } = default!;
        [CascadingParameter] public MainLayout Parent { get; set; } = default!;
        private IStringLocalizer<AccountShared> Localizer { get; set; } = AccountShared.CreateStringLocalizer();

        private OAuthSignUpModel _model = new();
        private bool _sendRequest = false;

        protected override async Task OnInitializedAsync()
        {
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomEnd;
            var httpMessageResponse = await HttpClient.GetAsync($"api/OAuth/GetSignUpModel/{Provider}");
            if (httpMessageResponse.IsSuccessStatusCode)
            {
                _model = (await httpMessageResponse.Content.ReadFromJsonAsync<OAuthSignUpModel>())!;
            }
        }

        private async Task OnValidSubmitAsync(EditContext context)
        {
            _sendRequest = true;

            var httpResponseMessage = await HttpClient.PostAsJsonAsync($"api/OAuth/SignUp/{Provider}", _model);
            if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                HostAuthenticationStateProvider.RefreshState();
                Parent.RefreshState();
                NavigationManager.NavigateTo("/");
            }
            else
            {
                Snackbar.Add(await httpResponseMessage.Content.ReadAsStringAsync(), Severity.Error);
            }

            _sendRequest = false;
        }
    }
}
