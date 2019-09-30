using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace NHS111.Online.Tools.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}