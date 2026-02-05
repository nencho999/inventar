namespace Inventar.Data.Models;

public class RecurringExpense
{
    public Guid Id { get; set; }

    public Guid? BaseId { get; set; }
    public PrimaryMaterialBase? Base { get; set; }

    public Guid? WarehouseId { get; set; }
    public Warehouse? Warehouse { get; set; }

    public string Name { get; set; }
    public decimal Amount { get; set; }

    public ExpenseFrequency Frequency { get; set; }

    public int? IntervalMonths { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime? NextDueDate { get; set; }
    public bool IsActive { get; set; } = true;
}