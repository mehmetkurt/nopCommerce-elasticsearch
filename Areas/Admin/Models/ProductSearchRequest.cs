namespace Nop.Plugin.SearchProvider.Elasticsearch.Areas.Admin.Models;
public record ProductSearchRequest
{
    public string Name { get; set; }
    public string ShortDescription { get; set; }
}
