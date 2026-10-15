using Microsoft.AspNetCore.Components;

using OnlineStore.Models.FrontendModels;

namespace OnlineStore.Components.ReusableComponnets
{
    public partial class OrderDashboardCard : ComponentBase
    {
        [Parameter]
        public OrderDetailsModel? OrderModel { get; set; }
    }
}
