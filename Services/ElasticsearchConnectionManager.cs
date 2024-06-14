using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Nop.Core;
using Nop.Plugin.SearchProvider.Elasticsearch.Settings;
using Nop.Services.Logging;
using Nop.Services.Messages;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Services;

/// <summary>
/// Manages the Elasticsearch connection and provides a singleton instance of ElasticsearchClient.
/// </summary>
public class ElasticsearchConnectionManager : IElasticsearchConnectionManager
{
    #region Fields
    private Task _initializationTask;
    private ElasticsearchClient _client;
    private ElasticsearchClientSettings _elasticsearchClientSettings;

    private readonly object _lock = new();
    private readonly ILogger _logger;
    private readonly INotificationService _notificationService;
    private readonly IElasticsearchService _elasticsearchService;
    private readonly ElasticsearchSettings _elasticsearchSettings;
    #endregion

    #region Ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticsearchConnectionManager"/> class.
    /// </summary>
    /// <param name="logger">The logger service.</param>
    /// <param name="notificationService">The notification service.</param>
    /// <param name="elasticsearchService"></param>
    /// <param name="elasticsearchSettings">The Elasticsearch settings.</param>
    public ElasticsearchConnectionManager(
        ILogger logger,
        INotificationService notificationService,
        IElasticsearchService elasticsearchService,
        ElasticsearchSettings elasticsearchSettings)
    {
        _logger = logger;
        _notificationService = notificationService;
        _elasticsearchService = elasticsearchService;
        _elasticsearchSettings = elasticsearchSettings;
    }
    #endregion

    #region Utilities
    /// <summary>
    /// Initializes the Elasticsearch client asynchronously. Validates the settings and sets up the client.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task InitializeClientAsync()
    {
        if (!await _elasticsearchService.IsConfiguredAsync())
            return;

        var hostNames = _elasticsearchSettings.Hostnames.Split(";", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        NodePool nodePool;

        if (hostNames.Length > 1)
        {
            var uriList = hostNames.Select(p =>
            {
                if (Uri.TryCreate(p, UriKind.Absolute, out var uri))
                    return uri;
                else
                    throw new ArgumentException($"Invalid URI: {p}");
            }).ToList();

            nodePool = new StaticNodePool(uriList);
        }
        else
        {
            var uri = hostNames.FirstOrDefault();
            if (Uri.TryCreate(uri, UriKind.Absolute, out var singleUri))
                nodePool = new SingleNodePool(singleUri);
            else
                throw new ArgumentException($"Invalid URI: {uri}");
        }

        _elasticsearchClientSettings = new ElasticsearchClientSettings(nodePool)
            .Authentication(GetAuthentication());

        if (_elasticsearchSettings.UseFingerprint)
        {
            if (!string.IsNullOrEmpty(_elasticsearchSettings.Fingerprint))
                _elasticsearchClientSettings.CertificateFingerprint(_elasticsearchSettings.Fingerprint);
            else
                throw new ArgumentException("Fingerprint is empty.");
        }

        _client = new ElasticsearchClient(_elasticsearchClientSettings);
    }


    /// <summary>
    /// Gets the authentication header based on the configured connection type.
    /// </summary>
    /// <returns>The <see cref="AuthorizationHeader"/> for the connection.</returns>
    /// <exception cref="NopException">Thrown when an unsupported connection type is configured.</exception>
    private AuthorizationHeader GetAuthentication()
    {
        var connectionType = (ConnectionType)_elasticsearchSettings.ConnectionType;

        return connectionType switch
        {
            ConnectionType.Api => new ApiKey(_elasticsearchSettings.ApiKey),
            ConnectionType.Basic => new BasicAuthentication(_elasticsearchSettings.Username, _elasticsearchSettings.Password),
            ConnectionType.Cloud => new ApiKey(_elasticsearchSettings.ApiKey),
            _ => throw new NopException("Unsupported connection type")
        };
    }
    #endregion

    #region Methods
    /// <summary>
    /// Asynchronously gets the Elasticsearch client. If the client is not already initialized,
    /// it initializes the client before returning it.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="ElasticsearchClient"/> instance.</returns>
    public async Task<ElasticsearchClient> GetClientAsync()
    {
        if (_client == null)
        {
            lock (_lock)
            {
                if (_client == null)
                {
                    _initializationTask = InitializeClientAsync();
                }
            }

            await _initializationTask;
        }

        return _client;
    }
    #endregion
}