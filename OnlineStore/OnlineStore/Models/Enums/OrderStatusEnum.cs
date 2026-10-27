using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Models.Enums
{
    public enum OrderStatusEnum
    {
        [Description("Waiting for order confirmation")]
        [Display(Name = nameof(Pending))]
        [Color("#FFAC1C")]
        Pending = 1,

        [Description("Order confirmed")]
        [Display(Name = nameof(Confirmed))]
        [Color("#40B5AD")]
        Confirmed = 2,

        [Description("Preparing the order")]
        [Display(Name = nameof(Processing))]
        [Color("#FF5F15")]
        Processing = 3,

        [Description("Order prepared, waiting for pickup")]
        [Display(Name = nameof(Prepared))]
        [Color("#0F52BA")]
        Prepared = 9,

        [Description("Order picked up by courier")]
        [Display(Name = nameof(PickedUp))]
        [Color("#6495ED")]
        PickedUp = 8,

        [Description("Shipment is transit")]
        [Display(Name = nameof(Shipped))]
        [Color("#7C3AED")]
        Shipped = 4,

        [Description("Order successfully delivered")]
        [Display(Name = nameof(Delivered))]
        [Color("#16A34A")]
        Delivered = 5,

        [Description("Order cancelled")]
        [Display(Name = nameof(Cancelled))]
        [Color("#D22B2B")]
        Cancelled = 6,

        [Description("Order returned")]
        [Display(Name = nameof(Returned))]
        [Color("#818589")]
        Returned = 7,

        [Description("Return requested")]
        [Display(Name = nameof(ReturnedRequested))]
        [Color("#818589")]
        ReturnedRequested = 10,

        [Description("Return picked up by courier")]
        [Display(Name = nameof(ReturnPickedUp))]
        [Color("#818589")]
        ReturnPickedUp = 11,

        [Description("Return is in transit")]
        [Display(Name = nameof(ReturnInTransit))]
        [Color("#818589")]
        ReturnInTransit = 12,

        [Description("Returned products under inspection")]
        [Display(Name = nameof(ReturnUnderInspection))]
        [Color("#818589")]
        ReturnUnderInspection = 13,

        [Description("Return approved")]
        [Display(Name = nameof(ReturnApproved))]
        [Color("#818589")]
        ReturnApproved = 14,

        [Description("Return is being processed")]
        [Display(Name = nameof(ReturnProcessing))]
        [Color("#818589")]
        ReturnProcessing = 15,

        [Description("Return rejected")]
        [Display(Name = nameof(ReturnRejected))]
        [Color("#818589")]
        ReturnRejected = 16,

        [Description("Products shipped back to customer")]
        [Display(Name = nameof(ReturnShippingBack))]
        [Color("#818589")]
        ReturnShippingBack = 17,

        [Description("Products delivered back to customer")]
        [Display(Name = nameof(ReturnDelivered))]
        [Color("#818589")]
        ReturnDelivered = 18,

        [Description("Return process closed")]
        [Display(Name = nameof(ReturnClosed))]
        [Color("#818589")]
        ReturnClosed = 19
    }
}
