using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace genetrix.Models
{
    public class CompteNostro
    {
        [Key]
        public int Id { get; set; }
        [Display(Name ="N° de compte")]
        public string Numero { get; set; }

        [Display(Name ="Clé"),StringLength(2)]
        public string Cle { get; set; }

        [Display(Name = "Libellé compte")]
        public string Libellé { get; set; }
        public string RIB { get; set; }

        [ForeignKey("Devise")]
        public int? IdDevise { get; set; } = null;
        public virtual DeviseMonetaire Devise { get; set; }

        [ForeignKey("Correspondant")]
        public int IdCorrespondant { get; set; }
        public virtual Correspondant Correspondant{ get; set; }

        public string GetCorrespondantNB
        {
            get {
                try
                {
                    if (Correspondant!=null)
                    {
                        return Correspondant.NomBanque;
                    }
                }
                catch (Exception)
                {}
                return ""; 
            }
        }
        
        public string GetDevise
        {
            get {
                try
                {
                    if (Devise!=null)
                    {
                        return Devise.Nom;
                    }
                }
                catch (Exception)
                {}
                return ""; 
            }
        }

    }
}