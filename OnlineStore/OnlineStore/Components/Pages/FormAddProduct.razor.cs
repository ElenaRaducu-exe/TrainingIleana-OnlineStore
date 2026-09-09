using Microsoft.AspNetCore.Components;
using OnlineStore.DBModels;
using OnlineStore.Models;

namespace OnlineStore.Components.Pages
{
    public partial class FormAddProduct : ComponentBase
    {
        [Inject]
        private HttpClient _httpClient { get; set; }

        [Inject]
        private IHttpClientFactory _httpClientFactory { get; set; }

        [Inject]
        private NavigationManager _navigation { get; set; }

        private ProductDTO _productDTO = new();

        private List<Brand> _brands = new();
        private List<Category> _categories = new();
        private string _message = string.Empty;

        private bool _priceError = false;
        private string _priceErrorMessage = string.Empty;
        private bool _stockError = false;
        private string _stockErrorMessage = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            _brands = await httpClient.GetFromJsonAsync<List<Brand>>("api/brands");
            _categories = await httpClient.GetFromJsonAsync<List<Category>>("api/categories");
        }

        private async Task AddProduct()
        {
            if (_productDTO.Stock < 0)
            {
                _stockError = true;
                _stockErrorMessage = "Stock has to be at least 0!";
            }

            if (_productDTO.Price <= 0)
            {
                _priceError = true;
                _priceErrorMessage = "Price can not be 0 or negative!";
            }

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            var response = await httpClient.PostAsJsonAsync("api/admin/products/add/product", _productDTO);

            if (response.IsSuccessStatusCode)
            {
                _productDTO = new ProductDTO();
                _message = "Product added successfully!";

                _stockError = false;
                _stockErrorMessage = string.Empty;
                _priceError = false;
                _priceErrorMessage = string.Empty;
            }
            else
            {
                _message = "Product added unsuccessfully!";
            }
        }
    }
}
