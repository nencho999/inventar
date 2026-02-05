using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventar.Data.Models
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }

        public string? Package { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Vat { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Gain { get; set; }

        public string? ProductType { get; set; } 
        public virtual ICollection<ProductionCenterStorage> ProductionCenters { get; set; } = new HashSet<ProductionCenterStorage>();
    }
}
