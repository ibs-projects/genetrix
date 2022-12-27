using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace genetrix.Models
{
    public class Entitee_Role
    {
        [Key, Column(Order = 0)]
        [ForeignKey("XRole")]
        public int IdXRole { get; set; }
        [Key, Column(Order = 1)]
        [ForeignKey("Entitee")]
        public int IdEntitee { get; set; }

        public virtual XtraRole XRole { get; set; }
        public virtual Entitee Entitee { get; set; }
        public bool Lire { get; set; }
        public bool Ecrire { get; set; }
        public bool Supprimer { get; set; }
        public bool Créer { get; set; }
        public bool Lire_Pour_Tout { get; set; }
    }
}