namespace Inventar.Web.ViewModels.SalesPoint;

public class SalesPointProductViewModel
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }

    public bool IsSelected { get; set; }

    public decimal PriceReduction { get; set; }

    public bool IsPercentage { get; set; }
    public decimal OriginalPrice { get; set; }
}