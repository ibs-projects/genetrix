using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Text;
using DisplayNameAttribute = System.ComponentModel.DisplayNameAttribute;

namespace genetrix.Models
{
    [Table("Dossier_StatutDossier")]
    public class Dossier_StatutDossier
    {
        public Dossier_StatutDossier()
        {
            try
            {
                if (Date == new DateTime())
                {
                    Date = DateTime.Now;
                }
            }
            catch (Exception)
            { }
        }

        [DisplayName("Statut du dossier")]
        //[Required(ErrorMessage = "Le statut du dossier est obligatoire !")]
        [Key]
        [Column(Order = 1)]
        [ScaffoldColumn(false)]
        [ForeignKey("StatutDossier")]
        public int StatutDossierId { get; set; }

        [DisplayName("Statut du dossier")]
        //[Required(ErrorMessage = "Le statut du dossier est obligatoire !")]
        public virtual StatutDossier StatutDossier { get; set; }
        [Key]
        [Column(Order = 2)]
        [ScaffoldColumn(false)]
        [ForeignKey("Dossier")]
        public int DossierId { get; set; }

        //[Required(ErrorMessage = "Le dossier est obligatoire !")]
        public virtual Dossier Dossier { get; set; }

        public EtatDossier Statut
        {
            get {
                try
                {
                  //return  StatutDossier.EtatDossier;
                }
                catch (Exception)
                {}
                return EtatDossier.Soumis; 
            }
        }

        public Color Couleur
        {
            get {
                try
                {
                    //return StatutDossier.Couleur;
                }
                catch (Exception)
                {}
                return default; 
            }
        }


        private DateTime date;

        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        private string motif;

        public string Motif
        {
            get { return motif; }
            set { motif = value; }
        }

    }
}