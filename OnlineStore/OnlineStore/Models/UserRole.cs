using System;
using System.Collections.Generic;

namespace OnlineStore.Models;

public partial class UserRole
{
    public int Id { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
