using Microsoft.AspNetCore.Components;
using ReviewEverything.Client.Services.Authorization;

namespace ReviewEverything.Client.Shared
{
    public partial class LoginMobilePartial
    {
        [Inject] private HostAuthenticationStateProvider HostAuthenticationStateProvider { get; set; } = default!;
        [CascadingParameter] public MainLayout Parent { get; set; } = default!;
        private bool _isAuthenticated;

        protected override async Task OnInitializedAsync()
        {
            _isAuthenticated = (await HostAuthenticationStateProvider.GetAuthenticationStateAsync()).User.Identity!.IsAuthenticated;
        }


        private async Task LogoutAsync()
        {
            await HttpClient.PostAsync("api/Account/Logout", null);
            HostAuthenticationStateProvider.RefreshState();
            await OnInitializedAsync();
            Parent.RefreshState();
        }
    }
}
