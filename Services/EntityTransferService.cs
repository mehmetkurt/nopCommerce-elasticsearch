using LinqToDB;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Plugin.SearchProvider.Elasticsearch.Data.Domain;
using System.Collections.Immutable;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Services;

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
    /// Asynchronously retrieves a list of entities of type <typeparamref name="TEntity"/> that are not marked as ignored 
    /// in the EntityTransfer table and optionally match a specified operation type.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="pageIndex">The index of the page to retrieve. Default is 0.</param>
    /// <param name="pageSize">The maximum number of entities to retrieve per page. Default is int.MaxValue.</param>
    /// <param name="getOnlyTotalCount">A boolean indicating whether to retrieve only the total count of entities without fetching the actual entities. Default is false.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an immutable list of entities that are not transferred based on the specified criteria.</returns>
    public virtual async Task<ImmutableList<TEntity>> GetNonTransferredEntitiesAsync<TEntity>(int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false) where TEntity : BaseEntity
    {
        var entityRepository = EngineContext.Current.Resolve<IRepository<TEntity>>();

        var entities = await entityRepository.GetAllPagedAsync(query =>
        {
            var transferQuery = _entityTransferRepository.Table;
            var entityName = typeof(TEntity).Name;

            // LINQ query to join entity and entityTransfer tables, and filter entities that are not transferred
            query = from entity in query
                    join entityTransfer in transferQuery
                        on new { EntityName = entityName, EntityId = entity.Id }
                        equals new { entityTransfer.EntityName, entityTransfer.EntityId }
                        into transfers
                    from transfer in transfers.DefaultIfEmpty()
                    where transfer == null
                    select entity;

            return query;
        }, pageIndex, pageSize, getOnlyTotalCount);

        return [.. entities];
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
    #endregion
}
