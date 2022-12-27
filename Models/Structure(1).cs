using e_apurement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eApurement.Models
{
    public class Structure
    {
        public int Id { get; set; }
        public virtual string Nom { get; set; }
        public string Adresse { get; set; }
        public string Ville { get; set; }
        public string Pays { get; set; }
        public string Telephone { get; set; }
        public string Telephone2 { get; set; }

        #region Permissions
        [Display(Name = "Niveau min du dossier")]
        public int? NiveauDossier { get; set; }
        [Display(Name = "Niveau max du dossier")]
        public int? NiveauMaxDossier { get; set; }

        [Display(Name = "Voir des dossiers de toute structure")]
        public bool VoirDossiersAutres { get; set; }

        [Display(Name = "Voir des utilisateurs de toute structure")]
        public bool VoirUsersAutres { get; set; }
        [Display(Name = "Voir des clients de toute structure")]
        public bool VoirClientAutres { get; set; } 
        #endregion

        [DisplayName("Type *")]
        [ForeignKey("TypeStructure")]
        //[Required]
        public int? IdTypeStructure { get; set; }
        public virtual TypeStructure TypeStructure { get; set; }
        public bool EstAgence { get; set; }

        [DisplayName("Niveau hiérarchique")]
        public int NiveauH { get; set; }
        //[ForeignKey("Banque")]
        //public int? IdBanque { get; set; } = null;
        //public virtual Banque Banque { get; set; }

        [ForeignKey("Responsable")]
        public string IdResponsable { get; set; }
        public CompteBanqueCommerciale Responsable { get; set; }

        public virtual ICollection<CompteBanqueCommerciale> Agents { get; set; }
        public virtual ICollection<IHMStructure> Composants { get; set; }
        public virtual ICollection<BanqueClient> Clients { get; set; }
        public virtual ICollection<Dossier> Dossiers { get; set; }
        public virtual ICollection<XtraRole> Roles { get; set; }
        public virtual ICollection<Structure> Structures { get; set; }
        [Display(Name ="Lire toutes les références banque")]
        public bool LireTouteReference { get; set; }

        [Display(Name = "Montant DFX")]
        public double MontantDFX { get; set; } = 50000000;

        public virtual int BanqueId(ApplicationDbContext db)
        {
            return 0;
        }

        public virtual string BanqueName(ApplicationDbContext db)
        {
            return "";
        }


        public string GetTypeElt()
        {
            return (this.GetType().Name == "Structure" ? TypeStructure != null ? TypeStructure.Intitule : GetType().Name : GetType().Name);
        }

    }
}