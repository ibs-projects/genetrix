using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace genetrix.Models
{
    public class Importateur:Contact
    {
        public Importateur()
        {
            Groupe = Groupe.Importateur;
        }

        [Display(Name ="Code d'agrément")]
        public string CodeAgrement { get; set; }
        [Display(Name ="Obtention")]
        public string DateOptention { get; set; }
        
        [Display(Name ="Code")]
        public string Code { get; set; }

        [Display(Name = "Immatriculation statique")]
        public string Immatriculation { get; set; }

        [Display(Name = "Numéro d'inscription au registre de commerce")]
        public string NumInscri { get; set; }

        [NotMapped]
        public int IdDossier { get; set; }
    }
}