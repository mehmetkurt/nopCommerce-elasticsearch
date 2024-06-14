using Nop.Web.Framework.Models;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Areas.Admin.Models.Transfers;

/// <summary>
/// Represents a list model for entity transfers.
/// </summary>
public record EntityTransferListModel : BasePagedListModel<EntityTransferModel>
{
}
