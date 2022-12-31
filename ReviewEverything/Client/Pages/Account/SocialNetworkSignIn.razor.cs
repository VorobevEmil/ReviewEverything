using Microsoft.Extensions.Localization;
using ReviewEverything.Client.Resources;

namespace ReviewEverything.Client.Pages.Account
{
    public partial class SocialNetworkSignIn
    {
        private IStringLocalizer<AccountShared> Localizer { get; set; } = AccountShared.CreateStringLocalizer();
    }
}