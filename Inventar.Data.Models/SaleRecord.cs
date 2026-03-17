using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventar.Data.Models
{
    public class SaleRecord
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime SaleDate { get; set; } = DateTime.Now;

        public Guid SalesPointId { get; set; }
        [ForeignKey(nameof(SalesPointId))]
        public SalesPoint SalesPoint { get; set; } = null!;

        public Guid ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;

        public int? Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalAmount { get; set; }

        public bool IsVatApplied { get; set; }
    }
}
