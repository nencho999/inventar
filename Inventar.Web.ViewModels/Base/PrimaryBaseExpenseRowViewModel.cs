namespace Inventar.Web.ViewModels.Base;

public class PrimaryBaseExpenseRowViewModel
{
    public Guid BaseId { get; set; }
    public string BaseName { get; set; }
    public string ExpenseName { get; set; }
    public decimal Amount { get; set; }
    public string DateOrFrequency { get; set; }
    public bool IsRecurring { get; set; }
}