using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace genetrix.Models
{
    public class DossierSupprime
    {
        public int Id { get; set; }
        public string Agence { get; set; }
        public string Gestionnaire { get; set; }
        [Display(Name ="Donneur d'ordre")]
        public string DonneurDordre { get; set; }
        [Display(Name = "Bénéficiaire")]
        public string Benefic { get; set; }
        public string Devise { get; set; }
        [Display(Name = "Montant en devise")]
        public double MontantDev{ get; set; }
        [Display(Name = "Montant XAF")]
        public double MontantXaf{ get; set; }
        [Display(Name = "Date de traitement")]
        public string DateTraitement { get; set; }
        [Display(Name = "Date de depot")]
        public string DateDepot { get; set; }
        [Display(Name = "Date suppression")]
        public string DateSupp{ get; set; }
        public string UserName { get; set; }

    }
}