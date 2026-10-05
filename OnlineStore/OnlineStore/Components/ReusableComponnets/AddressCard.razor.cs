using Microsoft.AspNetCore.Components;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;

namespace OnlineStore.Components.ReusableComponnets
{
    public partial class AddressCard : ComponentBase
    {
        [Parameter]
        public AddressModel addressModel { get; set; }
    }
}
