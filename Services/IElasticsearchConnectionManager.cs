using Elastic.Clients.Elasticsearch;

namespace Nop.Plugin.Misc.Elasticsearch.Services;

/// <summary>
/// Provides an interface for managing Elasticsearch connections.
/// </summary>
public interface IElasticsearchConnectionManager
{
    /// <summary>
    /// Asynchronously gets the Elasticsearch client. If the client is not already initialized,
    /// it initializes the client before returning it.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="ElasticsearchClient"/> instance.</returns>
    Task<ElasticsearchClient> GetClientAsync();
}
