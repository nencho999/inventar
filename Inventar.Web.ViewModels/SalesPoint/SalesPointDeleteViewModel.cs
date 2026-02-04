namespace Inventar.Web.ViewModels.SalesPoint;

public class SalesPointDeleteViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string Type { get; set; }

    public int LinkedProductsCount { get; set; }
    public int LinkedExpensesCount { get; set; }
}