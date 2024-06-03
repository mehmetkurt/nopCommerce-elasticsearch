using Elastic.Clients.Elasticsearch;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Plugin.Misc.Elasticsearch.Repositories;
using Nop.Plugin.Misc.Elasticsearch.Settings;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Menu;
using System.Collections.Immutable;

namespace Nop.Plugin.Misc.Elasticsearch;

public class ElasticsearchPlugin : BasePlugin, ISearchProvider, IAdminMenuPlugin
{
    #region Fields
    private readonly IImmutableList<ScheduleTask> _scheduleTasks;

    private readonly IWebHelper _webHelper;
    private readonly ISettingService _settingService;
    private readonly CatalogSettings _catalogSettings;
    private readonly IPermissionService _permissionService;
    private readonly ILocalizationService _localizationService;
    private readonly IScheduleTaskService _scheduleTaskService;
    private readonly IElasticsearchRepository<Product> _elasticsearchRepository;
    #endregion

    #region Ctor
    public ElasticsearchPlugin
    (
        IWebHelper webHelper,
        ISettingService settingService,
        CatalogSettings catalogSettings,
        IPermissionService permissionService,
        ILocalizationService localizationService,
        IScheduleTaskService scheduleTaskService,
        IElasticsearchRepository<Product> elasticsearchRepository
    )
    {
        _webHelper = webHelper;
        _settingService = settingService;
        _catalogSettings = catalogSettings;
        _permissionService = permissionService;
        _localizationService = localizationService;
        _scheduleTaskService = scheduleTaskService;
        _elasticsearchRepository = elasticsearchRepository;

        _scheduleTasks = _scheduleTaskService.GetAllTasksAsync().Result?.ToImmutableList();
    }
    #endregion

    #region Utilities
    /// <summary>
    /// Installs the locale resources for the Elasticsearch plugin.
    /// Adds localization strings used by the plugin's configuration and UI.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task InstallLocaleResourcesAsync()
    {
        // Localization
        var resources = new Dictionary<string, string>
    {
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Menu", "Elasticsearch" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration", "Elasticsearch Configuration" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.BlockTitle.General", "General" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Active", "Active" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Active.Hint", "Determines whether the Elasticsearch plugin is active. Set to 'true' to enable the plugin." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Hostnames", "Hostname(s)" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Hostnames.Hint", "Specifies the hostnames of the Elasticsearch servers. Multiple hostnames can be separated by commas." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Username", "Username" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Username.Hint", "The username for authenticating with the Elasticsearch server." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Password", "Password" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Password.Hint", "The password for authenticating with the Elasticsearch server." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.ApiKey", "API Key" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.ApiKey.Hint", "The API key used for accessing Elasticsearch services." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.CloudId", "Cloud Id" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.CloudId.Hint", "Connecting to an Elasticsearch Service deployment is achieved by providing the unique Cloud ID for your deployment when configuring the instance." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.UseFingerprint", "Use Fingerprint" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.UseFingerprint.Hint", "Indicates whether to use a certificate fingerprint for server verification. Set to 'true' to enable." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Fingerprint", "Fingerprint" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Fingerprint.Hint", "The fingerprint of the server's SSL/TLS certificate used for secure connections." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.ConnectionType", "Connection Type" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.ConnectionType.Hint", "Specifies the type of connection to use. Options are 'Api', 'Cloud', or 'Basic'." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.ConnectionType.Select", "Select" },
        { $"Enums.{ElasticsearchDefaults.LocalizationPrefix}.Settings.ConnectionType.Api", "API Connection" },
        { $"Enums.{ElasticsearchDefaults.LocalizationPrefix}.Settings.ConnectionType.Api.Hint", "Uses API key for connecting to Elasticsearch." },
        { $"Enums.{ElasticsearchDefaults.LocalizationPrefix}.Settings.ConnectionType.Basic", "Basic Connection" },
        { $"Enums.{ElasticsearchDefaults.LocalizationPrefix}.Settings.ConnectionType.Basic.Hint", "Uses basic authentication (username and password) for connecting to Elasticsearch." },
        { $"Enums.{ElasticsearchDefaults.LocalizationPrefix}.Settings.ConnectionType.Cloud", "Cloud Connection" },
        { $"Enums.{ElasticsearchDefaults.LocalizationPrefix}.Settings.ConnectionType.Cloud.Hint", "Connects to a managed Elasticsearch cloud service." }
    };
        await _localizationService.AddOrUpdateLocaleResourceAsync(resources);
    }

    /// <summary>
    /// Uninstalls the locale resources for the Elasticsearch plugin.
    /// Removes localization strings used by the plugin's configuration and UI.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task UninstallLocaleResourcesAsync()
    {
        await _localizationService.DeleteLocaleResourcesAsync($"{ElasticsearchDefaults.LocalizationPrefix}");
        await _localizationService.DeleteLocaleResourcesAsync($"Enums.{ElasticsearchDefaults.LocalizationPrefix}");
    }

    /// <summary>
    /// Installs the default settings for the Elasticsearch plugin.
    /// Configures initial settings for the Elasticsearch plugin including activation status, connection details, and other necessary configurations.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task InstallSettingsAsync()
    {
        var elasticsearchSettings = new ElasticsearchSettings
        {
            Active = true,
            Hostnames = string.Join(";", new[] { "http://localhost:9200" }),
            Username = "elastic",
            Password = "elastic",
            ConnectionType = (int)ConnectionType.Basic,
            ApiKey = string.Empty,
            UseFingerprint = false,
            CloudId = string.Empty,
            Fingerprint = string.Empty
        };

        _catalogSettings.ActiveSearchProviderSystemName = ElasticsearchDefaults.PluginSystemName;

        await _settingService.SaveSettingAsync(_catalogSettings);
        await _settingService.SaveSettingAsync(elasticsearchSettings);

        await _settingService.ClearCacheAsync();
    }

    /// <summary>
    /// Uninstalls the settings for the Elasticsearch plugin.
    /// Resets the active search provider system name and removes the Elasticsearch plugin settings.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task UninstallSettingsAsync()
    {
        if (_catalogSettings.AllowCustomersToSearchWithCategoryName.Equals(ElasticsearchDefaults.PluginSystemName))
        {
            _catalogSettings.ActiveSearchProviderSystemName = string.Empty;
            await _settingService.SaveSettingAsync(_catalogSettings);
        }

        await _settingService.DeleteSettingAsync<ElasticsearchSettings>();
        await _settingService.ClearCacheAsync();
    }

    /// <summary>
    /// Installs a scheduled transfer task if it does not already exist.
    /// </summary>
    /// <param name="name">The name of the task.</param>
    /// <param name="type">The type of the task.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task InstallTransferTaskAsync(string name, string type)
    {
        if (_scheduleTasks.Any(p => p.Type.Equals(type)))
            return;

        var task = new ScheduleTask
        {
            Name = name,
            Type = type,
            Seconds = 60,
            Enabled = false,
            StopOnError = false
        };

        await _scheduleTaskService.InsertTaskAsync(task);
    }

    /// <summary>
    /// Uninstalls a scheduled transfer task if it exists.
    /// </summary>
    /// <param name="type">The type of the task.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task UninstallTransferTaskAsync(string type)
    {
        var task = _scheduleTasks.FirstOrDefault(p => p.Type.Equals(type));
        if (task == null)
            return;

        await _scheduleTaskService.DeleteTaskAsync(task);
    }

    #endregion

    #region Base Plugin Methods
    /// <summary>
    /// Install plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public override async Task InstallAsync()
    {
        await InstallSettingsAsync();
        await InstallLocaleResourcesAsync();

        // Install the category transfer task if it does not already exist
        await InstallTransferTaskAsync(
            ElasticsearchDefaults.ScheduledTask.CategoryTransferTaskName,
            ElasticsearchDefaults.ScheduledTask.CategoryTransferTaskType);

        // Install the product transfer task if it does not already exist
        await InstallTransferTaskAsync(
            ElasticsearchDefaults.ScheduledTask.ProductTransferTaskName,
            ElasticsearchDefaults.ScheduledTask.ProductTransferTaskType);

        // Install the product attribute transfer task if it does not already exist
        await InstallTransferTaskAsync(
            ElasticsearchDefaults.ScheduledTask.ProductAttributeTransferTaskName,
            ElasticsearchDefaults.ScheduledTask.ProductAttributeTransferTaskType);

        // Install the product combination transfer task if it does not already exist
        await InstallTransferTaskAsync(
            ElasticsearchDefaults.ScheduledTask.ProductCombinationTransferTaskName,
            ElasticsearchDefaults.ScheduledTask.ProductCombinationTransferTaskType);
    }

    /// <summary>
    /// Uninstall plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public override async Task UninstallAsync()
    {
        await UninstallSettingsAsync();
        await UninstallLocaleResourcesAsync();

        // Uninstall the category transfer task if it exists
        await UninstallTransferTaskAsync(ElasticsearchDefaults.ScheduledTask.CategoryTransferTaskType);

        // Uninstall the product transfer task if it exists
        await UninstallTransferTaskAsync(ElasticsearchDefaults.ScheduledTask.ProductTransferTaskType);

        // Uninstall the product attribute transfer task if it exists
        await UninstallTransferTaskAsync(ElasticsearchDefaults.ScheduledTask.ProductAttributeTransferTaskType);

        // Uninstall the product combination transfer task if it exists
        await UninstallTransferTaskAsync(ElasticsearchDefaults.ScheduledTask.ProductCombinationTransferTaskType);

    }

    /// <summary>
    /// Gets the configuration page URL for the Elasticsearch plugin.
    /// </summary>
    /// <returns>The URL of the configuration page as a string.</returns>
    public override string GetConfigurationPageUrl()
    {
        return $"{_webHelper.GetStoreLocation()}Admin/Elasticsearch/Configure";
    }

    #endregion

    #region Search Provider Methods
    /// <summary>
    /// Searches the products asynchronously based on the provided keywords.
    /// </summary>
    /// <param name="keywords">The keywords to search for.</param>
    /// <param name="isLocalized">If set to <c>true</c>, the search will consider localized fields.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a list of product IDs that match the search criteria.
    /// </returns>
    public async Task<List<int>> SearchProductsAsync(string keywords, bool isLocalized)
    {
        var result = await _elasticsearchRepository.FindAsync(p => p.Query(q => q
            .Match(m => m
                .Field(f => f.Name)
                .Fuzziness(new Fuzziness(1))
                .Query(keywords)
            )
        ));

        return result.Select(p => p.Id).ToList();
    }

    #endregion

    #region Admin Menu Plugin Methods
    /// <summary>
    /// Manage sitemap. You can use "SystemName" of menu items to manage existing sitemap or add a new menu item.
    /// </summary>
    /// <param name="rootNode">Root node of the sitemap.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task ManageSiteMapAsync(SiteMapNode rootNode)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            return;

        var configuration = rootNode.ChildNodes.FirstOrDefault(node => node.SystemName.Equals("Configuration"));
        if (configuration == null)
            return;

        var plugins = configuration.ChildNodes.FirstOrDefault(node => node.SystemName.Equals("Local plugins"));

        if (plugins == null)
            return;

        var index = configuration.ChildNodes.IndexOf(plugins);

        if (index < 0)
            return;

        configuration.ChildNodes.Insert(index, new SiteMapNode
        {
            SystemName = ElasticsearchDefaults.AdminMenuSystemName,
            Title = await _localizationService.GetResourceAsync($"{ElasticsearchDefaults.LocalizationPrefix}.Menu"),
            IconClass = "far fa-dot-circle",
            ControllerName = "Elasticsearch",
            ActionName = "Configure",
            RouteValues = new Microsoft.AspNetCore.Routing.RouteValueDictionary { { "area", AreaNames.ADMIN } },
            Visible = true
        });
    }
    #endregion
}
