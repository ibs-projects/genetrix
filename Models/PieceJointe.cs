using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using DisplayNameAttribute = System.ComponentModel.DisplayNameAttribute;

namespace genetrix.Models
{
    [Table("PieceJointe")]
    public class PieceJointe : ClFichier
    {
        public int Id { get; set; }

        public string JustificatifId { get; set; }

        public Justificatif Justificatif { get; set; }
    }
}