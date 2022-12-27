
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace genetrix.Models
{
    [NotMapped]
    public class Afficheur 
    {
        public int Id { get; set; }

        private List<Dossier> dossiers;
        public ICollection<Dossier> Dossiers
        {
            get
            {
                if (dossiers == null)
                {
                    dossiers = new List<Dossier>();
                    //dossiers.AddRange(Session.Query<Dossier>());
                }
                return dossiers;
            }
        }
    }

}