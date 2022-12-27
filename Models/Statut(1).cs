using e_apurement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace eApurement.Models
{
    public class Statut
    {
        public int Id { get; set; }
        public string MessageCLient { get; set; }
        public string MessageAgence { get; set; }
        public string CouleurSt { get; set; }
        public int? Etape { get; set; }

        // [NotMapped]
        public Color Couleur { get; set; }

        [DisplayName("Couleur")]
        public Int32 Argb
        {
            get
            {
                return Couleur.ToArgb();
            }
            set
            {
                Couleur = Color.FromArgb(value);
            }
        }

        public ICollection<StatutDossier> StatusDossiers { get; set; }
    }
}