using AutoMapper;
using Microsoft.AspNetCore.Components;
using OnlineStore.JWTAuthentication.Services.Contracts;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;
using OnlineStore.Services.Frontend.Contracts;

namespace OnlineStore.Services.Frontend
{
    public class AddressFrontendService : IAddressFrontendService
    {
        private readonly IApiClient _apiClient;
        private readonly IMapper _mapper;

        public AddressFrontendService(IApiClient apiClient, IMapper mapper)
        {
            _apiClient = apiClient;
            _mapper = mapper;
        }

        public async Task<bool> AddAddressAsync(AddressModel addressModel)
        {
            AddressDTO addressDTO = _mapper.Map<AddressDTO>(addressModel);

            var response = await _apiClient.PostAsJsonAsync("api/address/add", addressDTO);

            if(response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }

        public async Task<List<AddressModel>?> GetAddressesByUser()
        {
            var addressDTOList = await _apiClient.GetAsync<List<AddressDTO>>("api/address");

            if(addressDTOList == null)
            {
                return null;
            }

            var addressModelList = _mapper.Map<List<AddressModel>>(addressDTOList);

            return addressModelList;
        }

        public async Task<AddressModel?> GetAddressModelById(int addressId)
        {
            var addressDTO = await _apiClient.GetAsync<AddressDTO>($"api/address/get/{addressId}");

            if (addressDTO == null)
            {
                return null;
            }

            var addressModel = _mapper.Map<AddressModel>(addressDTO);
            return addressModel;
        }
    }
}
