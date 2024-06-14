using AutoMapper;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.SearchProvider.Elasticsearch.Areas.Admin.Models.Transfers;
using Nop.Plugin.SearchProvider.Elasticsearch.Data.Domain;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Areas.Admin.Models;

/// <summary>
/// Configures AutoMapper profiles for mapping between domain and model classes.
/// </summary>
public class MapperConfiguration : Profile, IOrderedMapperProfile
{
    /// <summary>
    /// Gets the order of this mapper profile implementation.
    /// </summary>
    public int Order => 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapperConfiguration"/> class.
    /// </summary>
    public MapperConfiguration()
    {
        CreateEntityTransferMappings();
    }

    /// <summary>
    /// Configures mappings between <see cref="EntityTransfer"/> and <see cref="EntityTransferModel"/>.
    /// </summary>
    private void CreateEntityTransferMappings()
    {
        CreateMap<EntityTransfer, EntityTransferModel>().ReverseMap();
    }
}
