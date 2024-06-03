using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Plugin.SearchProvider.Elasticsearch.Models;
using Nop.Plugin.SearchProvider.Elasticsearch.Settings;
using Nop.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
public class ElasticsearchController : BasePluginController
{
    private readonly IWorkContext _workContext;
    private readonly IStoreContext _storeContext;
    private readonly ISettingService _settingService;
    private readonly IPermissionService _permissionService;
    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;

    public ElasticsearchController
    (
        IWorkContext workContext,
        IStoreContext storeContext,
        ISettingService settingService,
        IPermissionService permissionService,
        ILocalizationService localizationService,
        INotificationService notificationService
    )
    {
        _workContext = workContext;
        _storeContext = storeContext;
        _settingService = settingService;
        _permissionService = permissionService;
        _localizationService = localizationService;
        _notificationService = notificationService;
    }

    #region Utilities
    public async Task PrepareAvailableConnectionTypesAsync(List<SelectListItem> items)
    {
        var connectionTypes = await ConnectionType.Basic.ToSelectListAsync();

        var availableConnectionTypes = connectionTypes.Select(p => new SelectListItem
        {
            Text = p.Text,
            Value = p.Value
        })?.ToList();

        availableConnectionTypes.Insert(0, new SelectListItem
        {
            Text = await _localizationService.GetResourceAsync($"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.ConnectionType.Select"),
            Value = "0"
        });

        items.AddRange(availableConnectionTypes);
    }
    #endregion

    public async Task<IActionResult> Configure()
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            return AccessDeniedView();

        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var elasticsearchSettings = await _settingService.LoadSettingAsync<ElasticsearchSettings>(storeScope);

        var model = new ConfigurationModel
        {
            Active = elasticsearchSettings.Active,
            ConnectionType = elasticsearchSettings.ConnectionType,
            Hostnames = elasticsearchSettings.Hostnames,
            Username = elasticsearchSettings.Username,
            Password = elasticsearchSettings.Password,
            CloudId = elasticsearchSettings.CloudId,
            ApiKey = elasticsearchSettings.ApiKey,
            UseFingerprint = elasticsearchSettings.UseFingerprint,
            Fingerprint = elasticsearchSettings.Fingerprint,
            ActiveStoreScopeConfiguration = storeScope
        };

        await PrepareAvailableConnectionTypesAsync(model.AvailableConnectionTypes);

        if (storeScope > 0)
        {
            model.Active_OverrideForStore = await _settingService.SettingExistsAsync(elasticsearchSettings, x => x.Active, storeScope);
            model.ConnectionType_OverrideForStore = await _settingService.SettingExistsAsync(elasticsearchSettings, x => x.ConnectionType, storeScope);
            model.Hostnames_OverrideForStore = await _settingService.SettingExistsAsync(elasticsearchSettings, x => x.Hostnames, storeScope);
            model.Username_OverrideForStore = await _settingService.SettingExistsAsync(elasticsearchSettings, x => x.Username, storeScope);
            model.Password_OverrideForStore = await _settingService.SettingExistsAsync(elasticsearchSettings, x => x.Password, storeScope);
            model.CloudId_OverrideForStore = await _settingService.SettingExistsAsync(elasticsearchSettings, x => x.CloudId, storeScope);
            model.ApiKey_OverrideForStore = await _settingService.SettingExistsAsync(elasticsearchSettings, x => x.ApiKey, storeScope);
            model.UseFingerprint_OverrideForStore = await _settingService.SettingExistsAsync(elasticsearchSettings, x => x.UseFingerprint, storeScope);
            model.Fingerprint_OverrideForStore = await _settingService.SettingExistsAsync(elasticsearchSettings, x => x.Fingerprint, storeScope);
        }

        return View($"~/Plugins/{ElasticsearchDefaults.PluginSystemName}/Views/Configure.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            return AccessDeniedView();

        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var elasticsearchSettings = await _settingService.LoadSettingAsync<ElasticsearchSettings>(storeScope);

        elasticsearchSettings.Active = model.Active;
        elasticsearchSettings.ConnectionType = model.ConnectionType;
        elasticsearchSettings.Hostnames = model.Hostnames;
        elasticsearchSettings.Username = model.Username;
        elasticsearchSettings.Password = model.Password;
        elasticsearchSettings.CloudId = model.CloudId;
        elasticsearchSettings.ApiKey = model.ApiKey;
        elasticsearchSettings.UseFingerprint = model.UseFingerprint;
        elasticsearchSettings.Fingerprint = model.Fingerprint;

        await _settingService.SaveSettingOverridablePerStoreAsync(elasticsearchSettings, x => x.Active, model.Active_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(elasticsearchSettings, x => x.ConnectionType, model.ConnectionType_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(elasticsearchSettings, x => x.Hostnames, model.Hostnames_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(elasticsearchSettings, x => x.Username, model.Username_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(elasticsearchSettings, x => x.Password, model.Password_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(elasticsearchSettings, x => x.CloudId, model.CloudId_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(elasticsearchSettings, x => x.ApiKey, model.ApiKey_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(elasticsearchSettings, x => x.UseFingerprint, model.UseFingerprint_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(elasticsearchSettings, x => x.Fingerprint, model.Fingerprint_OverrideForStore, storeScope, false);

        await _settingService.ClearCacheAsync();

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return await Configure();
    }
}
