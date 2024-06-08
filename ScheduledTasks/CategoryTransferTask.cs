using Elastic.Clients.Elasticsearch;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.SearchProvider.Elasticsearch.Data.Domain;
using Nop.Plugin.SearchProvider.Elasticsearch.Repositories;
using Nop.Plugin.SearchProvider.Elasticsearch.Services;
using Nop.Plugin.SearchProvider.Elasticsearch.Settings;
using Nop.Services.Logging;
using Nop.Services.ScheduleTasks;

namespace Nop.Plugin.SearchProvider.Elasticsearch.ScheduledTasks;

/// <summary>
/// Represents a scheduled task for transferring categories to Elasticsearch.
/// </summary>
public class CategoryTransferTask : IScheduleTask
{
    private readonly ILogger _logger;
    private readonly ElasticsearchSettings _elasticsearchSettings;
    private readonly IEntityTransferService _entityTransferService;
    private readonly IElasticsearchCategoryService _elasticsearchCategoryService;
    private readonly IElasticsearchRepository<Category> _elasticsearchCategoryRepository;

    private readonly int _pageSize = 5;
    private readonly string _entityName = nameof(Category);
    private List<EntityTransfer> _insertEntityTransfers = [];
    private List<EntityTransfer> _updateEntityTransfers = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="CategoryTransferTask"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="entityTransferService">The entity transfer service.</param>
    /// <param name="elasticsearchCategoryService">The Elasticsearch category service.</param>
    /// <param name="elasticsearchCategoryRepository">The Elasticsearch category repository.</param>
    public CategoryTransferTask
    (
        ILogger logger,
        ElasticsearchSettings elasticsearchSettings,
        IEntityTransferService entityTransferService,
        IElasticsearchCategoryService elasticsearchCategoryService,
        IElasticsearchRepository<Category> elasticsearchCategoryRepository
    )
    {
        _logger = logger;
        _elasticsearchSettings = elasticsearchSettings;
        _entityTransferService = entityTransferService;
        _elasticsearchCategoryService = elasticsearchCategoryService;
        _elasticsearchCategoryRepository = elasticsearchCategoryRepository;
    }
    /// <summary>
    /// Processes each item in the BulkAllResponse asynchronously.
    /// </summary>
    /// <param name="response">The BulkAllResponse object containing items to process.</param>
    private async Task OnNextAsync(BulkAllResponse response)
    {
        // Iterate asynchronously through each item in the response
        await foreach (var item in response.Items.ToAsyncEnumerable())
        {
            // Convert item.Id to an integer
            var entityId = Convert.ToInt32(item.Id);
            // Retrieve the EntityTransfer object from the database
            var entityTransfer = await _entityTransferService.GetEntityTransferAsync<Category>(entityId, _entityName);

            // If entityTransfer is not found, create a new EntityTransfer object and add it to the list of entities to insert
            if (entityTransfer == null)
            {
                entityTransfer = new EntityTransfer
                {
                    EntityId = Convert.ToInt32(item.Id),
                    EntityName = nameof(Category),
                    Ignored = false,
                    OperationType = (item.Operation.Equals("index") ? OperationType.Inserted : (OperationType)Enum.Parse(typeof(OperationType), item.Operation, true)),
                    CreatedDateUtc = DateTime.UtcNow,
                    UpdatedDateUtc = DateTime.UtcNow,
                };

                _insertEntityTransfers.Add(entityTransfer);
            }
            // If entityTransfer is found, add it to the list of entities to update
            else
            {
                _updateEntityTransfers.Add(entityTransfer);
            }
        }
    }

    /// <summary>
    /// Handles post-processing tasks asynchronously after the execution completes.
    /// </summary>
    private async Task OnCompletedAsync()
    {
        // If there are items in the list of entities to insert, insert them into the database
        if (_insertEntityTransfers?.Count > 0)
            await _entityTransferService.InsertEntityTransfersAsync(_insertEntityTransfers);

        // If there are items in the list of entities to update, update them in the database
        if (_updateEntityTransfers?.Count > 0)
            await _entityTransferService.UpdateEntityTransfersAsync(_updateEntityTransfers);
    }

    /// <summary>
    /// Handles errors that occur during the execution asynchronously.
    /// </summary>
    /// <param name="exception">The exception that occurred.</param>
    private async Task OnErrorAsync(Exception exception)
    {
        // Log the exception that occurred during the execution of the Category Transfer task
        await _logger.ErrorAsync("An exception occurred while the Category Transfer task was running", exception);
    }

    /// <inheritdoc />
    /// <summary>
    /// Executes the Category Transfer task asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ExecuteAsync()
    {
        if (!_elasticsearchSettings.Active)
            return;

        // Retrieve summary information about all categories
        var summary = await _elasticsearchCategoryService.GetAllCategoriesAsync(pageIndex: 0, pageSize: _pageSize, getOnlyTotalCount: true);
        var totalCount = summary.TotalCount;
        var totalPages = summary.TotalPages;

        // If there are no categories, return immediately
        if (totalCount <= 0)
            return;

        // Iterate through each page of categories
        for (int i = 0; i < totalPages; i++)
        {
            // Retrieve categories for the current page
            var categories = await _elasticsearchCategoryService.GetAllCategoriesAsync(pageIndex: i, pageSize: _pageSize);

            // If there are no categories on the current page, continue to the next page
            if (categories?.Count <= 0)
                continue;

            // Bulk insert the categories asynchronously, handling onNext, onCompleted, and onError actions
            await _elasticsearchCategoryRepository.BulkAllAsync(categories,
                onNext: async o => await OnNextAsync(o),
                onCompleted: async () => await OnCompletedAsync(),
                onError: async (ex) => await OnErrorAsync(ex));
        }
    }
}
