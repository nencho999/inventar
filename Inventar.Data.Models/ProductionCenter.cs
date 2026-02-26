using Inventar.Data.Models;
using System.ComponentModel.DataAnnotations;

public class ProductionCenter
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = "Central production unit";

    public CenterStatus? Status { get; set; } = CenterStatus.Operational;

    public string? Location { get; set; }

    public string? Capacity { get; set; }

    public string? Contact { get; set; }

    public string? Expenses { get; set; }

    public virtual ICollection<ProductionCenterStorage> StorageCapacities { get; set; }
        = new List<ProductionCenterStorage>();
    public virtual ICollection<ProductionCenterExpense> ExpensesList { get; set; }
        = new List<ProductionCenterExpense>();

    public virtual ICollection<ProductionLog> ProductionLogs { get; set; }
        = new List<ProductionLog>();
}