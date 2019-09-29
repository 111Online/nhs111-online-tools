using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using NHS111.Online.Tools.Models.Security;

namespace NHS111.Online.Tools.Web.Helpers
{
    public static class PrincipalHelper
    {
        public static bool IsAdminUser(this IPrincipal user)
        {
            //return user.Identity.IsAuthenticated && user.IsInRole(Constants.WebToolsAdministratorsRole);
            return user.Identity.IsAuthenticated && user.IsInRole(Constants.WebToolsAdministratorsRole);
        }

        public static bool IsFeedbackUser(this IPrincipal user)
        {
            return user.Identity.IsAuthenticated && user.IsInRole(Constants.WebToolsFeedbackRole);
        }
    }
}
