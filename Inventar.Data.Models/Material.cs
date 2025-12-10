using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventar.Data.Models
{
    public class Material
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public string Unit { get; set; } = null!;
    }
}
