using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace e_apurement.Models
{
    [Table("NotifDossier")]
    public class NotifDossier : Notifications
    {
        public int Id { get; set; }

        public override TypeNotification TypeNotification 
        { 
            get => base.TypeNotification;
            set => base.TypeNotification = value; 
        }

    }
}