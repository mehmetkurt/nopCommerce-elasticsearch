using Nop.Core;
using Nop.Core.Caching;
using Nop.Data;
using Nop.Plugin.SearchProvider.Elasticsearch.Data.Domain;

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
    /// Gets an existing <see cref="EntityTransfer"/> for the specified entity, if it exists.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entity">The entity to find the transfer for.</param>
    /// <returns>The existing entity transfer, if found; otherwise, null.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the entity is null.</exception>
    public virtual async Task<EntityTransfer> GetExistsEntityTransferFromEntityAsync<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(entity);

        var entityName = typeof(TEntity).Name;
        var entityId = entity.Id;

        return await _shortTermCacheManager.GetAsync(
            async () =>
            {
                var query = await _entityTransferRepository.GetAllAsync(q =>
                    q.Where(et => et.EntityName == entityName && et.EntityId == entityId)
                );

                return query.FirstOrDefault();
            }, EntityTransferServiceDefaults.EntityTransfersCacheKey, entityId);
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
