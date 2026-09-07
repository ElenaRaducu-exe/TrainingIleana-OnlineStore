using System;
using System.Collections.Generic;

namespace OnlineStore.DBModels;

public partial class Cart
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual User User { get; set; } = null!;
}
