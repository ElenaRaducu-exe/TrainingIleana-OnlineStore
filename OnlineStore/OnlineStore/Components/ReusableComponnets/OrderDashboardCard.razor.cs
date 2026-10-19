using Microsoft.AspNetCore.Components;

using OnlineStore.Models.FrontendModels;
using OnlineStore.Models.Enums;

namespace OnlineStore.Components.ReusableComponnets
{
    public partial class OrderDashboardCard : ComponentBase
    {
        [Parameter]
        public OrderDetailsModel? OrderModel { get; set; }
    }
}
