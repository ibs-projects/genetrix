using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace genetrix.Models
{
    public class NumCompte
    {
        public int Id { get; set; }
        [Display(Name ="Numéro du compte")]
        [MaxLength(11),MinLength(11),StringLength(11)]
        public string Numero { get; set; }
        [StringLength(5),Display(Name ="Code agence")]
        public string CodeAgence { get; set; }
        [StringLength(2)]
        [Display(Name = "Clé")]
        public string Cle { get; set; }
        [Display(Name = "Intitulé du compte")]
        public string Nom { get; set; }
        [ForeignKey("BanqueClient")]
        public int IdBanqueClient { get; set; }
        public virtual BanqueClient BanqueClient { get; set; }
        public string RIB
        {
            get
            {
                try
                {
                    return $"{BanqueClient.CodeEts} {CodeAgence} {Numero} {Cle}";
                }
                catch (Exception)
                { }
                return "";
            }
        }

    }
}