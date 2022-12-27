using genetrix.Models;
using FileManagerApp.Areas.FileManager.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Data.Entity.Core.Objects;
using genetrix.Migrations;

namespace genetrix.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());
        }

        public DbSet<Importateur> Importateurs { get; set; }
        public DbSet<Bien> Biens { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ClientUserRole> ClientUserRoles { get; set; }
        public DbSet<FichierMail> FichierMails { get; set; }
        public DbSet<DossierSupprime> DossierSupprimes { get; set; }
        public DbSet<BanqueTierClient> banqueTierClients { get; set; }
        public DbSet<MotifsTransfert> ModifsTransferts { get; set; }
        public DbSet<NumCompteBeneficiaire> CompteBeneficiaires { get; set; }
        public DbSet<NumCompte> NumComptesClients { get; set; }
        public DbSet<Historisation> GetHistorisations { get; set; }
        public DbSet<Theme> GetThemes { get; set; }
        public DbSet<ePub> GetEPubs { get; set; }
        public DbSet<ImageChat> GetImageChats { get; set; }
        public DbSet<Contact> GetContacts { get; set; }
        public DbSet<Adresse> GetAdresses { get; set; }
        public DbSet<TypeStructure> GetTypeStructures { get; set; }
        public DbSet<Banque> GetBanques { get; set; }
        public DbSet<Backoffice> GetBackoffices { get; set; }
        public DbSet<BanqueClient> GetBanqueClients { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ClFichier> GetClFichiers { get; set; }
        public DbSet<Client> GetClients { get; set; }
        public DbSet<DeviseMonetaire> GetDeviseMonetaires { get; set; }
        public DbSet<XtraRole> XtraRoles { get; set; }
        public DbSet<Composant> GetComposants { get; set; }
        public DbSet<IHMStructure> GetIHMStructures { get; set; }
        public DbSet<IHM> GetIHMs { get; set; }
        public DbSet<Entitee> GetEntitees { get; set; }
        public DbSet<Entitee_Role> GetEntitee_Roles { get; set; }
        public DbSet<DocumentAttache> GetDocumentAttaches { get; set; }
        public DbSet<Documentation> GetDocumentations { get; set; }
        public DbSet<DocumentAttacheDossier> GetDocumentAttacheDossiers { get; set; }
        public DbSet<DocumentAttendus> GetDocumentAttendus { get; set; }
        public DbSet<Dossier> GetDossiers { get; set; }
        public DbSet<DossierNotification> GetDossierNotifications { get; set; }
        public DbSet<FormatRefInderne> GetFormatRefIndernes { get; set; }
        public DbSet<Justificatif> GetJustificatifs { get; set; }
        public DbSet<Mail> GetMails { get; set; }
        public DbSet<NotifDossier> GetNotifDossiers { get; set; }
        public DbSet<Notifications> GetNotifications { get; set; }
        public DbSet<NotifReference> GetNotifReferences { get; set; }
        public DbSet<Pays> GetPays { get; set; }
        public DbSet<PieceJointe> GetPieceJointes { get; set; }
        public DbSet<ReferenceBanque> GetReferenceBanques { get; set; }
        public DbSet<Fournisseurs> GetFournisseurs { get; set; }
        public DbSet<CompteBanqueCommerciale> GetCompteBanqueCommerciales { get; set; }
        public DbSet<CompteClient> GetCompteClients { get; set; }

        public DbSet<ReferenceNotification> GetReferenceNotifications { get; set; }
        public DbSet<Session> GetSessions { get; set; }
        public DbSet<StatutDossier> GetStatutDossiers { get; set; }
        public DbSet<Statut> GetStatuts { get; set; }
        public DbSet<StatutReference> GetStatutReferences { get; set; }
        public DbSet<Ville> GetVilles { get; set; }
        public DbSet<UneImage> GetAllImages { get; set; }
        public DbSet<ImageUtilisateur> GetImageUtilisateurs { get; set; }
        public DbSet<ImageJustificatif> GetImageJustificatifs { get; set; }
        public DbSet<ImageInstruction> GetImageInstructions { get; set; }
        public DbSet<ImageDocumentAttache> GetImageDocumentAttaches { get; set; }
        public DbSet<GroupeComposant> GroupeComposants { get; set; }
        public DbSet<ServiceTransfert> ServiceTransferts { get; set; }
        public DbSet<ActionIHM> Actions { get; set; }
        public DbSet<GroupeDisposionAccueil> GroupeDisposionAccueilss { get; set; }
        public DbSet<VariableStatut> GetVariableStatuts { get; set; }
        public DbSet<DossierStructure> GetDossierStructures { get; set; }
        public DbSet<UsersExterne> GetUsersExternes { get; set; }
        public DbSet<OperateurSwift> GetOperateurSwifts { get; set; }
        public DbSet<Signataire> GetSignataires { get; set; }
        public DbSet<Correspondant> GetCorrespondants { get; set; }
        public DbSet<CompteXAF> GetCompteXAFs { get; set; }
        public DbSet<FacturePiece> FacturePieces { get; set; }
        public DbSet<Reference> GetReferences { get; set; }
        public DbSet<DFX> GetDFXs { get; set; }
        public DbSet<MessageItem> GetMessageItems { get; set; }
        public IEnumerable ApplicationUsers { get; internal set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<genetrix.Models.TypeNotification> TypeNotifications { get; set; }

        public System.Data.Entity.DbSet<genetrix.Models.Agence> Agences { get; set; }
        public DbSet<Conformite> Conformites { get;  set; }
        public DbSet<Structure> Structures { get;  set; }
        //public DbSet<Agence> Agences { get;  set; }
        public DbSet<DirectionMetier> DirectionMetiers { get;  set; }
        public DbSet<FileItem> FileItems { get;  set; }

        public IQueryable<Dossier> _GetDossiers(ApplicationUser user=null)
        {
            try
            {
                if (user is CompteClient)
                {
                    var clientId = (user as CompteClient).ClientId;
                    return GetDossiers.Where(d => d.ClientId == clientId);
                }
                else if (user is CompteAdmin)
                {
                    return GetDossiers;
                }
                else if (user is CompteBanqueCommerciale)
                {
                    var banqueId = (user as CompteBanqueCommerciale).Structure.BanqueId(this);

                    if (!(user as CompteBanqueCommerciale).EstAdmin)
                    {
                        var res=VariablGlobales.Access(user as CompteBanqueCommerciale, "dossier");
                        if (res["lire_pour_tout"])
                            return GetDossiers.Where(d => d.Site.BanqueId(this) == banqueId);
                        else if (res["lire"])
                        {
                            var siteId = (user as CompteBanqueCommerciale).Structure.Id;
                            return GetDossiers.Where(d => d.Site.BanqueId(this) == banqueId && d.IdSite ==siteId);
                        }
                        else
                            return GetDossiers.Where(d => d.Site.BanqueId(this) == -1);
                    }else
                        return GetDossiers.Where(d => d.Site.BanqueId(this) == banqueId);
                }
            }
            catch (System.Exception)
            {}

            return null;
        }

        public IQueryable<Client> _GetClients(ApplicationUser user = null)
        {
            try
            {
                if (user is CompteClient)
                {
                    var clientId = (user as CompteClient).ClientId;
                    return GetClients.Where(d => d.Id == clientId);
                }
                else if (user is CompteAdmin)
                {
                    return GetClients;
                }
                else if (user is CompteBanqueCommerciale)
                {
                    var banqueId = (user as CompteBanqueCommerciale).Structure.BanqueId(this);
                    if (!(user as CompteBanqueCommerciale).EstAdmin)
                    {
                        var res = VariablGlobales.Access(user as CompteBanqueCommerciale, "dossier");
                        if (res["lire_pour_tout"])
                            return from d in GetBanqueClients
                                   where d.Site.BanqueId(this) == banqueId 
                                   select d.Client;
                        else if (res["lire"])
                        {
                            var siteId = (user as CompteBanqueCommerciale).Structure.Id;
                            return from d in GetBanqueClients
                                   where d.Site.BanqueId(this) == banqueId && d.IdSite==siteId
                                   select d.Client;
                        }
                        else
                            return GetClients.Where(d => d.Id == -1);
                    }
                    else
                       return from d in GetBanqueClients
                        where d.Site.BanqueId(this) == banqueId
                        select d.Client;
                }
            }
            catch (System.Exception)
            { }

            return null;
        }

        public System.Data.Entity.DbSet<genetrix.Models.CompteNostro> CompteNostroes { get; set; }

        public System.Data.Entity.DbSet<genetrix.Models.PubItem> PubItems { get; set; }

        //public class FilteredApplicationDbContext : ApplicationDbContext
        //{
        //    public IQueryable<Structure> GetFilteredProducts(string userRole)
        //    {
        //        //return Structures.Where(str => str.ut == userRole);
        //        return Structures;
        //    }
        //}
    }
}