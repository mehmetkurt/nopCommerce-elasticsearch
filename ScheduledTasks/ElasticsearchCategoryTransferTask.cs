using Nop.Services.ScheduleTasks;

namespace Nop.Plugin.Misc.Elasticsearch.ScheduledTasks
{
    /// <summary>
    /// Represents a scheduled task for transferring categories to Elasticsearch.
    /// </summary>
    public class ElasticsearchCategoryTransferTask : IScheduleTask
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
}
