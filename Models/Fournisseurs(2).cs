using e_apurement.Models;
using eApurement.Models.Fonctions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eApurement.Models
{
    public class Fournisseurs : IDelateCustom
    {
        public bool Remove(ApplicationDbContext db)
        {
            try
            {
                db.GetFournisseurs.Remove(this);
                return true;
            }
            catch (Exception)
            { }
            return false;
        }

        public int Id { get; set; }

        [Display(Name ="Nom complet")]
        public string Nom { get; set; }
        public string Pays { get; set; }

        [Display(Name = "Téléphone 2")]
        public string Tel2 { get; set; }

        [Display(Name = "Téléphone 1")]
        public string Tel1 { get; set; }

        [ForeignKey("Client")]
        public int ClientId { get; set; }
        public Client Client { get; set; }

        [DisplayName("Extrait RCCM ou autre document tenant lieu")]
        public Documentation RCCM { get; set; }
        
        [DisplayName("Liste gérants de la société")]
        public Documentation ListeGerants { get; set; }

        public virtual ICollection<Dossier> Dossiers { get; set; }


        public string Get_RCCM
        {
            get
            {
                try
                {
                    if (RCCM != null)
                        return RCCM.GetImageDocumentAttache().GetImage();
                }
                catch (Exception)
                { }
                return "#";
            }
        }
        public string Get_ListeGerants
        {
            get
            {
                try
                {
                    if (ListeGerants != null)
                        return ListeGerants.GetImageDocumentAttache().GetImage();
                }
                catch (Exception)
                { }
                return "#";
            }
        }
    }
}