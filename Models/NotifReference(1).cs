using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace e_apurement.Models
{
    [Table("NotifReference")]
    public class NotifReference : Notifications
    {
        public ICollection<ReferenceNotification> References { get; set; }

    }
}