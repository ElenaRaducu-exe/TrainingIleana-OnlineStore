using OnlineStore.Models.FrontendModels;

namespace OnlineStore.Services.Frontend.Contracts
{
    public interface IAddressFrontendService
    {
        Task<bool> AddAddressAsync(AddressModel addressModel);
        Task<List<AddressModel>?> GetAddressesByUser();
        Task<AddressModel?> GetAddressModelById(int addressId); 
    }
}
