using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventar.Web.ViewModels.Warehouse
{
    public class CapacityViewModel
    {
        public Guid MaterialId { get; set; }
        public decimal CapacityLimit { get; set; }
    }
}
