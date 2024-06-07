using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.SearchProvider.Elasticsearch.Factories;
using Nop.Plugin.SearchProvider.Elasticsearch.Models.Transfers;
using Nop.Plugin.SearchProvider.Elasticsearch.Services;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Controllers;

/// <summary>
/// Represents a controller for configuring Elasticsearch settings in the admin area.
/// </summary>
[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
public class EntityTransferController : BasePluginController
{
    #region Fields

    private readonly IPermissionService _permissionService;
    private readonly IEntityTransferService _entityTransferService;
    private readonly IEntityTransferModelFactory _entityTransferModelFactory;

    #endregion

    #region Ctor

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityTransferController"/> class.
    /// </summary>
    /// <param name="permissionService">The permission service.</param>
    /// <param name="entityTransferService">The entity transfer service.</param>
    /// <param name="entityTransferModelFactory">The entity transfer model factory.</param>
    public EntityTransferController
    (
        IPermissionService permissionService,
        IEntityTransferService entityTransferService,
        IEntityTransferModelFactory entityTransferModelFactory
    )
    {
        _permissionService = permissionService;
        _entityTransferService = entityTransferService;
        _entityTransferModelFactory = entityTransferModelFactory;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Displays the list of entity transfers.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> List()
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            return AccessDeniedView();

        var model = await _entityTransferModelFactory.PrepareEntityTransferSearchModelAsync(new EntityTransferSearchModel());
        return View($"~/Plugins/{ElasticsearchDefaults.PluginSystemName}/Views/EntityTransfer/List.cshtml", model);
    }

    /// <summary>
    /// Handles the POST request for retrieving entity transfer data.
    /// </summary>
    [HttpPost]
    public virtual async Task<IActionResult> List(EntityTransferSearchModel searchModel)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            return await AccessDeniedDataTablesJson();

        // Prepare model
        var model = await _entityTransferModelFactory.PrepareEntityTransferListModelAsync(searchModel);

        return Json(model);
    }

    /// <summary>
    /// Handles the POST request for deleting selected entity transfers.
    /// </summary>
    [HttpPost]
    public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            return await AccessDeniedDataTablesJson();

        if (selectedIds == null || selectedIds.Count == 0)
            return NoContent();

        await _entityTransferService.DeleteEntityTransfersAsync(selectedIds.ToList());

        return Json(new { Result = true });
    }

    #endregion
}
