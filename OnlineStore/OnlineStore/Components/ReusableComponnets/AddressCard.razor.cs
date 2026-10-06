using Microsoft.AspNetCore.Components;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;

namespace OnlineStore.Components.ReusableComponnets
{
    public partial class AddressCard : ComponentBase
    {
        [Parameter]
        public AddressModel addressModel { get; set; }

        [Parameter]
        public EventCallback<int> OnSelectedAddressId { get; set; }

        private async Task AddressCardClicked(int addressId)
        {
            await OnSelectedAddressId.InvokeAsync(addressId);
        }
    }
}
