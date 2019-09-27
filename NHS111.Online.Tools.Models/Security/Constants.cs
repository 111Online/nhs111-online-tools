using System;
using System.Collections.Generic;
using System.Text;

namespace NHS111.Online.Tools.Models.Security
{
    public enum RegistrationStatus
    {
        Submitted,
        Approved,
        Rejected
    }
    public class Constants
    {
        public static readonly string WebToolsAdministratorsRole = "Admin";
        public static readonly string WebToolsFeedbackRole = "FeedbackView";
    }
}
