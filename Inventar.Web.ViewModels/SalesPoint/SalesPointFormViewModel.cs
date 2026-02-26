using Inventar.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace Inventar.Web.ViewModels.SalesPoint;

public class SalesPointFormViewModel
{
    public Guid? Id { get; set; }

    public string Name { get; set; }

    public string Address { get; set; }

    public string? Description { get; set; }
    public WarehouseStatus Status { get; set; }

    [Required]
    public SalesPointType Type { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal FinalPrice { get; set; }

    public List<SalesPointProductViewModel> Products { get; set; } = new List<SalesPointProductViewModel>();
}