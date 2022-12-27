using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace genetrix.Models
{
    public class DossierAgent
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DossierId { get; set; }

        [Key]
        [Column(Order = 1)]
        public string AgentId { get; set; }

        public int? IdStructure { get; set; }

        public string NomStructure { get; set; }

        public string Observation { get; set; }

        public DateTime Date { get; set; }

        public virtual CompteBanqueCommerciale CompteBanqueCommerciale { get; set; }

        public virtual Dossier Dossier { get; set; }
    }
}