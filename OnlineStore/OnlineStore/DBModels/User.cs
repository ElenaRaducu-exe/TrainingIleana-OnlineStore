using System;
using System.Collections.Generic;

namespace OnlineStore.DBModels;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public bool IsActive { get; set; }

    public int? CustomerId { get; set; }

    public int RoleId { get; set; }

    public virtual ICollection<CardDetail> CardDetails { get; set; } = new List<CardDetail>();

    public virtual Cart? Cart { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual UserRole Role { get; set; } = null!;
}
