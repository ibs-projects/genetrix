using genetrix.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace genetrix.Models
{
    public class IHMStructure
    {


        [Key, Column(Order = 0)]
        [ForeignKey("Composant")]
        public int ComposantId { get; set; }

        [Key,Column(Order =1)]
        [ForeignKey("Structure")]
        public int IdStructure { get; set; }

        public virtual Structure Structure { get; set; }
        public virtual Composant Composant { get; set; }
        public bool Lire { get; set; }
        public bool Ecrire { get; set; }
        public bool Supprimer { get; set; }
        public bool Créer { get; set; }
    }
}