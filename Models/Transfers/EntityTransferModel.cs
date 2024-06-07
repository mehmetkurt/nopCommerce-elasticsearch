using Nop.Plugin.SearchProvider.Elasticsearch.Data.Domain;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Models.Transfers;

/// <summary>
/// Represents the model for transferring entities.
/// </summary>
public record EntityTransferModel : BaseNopEntityModel
{
    /// <summary>
    /// Gets or sets the name of the entity.
    /// </summary>
    [NopResourceDisplayName($"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Fields.EntityName")]
    public string EntityName { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the entity.
    /// </summary>
    [NopResourceDisplayName($"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Fields.EntityId")]
    public int EntityId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the transfer is ignored.
    /// </summary>
    [NopResourceDisplayName($"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Fields.Ignored")]
    public bool Ignored { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the operation type.
    /// </summary>
    [NopResourceDisplayName($"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Fields.OperationType")]
    public int OperationTypeId { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the transfer was created (in UTC).
    /// </summary>
    [NopResourceDisplayName($"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Fields.CreatedDateUtc")]
    public DateTime CreatedDateUtc { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the transfer was last updated (in UTC).
    /// </summary>
    [NopResourceDisplayName($"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Fields.UpdatedDateUtc")]
    public DateTime UpdatedDateUtc { get; set; }

    /// <summary>
    /// Gets or sets the name of the operation type.
    /// </summary>
    [NopResourceDisplayName($"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Fields.OperationType")]
    public string OperationTypeName { get; set; }

    /// <summary>
    /// Gets or sets the operation type.
    /// </summary>
    public OperationType OperationType
    {
        get => (OperationType)OperationTypeId;
        set => OperationTypeId = (int)value;
    }
}
