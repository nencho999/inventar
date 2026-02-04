namespace Inventar.Web.ViewModels.SalesPoint;

public class ExpenseListViewModel
{
    public Guid SalesPointId { get; set; }
    public string SalesPointName { get; set; }
    public string ExpenseName { get; set; }
    public decimal Amount { get; set; }
    public string DateOrFrequency { get; set; }
}