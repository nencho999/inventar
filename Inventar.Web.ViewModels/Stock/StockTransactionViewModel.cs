using System.ComponentModel.DataAnnotations;

namespace Inventar.Web.ViewModels.Stock;

public class StockTransactionViewModel : IValidatableObject
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

    [Range(0, double.MaxValue, ErrorMessage = "The price must be positive")]
    public decimal? UnitPrice { get; set; }
    public IEnumerable<DropdownPairViewModel>? Bases { get; set; }
    public IEnumerable<DropdownPairViewModel>? Materials { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (IsAcquisition && UnitPrice == null)
        {
            yield return new ValidationResult
            (
                "Unit Price is required when adding stock.",
                new[] {nameof(UnitPrice)}
            );
        }
    }
}