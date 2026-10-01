using System;
using System.Collections.Generic;

namespace OnlineStore.DBModels;

public partial class Order
{
    public int Id { get; set; }

    public string OrderNumber { get; set; } = null!;

    public DateTime OrderDate { get; set; }

    public int StatusId { get; set; }

    public int? AddressId { get; set; }

    public int? UserId { get; set; }

    public virtual Address? Address { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<OrderPayment> OrderPayments { get; set; } = new List<OrderPayment>();

    public virtual OrderStatus Status { get; set; } = null!;

    public virtual User? User { get; set; }
}
