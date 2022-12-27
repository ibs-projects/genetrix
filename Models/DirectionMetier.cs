using genetrix.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace genetrix.Models
{
    public class DirectionMetier:Structure
    {
        [ForeignKey("Banque")]
        [Required]
        public int IdBanque { get; set; }
        public Banque Banque { get; set; }

        public virtual ICollection<Agence> Agences { get; set; } = new List<Agence>();
        public override int BanqueId(ApplicationDbContext db)
        {
            if (Banque != null)
                return Banque.Id;
            else
            {
                try
                {
                    var d = db.GetBanques.Find(IdBanque);
                    if (d != null)
                        return d.Id;
                    d = null;
                }
                catch (System.Exception)
                { }
            }
            return 0;
        }
        
        public override string BanqueName(ApplicationDbContext db)
        {
            if (Banque != null)
                return Banque.Nom;
            else
            {
                try
                {
                    var d = db.GetBanques.Find(IdBanque);
                    if (d != null)
                        return d.Nom;
                    d = null;
                }
                catch (System.Exception)
                { }
            }
            return "";
        }

    }
}