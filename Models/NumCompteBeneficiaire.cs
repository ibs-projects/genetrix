using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace genetrix.Models
{
    public class NumCompteBeneficiaire
    {
        public int Id { get; set; }
        [Display(Name = "Clé")]
        public string Cle { get; set; }
        [Display(Name = "Numéro du compte"),StringLength(11)]
        public string Numero { get; set; }
        [Display(Name = "Intitulé du compte")]
        public string Nom { get; set; }
        [Display(Name = "Nom de la banque")]
        public string NomBanque { get; set; }
        [Display(Name = "Adresse de la banque")]
        public string Adresse { get; set; }
        [Display(Name = "Code agence"),StringLength(5)]
        public string CodeAgence { get; set; }
        [Display(Name = "Code Swift")]
        public string CodeSwift{ get; set; }
        public string Pays { get; set; }
        public string Ville { get; set; }
        [ForeignKey("Fournisseur")]
        public int IdFournisseur { get; set; }
        public virtual Fournisseurs Fournisseur { get; set; }

        public string CodeEts
        {
            get {
                try
                {
                    return Fournisseur.CodeEts;
                }
                catch (Exception)
                { }
                return ""; 
            }
        }

        public string AdresseBenf
        {
            get {
                try
                {
                    return Fournisseur.Adresse;
                }
                catch (Exception)
                { }
                return ""; 
            }
        }


        public string RIB
        {
            get {
                try
                {
                    return $"{Fournisseur.CodeEts} {CodeAgence} {Numero} {Cle}";
                }
                catch (Exception)
                {}
                return "";
            }
        }

    }
}