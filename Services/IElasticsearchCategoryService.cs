using Nop.Core;
using Nop.Core.Domain.Catalog;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Services;

/// <summary>
/// Provides methods to interact with categories stored in Elasticsearch.
/// </summary>
/// <remarks>
/// This interface defines methods for retrieving categories from an Elasticsearch repository.
/// It includes options for filtering categories by store, pagination, visibility, and publication status.
/// </remarks>
public interface IElasticsearchCategoryService
{
    /// <summary>
    /// Gets all categories based on the specified parameters.
    /// </summary>
    /// <param name="storeId">The store identifier. Use 0 to get all records, or a specific store ID to filter by store.</param>
    /// <param name="pageIndex">The index of the page to retrieve. Use 0 for the first page.</param>
    /// <param name="pageSize">The number of items per page. Use <see cref="int.MaxValue"/> to retrieve all items.</param>
    /// <param name="showHidden">Indicates whether to show hidden records.</param>
    /// <param name="overridePublished">
    /// Indicates whether to override the "Published" property:
    /// - null: Process the "Published" property according to the "showHidden" parameter.
    /// - true: Load only "Published" categories.
    /// - false: Load only "Unpublished" categories.
    /// </param>
    /// <param name="getOnlyTotalCount">Indicates whether to only get the total count of items, without retrieving the actual items.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a paged list of categories if <paramref name="getOnlyTotalCount"/> is false; otherwise, a paged list with only the total count.
    /// </returns>
    /// <remarks>
    /// When <paramref name="getOnlyTotalCount"/> is true, the method returns a paged list containing only the total count of categories,
    /// without retrieving the actual category data. This can be useful for quickly determining the number of categories that match the specified criteria.
    /// </remarks>
    Task<IPagedList<Category>> GetAllCategoriesAsync(int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false, bool? overridePublished = null, bool getOnlyTotalCount = false);
}
