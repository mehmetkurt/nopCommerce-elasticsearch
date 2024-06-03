using Elastic.Clients.Elasticsearch;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.Elasticsearch.Repositories;
using Nop.Plugin.Misc.Elasticsearch.Settings;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Misc.Elasticsearch;

public class ElasticsearchPlugin : BasePlugin, IMiscPlugin, ISearchProvider, IAdminMenuPlugin
{
    private readonly IWebHelper _webHelper;
    private readonly ISettingService _settingService;
    private readonly CatalogSettings _catalogSettings;
    private readonly IPermissionService _permissionService;
    private readonly ILocalizationService _localizationService;
    private readonly IElasticsearchRepository<Product> _elasticsearchRepository;

    public ElasticsearchPlugin
    (
        IWebHelper webHelper,
        ISettingService settingService,
        CatalogSettings catalogSettings,
        IPermissionService permissionService,
        ILocalizationService localizationService,
        IElasticsearchRepository<Product> elasticsearchRepository
    )
    {
        _webHelper = webHelper;
        _settingService = settingService;
        _catalogSettings = catalogSettings;
        _permissionService = permissionService;
        _localizationService = localizationService;
        _elasticsearchRepository = elasticsearchRepository;
    }

    /// <summary>
    /// Install plugin
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// </returns>
    public override async Task InstallAsync()
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
            Fingerprint = string.Empty
        };

        _catalogSettings.ActiveSearchProviderSystemName = ElasticsearchDefaults.PluginSystemName;

        await _settingService.SaveSettingAsync(_catalogSettings);
        await _settingService.SaveSettingAsync(elasticsearchSettings);

        await _settingService.ClearCacheAsync();

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
    /// Uninstall plugin
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// </returns>
    public override async Task UninstallAsync()
    {
        if (_catalogSettings.AllowCustomersToSearchWithCategoryName.Equals(ElasticsearchDefaults.PluginSystemName))
        {
            _catalogSettings.ActiveSearchProviderSystemName = string.Empty;
            await _settingService.SaveSettingAsync(_catalogSettings);
        }

        await _settingService.DeleteSettingAsync<ElasticsearchSettings>();
        await _settingService.ClearCacheAsync();

        await _localizationService.DeleteLocaleResourcesAsync($"{ElasticsearchDefaults.LocalizationPrefix}");
        await _localizationService.DeleteLocaleResourcesAsync($"Enums.{ElasticsearchDefaults.LocalizationPrefix}");
    }

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
        return $"{_webHelper.GetStoreLocation()}Admin/Elasticsearch/Configure";
    }

    /// <summary>
    /// Searches the products asynchronous.
    /// </summary>
    /// <param name="keywords">The keywords.</param>
    /// <param name="isLocalized">if set to <c>true</c> [is localized].</param>
    /// <returns>
    /// A task that represents the asynchronous operation. System.Collections.Generic.List<int>
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

        return await Task.FromResult(result.Select(p => p.Id)?.ToList());
    }

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
}
