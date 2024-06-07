using Elastic.Clients.Elasticsearch;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.SearchProvider.Elasticsearch.Data.Domain;
using Nop.Plugin.SearchProvider.Elasticsearch.Repositories;
using Nop.Plugin.SearchProvider.Elasticsearch.Services;
using Nop.Services.Logging;
using Nop.Services.ScheduleTasks;

namespace Nop.Plugin.SearchProvider.Elasticsearch.ScheduledTasks;

/// <summary>
/// Represents a scheduled task for transferring categories to Elasticsearch.
/// </summary>
public class CategoryTransferTask : IScheduleTask
{
    private readonly ILogger _logger;
    private readonly IEntityTransferService _entityTransferService;
    private readonly IElasticsearchCategoryService _elasticsearchCategoryService;
    private readonly IElasticsearchRepository<Category> _elasticsearchCategoryRepository;
    private const int _pageSize = 5;

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
        IEntityTransferService entityTransferService,
        IElasticsearchCategoryService elasticsearchCategoryService,
        IElasticsearchRepository<Category> elasticsearchCategoryRepository
    )
    {
        _logger = logger;
        _entityTransferService = entityTransferService;
        _elasticsearchCategoryService = elasticsearchCategoryService;
        _elasticsearchCategoryRepository = elasticsearchCategoryRepository;
    }

    /// <inheritdoc />
    public async Task ExecuteAsync()
    {
        var entityName = nameof(Category);
        var summary = await _elasticsearchCategoryService.GetAllCategoriesAsync(pageIndex: 0, pageSize: _pageSize, getOnlyTotalCount: true);
        var totalCount = summary.TotalCount;
        var totalPages = summary.TotalPages;

        if (totalCount <= 0)
        {
            await Task.CompletedTask;
            return;
        }

        for (int i = 0; i < totalPages; i++)
        {
            var categories = await _elasticsearchCategoryService.GetAllCategoriesAsync(pageIndex: i, pageSize: _pageSize);

            Action<BulkAllResponse> onNext = async o =>
            {
                var insertEntityTransfers = new List<EntityTransfer>();
                var updateEntityTransfers = new List<EntityTransfer>();

                await foreach (var item in o.Items.ToAsyncEnumerable())
                {
                    var entityId = Convert.ToInt32(item.Id);
                    var entityTransfer = await _entityTransferService.GetEntityTransferAsync<Category>(entityId, entityName);

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

                        insertEntityTransfers.Add(entityTransfer);
                    }
                    else
                    {
                        //entityTransfer.UpdatedDateUtc = DateTime.UtcNow;
                        updateEntityTransfers.Add(entityTransfer);
                    }
                }

                if (insertEntityTransfers?.Count > 0)
                    await _entityTransferService.InsertEntityTransfersAsync(insertEntityTransfers);

                if (updateEntityTransfers?.Count > 0)
                    await _entityTransferService.UpdateEntityTransfersAsync(updateEntityTransfers);
            };

            await _elasticsearchCategoryRepository.BulkAllAsync(categories, onNext: onNext);
        }
    }
}
