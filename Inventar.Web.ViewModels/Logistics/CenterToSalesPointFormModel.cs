using Inventar.Web.ViewModels.DropDowns;
using System.ComponentModel.DataAnnotations;

public class CenterToSalesPointFormModel
{
    [Required]
    public Guid FromProductionCenterId { get; set; }

    [Required]
    public Guid ToSalesPointId { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    [Required]
    public int Quantity { get; set; }

    public IEnumerable<ProductionCentersDropDown> ProductionCenters { get; set; } = new List<ProductionCentersDropDown>();
    public IEnumerable<SalesPointDropDownModel> SalesPoints { get; set; } = new List<SalesPointDropDownModel>();
}