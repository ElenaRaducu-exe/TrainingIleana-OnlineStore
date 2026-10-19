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

            CreateMap<OrderDetailsDTO, OrderDetailsModel>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.StatusEnum, opt => opt.MapFrom(src => src.Status))
                .ReverseMap();

            CreateMap<OrderItemDetailsSP, OrderItemDetailsDTO>().ReverseMap(); 

            CreateMap<OrderItemDetailsDTO, OrderItemDetailsModel>().ReverseMap();

            CreateMap<OrderDetailsPageDTO, OrderDetailsPageModel>().ReverseMap();
        }   
    }
}
