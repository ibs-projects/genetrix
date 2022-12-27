using eApurement.Models.Fonctions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Text;
using DisplayNameAttribute = System.ComponentModel.DisplayNameAttribute;

namespace e_apurement.Models
{
    [Table("ClFichier")]
    public class ClFichier : IDelateCustom
    {
        public bool Remove(ApplicationDbContext db)
        {
            try
            {
                db.GetClFichiers.Remove(this);
                return true;
            }
            catch (Exception)
            { }
            return false;
        }

        public int Id { get; set; }

        public string Url { get; set; }
        public byte[] FichierImage { get; set; }

        [DisplayName("Date numérisation")]
        public DateTime DateCreaApp { get; set; }

        [DisplayName("Dernière modif")]
        public DateTime DateModif { get; set; }

        public string ClientId { get; set; }

        public Client Client { get; set; }


        private bool memeEntreprise;
        public bool EstDansEntreprise
        {
            get
            {
                try
                {
                    //if (SecuritySystem.CurrentUser is CompteClient)
                    //{
                    //    if ((SecuritySystem.CurrentUser as CompteClient).Client == Client)
                    //    {
                    //        memeEntreprise = true;
                    //    }
                    //}
                }
                catch (Exception)
                { }
                return memeEntreprise;
            }
        }

        protected  void OnSaving()
        {
            try
            {
                if (DateModif < DateTime.Now)
                {
                    DateModif = DateTime.Now;
                }
            }
            catch (Exception)
            {}
        }

    }
}