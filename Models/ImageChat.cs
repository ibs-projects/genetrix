using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace genetrix.Models
{
    public class ImageChat
    {
        [Key]
        public int Id { get; set; }
        public string Url { get; set; }
        [ForeignKey("Chat")]
        public int IdChat { get; set; }
        public virtual Chat Chat { get; set; }
    }
}