using Microsoft.AspNetCore.Components;

using OnlineStore.Models.FrontendModels;

namespace OnlineStore.Components.ReusableComponnets
{
    public partial class OrderDashboardCard : ComponentBase
    {
        [Parameter]
        public OrderModel? OrderModel { get; set; }
    }
}
