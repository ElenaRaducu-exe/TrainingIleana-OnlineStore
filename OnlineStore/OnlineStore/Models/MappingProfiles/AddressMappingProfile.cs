using AutoMapper; 
using OnlineStore.DBModels;
using OnlineStore.Models.FrontendModels;
using OnlineStore.Models.DTOs;

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
        }
    }
}
