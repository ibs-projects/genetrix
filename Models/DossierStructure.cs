using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace genetrix.Models
{
    public class DossierStructure
    {
        [Key]
        [Column(Order = 1)]
        public int IdDossier { get; set; }
        public virtual Dossier Dossier { get; set; }
        [Key]
        [Column(Order = 2)]
        public int? IdStructure { get; set; } = null;
        public virtual Structure Structure { get; set; }
        public DateTime Date { get; set; }
        public string AttribuerPar { get; set; }// la personne qui attribue

        public string IdResponsable { get; set; }
        public string NomResponsable { get; set; }
    }
}