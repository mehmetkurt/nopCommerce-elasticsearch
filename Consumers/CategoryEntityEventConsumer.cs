using Nop.Core.Domain.Catalog;
using Nop.Plugin.SearchProvider.Elasticsearch.Services;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Consumers;

public class CategoryEntityEventConsumer : BaseEntityConsumer<Category>
{
    public CategoryEntityEventConsumer(IEntityTransferService entityTransferService) : base(entityTransferService)
    {
    }
}
