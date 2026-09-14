using System;
using System.Collections.Generic;

namespace OnlineStore.DBModels;

public partial class Brand
{
    public int Id { get; set; }

    public string BrandName { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
