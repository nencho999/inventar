using System.ComponentModel.DataAnnotations;

namespace Inventar.Web.ViewModels.Base;

public class BaseFormViewModel
{
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Description { get; set; } = null!;
    public List<BaseCapacityViewModel> Capacities { get; set; } = new List<BaseCapacityViewModel>();
}