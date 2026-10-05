using OnlineStore.Models.DTOs;

namespace OnlineStore.Services.Contracts
{
    public interface IAddressService
    {
        Task<bool> AddAddressAsync(AddressDTO adressDTO, int userId);

        Task<List<AddressDTO>?> GetAddressesByUserId(int userId);
    }
}
