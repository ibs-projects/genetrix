using genetrix.Models;
using System;

namespace genetrix
{
    public class SecuritySystem
    {
        public static ApplicationUser CurrentUser { get; set; }
        public static string CurrentUserName { get; internal set; }
        public static Guid CurrentUserId { get; internal set; }
    }
}