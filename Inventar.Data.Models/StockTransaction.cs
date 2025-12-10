using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Inventar.Data.Models;

public class StockTransaction
{
    public Guid Id { get; set; }

    public Guid BaseId { get; set; }
    public PrimaryMaterialBase? Base { get; set; }

    public Guid MaterialId { get; set; }
    public Material? Material { get; set; }

    [Required]
    public DateTime TransactionDate { get; set; } = DateTime.Now;

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal QuantityChange { get; set; }

    public string? CreatedByUserId { get; set; }
    public string? Notes { get; set; }
}