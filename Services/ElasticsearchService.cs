using Nop.Plugin.SearchProvider.Elasticsearch.Settings;
using Nop.Plugin.SearchProvider.Elasticsearch.Validators;
using Nop.Services.Localization;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Services;
public class ElasticsearchService : IElasticsearchService
{
    private readonly ILocalizationService _localizationService;
    private readonly ElasticsearchSettings _elasticsearchSettings;

    public ElasticsearchService
    (
        ILocalizationService localizationService,
        ElasticsearchSettings elasticsearchSettings
    )
    {
        _localizationService = localizationService;
        _elasticsearchSettings = elasticsearchSettings;
    }

    /// <summary>
    /// Asynchronously checks if the Elasticsearch settings are configured properly.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous operation. 
    /// The task result is a boolean indicating whether the Elasticsearch settings are valid.
    /// </returns>
    public async Task<bool> IsConfiguredAsync()
    {
        var validator = new ElasticsearchSettingsValidator(_localizationService);
        var validationResult = await validator.ValidateAsync(_elasticsearchSettings);

        return validationResult.IsValid && _elasticsearchSettings.Active;
    }
}
