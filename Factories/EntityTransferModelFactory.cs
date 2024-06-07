using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.SearchProvider.Elasticsearch.Data.Domain;
using Nop.Plugin.SearchProvider.Elasticsearch.Models.Transfers;
using Nop.Plugin.SearchProvider.Elasticsearch.Services;
using Nop.Services;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Factories;

/// <summary>
/// Represents a factory for preparing models related to entity transfers.
/// </summary>
public class EntityTransferModelFactory : IEntityTransferModelFactory
{
    #region Fields

    private readonly ILocalizationService _localizationService;
    private readonly IEntityTransferService _entityTransferService;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityTransferModelFactory"/> class.
    /// </summary>
    /// <param name="localizationService">The localization service.</param>
    /// <param name="entityTransferService">The entity transfer service.</param>
    public EntityTransferModelFactory
    (
        ILocalizationService localizationService,
        IEntityTransferService entityTransferService
    )
    {
        _localizationService = localizationService;
        _entityTransferService = entityTransferService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepares the entity transfer search model asynchronously.
    /// </summary>
    /// <param name="searchModel">The entity transfer search model.</param>
    /// <returns>The prepared entity transfer search model.</returns>
    public async Task<EntityTransferSearchModel> PrepareEntityTransferSearchModelAsync(EntityTransferSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        // Operation Types
        searchModel.AvailableOperationTypes = (await OperationType.Inserted.ToSelectListAsync(false)).Select(s => s).ToList();
        searchModel.AvailableOperationTypes.Insert(0, new()
        {
            Text = await _localizationService.GetResourceAsync($"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Search.OperationType.All"),
            Value = "0"
        });

        if (searchModel.AvailableOperationTypes.Count != 0)
        {
            if (searchModel.SearchOperationTypeIds?.Any() ?? false)
            {
                var ids = searchModel.SearchOperationTypeIds.Select(id => id.ToString());
                var operationTypeItems = searchModel.AvailableOperationTypes.Where(operationTypeItem => ids.Contains(operationTypeItem.Value)).ToList();
                foreach (var operationTypeItem in operationTypeItems)
                    operationTypeItem.Selected = true;
            }
            else
                searchModel.AvailableOperationTypes.FirstOrDefault().Selected = true;
        }

        // Entity Names
        searchModel.AvailableEntityNames = (await _entityTransferService.GetExistingEntityNames()).Select(s => new SelectListItem
        {
            Text = s,
            Value = s,
        }).ToList();

        searchModel.AvailableEntityNames.Insert(0, new()
        {
            Text = await _localizationService.GetResourceAsync($"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Search.EntityName.All"),
            Value = "0",
        });

        if (searchModel.AvailableEntityNames.Count != 0)
        {
            if (searchModel.SearchEntityNames?.Any() ?? false)
            {
                var ids = searchModel.SearchEntityNames.Select(id => id);
                var entityNameItems = searchModel.AvailableEntityNames.Where(entityNameItem => ids.Contains(entityNameItem.Value)).ToList();
                foreach (var entityNameItem in entityNameItems)
                    entityNameItem.Selected = true;
            }
            else
                searchModel.AvailableEntityNames.FirstOrDefault().Selected = true;
        }

        // Ignore Options
        searchModel.AvailableIgnoreOptions =
        [
            new() {
                Value = "0",
                Text = await _localizationService.GetResourceAsync($"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Search.Ignored.All")
            },
            new() {
                Value = "1",
                Text = await _localizationService.GetResourceAsync($"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Search.Ignored.IgnoredOnly")
            },
            new() {
                Value = "2",
                Text = await _localizationService.GetResourceAsync($"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Search.Ignored.NotIgnoredOnly")
            }
        ];

        searchModel.SetGridPageSize();

        return searchModel;
    }

    /// <summary>
    /// Prepares the entity transfer list model asynchronously.
    /// </summary>
    /// <param name="searchModel">The entity transfer search model.</param>
    /// <returns>The prepared entity transfer list model.</returns>
    public async Task<EntityTransferListModel> PrepareEntityTransferListModelAsync(EntityTransferSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        // Get parameters to filter entity transfers
        var entityNames = (searchModel.SearchEntityNames?.Contains("0") ?? true) ? null : searchModel.SearchEntityNames.ToList();
        var operationTypeIds = (searchModel.SearchOperationTypeIds?.Contains(0) ?? true) ? null : searchModel.SearchOperationTypeIds.ToList();
        var overrideIgnored = searchModel.SearchIgnoredId == 0 ? null : (bool?)(searchModel.SearchIgnoredId == 1);

        // Search entity transfers
        var entityTransfers = await _entityTransferService.SearchEntityTransfersAsync(
            entityNames: entityNames,
            operationTypeIds: operationTypeIds,
            overrideIgnored: overrideIgnored,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        // Prepare the entity transfer list model
        var model = await new EntityTransferListModel().PrepareToGridAsync(searchModel, entityTransfers, () =>
        {
            return entityTransfers.SelectAwait(async entityTransfer =>
            {
                var entityTransferModel = entityTransfer.ToModel<EntityTransferModel>();
                entityTransferModel.OperationTypeName = await _localizationService.GetLocalizedEnumAsync(entityTransfer.OperationType);
                return entityTransferModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepares the entity transfer model asynchronously.
    /// </summary>
    /// <param name="model">The entity transfer model.</param>
    /// <param name="entityTransfer">The entity transfer.</param>
    /// <param name="excludeProperties">A value indicating whether to exclude properties.</param>
    /// <returns>The prepared entity transfer model.</returns>
    public async Task<EntityTransferModel> PrepareEntityTransferModelAsync(EntityTransferModel model, EntityTransfer entityTransfer, bool excludeProperties = false)
    {
        if (entityTransfer != null)
        {
            if (model == null)
            {
                model = entityTransfer.ToModel<EntityTransferModel>();
                model.OperationTypeName = await _localizationService.GetLocalizedEnumAsync(entityTransfer.OperationType);
            }
        }

        return model;
    }

    #endregion
}
