using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;
using OnlineStore.DBModels;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;
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

        public async Task<bool> AddAddressAsync(AddressDTO adressDTO, int userId)
        {
            var address = _mapper.Map<Address>(adressDTO);
            address.UserId = userId;

            _dbContext.Addresses.Add(address); 

            await _dbContext.SaveChangesAsync();

            return true;
        }

        
        public async Task<List<AddressDTO>?> GetAddressesByUserId(int userId)
        {
            var addressListByUserId = await _dbContext.Addresses.Where(a => a.UserId == userId).ToListAsync();

            var addressListDTO = _mapper.Map<List<AddressDTO>>(addressListByUserId);

            return addressListDTO;
        }
    }
}
