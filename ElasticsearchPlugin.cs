using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Plugin.SearchProvider.Elasticsearch.Repositories;
using Nop.Plugin.SearchProvider.Elasticsearch.Settings;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.SearchProvider.Elasticsearch;

/// <summary>
/// Represents the Elasticsearch plugin, implementing functionality for search provider and admin menu plugin interfaces.
/// </summary>
public class ElasticsearchPlugin : BasePlugin, ISearchProvider, IAdminMenuPlugin
{
    #region Fields
    private readonly IWebHelper _webHelper;
    private readonly ISettingService _settingService;
    private readonly CatalogSettings _catalogSettings;
    private readonly IPermissionService _permissionService;
    private readonly ILocalizationService _localizationService;
    private readonly IScheduleTaskService _scheduleTaskService;
    private readonly IElasticsearchRepository<Product> _elasticsearchRepository;
    #endregion

    #region Ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticsearchPlugin"/> class.
    /// </summary>
    /// <param name="webHelper">The web helper.</param>
    /// <param name="settingService">The setting service.</param>
    /// <param name="catalogSettings">The catalog settings.</param>
    /// <param name="permissionService">The permission service.</param>
    /// <param name="localizationService">The localization service.</param>
    /// <param name="scheduleTaskService">The schedule task service.</param>
    /// <param name="elasticsearchRepository">The Elasticsearch repository.</param>
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
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Menu.Configuration", "Configuration" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Menu.EntityTransfer", "Entity Transfers" },

        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration", "Elasticsearch Configuration" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Tabs.General", "General" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Tabs.Security", "Security" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Tabs.Search", "Search" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Buttons.RebuildIndex", "Rebuild Index" },

        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Active", "Active" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Active.Hint", "Determines whether the Elasticsearch plugin is active. Set to 'true' to enable the plugin." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.ApiKey", "API Key" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.ApiKey.Hint", "The API key used for accessing Elasticsearch services." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.ApiKey.NotEmpty", "API key cannot be empty." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.ApiKey.NotNull", "API key must not be null." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.CloudId", "Cloud Id" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.CloudId.Hint", "Connecting to an Elasticsearch Service deployment is achieved by providing the unique Cloud ID for your deployment when configuring the instance." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.CloudId.NotEmpty", "Cloud ID cannot be empty." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.CloudId.NotNull", "Cloud ID must not be null." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.ConnectionType", "Connection Type" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.ConnectionType.Hint", "Specifies the type of connection to use. Options are 'Api', 'Cloud', or 'Basic'." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.ConnectionType.NotEqual", "Connection type must not be equal to 0." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.ConnectionType.Select", "Select" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Fingerprint", "Fingerprint" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Fingerprint.Hint", "The fingerprint of the server's SSL/TLS certificate used for secure connections." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Fingerprint.NotEmpty", "Fingerprint cannot be empty." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Fingerprint.NotNull", "Fingerprint must not be null." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Hostnames", "Hostname(s)" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Hostnames.Hint", "Specifies the hostnames of the Elasticsearch servers. Multiple hostnames can be separated by commas." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Hostnames.NotEmpty", "Hostnames cannot be empty." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Hostnames.NotNull", "Hostnames must not be null." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Password", "Password" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Password.Hint", "The password for authenticating with the Elasticsearch server." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Password.NotEmpty", "Password cannot be empty." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Password.NotNull", "Password must not be null." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Username", "Username" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Username.Hint", "The username for authenticating with the Elasticsearch server." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Username.NotEmpty", "Username cannot be empty." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Username.NotNull", "Username must not be null." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.UseFingerprint", "Use Fingerprint" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.UseFingerprint.Hint", "Indicates whether to use a certificate fingerprint for server verification. Set to 'true' to enable." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.SearchType", "Search Type" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.SearchType.Hint", "Specifies the type of search strategy to use when querying Elasticsearch. " +
            "Different search types provide varying levels of flexibility and precision in matching query terms against indexed data." +
            "<br/><br/>" +
            "For example:" +
            "<ul>" +
            "<li><strong>Fuzzy:</strong> Matches terms even if they are slightly misspelled or contain typographical errors. Useful for improving search result recall when user input may vary.</li>" +
            "<li><strong>Wildcard:</strong> Allows pattern-based searches using wildcards like '*', which match zero or more characters, and '?', which matches a single character. Ideal for advanced searching where partial matching or pattern recognition is needed.</li>" +
            "<li><strong>Exact:</strong> Strictly matches the query terms to indexed tokens without any variations or extensions. Suitable for precise search requirements where exact matches are essential.</li>" +
            "</ul>" +
            "Choose the appropriate search type based on your application's search requirements and user expectations."
        },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.ResultLimit", "Result Limit" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.ResultLimit.Hint", "Specifies the maximum number of search results to retrieve from Elasticsearch." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.ImmediatelyUpdateIndexes", "Immediately Update Indexes" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.ImmediatelyUpdateIndexes.Hint", "Determines whether changes to indexed documents, such as product names or descriptions, should be applied immediately or scheduled for reindexing." },

        // Entity Transfer
        { $"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.List", "Elasticsearch Entity Transfer" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Fields.EntityName", "Entity Name" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Fields.EntityName.Hint", "Enter the name of the entity." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Fields.EntityId", "Entity Id" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Fields.EntityId.Hint", "Enter the identifier of the entity." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Fields.Ignored", "Ignored" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Fields.Ignored.Hint", "Check this box if the transfer is ignored." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Fields.OperationType", "Operation Type" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Fields.OperationType.Hint", "Select the operation type." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Fields.CreatedDateUtc", "Created Date Utc" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Fields.CreatedDateUtc.Hint", "Enter the date and time when the transfer was created (in UTC)." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Fields.UpdatedDateUtc", "Updated Date Utc" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Fields.UpdatedDateUtc.Hint", "Enter the date and time when the transfer was last updated (in UTC)." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Search.OperationType.All", "All" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Search.EntityName.All", "All" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Search.Ignored.All", "All" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Search.Ignored.IgnoredOnly", "Ignored Only" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Search.Ignored.NotIgnoredOnly", "Not Ignored Only" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Search.EntityName", "Entity Name" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Search.EntityName.Hint", "Enter the name of the entity to filter by." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Search.Ignored", "Ignored" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Search.Ignored.Hint", "Filter by whether the entity is ignored." },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Search.OperationType", "Operation Type" },
        { $"{ElasticsearchDefaults.LocalizationPrefix}.EntityTransfer.Search.OperationType.Hint", "Select the operation type to filter by." },
        { $"Enums.{ElasticsearchDefaults.LocalizationPrefix}.Settings.ConnectionType.Api", "API Connection" },
        { $"Enums.{ElasticsearchDefaults.LocalizationPrefix}.Settings.ConnectionType.Basic", "Basic Connection" },
        { $"Enums.{ElasticsearchDefaults.LocalizationPrefix}.Settings.ConnectionType.Cloud", "Cloud Connection" },
        { $"Enums.{ElasticsearchDefaults.LocalizationPrefix}.Settings.SearchType.Fuzzy", "Fuzzy" },
        { $"Enums.{ElasticsearchDefaults.LocalizationPrefix}.Settings.SearchType.Wildcard", "Wildcard" },
        { $"Enums.{ElasticsearchDefaults.LocalizationPrefix}.Settings.SearchType.Exact", "Exact" },
        { $"Enums.{ElasticsearchDefaults.LocalizationPrefix}.Data.Domain.OperationType.Inserted", "Inserted" },
        { $"Enums.{ElasticsearchDefaults.LocalizationPrefix}.Data.Domain.OperationType.Updated", "Updated" },
        { $"Enums.{ElasticsearchDefaults.LocalizationPrefix}.Data.Domain.OperationType.Deleted", "Deleted" },
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
            Hostnames = string.Join(";", ["http://localhost:9200"]),
            Username = "elastic",
            Password = "elastic",
            ConnectionType = (int)ConnectionType.Basic,
            ApiKey = string.Empty,
            UseFingerprint = false,
            CloudId = string.Empty,
            Fingerprint = string.Empty,
            ResultLimit = 100,
            ImmediatelyUpdateIndexes = true,
            SearchType = (int)SearchType.Fuzzy
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
        var scheduleTask = await _scheduleTaskService.GetTaskByTypeAsync(type);
        if (scheduleTask != null)
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
        var scheduleTask = await _scheduleTaskService.GetTaskByTypeAsync(type);
        if (scheduleTask == null)
            return;

        await _scheduleTaskService.DeleteTaskAsync(scheduleTask);
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
        return new List<int>();
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
            Visible = true,
            ChildNodes = [
                new()
                {
                    SystemName = ElasticsearchDefaults.AdminMenuConfigurationSystemName,
                    Title = await _localizationService.GetResourceAsync($"{ElasticsearchDefaults.LocalizationPrefix}.Menu.Configuration"),
                    IconClass = "far fa-dot-circle",
                    Visible = true,
                    ControllerName = "Elasticsearch",
                    ActionName = "Configure",
                    RouteValues = new Microsoft.AspNetCore.Routing.RouteValueDictionary { { "area", AreaNames.ADMIN } },
                },
                   new()
                {
                    SystemName = ElasticsearchDefaults.AdminMenuEntityTransferSystemName,
                    Title = await _localizationService.GetResourceAsync($"{ElasticsearchDefaults.LocalizationPrefix}.Menu.EntityTransfer"),
                    IconClass = "far fa-dot-circle",
                    Visible = true,
                    ControllerName = "EntityTransfer",
                    ActionName = "List",
                    RouteValues = new Microsoft.AspNetCore.Routing.RouteValueDictionary { { "area", AreaNames.ADMIN } },
                }
            ]
        });
    }
    #endregion
}
