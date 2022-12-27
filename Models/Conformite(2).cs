using e_apurement.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace eApurement.Models
{
    public class Conformite:Structure
    {
        [DisplayName("Non conformité")]
        public override string Nom { get; set; }

        [ForeignKey("Banque")]
        public int? IdBanque { get; set; } = null;
        public virtual Banque Banque { get; set; }

        public override int BanqueId(ApplicationDbContext db)
        {
            return (int)IdBanque;
        }

        private string banquename;

        public override string BanqueName(ApplicationDbContext db)
        {
            try
            {
                if (Banque == null)
                {
                    Banque = db.GetBanques.Find(IdBanque);
                }
                banquename = Banque.Nom;
            }
            catch (System.Exception)
            { }
            return banquename;
        }
    }
}