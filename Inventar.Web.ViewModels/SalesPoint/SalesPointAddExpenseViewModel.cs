using Inventar.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace Inventar.Web.ViewModels.SalesPoint;

public class SalesPointAddExpenseViewModel
{
    public Guid SalesPointId { get; set; }

    public string SalesPointName { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    public bool IsRecurring { get; set; }

    public ExpenseFrequency Frequency { get; set; }

    [Range(1, 120)]
    public int CustomIntervalCount { get; set; } = 1;

    [DataType(DataType.Date)]
    public DateTime? OneTimeDate { get; set; } = DateTime.Today;

    public string? Description { get; set; }
}