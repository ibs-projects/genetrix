using genetrix.Models.Fonctions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace genetrix.Models
{
    [Table("DossierNotification")]
    public class DossierNotification : IDelateCustom
    {
        public bool Remove(ApplicationDbContext db)
        {
            try
            {
                db.GetDossierNotifications.Remove(this);
                return true;
            }
            catch (Exception)
            { }
            return false;
        }

        public int Id { get; set; }

        public string DossierId { get; set; }

        public virtual Dossier Dossier { get; set; }

        private string NotifDossierId{ get; set; }
         public NotifDossier Notification { get; set; }

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

        private EtatDossier typeNotif;

        public EtatDossier TypeNotification
        {
            get {
                try
                {
                    //typeNotif = Dossier.StatutDossier.StatutDossier.EtatDossier;
                }
                catch (Exception)
                {}
                return typeNotif; 
            }
        }


    }
}