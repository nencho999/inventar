using System.ComponentModel.DataAnnotations;

namespace Inventar.Data.Models
{
    public class PrimaryMaterialBase
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public ICollection<Capacity> Capacities { get; set; } = new List<Capacity>();
        public ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
        public ICollection<RecurringExpense> RecurringExpenses { get; set; } = new List<RecurringExpense>();
    }
}
