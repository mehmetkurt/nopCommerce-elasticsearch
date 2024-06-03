using Nop.Core;

namespace Nop.Plugin.Misc.Elasticsearch.Data.Domain;
public class EntityTransfer : BaseEntity
{
    public string EntityName { get; set; }
    public int EntityId { get; set; }
    public bool Ignored { get; set; }
    public int OperationTypeId { get; set; }
    public DateTime CreatedDateUtc { get; set; }
    public DateTime UpdatedDateUtc { get; set; }

    public OperationType OperationType
    {
        get => (OperationType)OperationTypeId;
        set => OperationTypeId = (int)value;
    }
}
