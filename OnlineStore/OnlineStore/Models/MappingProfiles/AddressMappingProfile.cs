using AutoMapper;
using OnlineStore.DBModels;
using OnlineStore.Models.FrontendModels;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.StoredProcedureModels;

namespace OnlineStore.Models.MappingProfiles
{
    public class AddressMappingProfile : Profile
    {
        public AddressMappingProfile()
        {
            CreateMap<Address, AddressDTO>().ForMember(dest => dest.AddressId, opt => opt.MapFrom(src => src.Id));
            CreateMap<AddressDTO, Address>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AddressId));

            //CreateMap<AddressDTO, AdressModel>(); 
            CreateMap<AddressDTO, AddressModel>().ReverseMap(); 

            CreateMap<OrderSummaryRaportSP, AddressDTO>()
                .ForMember(dest => dest.AddressId, opt => opt.MapFrom(src => src.AddressId))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstNameAddress))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastNameAddress))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Street))
                .ForMember(dest => dest.StreetNumber, opt => opt.MapFrom(src => src.StreetNumber))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.ZipCode, opt => opt.MapFrom(src => src.ZipCode))
                .ForMember(dest => dest.BuildingName, opt => opt.MapFrom(src => src.BuildingName))
                .ForMember(dest => dest.Entrance, opt => opt.MapFrom(src => src.Entrance))
                .ForMember(dest => dest.FloorNumber, opt => opt.MapFrom(src => src.FloorNumber))
                .ForMember(dest => dest.Apartment, opt => opt.MapFrom(src => src.Apartment))
                .ReverseMap();
        }
    }
}
