using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace genetrix.Models
{
    public class PubItem
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Titre { get; set; }

        [Display(Name = "Node de bas")]
        public string NoteBas { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        [Display(Name = "Est en Html")]
        public bool IsHtml { get; set; }
        public bool Acive { get; set; }
        [NotMapped]
        public string Cards { get; set; }

        #region Disposition
        public Unit Unit { get; set; }
        [Display(Name = "Gauche")]
        public double Left { get; set; }
        [Display(Name = "Droite")]
        public double Rigth { get; set; }
        [Display(Name = "Haut")]
        public double Top { get; set; }
        [Display(Name = "Bas")]
        public double Bottom { get; set; }
        [Display(Name ="Couleur du titre")]
        public string TitreColor { get; set; }
        [Display(Name = "Couleur de la description")]
        public string DescriptionColor { get; set; }
        #endregion
        [ForeignKey("EPub")]
        public int? IdePub { get; set; }
        public virtual ePub EPub { get; set; }
        [Display(Name ="Type poste")]
        public ePubItemType ePubItemType { get; set; }
        /// <summary>
        /// 0=ePubItemType.CarteDroite  1=ePubItemType.CarteGauche; 2=ePubItemType.Haut; 3=ePubItemType.Bas
        /// </summary>
        public byte? eType { get; set; }
        public byte? GetEtype()
        {
            try
            {
                switch (ePubItemType)
                {
                    case ePubItemType.CarteDroite:
                        return 0;
                    case ePubItemType.CarteGauche:
                        return 1;
                    case ePubItemType.Haut:
                        return 2;
                    case ePubItemType.Bas:
                        return 3;
                    default:
                        break;
                }
            }
            catch (Exception)
            {}
            return null;
        }
        public string Theme { get; set; }
        [Display(Name = "Date début")]
        public DateTime? DateDebut { get; set; }
        [Display(Name = "Heure début")]
        public TimeSpan? HeureDebut { get; set; }
        [Display(Name ="Date fin")]
        public DateTime? DateFin { get; set; } 
        [Display(Name ="Heure fin")]
        public TimeSpan? HeureFin { get; set; }
        [Display(Name ="Chaque x heure")]
        public TimeSpan? ChaqueHeure { get; set; }
        [Display(Name = "Durée d'apparition(s)")]
        public int? DureeApp { get; set; }

        [Display(Name = "Durée d'attente apres une apparition(s)")]
        public int? DuréeAtt { get; set; }
        [Display(Name = "Largeur")]
        public int? Width { get; set; }
        [Display(Name = "Hauteur")]
        public int? Heigth { get; set; }
        [Display(Name = "Couleur de fond")]
        public string FondColor { get; set; }
        [Display(Name = "Lien (url)")]
        public string LienUrl { get; set; }

        [Display(Name = "Lien (Texte)")]
        public string LienText { get; set; }

        [NotMapped]
        public string FrameId { get; set; }
        public bool EstLibre { get; set; }

        public int GetDureeAtt(int d)
        {
            if (DuréeAtt > 0)
                return (int)DuréeAtt;
            return d;
        }

        public string ImageView()
        {
            //var bb= Fonctions.Fonctions.ImageBase64ImgSrc(this.Image);
            //return Fonctions.Fonctions.ImageBase64ImgSrc(this.Image);
            return this.Image;
        }
        public override string ToString()
        {
            return string.Empty;
        }
    }

    public enum ePubItemType
    {
        CarteGauche,
        CarteDroite,
        Haut,
        Bas
    }
}