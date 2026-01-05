namespace Inventar.Web.ViewModels.SalesPoint;

public class SalesPointListDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string Type { get; set; }
    public int ProductCount { get; set; }

    public decimal MonthlyExpenses { get; set; }
    public decimal OneTimeExpenses { get; set; }
}