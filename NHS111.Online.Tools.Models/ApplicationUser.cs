using Microsoft.AspNetCore.Identity;
using NHS111.Online.Tools.Models.Security;

namespace NHS111.Online.Tools.Models
{
    public class ApplicationUser : IdentityUser
    {
        public RegistrationStatus Status { get; set; }
    }
}
