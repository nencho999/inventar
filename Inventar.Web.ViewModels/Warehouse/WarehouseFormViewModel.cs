using Inventar.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventar.Web.ViewModels.Warehouse
{
    public class WarehouseFormViewModel
    {
        public Guid? Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Location { get; set; }

        public string? ContactInfo { get; set; }

        public string? Capacity { get; set; }

        [Required]
        public WarehouseStatus Status { get; set; }
    }
}
