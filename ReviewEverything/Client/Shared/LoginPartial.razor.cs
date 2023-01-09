using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace ReviewEverything.Client.Shared
{
    public partial class LoginPartial
    {
        [CascadingParameter] public MainLayout Parent { get; set; } = default!;
        [Inject] private IStringLocalizer<LoginPartial> Localizer { get; set; } = default!;
        public async Task LogoutAsync()
        {
            await HttpClient.PostAsync("api/Account/Logout", null);
            await Parent.RefreshStateAsync();
            Parent.ChangeDrawerOpen();
        }
    }
}
