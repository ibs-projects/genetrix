using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace genetrix.Models
{
    public class MailModel
    {
        public string To { get; set; }
        public List<string> CC { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}