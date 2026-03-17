using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventar.Web.ViewModels.Logistics
{
    public class SalesActivityViewModel
    {
        [Required]
        public Guid SalesPointId { get; set; }

        public IEnumerable<SelectListItem> SalesPoints { get; set; } = new List<SelectListItem>();

        public bool IsVatApplicable { get; set; }

        public List<SalesProductItemViewModel> Products { get; set; } = new List<SalesProductItemViewModel>();
    }
}
