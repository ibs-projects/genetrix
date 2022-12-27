using e_apurement.Models;
using eApurement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace e_apurement.Models
{

    [Table("CompteBanqueCommerciale")]
    public class CompteBanqueCommerciale : ApplicationUser
    {
        public CompteBanqueCommerciale(Session session)
            : base(session)
        {
        }

        public CompteBanqueCommerciale()
        {

        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        /// <summary>
        /// 0:admin banque, 1: gestionnaire (agence), 2: chef d'agence, 3 conformité, 4 service transfert
        /// </summary>
        public byte Categorie { get; set; }

        public string GetCategorie
        {
            get
            {
                switch (Categorie)
                {
                    case 1:return "Gestionnaire";
                    case 2:return "Chef d'agence";
                    case 3:return "Conformité";
                    case 4:return "Service de transfert";
                    default:return "Admin";
                }
            }
        }

        /// <summary>
        /// Les client du portefeuille gestionnaire
        /// </summary>
        public virtual ICollection<BanqueClient> Clients { get; set; }

        /// <summary>
        /// Administrateur de la base
        /// </summary>
        //[ForeignKey("Banque")]
        //public int? IdBanque { get; set; } = null;
        //public virtual Banque Banque { get; set; } = null;

        #region  compte banque

        [ForeignKey("XRole")]
        public int? IdXRole { get; set; } = null;
        public virtual XtraRole XRole { get; set; }

        [ForeignKey("Structure")]
        public int? IdStructure { get; set; } = null;

        public string GetGestionnaireId(int structureId)
        {
            try
            {
                return this.Clients.FirstOrDefault(c=>c.IdSite==IdStructure).IdGestionnaire;
            }
            catch (Exception)
            { }
            return "";
        }

        public virtual Structure Structure { get; set; }
        #endregion

        #region Notifications
        public override IList<AbsNotification> DossiersNotifications
        {
            get
            {
                IDictionary<EtatDossier, AbsNotification> Listabs = new Dictionary<EtatDossier, AbsNotification>();

                //Document encours
                Listabs[EtatDossier.Encours] = new AbsNotification()
                {
                    Image = this.GetImageNotifDossier(EtatDossier.Encours),
                    Couleur = this.GetCouleurNotifDossier(EtatDossier.Encours),
                    Message = this.GetMessageNotifDossier(EtatDossier.Encours),
                    Titre = this.GetTitreNotifDossier(EtatDossier.Encours),
                    Lien = this.GetLienNotifDossier(EtatDossier.Encours)
                };

                //Document Apuré
                Listabs[EtatDossier.Echus] = new AbsNotification()
                {
                    Image = this.GetImageNotifDossier(EtatDossier.Echus),
                    Couleur = this.GetCouleurNotifDossier(EtatDossier.Echus),
                    Message = this.GetMessageNotifDossier(EtatDossier.Echus),
                    Titre = this.GetTitreNotifDossier(EtatDossier.Echus),
                    Lien = this.GetLienNotifDossier(EtatDossier.Echus)
                };
                //Document Echus
                Listabs[EtatDossier.Apuré] = new AbsNotification()
                {
                    Image = this.GetImageNotifDossier(EtatDossier.Apuré),
                    Couleur = this.GetCouleurNotifDossier(EtatDossier.Apuré),
                    Message = this.GetMessageNotifDossier(EtatDossier.Apuré),
                    Titre = this.GetTitreNotifDossier(EtatDossier.Apuré),
                    Lien = this.GetLienNotifDossier(EtatDossier.Apuré)
                };
                //Document Archivé
                Listabs[EtatDossier.Archivé] = new AbsNotification()
                {
                    Image = this.GetImageNotifDossier(EtatDossier.Archivé),
                    Couleur = this.GetCouleurNotifDossier(EtatDossier.Archivé),
                    Message = this.GetMessageNotifDossier(EtatDossier.Archivé),
                    Titre = this.GetTitreNotifDossier(EtatDossier.Archivé),
                    Lien = this.GetLienNotifDossier(EtatDossier.Archivé)
                };


                try
                {
                    var dos = Structure.Dossiers.Where(d => d != null && d.StatutDossier != null && d.StatutDossier.StatutDossier.EtatDossier != EtatDossier.Archivé).GroupBy(d => d.StatutDossier.StatutDossier.EtatDossier).ToList();
                    foreach (var item in dos)
                    {
                        try
                        {
                            Listabs[item.Key].NbrItems = item.Count();
                        }
                        catch (Exception)
                        { }
                    }
                }
                catch (Exception)
                { }

                return Listabs.Values.ToList();

            }
        }

        public IEnumerable<AbsNotification> GetAbsNotifications()
        {
            return DossiersNotifications;
        }

        public override IList<AbsNotification> ReferencesNotifications
        {
            get
            {
                IList<AbsNotification> Listabs = new List<AbsNotification>();
                AbsNotification abs = null;
                try
                {
                    foreach (var cl in Structure.Clients)
                    {
                        foreach (var item in cl.Client.ReferenceBanques.Where(d => d.GetStatutReference != EtatDossier.Archivé).GroupBy(d => d.GetStatutReference))
                        {
                            try
                            {
                                abs = new AbsNotification();
                                abs.NbrItems = item.Count();
                                abs.Message = item.ElementAt(0).GetMessage(item.ElementAt(0).GetStatutReference);
                                abs.Titre = item.ElementAt(0).GetTitre(item.ElementAt(0).GetStatutReference);
                                abs.Image = item.ElementAt(0).GetImage(item.ElementAt(0).GetStatutReference);
                                abs.Couleur = item.ElementAt(0).GetCouleur(item.ElementAt(0).GetStatutReference);
                                Listabs.Add(abs);
                            }
                            catch (Exception)
                            { }
                        } 
                    }
                }
                catch (Exception)
                { }

                return Listabs;

            }
        }
        public override IList<AbsNotification> UsersNotifications
        {
            get
            {
                IList<AbsNotification> Listabs = new List<AbsNotification>();
                AbsNotification abs = null;
                try
                {
                    //foreach (var item in Client.Dossiers.Where(d => d.StatutDossier.EtatDossier != EtatDossier.Archivé).GroupBy(d => d.StatutDossier.EtatDossier))
                    {
                        try
                        {
                            //abs = new AbsNotification();
                            //abs.NbrItems = item.Count();
                            //abs.Message = item.ElementAt(0).GetMessage(item.ElementAt(0).StatutReference);
                            //abs.Titre = item.ElementAt(0).GetTitre(item.ElementAt(0).StatutReference);
                            //abs.Image = item.ElementAt(0).GetImage(item.ElementAt(0).StatutReference);
                            //abs.Couleur = item.ElementAt(0).GetCouleur(item.ElementAt(0).StatutReference);
                            //Listabs.Add(abs);
                        }
                        catch (Exception)
                        { }
                    }
                }
                catch (Exception)
                { }

                return Listabs;

            }
        }

        /// <summary>
        /// Specifier si l'utilisateur est un gestionnaire
        /// </summary>
        public bool EstGestionnaire { get; set; }

        public override void SetDossierNotification(Dossier dossier)
        {

        }

        public override void SetReferenceNotification(ReferenceBanque reference)
        {

        }

        public override void SetUserNotification(ApplicationUser utilisateur)
        {

        }

        #endregion
    }

    public  enum TStructure
    {
        Agence,
        Conformité,
        SertviceTransfert
    }
}