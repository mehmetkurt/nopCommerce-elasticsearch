using Nop.Plugin.SearchProvider.Elasticsearch.Data.Domain;
using Nop.Services.Caching;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Services.Caching;

/// <summary>
/// Represents a entity transfer cache event consumer
/// </summary>
public partial class EntityTransferCacheEventConsumer : CacheEventConsumer<EntityTransfer>
{
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(EntityTransfer entity)
    {
        await RemoveAsync(EntityTransferServiceDefaults.EntityTransfersCacheKey, entity.EntityId, entity.OperationTypeId);
    }
}