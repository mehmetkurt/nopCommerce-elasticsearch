using Nop.Core;
using Nop.Plugin.SearchProvider.Elasticsearch.Data.Domain;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Services;

/// <summary>
/// Defines methods to manage entity transfers asynchronously. Implementations of this interface provide functionality to retrieve, insert, update, and delete entity transfers, as well as methods to retrieve non-transferred entities and entity transfers associated with specific entities.
/// </summary>
public interface IEntityTransferService
{
    /// <summary>
    /// Retrieves an entity transfer record for a specific entity type and ID.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entityId">The identifier of the entity.</param>
    /// <param name="entityName">The name of the entity.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the entity transfer record, or null if no matching record is found.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="entityId"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="entityName"/> is null or empty.</exception>
    /// <remarks>
    /// This method retrieves an entity transfer record from the cache if available, or from the repository if not cached.
    /// The cache key is based on the entity ID and name, ensuring unique cache entries for different entities.
    /// </remarks>
    Task<EntityTransfer> GetEntityTransferAsync<TEntity>(int entityId, string entityName) where TEntity : BaseEntity;

    /// <summary>
    /// Asynchronously retrieves an <see cref="EntityTransfer"/> object from the specified entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entity">The entity from which to retrieve the entity transfer.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="EntityTransfer"/> object.</returns>
    /// <exception cref="ArgumentException">Thrown when the Id of the entity is zero or negative.</exception>
    Task<EntityTransfer> GetEntityTransferFromEntityAsync<TEntity>(TEntity entity) where TEntity : BaseEntity;

    /// <summary>
    /// Asynchronously retrieves a list of <see cref="EntityTransfer"/> objects from the specified entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entity">The entity from which to retrieve the entity transfers.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="EntityTransfer"/> objects.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the entity is null.</exception>
    Task<IList<EntityTransfer>> GetEntityTransfersFromEntityAsync<TEntity>(TEntity entity) where TEntity : BaseEntity;

    /// <summary>
    /// Asynchronously retrieves a paged list of entities of type <typeparamref name="TEntity"/> that are not marked as ignored 
    /// in the EntityTransfer table and optionally match a specified operation type.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="pageIndex">The index of the page to retrieve. Default is 0.</param>
    /// <param name="pageSize">The maximum number of entities to retrieve per page. Default is int.MaxValue.</param>
    /// <param name="getOnlyTotalCount">A boolean indicating whether to retrieve only the total count of entities without fetching the actual entities. Default is false.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a paged list of entities that are not transferred based on the specified criteria.</returns>
    Task<IPagedList<TEntity>> GetNonTransferredEntitiesAsync<TEntity>(int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false) where TEntity : BaseEntity;

    /// <summary>
    /// Searches for entity transfers based on specified criteria.
    /// </summary>
    /// <param name="entityNames">List of entity names to filter by (optional).</param>
    /// <param name="operationTypeIds">List of operation type IDs to filter by (optional).</param>
    /// <param name="overrideIgnored">Flag to filter by Ignored status (optional).</param>
    /// <param name="pageIndex">Index of the page to retrieve (default is 0).</param>
    /// <param name="pageSize">Size of the page to retrieve (default is int.MaxValue).</param>
    /// <param name="getOnlyTotalCount">Flag to indicate if only the total count is needed (default is false).</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a paged list of entity transfers.</returns>
    /// <remarks>
    /// This method constructs a query to filter entity transfers based on the provided criteria.
    /// If no criteria are provided, it returns all entity transfers.
    /// </remarks>
    Task<IPagedList<EntityTransfer>> SearchEntityTransfersAsync(List<string> entityNames = null, List<int> operationTypeIds = null, bool? overrideIgnored = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

    /// <summary>
    /// Gets an <see cref="EntityTransfer"/> by its identifier.
    /// </summary>
    /// <param name="entityTransferId">The identifier of the entity transfer.</param>
    /// <param name="includeIgnored">Whether to include ignored transfers.</param>
    /// <returns>The entity transfer.</returns>
    Task<EntityTransfer> GetEntityTransferByIdAsync(int entityTransferId, bool includeIgnored = false);

    /// <summary>
    /// Inserts a new <see cref="EntityTransfer"/>.
    /// </summary>
    /// <param name="entityTransfer">The entity transfer to insert.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the entity transfer is null.</exception>
    Task InsertEntityTransferAsync(EntityTransfer entityTransfer);

    /// <summary>
    /// Asynchronously inserts a collection of <see cref="EntityTransfer"/> instances into the database.
    /// </summary>
    /// <param name="entityTransfers">The collection of entity transfers to insert.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the collection of entity transfers is null.</exception>
    Task InsertEntityTransfersAsync(IList<EntityTransfer> entityTransfers);

    /// <summary>
    /// Updates an existing <see cref="EntityTransfer"/>.
    /// </summary>
    /// <param name="entityTransfer">The entity transfer to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the entity transfer is null.</exception>
    Task UpdateEntityTransferAsync(EntityTransfer entityTransfer);

    /// <summary>
    /// Updates entity transfers asynchronously.
    /// </summary>
    /// <param name="entityTransfers">The list of entity transfers to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateEntityTransfersAsync(IList<EntityTransfer> entityTransfers);

    /// <summary>
    /// Deletes an existing <see cref="EntityTransfer"/>.
    /// </summary>
    /// <param name="entityTransfer">The entity transfer to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the entity transfer is null.</exception>
    Task DeleteEntityTransferAsync(EntityTransfer entityTransfer);

    /// <summary>
    /// Deletes entity transfers asynchronously.
    /// </summary>
    /// <param name="entityTransferIds">The list of entity transfer IDs to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteEntityTransfersAsync(List<int> entityTransferIds);

    /// <summary>
    /// Retrieves a list of existing entity names asynchronously.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous operation. The task result is a list of strings containing existing entity names.
    /// </returns>
    Task<List<string>> GetExistingEntityNames();
}
