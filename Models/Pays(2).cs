
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace e_apurement.Models
{
    [Table("Pays")]
    public class Pays 
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Indicatif { get; set; }

        public virtual ICollection<Ville> Villes { get; set; }
    }
}