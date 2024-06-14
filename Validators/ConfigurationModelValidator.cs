using Nop.Plugin.SearchProvider.Elasticsearch.Areas.Admin.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Validators;
public class ConfigurationModelValidator : BaseNopValidator<ConfigurationModel>
{
    public ConfigurationModelValidator(ILocalizationService localizationService)
    {
        BaseConfigurationValidator.ConfigureCommonRules(this, localizationService);
    }
}
