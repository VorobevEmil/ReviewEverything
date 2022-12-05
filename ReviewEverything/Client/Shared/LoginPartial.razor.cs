using Microsoft.AspNetCore.Components;
using ReviewEverything.Client.Services.Authorization;

namespace ReviewEverything.Client.Shared
{
    public partial class LoginPartial
    {
        [Inject] private HostAuthenticationStateProvider HostAuthenticationStateProvider { get; set; } = default!;
        [CascadingParameter] public MainLayout Parent { get; set; } = default!;

        private async Task LogoutAsync()
        {
            await HttpClient.PostAsync("api/Account/Logout", null);
            HostAuthenticationStateProvider.RefreshState();
            Parent.RefreshState();
        }
    }
}
