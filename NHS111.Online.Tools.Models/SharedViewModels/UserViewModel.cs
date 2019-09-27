using System.ComponentModel.DataAnnotations;

namespace NHS111.Online.Tools.Models.SharedViewModels
{
    public class UserViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public string Status { get; set; }
    }
}
