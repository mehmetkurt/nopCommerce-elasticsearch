using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.SearchProvider.Elasticsearch.Areas.Admin.Models;
using Nop.Plugin.SearchProvider.Elasticsearch.Settings;
using Nop.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Areas.Admin.Controllers;

/// <summary>
/// Represents a controller for configuring Elasticsearch settings in the admin area.
/// </summary>
[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
public class ElasticsearchController : BasePluginController
{
    #region Fields
    private readonly IWorkContext _workContext;
    private readonly IStoreContext _storeContext;
    private readonly ISettingService _settingService;
    private readonly IPermissionService _permissionService;
    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;
    #endregion

    #region Ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticsearchController" /> class.
    /// </summary>
    /// <param name="workContext">The work context.</param>
    /// <param name="storeContext">The store context.</param>
    /// <param name="settingService">The setting service.</param>
    /// <param name="permissionService">The permission service.</param>
    /// <param name="localizationService">The localization service.</param>
    /// <param name="notificationService">The notification service.</param>
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
    #endregion

    #region Utilities
    /// <summary>
    /// Prepares the available connection types for selection.
    /// </summary>
    /// <param name="items">The list to populate with available connection types.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task PrepareAvailableConnectionTypesAsync(List<SelectListItem> items)
    {
        var connectionTypes = await ConnectionType.Basic.ToSelectListAsync();

        var availableConnectionTypes = connectionTypes.Select(p => new SelectListItem
        {
            Text = p.Text,
            Value = p.Value
        }).ToList();

        availableConnectionTypes.Insert(0, new SelectListItem
        {
            Text = await _localizationService.GetResourceAsync($"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.ConnectionType.Select"),
            Value = "0"
        });

        items.AddRange(availableConnectionTypes);
    }

    /// <summary>
    /// Prepares the available search types for selection.
    /// </summary>
    /// <param name="items">The list to populate with available search types.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task PrepareAvailableSearchTypesAsync(List<SelectListItem> items)
    {
        var searchTypes = await SearchType.Exact.ToSelectListAsync();

        var availableSearchTypes = searchTypes.Select(p => new SelectListItem
        {
            Text = p.Text,
            Value = p.Value
        }).ToList();

        availableSearchTypes.Insert(0, new SelectListItem
        {
            Text = await _localizationService.GetResourceAsync($"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.SearchType.Select"),
            Value = "0"
        });

        items.AddRange(availableSearchTypes);
    }

    #endregion

    #region Configuration

    /// <summary>
    /// Displays the Elasticsearch configuration page.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Configure()
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            return AccessDeniedView();

        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var elasticsearchSettings = await _settingService.LoadSettingAsync<ElasticsearchSettings>(storeScope);

        var model = new ConfigurationModel
        {
            ActiveStoreScopeConfiguration = storeScope,
            Active = elasticsearchSettings.Active,
            ConnectionType = elasticsearchSettings.ConnectionType,
            Hostnames = elasticsearchSettings.Hostnames,
            Username = elasticsearchSettings.Username,
            Password = elasticsearchSettings.Password,
            CloudId = elasticsearchSettings.CloudId,
            ApiKey = elasticsearchSettings.ApiKey,
            UseFingerprint = elasticsearchSettings.UseFingerprint,
            Fingerprint = elasticsearchSettings.Fingerprint,
            SearchType = elasticsearchSettings.SearchType,
            ResultLimit = elasticsearchSettings.ResultLimit,
            ImmediatelyUpdateIndexes = elasticsearchSettings.ImmediatelyUpdateIndexes
        };

        await PrepareAvailableConnectionTypesAsync(model.AvailableConnectionTypes);
        await PrepareAvailableSearchTypesAsync(model.AvailableSeachTypes);

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
            model.SearchType_OverrideForStore = await _settingService.SettingExistsAsync(elasticsearchSettings, x => x.SearchType, storeScope);
            model.ResultLimit_OverrideForStore = await _settingService.SettingExistsAsync(elasticsearchSettings, x => x.ResultLimit, storeScope);
            model.ImmediatelyUpdateIndexes_OverrideForStore = await _settingService.SettingExistsAsync(elasticsearchSettings, x => x.ImmediatelyUpdateIndexes, storeScope);
        }

        return View(model);
    }

    /// <summary>
    /// Handles the HTTP POST request to save Elasticsearch configuration.
    /// </summary>
    /// <param name="model">The configuration model.</param>
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
        elasticsearchSettings.SearchType = model.SearchType;
        elasticsearchSettings.ResultLimit = model.ResultLimit;
        elasticsearchSettings.ImmediatelyUpdateIndexes = model.ImmediatelyUpdateIndexes;

        await _settingService.SaveSettingOverridablePerStoreAsync(elasticsearchSettings, x => x.Active, model.Active_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(elasticsearchSettings, x => x.ConnectionType, model.ConnectionType_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(elasticsearchSettings, x => x.Hostnames, model.Hostnames_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(elasticsearchSettings, x => x.Username, model.Username_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(elasticsearchSettings, x => x.Password, model.Password_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(elasticsearchSettings, x => x.CloudId, model.CloudId_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(elasticsearchSettings, x => x.ApiKey, model.ApiKey_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(elasticsearchSettings, x => x.UseFingerprint, model.UseFingerprint_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(elasticsearchSettings, x => x.Fingerprint, model.Fingerprint_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(elasticsearchSettings, x => x.SearchType, model.SearchType_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(elasticsearchSettings, x => x.ResultLimit, model.ResultLimit_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(elasticsearchSettings, x => x.ImmediatelyUpdateIndexes, model.ImmediatelyUpdateIndexes_OverrideForStore, storeScope, false);

        await _settingService.ClearCacheAsync();

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return await Configure();
    }
    #endregion

    #region Rebuild Index
    [HttpGet]
    public async Task<IActionResult> RebuildIndex(CancellationToken cancellationToken)
    {
        Response.Headers.ContentType = "text/event-stream";

        try
        {
            await SendProgressMessageAsync(new IndexEventResult
            {
                IndexingState = IndexingState.Initializing,
                Message = await _localizationService.GetLocalizedEnumAsync<IndexingState>(IndexingState.Initializing)
            });

            await Task.Delay(1000, cancellationToken);
            await SendProgressMessageAsync(new IndexEventResult
            {
                IndexingState = IndexingState.CalculatingCategories,
                Message = await _localizationService.GetLocalizedEnumAsync<IndexingState>(IndexingState.CalculatingCategories)
            });

            await Task.Delay(1500, cancellationToken);
            await SendProgressMessageAsync(new IndexEventResult
            {
                IndexingState = IndexingState.CalculatingProducts,
                Message = await _localizationService.GetLocalizedEnumAsync<IndexingState>(IndexingState.CalculatingProducts)
            });

            // 3. madde: Mapping model oluşturulması
            await Task.Delay(800, cancellationToken);
            await SendProgressMessageAsync(new IndexEventResult
            {
                IndexingState = IndexingState.CreatingMappingModel,
                Message = await _localizationService.GetLocalizedEnumAsync<IndexingState>(IndexingState.CreatingMappingModel)
            });

            // 4. madde: Transfer kuyruğuna eklenen nesneler
            await Task.Delay(1200, cancellationToken);
            await SendProgressMessageAsync(new IndexEventResult
            {
                IndexingState = IndexingState.AddingObjectsToTransferQueue,
                Message = await _localizationService.GetLocalizedEnumAsync<IndexingState>(IndexingState.AddingObjectsToTransferQueue)
            });

            // 5. madde: Elasticsearch bağlantısı kontrol ediliyor
            var connectionEstablished = await CheckConnectionAsync();
            if (!connectionEstablished)
            {
                await SendProgressMessageAsync(new IndexEventResult
                {
                    IndexingState = IndexingState.CheckingElasticsearchConnection,
                    Message = await _localizationService.GetLocalizedEnumAsync<IndexingState>(IndexingState.CheckingElasticsearchConnection)
                });

                return StatusCode(StatusCodes.Status400BadRequest, "Bağlantı sağlanamadı, işlem iptal edildi.");
            }

            // 6. madde: Modellerin Elasticsearch'e gönderilmesi
            await SendModelsToElasticsearchAsync(50, 0, 0);

            // İşlem tamamlandı mesajı
            await SendProgressMessageAsync(new IndexEventResult
            {
                IndexingState = IndexingState.OperationCompleted,
                Message = await _localizationService.GetLocalizedEnumAsync<IndexingState>(IndexingState.OperationCompleted)
            });

            return StatusCode(StatusCodes.Status200OK, "İşlem başarıyla tamamlandı.");
        }
        catch (OperationCanceledException)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "İşlem iptal edildi.");
        }
    }

    private async Task<bool> CheckConnectionAsync()
    {
        await SendProgressMessageAsync(new IndexEventResult
        {
            IndexingState = IndexingState.CheckingElasticsearchConnection,
            Message = await _localizationService.GetLocalizedEnumAsync<IndexingState>(IndexingState.CheckingElasticsearchConnection)
        });

        var isConnected = Random.Shared.Next(1, 101) <= 50;
        Thread.Sleep(2000);

        return isConnected;
    }

    private async Task SendModelsToElasticsearchAsync(int totalCount, int successed = 0, int failed = 0)
    {
        for (var i = 0; i < totalCount; i++)
        {
            var isSuccess = Random.Shared.Next(1, 101) <= 90;

            var data = new
            {
                TotalCount = totalCount,
                Successed = (isSuccess ? ++successed : successed),
                Failed = (isSuccess ? failed : ++failed),
                Processed = (successed + failed)
            };

            await SendProgressMessageAsync(new IndexEventResult
            {
                IndexingState = IndexingState.SendingModelsToElasticsearch,
                Message = await _localizationService.GetLocalizedEnumAsync<IndexingState>(IndexingState.SendingModelsToElasticsearch),
                Data = data
            });

            await Task.Delay(200);
        }
    }

    private async Task SendProgressMessageAsync(IndexEventResult result)
    {
        var message = JsonConvert.SerializeObject(result);
        await Response.WriteAsync($"data: {message}\n\n");
        await Response.Body.FlushAsync();
    }

    #endregion
}

public class IndexEventResult
{
    public IndexingState IndexingState { get; set; }
    public string Message { get; set; }
    public object Data { get; set; } = null;
}

public enum IndexingState
{
    Initializing = 10,
    CalculatingCategories = 20,
    CategoriesCalculated = 21,
    CalculatingProducts = 30,
    ProductsCalculated = 31,
    CreatingMappingModel = 40,
    MappingModelCreated = 41,
    AddingObjectsToTransferQueue = 50,
    ObjectsAddedToQueue = 51,
    CheckingElasticsearchConnection = 60,
    ElasticsearchConnectionFailed = 63,
    SendingModelsToElasticsearch = 65,
    OperationCompleted = 100,
    AnyError = 1000
}
