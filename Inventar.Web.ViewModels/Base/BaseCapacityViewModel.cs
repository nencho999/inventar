using System.ComponentModel.DataAnnotations;

namespace Inventar.Web.ViewModels.Base;

public class BaseCapacityViewModel
{
    public Guid MaterialId { get; set; }

    public string MaterialName { get; set; } = null!;

    [Range(0, double.MaxValue, ErrorMessage = "The limit must be positive number")]
    public decimal Limit { get; set; }
    public bool IsSelected { get; set; }
}