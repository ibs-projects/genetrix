using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace genetrix.Models
{
    public class BanqueTierClient
    {
        public int Id { get; set; }
        [Display(Name = "Nom de la banque")]
        public string Nom { get; set; }
        [Display(Name = "Ville de la banque")]
        public string Ville { get; set; }
        [Display(Name = "Pays de la banque")]
        public string Pays { get; set; }
        [Display(Name ="Adresse de la banque")]
        public string Adresse { get; set; }

        [ForeignKey("Client")]
        public int IdClient { get; set; }
        public virtual Client Client { get; set; }

        [Display(Name ="Attestation de non defaut d'apurement")]
        public virtual ICollection<DocumentAttache> AttestationNonDefautAp { get; set; }

        public DocumentAttache AttestationValid
        {
            get
            {
                try
                {
                    foreach (var d in AttestationNonDefautAp.OrderByDescending(d => d.DateSignature).ToList())
                    {
                        try
                        {
                            if (d.DateCreation != null && (DateTime.Now - d.DateSignature.Value).TotalDays < 30)
                            {
                                return d;
                            }
                        }
                        catch (Exception e)
                        { }
                    }
                }
                catch (Exception)
                { }
                return null;
            }
        }


    }
}