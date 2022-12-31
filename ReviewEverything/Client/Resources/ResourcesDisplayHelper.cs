using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace ReviewEverything.Client.Resources
{
    public class ResourcesDisplayHelper
    {
        public static IStringLocalizer<ResourcesDisplayHelper> CreateStringLocalizer()
        {
            IStringLocalizerFactory factory = new ResourceManagerStringLocalizerFactory(
                new OptionsManager<LocalizationOptions>(
                    new OptionsFactory<LocalizationOptions>(new List<IConfigureOptions<LocalizationOptions>>(),
                        new List<IPostConfigureOptions<LocalizationOptions>>())),
                new LoggerFactory()
            );
            return new StringLocalizer<ResourcesDisplayHelper>(factory);
        }
    }
}