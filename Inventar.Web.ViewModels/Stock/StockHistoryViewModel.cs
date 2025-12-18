namespace Inventar.Web.ViewModels.Stock;

public class StockHistoryViewModel
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public string BaseName { get; set; } = null!;
    public string MaterialName { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public bool IsAcquisition { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = null!;
    public decimal? UnitPrice { get; set; }

    public decimal TotalPrice => (UnitPrice ?? 0) * Quantity;
}