namespace Nop.Plugin.SearchProvider.Elasticsearch.Data.Domain;

/// <summary>
/// Represents the type of operation performed on an entity.
/// </summary>
public enum OperationType
{
    /// <summary>
    /// Indicates that an entity has been inserted.
    /// </summary>
    Inserted = 10,

    /// <summary>
    /// Indicates that an entity has been updated.
    /// </summary>
    Updated = 20,

    /// <summary>
    /// Indicates that an entity has been deleted.
    /// </summary>
    Deleted = 30
}
