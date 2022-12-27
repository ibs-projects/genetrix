using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace genetrix.Models
{
    [Serializable()]
    public class UserSession
    {
        public string Id { get; set; }
        public string Type { get; set; }
    }
}