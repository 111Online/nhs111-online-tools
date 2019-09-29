using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using NHS111.Online.Tools.Models.Security;

namespace NHS111.Online.Tools.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public RegistrationStatus Status { get; set; }
        public ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}