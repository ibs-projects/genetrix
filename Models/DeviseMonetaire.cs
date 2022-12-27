using genetrix.Models.Fonctions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Text;

namespace genetrix.Models
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

        [DisplayName("Nom (Euro: EUR; Dollar: USD)")]
        public string Nom
        {
            get { return nom; }
            set { nom = value; }
        }
        [Display(Name = "Parité XAF")]
        public double ParitéXAF { get; set; }
        [Display(Name ="Libellé")]
        public string Libelle { get; set; }

        public byte[] ImageLogo { get; set; }

        public virtual ICollection<CompteNostro> GetCompteNostros{ get; set; }

    }
}