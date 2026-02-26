using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventar.Web.ViewModels.Warehouse
{
    public class WarehouseListItemViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public Inventar.Data.Models.WarehouseStatus Status { get; set; }
        public decimal TotalCapacity { get; set; }
        public decimal UsedCapacity { get; set; }
        public decimal MonthlyExpensesTotal { get; set; }
        public decimal OneTimeExpensesTotal { get; set; }

        public double FillPercentage
        {
            get
            {
                if (TotalCapacity == 0) return 0;
                var percent = (double)(UsedCapacity / TotalCapacity) * 100;
                return Math.Min(percent, 100);
            }
        }
    }
}
