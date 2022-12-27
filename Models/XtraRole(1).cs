using e_apurement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eApurement.Models
{
    public class XtraRole
    {
        [Key]
        public int RoleId { get; set; }
        public string Nom { get; set; }
        [ForeignKey("Structure")]
        public int? IdStructure { get; set; } = null;
        public Structure Structure { get; set; }

        public virtual ICollection<CompteBanqueCommerciale> Users { get; set; }
        public virtual ICollection<Entitee_Role> GetEntitee_Roles { get; set; }
        public IEnumerable<IHM> GetIHMs { get; set; }
        public ICollection<String> Navigations { get; set; }
        public ICollection<string> Actions { get; set; }
        public bool EstAdmin { get; set; }

        #region Permissions
        [Display(Name = "Niveau du dossier")]
        public int? NiveauDossier { get; set; }

        [Display(Name = "Voir des dossiers de tout le monde")]
        public bool VoirDossiersAutres { get; set; }

        [Display(Name = "Voir tous les utilisateurs")]
        public bool VoirUsersAutres { get; set; }
        [Display(Name = "Voir des clients de tout le mo,de")]
        public bool VoirClientAutres { get; set; }
        #endregion


        [NotMapped]
        public virtual List<CompteBanqueCommerciale> UsersTmp { get; set; }

        [NotMapped]
        public virtual List<Entitee> GetEntiteesTmp { get; set; }

    }
}