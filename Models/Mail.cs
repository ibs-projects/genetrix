using genetrix.Models.Fonctions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace genetrix.Models
{
    [Table("Mail")]
    public class Mail : Notifications,IDelateCustom
    {
        public bool Remove(ApplicationDbContext db)
        {
            try
            {
                db.GetMails.Remove(this);
                return true;
            }
            catch (Exception)
            { }
            return false;
        }


        private string adresseEmet;
        [DisplayName("Adresse éméteur")]
        public string AdresseEmeteur
        {
            get { return adresseEmet; }
            set { adresseEmet = value; }
        }

        public string GetUserId
        {
            get
            {
                return DestinataireId;
            }
        }

        public string CC { get; set; }

        public bool Envoyé { get; set; }
        public bool Corbeille { get; set; }

        private string adresseDes;
        [DisplayName("Adresse destinataire")]
        public string AdresseDest
        {
            get { return adresseDes; }
            set { adresseDes = value; }
        }
        public virtual ICollection<FichierMail> GetFichierMails { get; set; }
        public virtual ICollection<MailSupprime> GetSuppressions { get; set; }
    }

    public class MailSupprime
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        [ForeignKey("Mail")]
        public int MailId { get; set; }
        public virtual Mail Mail { get; set; }
    }
}