namespace Nop.Plugin.Misc.Elasticsearch.Services;
public interface IElasticsearchService
{
    /// <summary>
    /// Asynchronously checks if the Elasticsearch settings are configured properly.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous operation. 
    /// The task result is a boolean indicating whether the Elasticsearch settings are valid.
    /// </returns>
    Task<bool> IsConfiguredAsync();
}
