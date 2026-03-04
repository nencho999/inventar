using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventar.Web.ViewModels.Logistics
{
    public class TransferProductItemViewModel
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public double AvailableQuantity { get; set; }
        public double TransferQuantity { get; set; }
    }
}
