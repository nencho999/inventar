namespace Inventar.Web.ViewModels.Product
{
    using System.ComponentModel.DataAnnotations;

    public class ProductFormModel
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        public string? ProductType { get; set; }

        public string? Package { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 1000000, ErrorMessage = "Price must be positive")]
        public decimal Price { get; set; }

        public decimal Gain { get; set; }
    }
}