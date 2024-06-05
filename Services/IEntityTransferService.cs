using Nop.Core;
using Nop.Plugin.SearchProvider.Elasticsearch.Data.Domain;
using System.Collections.Immutable;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Services;

public interface IEntityTransferService
{
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
    /// Asynchronously retrieves a list of entities of type <typeparamref name="TEntity"/> that are not marked as ignored 
    /// in the EntityTransfer table and optionally match a specified operation type.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="operationType">An optional parameter to filter entities by a specific operation type. If null, the filter is not applied.</param>
    /// <param name="pageIndex">The index of the page to retrieve. Default is 0.</param>
    /// <param name="pageSize">The maximum number of entities to retrieve per page. Default is int.MaxValue.</param>
    /// <param name="getOnlyTotalCount">A boolean indicating whether to retrieve only the total count of entities without fetching the actual entities. Default is false.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an immutable list of entities that are not transferred based on the specified criteria.</returns>
    Task<ImmutableList<TEntity>> GetNonTransferredEntitiesAsync<TEntity>(OperationType operationType = OperationType.Inserted, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false) where TEntity : BaseEntity;

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
