namespace Inventar.Data.Models;

public class SalesPointExpense
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; }
    public decimal Amount { get; set; }

    public bool IsRecurring { get; set; }

    public ExpenseFrequency? Frequency { get; set; }

    public int? CustomIntervalCount { get; set; }

    public DateTime? OneTimeDate { get; set; }

    public Guid SalesPointId { get; set; }
    public SalesPoint SalesPoint { get; set; }
}