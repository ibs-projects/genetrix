using genetrix.Models;
using genetrix.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace genetrix.Models
{

    [Table("CompteBanqueCommerciale")]
    [Serializable()]
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
        //public virtual ICollection<DossierAgent> DossierAgents { get; set; }

        /// <summary>
        /// Les client du portefeuille gestionnaire
        /// </summary>
        public virtual ICollection<BanqueClient> Clients { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; }

        private bool suiviChat;

        public override bool SuitChat {
            get
            {
                try
                {
                    if (Structure != null && Structure.SuitChat)
                        return true;
                }
                catch (Exception)
                { }
               return base.SuitChat;
            }
            set
            {
                base.SuitChat = value;
            }
        }


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
        public int? IdStructure { get; set; }
        public virtual Structure Structure { get; set; }

        public string StructureName
        {
            get {
                try
                {
                    if (Structure != null)
                        return Structure.Nom;
                }
                catch (Exception)
                {}
                return ""; 
            }
        }


        [Display(Name ="Est back-office")]
        public bool EstBackOff { get; set; }

        bool estBack;
        public bool EstBackOffice
        {
            get {
                try
                {
                    if (XRole.EstBackOffice) estBack= true;
                    else if (EstBackOff) estBack= true;
                    return estBack;
                }
                catch (Exception)
                {}
                return estBack; 
            }
        }


        internal IEnumerable<Contact> GetDefaultContacts()
        {
            var getContacts = new List<Contact>();
            //Ajout des collegues comme contacts
            try
            {
                this.Structure.Agents.ToList().ForEach(u =>
                {
                    try
                    {
                        getContacts.Add(new Contact()
                        {
                            Email = u.Email,
                            Groupe = Groupe.Collegue,
                            NomComplet = u.Nom,
                            Telephone = u.Tel1
                        });
                    }
                    catch (Exception)
                    { }
                });
            }
            catch (Exception)
            { }
            //Ajouter les clients comme du gestionnaire comme contacts
            try
            {
                this.Clients.ToList().ForEach(c =>
                {
                    try
                    {
                        getContacts.Add(new Contact()
                        {
                            Email = c.GetClientEmail,
                            Groupe = Groupe.Client,
                            NomComplet = c.GetClientName,
                            Telephone = c.GetClientTel,
                            Pays = c.GetClientPays,
                            Ville = c.GetClientVille
                        });
                    }
                    catch (Exception)
                    { }
                });

            }
            catch (Exception)
            { }

            if (getContacts == null) getContacts = new List<Contact>();
            return getContacts;
        }


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
        ///         
        [Display(Name ="Est gestionnaire")]
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