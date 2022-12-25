using Microsoft.AspNetCore.Components;
using ReviewEverything.Client.Services.Authorization;

namespace ReviewEverything.Client.Shared
{
    public partial class LoginPartial
    {
        [CascadingParameter] public MainLayout Parent { get; set; } = default!;

        public async Task LogoutAsync()
        {
            await HttpClient.PostAsync("api/Account/Logout", null);
            Parent.RefreshState();
            Parent.ChangeDrawerOpen();
        }
    }
}
