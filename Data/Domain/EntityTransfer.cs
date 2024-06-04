using Nop.Core;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Data.Domain;

/// <summary>
/// Represents an entity transfer record.
/// </summary>
public class EntityTransfer : BaseEntity
{
    /// <summary>
    /// Gets or sets the name of the entity.
    /// </summary>
    public string EntityName { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the entity.
    /// </summary>
    public int EntityId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the transfer is ignored.
    /// </summary>
    public bool Ignored { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the operation type.
    /// </summary>
    public int OperationTypeId { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the transfer was created (in UTC).
    /// </summary>
    public DateTime CreatedDateUtc { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the transfer was last updated (in UTC).
    /// </summary>
    public DateTime UpdatedDateUtc { get; set; }

    /// <summary>
    /// Gets or sets the operation type.
    /// </summary>
    public OperationType OperationType
    {
        get => (OperationType)OperationTypeId;
        set => OperationTypeId = (int)value;
    }
}
