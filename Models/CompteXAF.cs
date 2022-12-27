using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace genetrix.Models
{
    public class CompteXAF
    {
        [Key]
        public int Id { get; set; }
        [Display(Name ="N° compte XAF")]
        public string NumCompte { get; set; }
        [Display(Name = "Clé"), StringLength(2)]
        public string Cle { get; set; }
        public string RIB { get; set; }
        public string Libellé { get; set; }

        [ForeignKey("Banque")]
        public int BanqueId { get; set; }
        public virtual Banque Banque{ get; set; }
    }
}