using System.ComponentModel.DataAnnotations;

namespace Inventar.Web.ViewModels.Stock;

public class StockTransactionViewModel
{
    [Required]
    public Guid BaseId { get; set; }

    [Required]
    public Guid MaterialId { get; set; }

    [Required]
    public bool IsAcquisition { get; set; } = true;

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Quantity { get; set; }

    [DataType(DataType.Date)]
    public DateTime TransactionDate { get; set; } = DateTime.Now;

    public string Notes { get; set; } = null!;

    public IEnumerable<DropdownPairViewModel>? Bases { get; set; }
    public IEnumerable<DropdownPairViewModel>? Materials { get; set; }
}