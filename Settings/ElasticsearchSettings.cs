using Nop.Core.Configuration;
using Nop.Plugin.SearchProvider.Elasticsearch.Validators;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Settings;

/// <summary>
/// Represents the Elasticsearch settings.
/// </summary>
public class ElasticsearchSettings : ISettings, IConfigurationValidator
{
    /// <summary>
    /// Gets or sets a value indicating whether the Elasticsearch configuration is active.
    /// </summary>
    public bool Active { get; set; }

    /// <summary>
    /// Gets or sets the type of connection to Elasticsearch.
    /// This property uses the <see cref="ConnectionType"/> enumeration to specify the connection strategy.
    /// </summary>
    public int ConnectionType { get; set; }

    /// <summary>
    /// Gets or sets the hostnames of the Elasticsearch nodes.
    /// This can be a single hostname or multiple hostnames for connecting to multiple nodes in a cluster.
    /// For example: "node1.example.com;node2.example.com;node3.example.com"
    /// </summary>
    public string Hostnames { get; set; }

    /// <summary>
    /// Gets or sets the username used for authentication with Elasticsearch.
    /// This is required for basic authentication when connecting to the cluster.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Gets or sets the password used for authentication with Elasticsearch.
    /// This is required along with the username for basic authentication.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets the Cloud ID for connecting to Elasticsearch on the cloud.
    /// The Cloud ID is used for optimal configuration when connecting to an Elastic Cloud deployment.
    /// </summary>
    public string CloudId { get; set; }

    /// <summary>
    /// Gets or sets the API key used for authentication with Elasticsearch.
    /// Using an API key is a recommended security best practice for authenticating applications.
    /// </summary>
    public string ApiKey { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use fingerprint authentication with Elasticsearch.
    /// This determines if the client should use the CA certificate fingerprint for secure connections.
    /// </summary>
    public bool UseFingerprint { get; set; }

    /// <summary>
    /// Gets or sets the fingerprint used for authentication with Elasticsearch.
    /// The fingerprint is the hex-encoded SHA-256 of the CA certificate used for secure connections.
    /// </summary>
    public string Fingerprint { get; set; }


    /// <summary>
    /// Gets or sets the type of search to be performed.
    /// This property uses the <see cref="SearchType"/> enumeration to specify the search strategy.
    /// </summary>
    public int SearchType { get; set; }

    /// <summary>
    /// Gets or sets the result limit for search queries to Elasticsearch.
    /// The result limit specifies the maximum number of matching results that will be retrieved from the server.
    /// </summary>
    public int ResultLimit { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to immediately update search index documents.
    /// If set to true, changes such as updates to product names or descriptions will be applied immediately.
    /// Otherwise, changes will be scheduled for reindexing.
    /// </summary>
    public bool ImmediatelyUpdateIndexes { get; set; }
}
