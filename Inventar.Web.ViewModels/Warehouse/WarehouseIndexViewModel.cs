using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventar.Web.ViewModels.Warehouse
{
    public class WarehouseIndexViewModel
    {
        public int TotalWarehouses { get; set; }
        public decimal TotalMonthlyExpenses { get; set; }
        public decimal TotalOneTimeExpenses { get; set; }

        public IEnumerable<Inventar.Data.Models.Warehouse> Warehouses { get; set; }
    }
}
