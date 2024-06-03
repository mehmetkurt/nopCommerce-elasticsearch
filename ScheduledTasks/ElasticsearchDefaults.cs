namespace Nop.Plugin.SearchProvider.Elasticsearch;

/// <summary>
/// Contains default values and constants for scheduled tasks related to Elasticsearch.
/// </summary>
public static partial class ElasticsearchDefaults
{
    /// <summary>
    /// Contains default values and constants for scheduled tasks related to Elasticsearch.
    /// </summary>
    public static partial class ScheduledTask
    {
        /// <summary>
        /// Specifies the name of the scheduled task for transferring categories to Elasticsearch.
        /// </summary>
        public static string CategoryTransferTaskName => "Elasticsearch (Category)";

        /// <summary>
        /// Specifies the system name of the scheduled task for transferring categories to Elasticsearch.
        /// </summary>
        public static string CategoryTransferTaskType => "Nop.Plugin.SearchProvider.Elasticsearch.ScheduledTasks.ElasticsearchCategoryTransferTask, Nop.Plugin.SearchProvider.Elasticsearch";

        /// <summary>
        /// Specifies the name of the scheduled task for transferring products to Elasticsearch.
        /// </summary>
        public static string ProductTransferTaskName => "Elasticsearch (Product)";

        /// <summary>
        /// Specifies the system name of the scheduled task for transferring products to Elasticsearch.
        /// </summary>
        public static string ProductTransferTaskType => "Nop.Plugin.SearchProvider.Elasticsearch.ScheduledTasks.ElasticsearchProductTransferTask, Nop.Plugin.SearchProvider.Elasticsearch";

        /// <summary>
        /// Specifies the name of the scheduled task for transferring product attributes to Elasticsearch.
        /// </summary>
        public static string ProductAttributeTransferTaskName => "Elasticsearch (Product Attribute)";

        /// <summary>
        /// Specifies the system name of the scheduled task for transferring product attributes to Elasticsearch.
        /// </summary>
        public static string ProductAttributeTransferTaskType => "Nop.Plugin.SearchProvider.Elasticsearch.ScheduledTasks.ElasticsearchProductAttributeTransferTask, Nop.Plugin.SearchProvider.Elasticsearch";

        /// <summary>
        /// Specifies the name of the scheduled task for transferring product combinations to Elasticsearch.
        /// </summary>
        public static string ProductCombinationTransferTaskName => "Elasticsearch (Product Combination)";

        /// <summary>
        /// Specifies the system name of the scheduled task for transferring product combinations to Elasticsearch.
        /// </summary>
        public static string ProductCombinationTransferTaskType => "Nop.Plugin.SearchProvider.Elasticsearch.ScheduledTasks.ElasticsearchProductCombinationTransferTask, Nop.Plugin.SearchProvider.Elasticsearch";
    }
}
