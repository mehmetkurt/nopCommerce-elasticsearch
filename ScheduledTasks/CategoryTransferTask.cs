using Nop.Core.Domain.Catalog;
using Nop.Plugin.SearchProvider.Elasticsearch.Data.Domain;
using Nop.Plugin.SearchProvider.Elasticsearch.Services;
using Nop.Services.ScheduleTasks;

namespace Nop.Plugin.SearchProvider.Elasticsearch.ScheduledTasks;

/// <summary>
/// Represents a scheduled task for transferring categories to Elasticsearch.
/// </summary>
public class CategoryTransferTask : IScheduleTask
{
    private readonly IEntityTransferService _entityTransferService;

    public CategoryTransferTask
    (
        IEntityTransferService entityTransferService
    )
    {
        _entityTransferService = entityTransferService;
    }

    /// <summary>
    /// Executes the scheduled task asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task ExecuteAsync()
    {
        var categories = await _entityTransferService.GetNonTransferredEntitiesAsync<Category>(OperationType.Inserted);
    }
}
