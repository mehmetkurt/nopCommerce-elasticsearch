using FluentMigrator;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.SearchProvider.Elasticsearch.Data.Domain;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Data.Builders;
public class EntityTransferBuilder : NopEntityBuilder<EntityTransfer>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(EntityTransfer.EntityName)).AsString().NotNullable()
            .WithColumn(nameof(EntityTransfer.EntityId)).AsInt32().NotNullable()
            .WithColumn(nameof(EntityTransfer.Ignored)).AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn(nameof(EntityTransfer.OperationTypeId)).AsInt32().NotNullable()
            .WithColumn(nameof(EntityTransfer.CreatedDateUtc)).AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn(nameof(EntityTransfer.UpdatedDateUtc)).AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);
    }
}
