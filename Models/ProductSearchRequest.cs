namespace Nop.Plugin.SearchProvider.Elasticsearch.Models;
public record ProductSearchRequest
{
    public string Name { get; set; }
    public string ShortDescription { get; set; }
}
