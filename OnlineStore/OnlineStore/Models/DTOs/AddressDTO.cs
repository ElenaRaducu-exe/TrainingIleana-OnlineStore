using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Models.DTOs
{
    public class AddressDTO
    {
        public int AddressId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public string? Street { get; set; }
        public string? StreetNumber { get; set; }
        public string? City { get; set; }
        public string? ZipCode { get; set; }
        public string? BuildingName { get; set; }
        public string? Entrance { get; set; }
        public int? FloorNumber { get; set; }
        public int? Apartment { get; set; }
    }
}
