using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace genetrix.Models
{
    public class Historisation
    {
        public int Id { get; set; }
        [Display(Name = "Date de début")]
        public DateTime? DateDebut { get; set; } = DateTime.Now;
        [Display(Name ="Date de fin")]
        public DateTime? DateFin { get; set; }
        /// <summary>
        /// 0=Affectation; 1=PriseEnChargeCLient; 2=PriseEnchargeDossier; 3=PriseEnchargeStructure
        /// </summary>
        [Display(Name ="Type")]
        public short TypeHistorique { get; set; }
        public string Client { get; set; }
        public string Agent { get; set; }
        public string Structure { get; set; }
        public string Dossier { get; set; }
        public int? IdDossier { get; set; }
        public int? IdClient { get; set; }
        public int? IdStructure { get; set; }
        public string IdAgant { get; set; }
        public string Cible { get; set; }
    }

    public enum TypeHistorique
    {
        Affectation,
        PriseEnChargeCLient,
        PriseEnchargeDossier,
        PriseEnchargeStructure
    }
}