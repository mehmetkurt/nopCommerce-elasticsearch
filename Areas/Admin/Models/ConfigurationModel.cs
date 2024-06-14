using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.SearchProvider.Elasticsearch.Validators;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Areas.Admin.Models;

/// <summary>
/// Represents the configuration model for Elasticsearch settings in nopCommerce.
/// </summary>
public record ConfigurationModel : BaseNopModel, ISettingsModel, IConfigurationValidator
{
    /// <summary>
    /// Gets or sets an active store scope configuration (store identifier).
    /// </summary>
    public int ActiveStoreScopeConfiguration { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the Elasticsearch plugin is active.
    /// </summary>
    [NopResourceDisplayName($"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Active")]
    public bool Active { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the Active setting is overridden for the current store.
    /// </summary>
    public bool Active_OverrideForStore { get; set; }

    /// <summary>
    /// Gets or sets the type of connection to Elasticsearch.
    /// </summary>
    [NopResourceDisplayName($"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.ConnectionType")]
    public int ConnectionType { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the ConnectionType setting is overridden for the current store.
    /// </summary>
    public bool ConnectionType_OverrideForStore { get; set; }

    /// <summary>
    /// Gets or sets the hostnames of the Elasticsearch nodes.
    /// </summary>
    [NopResourceDisplayName($"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Hostnames")]
    public string Hostnames { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the Hostnames setting is overridden for the current store.
    /// </summary>
    public bool Hostnames_OverrideForStore { get; set; }

    /// <summary>
    /// Gets or sets the username used for authentication with Elasticsearch.
    /// </summary>
    [NopResourceDisplayName($"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Username")]
    public string Username { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the Username setting is overridden for the current store.
    /// </summary>
    public bool Username_OverrideForStore { get; set; }

    /// <summary>
    /// Gets or sets the password used for authentication with Elasticsearch.
    /// </summary>
    [NopResourceDisplayName($"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Password")]
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the Password setting is overridden for the current store.
    /// </summary>
    public bool Password_OverrideForStore { get; set; }

    /// <summary>
    /// Gets or sets the Cloud ID for connecting to Elasticsearch on the cloud.
    /// </summary>
    [NopResourceDisplayName($"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.CloudId")]
    public string CloudId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the CloudId setting is overridden for the current store.
    /// </summary>
    public bool CloudId_OverrideForStore { get; set; }

    /// <summary>
    /// Gets or sets the API key used for authentication with Elasticsearch.
    /// </summary>
    [NopResourceDisplayName($"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.ApiKey")]
    public string ApiKey { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the ApiKey setting is overridden for the current store.
    /// </summary>
    public bool ApiKey_OverrideForStore { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use fingerprint authentication with Elasticsearch.
    /// </summary>
    [NopResourceDisplayName($"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.UseFingerprint")]
    public bool UseFingerprint { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the UseFingerprint setting is overridden for the current store.
    /// </summary>
    public bool UseFingerprint_OverrideForStore { get; set; }

    /// <summary>
    /// Gets or sets the fingerprint used for authentication with Elasticsearch.
    /// </summary>
    [NopResourceDisplayName($"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Fingerprint")]
    public string Fingerprint { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the Fingerprint setting is overridden for the current store.
    /// </summary>
    public bool Fingerprint_OverrideForStore { get; set; }

    /// <summary>
    /// Gets or sets the type of search to be performed.
    /// </summary>
    [NopResourceDisplayName($"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.SearchType")]
    public int SearchType { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the SearchType setting is overridden for the current store.
    /// </summary>
    public bool SearchType_OverrideForStore { get; set; }

    /// <summary>
    /// Gets or sets the result limit for search queries to Elasticsearch.
    /// </summary>
    [NopResourceDisplayName($"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.ResultLimit")]
    public int ResultLimit { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the ResultLimit setting is overridden for the current store.
    /// </summary>
    public bool ResultLimit_OverrideForStore { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to immediately update search index documents.
    /// </summary>
    [NopResourceDisplayName($"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.ImmediatelyUpdateIndexes")]
    public bool ImmediatelyUpdateIndexes { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the ImmediatelyUpdateIndexes setting is overridden for the current store.
    /// </summary>
    public bool ImmediatelyUpdateIndexes_OverrideForStore { get; set; }

    /// <summary>
    /// Gets or sets the available connection types for Elasticsearch.
    /// </summary>
    public List<SelectListItem> AvailableConnectionTypes { get; set; } = new();

    /// <summary>
    /// Gets or sets the available search types for Elasticsearch.
    /// </summary>
    public List<SelectListItem> AvailableSeachTypes { get; set; } = new();
}
