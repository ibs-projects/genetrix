namespace genetrix.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ActionIHMs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Intitule = c.String(),
                        Url = c.String(),
                        Icon = c.Binary(),
                        Recherche = c.String(),
                        IconName = c.String(),
                        TypeObjet = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Composants",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Numero = c.Int(nullable: false),
                        EstActif = c.Boolean(nullable: false),
                        Description = c.String(),
                        NumDiscontinue = c.String(),
                        Localistion = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        IdGroupe = c.Int(),
                        IdAction = c.Int(),
                        IdGroupeDisposionAccueil = c.Int(),
                        Recherche = c.String(),
                        NumeroMax = c.Int(nullable: false),
                        NumeroMin = c.Int(nullable: false),
                        Info = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ActionIHMs", t => t.IdAction)
                .ForeignKey("dbo.GroupeComposants", t => t.IdGroupe)
                .ForeignKey("dbo.GroupeDisposionAccueils", t => t.IdGroupeDisposionAccueil)
                .Index(t => t.IdGroupe)
                .Index(t => t.IdAction)
                .Index(t => t.IdGroupeDisposionAccueil);
            
            CreateTable(
                "dbo.IHMs",
                c => new
                    {
                        XRoleId = c.Int(nullable: false),
                        ComposantId = c.Int(nullable: false),
                        Lire = c.Boolean(nullable: false),
                        Ecrire = c.Boolean(nullable: false),
                        Supprimer = c.Boolean(nullable: false),
                        Créer = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.XRoleId, t.ComposantId })
                .ForeignKey("dbo.Composants", t => t.ComposantId, cascadeDelete: true)
                .ForeignKey("dbo.XtraRoles", t => t.XRoleId, cascadeDelete: true)
                .Index(t => t.XRoleId)
                .Index(t => t.ComposantId);
            
            CreateTable(
                "dbo.XtraRoles",
                c => new
                    {
                        RoleId = c.Int(nullable: false, identity: true),
                        Nom = c.String(),
                        IdStructure = c.Int(),
                        EstAdmin = c.Boolean(nullable: false),
                        EstBackOffice = c.Boolean(nullable: false),
                        NiveauDossier = c.Int(),
                        VoirDossiersAutres = c.Boolean(nullable: false),
                        VoirUsersAutres = c.Boolean(nullable: false),
                        VoirClientAutres = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.RoleId)
                .ForeignKey("dbo.Structures", t => t.IdStructure)
                .Index(t => t.IdStructure);
            
            CreateTable(
                "dbo.Entitee_Role",
                c => new
                    {
                        IdXRole = c.Int(nullable: false),
                        IdEntitee = c.Int(nullable: false),
                        Lire = c.Boolean(nullable: false),
                        Ecrire = c.Boolean(nullable: false),
                        Supprimer = c.Boolean(nullable: false),
                        Créer = c.Boolean(nullable: false),
                        Lire_Pour_Tout = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.IdXRole, t.IdEntitee })
                .ForeignKey("dbo.Entitees", t => t.IdEntitee, cascadeDelete: true)
                .ForeignKey("dbo.XtraRoles", t => t.IdXRole, cascadeDelete: true)
                .Index(t => t.IdXRole)
                .Index(t => t.IdEntitee);
            
            CreateTable(
                "dbo.Entitees",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.String(),
                        Niveau = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Structures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nom = c.String(),
                        Adresse = c.String(),
                        Ville = c.String(),
                        Pays = c.String(),
                        Telephone = c.String(),
                        Telephone2 = c.String(),
                        CodeEtablissement = c.String(maxLength: 5),
                        NiveauDossier = c.Int(),
                        NiveauMaxManager = c.Int(),
                        NiveauMaxDossier = c.Int(),
                        VoirDossiersAutres = c.Boolean(nullable: false),
                        VoirUsersAutres = c.Boolean(nullable: false),
                        VoirClientAutres = c.Boolean(nullable: false),
                        IdTypeStructure = c.Int(),
                        EstAgence = c.Boolean(nullable: false),
                        SuitChat = c.Boolean(nullable: false),
                        NiveauH = c.Int(nullable: false),
                        IdResponsable = c.String(maxLength: 128),
                        LireTouteReference = c.Boolean(nullable: false),
                        IdDirectionMetier = c.Int(),
                        IdBanque = c.Int(),
                        IdBanque1 = c.Int(),
                        IdBanque2 = c.Int(),
                        IdAgence = c.Int(),
                        Discriminator = c.String(maxLength: 128),
                        TypeStructure_Id = c.Int(),
                        Structure_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Banque", t => t.IdBanque)
                .ForeignKey("dbo.TypeStructures", t => t.TypeStructure_Id)
                .ForeignKey("dbo.Structures", t => t.IdDirectionMetier)
                .ForeignKey("dbo.Banque", t => t.IdBanque1)
                .ForeignKey("dbo.Banque", t => t.IdBanque2)
                .ForeignKey("dbo.CompteBanqueCommerciale", t => t.IdResponsable)
                .ForeignKey("dbo.Structures", t => t.Structure_Id)
                .ForeignKey("dbo.TypeStructures", t => t.IdTypeStructure)
                .ForeignKey("dbo.Structures", t => t.IdAgence)
                .Index(t => t.IdTypeStructure)
                .Index(t => t.IdResponsable)
                .Index(t => t.IdDirectionMetier)
                .Index(t => t.IdBanque)
                .Index(t => t.IdBanque1)
                .Index(t => t.IdBanque2)
                .Index(t => t.IdAgence)
                .Index(t => t.TypeStructure_Id)
                .Index(t => t.Structure_Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        EstAdmin = c.Boolean(nullable: false),
                        Nom = c.String(),
                        NomUtilisateur = c.String(),
                        Prenom = c.String(),
                        SuitChat = c.Boolean(nullable: false),
                        Sexe = c.Int(nullable: false),
                        Tel2 = c.String(),
                        Tel1 = c.String(),
                        ChatId = c.Int(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                        ImageProfile_Id = c.Int(),
                        Chat_ChatId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UneImages", t => t.ImageProfile_Id)
                .ForeignKey("dbo.Chat", t => t.Chat_ChatId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex")
                .Index(t => t.ImageProfile_Id)
                .Index(t => t.Chat_ChatId);
            
            CreateTable(
                "dbo.Chat",
                c => new
                    {
                        ChatId = c.Int(nullable: false, identity: true),
                        NumeroTopic = c.String(),
                        Sujet = c.String(),
                        DateHeure = c.DateTime(),
                        DateFermeture = c.DateTime(),
                        Token = c.Guid(nullable: false),
                        Statut = c.Int(nullable: false),
                        Situation = c.Int(nullable: false),
                        EntrepriseId = c.Int(),
                        EmetteurId = c.String(maxLength: 128),
                        DernierEcrit = c.Byte(nullable: false),
                        ApplicationUser_Id = c.String(maxLength: 128),
                        ApplicationUser_Id1 = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ChatId)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id1)
                .ForeignKey("dbo.AspNetUsers", t => t.EmetteurId)
                .Index(t => t.EmetteurId)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id1);
            
            CreateTable(
                "dbo.MessageItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        Date = c.DateTime(nullable: false),
                        EmetteurName = c.String(),
                        Lu = c.Boolean(nullable: false),
                        LienImage = c.String(),
                        ChatId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Chat", t => t.ChatId, cascadeDelete: true)
                .Index(t => t.ChatId);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UneImages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Url = c.String(),
                        DateCreation = c.DateTime(nullable: false),
                        DerniereModif = c.DateTime(nullable: false),
                        Titre = c.String(),
                        NomCreateur = c.String(),
                        ApplicationUserId = c.String(maxLength: 128),
                        JustificatifId = c.Int(),
                        DocumentAttacheId = c.Int(),
                        DossierId = c.Int(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .ForeignKey("dbo.Justificatif", t => t.JustificatifId)
                .ForeignKey("dbo.DocumentAttache", t => t.DocumentAttacheId, cascadeDelete: true)
                .ForeignKey("dbo.Dossier", t => t.DossierId, cascadeDelete: true)
                .Index(t => t.ApplicationUserId)
                .Index(t => t.JustificatifId)
                .Index(t => t.DocumentAttacheId)
                .Index(t => t.DossierId);
            
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Message = c.String(),
                        Objet = c.String(),
                        LienImage = c.String(),
                        DestinataireId = c.String(),
                        DestinataireNom = c.String(),
                        EmetteurNom = c.String(),
                        EmetteurImage = c.String(),
                        Date = c.DateTime(),
                        Lu = c.Boolean(nullable: false),
                        DossierId = c.Int(),
                        Image = c.Binary(),
                        TypeNotification_Id = c.Int(),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TypeNotifications", t => t.TypeNotification_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.TypeNotification_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.TypeNotifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Message = c.String(),
                        Type = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FichierMails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Url = c.String(),
                        IdMail = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Mail", t => t.IdMail)
                .Index(t => t.IdMail);
            
            CreateTable(
                "dbo.MailSupprimes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(),
                        UserId = c.String(),
                        UserEmail = c.String(),
                        MailId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Mail", t => t.MailId)
                .Index(t => t.MailId);
            
            CreateTable(
                "dbo.ReferenceNotification",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Desactive = c.Boolean(nullable: false),
                        Lue = c.Boolean(nullable: false),
                        Titre = c.String(),
                        Detail = c.String(),
                        Notification_Id = c.Int(),
                        ReferenceBanque_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.NotifReference", t => t.Notification_Id)
                .ForeignKey("dbo.ReferenceBanque", t => t.ReferenceBanque_Id)
                .Index(t => t.Notification_Id)
                .Index(t => t.ReferenceBanque_Id);
            
            CreateTable(
                "dbo.References",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NumeroRef = c.String(),
                        DateReception = c.DateTime(),
                        BanqueId = c.Int(nullable: false),
                        CompteBEACEditer = c.String(),
                        BanqueDomiciliaire = c.String(),
                        Pays = c.String(),
                        Ville = c.String(),
                        CodeSwift = c.String(),
                        IdCompteAcrediter = c.Int(nullable: false),
                        Signataire1 = c.String(),
                        Signataire2 = c.String(),
                        Signataire3 = c.String(),
                        Signataire4 = c.String(),
                        NOTIFICATION = c.Int(),
                        Numero = c.String(),
                        NumeroAnnexe = c.String(),
                        CorrespondantB = c.String(),
                        DateDebut = c.DateTime(),
                        DateFin = c.DateTime(),
                        Telechargements = c.Int(),
                        Discriminator = c.String(maxLength: 128),
                        Courrier_Id = c.Int(),
                        MiseEnDemeure_Id = c.Int(),
                        MT_Id = c.Int(),
                        RecapTransfert_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Banque", t => t.BanqueId)
                .ForeignKey("dbo.DocumentAttache", t => t.Courrier_Id)
                .ForeignKey("dbo.DocumentAttache", t => t.MiseEnDemeure_Id)
                .ForeignKey("dbo.DocumentAttache", t => t.MT_Id)
                .ForeignKey("dbo.DocumentAttache", t => t.RecapTransfert_Id)
                .Index(t => t.BanqueId)
                .Index(t => t.Courrier_Id)
                .Index(t => t.MiseEnDemeure_Id)
                .Index(t => t.MT_Id)
                .Index(t => t.RecapTransfert_Id);
            
            CreateTable(
                "dbo.BanqueClient",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        IdSite = c.Int(nullable: false),
                        IdGestionnaire = c.String(maxLength: 128),
                        DateCreation = c.DateTime(),
                        Structure_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Client", t => t.ClientId, cascadeDelete: true)
                .ForeignKey("dbo.CompteBanqueCommerciale", t => t.IdGestionnaire)
                .ForeignKey("dbo.Structures", t => t.IdSite, cascadeDelete: true)
                .ForeignKey("dbo.Structures", t => t.Structure_Id)
                .Index(t => t.ClientId)
                .Index(t => t.IdSite)
                .Index(t => t.IdGestionnaire)
                .Index(t => t.Structure_Id);
            
            CreateTable(
                "dbo.Client",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nom = c.String(nullable: false),
                        Email = c.String(),
                        CodeEtablissement = c.String(maxLength: 5),
                        Profession = c.String(),
                        Pays = c.String(),
                        Logo = c.String(),
                        Adresse = c.String(),
                        Ville = c.String(),
                        Telephone = c.String(),
                        DateCreation = c.DateTime(),
                        ModeRestraint = c.Boolean(nullable: false),
                        AtestationHinneur_Id = c.Int(),
                        CarteIdentie_Id = c.Int(),
                        EtatFinanciers_Id = c.Int(),
                        FicheKYC_Id = c.Int(),
                        JustifDomicile_Id = c.Int(),
                        PlanLOcalisationDomi_Id = c.Int(),
                        PlanLSS_Id = c.Int(),
                        ProcesVerbal_Id = c.Int(),
                        RCCM_Cl_Id = c.Int(),
                        Statut_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DocumentAttache", t => t.AtestationHinneur_Id)
                .ForeignKey("dbo.DocumentAttache", t => t.CarteIdentie_Id)
                .ForeignKey("dbo.DocumentAttache", t => t.EtatFinanciers_Id)
                .ForeignKey("dbo.DocumentAttache", t => t.FicheKYC_Id)
                .ForeignKey("dbo.DocumentAttache", t => t.JustifDomicile_Id)
                .ForeignKey("dbo.DocumentAttache", t => t.PlanLOcalisationDomi_Id)
                .ForeignKey("dbo.DocumentAttache", t => t.PlanLSS_Id)
                .ForeignKey("dbo.DocumentAttache", t => t.ProcesVerbal_Id)
                .ForeignKey("dbo.DocumentAttache", t => t.RCCM_Cl_Id)
                .ForeignKey("dbo.DocumentAttache", t => t.Statut_Id)
                .Index(t => t.AtestationHinneur_Id)
                .Index(t => t.CarteIdentie_Id)
                .Index(t => t.EtatFinanciers_Id)
                .Index(t => t.FicheKYC_Id)
                .Index(t => t.JustifDomicile_Id)
                .Index(t => t.PlanLOcalisationDomi_Id)
                .Index(t => t.PlanLSS_Id)
                .Index(t => t.ProcesVerbal_Id)
                .Index(t => t.RCCM_Cl_Id)
                .Index(t => t.Statut_Id);
            
            CreateTable(
                "dbo.Adresse",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Tel1 = c.String(),
                        Email = c.String(),
                        CodePostal = c.String(),
                        ClientId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Client", t => t.ClientId, cascadeDelete: true)
                .Index(t => t.ClientId);
            
            CreateTable(
                "dbo.DocumentAttache",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nom = c.String(),
                        DateCreation = c.DateTime(),
                        EstAttestation = c.Boolean(nullable: false),
                        DateSignature = c.DateTime(),
                        IdBanqueTierce = c.Int(),
                        NomBreDoc = c.Int(nullable: false),
                        Signature1 = c.Boolean(nullable: false),
                        Signature2 = c.Boolean(nullable: false),
                        AttestSurHonneur = c.Boolean(nullable: false),
                        DossierId = c.Int(),
                        IdReference = c.Int(),
                        TypeDocumentAttaché = c.Int(nullable: false),
                        ClientId = c.Int(),
                        IdFournisseur = c.Int(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        Dossier_Dossier_Id = c.Int(),
                        Dossier_Dossier_Id1 = c.Int(),
                        Fournisseurs_Id = c.Int(),
                        FichierImage_Id = c.Int(),
                        Client_Id = c.Int(),
                        BanqueTierClient_Id = c.Int(),
                        Client_Id1 = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Dossier", t => t.Dossier_Dossier_Id)
                .ForeignKey("dbo.Dossier", t => t.Dossier_Dossier_Id1)
                .ForeignKey("dbo.Fournisseurs", t => t.Fournisseurs_Id)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.Dossier", t => t.DossierId)
                .ForeignKey("dbo.ClFichier", t => t.FichierImage_Id)
                .ForeignKey("dbo.Client", t => t.Client_Id)
                .ForeignKey("dbo.BanqueTierClients", t => t.BanqueTierClient_Id)
                .ForeignKey("dbo.Client", t => t.Client_Id1)
                .Index(t => t.DossierId)
                .Index(t => t.ClientId)
                .Index(t => t.Dossier_Dossier_Id)
                .Index(t => t.Dossier_Dossier_Id1)
                .Index(t => t.Fournisseurs_Id)
                .Index(t => t.FichierImage_Id)
                .Index(t => t.Client_Id)
                .Index(t => t.BanqueTierClient_Id)
                .Index(t => t.Client_Id1);
            
            CreateTable(
                "dbo.Dossier",
                c => new
                    {
                        Dossier_Id = c.Int(nullable: false, identity: true),
                        NumeroDossier = c.Int(nullable: false),
                        NatureOperation = c.Int(nullable: false),
                        SupprimeClient = c.Boolean(nullable: false),
                        SupprimeBanque = c.Boolean(nullable: false),
                        DFX6FP6BEAC = c.Byte(nullable: false),
                        ValiderDouane = c.Boolean(nullable: false),
                        CodeEtablissement = c.String(maxLength: 5),
                        CodeAgence = c.String(maxLength: 5),
                        DateSignInst = c.DateTime(),
                        EtapeValidationClient = c.Byte(nullable: false),
                        EtapesDosier = c.Int(),
                        EtapePrecedenteDosier = c.Int(),
                        NumCompteClient = c.String(maxLength: 11),
                        PaysClient = c.String(),
                        Cle = c.String(maxLength: 2),
                        NumCompteBenef = c.String(maxLength: 11),
                        CleCompteBenf = c.String(maxLength: 2),
                        IdBanqueBenef = c.Int(),
                        PaysBanqueBenf = c.String(),
                        RibBanqueBenf = c.String(),
                        NomBanqueBenf = c.String(),
                        AdresseBanqueBenf = c.String(),
                        CodeAgenceBenf = c.String(),
                        CodeEtsBenf = c.String(),
                        NumBanqueBenf = c.String(),
                        Message = c.String(),
                        Motif = c.String(),
                        EstPasséConformite = c.Boolean(nullable: false),
                        ObtDevise = c.DateTime(),
                        Date_Etape2 = c.DateTime(),
                        Date_Etape3 = c.DateTime(),
                        Date_Etape4 = c.DateTime(),
                        Date_Etape5 = c.DateTime(),
                        Date_Etape6 = c.DateTime(),
                        Date_Etape7 = c.DateTime(),
                        Date_Etape8 = c.DateTime(),
                        Date_Etape9 = c.DateTime(),
                        Date_Etape10 = c.DateTime(),
                        Date_Etape11 = c.DateTime(),
                        Date_Etape12 = c.DateTime(),
                        Date_Etape13 = c.DateTime(),
                        Date_Etape14 = c.DateTime(),
                        Date_Etape15 = c.DateTime(),
                        Date_Etape16 = c.DateTime(),
                        Date_Etape17 = c.DateTime(),
                        Date_Etape18 = c.DateTime(),
                        Date_Etape19 = c.DateTime(),
                        Date_Etape20 = c.DateTime(),
                        Date_Etape21 = c.DateTime(),
                        Date_Etape22 = c.DateTime(),
                        Date_Etape23 = c.DateTime(),
                        Date_Etape24 = c.DateTime(),
                        Date_Etape25 = c.DateTime(),
                        DateNouvelleEtape = c.DateTime(),
                        NbrTelechargement = c.Int(nullable: false),
                        NbrTelechargementApurement = c.Int(nullable: false),
                        Intitule = c.String(),
                        RefInterne = c.String(),
                        Categorie = c.Byte(),
                        CategorieDossier = c.Int(nullable: false),
                        IdAgentResponsableDossier = c.String(maxLength: 128),
                        IdPrecedentResponsable = c.String(),
                        IdResponsableAgence = c.String(maxLength: 128),
                        IdResponsableConformite = c.String(maxLength: 128),
                        IdResponsableBackOffice = c.String(maxLength: 128),
                        IdResponsableTransfert = c.String(maxLength: 128),
                        DateModif = c.DateTime(),
                        NbreJustif = c.Int(nullable: false),
                        IdSite = c.Int(nullable: false),
                        IdGestionnaire = c.String(),
                        Montant = c.Single(nullable: false),
                        ContreValeurXAF = c.Single(),
                        FournisseurId = c.Int(),
                        AdresseFournisseur = c.String(),
                        DateDepotBank = c.DateTime(),
                        DateEnvoiBeac = c.DateTime(),
                        DateArchivage = c.DateTime(),
                        DateCreationApp = c.DateTime(),
                        ApplicationUser = c.String(),
                        DeviseMonetaireId = c.Int(nullable: false),
                        Traité = c.Boolean(nullable: false),
                        MarchandiseArrivee = c.Boolean(nullable: false),
                        ValiderConformite = c.Boolean(nullable: false),
                        ReferenceExterneId = c.Int(),
                        DfxId = c.Int(),
                        NumDFX = c.String(),
                        RefDFX = c.String(),
                        TailleFiles = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        MontantEnLettre = c.String(),
                        Courrier_Id = c.Int(),
                        DeclarImport_Id = c.Int(),
                        DocumentTransport_Id = c.Int(),
                        DomicilImport_Id = c.Int(),
                        EnDemeure_Id = c.Int(),
                        EnDemeure2_Id = c.Int(),
                        LettreEngage_Id = c.Int(),
                        MT_Id = c.Int(),
                        QuittancePay_Id = c.Int(),
                        Reference_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Dossier_Id)
                .ForeignKey("dbo.CompteBanqueCommerciale", t => t.IdResponsableAgence)
                .ForeignKey("dbo.CompteBanqueCommerciale", t => t.IdResponsableBackOffice)
                .ForeignKey("dbo.CompteBanqueCommerciale", t => t.IdResponsableConformite)
                .ForeignKey("dbo.CompteBanqueCommerciale", t => t.IdAgentResponsableDossier)
                .ForeignKey("dbo.CompteBanqueCommerciale", t => t.IdResponsableTransfert)
                .ForeignKey("dbo.Client", t => t.ClientId, cascadeDelete: true)
                .ForeignKey("dbo.DocumentAttache", t => t.Courrier_Id)
                .ForeignKey("dbo.DocumentAttache", t => t.DeclarImport_Id)
                .ForeignKey("dbo.DeviseMonetaire", t => t.DeviseMonetaireId, cascadeDelete: true)
                .ForeignKey("dbo.References", t => t.DfxId)
                .ForeignKey("dbo.DocumentAttache", t => t.DocumentTransport_Id)
                .ForeignKey("dbo.DocumentAttache", t => t.DomicilImport_Id)
                .ForeignKey("dbo.DocumentAttache", t => t.EnDemeure_Id)
                .ForeignKey("dbo.DocumentAttache", t => t.EnDemeure2_Id)
                .ForeignKey("dbo.Fournisseurs", t => t.FournisseurId)
                .ForeignKey("dbo.DocumentAttache", t => t.LettreEngage_Id)
                .ForeignKey("dbo.DocumentAttache", t => t.MT_Id)
                .ForeignKey("dbo.DocumentAttache", t => t.QuittancePay_Id)
                .ForeignKey("dbo.ReferenceBanque", t => t.ReferenceExterneId)
                .ForeignKey("dbo.Structures", t => t.IdSite, cascadeDelete: true)
                .ForeignKey("dbo.References", t => t.Reference_Id)
                .Index(t => t.IdAgentResponsableDossier)
                .Index(t => t.IdResponsableAgence)
                .Index(t => t.IdResponsableConformite)
                .Index(t => t.IdResponsableBackOffice)
                .Index(t => t.IdResponsableTransfert)
                .Index(t => t.IdSite)
                .Index(t => t.FournisseurId)
                .Index(t => t.DeviseMonetaireId)
                .Index(t => t.ReferenceExterneId)
                .Index(t => t.DfxId)
                .Index(t => t.ClientId)
                .Index(t => t.Courrier_Id)
                .Index(t => t.DeclarImport_Id)
                .Index(t => t.DocumentTransport_Id)
                .Index(t => t.DomicilImport_Id)
                .Index(t => t.EnDemeure_Id)
                .Index(t => t.EnDemeure2_Id)
                .Index(t => t.LettreEngage_Id)
                .Index(t => t.MT_Id)
                .Index(t => t.QuittancePay_Id)
                .Index(t => t.Reference_Id);
            
            CreateTable(
                "dbo.DeviseMonetaire",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nom = c.String(),
                        ParitéXAF = c.Double(nullable: false),
                        Libelle = c.String(),
                        ImageLogo = c.Binary(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CompteNostroes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Numero = c.String(),
                        Libellé = c.String(),
                        RIB = c.String(),
                        IdDevise = c.Int(),
                        IdCorrespondant = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Correspondants", t => t.IdCorrespondant, cascadeDelete: true)
                .ForeignKey("dbo.DeviseMonetaire", t => t.IdDevise)
                .Index(t => t.IdDevise)
                .Index(t => t.IdCorrespondant);
            
            CreateTable(
                "dbo.Correspondants",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NomBanque = c.String(),
                        SwiftCode = c.String(),
                        Pays = c.String(),
                        Ville = c.String(),
                        BanqueId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Banque", t => t.BanqueId)
                .Index(t => t.BanqueId);
            
            CreateTable(
                "dbo.Fournisseurs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nom = c.String(),
                        Pays = c.String(),
                        Tel2 = c.String(),
                        Tel1 = c.String(),
                        Email = c.String(),
                        Ville = c.String(),
                        Adresse = c.String(),
                        CodeEts = c.String(maxLength: 5),
                        ClientId = c.Int(nullable: false),
                        CarteIdentiteDirigeants_Id = c.Int(),
                        CopieStatuts_Id = c.Int(),
                        FicheKYCBenefi_Id = c.Int(),
                        JustifDomicileBenefi_Id = c.Int(),
                        ListeAyantDroits_Id = c.Int(),
                        ListeGerants_Id = c.Int(),
                        PriseActeDeclarationCompte_Id = c.Int(),
                        ProcesVerbalNommantDirigeants_Id = c.Int(),
                        RCCM_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DocumentAttache", t => t.CarteIdentiteDirigeants_Id)
                .ForeignKey("dbo.Client", t => t.ClientId, cascadeDelete: true)
                .ForeignKey("dbo.DocumentAttache", t => t.CopieStatuts_Id)
                .ForeignKey("dbo.DocumentAttache", t => t.FicheKYCBenefi_Id)
                .ForeignKey("dbo.DocumentAttache", t => t.JustifDomicileBenefi_Id)
                .ForeignKey("dbo.DocumentAttache", t => t.ListeAyantDroits_Id)
                .ForeignKey("dbo.DocumentAttache", t => t.ListeGerants_Id)
                .ForeignKey("dbo.DocumentAttache", t => t.PriseActeDeclarationCompte_Id)
                .ForeignKey("dbo.DocumentAttache", t => t.ProcesVerbalNommantDirigeants_Id)
                .ForeignKey("dbo.DocumentAttache", t => t.RCCM_Id)
                .Index(t => t.ClientId)
                .Index(t => t.CarteIdentiteDirigeants_Id)
                .Index(t => t.CopieStatuts_Id)
                .Index(t => t.FicheKYCBenefi_Id)
                .Index(t => t.JustifDomicileBenefi_Id)
                .Index(t => t.ListeAyantDroits_Id)
                .Index(t => t.ListeGerants_Id)
                .Index(t => t.PriseActeDeclarationCompte_Id)
                .Index(t => t.ProcesVerbalNommantDirigeants_Id)
                .Index(t => t.RCCM_Id);
            
            CreateTable(
                "dbo.ClFichier",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Url = c.String(),
                        FichierImage = c.Binary(),
                        DateCreaApp = c.DateTime(nullable: false),
                        DateModif = c.DateTime(nullable: false),
                        ClientId = c.String(),
                        Client_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Client", t => t.Client_Id)
                .Index(t => t.Client_Id);
            
            CreateTable(
                "dbo.Justificatif",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EstPartielle = c.Boolean(nullable: false),
                        IdAncienneFacture = c.Int(),
                        Libellé = c.String(),
                        FournisseurJustif = c.String(),
                        BanqueJustif = c.String(),
                        MontantJustif = c.Double(nullable: false),
                        MontantPartiel = c.Double(nullable: false),
                        MontantRestant = c.Double(nullable: false),
                        NumeroJustif = c.String(nullable: false),
                        DateEmissioJustif = c.DateTime(nullable: false),
                        DateCreaAppJustif = c.DateTime(nullable: false),
                        DateModifJustif = c.DateTime(nullable: false),
                        UtilisateurId = c.String(),
                        DeviseJustif = c.String(nullable: false),
                        IdDevise = c.Int(nullable: false),
                        DossierId = c.Int(nullable: false),
                        IdClient = c.Int(),
                        CompteClient_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Justificatif", t => t.IdAncienneFacture)
                .ForeignKey("dbo.Dossier", t => t.DossierId, cascadeDelete: true)
                .ForeignKey("dbo.CompteClient", t => t.CompteClient_Id)
                .Index(t => t.IdAncienneFacture)
                .Index(t => t.DossierId)
                .Index(t => t.CompteClient_Id);
            
            CreateTable(
                "dbo.FacturePieces",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdFacture = c.Int(nullable: false),
                        IdFichier = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Justificatif", t => t.IdFacture, cascadeDelete: true)
                .Index(t => t.IdFacture);
            
            CreateTable(
                "dbo.NumCompteBeneficiaires",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Cle = c.String(),
                        Numero = c.String(maxLength: 11),
                        Nom = c.String(),
                        NomBanque = c.String(),
                        Adresse = c.String(),
                        CodeAgence = c.String(maxLength: 5),
                        Pays = c.String(),
                        Ville = c.String(),
                        IdFournisseur = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Fournisseurs", t => t.IdFournisseur, cascadeDelete: true)
                .Index(t => t.IdFournisseur);
            
            CreateTable(
                "dbo.DossierStructures",
                c => new
                    {
                        IdDossier = c.Int(nullable: false),
                        IdStructure = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        AttribuerPar = c.String(),
                        IdResponsable = c.String(),
                        NomResponsable = c.String(),
                        Dossier_Dossier_Id = c.Int(),
                        Structure_Id = c.Int(),
                    })
                .PrimaryKey(t => new { t.IdDossier, t.IdStructure })
                .ForeignKey("dbo.Dossier", t => t.Dossier_Dossier_Id)
                .ForeignKey("dbo.Structures", t => t.Structure_Id)
                .Index(t => t.Dossier_Dossier_Id)
                .Index(t => t.Structure_Id);
            
            CreateTable(
                "dbo.DossierNotification",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DossierId = c.String(),
                        Date = c.DateTime(nullable: false),
                        Desactive = c.Boolean(nullable: false),
                        Lue = c.Boolean(nullable: false),
                        Titre = c.String(),
                        Detail = c.String(),
                        Dossier_Dossier_Id = c.Int(),
                        Notification_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Dossier", t => t.Dossier_Dossier_Id)
                .ForeignKey("dbo.NotifDossier", t => t.Notification_Id)
                .Index(t => t.Dossier_Dossier_Id)
                .Index(t => t.Notification_Id);
            
            CreateTable(
                "dbo.IHMStructures",
                c => new
                    {
                        ComposantId = c.Int(nullable: false),
                        IdStructure = c.Int(nullable: false),
                        Lire = c.Boolean(nullable: false),
                        Ecrire = c.Boolean(nullable: false),
                        Supprimer = c.Boolean(nullable: false),
                        Créer = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.ComposantId, t.IdStructure })
                .ForeignKey("dbo.Composants", t => t.ComposantId, cascadeDelete: true)
                .ForeignKey("dbo.Structures", t => t.IdStructure, cascadeDelete: true)
                .Index(t => t.ComposantId)
                .Index(t => t.IdStructure);
            
            CreateTable(
                "dbo.TypeStructures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Intitule = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StatutDossier",
                c => new
                    {
                        IdStatut = c.Int(nullable: false),
                        IdDossier = c.Int(nullable: false),
                        Id = c.Int(nullable: false, identity: true),
                        Etat = c.Int(),
                        Date = c.DateTime(nullable: false),
                        IdStructure = c.Int(),
                        IdAgent = c.String(),
                        Objet = c.String(),
                        Objet2 = c.String(),
                        Message = c.String(),
                        Message2 = c.String(),
                        Motif = c.String(),
                        EstConformite = c.Boolean(nullable: false),
                        Statut1 = c.String(),
                    })
                .PrimaryKey(t => new { t.IdStatut, t.IdDossier, t.Id })
                .ForeignKey("dbo.Dossier", t => t.IdDossier, cascadeDelete: true)
                .ForeignKey("dbo.Statuts", t => t.IdStatut, cascadeDelete: true)
                .Index(t => t.IdStatut)
                .Index(t => t.IdDossier);
            
            CreateTable(
                "dbo.Statuts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Status1 = c.String(),
                        Message2 = c.String(),
                        CouleurSt = c.String(),
                        Etape = c.Int(),
                        Message = c.String(),
                        Argb = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BanqueTierClients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nom = c.String(),
                        Ville = c.String(),
                        Pays = c.String(),
                        Adresse = c.String(),
                        IdClient = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Client", t => t.IdClient, cascadeDelete: true)
                .Index(t => t.IdClient);
            
            CreateTable(
                "dbo.Contacts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NomComplet = c.String(),
                        Pays = c.String(),
                        Ville = c.String(),
                        Telephone = c.String(),
                        Telephone2 = c.String(),
                        Email = c.String(),
                        IdGestionnaire = c.String(maxLength: 128),
                        IdClient = c.Int(),
                        Groupe = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Client", t => t.IdClient)
                .ForeignKey("dbo.CompteBanqueCommerciale", t => t.IdGestionnaire)
                .Index(t => t.IdGestionnaire)
                .Index(t => t.IdClient);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.ClientUserRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nom = c.String(),
                        CreerDossier = c.Boolean(nullable: false),
                        SoumettreDossier = c.Boolean(nullable: false),
                        CreerUser = c.Boolean(nullable: false),
                        SuppUser = c.Boolean(nullable: false),
                        ModifUser = c.Boolean(nullable: false),
                        CreerBenef = c.Boolean(nullable: false),
                        SuppBenef = c.Boolean(nullable: false),
                        ModifBenef = c.Boolean(nullable: false),
                        Default = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.NumComptes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Numero = c.String(maxLength: 11),
                        CodeAgence = c.String(maxLength: 5),
                        Cle = c.String(maxLength: 2),
                        Nom = c.String(),
                        IdBanqueClient = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BanqueClient", t => t.IdBanqueClient, cascadeDelete: true)
                .Index(t => t.IdBanqueClient);
            
            CreateTable(
                "dbo.CompteXAFs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NumCompte = c.String(),
                        RIB = c.String(),
                        Libellé = c.String(),
                        BanqueId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Banque", t => t.BanqueId)
                .Index(t => t.BanqueId);
            
            CreateTable(
                "dbo.ePubs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Etat = c.Byte(nullable: false),
                        CardLeft = c.Boolean(nullable: false),
                        PostionCarteLeftV = c.Int(nullable: false),
                        PostionCarteLeftH = c.Int(),
                        NbrMaxAffCG = c.Int(),
                        NbrPosteAffCG = c.Int(),
                        WidhtCG = c.Int(),
                        HeigthCG = c.Int(),
                        DelaitAttCG = c.Int(),
                        CardRigth = c.Boolean(nullable: false),
                        PostionCarteRigthtV = c.Int(),
                        PostionCarteRigthtH = c.Int(),
                        NbrMaxAffCD = c.Int(),
                        NbrPosteAffCD = c.Int(),
                        WidhtCD = c.Int(),
                        HeigthCD = c.Int(),
                        DelaitAttCD = c.Int(),
                        CardTop = c.Boolean(nullable: false),
                        CardBottom = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PubItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nom = c.String(),
                        Titre = c.String(),
                        NoteBas = c.String(),
                        Description = c.String(),
                        Image = c.String(),
                        IsHtml = c.Boolean(nullable: false),
                        Acive = c.Boolean(nullable: false),
                        Left = c.Double(nullable: false),
                        Rigth = c.Double(nullable: false),
                        Top = c.Double(nullable: false),
                        Bottom = c.Double(nullable: false),
                        TitreColor = c.String(),
                        DescriptionColor = c.String(),
                        IdePub = c.Int(),
                        ePubItemType = c.Int(nullable: false),
                        eType = c.Byte(),
                        Theme = c.String(),
                        DateDebut = c.DateTime(),
                        HeureDebut = c.Time(precision: 7),
                        DateFin = c.DateTime(),
                        HeureFin = c.Time(precision: 7),
                        ChaqueHeure = c.Time(precision: 7),
                        DureeApp = c.Int(),
                        DuréeAtt = c.Int(),
                        Width = c.Int(),
                        Heigth = c.Int(),
                        FondColor = c.String(),
                        LienUrl = c.String(),
                        LienText = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ePubs", t => t.IdePub)
                .Index(t => t.IdePub);
            
            CreateTable(
                "dbo.UsersExternes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NomComplet = c.String(),
                        Telephone = c.String(),
                        Email = c.String(),
                        BanqueId = c.Int(nullable: false),
                        Rang = c.Int(),
                        Fonction = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Banque", t => t.BanqueId)
                .Index(t => t.BanqueId);
            
            CreateTable(
                "dbo.ImageChats",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Url = c.String(),
                        IdChat = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Chat", t => t.IdChat, cascadeDelete: true)
                .Index(t => t.IdChat);
            
            CreateTable(
                "dbo.GroupeComposants",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nom = c.String(),
                        Priorité = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GroupeDisposionAccueils",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClasseFille = c.String(),
                        BorderColor = c.String(),
                        BgColor = c.String(),
                        Card = c.String(),
                        Titre = c.String(),
                        BG_Header = c.String(),
                        Info_pied = c.String(),
                        BG_Footer = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DossierSupprimes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Agence = c.String(),
                        Gestionnaire = c.String(),
                        DonneurDordre = c.String(),
                        Benefic = c.String(),
                        Devise = c.String(),
                        MontantDev = c.Double(nullable: false),
                        MontantXaf = c.Double(nullable: false),
                        DateTraitement = c.String(),
                        DateDepot = c.String(),
                        DateSupp = c.String(),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FileItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        MimeType = c.String(),
                        Path = c.String(nullable: false),
                        IsFolder = c.Boolean(nullable: false),
                        CDate = c.DateTime(nullable: false),
                        MDate = c.DateTime(nullable: false),
                        FileId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FileItems", t => t.FileId)
                .Index(t => t.FileId);
            
            CreateTable(
                "dbo.DocumentAttendus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Intitulé = c.Int(nullable: false),
                        ReferenceBanqueId = c.String(),
                        Reference_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ReferenceBanque", t => t.Reference_Id)
                .Index(t => t.Reference_Id);
            
            CreateTable(
                "dbo.FormatRefInderne",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CodeFormat = c.String(),
                        CodeFormatTaile = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Historisations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateDebut = c.DateTime(),
                        DateFin = c.DateTime(),
                        TypeHistorique = c.Short(nullable: false),
                        Client = c.String(),
                        Agent = c.String(),
                        Structure = c.String(),
                        Dossier = c.String(),
                        IdDossier = c.Int(),
                        IdClient = c.Int(),
                        IdStructure = c.Int(),
                        IdAgant = c.String(),
                        Cible = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Pays",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Indicatif = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Ville",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nom = c.String(),
                        PaysId = c.String(),
                        Pays_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pays", t => t.Pays_Id)
                .Index(t => t.Pays_Id);
            
            CreateTable(
                "dbo.Session",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StatutReference",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EtatDossier = c.Int(nullable: false),
                        Message = c.String(),
                        Titre = c.String(),
                        Image = c.Binary(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Themes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nom = c.String(),
                        HasImage = c.Boolean(nullable: false),
                        HasFooter = c.Boolean(nullable: false),
                        HasLink = c.Boolean(nullable: false),
                        LinkPosition = c.Int(nullable: false),
                        ImagePosition = c.Int(nullable: false),
                        Html = c.String(),
                        Libelle = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.VariableStatuts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Variable = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MotifsTransferts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Intitule = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.Banque",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        GetEPub_Id = c.Int(),
                        ConfigPersonnel = c.Boolean(nullable: false),
                        Epub = c.Boolean(nullable: false),
                        CodeSwift = c.String(maxLength: 5),
                        FileCourierIns = c.Int(nullable: false),
                        Image = c.String(),
                        MontantDFX = c.Double(nullable: false),
                        Activetimer = c.Boolean(nullable: false),
                        Interval = c.Int(),
                        HeureExecuteDebut = c.Short(),
                        HeureExecuteFin = c.Short(),
                        TempsPassageArchivage = c.Int(),
                        DureeArchivage = c.Int(),
                        DureeRappelMiseEnDemaure = c.Int(),
                        JourRecptionRecapGes = c.Int(nullable: false),
                        RelanceDossierEchu = c.Int(),
                        DateCreation = c.DateTime(),
                        StopDataSynchrone = c.Boolean(nullable: false),
                        DateExecuteFin = c.DateTime(),
                        DateExecuteDebut = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Structures", t => t.Id)
                .ForeignKey("dbo.ePubs", t => t.GetEPub_Id)
                .Index(t => t.Id)
                .Index(t => t.GetEPub_Id);
            
            CreateTable(
                "dbo.CompteBanqueCommerciale",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Structure_Id = c.Int(),
                        Categorie = c.Byte(nullable: false),
                        IdXRole = c.Int(),
                        IdStructure = c.Int(),
                        EstBackOff = c.Boolean(nullable: false),
                        EstGestionnaire = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Id)
                .ForeignKey("dbo.Structures", t => t.Structure_Id)
                .ForeignKey("dbo.XtraRoles", t => t.IdXRole)
                .ForeignKey("dbo.Structures", t => t.IdStructure)
                .Index(t => t.Id)
                .Index(t => t.Structure_Id)
                .Index(t => t.IdXRole)
                .Index(t => t.IdStructure);
            
            CreateTable(
                "dbo.CompteClient",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ClientId = c.Int(nullable: false),
                        CreerDossier = c.Boolean(nullable: false),
                        SoumettreDossier = c.Boolean(nullable: false),
                        CreerUser = c.Boolean(nullable: false),
                        SuppUser = c.Boolean(nullable: false),
                        ModifUser = c.Boolean(nullable: false),
                        CreerBenef = c.Boolean(nullable: false),
                        SuppBenef = c.Boolean(nullable: false),
                        ModifBenef = c.Boolean(nullable: false),
                        IdUserRole = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Id)
                .ForeignKey("dbo.Client", t => t.ClientId, cascadeDelete: true)
                .ForeignKey("dbo.ClientUserRoles", t => t.IdUserRole)
                .Index(t => t.Id)
                .Index(t => t.ClientId)
                .Index(t => t.IdUserRole);
            
            CreateTable(
                "dbo.DocumentAttacheDossier",
                c => new
                    {
                        Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClFichier", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.Mail",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        AdresseEmeteur = c.String(),
                        CC = c.String(),
                        Envoyé = c.Boolean(nullable: false),
                        Corbeille = c.Boolean(nullable: false),
                        AdresseDest = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Notifications", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.NotifDossier",
                c => new
                    {
                        Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Notifications", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.NotifReference",
                c => new
                    {
                        Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Notifications", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.PieceJointe",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Justificatif_Id = c.Int(),
                        JustificatifId = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClFichier", t => t.Id)
                .ForeignKey("dbo.Justificatif", t => t.Justificatif_Id)
                .Index(t => t.Id)
                .Index(t => t.Justificatif_Id);
            
            CreateTable(
                "dbo.ReferenceBanque",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Client_Id = c.Int(),
                        DepotBEAC = c.DateTime(),
                        DélaitT = c.Int(nullable: false),
                        DateNotif = c.DateTime(),
                        MT99 = c.String(),
                        DateMT99 = c.DateTime(),
                        DateMT202 = c.DateTime(),
                        DateCredit = c.DateTime(),
                        DateTraité = c.DateTime(),
                        ObsTransmission = c.String(),
                        ObsBEAC = c.String(),
                        NATURE = c.String(),
                        CvEURO = c.Double(nullable: false),
                        Apuré = c.Boolean(nullable: false),
                        Accordé = c.Boolean(nullable: false),
                        Echus = c.Boolean(nullable: false),
                        EnvoieBEAC = c.Boolean(nullable: false),
                        NbrTelechargement = c.Int(nullable: false),
                        Message = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.References", t => t.Id)
                .ForeignKey("dbo.Client", t => t.Client_Id)
                .Index(t => t.Id)
                .Index(t => t.Client_Id);
            
            CreateTable(
                "dbo.CompteAdmin",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.RoleClient",
                c => new
                    {
                        RoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RoleId)
                .ForeignKey("dbo.XtraRoles", t => t.RoleId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.RoleCommercialeBanque",
                c => new
                    {
                        RoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RoleId)
                .ForeignKey("dbo.XtraRoles", t => t.RoleId)
                .Index(t => t.RoleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RoleCommercialeBanque", "RoleId", "dbo.XtraRoles");
            DropForeignKey("dbo.RoleClient", "RoleId", "dbo.XtraRoles");
            DropForeignKey("dbo.CompteAdmin", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ReferenceBanque", "Client_Id", "dbo.Client");
            DropForeignKey("dbo.ReferenceBanque", "Id", "dbo.References");
            DropForeignKey("dbo.PieceJointe", "Justificatif_Id", "dbo.Justificatif");
            DropForeignKey("dbo.PieceJointe", "Id", "dbo.ClFichier");
            DropForeignKey("dbo.NotifReference", "Id", "dbo.Notifications");
            DropForeignKey("dbo.NotifDossier", "Id", "dbo.Notifications");
            DropForeignKey("dbo.Mail", "Id", "dbo.Notifications");
            DropForeignKey("dbo.DocumentAttacheDossier", "Id", "dbo.ClFichier");
            DropForeignKey("dbo.CompteClient", "IdUserRole", "dbo.ClientUserRoles");
            DropForeignKey("dbo.CompteClient", "ClientId", "dbo.Client");
            DropForeignKey("dbo.CompteClient", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.CompteBanqueCommerciale", "IdStructure", "dbo.Structures");
            DropForeignKey("dbo.CompteBanqueCommerciale", "IdXRole", "dbo.XtraRoles");
            DropForeignKey("dbo.CompteBanqueCommerciale", "Structure_Id", "dbo.Structures");
            DropForeignKey("dbo.CompteBanqueCommerciale", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Banque", "GetEPub_Id", "dbo.ePubs");
            DropForeignKey("dbo.Banque", "Id", "dbo.Structures");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.References", "RecapTransfert_Id", "dbo.DocumentAttache");
            DropForeignKey("dbo.References", "MT_Id", "dbo.DocumentAttache");
            DropForeignKey("dbo.References", "MiseEnDemeure_Id", "dbo.DocumentAttache");
            DropForeignKey("dbo.Dossier", "Reference_Id", "dbo.References");
            DropForeignKey("dbo.References", "Courrier_Id", "dbo.DocumentAttache");
            DropForeignKey("dbo.References", "BanqueId", "dbo.Banque");
            DropForeignKey("dbo.Ville", "Pays_Id", "dbo.Pays");
            DropForeignKey("dbo.DocumentAttendus", "Reference_Id", "dbo.ReferenceBanque");
            DropForeignKey("dbo.FileItems", "FileId", "dbo.FileItems");
            DropForeignKey("dbo.Composants", "IdGroupeDisposionAccueil", "dbo.GroupeDisposionAccueils");
            DropForeignKey("dbo.Composants", "IdGroupe", "dbo.GroupeComposants");
            DropForeignKey("dbo.IHMs", "XRoleId", "dbo.XtraRoles");
            DropForeignKey("dbo.XtraRoles", "IdStructure", "dbo.Structures");
            DropForeignKey("dbo.Structures", "IdAgence", "dbo.Structures");
            DropForeignKey("dbo.Structures", "IdTypeStructure", "dbo.TypeStructures");
            DropForeignKey("dbo.Structures", "Structure_Id", "dbo.Structures");
            DropForeignKey("dbo.Structures", "IdResponsable", "dbo.CompteBanqueCommerciale");
            DropForeignKey("dbo.BanqueClient", "Structure_Id", "dbo.Structures");
            DropForeignKey("dbo.ImageChats", "IdChat", "dbo.Chat");
            DropForeignKey("dbo.Chat", "EmetteurId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "Chat_ChatId", "dbo.Chat");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Chat", "ApplicationUser_Id1", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "ImageProfile_Id", "dbo.UneImages");
            DropForeignKey("dbo.Notifications", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ReferenceNotification", "ReferenceBanque_Id", "dbo.ReferenceBanque");
            DropForeignKey("dbo.Structures", "IdBanque2", "dbo.Banque");
            DropForeignKey("dbo.UsersExternes", "BanqueId", "dbo.Banque");
            DropForeignKey("dbo.PubItems", "IdePub", "dbo.ePubs");
            DropForeignKey("dbo.CompteXAFs", "BanqueId", "dbo.Banque");
            DropForeignKey("dbo.Structures", "IdBanque1", "dbo.Banque");
            DropForeignKey("dbo.BanqueClient", "IdSite", "dbo.Structures");
            DropForeignKey("dbo.NumComptes", "IdBanqueClient", "dbo.BanqueClient");
            DropForeignKey("dbo.BanqueClient", "IdGestionnaire", "dbo.CompteBanqueCommerciale");
            DropForeignKey("dbo.BanqueClient", "ClientId", "dbo.Client");
            DropForeignKey("dbo.Justificatif", "CompteClient_Id", "dbo.CompteClient");
            DropForeignKey("dbo.Client", "Statut_Id", "dbo.DocumentAttache");
            DropForeignKey("dbo.Client", "RCCM_Cl_Id", "dbo.DocumentAttache");
            DropForeignKey("dbo.Client", "ProcesVerbal_Id", "dbo.DocumentAttache");
            DropForeignKey("dbo.Client", "PlanLSS_Id", "dbo.DocumentAttache");
            DropForeignKey("dbo.Client", "PlanLOcalisationDomi_Id", "dbo.DocumentAttache");
            DropForeignKey("dbo.Client", "JustifDomicile_Id", "dbo.DocumentAttache");
            DropForeignKey("dbo.Client", "FicheKYC_Id", "dbo.DocumentAttache");
            DropForeignKey("dbo.Client", "EtatFinanciers_Id", "dbo.DocumentAttache");
            DropForeignKey("dbo.DocumentAttache", "Client_Id1", "dbo.Client");
            DropForeignKey("dbo.Contacts", "IdGestionnaire", "dbo.CompteBanqueCommerciale");
            DropForeignKey("dbo.Contacts", "IdClient", "dbo.Client");
            DropForeignKey("dbo.Client", "CarteIdentie_Id", "dbo.DocumentAttache");
            DropForeignKey("dbo.BanqueTierClients", "IdClient", "dbo.Client");
            DropForeignKey("dbo.DocumentAttache", "BanqueTierClient_Id", "dbo.BanqueTierClients");
            DropForeignKey("dbo.DocumentAttache", "Client_Id", "dbo.Client");
            DropForeignKey("dbo.Client", "AtestationHinneur_Id", "dbo.DocumentAttache");
            DropForeignKey("dbo.DocumentAttache", "FichierImage_Id", "dbo.ClFichier");
            DropForeignKey("dbo.DocumentAttache", "DossierId", "dbo.Dossier");
            DropForeignKey("dbo.StatutDossier", "IdStatut", "dbo.Statuts");
            DropForeignKey("dbo.StatutDossier", "IdDossier", "dbo.Dossier");
            DropForeignKey("dbo.Dossier", "IdSite", "dbo.Structures");
            DropForeignKey("dbo.Structures", "IdDirectionMetier", "dbo.Structures");
            DropForeignKey("dbo.Structures", "TypeStructure_Id", "dbo.TypeStructures");
            DropForeignKey("dbo.Structures", "IdBanque", "dbo.Banque");
            DropForeignKey("dbo.IHMStructures", "IdStructure", "dbo.Structures");
            DropForeignKey("dbo.IHMStructures", "ComposantId", "dbo.Composants");
            DropForeignKey("dbo.Dossier", "ReferenceExterneId", "dbo.ReferenceBanque");
            DropForeignKey("dbo.Dossier", "QuittancePay_Id", "dbo.DocumentAttache");
            DropForeignKey("dbo.DossierNotification", "Notification_Id", "dbo.NotifDossier");
            DropForeignKey("dbo.DossierNotification", "Dossier_Dossier_Id", "dbo.Dossier");
            DropForeignKey("dbo.Dossier", "MT_Id", "dbo.DocumentAttache");
            DropForeignKey("dbo.Dossier", "LettreEngage_Id", "dbo.DocumentAttache");
            DropForeignKey("dbo.DossierStructures", "Structure_Id", "dbo.Structures");
            DropForeignKey("dbo.DossierStructures", "Dossier_Dossier_Id", "dbo.Dossier");
            DropForeignKey("dbo.UneImages", "DossierId", "dbo.Dossier");
            DropForeignKey("dbo.Dossier", "FournisseurId", "dbo.Fournisseurs");
            DropForeignKey("dbo.Fournisseurs", "RCCM_Id", "dbo.DocumentAttache");
            DropForeignKey("dbo.Fournisseurs", "ProcesVerbalNommantDirigeants_Id", "dbo.DocumentAttache");
            DropForeignKey("dbo.Fournisseurs", "PriseActeDeclarationCompte_Id", "dbo.DocumentAttache");
            DropForeignKey("dbo.Fournisseurs", "ListeGerants_Id", "dbo.DocumentAttache");
            DropForeignKey("dbo.Fournisseurs", "ListeAyantDroits_Id", "dbo.DocumentAttache");
            DropForeignKey("dbo.Fournisseurs", "JustifDomicileBenefi_Id", "dbo.DocumentAttache");
            DropForeignKey("dbo.Fournisseurs", "FicheKYCBenefi_Id", "dbo.DocumentAttache");
            DropForeignKey("dbo.Fournisseurs", "CopieStatuts_Id", "dbo.DocumentAttache");
            DropForeignKey("dbo.NumCompteBeneficiaires", "IdFournisseur", "dbo.Fournisseurs");
            DropForeignKey("dbo.Fournisseurs", "ClientId", "dbo.Client");
            DropForeignKey("dbo.Fournisseurs", "CarteIdentiteDirigeants_Id", "dbo.DocumentAttache");
            DropForeignKey("dbo.UneImages", "DocumentAttacheId", "dbo.DocumentAttache");
            DropForeignKey("dbo.UneImages", "JustificatifId", "dbo.Justificatif");
            DropForeignKey("dbo.FacturePieces", "IdFacture", "dbo.Justificatif");
            DropForeignKey("dbo.Justificatif", "DossierId", "dbo.Dossier");
            DropForeignKey("dbo.Justificatif", "IdAncienneFacture", "dbo.Justificatif");
            DropForeignKey("dbo.ClFichier", "Client_Id", "dbo.Client");
            DropForeignKey("dbo.DocumentAttache", "ClientId", "dbo.Client");
            DropForeignKey("dbo.DocumentAttache", "Fournisseurs_Id", "dbo.Fournisseurs");
            DropForeignKey("dbo.Dossier", "EnDemeure2_Id", "dbo.DocumentAttache");
            DropForeignKey("dbo.Dossier", "EnDemeure_Id", "dbo.DocumentAttache");
            DropForeignKey("dbo.Dossier", "DomicilImport_Id", "dbo.DocumentAttache");
            DropForeignKey("dbo.Dossier", "DocumentTransport_Id", "dbo.DocumentAttache");
            DropForeignKey("dbo.DocumentAttache", "Dossier_Dossier_Id1", "dbo.Dossier");
            DropForeignKey("dbo.DocumentAttache", "Dossier_Dossier_Id", "dbo.Dossier");
            DropForeignKey("dbo.Dossier", "DfxId", "dbo.References");
            DropForeignKey("dbo.Dossier", "DeviseMonetaireId", "dbo.DeviseMonetaire");
            DropForeignKey("dbo.CompteNostroes", "IdDevise", "dbo.DeviseMonetaire");
            DropForeignKey("dbo.CompteNostroes", "IdCorrespondant", "dbo.Correspondants");
            DropForeignKey("dbo.Correspondants", "BanqueId", "dbo.Banque");
            DropForeignKey("dbo.Dossier", "DeclarImport_Id", "dbo.DocumentAttache");
            DropForeignKey("dbo.Dossier", "Courrier_Id", "dbo.DocumentAttache");
            DropForeignKey("dbo.Dossier", "ClientId", "dbo.Client");
            DropForeignKey("dbo.Dossier", "IdResponsableTransfert", "dbo.CompteBanqueCommerciale");
            DropForeignKey("dbo.Dossier", "IdAgentResponsableDossier", "dbo.CompteBanqueCommerciale");
            DropForeignKey("dbo.Dossier", "IdResponsableConformite", "dbo.CompteBanqueCommerciale");
            DropForeignKey("dbo.Dossier", "IdResponsableBackOffice", "dbo.CompteBanqueCommerciale");
            DropForeignKey("dbo.Dossier", "IdResponsableAgence", "dbo.CompteBanqueCommerciale");
            DropForeignKey("dbo.Adresse", "ClientId", "dbo.Client");
            DropForeignKey("dbo.ReferenceNotification", "Notification_Id", "dbo.NotifReference");
            DropForeignKey("dbo.MailSupprimes", "MailId", "dbo.Mail");
            DropForeignKey("dbo.FichierMails", "IdMail", "dbo.Mail");
            DropForeignKey("dbo.Notifications", "TypeNotification_Id", "dbo.TypeNotifications");
            DropForeignKey("dbo.UneImages", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Chat", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.MessageItems", "ChatId", "dbo.Chat");
            DropForeignKey("dbo.Entitee_Role", "IdXRole", "dbo.XtraRoles");
            DropForeignKey("dbo.Entitee_Role", "IdEntitee", "dbo.Entitees");
            DropForeignKey("dbo.IHMs", "ComposantId", "dbo.Composants");
            DropForeignKey("dbo.Composants", "IdAction", "dbo.ActionIHMs");
            DropIndex("dbo.RoleCommercialeBanque", new[] { "RoleId" });
            DropIndex("dbo.RoleClient", new[] { "RoleId" });
            DropIndex("dbo.CompteAdmin", new[] { "Id" });
            DropIndex("dbo.ReferenceBanque", new[] { "Client_Id" });
            DropIndex("dbo.ReferenceBanque", new[] { "Id" });
            DropIndex("dbo.PieceJointe", new[] { "Justificatif_Id" });
            DropIndex("dbo.PieceJointe", new[] { "Id" });
            DropIndex("dbo.NotifReference", new[] { "Id" });
            DropIndex("dbo.NotifDossier", new[] { "Id" });
            DropIndex("dbo.Mail", new[] { "Id" });
            DropIndex("dbo.DocumentAttacheDossier", new[] { "Id" });
            DropIndex("dbo.CompteClient", new[] { "IdUserRole" });
            DropIndex("dbo.CompteClient", new[] { "ClientId" });
            DropIndex("dbo.CompteClient", new[] { "Id" });
            DropIndex("dbo.CompteBanqueCommerciale", new[] { "IdStructure" });
            DropIndex("dbo.CompteBanqueCommerciale", new[] { "IdXRole" });
            DropIndex("dbo.CompteBanqueCommerciale", new[] { "Structure_Id" });
            DropIndex("dbo.CompteBanqueCommerciale", new[] { "Id" });
            DropIndex("dbo.Banque", new[] { "GetEPub_Id" });
            DropIndex("dbo.Banque", new[] { "Id" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Ville", new[] { "Pays_Id" });
            DropIndex("dbo.DocumentAttendus", new[] { "Reference_Id" });
            DropIndex("dbo.FileItems", new[] { "FileId" });
            DropIndex("dbo.ImageChats", new[] { "IdChat" });
            DropIndex("dbo.UsersExternes", new[] { "BanqueId" });
            DropIndex("dbo.PubItems", new[] { "IdePub" });
            DropIndex("dbo.CompteXAFs", new[] { "BanqueId" });
            DropIndex("dbo.NumComptes", new[] { "IdBanqueClient" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.Contacts", new[] { "IdClient" });
            DropIndex("dbo.Contacts", new[] { "IdGestionnaire" });
            DropIndex("dbo.BanqueTierClients", new[] { "IdClient" });
            DropIndex("dbo.StatutDossier", new[] { "IdDossier" });
            DropIndex("dbo.StatutDossier", new[] { "IdStatut" });
            DropIndex("dbo.IHMStructures", new[] { "IdStructure" });
            DropIndex("dbo.IHMStructures", new[] { "ComposantId" });
            DropIndex("dbo.DossierNotification", new[] { "Notification_Id" });
            DropIndex("dbo.DossierNotification", new[] { "Dossier_Dossier_Id" });
            DropIndex("dbo.DossierStructures", new[] { "Structure_Id" });
            DropIndex("dbo.DossierStructures", new[] { "Dossier_Dossier_Id" });
            DropIndex("dbo.NumCompteBeneficiaires", new[] { "IdFournisseur" });
            DropIndex("dbo.FacturePieces", new[] { "IdFacture" });
            DropIndex("dbo.Justificatif", new[] { "CompteClient_Id" });
            DropIndex("dbo.Justificatif", new[] { "DossierId" });
            DropIndex("dbo.Justificatif", new[] { "IdAncienneFacture" });
            DropIndex("dbo.ClFichier", new[] { "Client_Id" });
            DropIndex("dbo.Fournisseurs", new[] { "RCCM_Id" });
            DropIndex("dbo.Fournisseurs", new[] { "ProcesVerbalNommantDirigeants_Id" });
            DropIndex("dbo.Fournisseurs", new[] { "PriseActeDeclarationCompte_Id" });
            DropIndex("dbo.Fournisseurs", new[] { "ListeGerants_Id" });
            DropIndex("dbo.Fournisseurs", new[] { "ListeAyantDroits_Id" });
            DropIndex("dbo.Fournisseurs", new[] { "JustifDomicileBenefi_Id" });
            DropIndex("dbo.Fournisseurs", new[] { "FicheKYCBenefi_Id" });
            DropIndex("dbo.Fournisseurs", new[] { "CopieStatuts_Id" });
            DropIndex("dbo.Fournisseurs", new[] { "CarteIdentiteDirigeants_Id" });
            DropIndex("dbo.Fournisseurs", new[] { "ClientId" });
            DropIndex("dbo.Correspondants", new[] { "BanqueId" });
            DropIndex("dbo.CompteNostroes", new[] { "IdCorrespondant" });
            DropIndex("dbo.CompteNostroes", new[] { "IdDevise" });
            DropIndex("dbo.Dossier", new[] { "Reference_Id" });
            DropIndex("dbo.Dossier", new[] { "QuittancePay_Id" });
            DropIndex("dbo.Dossier", new[] { "MT_Id" });
            DropIndex("dbo.Dossier", new[] { "LettreEngage_Id" });
            DropIndex("dbo.Dossier", new[] { "EnDemeure2_Id" });
            DropIndex("dbo.Dossier", new[] { "EnDemeure_Id" });
            DropIndex("dbo.Dossier", new[] { "DomicilImport_Id" });
            DropIndex("dbo.Dossier", new[] { "DocumentTransport_Id" });
            DropIndex("dbo.Dossier", new[] { "DeclarImport_Id" });
            DropIndex("dbo.Dossier", new[] { "Courrier_Id" });
            DropIndex("dbo.Dossier", new[] { "ClientId" });
            DropIndex("dbo.Dossier", new[] { "DfxId" });
            DropIndex("dbo.Dossier", new[] { "ReferenceExterneId" });
            DropIndex("dbo.Dossier", new[] { "DeviseMonetaireId" });
            DropIndex("dbo.Dossier", new[] { "FournisseurId" });
            DropIndex("dbo.Dossier", new[] { "IdSite" });
            DropIndex("dbo.Dossier", new[] { "IdResponsableTransfert" });
            DropIndex("dbo.Dossier", new[] { "IdResponsableBackOffice" });
            DropIndex("dbo.Dossier", new[] { "IdResponsableConformite" });
            DropIndex("dbo.Dossier", new[] { "IdResponsableAgence" });
            DropIndex("dbo.Dossier", new[] { "IdAgentResponsableDossier" });
            DropIndex("dbo.DocumentAttache", new[] { "Client_Id1" });
            DropIndex("dbo.DocumentAttache", new[] { "BanqueTierClient_Id" });
            DropIndex("dbo.DocumentAttache", new[] { "Client_Id" });
            DropIndex("dbo.DocumentAttache", new[] { "FichierImage_Id" });
            DropIndex("dbo.DocumentAttache", new[] { "Fournisseurs_Id" });
            DropIndex("dbo.DocumentAttache", new[] { "Dossier_Dossier_Id1" });
            DropIndex("dbo.DocumentAttache", new[] { "Dossier_Dossier_Id" });
            DropIndex("dbo.DocumentAttache", new[] { "ClientId" });
            DropIndex("dbo.DocumentAttache", new[] { "DossierId" });
            DropIndex("dbo.Adresse", new[] { "ClientId" });
            DropIndex("dbo.Client", new[] { "Statut_Id" });
            DropIndex("dbo.Client", new[] { "RCCM_Cl_Id" });
            DropIndex("dbo.Client", new[] { "ProcesVerbal_Id" });
            DropIndex("dbo.Client", new[] { "PlanLSS_Id" });
            DropIndex("dbo.Client", new[] { "PlanLOcalisationDomi_Id" });
            DropIndex("dbo.Client", new[] { "JustifDomicile_Id" });
            DropIndex("dbo.Client", new[] { "FicheKYC_Id" });
            DropIndex("dbo.Client", new[] { "EtatFinanciers_Id" });
            DropIndex("dbo.Client", new[] { "CarteIdentie_Id" });
            DropIndex("dbo.Client", new[] { "AtestationHinneur_Id" });
            DropIndex("dbo.BanqueClient", new[] { "Structure_Id" });
            DropIndex("dbo.BanqueClient", new[] { "IdGestionnaire" });
            DropIndex("dbo.BanqueClient", new[] { "IdSite" });
            DropIndex("dbo.BanqueClient", new[] { "ClientId" });
            DropIndex("dbo.References", new[] { "RecapTransfert_Id" });
            DropIndex("dbo.References", new[] { "MT_Id" });
            DropIndex("dbo.References", new[] { "MiseEnDemeure_Id" });
            DropIndex("dbo.References", new[] { "Courrier_Id" });
            DropIndex("dbo.References", new[] { "BanqueId" });
            DropIndex("dbo.ReferenceNotification", new[] { "ReferenceBanque_Id" });
            DropIndex("dbo.ReferenceNotification", new[] { "Notification_Id" });
            DropIndex("dbo.MailSupprimes", new[] { "MailId" });
            DropIndex("dbo.FichierMails", new[] { "IdMail" });
            DropIndex("dbo.Notifications", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Notifications", new[] { "TypeNotification_Id" });
            DropIndex("dbo.UneImages", new[] { "DossierId" });
            DropIndex("dbo.UneImages", new[] { "DocumentAttacheId" });
            DropIndex("dbo.UneImages", new[] { "JustificatifId" });
            DropIndex("dbo.UneImages", new[] { "ApplicationUserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.MessageItems", new[] { "ChatId" });
            DropIndex("dbo.Chat", new[] { "ApplicationUser_Id1" });
            DropIndex("dbo.Chat", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Chat", new[] { "EmetteurId" });
            DropIndex("dbo.AspNetUsers", new[] { "Chat_ChatId" });
            DropIndex("dbo.AspNetUsers", new[] { "ImageProfile_Id" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Structures", new[] { "Structure_Id" });
            DropIndex("dbo.Structures", new[] { "TypeStructure_Id" });
            DropIndex("dbo.Structures", new[] { "IdAgence" });
            DropIndex("dbo.Structures", new[] { "IdBanque2" });
            DropIndex("dbo.Structures", new[] { "IdBanque1" });
            DropIndex("dbo.Structures", new[] { "IdBanque" });
            DropIndex("dbo.Structures", new[] { "IdDirectionMetier" });
            DropIndex("dbo.Structures", new[] { "IdResponsable" });
            DropIndex("dbo.Structures", new[] { "IdTypeStructure" });
            DropIndex("dbo.Entitee_Role", new[] { "IdEntitee" });
            DropIndex("dbo.Entitee_Role", new[] { "IdXRole" });
            DropIndex("dbo.XtraRoles", new[] { "IdStructure" });
            DropIndex("dbo.IHMs", new[] { "ComposantId" });
            DropIndex("dbo.IHMs", new[] { "XRoleId" });
            DropIndex("dbo.Composants", new[] { "IdGroupeDisposionAccueil" });
            DropIndex("dbo.Composants", new[] { "IdAction" });
            DropIndex("dbo.Composants", new[] { "IdGroupe" });
            DropTable("dbo.RoleCommercialeBanque");
            DropTable("dbo.RoleClient");
            DropTable("dbo.CompteAdmin");
            DropTable("dbo.ReferenceBanque");
            DropTable("dbo.PieceJointe");
            DropTable("dbo.NotifReference");
            DropTable("dbo.NotifDossier");
            DropTable("dbo.Mail");
            DropTable("dbo.DocumentAttacheDossier");
            DropTable("dbo.CompteClient");
            DropTable("dbo.CompteBanqueCommerciale");
            DropTable("dbo.Banque");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.MotifsTransferts");
            DropTable("dbo.VariableStatuts");
            DropTable("dbo.Themes");
            DropTable("dbo.StatutReference");
            DropTable("dbo.Session");
            DropTable("dbo.Ville");
            DropTable("dbo.Pays");
            DropTable("dbo.Historisations");
            DropTable("dbo.FormatRefInderne");
            DropTable("dbo.DocumentAttendus");
            DropTable("dbo.FileItems");
            DropTable("dbo.DossierSupprimes");
            DropTable("dbo.GroupeDisposionAccueils");
            DropTable("dbo.GroupeComposants");
            DropTable("dbo.ImageChats");
            DropTable("dbo.UsersExternes");
            DropTable("dbo.PubItems");
            DropTable("dbo.ePubs");
            DropTable("dbo.CompteXAFs");
            DropTable("dbo.NumComptes");
            DropTable("dbo.ClientUserRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.Contacts");
            DropTable("dbo.BanqueTierClients");
            DropTable("dbo.Statuts");
            DropTable("dbo.StatutDossier");
            DropTable("dbo.TypeStructures");
            DropTable("dbo.IHMStructures");
            DropTable("dbo.DossierNotification");
            DropTable("dbo.DossierStructures");
            DropTable("dbo.NumCompteBeneficiaires");
            DropTable("dbo.FacturePieces");
            DropTable("dbo.Justificatif");
            DropTable("dbo.ClFichier");
            DropTable("dbo.Fournisseurs");
            DropTable("dbo.Correspondants");
            DropTable("dbo.CompteNostroes");
            DropTable("dbo.DeviseMonetaire");
            DropTable("dbo.Dossier");
            DropTable("dbo.DocumentAttache");
            DropTable("dbo.Adresse");
            DropTable("dbo.Client");
            DropTable("dbo.BanqueClient");
            DropTable("dbo.References");
            DropTable("dbo.ReferenceNotification");
            DropTable("dbo.MailSupprimes");
            DropTable("dbo.FichierMails");
            DropTable("dbo.TypeNotifications");
            DropTable("dbo.Notifications");
            DropTable("dbo.UneImages");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.MessageItems");
            DropTable("dbo.Chat");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Structures");
            DropTable("dbo.Entitees");
            DropTable("dbo.Entitee_Role");
            DropTable("dbo.XtraRoles");
            DropTable("dbo.IHMs");
            DropTable("dbo.Composants");
            DropTable("dbo.ActionIHMs");
        }
    }
}
