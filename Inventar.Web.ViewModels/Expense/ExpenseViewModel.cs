namespace Inventar.Web.ViewModels.Expense;

public class ExpenseViewModel
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = null!;
    public string BaseName { get; set; } = null!;
}