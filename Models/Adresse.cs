
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using DisplayNameAttribute = System.ComponentModel.DisplayNameAttribute;

namespace genetrix.Models
{
    [Table("Adresse")]
    public class Adresse 
    {
        public int Id { get; set; }

        [DisplayName("Téléphone")]
        public string Tel1 { get; set; }
        public string Email { get; set; }

        [DisplayName("Code postal")]
        // [DataType(DataType.PostalCode)]
        public string CodePostal { get; set; }

        [Required(ErrorMessage = "Le champs entreprise est obligatoire !")]
        [ForeignKey("Client")]
        public int? ClientId { get; set; } = null;

        public Client Client { get; set; }
    }
}