using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;

namespace genetrix.Models
{
    [NotMapped]
    public class AbsNotification //
    { 
        //public AbsNotification(Session session)
        //    : base(session)
        //{
        //}
        //public override void AfterConstruction()
        //{
        //    base.AfterConstruction();
        //}

        private int nbr;

        public int NbrItems
        {
            get { return nbr; }
            set { nbr = value; }
        }

        //private Notifications notification;

        //public Notifications Notifications
        //{
        //    get { return notification; }
        //    set { SetPropertyValue(nameof(Notifications), ref notification, value); }
        //}

        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        private string titre;

        public string Titre
        {
            get { return titre; }
            set { titre = value; }
        }

        private Color color;

        public Color Couleur
        {
            get { return color; }
            set { color = value; }
        }

        private string lien;

        public string Lien
        {
            get { return lien; }
            set { lien = value; }
        }

        public byte[] Image { get; set; }

    }
}