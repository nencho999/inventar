using Inventar.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace Inventar.Web.ViewModels.Expense;

public class ExpenseFormViewModel
{
    [Required]
    public Guid BaseId { get; set; }

    [Required] 
    public string Name { get; set; } = null!;

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    public bool IsRecurring { get; set; }

    [DataType(DataType.Date)]
    public DateTime ExpenseDate { get; set; } = DateTime.Now;

    public ExpenseFrequency Frequency { get; set; }

    public int? IntervalMonths { get; set; }

    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; } = DateTime.Now;

    public string? Description { get; set; }

    public IEnumerable<DropdownPairViewModel>? Bases { get; set; }
}