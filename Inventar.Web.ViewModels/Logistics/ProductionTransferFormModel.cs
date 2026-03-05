using Inventar.Web.ViewModels.DropDowns;
using System.ComponentModel.DataAnnotations;

public class ProductionTransferFormModel
{
    [Required]
    public Guid FromProductionCenterId { get; set; }

    [Required]
    public Guid ToWarehouseId { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    [Required]
    public int Quantity { get; set; }
    public IEnumerable<ProductionCentersDropDown> ProductionCenters { get; set; } = new List<ProductionCentersDropDown>();
    public IEnumerable<WarehouseDropDownModel> Warehouses { get; set; } = new List<WarehouseDropDownModel>();
}