using e_apurement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eApurement.Models
{
    //[Table("ImageJustificatif")]
    public class ImageJustificatif:UneImage
    {
        //[Key]
        //public override int Id { get; set; }

        [ForeignKey("Justificatif")]
        public int? JustificatifId { get; set; } = null;
        public virtual Justificatif Justificatif { get; set; }
    }
}