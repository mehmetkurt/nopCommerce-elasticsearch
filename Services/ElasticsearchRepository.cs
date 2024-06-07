using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Nop.Core;
using Nop.Data;
using Nop.Plugin.SearchProvider.Elasticsearch.Services;
using Nop.Services.Logging;
using System.Collections.Immutable;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Repositories;

/// <summary>
/// Generic repository implementation for Elasticsearch operations.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public class ElasticsearchRepository<TEntity> : IElasticsearchRepository<TEntity> where TEntity : BaseEntity
{
    #region Fields
    private readonly ILogger _logger;
    private readonly IRepository<TEntity> _entityRepository;
    private readonly ElasticsearchClient _client;
    private readonly IElasticsearchConnectionManager _elasticsearchConnectionManager;
    private readonly string _indexName = typeof(TEntity).Name.ToLowerInvariant();
    #endregion

    #region Ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticsearchRepository{TEntity}"/> class.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="entityRepository"></param>
    /// <param name="elasticsearchConnectionManager"></param>
    public ElasticsearchRepository
    (
        ILogger logger,
        IRepository<TEntity> entityRepository,
        IElasticsearchConnectionManager elasticsearchConnectionManager
    )
    {
        _logger = logger;
        _entityRepository = entityRepository;
        _elasticsearchConnectionManager = elasticsearchConnectionManager;
        _client = _elasticsearchConnectionManager.GetClientAsync().GetAwaiter().GetResult();
    }
    #endregion

    #region Methods

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
        var response = await _client.CreateAsync(entity, _indexName, entity.Id);

        if (!response.IsValidResponse)
        {
            throw new Exception($"Failed to insert document. Error: {response.DebugInformation}");
        }
    }

    /// <summary>
    /// Performs a bulk operation asynchronously by adding a collection of specified entity objects as documents into Elasticsearch. If a custom request descriptor is not provided, it defaults to using the specified index name to add documents to the index.
    /// </summary>
    /// <param name="entities">The collection of entity objects to be added.</param>
    /// <param name="requestDescriptor">An optional custom action to configure the bulk request descriptor. If not specified, the default action adds documents to the index specified by the `_indexName` field.</param>
    /// <returns>A task representing the asynchronous bulk operation, returning a <see cref="BulkResponse"/> object representing the result of the operation.</returns>
    public async Task<BulkResponse> BulkAsync(IEnumerable<TEntity> entities, Action<BulkRequestDescriptor> requestDescriptor = null)
    {
        requestDescriptor ??= descriptor =>
        {
            descriptor.Index(_indexName).IndexMany(entities);
        };

        return await _client.BulkAsync(requestDescriptor);
    }

    /// <summary>
    /// Asynchronously performs a bulk operation on the specified collection of entities.
    /// </summary>
    /// <param name="entities">The collection of entities to perform the bulk operation on.</param>
    /// <param name="onNext">An optional action to handle the bulk operation response.</param>
    /// <param name="onError">An optional action to handle any errors that occur during the bulk operation.</param>
    /// <param name="onCompleted">An optional action to handle the completion of the bulk operation.</param>
    /// <param name="requestDescriptor">An optional action to configure the bulk request descriptor.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task BulkAllAsync(
        IEnumerable<TEntity> entities,
        Action<BulkAllResponse> onNext = null,
        Action<Exception> onError = null,
        Action onCompleted = null,
        Action<BulkAllRequestDescriptor<TEntity>> requestDescriptor = null)
    {
        requestDescriptor ??= descriptor =>
        {
            descriptor
                .Index(_indexName)
                .BackOffRetries(2)
                .BackOffTime("30s")
                .RefreshOnCompleted()
                .MaxDegreeOfParallelism(4)
                .Size(100);
        };

        var bulkAllObservable = _client.BulkAll(entities, requestDescriptor);

        var completionSource = new TaskCompletionSource<bool>();
        var observer = new BulkAllObserver(
            onNext: o =>
            {
                onNext?.Invoke(o);
            },
            onError: async e =>
            {
                onError?.Invoke(e);
                await _logger.ErrorAsync(e.Message, e);
                completionSource.SetException(e);
            },
            onCompleted: () =>
            {
                onCompleted?.Invoke();
                completionSource.SetResult(true);
            }
        );

        var subscribe = bulkAllObservable.Subscribe(observer);

        await completionSource.Task;
    }

    /// <summary>
    /// Updates the entity in Elasticsearch asynchronously.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    public async Task UpdateAsync(TEntity entity)
    {
        var response = await _client.UpdateAsync<TEntity, TEntity>(_indexName, entity.Id.ToString(), u => u.Doc(entity));

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
        var response = await _client.DeleteAsync<TEntity>(_indexName, entity.Id.ToString());

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
    #endregion
}
