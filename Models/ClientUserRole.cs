using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace genetrix.Models
{
    public class ClientUserRole
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        #region habilitations
        [Display(Name = "Peut créer un dossier")]
        public bool CreerDossier { get; set; }
        [Display(Name = "Peut soumettre un dossier")]
        public bool SoumettreDossier { get; set; }
        [Display(Name = "Peut créer un utilisateur")]
        public bool CreerUser { get; set; }
        [Display(Name = "Peut supprimer un utilisateur")]
        public bool SuppUser { get; set; }
        [Display(Name = "Peut modifier un utilisateur")]
        public bool ModifUser { get; set; }
        [Display(Name = "Peut créer un fournisseur")]
        public bool CreerBenef { get; set; }
        [Display(Name = "Peut supprimer un fournisseur")]
        public bool SuppBenef { get; set; }
        [Display(Name = "Peut modifier un fournisseur")]
        public bool ModifBenef { get; set; }
        #endregion

        public virtual ICollection<CompteClient> GetCompteClients { get; set; }
        public bool Default { get; internal set; }
    }
}