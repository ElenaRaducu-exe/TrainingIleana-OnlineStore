using System;
using System.Collections.Generic;

namespace OnlineStore.DBModels;

public partial class Address
{
    public int Id { get; set; }

    public string Street { get; set; } = null!;

    public string StreetNumber { get; set; } = null!;

    public string City { get; set; } = null!;

    public string ZipCode { get; set; } = null!;

    public string? BuildingName { get; set; }

    public string? Entrance { get; set; }

    public int? FloorNumber { get; set; }

    public int? Apartment { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public int? UserId { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual User? User { get; set; }
}
