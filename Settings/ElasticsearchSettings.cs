using Nop.Core.Configuration;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Settings;

/// <summary>
/// Represents the Elasticsearch settings.
/// </summary>
public class ElasticsearchSettings : ISettings
{
    /// <summary>
    /// Gets or sets a value indicating whether the Elasticsearch configuration is active.
    /// </summary>
    public bool Active { get; set; }

    /// <summary>
    /// Gets or sets the type of connection to Elasticsearch.
    /// </summary>
    public int ConnectionType { get; set; }

    /// <summary>
    /// Gets or sets the hostnames of the Elasticsearch nodes.
    /// </summary>
    public string Hostnames { get; set; }

    /// <summary>
    /// Gets or sets the username used for authentication with Elasticsearch.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Gets or sets the password used for authentication with Elasticsearch.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets the Cloud ID for connecting to Elasticsearch on the cloud.
    /// </summary>
    public string CloudId { get; set; }

    /// <summary>
    /// Gets or sets the API key used for authentication with Elasticsearch.
    /// </summary>
    public string ApiKey { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use fingerprint authentication with Elasticsearch.
    /// </summary>
    public bool UseFingerprint { get; set; }

    /// <summary>
    /// Gets or sets the fingerprint used for authentication with Elasticsearch.
    /// </summary>
    public string Fingerprint { get; set; }
}
