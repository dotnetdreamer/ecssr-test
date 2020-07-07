using AutoMapper;
using Ecssr.Api.Dto;
using Ecssr.Core.Domain;

namespace Ecssr.Web.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //product
            CreateMap<ProductDto, Product>()
                .ForMember(dest => dest.ProductPictures, opt => opt.Ignore());
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.Pictures, opt => opt.MapFrom(p => p.ProductPictures));

            //picture
            CreateMap<ProductPicture, ProductPictureDto>()
                .ReverseMap();

        }
    }
}