using Nop.Plugin.SearchProvider.Elasticsearch.Areas.Admin.Models.Transfers;
using Nop.Plugin.SearchProvider.Elasticsearch.Data.Domain;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Factories;

/// <summary>
/// Represents a factory for preparing models related to entity transfers.
/// </summary>
public interface IEntityTransferModelFactory
{
    /// <summary>
    /// Prepares the entity transfer search model asynchronously.
    /// </summary>
    /// <param name="searchModel">The entity transfer search model.</param>
    /// <returns>The prepared entity transfer search model.</returns>
    Task<EntityTransferSearchModel> PrepareEntityTransferSearchModelAsync(EntityTransferSearchModel searchModel);

    /// <summary>
    /// Prepares the entity transfer list model asynchronously.
    /// </summary>
    /// <param name="searchModel">The entity transfer search model.</param>
    /// <returns>The prepared entity transfer list model.</returns>
    Task<EntityTransferListModel> PrepareEntityTransferListModelAsync(EntityTransferSearchModel searchModel);

    /// <summary>
    /// Prepares the entity transfer model asynchronously.
    /// </summary>
    /// <param name="model">The entity transfer model.</param>
    /// <param name="entityTransfer">The entity transfer.</param>
    /// <param name="excludeProperties">A value indicating whether to exclude properties.</param>
    /// <returns>The prepared entity transfer model.</returns>
    Task<EntityTransferModel> PrepareEntityTransferModelAsync(EntityTransferModel model, EntityTransfer entityTransfer, bool excludeProperties = false);
}
