using Nop.Core.Caching;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Services;
public static class EntityTransferServiceDefaults
{
    /// <summary>
    /// Gets the cache key for an entity transfer, formatted with specific identifiers.
    /// This cache key is used to store and retrieve cached entity transfer data.
    /// The format of the cache key is: "Nop.elasticsearch.entitytransfer.{0}-{1}"
    /// <paramref name="entityId"/> represents the entity ID, and <paramref name="entityName"/> represents the entity name.
    /// </summary>
    public static CacheKey EntityTransferCacheKey => new("Nop.elasticsearch.entitytransfer.{0}-{1}", EntityTransferPrefix);

    /// <summary>
    /// Gets the cache key for entity transfers, formatted with a specific identifier.
    /// This cache key is used to store and retrieve cached entity transfer data.
    /// The format of the cache key is: "Nop.elasticsearch.entitytransfer.{0}"
    /// </summary>
    public static CacheKey EntityTransfersCacheKey => new("Nop.elasticsearch.entitytransfer.", EntityTransferPrefix);

    /// <summary>
    /// Gets the prefix used for entity transfer cache keys.
    /// This prefix is used to group related cache keys together.
    /// The format of the prefix is: "Nop.elasticsearch.entitytransfer."
    /// </summary>
    public static string EntityTransferPrefix => "Nop.elasticsearch.entitytransfer.";

}
