using Nop.Core.Domain.Catalog;
using Nop.Plugin.SearchProvider.Elasticsearch.Services;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Consumers;

public class ProductEntityEventConsumer : BaseEntityConsumer<Category>
{
    public ProductEntityEventConsumer(IEntityTransferService entityTransferService) : base(entityTransferService)
    {
    }
}
