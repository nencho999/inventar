using System.ComponentModel.DataAnnotations;
using Inventar.Data.Models;

namespace Inventar.Web.ViewModels.ProductionCenter;

public class CenterCreateInputModel
{
    [Required]
    public string Name { get; set; } = "Central Production Unit";
    public string? Location { get; set; }
    public string? Capacity { get; set; }
    public string? Contact { get; set; }
    public CenterStatus? Status { get; set; }
    public string? Expenses { get; set; }
    public List<StorageInputModel> Storages { get; set; } = new List<StorageInputModel>();
    public List<MaterialDropDownModel> Materials { get; set; } = new List<MaterialDropDownModel>();
}
public class StorageInputModel
{
    public Guid MaterialId { get; set; }
    public double MaxCapacity { get; set; }
}