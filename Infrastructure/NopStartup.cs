using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.Elasticsearch.Repositories;
using Nop.Plugin.Misc.Elasticsearch.Services;

namespace Nop.Plugin.Misc.Elasticsearch.Infrastructure;
public class NopStartup : INopStartup
{
    public int Order => int.MaxValue;

    public void Configure(IApplicationBuilder application)
    {
    }

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IElasticsearchService, ElasticsearchService>();
        services.AddSingleton<IElasticsearchConnectionManager, ElasticsearchConnectionManager>();
        services.AddTransient(typeof(IElasticsearchRepository<>), typeof(ElasticsearchRepository<>));
    }
}
