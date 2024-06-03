using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Nop.Core;
using System.Collections.Immutable;

namespace Nop.Plugin.Misc.Elasticsearch.Repositories;

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
