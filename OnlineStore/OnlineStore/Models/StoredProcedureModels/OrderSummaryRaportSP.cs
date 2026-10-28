using OnlineStore.Models.Enums;

namespace OnlineStore.Models.StoredProcedureModels
{
    public class OrderSummaryRaportSP
    {
        // common information 
        public int AddressId { get; set; }
        public int UserId { get; set; }

        // order information
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public int ProductsNumber { get; set; }
        public int TotalQuantityItems { get; set; }
        public decimal TotalPrice { get; set; }
        public int StatusId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime ModifiedAt { get; set; }

        // address information
        public string? FirstNameAddress { get; set; }
        public string? LastNameAddress { get; set; }
        public string? Phone { get; set; }
        public string? Street { get; set; }
        public string? StreetNumber { get; set; }
        public string? City { get; set; }
        public string? ZipCode { get; set; }
        public string? BuildingName { get; set; }
        public string? Entrance { get; set; }
        public int? FloorNumber { get; set; }
        public int? Apartment { get; set; }

        // order items list
        public List<OrderItemDetailsSP> OrderItems { get; set; } = new List<OrderItemDetailsSP>();
    }
}
