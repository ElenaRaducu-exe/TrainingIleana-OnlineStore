using System;
using System.Collections.Generic;

namespace OnlineStore.DBModels;

public partial class CardDetail
{
    public int Id { get; set; }

    public string CardNumber { get; set; } = null!;

    public int UserId { get; set; }

    public virtual ICollection<OrderPayment> OrderPayments { get; set; } = new List<OrderPayment>();

    public virtual User User { get; set; } = null!;
}
