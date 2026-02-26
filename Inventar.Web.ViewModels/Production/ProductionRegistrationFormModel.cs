using Inventar.Web.ViewModels.Production;
using System.ComponentModel.DataAnnotations;

public class ProductionRegistrationFormModel
{
    [Required]
    [Display(Name = "Production Center")]
    public Guid ProductionCenterId { get; set; }

    [Required]
    [Display(Name = "Product")]
    public Guid ProductId { get; set; }

    [Required]
    public double Quantity { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime ProductionDate { get; set; } = DateTime.Now;

    public IEnumerable<ProductionCentersDropDown> ProductionCenters { get; set; } = new List<ProductionCentersDropDown>();
    public IEnumerable<ProductSelectViewModel> Products { get; set; } = new List<ProductSelectViewModel>();
}
public class ProductionCentersDropDown
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}