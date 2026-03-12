using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventar.Web.ViewModels.Logistics
{
    public class WarehouseToSalesPointViewModel
    {
        [Required]
        public Guid FromWarehouseId { get; set; }

        [Required]
        public Guid ToSalesPointId { get; set; }

        public IEnumerable<SelectListItem> Warehouses { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> SalesPoints { get; set; } = new List<SelectListItem>();

        public List<TransferProductItemViewModel> Products { get; set; } = new List<TransferProductItemViewModel>();
    }
}
