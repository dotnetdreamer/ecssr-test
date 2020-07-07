using AutoMapper;
using Ecssr.Api.Dto;
using Ecssr.Core.Domain;

namespace Ecssr.Web.Mappings
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<ProductDto, Product>().ReverseMap();
        }
    }
}
