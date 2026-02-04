namespace Inventar.Data.Models;

public class SalesPointProduct
{
    public Guid SalesPointId { get; set; }
    public SalesPoint SalesPoint { get; set; }

    public Guid ProductId { get; set; }
    public Material Product { get; set; }

    public bool IsSelected { get; set; } = true;

    public decimal PriceReductionValue { get; set; }
    public bool IsPercentage { get; set; }
}