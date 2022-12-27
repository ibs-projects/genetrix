using eApurement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;

namespace e_apurement.Models
{
    [DefaultProperty("EtatDossier")]
    [Table("StatutDossier")]
    public class StatutDossier
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Statut_Id { get; set; }
        public DateTime Date { get; set; }
        public Structure Structure { get; set; }

        [ForeignKey("Statut")]
        public int IdStatut { get; set; }
        public virtual Statut Statut { get; set; }
        [ForeignKey("Dossier")]
        public int? IdDossier { get; set; }
        public virtual Dossier Dossier { get; set; }
        public EtatDossier EtatDossier { get; set; }

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


        [Browsable(false)]
        public virtual ICollection<Dossier_StatutDossier> GetStatutDossiers { get; set; }
    }
}