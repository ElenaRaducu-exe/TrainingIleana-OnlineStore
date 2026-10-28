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

            CreateMap<OrderSummaryRaportSP, OrderItemDetailsSP>()
                .ForMember(dest => dest.OrderItemId, opt => opt.MapFrom(src => src.OrderItems))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.OrderItems))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.OrderItems))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.OrderItems))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.OrderItems))
                .ForMember(dest => dest.ProductDescription, opt => opt.MapFrom(src => src.OrderItems))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.OrderItems))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.OrderItems))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.OrderItems))
                .ReverseMap();

            CreateMap<OrderSummaryRaportSP, OrderDetailsDTO>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.AddressId, opt => opt.MapFrom(src => src.AddressId))
                .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => src.OrderNumber))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
                .ForMember(dest => dest.ProductsNumber, opt => opt.MapFrom(src => src.ProductsNumber))
                .ForMember(dest => dest.TotalQuantityItems, opt => opt.MapFrom(src => src.TotalQuantityItems))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (OrderStatusEnum)src.StatusId))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.ModifiedAt, opt => opt.MapFrom(src => src.ModifiedAt))
                .ReverseMap();
        }   
    }
}
