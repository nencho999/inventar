namespace Inventar.Web.ViewModels;

public class MaterialViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Unit { get; set; } = null!;

    public string DisplayName => $"{Name} ({Unit})";
}