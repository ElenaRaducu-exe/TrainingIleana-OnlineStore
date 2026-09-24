using AutoMapper;
using OnlineStore.DBModels;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;

namespace OnlineStore.Models.MappingProfiles
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile() 
        {
            // Basic mapping - properties with matching names are mapped automatically
            CreateMap<Product, ProductDTO>();

            // Reverse mapping for going both directions
            CreateMap<ProductDTO, Product>();

            // Or use ReverseMap() for bidirectional mapping
            //CreateMap<Product, ProductDTO>().ReverseMap();

            CreateMap<ProductDTO, ProductModel>();
            CreateMap<ProductDTO, ProductModel>().ReverseMap();
        }
    }
}
