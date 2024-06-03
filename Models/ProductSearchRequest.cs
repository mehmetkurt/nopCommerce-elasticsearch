namespace Nop.Plugin.Misc.Elasticsearch.Models;
public record ProductSearchRequest
{
    public string Name { get; set; }
    public string ShortDescription { get; set; }
}
