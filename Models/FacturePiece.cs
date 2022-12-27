using genetrix.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace genetrix.Models
{
    public class FacturePiece
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Justificatif")]
        public int IdFacture { get; set; }

        public int IdFichier { get; set; }
        public virtual Justificatif Justificatif { get; set; }
    }
}