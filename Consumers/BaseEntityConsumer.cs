using Nop.Core;
using Nop.Core.Events;
using Nop.Plugin.SearchProvider.Elasticsearch.Data.Domain;
using Nop.Plugin.SearchProvider.Elasticsearch.Services;
using Nop.Services.Events;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Consumers;

/// <summary>
/// Represents a base entity consumer that handles entity events such as insertion, update, and deletion.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public abstract partial class BaseEntityConsumer<TEntity> :
    IConsumer<EntityInsertedEvent<TEntity>>,
    IConsumer<EntityUpdatedEvent<TEntity>>,
    IConsumer<EntityDeletedEvent<TEntity>>
    where TEntity : BaseEntity
{
    #region Fields
    private readonly IEntityTransferService _entityTransferService;
    #endregion

    #region Ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseEntityConsumer{TEntity}"/> class.
    /// </summary>
    /// <param name="entityTransferService">The entity transfer service.</param>
    protected BaseEntityConsumer(IEntityTransferService entityTransferService)
    {
        _entityTransferService = entityTransferService;
    }
    #endregion

    #region Utilities
    /// <summary>
    /// Prepares the entity for transfer based on the operation type.
    /// </summary>
    /// <param name="entity">The entity to be transferred.</param>
    /// <param name="operationType">The type of the operation being performed.</param>
    /// <returns>A task that represents the asynchronous operation, containing a boolean result indicating if the preparation was successful.</returns>
    protected virtual async Task<bool> PrepareToTransferAsync(TEntity entity, OperationType operationType)
    {
        return await Task.FromResult(false);
    }
    #endregion

    #region Methods

    /// <summary>
    /// Handles the entity inserted event.
    /// </summary>
    /// <param name="eventMessage">The event message containing the entity data.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async Task HandleEventAsync(EntityInsertedEvent<TEntity> eventMessage)
    {
        await Task.CompletedTask;
    }

    /// <summary>
    /// Handles the entity updated event.
    /// </summary>
    /// <param name="eventMessage">The event message containing the entity data.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async Task HandleEventAsync(EntityUpdatedEvent<TEntity> eventMessage)
    {
        await Task.CompletedTask;
    }

    /// <summary>
    /// Handles the entity deleted event.
    /// </summary>
    /// <param name="eventMessage">The event message containing the entity data.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async Task HandleEventAsync(EntityDeletedEvent<TEntity> eventMessage)
    {
        await Task.CompletedTask;
    }

    #endregion
}
