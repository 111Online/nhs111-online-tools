using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace NHS111.Online.Tools.Models.Identity
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole(string name) : base(name) { }
        public ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}
