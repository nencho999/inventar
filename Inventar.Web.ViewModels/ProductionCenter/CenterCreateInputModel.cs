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
    public List<ExpenseInputModel> ExpensesList { get; set; } = new List<ExpenseInputModel>();
    public List<StorageInputModel> Storages { get; set; } = new List<StorageInputModel>();
    public List<ProductDropdownViewModel> Products { get; set; } = new List<ProductDropdownViewModel>();
}

public class ExpenseInputModel
{
    public string Name { get; set; } = null!;
    public decimal Amount { get; set; }
    public ExpenseType Type { get; set; }
    public DateTime? Date { get; set; }
    public Frequency? Frequency { get; set; }
    public int? EveryXMonths { get; set; }
}
public class StorageInputModel
{
    public Guid MaterialId { get; set; }
    public double CurrentStock { get; set; }
    public double MaxCapacity { get; set; }
}