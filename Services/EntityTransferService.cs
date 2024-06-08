using LinqToDB;
using Microsoft.CodeAnalysis;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Plugin.SearchProvider.Elasticsearch.Data.Domain;
using System.Diagnostics.CodeAnalysis;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Services;

/// <summary>
/// Provides functionality to manage entity transfers asynchronously. It includes methods to retrieve, insert, update, and delete entity transfers, as well as methods to retrieve non-transferred entities and entity transfers associated with specific entities.
/// </summary>
public class EntityTransferService : IEntityTransferService
{
    #region Fields
    private readonly IShortTermCacheManager _shortTermCacheManager;
    private readonly IRepository<EntityTransfer> _entityTransferRepository;
    #endregion

    #region Ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="EntityTransferService"/> class.
    /// </summary>
    /// <param name="shortTermCacheManager">The cache manager.</param>
    /// <param name="entityTransferRepository">The entity transfer repository.</param>
    public EntityTransferService(
        IShortTermCacheManager shortTermCacheManager,
        IRepository<EntityTransfer> entityTransferRepository)
    {
        _shortTermCacheManager = shortTermCacheManager;
        _entityTransferRepository = entityTransferRepository;
    }
    #endregion

    #region Methods
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
    public virtual async Task<EntityTransfer> GetEntityTransferAsync<TEntity>([NotNull] int entityId, [NotNull] string entityName) where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(entityId, nameof(entityId));
        ArgumentException.ThrowIfNullOrEmpty(entityName, nameof(entityName));

        var entityTransfers = await _shortTermCacheManager.GetAsync(async () =>
        {
            var query = await _entityTransferRepository.GetAllAsync(q =>
            {
                q = q.Where(p => p.EntityId == entityId);
                q = q.Where(et => string.Compare(et.EntityName, entityName, StringComparison.OrdinalIgnoreCase) == 0);

                return q;
            });

            return await query.ToListAsync();
        }, EntityTransferServiceDefaults.EntityTransferCacheKey, entityId, entityName);

        return entityTransfers.FirstOrDefault();
    }

    /// <summary>
    /// Asynchronously retrieves an <see cref="EntityTransfer"/> object from the specified entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entity">The entity from which to retrieve the entity transfer.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="EntityTransfer"/> object.</returns>
    /// <exception cref="ArgumentException">Thrown when the Id of the entity is zero or negative.</exception>
    public virtual async Task<EntityTransfer> GetEntityTransferFromEntityAsync<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        if (entity.Id <= 0)
            throw new ArgumentException($"{typeof(TEntity).Name}.Id cannot be zero or negative.");

        var entityTransfer = await GetEntityTransfersFromEntityAsync(entity);
        return entityTransfer.FirstOrDefault();
    }

    /// <summary>
    /// Asynchronously retrieves a list of <see cref="EntityTransfer"/> objects from the specified entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entity">The entity from which to retrieve the entity transfers.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="EntityTransfer"/> objects.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the entity is null.</exception>
    public virtual async Task<IList<EntityTransfer>> GetEntityTransfersFromEntityAsync<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(entity);

        var entityName = typeof(TEntity).Name;
        var entityId = entity.Id;

        return await _shortTermCacheManager.GetAsync(
            async () =>
            {
                var query = await _entityTransferRepository.GetAllAsync(q =>
                {
                    if (entityId > 0)
                        q.Where(p => p.EntityId == entityId);

                    q.Where(et => string.Compare(et.EntityName, entityName, StringComparison.OrdinalIgnoreCase) == 0);

                    return q;
                });

                return await query.ToListAsync();
            }, EntityTransferServiceDefaults.EntityTransfersCacheKey, entityId);
    }

    /// <summary>
    /// Asynchronously retrieves a paged list of entities of type <typeparamref name="TEntity"/> that are not marked as ignored 
    /// in the EntityTransfer table and optionally match a specified operation type.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="pageIndex">The index of the page to retrieve. Default is 0.</param>
    /// <param name="pageSize">The maximum number of entities to retrieve per page. Default is int.MaxValue.</param>
    /// <param name="getOnlyTotalCount">A boolean indicating whether to retrieve only the total count of entities without fetching the actual entities. Default is false.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a paged list of entities that are not transferred based on the specified criteria.</returns>
    public virtual async Task<IPagedList<TEntity>> GetNonTransferredEntitiesAsync<TEntity>(int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false) where TEntity : BaseEntity
    {
        var entityQuery = EngineContext.Current.Resolve<IRepository<TEntity>>().Table;
        var transferQuery = _entityTransferRepository.Table;
        var entityName = typeof(TEntity).Name;

        // LINQ query to join entity and entityTransfer tables, and filter entities that are not transferred
        var query = from entity in entityQuery
                    join entityTransfer in transferQuery
                        on new { EntityName = entityName, EntityId = entity.Id }
                        equals new { entityTransfer.EntityName, entityTransfer.EntityId }
                        into transfers
                    from transfer in transfers.DefaultIfEmpty()
                    where transfer == null
                    select entity;

        return await query.ToPagedListAsync(pageIndex, pageSize, getOnlyTotalCount);
    }

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
    public virtual async Task<IPagedList<EntityTransfer>> SearchEntityTransfersAsync(List<string> entityNames = null, List<int> operationTypeIds = null, bool? overrideIgnored = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
    {
        var query = _entityTransferRepository.Table;

        if (overrideIgnored.HasValue)
        {
            query = query.Where(p => p.Ignored == overrideIgnored.Value);
        }

        if (entityNames is not null)
        {
            if (entityNames.Count != 0)
            {
                query = from et in query
                        where entityNames.Contains(et.EntityName)
                        select et;
            }
        }

        if (operationTypeIds is not null)
        {
            if (operationTypeIds.Count != 0)
            {
                query = from et in query
                        where operationTypeIds.Contains(et.EntityId)
                        select et;
            }
        }

        return await query.ToPagedListAsync(pageIndex, pageSize, getOnlyTotalCount);
    }

    /// <summary>
    /// Gets an <see cref="EntityTransfer"/> by its identifier.
    /// </summary>
    /// <param name="entityTransferId">The identifier of the entity transfer.</param>
    /// <param name="includeIgnored">Whether to include ignored transfers.</param>
    /// <returns>The entity transfer.</returns>
    public virtual async Task<EntityTransfer> GetEntityTransferByIdAsync(int entityTransferId, bool includeIgnored = false)
    {
        return await _entityTransferRepository.GetByIdAsync(entityTransferId, cache => default, includeDeleted: false, useShortTermCache: true);
    }

    /// <summary>
    /// Inserts a new <see cref="EntityTransfer"/>.
    /// </summary>
    /// <param name="entityTransfer">The entity transfer to insert.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the entity transfer is
    /// <exception cref="ArgumentNullException">Thrown if the entity transfer is null.</exception>
    public virtual async Task InsertEntityTransferAsync(EntityTransfer entityTransfer)
    {
        ArgumentNullException.ThrowIfNull(entityTransfer);
        await _entityTransferRepository.InsertAsync(entityTransfer, false);
    }

    /// <summary>
    /// Asynchronously inserts a collection of <see cref="EntityTransfer"/> instances into the database.
    /// </summary>
    /// <param name="entityTransfers">The collection of entity transfers to insert.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the collection of entity transfers is null.</exception>
    public virtual async Task InsertEntityTransfersAsync(IList<EntityTransfer> entityTransfers)
    {
        ArgumentNullException.ThrowIfNull(entityTransfers);
        await _entityTransferRepository.InsertAsync(entityTransfers, false);
    }

    /// <summary>
    /// Updates an existing <see cref="EntityTransfer"/>.
    /// </summary>
    /// <param name="entityTransfer">The entity transfer to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the entity transfer is null.</exception>
    public virtual async Task UpdateEntityTransferAsync(EntityTransfer entityTransfer)
    {
        ArgumentNullException.ThrowIfNull(entityTransfer);
        await _entityTransferRepository.UpdateAsync(entityTransfer, false);
    }

    /// <summary>
    /// Updates entity transfers asynchronously.
    /// </summary>
    /// <param name="entityTransfers">The list of entity transfers to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual async Task UpdateEntityTransfersAsync(IList<EntityTransfer> entityTransfers)
    {
        ArgumentNullException.ThrowIfNull(entityTransfers);
        await _entityTransferRepository.UpdateAsync(entityTransfers, false);
    }

    /// <summary>
    /// Deletes an existing <see cref="EntityTransfer"/>.
    /// </summary>
    /// <param name="entityTransfer">The entity transfer to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the entity transfer is null.</exception>
    public virtual async Task DeleteEntityTransferAsync(EntityTransfer entityTransfer)
    {
        ArgumentNullException.ThrowIfNull(entityTransfer);

        await _entityTransferRepository.DeleteAsync(entityTransfer, false);
    }

    /// <summary>
    /// Deletes entity transfers asynchronously.
    /// </summary>
    /// <param name="entityTransferIds">The list of entity transfer IDs to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual async Task DeleteEntityTransfersAsync(List<int> entityTransferIds)
    {
        ArgumentNullException.ThrowIfNull(entityTransferIds);

        var entities = _entityTransferRepository.Table.Where(p => entityTransferIds.Contains(p.Id)).ToList();

        if (entities?.Any() ?? false)
            await _entityTransferRepository.DeleteAsync(entities);
    }


    /// <summary>
    /// Retrieves a list of unique entity names asynchronously from the EntityTransfer table.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous operation. The task result is a list of strings containing unique entity names.
    /// </returns>
    public Task<List<string>> GetExistingEntityNames()
    {
        return _entityTransferRepository.Table
            .Select(p => p.EntityName)
            .Distinct()
            .ToListAsync();
    }
    #endregion
}
