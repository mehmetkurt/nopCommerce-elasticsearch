using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.SearchProvider.Elasticsearch.Factories;
using Nop.Plugin.SearchProvider.Elasticsearch.Repositories;
using Nop.Plugin.SearchProvider.Elasticsearch.Services;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Infrastructure;

/// <summary>
/// Represents the startup configuration for the Elasticsearch plugin.
/// </summary>
public class NopStartup : INopStartup
{
    /// <summary>
    /// Gets the order of this startup configuration implementation.
    /// </summary>
    public int Order => int.MaxValue;

    /// <summary>
    /// Configures the application services. This method is intentionally left empty.
    /// </summary>
    /// <param name="application">The application builder.</param>
    public void Configure(IApplicationBuilder application)
    {
    }

    /// <summary>
    /// Configures the services for the Elasticsearch plugin.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IElasticsearchCategoryService, ElasticsearchCategoryService>();
        services.AddTransient<IElasticsearchService, ElasticsearchService>();
        services.AddTransient<IEntityTransferService, EntityTransferService>();
        services.AddSingleton<IElasticsearchConnectionManager, ElasticsearchConnectionManager>();
        services.AddTransient(typeof(IElasticsearchRepository<>), typeof(ElasticsearchRepository<>));

        services.AddTransient<IEntityTransferModelFactory, EntityTransferModelFactory>();

        services.Configure<RazorViewEngineOptions>(options =>
        {
            options.ViewLocationExpanders.Add(new ThemeViewLocationExpander());
        });
    }
}
