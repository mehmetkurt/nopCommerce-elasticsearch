using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Nop.Core;
using System.Collections.Immutable;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Repositories;

/// <summary>
/// Generic repository for Elasticsearch operations.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public interface IElasticsearchRepository<TEntity> where TEntity : BaseEntity
{
    /// <summary>
    /// Gets the entity by its identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>The entity with the specified identifier.</returns>
    Task<TEntity> GetByIdAsync(int id);

    /// <summary>
    /// Inserts the entity into Elasticsearch.
    /// </summary>
    /// <param name="entity">The entity to insert.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task InsertAsync(TEntity entity);

    /// <summary>
    /// Performs a bulk operation asynchronously by adding a collection of specified entity objects as documents into Elasticsearch. If a custom request descriptor is not provided, it defaults to using the specified index name to add documents to the index.
    /// </summary>
    /// <param name="entities">The collection of entity objects to be added.</param>
    /// <param name="requestDescriptor">An optional custom action to configure the bulk request descriptor. If not specified, the default action adds documents to the index specified by the `_indexName` field.</param>
    /// <returns>A task representing the asynchronous bulk operation, returning a <see cref="BulkResponse"/> object representing the result of the operation.</returns>
    Task<BulkResponse> BulkAsync(IEnumerable<TEntity> entities, Action<BulkRequestDescriptor> requestDescriptor = null);

    /// <summary>
    /// Asynchronously performs a bulk operation on the specified collection of entities.
    /// </summary>
    /// <param name="entities">The collection of entities to perform the bulk operation on.</param>
    /// <param name="onNext">An optional action to handle the bulk operation response.</param>
    /// <param name="onError">An optional action to handle any errors that occur during the bulk operation.</param>
    /// <param name="onCompleted">An optional action to handle the completion of the bulk operation.</param>
    /// <param name="requestDescriptor">An optional action to configure the bulk request descriptor.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task BulkAllAsync(IEnumerable<TEntity> entities, Action<BulkAllResponse> onNext = null, Action<Exception> onError = null, Action onCompleted = null, Action<BulkAllRequestDescriptor<TEntity>> requestDescriptor = null);

    /// <summary>
    /// Updates the entity in Elasticsearch.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateAsync(TEntity entity);

    /// <summary>
    /// Deletes the entity from Elasticsearch.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAsync(TEntity entity);

    /// <summary>
    /// Finds entities in Elasticsearch that match the specified query asynchronously.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="indexName">Name of the index.</param>
    /// <param name="from">The starting index for the search results (default is 0).</param>
    /// <param name="size">The maximum number of search results to return (default is int.MaxValue).</param>
    /// <returns>A list of entities that match the query.</returns>
    Task<ImmutableList<TEntity>> FindAsync(Query query, string indexName = "", int from = 0, int size = int.MaxValue);

    /// <summary>
    /// Finds and lists documents in Elasticsearch by configuring a specific search request asynchronously.
    /// </summary>
    /// <param name="configureRequest">An action to configure the search request descriptor.</param>
    /// <returns>A list of found documents.</returns>
    Task<ImmutableList<TEntity>> FindAsync(Action<SearchRequestDescriptor<TEntity>> configureRequest);
}
