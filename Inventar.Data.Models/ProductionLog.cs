using Inventar.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ProductionLog
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [ForeignKey(nameof(ProductionCenterId))]
    public Guid ProductionCenterId { get; set; }
    public ProductionCenter ProductionCenter { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(ProductId))]
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    [Required]
    public decimal Quantity { get; set; }

    [Required]
    public DateTime ProductionDate { get; set; }
}