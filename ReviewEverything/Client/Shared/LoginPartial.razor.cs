using Microsoft.AspNetCore.Components;
using ReviewEverything.Client.Services.Authorization;

namespace ReviewEverything.Client.Shared
{
    public partial class LoginPartial
    {
        [CascadingParameter] public MainLayout Parent { get; set; } = default!;

        private async Task LogoutAsync()
        {
            await HttpClient.PostAsync("api/Account/Logout", null);
            Parent.RefreshState();
        }
    }
}
