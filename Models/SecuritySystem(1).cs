using e_apurement.Models;
using System;

namespace e_apurement
{
    public class SecuritySystem
    {
        public static ApplicationUser CurrentUser { get; set; }
        public static string CurrentUserName { get; internal set; }
        public static Guid CurrentUserId { get; internal set; }
    }
}