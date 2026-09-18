using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Models.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Stock { get; set; }

        [Required]
        public string? ImageUrl { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public int CategoryId {  get; set; }

        [Required]
        public int BrandId {  get; set; }
    }
}
