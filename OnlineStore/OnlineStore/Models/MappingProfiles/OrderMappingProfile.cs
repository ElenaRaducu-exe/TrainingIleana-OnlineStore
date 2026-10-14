using AutoMapper;

using OnlineStore.Models.DTOs;
using OnlineStore.Models.Enums;
using OnlineStore.Models.FrontendModels;
using OnlineStore.Models.StoredProcedureModels;

namespace OnlineStore.Models.MappingProfiles
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<OrderDetailsSP, OrderDetailsDTO>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (OrderStatusEnum)src.StatusId))
                .ReverseMap();

            CreateMap<OrderDetailsDTO, OrderModel>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ReverseMap();
        }   
    }
}
