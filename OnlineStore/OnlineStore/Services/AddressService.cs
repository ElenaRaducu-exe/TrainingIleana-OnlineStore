using AutoMapper;
using OnlineStore.Data;
using OnlineStore.DBModels;
using OnlineStore.Models.DTOs;
using OnlineStore.Services.Contracts;

namespace OnlineStore.Services
{
    public class AddressService : IAddressService
    {
        private readonly OnlineStoreContext _dbContext;
        private readonly IMapper _mapper;

        public AddressService(OnlineStoreContext onlineStoreContext,IMapper mapper)
        {
            _dbContext = onlineStoreContext;
            _mapper = mapper;
        }

        public async Task<bool> AddAddressAsync(AddressDTO adressDTO)
        {
            var address = _mapper.Map<Address>(adressDTO);

            _dbContext.Addresses.Add(address); 

            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
