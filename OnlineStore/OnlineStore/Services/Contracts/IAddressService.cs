using OnlineStore.Models.DTOs;

namespace OnlineStore.Services.Contracts
{
    public interface IAddressService
    {
        Task<bool> AddAddressAsync(AddressDTO adressDTO);
    }
}
