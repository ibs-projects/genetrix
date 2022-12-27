using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace genetrix.Models
{
    public class Contact
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Display(Name = "Nom complet")]
        public string NomComplet { get; set; }
        public string Pays { get; set; }
        [Display(Name ="Pays d'origine")]
        public string PaysOrigine { get; set; }
        public string Ville { get; set; }
        public string Telephone { get; set; }
        public string Fax { get; set; }
        public string Profession { get; set; }
        public string Email { get; set; }
        [Display(Name = "Adresse complete")]
        public string Adresse { get; set; }
        [ForeignKey("Grestionnaire")]
        public string IdGestionnaire { get; set; } = null;
        public virtual CompteBanqueCommerciale Grestionnaire { get; set; } = null;
        [ForeignKey("Client")]
        public int? IdClient { get; set; }
        public virtual Client Client { get; set; }
        public Groupe Groupe { get; set; }

        public virtual ICollection<Dossier> Dossiers { get; set; }
    }

    public enum Groupe
    {
        Collegue,
        Client,
        Contact,
        Gestionnaire,
        Fournisseur,
        Importateur,
        Vendeur
    }
}