using Microsoft.AspNetCore.Components;
using OnlineStore.Models;

namespace OnlineStore.Components.Pages
{
    public partial class ProductDetailsPage : ComponentBase
    {
        [Inject]
        private IHttpClientFactory _httpClientFactory { get; set; }

        [Inject]
        private NavigationManager _navigation { get; set; }

        [Parameter]
        public int? Id { get; set; }

        protected ProductDTO productDTO { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_navigation.BaseUri);

            productDTO = await httpClient.GetFromJsonAsync<ProductDTO>($"api/admin/products/get/product/{Id}");
        }

        protected void DeleteProduct()
        {
            Console.WriteLine("------------ DeleteProduct");
        }
        protected void EditProduct()
        {
            Console.WriteLine("------------ EditProduct");
        }
    }
}
