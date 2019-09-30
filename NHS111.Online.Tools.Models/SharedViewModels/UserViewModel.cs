using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NHS111.Online.Tools.Models.Identity;

namespace NHS111.Online.Tools.Models.SharedViewModels
{
    public class UserViewModel
    {
        public UserViewModel()
        {
            SelectedRoles = new List<string>();
        }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "User roles")]
        public IEnumerable<string> SelectedRoles { get; set; }
    }
}
