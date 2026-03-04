using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventar.Web.ViewModels.Warehouse
{
    public class WarehouseProductSelectionViewModel
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public bool IsSelected { get; set; }
        public double Quantity { get; set; }
    }
}
