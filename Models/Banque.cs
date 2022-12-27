using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using genetrix.Models.Fonctions;
using genetrix.Models;
using System.ComponentModel.DataAnnotations;

namespace genetrix.Models
{
    [DefaultProperty("Nom")]
    [Table("Banque")]
    public class Banque: Structure,IDelateCustom
    {
        public bool Remove(ApplicationDbContext db)
        {
            try
            {
                db.GetBanques.Remove(this);
                return true;
            }
            catch (Exception)
            { }
            return false;
        }

        [DisplayName("Utiliser les configuration personnelles")]
        public bool ConfigPersonnel { get; set; }
        public bool Epub { get; set; }
        public virtual ePub GetEPub { get; set; }
        [Display(Name = "Code Swift"), MaxLength(5)]
        public string CodeSwift { get; set; }
        /// <summary>
        /// Ligne inserée à la derniere génération
        /// </summary>
        public int FileCourierIns { get; set; } = 0;

        public string Image { get; set; }

        public virtual ICollection<DirectionMetier> DirectionMetiers { get; set; } = new List<DirectionMetier>();
        public virtual ICollection<Conformite> Conformites { get; set; } = new List<Conformite>();
        public virtual ICollection<ServiceTransfert> ServiceTransferts { get; set; } = new List<ServiceTransfert>();
        public virtual ICollection<CompteXAF> GetCompteXAFs { get; set; } = new List<CompteXAF>();
        public virtual ICollection<Correspondant> GetCorrespondants { get; set; } = new List<Correspondant>();
        public virtual ICollection<UsersExterne> GetUsersExternes { get; set; } = new List<UsersExterne>();
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="">Ids directions metiers</param>
        /// <returns></returns>
        public ICollection<Agence> GetAgences(int[] idsDM)
        {
            List<Agence> ss = new List<Agence>();
            if (idsDM == null || idsDM.Length == 0)
            {
                DirectionMetiers.ToList().ForEach(d=>
                {
                    ss.AddRange(d.Agences);
                });
            }
            else
            {
                idsDM.ToList().ForEach(id =>
                {
                    try
                    {
                        ss.AddRange(DirectionMetiers.FirstOrDefault(d => d.Id == id).Agences);
                    }
                    catch
                    {}                
                });
            }
            return ss;
        }

        private double moyenneDossierTr;

        public double MoyenDT
        {
            get {
                try
                {
                    //if ((SecuritySystem.CurrentUser as CompteClient).Client.Dossiers.Count>0)
                    //{
                    //    moyenneDossierTr = Math.Round((double)(this.Dossiers.Count / (SecuritySystem.CurrentUser as CompteClient).Client.Dossiers.Count), 2);
                    //}
                }
                catch (System.Exception)
                {}
                return moyenneDossierTr; 
            }
        }

        [Display(Name = "Montant DFX")]
        public double MontantDFX { get; set; } = 50000000;

        private int compteDossiers;
        public int CompteDossier
        {
            get {
                try
                {
                    //compteDossiers = (SecuritySystem.CurrentUser as CompteClient).Client.Dossiers.Where(d=>d.Banque.Nom==this.Nom).Count();
                }
                catch (Exception)
                {}
                return compteDossiers; 
            }
        }

        #region Synchronisation de données
        [Display(Name = "Activer la synchronisation")]
        public bool Activetimer { get; set; } = true;
        public int? Interval { get; set; } = 43200;//12H
        [Display(Name = "Heure debut")]
        public short? HeureExecuteDebut { get; set; } = 0;
        [Display(Name ="Heure fin")]
        public short? HeureExecuteFin { get; set; } = 5;

        [Display(Name = "Nombre de jours avant le passage d'un dossier aux archives")]
        public int? TempsPassageArchivage { get; set; } = 7;

        [Display(Name = "Durée de vie d'un dossier aux archives (en jour)")]
        public int? DureeArchivage { get; set; } = 365;
        [Display(Name = "Nombre de jours pour les relances du mise en demeure")]
        public int? DureeRappelMiseEnDemaure { get; set; } = 2;

        [Display(Name = "Reception email recapitulatif du gestionnaire")]
        public JourSemaine JourRecptionRecapGes { get; set; } = JourSemaine.Jeudi;
        
        [Display(Name = "Nombre de jour pour les relances dossier échu aupres du client")]
        public int? RelanceDossierEchu { get; set; } = 2;
        public DateTime? DateCreation { get; set; }
        public bool StopDataSynchrone { get; set; }
        public DateTime? DateExecuteFin { get; set; }
        public DateTime? DateExecuteDebut { get; set; }

        #endregion

        public override int BanqueId(ApplicationDbContext db)
        {
            return Id;
        }

    }
}