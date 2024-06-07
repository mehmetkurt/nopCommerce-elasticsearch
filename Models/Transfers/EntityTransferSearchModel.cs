using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Models.Transfers;

/// <summary>
/// Represents a search model for entity transfers.
/// </summary>
public record EntityTransferSearchModel : BaseSearchModel
{
    /// <summary>
    /// Gets or sets the list of entity names to search for.
    /// </summary>
    [NopResourceDisplayName($"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Search.EntityName")]
    public List<string> SearchEntityNames { get; set; }

    /// <summary>
    /// Gets or sets the ID of the search criteria for ignored transfers.
    /// </summary>
    [NopResourceDisplayName($"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Search.Ignored")]
    public int SearchIgnoredId { get; set; }

    /// <summary>
    /// Gets or sets the list of operation type IDs to search for.
    /// </summary>
    [NopResourceDisplayName($"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Search.OperationType")]
    public List<int> SearchOperationTypeIds { get; set; }

    /// <summary>
    /// Gets or sets the available entity names for selection.
    /// </summary>
    public List<SelectListItem> AvailableEntityNames { get; set; } = [];

    /// <summary>
    /// Gets or sets the available options for searching ignored transfers.
    /// </summary>
    public List<SelectListItem> AvailableIgnoreOptions { get; set; } = [];

    /// <summary>
    /// Gets or sets the available operation types for selection.
    /// </summary>
    public List<SelectListItem> AvailableOperationTypes { get; set; } = [];
}
