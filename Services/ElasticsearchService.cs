using Nop.Plugin.SearchProvider.Elasticsearch.Settings;
using Nop.Plugin.SearchProvider.Elasticsearch.Validators;
using Nop.Services.Localization;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Services;

/// <summary>
/// Service for managing Elasticsearch configuration and operations.
/// </summary>
public class ElasticsearchService : IElasticsearchService
{
    private readonly ILocalizationService _localizationService;
    private readonly ElasticsearchSettings _elasticsearchSettings;

    /// <summary>
    /// Initializes a new instance of the ElasticsearchService class.
    /// </summary>
    /// <param name="localizationService">Localization service for retrieving localized resources.</param>
    /// <param name="elasticsearchSettings">Settings object containing Elasticsearch configuration.</param>
    public ElasticsearchService(ILocalizationService localizationService, ElasticsearchSettings elasticsearchSettings)
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
