using genetrix.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace genetrix.Models
{
    public class Agence:Structure
    {
        public Agence()
        {
            
        }

        [ForeignKey("DirectionMetier")]
        public int? IdDirectionMetier { get; set; } = null;
        public DirectionMetier DirectionMetier { get; set; }

       // public override ICollection<Dossier> Dossiers { get; set; }

        public override ICollection<BanqueClient> Clients { get; set; }


        public override int BanqueId(ApplicationDbContext db)
        {
            try
            {
                if (DirectionMetier != null)
                {
                    return DirectionMetier.IdBanque;
                }
                else
                {
                    try
                    {
                        var d = db.DirectionMetiers.Find(IdDirectionMetier);
                        if (d != null)
                            return d.IdBanque;
                        d = null;
                    }
                    catch (System.Exception)
                    { }
                }
            }
            catch (System.Exception)
            {}
            return 0;
        }

        private string banquename;

        public override string BanqueName(ApplicationDbContext db)
        {
            try
            {
                if (DirectionMetier==null)
                {
                    DirectionMetier = db.DirectionMetiers.Include("Banque").FirstOrDefault(d=>d.Id== IdDirectionMetier);
                }
                banquename = DirectionMetier.Banque.Nom;
            }
            catch (System.Exception)
            { }
            return banquename;
        }

    }
}