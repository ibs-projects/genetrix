using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Text;

namespace genetrix.Models
{
    [Table("Notifications")]
    [Serializable()]
    public class Notifications 
    {
        public int Id { get; set; }

        private string notif;

        public string Message
        {
            get { return notif; }
            set { notif = value; }
        }

        private string sujet;

        public string Objet
        {
            get { return sujet; }
            set { sujet = value; }
        }


        private Color color;

        public Color Couleur
        {
            get { return color; }
            set { color = value; }
        }

        private string lienImg;
        [Browsable(false)]
        public string LienImage
        {
            get { return lienImg; }
            set { lienImg = value; }
        }

        private string TypeNotificationId { get; set; }

        public string DestinataireId { get; set; }
        public string DestinataireNom { get; set; }
        public string EmetteurNom { get; set; }
        public string EmetteurImage { get; set; }

        public DateTime? Date { get; set; }
        public bool Lu { get; set; }
        public int? DossierId { get; set; }

        public virtual TypeNotification TypeNotification { get; set; }


        byte[] _image;

        public byte[] Image
        {
            get { return _image; }
            set { _image = value; }
        }

        private bool memeEntreprise;
        public virtual bool EstDansEntreprise
        {
            get
            {
                try
                {
                    //if (SecuritySystem.CurrentUser is CompteClient)
                    {
                        //if ((SecuritySystem.CurrentUser as CompteClient).Client == Client)
                        {
                           // memeEntreprise = true;
                        }
                    }
                }
                catch (Exception)
                { }
                return memeEntreprise;
            }
        }

    }

}