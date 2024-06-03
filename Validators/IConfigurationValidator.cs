namespace Nop.Plugin.SearchProvider.Elasticsearch.Validators;
public interface IConfigurationValidator
{
    /// <summary>
    /// Gets or sets a value indicating whether the Elasticsearch configuration is active.
    /// </summary>
    bool Active { get; set; }

    /// <summary>
    /// Gets or sets the type of connection to Elasticsearch.
    /// </summary>
    int ConnectionType { get; set; }

    /// <summary>
    /// Gets or sets the hostnames of the Elasticsearch nodes.
    /// </summary>
    string Hostnames { get; set; }

    /// <summary>
    /// Gets or sets the username used for authentication with Elasticsearch.
    /// </summary>
    string Username { get; set; }

    /// <summary>
    /// Gets or sets the password used for authentication with Elasticsearch.
    /// </summary>
    string Password { get; set; }

    /// <summary>
    /// Gets or sets the Cloud ID for connecting to Elasticsearch on the cloud.
    /// </summary>
    string CloudId { get; set; }

    /// <summary>
    /// Gets or sets the API key used for authentication with Elasticsearch.
    /// </summary>
    string ApiKey { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use fingerprint authentication with Elasticsearch.
    /// </summary>
    bool UseFingerprint { get; set; }

    /// <summary>
    /// Gets or sets the fingerprint used for authentication with Elasticsearch.
    /// </summary>
    string Fingerprint { get; set; }
}
