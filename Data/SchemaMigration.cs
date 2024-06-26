﻿using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.SearchProvider.Elasticsearch.Data.Domain;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Data;

[NopSchemaMigration("2023/06/03 07:20:00:1453071", "Nop.Plugin.SearchProvider.Elasticsearch base schema", MigrationProcessType.Installation)]
public class SchemaMigration : AutoReversingMigration
{
    public override void Up()
    {
        Create.TableFor<EntityTransfer>();
    }
}
