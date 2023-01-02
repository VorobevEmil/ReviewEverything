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
        private MudThemeProvider _mudThemeProvider = default!;
        private LoginPartial _loginPartial = default!;
        private HubConnection _hubConnection = default!;
        protected override async Task OnInitializedAsync()
        {
            LayoutService.SetBaseTheme(CustomTheme.LandingPageTheme());
            await ConfigureHubConnectionAsync();
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

            await _hubConnection.StartAsync();
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
        public void RefreshState()
        {
            HostAuthenticationStateProvider.RefreshState();
            StateHasChanged();
        }

        public void NavigateMyPage(string userId)
        {
            NavigationManager.NavigateTo("");
            NavigationManager.NavigateTo($"./user/{userId}");
            ChangeDrawerOpen();
        }
    }
}