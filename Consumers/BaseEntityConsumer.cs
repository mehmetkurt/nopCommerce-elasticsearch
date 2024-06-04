using Nop.Core;
using Nop.Core.Events;
using Nop.Plugin.SearchProvider.Elasticsearch.Data.Domain;
using Nop.Plugin.SearchProvider.Elasticsearch.Services;
using Nop.Services.Events;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Consumers;

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
    protected BaseEntityConsumer
    (
        IEntityTransferService entityTransferService
    )
    {
        _entityTransferService = entityTransferService;
    }
    #endregion

    #region Utilities
    protected virtual async Task<bool> PrepareToTransferAsync(TEntity entity, OperationType operationType)
    {
        // Implementation details here
        return false;
    }
    #endregion

    #region Methods

    /// <summary>
    /// Handle entity inserted event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityInsertedEvent<TEntity> eventMessage)
    {
        // Implementation details here
    }

    /// <summary>
    /// Handle entity updated event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityUpdatedEvent<TEntity> eventMessage)
    {
        // Implementation details here
    }

    /// <summary>
    /// Handle entity deleted event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityDeletedEvent<TEntity> eventMessage)
    {
        // Implementation details here
    }

    #endregion
}
