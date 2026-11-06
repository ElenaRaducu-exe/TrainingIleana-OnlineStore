using AutoMapper;
using OnlineStore.JWTAuthentication.Services.Contracts;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;
using OnlineStore.Services.Frontend.Contracts;
using OnlineStore.DBModels;

namespace OnlineStore.Services.Frontend
{
    public class UserFrontendService : IUserFrontendService
    {
        private readonly IApiClient _apiClient;
        private readonly IMapper _mapper;

        public UserFrontendService(IApiClient apiClient, IMapper mapper)
        {
            _apiClient = apiClient;
            _mapper = mapper;
        }

        public async Task<bool> CreateUser(UserModel newUserModel)
        {
            var userDTO = _mapper.Map<UserDTO>(newUserModel);

            var responseAddingUser = await _apiClient.PostAsJsonAsync("api/users/add", userDTO);

            if (responseAddingUser.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> UpdateUserActiveMode(User user)
        {
            var responseUpdatingStatusUser = await _apiClient.PutAsJsonAsync($"api/users/dashboard/update/active/{user.Id}", user);

            if (responseUpdatingStatusUser.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }

        public async Task<List<User>> GetUsers()
        {
            var users = await _apiClient.GetAsync<List<User>>("api/users/dashboard");

            if (users == null)
            {
                return new List<User>();
            }

            return users;
        }
    }
}