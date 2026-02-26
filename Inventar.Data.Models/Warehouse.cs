using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventar.Data.Models
{
    public class Warehouse
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Location { get; set; }

        public string? ContactInfo { get; set; }

        public string? Capacity { get; set; }

        public WarehouseStatus Status { get; set; }
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
        public ICollection<RecurringExpense> RecurringExpenses { get; set; } = new List<RecurringExpense>();
        public ICollection<Capacity> Capacities { get; set; } = new List<Capacity>();
    }
}
