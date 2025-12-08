using System.ComponentModel.DataAnnotations;

namespace Inventar.Web.ViewModels.BaseViewModels;

public class BaseEditViewModel
{
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<CapacityViewModel> Capacities { get; set; } = new List<CapacityViewModel>();
}