using Inventar.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventar.Web.ViewModels.Warehouse
{
    public class WarehouseAddExpenseViewModel
    {
        public Guid WarehouseId { get; set; }

        public string WarehouseName { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }

        public string? Description { get; set; }

        public bool IsRecurring { get; set; }

        public ExpenseFrequency Frequency { get; set; }

        public int IntervalMonths { get; set; } = 1;

        [DataType(DataType.Date)]
        public DateTime ExpenseDate { get; set; } = DateTime.Today;
    }
}
