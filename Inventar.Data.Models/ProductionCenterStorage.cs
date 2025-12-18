using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventar.Data.Models
{
    public class ProductionCenterStorage
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid ProductionCenterId { get; set; }
        [ForeignKey(nameof(ProductionCenterId))]
        public ProductionCenter ProductionCenter { get; set; } = null!;

        [Required]
        public Guid MaterialId { get; set; }
        [ForeignKey(nameof(MaterialId))]
        public Material Material { get; set; } = null!;

        [Required]
        public double MaxStorageCapacity { get; set; }
    }
}
