using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHS111.Online.Tools.Models.SharedViewModels;

namespace NHS111.Online.Tools.Models.AccountViewModels
{
    public class RegisterViewModel : UserViewModel
    {
        public RegisterViewModel()
        {
            Roles = new List<SelectListItem>();
        }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public IEnumerable<SelectListItem> Roles { get; set; }
    }
}
