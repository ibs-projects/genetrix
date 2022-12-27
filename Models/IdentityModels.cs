using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using genetrix.Models;
using genetrix.Models;
using genetrix.Models.Fonctions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace genetrix.Models
{
    // Vous pouvez ajouter des données de profil pour l'utilisateur en ajoutant d'autres propriétés à votre classe ApplicationUser. Pour en savoir plus, consultez https://go.microsoft.com/fwlink/?LinkID=317594.
    [Serializable()]
    public class ApplicationUser : IdentityUser,IDelateCustom
    {
        public bool Remove(ApplicationDbContext db)
        {
            try
            {
                db.Users.Remove(this);
                return true;
            }
            catch (Exception)
            { }
            return false;
        }

         public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Notez qu'authenticationType doit correspondre à l'élément défini dans CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Ajouter les revendications personnalisées de l’utilisateur ici
            
            return userIdentity;
        }

        Session Session;
        public ApplicationUser(Session session)
        //: base(session)
        {
            new Session();
        }

        public ApplicationUser()
        {
            
        }

        public virtual void AfterConstruction()
        {

        }

        [Display(Name ="Est administrateur")]
        public bool EstAdmin { get; set; }

        public string NomComplet
        {
            get
            {
                var nc= $"{Prenom} {Nom}";
                try
                {
                    if (string.IsNullOrEmpty(nc.Replace(" ", "")))
                        nc = NomUtilisateur;
                    if (string.IsNullOrEmpty(nc))
                        nc = Email.Split('@')[0];
                }
                catch (Exception)
                {}

                return nc;
            }
        }

        [NotMapped]
        public string PassWordTmp { get; set; }

        //[ForeignKey("ImageUtilisateur")]
        //[Display(Name ="Photo")]
        //public int? ImageUtilisateurId { get; set; } = null;

        /// <summary>
        /// Une seule image est prise en charge
        /// </summary>
        public virtual ICollection<ImageUtilisateur> GetImageUtilisateurs { get; set; }
        public virtual ICollection<Notifications> GetNotifications { get; set; }

        private string name;
        public string Nom
        {
            get { return name; }
            set { name = value; }
        }

        public string NomUtilisateur { get; set; }

        private string prenom;
        public string Prenom
        {
            get { return prenom; }
            set { prenom = value; }
        }

        [Display(Name = "Suivi chat")]
        public virtual bool SuitChat { get; set; }

        public UneImage ImageProfile { get; set; }

        public string GetImageProfile
        {
            get {
                if (ImageProfile != null) return ImageProfile.GetImage();
                return "#";
            }
        }

        [Display(Name ="Civilité")]
        public Sexe Sexe { get; set; }

        [Display(Name = "Téléphone 2")]
        public string Tel2 { get; set; }

        [Display(Name = "Téléphone 1")]
        public string Tel1 { get; set; }

        public string AllPhone
        {
            get { return Tel1+(!string.IsNullOrEmpty(Tel2)?" - "+Tel2:""); }
        }


        //public int? RoleId { get; set; } = null;

        //public virtual XtraRole Role { get; set; }

        public virtual ICollection<Chat> MesChats { get; set; }

        public virtual ICollection<Chat> Chats { get; set; }

        //public virtual ICollection<Mail> Mails { get; set; }

        protected void OnChanged(string propertyName, object oldValue, object newValue)
        {
            try
            {
                if (propertyName == "Specialite" || propertyName == "Specialite")
                {
                    try
                    {
                        UserName = (Prenom + "." + Nom).ToLower();
                    }
                    catch (Exception)
                    { }
                }
            }
            catch (System.Exception)
            { }

        }

        #region Notifications
        //public virtual Type GetType(Guid oid)
        //{
        //    return this.GetType();
        //}

        public virtual IEnumerable GetMessages(List<Dossier> dossiers = null, Client client = null, IdentityUserRole role = null, ApplicationUser user = null, EtatDossier etat = EtatDossier.Encours, DateTime dateTime = default, string RefInterne = "", DateTime DateCreaBanque = default, DateTime DateModif = default, Guid oidBanque = default, double Montant = 0, string Fournisseur = "", Guid oidReferenceExterne = default, Guid oidClient = default)
        {
            ICollection<Notifications> notifications = new List<Notifications>()
            {
                new Notifications()
                {
                    Objet="Dossiers échus",
                    Message=" dossiers échus veillez les vérifier à nouveau",
                    Couleur=Color.Red
                }
                //new Notifications(this.Session)
                //{
                //    Objet="Dossiers épurés",
                //    Message="Vos dossiers épurés. Vous pouvez les archiver.",
                //    Couleur=Color.Green
                //},
                //new Notifications(this.Session)
                //{
                //    Objet="Dossiers encours",
                //    Message="dossiers encours de validation",
                //    Couleur=Color.Red
                //}
            };

            ICollection<AbsNotification> absNotifications = new List<AbsNotification>();
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\dossier_echus.png");

            string[] files = File.ReadAllLines(path);
            var n = Session.Query<Notifications>().FirstOrDefault();
            foreach (var item in notifications)
            {
                absNotifications.Add(
                    new AbsNotification()
                    {
                        //Notifications=item
                    }
                );
            }
            return absNotifications;
        }

        public virtual void SetDossierNotification(Dossier dossier)
        {

        }

        public virtual void SetReferenceNotification(ReferenceBanque reference)
        {

        }

        public virtual void SetUserNotification(ApplicationUser utilisateur)
        {

        }

        public virtual IList<AbsNotification> DossiersNotifications { get => throw new NotImplementedException(); }
        public virtual IList<AbsNotification> ReferencesNotifications { get => throw new NotImplementedException(); }
        public virtual IList<AbsNotification> UsersNotifications { get => throw new NotImplementedException(); }
        public int? ChatId { get; set; }

        #endregion


        public byte[] GetImageNotifDossier(EtatDossier etatDossier)
        {
            byte[] img = null;
            try
            {
               // img = (Session.Query<StatutDossier>().FirstOrDefault(s => (s as StatutDossier).EtatDossier == etatDossier) as StatutDossier).Image;
            }
            catch (Exception)
            { }
            return img;
        }

        public string GetMessageNotifDossier(EtatDossier etatDossier)
        {
            string msg = null;
            try
            {
                //msg = (Session.Query<StatutDossier>().FirstOrDefault(s => (s as StatutDossier).EtatDossier == etatDossier) as StatutDossier).Message;
            }
            catch (Exception)
            { }
            return msg;
        }

        public string GetTitreNotifDossier(EtatDossier etatDossier)
        {
            string title = null;
            try
            {
                //title = (Session.Query<StatutDossier>().FirstOrDefault(s => (s as StatutDossier).EtatDossier == etatDossier) as StatutDossier).Titre;
            }
            catch (Exception)
            { }
            return title;
        }

        public Color GetCouleurNotifDossier(EtatDossier etatDossier)
        {
            Color color = default;
            try
            {
                //color = (Session.Query<StatutDossier>().FirstOrDefault(s => (s as StatutDossier).EtatDossier == etatDossier) as StatutDossier).Couleur;
            }
            catch (Exception)
            { }
            return color;
        }

        public string GetLienNotifDossier(EtatDossier etatDossier)
        {
            string lien = "";
            try
            {
               // lien = (Session.Query<StatutDossier>().FirstOrDefault(s => (s as StatutDossier).EtatDossier == etatDossier) as StatutDossier).Lien;
            }
            catch (Exception)
            { }
            return lien;
        }
    }
}