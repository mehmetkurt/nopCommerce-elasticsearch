using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.SearchProvider.Elasticsearch.Data.Domain;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Data.Builders;

/// <summary>
/// Represents a builder for the EntityTransfer entity.
/// </summary>
public class EntityTransferBuilder : NopEntityBuilder<EntityTransfer>
{
    /// <summary>
    /// Maps the EntityTransfer entity to its table representation.
    /// </summary>
    /// <param name="table">The fluent builder for creating a table.</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(EntityTransfer.EntityName)).AsString().NotNullable()
            .WithColumn(nameof(EntityTransfer.EntityId)).AsInt32().NotNullable()
            .WithColumn(nameof(EntityTransfer.Ignored)).AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn(nameof(EntityTransfer.OperationTypeId)).AsInt32().NotNullable();
    }
}
