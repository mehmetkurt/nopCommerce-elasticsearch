namespace Nop.Plugin.SearchProvider.Elasticsearch.Settings;

/// <summary>
/// Enumeration for different types of connections to an Elasticsearch cluster.
/// </summary>
public enum ConnectionType
{
    /// <summary>
    /// API connection type for using an API key to authenticate and connect.
    /// This is recommended for security purposes, where each application has its own API key.
    /// </summary>
    Api = 10,

    /// <summary>
    /// Basic connection type for connecting to a single Elasticsearch node.
    /// Suitable for local development or when connecting through a load balancer.
    /// </summary>
    Basic = 20,

    /// <summary>
    /// Cloud connection type for connecting to an Elasticsearch cluster hosted on Elastic Cloud.
    /// It is recommended to use the Cloud ID for optimal configuration and security features.
    /// </summary>
    Cloud = 30
}