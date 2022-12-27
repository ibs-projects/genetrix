using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace genetrix.Models
{
    public class DFX:Reference
    {
        public DFX():base()
        {

        }

        public int? NbrTelechargement { get; set; }
        public string Numero { get; set; } = "DFX 420";

        [Display(Name = "Libellé")]
        public string NumeroAnnexe { get; set; } = "Annexe 1";
        public string CorrespondantB{ get; set; }
        [Display(Name="Date de début")]
        public DateTime DateDebut { get; set; }
        [Display(Name = "Date de fin")]
        public DateTime DateFin{ get; set; }
        public int? Telechargements { get; set; }
        [NotMapped]
        public string GetIdsDossiers { get; set; }
        public string Periode
        {
            get {
                try
                {
                  return DateDebut.ToString("dd/MM/yyyy") + " au " + DateFin.ToString("dd/MM/yyyy");
                }
                catch (Exception)
                {}
                return ""; 
            }
        }
    }
}