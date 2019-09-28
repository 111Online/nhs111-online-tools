using Microsoft.AspNetCore.Identity;

namespace NHS111.Online.Tools.Models
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole(string name) : base(name) { }
    }
}
