using System;
using System.Collections.Generic;

namespace OnlineStore.Models;

public partial class Address
{
    public int Id { get; set; }

    public string Street { get; set; } = null!;

    public string StreetNumber { get; set; } = null!;

    public string City { get; set; } = null!;

    public string ZipCode { get; set; } = null!;

    public string? BuildingName { get; set; }

    public string? Entrace { get; set; }

    public int? FloorNumber { get; set; }

    public int? Apartment { get; set; }

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
}
