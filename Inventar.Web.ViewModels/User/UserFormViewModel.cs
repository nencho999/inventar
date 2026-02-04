using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventar.Web.ViewModels.User
{
    public class UserFormViewModel
    {
        public string? Id { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Display(Name = "Phone Number")]
        [StringLength(15, MinimumLength = 10)]
        [RegularExpression(@"^[0-9\-\ ]+$")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string RoleId { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }
    }
}
