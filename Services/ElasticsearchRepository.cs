using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Nop.Core;
using Nop.Plugin.SearchProvider.Elasticsearch.Services;
using System.Collections.Immutable;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Repositories;

/// <summary>
/// Generic repository implementation for Elasticsearch operations.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public class ElasticsearchRepository<TEntity> : IElasticsearchRepository<TEntity> where TEntity : BaseEntity
{
    private readonly ElasticsearchClient _client;
    private readonly IElasticsearchConnectionManager _elasticsearchConnectionManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticsearchRepository{TEntity}"/> class.
    /// </summary>
    /// <param name="client">The Elasticsearch client.</param>
    public ElasticsearchRepository(IElasticsearchConnectionManager elasticsearchConnectionManager)
    {
        _elasticsearchConnectionManager = elasticsearchConnectionManager;
        _client = _elasticsearchConnectionManager.GetClientAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Retrieves the entity by its identifier asynchronously.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>The entity with the specified identifier.</returns>
    public async Task<TEntity> GetByIdAsync(int id)
    {
        var response = await _client.GetAsync<TEntity>(id.ToString());

        if (response.IsValidResponse && response.ApiCallDetails.HttpStatusCode == 200)
        {
            return response.Source;
        }

        return null;
    }

    /// <summary>
    /// Inserts the entity into Elasticsearch asynchronously.
    /// </summary>
    /// <param name="entity">The entity to insert.</param>
    public async Task InsertAsync(TEntity entity)
    {
        var response = await _client.IndexAsync(entity);

        if (!response.IsValidResponse)
        {
            throw new Exception($"Failed to insert document. Error: {response.DebugInformation}");
        }
    }

    /// <summary>
    /// Updates the entity in Elasticsearch asynchronously.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    public async Task UpdateAsync(TEntity entity)
    {
        var response = await _client.UpdateAsync<TEntity, TEntity>(entity.Id.ToString(), u => u.Doc(entity));

        if (!response.IsValidResponse)
        {
            throw new Exception($"Failed to update document. Error: {response.DebugInformation}");
        }
    }

    /// <summary>
    /// Deletes the entity from Elasticsearch asynchronously.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    public async Task DeleteAsync(TEntity entity)
    {
        var response = await _client.DeleteAsync<TEntity>(entity.Id.ToString());

        if (!response.IsValidResponse)
        {
            throw new Exception($"Failed to delete document. Error: {response.DebugInformation}");
        }
    }

    /// <summary>
    /// Finds entities in Elasticsearch that match the specified query asynchronously.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="indexName">Name of the index.</param>
    /// <param name="from">The starting index for the search results (default is 0).</param>
    /// <param name="size">The maximum number of search results to return (default is int.MaxValue).</param>
    /// <returns>A list of entities that match the query.</returns>
    public async Task<ImmutableList<TEntity>> FindAsync(Query query, string indexName = "", int from = 0, int size = int.MaxValue)
    {
        if (string.IsNullOrEmpty(indexName))
            indexName = typeof(TEntity).Name.ToLower();

        var searchRequest = new SearchRequest(indexName)
        {
            From = from,
            Size = size,
            Query = query,
        };

        var searchResponse = await _client.SearchAsync<TEntity>(searchRequest);

        if (searchResponse.IsValidResponse && searchResponse.ApiCallDetails.HttpStatusCode == 200)
            return [.. searchResponse.Documents];

        return [];
    }

    /// <summary>
    /// Finds and lists documents in Elasticsearch by configuring a specific search request asynchronously.
    /// </summary>
    /// <param name="configureRequest">An action to configure the search request descriptor.</param>
    /// <returns>A list of found documents.</returns>
    public async Task<ImmutableList<TEntity>> FindAsync(Action<SearchRequestDescriptor<TEntity>> configureRequest)
    {
        var searchResponse = await _client.SearchAsync(configureRequest);
        if (searchResponse.IsValidResponse && searchResponse.ApiCallDetails.HttpStatusCode == 200)
            return [.. searchResponse.Documents];

        return [];
    }

}
