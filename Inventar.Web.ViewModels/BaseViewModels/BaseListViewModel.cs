namespace Inventar.Web.ViewModels.BaseViewModels;

public class BaseListViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string CapacitySummary { get; set; } = string.Empty;
}