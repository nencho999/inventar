using System.ComponentModel.DataAnnotations;

namespace Inventar.Web.ViewModels.BaseViewModels;

public class CapacityViewModel
{
    public Guid Id { get; set; }

    [Required]
    public string Type { get; set; } = string.Empty;

    [Range(0, 100000)]
    public decimal Quantity { get; set; }

    public string Unit { get; set; } = string.Empty;
}