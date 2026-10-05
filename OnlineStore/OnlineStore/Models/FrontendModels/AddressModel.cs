using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Models.FrontendModels
{
    public class AddressModel
    {
        public int AddressId { get; set; }
        public int UserId { get; set; }

        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        [Required]
        public string? Phone { get; set; }

        [Required]
        public string? Street { get; set; }

        [Required]
        public string? StreetNumber { get; set; }

        [Required]
        public string? City { get; set; }

        [Required]
        public string? ZipCode { get; set; }

        [Required]
        public string? BuildingName { get; set; }

        [Required]
        public string? Entrance { get; set; }

        [Required]
        public int? FloorNumber { get; set; }

        [Required]
        public int? Apartment { get; set; }
    }
}
