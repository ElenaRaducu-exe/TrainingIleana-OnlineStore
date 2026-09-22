using AutoMapper;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;
using OnlineStore.Models.StoredProcedureModels;
using OnlineStore.DBModels; 

namespace OnlineStore.Models.MappingProfiles
{
    public class CartItemMappingProfile : Profile
    {
        public CartItemMappingProfile() 
        {
            CreateMap<CartItemSummaries, CartItemDTO>();
            CreateMap<CartItemSummaries, CartItemDTO>().ReverseMap();

            CreateMap<CartItemDTO, CartItemModel>(); 
            CreateMap<CartItemDTO, CartItemModel>().ReverseMap();
        }
    }
}
