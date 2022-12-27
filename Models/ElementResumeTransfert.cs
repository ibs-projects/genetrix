using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace genetrix.Models
{
    public class ElementResumeTransfert
    {
        [Display(Name = "Elément identifié")]
        public string Nom { get; set; }
        [Display(Name ="Date création")]
        public string DateCreation { get; set; }
        [Display(Name ="Date modification")]
        public string DateModif { get; set; }
        public string Description { get; set; }

        public string _Description
        {
            get {
                try
                {
                    if (string.IsNullOrEmpty(Description)) return "...";
                    return Description;
                }
                catch (Exception)
                {}
                return "..."; 
            }
        }

        [Display(Name ="Fichier(s) associé(s)")]
        public string Nbrfichier { get; set; }
        [Display(Name ="Bénéficiaire")]
        public string Beneficiaire { get; internal set; }
        public string Devise { get; internal set; }
        [Display(Name = "Equivalent XAF")]
        public string MontantCvString { get; internal set; }
        [Display(Name = "Equivalent XAF")]
        public double MontantCv { get; internal set; }
        [Display(Name = "Montant en devise")]
        public string MontantSting { get; internal set; }
        [Display(Name = "Montant en devise")]
        public double Montant { get; internal set; }
        [Display(Name = "Date de depôt")]
        public string DateDepotBanque { get; internal set; }
        [Display(Name = "Date de traitement")]
        public string DateTraitement { get; internal set; }
        public string GetCategorie { get; set; }
        public string GetReference { get; set; }
        public string GetAgenceName { get; set; }
        public string GetGestionnaire { get; internal set; }
        [Display(Name = "Donneur d'ordres")]
        public string Client { get; internal set; }

        public string Delai { get; internal set; }


    }
}