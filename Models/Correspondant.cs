using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace genetrix.Models
{
    public class Correspondant
    {
        [Key]
        public int Id { get; set; }

        [Display(Name ="Nom de la baqnue")]
        public string NomBanque { get; set; }
        [Display(Name ="SWIFT CODE")]
        public string SwiftCode { get; set; }
        public string Pays { get; set; }
        public string Ville { get; set; }

        //public string NumCompte { get; set; }

        [ForeignKey("Banque")]
        [Display(Name = "Compte Nostro")]
        public int BanqueId { get; set; }
        public virtual Banque Banque { get; set; }
        public virtual ICollection<CompteNostro> GetCompteNostros { get; set; }

    }
}