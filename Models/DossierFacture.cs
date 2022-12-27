using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace genetrix.Models
{
    public class DossierFacture
    {
        public int Id { get; set; }
        [ForeignKey("Dossier")]
        public int IdDossier { get; set; }
        [ForeignKey("Facture")]
        public int IdFacture { get; set; }
        public virtual Dossier Dossier { get; set; }
        public virtual Justificatif Facture { get; set; }
        public string MontantPayeString
        {
            get
            {
                try
                {
                    if (Dossier != null) return Dossier.MontantString;
                }
                catch (Exception)
                { }
                return "0";
            }
        }

    }
}