using e_apurement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eApurement.Models
{
    public class IHM
    {
        [Key,Column(Order =0)]
        [ForeignKey("XRole")]
        public int XRoleId { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey("Composant")]
        public int ComposantId { get; set; }

        public virtual XtraRole XRole { get; set; }
        public virtual Composant Composant { get; set; }
        public bool Lire { get; set; }
        public bool Ecrire { get; set; }
        public bool Supprimer { get; set; }
        public bool Créer { get; set; }
    }
}