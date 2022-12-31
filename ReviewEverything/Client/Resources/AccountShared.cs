using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace ReviewEverything.Client.Resources;

public class AccountShared
{
    public static IStringLocalizer<AccountShared> CreateStringLocalizer()
    {
        IStringLocalizerFactory factory = new ResourceManagerStringLocalizerFactory(
            new OptionsManager<LocalizationOptions>(
                new OptionsFactory<LocalizationOptions>(new List<IConfigureOptions<LocalizationOptions>>(),
                    new List<IPostConfigureOptions<LocalizationOptions>>())),
            new LoggerFactory()
        );
        return new StringLocalizer<AccountShared>(factory);
    }
}