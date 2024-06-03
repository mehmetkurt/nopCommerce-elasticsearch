using Nop.Plugin.Misc.Elasticsearch.Settings;
using Nop.Plugin.Misc.Elasticsearch.Validators;

namespace Nop.Plugin.Misc.Elasticsearch.Services;
public class ElasticsearchService : IElasticsearchService
{
    private readonly ElasticsearchSettings _elasticsearchSettings;

    public ElasticsearchService(ElasticsearchSettings elasticsearchSettings)
    {
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
        var validator = new ElasticsearchSettingsValidator();
        var validationResult = await validator.ValidateAsync(_elasticsearchSettings);

        return validationResult.IsValid;
    }
}
