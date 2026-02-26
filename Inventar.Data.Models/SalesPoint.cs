namespace Inventar.Data.Models;

public class SalesPoint
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Description { get; set; }

    public SalesPointType Type { get; set; }
    public WarehouseStatus Status { get; set; }

    public ICollection<SalesPointProduct> SalesPointProducts { get; set; } = new List<SalesPointProduct>();
    public ICollection<SalesPointExpense> Expenses { get; set; } = new List<SalesPointExpense>();
}