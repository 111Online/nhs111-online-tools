using System.ComponentModel.DataAnnotations;

namespace NHS111.Online.Tools.Models.RoleViewModels
{
    public class DeleteRoleViewModel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [Display(Name = "Role name")]
        public string Name { get; set; }
    }
}
