using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace OnlineStore.Models.Enums
{
    public class OrderStatusExtension
    {
        public static string GetLabel(OrderStatusEnum status)
        {
            var field = status.GetType().GetField(status.ToString());
            var attribute = field?.GetCustomAttribute<DisplayAttribute>();
            return attribute?.GetName() ?? status.ToString();
        }

        public static string GetColor(OrderStatusEnum status)
        {
            var field = status.GetType().GetField(status.ToString());
            var attribute = field?.GetCustomAttribute<ColorAttribute>();
            return attribute?.HexColor ?? status.ToString();
        }

        public static string GetDescription(OrderStatusEnum status)
        {
            var field = status.GetType().GetField(status.ToString());
            var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? status.ToString();
        }
    }
}
