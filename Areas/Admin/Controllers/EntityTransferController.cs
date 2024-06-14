using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.SearchProvider.Elasticsearch.Areas.Admin.Models.Transfers;
using Nop.Plugin.SearchProvider.Elasticsearch.Factories;
using Nop.Plugin.SearchProvider.Elasticsearch.Services;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Areas.Admin.Controllers;

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
    /// Handles the HTTP GET request for listing Entity Transfer objects.
    /// </summary>
    /// <returns>An asynchronous operation representing the view displaying the listed Entity Transfer objects.</returns>
    [HttpGet]
    public async Task<IActionResult> List()
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            return AccessDeniedView();

        var model = await _entityTransferModelFactory.PrepareEntityTransferSearchModelAsync(new EntityTransferSearchModel());
        return View(model);
    }

    /// <summary>
    /// Handles the HTTP POST request for listing Entity Transfer objects based on the provided search criteria.
    /// </summary>
    /// <param name="searchModel">The search model containing the criteria for listing Entity Transfers.</param>
    /// <returns>An asynchronous operation representing the JSON result containing the listed Entity Transfer objects.</returns>
    [HttpPost]
    public async Task<IActionResult> List(EntityTransferSearchModel searchModel)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            return await AccessDeniedJsonAsync();

        // Prepare model
        var model = await _entityTransferModelFactory.PrepareEntityTransferListModelAsync(searchModel);

        return Json(model);
    }

    /// <summary>
    /// Handles the HTTP POST request for updating an Entity Transfer object.
    /// </summary>
    /// <param name="model">The model containing the data for updating the Entity Transfer.</param>
    /// <returns>An asynchronous operation representing the result of the update operation.</returns>
    [HttpPost]
    public async Task<IActionResult> Update(EntityTransferModel model)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            return await AccessDeniedJsonAsync();

        var entityTransfer = await _entityTransferService.GetEntityTransferByIdAsync(model.Id);
        if (entityTransfer == null)
            return new NullJsonResult();

        entityTransfer.Ignored = model.Ignored;
        entityTransfer.UpdatedDateUtc = DateTime.UtcNow;
        await _entityTransferService.UpdateEntityTransferAsync(entityTransfer);

        return new NullJsonResult();
    }

    /// <summary>
    /// Handles the HTTP POST request for deleting selected Entity Transfer objects.
    /// </summary>
    /// <param name="selectedIds">A collection of IDs representing the Entity Transfer objects to be deleted.</param>
    /// <returns>An asynchronous operation representing the JSON result indicating the success of the deletion operation.</returns>
    [HttpPost]
    public async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            return await AccessDeniedJsonAsync();

        if (selectedIds == null || selectedIds.Count == 0)
            return NoContent();

        await _entityTransferService.DeleteEntityTransfersAsync(selectedIds.ToList());

        return Json(new { Result = true });
    }

    #endregion
}
