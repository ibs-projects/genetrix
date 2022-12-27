using genetrix.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace genetrix.Models
{
   // [Table("ImageInstruction")]
    public class ImageInstruction:UneImage
    {
        //[Key]
        //public override int Id { get; set; }

        [ForeignKey("Dossier")]
        public int DossierId { get; set; }
        public virtual Dossier Dossier { get; set; }
    }
}