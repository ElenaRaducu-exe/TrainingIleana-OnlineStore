using Microsoft.AspNetCore.Components;
using OnlineStore.DBModels;
using OnlineStore.Models.FrontendModels;
using OnlineStore.Services.Frontend.Contracts;

namespace OnlineStore.Components.ReusableComponnets
{
    public partial class ProductCard : ComponentBase
    {
        [Parameter]
        public ProductModel ProductModel { get; set; }

        [Parameter]
        public EventCallback<int> OnSelectedId { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        private async Task ProductCardClicked(int ProductId)
        {
            await OnSelectedId.InvokeAsync(ProductId);
        }
    }
}
