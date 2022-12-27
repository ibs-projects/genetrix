using e_apurement;
using e_apurement.Models.Fonctions;
using e_apurement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using eApurement.Models.Fonctions;

namespace e_apurement.Models
{
    [DefaultProperty("Message")]
    [Table("Chat")]
    public class Chat :Notifications ,IDelateCustom
    {
        public bool Remove(ApplicationDbContext db)
        {
            try
            {
                db.Chats.Remove(this);
                return true;
            }
            catch (Exception)
            { }
            return false;
        }
      

        private DateTime dateTime;

        public DateTime DateHeure
        {
            get { return dateTime; }
            set { dateTime = value; }
        }

        public string EmetteurId { get; set; }
        public virtual ApplicationUser Emetteur { get; set; }

        public virtual ICollection<ApplicationUser> Destinataires { get; set; }

        private bool memeEntreprise;
       
    }
}