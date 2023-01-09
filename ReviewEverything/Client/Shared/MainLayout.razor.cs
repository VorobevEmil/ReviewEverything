using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Localization;
using MudBlazor;
using ReviewEverything.Client.Services;
using ReviewEverything.Client.Services.Authorization;
using ReviewEverything.Client.Theme;

namespace ReviewEverything.Client.Shared
{
    public partial class MainLayout
    {
        [Inject] private ISnackbar Snackbar { get; set; } = default!;
        [Inject] private LayoutService LayoutService { get; set; } = default!;
        [Inject] private HostAuthenticationStateProvider HostAuthenticationStateProvider { get; set; } = default!;
        [Inject] private IStringLocalizer<MainLayout> Localizer { get; set; } = default!;
        private bool DrawerOpen { get; set; }
        private bool _userLoggedIn = false;
        private MudThemeProvider _mudThemeProvider = default!;
        private LoginPartial _loginPartial = default!;
        private HubConnection _hubConnection = default!;
        protected override async Task OnInitializedAsync()
        {
            LayoutService.SetBaseTheme(CustomTheme.LandingPageTheme());
            await CheckUserLoggedIn();
            await ConfigureHubConnectionAsync();
            await CheckMyAccountAsync();
        }

        private async Task CheckUserLoggedIn()
        {
            _userLoggedIn = (await HostAuthenticationStateProvider.GetAuthenticationStateAsync()).User.Identity!
                .IsAuthenticated;
        }

        private async Task ConfigureHubConnectionAsync()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri("/userManagerHub"))
                .Build();

            _hubConnection.On<string>("LogoutAccount", async (message) =>
            {
                await _loginPartial.LogoutAsync();
                Snackbar.Add(message, Severity.Error);
            });

            await StartOrStopHubConnectionAsync();
        }

        private async Task StartOrStopHubConnectionAsync()
        {
            await CheckUserLoggedIn();
            if(_userLoggedIn)
                await _hubConnection.StartAsync();
            else
                await _hubConnection.StopAsync();
        }

        private async Task CheckMyAccountAsync()
        {
            if (!_userLoggedIn)
                return;

            if (await HttpClient.GetFromJsonAsync<bool>("api/User/CheckAccount"))
                await _loginPartial.LogoutAsync();
        }


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await ApplyUserPreferencesAsync();
                StateHasChanged();
            }
        }

        private async Task ApplyUserPreferencesAsync()
        {
            var defaultDarkMode = await _mudThemeProvider.GetSystemPreference();
            await LayoutService.ApplyUserPreferencesAsync(defaultDarkMode);
        }

        public void ChangeDrawerOpen() => DrawerOpen = !DrawerOpen;
        public async Task RefreshStateAsync()
        {
            HostAuthenticationStateProvider.RefreshState();
            StateHasChanged();
            await StartOrStopHubConnectionAsync();
        }

        public void NavigateMyPage(string userId)
        {
            NavigationManager.NavigateTo("");
            NavigationManager.NavigateTo($"./user/{userId}");
            ChangeDrawerOpen();
        }
    }
}