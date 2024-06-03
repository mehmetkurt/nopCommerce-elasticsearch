using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.Elasticsearch.Models;
public record ConfigurationModel : BaseNopModel, ISettingsModel
{
    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName($"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Active")]
    public bool Active { get; set; }
    public bool Active_OverrideForStore { get; set; }

    [NopResourceDisplayName($"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.ConnectionType")]
    public int ConnectionType { get; set; }
    public bool ConnectionType_OverrideForStore { get; set; }

    [NopResourceDisplayName($"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Hostnames")]
    public string Hostnames { get; set; }
    public bool Hostnames_OverrideForStore { get; set; }

    [NopResourceDisplayName($"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Username")]
    public string Username { get; set; }
    public bool Username_OverrideForStore { get; set; }

    [NopResourceDisplayName($"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Password")]
    public string Password { get; set; }
    public bool Password_OverrideForStore { get; set; }

    [NopResourceDisplayName($"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.CloudId")]
    public string CloudId { get; set; }
    public bool CloudId_OverrideForStore { get; set; }

    [NopResourceDisplayName($"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.ApiKey")]
    public string ApiKey { get; set; }
    public bool ApiKey_OverrideForStore { get; set; }

    [NopResourceDisplayName($"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.UseFingerprint")]
    public bool UseFingerprint { get; set; }
    public bool UseFingerprint_OverrideForStore { get; set; }

    [NopResourceDisplayName($"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Fingerprint")]
    public string Fingerprint { get; set; }
    public bool Fingerprint_OverrideForStore { get; set; }

    public List<SelectListItem> AvailableConnectionTypes { get; set; } = [];
}
