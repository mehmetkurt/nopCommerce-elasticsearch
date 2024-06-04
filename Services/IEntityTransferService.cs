using Nop.Core;
using Nop.Plugin.SearchProvider.Elasticsearch.Data.Domain;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Services;

public interface IEntityTransferService
{
    /// <summary>
    /// Gets an existing <see cref="EntityTransfer"/> for the specified entity, if it exists.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entity">The entity to find the transfer for.</param>
    /// <returns>The existing entity transfer, if found; otherwise, null.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the entity is null.</exception>
    Task<EntityTransfer> GetExistsEntityTransferFromEntityAsync<TEntity>(TEntity entity) where TEntity : BaseEntity;

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
    /// Updates an existing <see cref="EntityTransfer"/>.
    /// </summary>
    /// <param name="entityTransfer">The entity transfer to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the entity transfer is null.</exception>
    Task UpdateEntityTransferAsync(EntityTransfer entityTransfer);

    /// <summary>
    /// Deletes an existing <see cref="EntityTransfer"/>.
    /// </summary>
    /// <param name="entityTransfer">The entity transfer to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the entity transfer is null.</exception>
    Task DeleteEntityTransferAsync(EntityTransfer entityTransfer);
}
