using genetrix.Models.Fonctions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace genetrix.Models
{
    public class Backoffice:Structure, IDelateCustom
    {
        public bool Remove(ApplicationDbContext db)
        {
            try
            {
                db.GetBackoffices.Remove(this);
                return true;
            }
            catch (Exception)
            { }
            return false;
        }

        [ForeignKey("Agence")]
        public int? IdAgence { get; set; } = null;
        public virtual Agence Agence { get; set; }

        private string banquename;
        public override string BanqueName(ApplicationDbContext db)
        {
            try
            {
                if (Agence == null)
                    banquename = Agence.BanqueName(db);
            }
            catch (System.Exception)
            { }
            return banquename;
        }


        public override int BanqueId(ApplicationDbContext db)
        {
            try
            {
                if (Agence != null)
                {
                    return Agence.BanqueId(db);
                }
            }
            catch (System.Exception)
            { }
            return 0;
        }

    }
}