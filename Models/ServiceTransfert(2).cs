using e_apurement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eApurement.Models
{
    public class ServiceTransfert:Structure
    {
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