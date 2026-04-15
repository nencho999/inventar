using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventar.Web.ViewModels.Logistics
{
    public class SalesPointReturnViewModel
    {
        public Guid FromSalesPointId { get; set; }
        public string DestinationType { get; set; } = null!;

        public Guid ToDestinationId { get; set; }

        public IEnumerable<SelectListItem> SalesPoints { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Warehouses { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> ProductionCenters { get; set; } = new List<SelectListItem>();

        public List<ReturnProductSelection> Products { get; set; } = new List<ReturnProductSelection>();
    }
}
