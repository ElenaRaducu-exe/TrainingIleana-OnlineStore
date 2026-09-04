using System;
using System.Collections.Generic;

namespace OnlineStore.Models;

public partial class OrderPayment
{
    public int Id { get; set; }

    public string PaytmentMethod { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public DateTime PaymentDate { get; set; }

    public int OrderId { get; set; }

    public int CardDetailsId { get; set; }

    public int StatusId { get; set; }

    public virtual CardDetail CardDetails { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;

    public virtual OrderPaymentStatus Status { get; set; } = null!;
}
