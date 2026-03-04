using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventar.Data.Models
{
    public class WarehouseProduct
    {
        [Required]
        public Guid WarehouseId { get; set; }

        [ForeignKey(nameof(WarehouseId))]
        public Warehouse Warehouse { get; set; } = null!;

        [Required]
        public Guid ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;

        [Required]
        [Range(0, double.MaxValue)]
        public double Quantity { get; set; }
    }
}
