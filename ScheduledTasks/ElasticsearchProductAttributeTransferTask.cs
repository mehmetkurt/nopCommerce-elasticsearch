using Nop.Services.ScheduleTasks;

namespace Nop.Plugin.SearchProvider.Elasticsearch.ScheduledTasks;

/// <summary>
/// Represents a scheduled task for transferring product attributes to Elasticsearch.
/// </summary>
public class ElasticsearchProductAttributeTransferTask : IScheduleTask
{
    /// <summary>
    /// Executes the scheduled task asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task ExecuteAsync()
    {
        await Task.CompletedTask;
    }
}
