using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace genetrix.Models
{
    [Table("ReferenceNotification")]
    public class ReferenceNotification 
    {
        public int Id { get; set; }

        private string ReferenceBanqueId { get; set; }
        public ReferenceBanque ReferenceBanque { get; set; }

        private string NotificationId { get; set; }
        public NotifReference Notification { get; set; }


        private DateTime date;

        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        private bool desactive;

        public bool Desactive
        {
            get { return desactive; }
            set { desactive = value; }
        }

        private bool lue;
        [Browsable(false)]
        public bool Lue
        {
            get { return lue; }
            set { lue = value; }
        }

        private string title;

        public string Titre
        {
            get { return title; }
            set { title = value; }
        }

        private string detail;

        public string Detail
        {
            get { return detail; }
            set { detail = value; }
        }


    }
}