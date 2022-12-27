using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;

namespace genetrix.Models
{
    [Table("CompteClient")]
    [Serializable()]
    public class CompteClient : ApplicationUser
    {
        public CompteClient(Session session)
            : base(session)
        {
        }

        public CompteClient()
        {

        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }


        public virtual ICollection<Justificatif> Justificatifs { get; set; }


        [Required(ErrorMessage = "L'utilisateur doit appartenir à une entreprise !")]
        [Display(Name ="Entreprise")]
        public int ClientId { get; set; }
        public virtual Client Client { get; set; }

        #region habilitations
        [Display(Name ="Peut créer un dossier")]
        public bool CreerDossier { get; set; }
        [Display(Name = "Peut soumettre un dossier")]
        public bool SoumettreDossier { get; set; }
        [Display(Name = "Peut créer un utilisateur")]
        public bool CreerUser { get; set; }
        [Display(Name = "Peut supprimer un utilisateur")]
        public bool SuppUser { get; set; }
        [Display(Name = "Peut modifier un utilisateur")]
        public bool ModifUser { get; set; }
        [Display(Name = "Peut créer un fournisseur")]
        public bool CreerBenef { get; set; }
        [Display(Name = "Peut supprimer un fournisseur")]
        public bool SuppBenef { get; set; }
        [Display(Name = "Peut modifier un fournisseur")]
        public bool ModifBenef { get; set; }
        #endregion

        [ForeignKey("UserRole")]
        public int? IdUserRole { get; set; }
        public virtual ClientUserRole UserRole { get; set; }

        public void SetHabilitations()
        {
            try
            {
                if (UserRole != null && UserRole.Id > 0)
                {
                    CreerBenef = UserRole.CreerBenef;
                    CreerDossier = UserRole.CreerDossier;
                    CreerUser = UserRole.CreerUser;
                    ModifUser = UserRole.ModifUser;
                    ModifBenef = UserRole.ModifBenef;
                    SoumettreDossier = UserRole.SoumettreDossier;
                    SuppBenef = UserRole.SuppBenef;
                    SuppUser = UserRole.SuppUser;
                }
            }
            catch (Exception)
            {}
        }


        public string Get_RoleName
        {
            get {
                try
                {
                    if (UserRole != null)
                        return UserRole.Nom;
                }
                catch (Exception)
                {}
                return ""; 
            }
        }

        public int Get_RoleId
        {
            get {
                try
                {
                    if (UserRole != null)
                        return UserRole.Id;
                }
                catch (Exception)
                {}
                return 0; 
            }
        }

        
        public string ClientName
        {
            get {
                try
                {
                    if (Client != null)
                        return Client.Nom;
                }
                catch (Exception)
                {}
                return ""; 
            }
        }


        private bool memeEntreprise;
        public bool EstDansEntreprise
        {
            get
            {
                try
                {
                    if (SecuritySystem.CurrentUser is CompteClient)
                    {
                        if ((SecuritySystem.CurrentUser as CompteClient).Client == Client)
                        {
                            memeEntreprise = true;
                        }
                    }
                }
                catch (Exception)
                { }
                return memeEntreprise;
            }
        }


        #region Notifications
        public override IList<AbsNotification> DossiersNotifications {
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

                //Document à apurer
                Listabs[EtatDossier.AApurer] = new AbsNotification()
                {
                    Image = this.GetImageNotifDossier(EtatDossier.AApurer),
                    Couleur = this.GetCouleurNotifDossier(EtatDossier.AApurer),
                    Message = this.GetMessageNotifDossier(EtatDossier.AApurer),
                    Titre = this.GetTitreNotifDossier(EtatDossier.AApurer),
                    Lien = this.GetLienNotifDossier(EtatDossier.AApurer)
                };
                //Document Echus
                Listabs[EtatDossier.Echus] = new AbsNotification()
                {
                    Image = this.GetImageNotifDossier(EtatDossier.Echus),
                    Couleur = this.GetCouleurNotifDossier(EtatDossier.Echus),
                    Message = this.GetMessageNotifDossier(EtatDossier.Echus),
                    Titre = this.GetTitreNotifDossier(EtatDossier.Echus),
                    Lien = this.GetLienNotifDossier(EtatDossier.Echus)
                };
                //Document brouillo
                Listabs[EtatDossier.Brouillon] = new AbsNotification()
                {
                    Image = this.GetImageNotifDossier(EtatDossier.Brouillon),
                    Couleur = this.GetCouleurNotifDossier(EtatDossier.Brouillon),
                    Message = this.GetMessageNotifDossier(EtatDossier.Brouillon),
                    Titre = this.GetTitreNotifDossier(EtatDossier.Brouillon),
                    Lien = this.GetLienNotifDossier(EtatDossier.Brouillon)
                };


                try
                {
                    //var dos= Client.Dossiers.Where(d => d != null && d.StatutDossier != null && d.StatutDossier.StatutDossier.EtatDossier != EtatDossier.Archivé).GroupBy(d => d.StatutDossier.StatutDossier.EtatDossier).ToList();
                    //foreach (var item in dos)
                    //{
                    //    try
                    //    {
                    //        Listabs[item.Key].NbrItems = item.Count();
                    //    }
                    //    catch (Exception)
                    //    { }
                    //}
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
        
        public override IList<AbsNotification> ReferencesNotifications {
            get
            {
                IList<AbsNotification> Listabs = new List<AbsNotification>();
                AbsNotification abs = null;
                try
                {
                    foreach (var item in Client.ReferenceBanques.Where(d => d.GetStatutReference != EtatDossier.Archivé).GroupBy(d => d.GetStatutReference))
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
                catch (Exception)
                { }

                return Listabs;

            }
        }
        public override IList<AbsNotification> UsersNotifications {
            get
            {
                IList<AbsNotification> Listabs = new List<AbsNotification>();
                AbsNotification abs = null;
                try
                {
                    //foreach (var item in Client.Dossiers.Where(d => d.StatutDossier.StatutDossier.EtatDossier != EtatDossier.Archivé).GroupBy(d => d.StatutDossier.StatutDossier.EtatDossier))
                    //{
                    //    try
                    //    {
                    //        //abs = new AbsNotification();
                    //        //abs.NbrItems = item.Count();
                    //        //abs.Message = item.ElementAt(0).GetMessage(item.ElementAt(0).StatutReference);
                    //        //abs.Titre = item.ElementAt(0).GetTitre(item.ElementAt(0).StatutReference);
                    //        //abs.Image = item.ElementAt(0).GetImage(item.ElementAt(0).StatutReference);
                    //        //abs.Couleur = item.ElementAt(0).GetCouleur(item.ElementAt(0).StatutReference);
                    //        //Listabs.Add(abs);
                    //    }
                    //    catch (Exception)
                    //    { }
                    //}
                }
                catch (Exception)
                { }

                return Listabs;

            }
        }

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


        #region Dossier Notification

        //public override Type GetType(Guid oid)
        //{
        //    return this.GetType();
        //}

        public override IEnumerable GetMessages(List<Dossier> dossiers = null, Client client = null, IdentityUserRole role = null, ApplicationUser user = null, EtatDossier etat = EtatDossier.Encours, DateTime dateTime = default, string RefInterne = "", DateTime DateCreaBanque = default, DateTime DateModif = default, Guid oidBanque = default, double Montant = 0, string Fournisseur = "", Guid oidReferenceExterne = default, Guid oidClient = default)
        {
            List<DossierNotification> notifications = new List<DossierNotification>();
            ICollection<AbsNotification> absNotifications = null;
            try
            {
                //dossier Encours
                foreach (var d in dossiers)
                {
                    try
                    {
                        notifications.AddRange(d.Notifications.ToList());
                    }
                    catch (Exception)
                    { }
                }
                //dossier Apuré

                //dossier Echus

                //dossier Archivé

                absNotifications = new List<AbsNotification>();
                string etat1 = "";
                AbsNotification abs =null;
                foreach (var item in notifications.OrderBy(n => n.TypeNotification).ToList())
                {
                    try
                    {
                        string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Images\dossier_echus.png");
                        string[] files = File.ReadAllLines(path);

                        if (etat1 != item.TypeNotification.ToString())
                        {
                            abs = new AbsNotification();
                            abs.NbrItems++;
                        }
                        else
                        {
                            abs.NbrItems++;
                        }
                        etat1 = item.TypeNotification.ToString();
                    }
                    catch (Exception)
                    {}
                }
            }
            catch (Exception)
            {}
            return absNotifications.ToList();
        }
        #endregion


    }
}