using System;
using System.Collections.Generic;

namespace OnlineStore.DBModels;

public partial class Cart
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public int UserId { get; set; }

    public int? CartItemId { get; set; }

    public virtual CartItem? CartItem { get; set; }

    public virtual User User { get; set; } = null!;
}
