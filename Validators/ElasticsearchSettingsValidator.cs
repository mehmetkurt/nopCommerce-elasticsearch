﻿using Nop.Plugin.SearchProvider.Elasticsearch.Settings;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Validators;
public class ElasticsearchSettingsValidator : BaseNopValidator<ElasticsearchSettings>
{
    public ElasticsearchSettingsValidator(ILocalizationService localizationService)
    {
        BaseConfigurationValidator.ConfigureCommonRules(this, localizationService);
    }
}
