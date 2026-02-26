using Inventar.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace Inventar.Web.ViewModels.Base;

public class BaseFormViewModel
{
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Description { get; set; } = null!;
    public WarehouseStatus Status { get; set; }
    public List<BaseCapacityViewModel> Capacities { get; set; } = new List<BaseCapacityViewModel>();
}