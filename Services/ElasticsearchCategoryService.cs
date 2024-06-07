using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Services;
/// <summary>
/// Represents a service for managing categories stored in Elasticsearch, extending the base <see cref="CategoryService"/>.
/// </summary>
/// <remarks>
/// This service provides methods to retrieve and manage categories with additional support for Elasticsearch operations.
/// </remarks>
public class ElasticsearchCategoryService : CategoryService, IElasticsearchCategoryService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticsearchCategoryService"/> class.
    /// </summary>
    /// <param name="aclService">The ACL service used to apply access control list constraints.</param>
    /// <param name="workContext">The work context for accessing information about the current request and user.</param>
    /// <param name="storeContext">The store context for accessing information about the current store.</param>
    /// <param name="customerService">The customer service for handling customer-related operations.</param>
    /// <param name="productRepository">The repository for accessing product data.</param>
    /// <param name="staticCacheManager">The static cache manager for handling caching operations.</param>
    /// <param name="localizationService">The localization service for handling localization operations.</param>
    /// <param name="storeMappingService">The store mapping service for applying store mapping constraints.</param>
    /// <param name="categoryRepository">The repository for accessing category data.</param>
    /// <param name="productCategoryRepository">The repository for accessing product-category mapping data.</param>
    /// <param name="discountCategoryMappingRepository">The repository for accessing discount-category mapping data.</param>
    /// <remarks>
    /// This constructor initializes the service with dependencies necessary for managing categories, 
    /// including ACL constraints, store mappings, and localization. 
    /// It extends the base <see cref="CategoryService"/> to include additional functionality specific to Elasticsearch.
    /// </remarks>
    public ElasticsearchCategoryService
    (
        IAclService aclService,
        IWorkContext workContext,
        IStoreContext storeContext,
        ICustomerService customerService,
        IRepository<Product> productRepository,
        IStaticCacheManager staticCacheManager,
        ILocalizationService localizationService,
        IStoreMappingService storeMappingService,
        IRepository<Category> categoryRepository,
        IRepository<ProductCategory> productCategoryRepository,
        IRepository<DiscountCategoryMapping> discountCategoryMappingRepository
    ) : base(aclService, customerService, localizationService, categoryRepository, discountCategoryMappingRepository, productRepository, productCategoryRepository, staticCacheManager, storeContext, storeMappingService, workContext)
    {
    }

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
    public virtual async Task<IPagedList<Category>> GetAllCategoriesAsync(int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false, bool? overridePublished = null, bool getOnlyTotalCount = false)
    {
        var unsortedCategories = await _categoryRepository.GetAllPagedAsync(async query =>
        {
            if (!showHidden)
            {
                query = query.Where(c => c.Published);
            }
            else if (overridePublished.HasValue)
            {
                query = query.Where(c => c.Published == overridePublished.Value);
            }

            if (!showHidden || storeId > 0)
            {
                // Apply store mapping constraints
                query = await _storeMappingService.ApplyStoreMapping(query, storeId);
            }

            if (!showHidden)
            {
                // Apply ACL constraints
                var customer = await _workContext.GetCurrentCustomerAsync();
                query = await _aclService.ApplyAcl(query, customer);
            }

            return query.Where(c => !c.Deleted);
        }, pageIndex, pageSize, getOnlyTotalCount);

        if (getOnlyTotalCount)
        {
            return unsortedCategories;
        }

        // Sort categories
        var sortedCategories = SortCategoriesForTree(unsortedCategories.ToLookup(c => c.ParentCategoryId))
            .ToList();

        return new PagedList<Category>(sortedCategories, pageIndex, pageSize);
    }

}
