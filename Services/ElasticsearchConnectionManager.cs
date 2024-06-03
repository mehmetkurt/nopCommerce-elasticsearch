using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Nop.Core;
using Nop.Plugin.Misc.Elasticsearch.Settings;
using Nop.Services.Logging;
using Nop.Services.Messages;

namespace Nop.Plugin.Misc.Elasticsearch.Services;

/// <summary>
/// Manages the Elasticsearch connection and provides a singleton instance of ElasticsearchClient.
/// </summary>
public class ElasticsearchConnectionManager : IElasticsearchConnectionManager
{
    #region Fields
    private Task _initializationTask;
    private ElasticsearchClient _client;
    private ElasticsearchClientSettings _settings;

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

        _settings = new ElasticsearchClientSettings(new Uri(_elasticsearchSettings.Hostnames))
            .Authentication(GetAuthentication());

        if (_elasticsearchSettings.UseFingerprint)
            _settings.CertificateFingerprint(_elasticsearchSettings.Fingerprint);

        _client = new ElasticsearchClient(_settings);
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