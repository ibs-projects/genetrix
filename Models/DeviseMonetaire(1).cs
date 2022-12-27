using eApurement.Models.Fonctions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Text;

namespace e_apurement.Models
{

    [DefaultProperty("Nom")]
    [Table("DeviseMonetaire")]
    public class DeviseMonetaire : IDelateCustom
    {
        public bool Remove(ApplicationDbContext db)
        {
            try
            {
                db.GetDeviseMonetaires.Remove(this);
                return true;
            }
            catch (Exception)
            { }
            return false;
        }

        public int Id { get; set; }

        private string nom;

        public string Nom
        {
            get { return nom; }
            set { nom = value; }
        }

        public byte[] ImageLogo { get; set; }

    }
}