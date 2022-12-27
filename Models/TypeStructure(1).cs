using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eApurement.Models
{
    public class TypeStructure
    {
        [Key]
        public int Id { get; set; }
        public string Intitule { get; set; }
        public virtual ICollection<Agence> Sites { get; set; }
    }
}