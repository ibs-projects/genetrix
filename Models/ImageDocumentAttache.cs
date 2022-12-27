using genetrix.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace genetrix.Models
{
    //[Table("ImageDocumentAttache")]
    public class ImageDocumentAttache:UneImage
    {
        //[Key]
        //public override int Id { get; set; }

        [ForeignKey("DocumentAttache")]
        public int DocumentAttacheId { get; set; }
        public virtual DocumentAttache DocumentAttache { get; set; }
    }
}