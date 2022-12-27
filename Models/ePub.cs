using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace genetrix.Models
{
    public class ePub
    {
        public int Id { get; set; }
        public byte Etat { get; set; }
        public virtual ICollection<PubItem> PubItems { get; set; }

        [Display(Name ="CG Visible")]
        public bool CardLeft { get; set; }
        [Display(Name = "Position verticale CG(en %)")]
        public int PostionCarteLeftV { get; set; }
        [Display(Name = "Position horizontale CG(en %)")]
        public int? PostionCarteLeftH { get; set; }
        [Display(Name = "Maximun des postes CG")]
        public int? NbrMaxAffCG { get; set; } = 20;
        [Display(Name = "Nombre des postes affichés CG")]
        public int? NbrPosteAffCG { get; set; } = 2;
        [Display(Name = "Longueur CG (en px)")]
        public int? WidhtCG { get; set; } = 450;
        [Display(Name = "Hauteur CG (en %)")]
        public int? HeigthCG { get; set; } = 100;
        [Display(Name = "Durée de transitione CG (en s)")]
        public int? DelaitAttCG { get; set; } = 10000;

        [Display(Name = "CD visible")]
        public bool CardRigth { get; set; }
        [Display(Name = "Position verticale CD(en %)")]
        public int? PostionCarteRigthtV { get; set; }
        [Display(Name = "Position horizontale CD(en %)")]
        public int? PostionCarteRigthtH { get; set; }
        [Display(Name = "Maximun des postes CD")]
        public int? NbrMaxAffCD { get; set; } = 20;
        [Display(Name = "Nombre des postes affichés CD")]
        public int? NbrPosteAffCD { get; set; } = 2;
        [Display(Name = "Longueur CD (en px)")]
        public int? WidhtCD { get; set; } = 450;
        [Display(Name = "Hauteur CD (en %)")]
        public int? HeigthCD { get; set; } = 100;
        [Display(Name = "Durée de transitione CD (en s)")]
        public int? DelaitAttCD { get; set; } = 10000;

        [Display(Name = "CLH Carte laterale haut")]
        public bool CardTop{ get; set; }
        [Display(Name = "Pancarte laterale bas")]
        public bool CardBottom{ get; set; }
    }
}