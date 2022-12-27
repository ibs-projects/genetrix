using genetrix;
using genetrix.Models.Fonctions;
using System;
using System.Linq;
using System.ComponentModel;
using DisplayNameAttribute = System.ComponentModel.DisplayNameAttribute;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using genetrix.Models.Fonctions;
using genetrix.Models;

namespace genetrix.Models
{
    [DefaultProperty("Banque")]
    [Table("BanqueClient")]
    public class BanqueClient : IDelateCustom
    {
        public bool Remove(ApplicationDbContext db)
        {
            try
            {
                db.GetBanqueClients.Remove(this);
                return true;
            }
            catch (Exception)
            { }
            return false;
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Le champs Client est obligatoire !")]
        [ForeignKey("Client")]
        public int ClientId { get; set; }
        public virtual Client Client { get; set; }

        //[Required(ErrorMessage = "Le champs Banque est obligatoire !")]
        //[ForeignKey("Banque")]
        //public int BanqueId { get; set; }
        //public virtual Banque Banque { get; set; }

        [Required(ErrorMessage = "L'agence est obligatoire !")]
        [ForeignKey("Site")]
        public int IdSite { get; set; }
        public virtual Agence Site { get; set; }

        public string Nom
        {
            get
            {
                try
                {
                    return $"{Site.Nom}";
                }
                catch (Exception)
                { }
                return "";
            }
        }
        public string CodeAgence
        {
            get
            {
                try
                {
                    return $"{Site.CodeEtablissement}";
                }
                catch (Exception)
                { }
                return "";
            }
        }

        public string Ville
        {
            get
            {
                try
                {
                    return $"{Site.Ville}";
                }
                catch (Exception)
                { }
                return "";
            }
        }

        [ForeignKey("Gestionnaire")]
        public string IdGestionnaire { get; set; }
        public virtual CompteBanqueCommerciale Gestionnaire { get; set; }
        public DateTime? DateCreation { get; set; }

        public virtual ICollection<NumCompte> NumComptes { get; set; }

        public string CodeEts
        {
            get
            {
                try
                {
                    return Client.CodeEtablissement;
                }
                catch (Exception)
                { }
                return "";
            }
        }

        public string GestionnaireName
        {
            get
            {
                try
                {
                    return Gestionnaire.NomComplet;
                }
                catch (Exception)
                { }
                return "";
            }
        }

        public string GestionnaireTel
        {
            get
            {
                try
                {
                    return Gestionnaire.Tel1;
                }
                catch (Exception)
                { }
                return "";
            }
        }

        public string GestionnaireEmail
        {
            get
            {
                try
                {
                    return Gestionnaire.Email;
                }
                catch (Exception)
                { }
                return "";
            }
        }

        public string GetClientEmail
        {
            get
            {
                try
                {
                    return Client.Email;
                }
                catch (Exception)
                { }
                return "";
            }
        }

        public string GetClientName
        {
            get
            {
                try
                {
                    return Client.Nom;
                }
                catch (Exception)
                { }
                return "";
            }
        }

        public string GetClientTel
        {
            get
            {
                try
                {
                    return Client.Telephone;
                }
                catch (Exception)
                { }
                return "";
            }
        }

        public string GetClientPays
        {
            get
            {
                try
                {
                    return Client.Pays;
                }
                catch (Exception)
                { }
                return "";
            }
        }

        public string GetClientVille
        {
            get
            {
                try
                {
                    return Client.Ville;
                }
                catch (Exception)
                { }
                return "";
            }
        }

        public string AgenceNAme
        {
            get
            {
                try
                {
                    return Site.Nom;
                }
                catch (Exception)
                { }
                return "";
            }
        }


        public string AgenceAdresse
        {
            get
            {
                try
                {
                    return Site.Adresse;
                }
                catch (Exception)
                { }
                return "";
            }
        }


        public PercentageColor GetColorCssClass(List<Dossier> dossiers)
        {
            if (dossiers.Count == 0) return new PercentageColor();
            var count = 0;// this.Site.Dossiers.Count *100/ dossiers.Count;
            if (count == 0) return new PercentageColor();
            else if (count < 50) return new PercentageColor("warning", count);
            else if (count < 90) return new PercentageColor("info", count);
            else return new PercentageColor("primary", count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="annee"></param>
        /// <param name="devise"></param>
        /// <param name="afficheMontantOuNombre">true: affiche dossier-montant. false: affiche dossier-nombre</param>
        /// <returns></returns>
        public double[] GetDossierAllMonths(int annee, int deviseId, ApplicationDbContext db, bool afficheMontantOuNombre = true)
        {
            double[] tab = new double[12];
            try
            {
                var donnees = db.GetDossiers.Where(d => d.IdSite == this.IdSite && d.DateDepotBank.Value.Year == annee && d.DeviseMonetaireId == deviseId).GroupBy(d => d.DateDepotBank.Value.Month);
                donnees.ToList().ForEach(d =>
                {
                    try
                    {
                        tab[d.Key] += d.ElementAt(0).Montant;
                    }
                    catch (Exception)
                    { }
                });
            }
            catch (Exception)
            { }
            return tab;
        }


    }


    public class PercentageColor
    {
        public string CssCalssColor = "danger";
        public int Percentage = 0;
        public string Banque;

        public PercentageColor()
        {

        }

        public PercentageColor(string css, int per)
        {
            this.CssCalssColor = css; this.Percentage = per;
        }
    }
}