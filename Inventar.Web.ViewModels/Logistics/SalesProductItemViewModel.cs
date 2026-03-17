using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventar.Web.ViewModels.Logistics
{
    public class SalesProductItemViewModel
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int? AvailableInStock { get; set; }
        public int? SoldQuantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
