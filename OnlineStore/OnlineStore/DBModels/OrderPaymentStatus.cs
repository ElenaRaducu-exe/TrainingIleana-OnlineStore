using System;
using System.Collections.Generic;

namespace OnlineStore.DBModels;

public partial class OrderPaymentStatus
{
    public int Id { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<OrderPayment> OrderPayments { get; set; } = new List<OrderPayment>();
}
