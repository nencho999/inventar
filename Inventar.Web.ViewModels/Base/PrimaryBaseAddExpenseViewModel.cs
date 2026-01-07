using System.ComponentModel.DataAnnotations;
using Inventar.Data.Models;

namespace Inventar.Web.ViewModels.PrimaryBase
{
    public class PrimaryBaseAddExpenseViewModel
    {
        public Guid BaseId { get; set; }

        public string BaseName { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        public bool IsRecurring { get; set; }

        public ExpenseFrequency Frequency { get; set; }

        [Range(1, 120)]
        public int IntervalMonths { get; set; } = 1;

        [DataType(DataType.Date)]
        public DateTime ExpenseDate { get; set; } = DateTime.Today;

        public string? Description { get; set; }
    }
}