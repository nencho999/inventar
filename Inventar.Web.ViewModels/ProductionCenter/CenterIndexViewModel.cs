using Inventar.Data.Models;

namespace Inventar.Web.ViewModels.ProductionCenter;
public class CenterIndexViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public CenterStatus? Status { get; set; }
    public string Location { get; set; }
    public string Contact { get; set; }
    public string Capacity { get; set; }
    public string Expenses { get; set; }
    public List<ExpenseInputModel> ExpensesList { get; set; } = new List<ExpenseInputModel>();
}