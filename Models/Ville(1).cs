﻿
using System.ComponentModel.DataAnnotations.Schema;

namespace e_apurement.Models
{
    [Table("Ville")]
    public class Ville 
    {
        public int Id { get; set; }

        public string Nom { get; set; }

        public string PaysId { get; set; }
        public virtual Pays Pays { get; set; }


    }
}