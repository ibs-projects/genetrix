using System;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using genetrix.Models;
using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.UI.WebControls;

namespace genetrix
{
    public partial class Startup
    {
        // Pour plus d'informations sur la configuration de l'authentification, visitez https://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            //SignalR
            app.MapSignalR();
            //Initialisation des données personnalisées
            DonnéesInitiales();

            //roles
            createRolesandUsers();

            // Configurer le contexte de base de données, le gestionnaire des utilisateurs et le gestionnaire des connexions pour utiliser une instance unique par demande
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Autoriser l’application à utiliser un cookie pour stocker des informations pour l’utilisateur connecté
            // et pour utiliser un cookie à des fins de stockage temporaire des informations sur la connexion utilisateur avec un fournisseur de connexion tiers
            // Configurer le cookie de connexion
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Permet à l'application de valider le timbre de sécurité quand l'utilisateur se connecte.
                    // Cette fonction de sécurité est utilisée quand vous changez un mot de passe ou ajoutez une connexion externe à votre compte.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Permet à l'application de stocker temporairement les informations utilisateur lors de la vérification du second facteur dans le processus d'authentification à 2 facteurs.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Permet à l'application de mémoriser le second facteur de vérification de la connexion, un numéro de téléphone ou un e-mail par exemple.
            // Lorsque vous activez cette option, votre seconde étape de vérification pendant le processus de connexion est mémorisée sur le poste à partir duquel vous vous êtes connecté.
            // Ceci est similaire à l'option RememberMe quand vous vous connectez.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Supprimer les commentaires des lignes suivantes pour autoriser la connexion avec des fournisseurs de connexions tiers
            app.UseMicrosoftAccountAuthentication(
                clientId: "464895986072-3n36bh547si2sj2srdvuckdn07kmuj25.apps.googleusercontent.com",
                clientSecret: "BZUF7Tea8TsxslQJgUEdGZi-");

            app.UseTwitterAuthentication(
               consumerKey: "464895986072-3n36bh547si2sj2srdvuckdn07kmuj25.apps.googleusercontent.com",
               consumerSecret: "BZUF7Tea8TsxslQJgUEdGZi-");

            app.UseFacebookAuthentication(
               appId: "464895986072-3n36bh547si2sj2srdvuckdn07kmuj25.apps.googleusercontent.com",
               appSecret: "BZUF7Tea8TsxslQJgUEdGZi-");

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "464895986072-3n36bh547si2sj2srdvuckdn07kmuj25.apps.googleusercontent.com",
                ClientSecret = "BZUF7Tea8TsxslQJgUEdGZi-"
            });
        }

        private void DonnéesInitiales()
        {
            VariablGlobales.UserInfos = new Dictionary<string, List<UserInfo>>();
            ApplicationDbContext db = new ApplicationDbContext();

            //Compte client par defaut
            try
            {
                if (db.ClientUserRoles.Count()==0)
                {
                    db.ClientUserRoles.Add(new ClientUserRole()
                    {
                        Nom="Administrateur",
                        CreerBenef=true,
                        CreerDossier=true,
                        CreerUser=true,
                        ModifBenef=true,
                        ModifUser=true,
                        SoumettreDossier=true,
                        SuppBenef=true,
                        SuppUser=true,
                        Default=true
                    });
                    
                    db.ClientUserRoles.Add(new ClientUserRole()
                    {
                        Nom="Opérant",
                        CreerDossier=true,
                        ModifBenef=true,
                        CreerBenef=true,
                        Default = true
                    });

                    db.ClientUserRoles.Add(new ClientUserRole()
                    {
                        Nom="Responsable",
                        ModifBenef=true,
                        SoumettreDossier=true,
                        Default = true
                    });
                }
            }
            catch (Exception)
            {}

            //Theme publication
            var themes = db.GetThemes;
            if (themes.Count()==0)
            {
                //Special title 1
                themes.Add(new Theme() {Nom = "CardBonus",Libelle="Card Bonus"});
                themes.Add(new Theme() {Nom = "CardImgBottom",Libelle="Card avec image en bas" });
                themes.Add(new Theme() {Nom = "CardImgLeft",Libelle="Card avec image à gauche" });
                themes.Add(new Theme() {Nom = "CardImgTop", Libelle = "Card avec image en haut" });
                themes.Add(new Theme() {Nom = "CardVideo", Libelle = "Card vidéo" });
            }
            themes = null;
            //statut dossier
            var status = db.GetStatuts.ToList();
            if (status.Count==0)
            {
                //null
                db.GetStatuts.Add(new Statut()
                {
                    CouleurSt = "cornflowerblue",
                    Status1 = "Brouillon",
                    Message2 = "",
                    Etape = null
                });
                var dos = new Dossier();
                for (int i = -3; i <=27; i++)
                {
                    try
                    {
                        db.GetStatuts.Add(new Statut()
                        {
                            CouleurSt = dos.GetEtapDossier(i)[1],
                            Status1 = dos.GetEtapDossier(i)[0],
                            Message2 = dos.GetEtapDossier(i)[3],
                            Message = dos.GetEtapDossier(i)[2],
                            Etape=i
                        });
                    }
                    catch (Exception)
                    {}
                }

                try
                {
                    db.GetStatuts.Add(new Statut()
                    {
                        CouleurSt = dos.GetEtapDossier(230)[1],
                        Status1 = dos.GetEtapDossier(230)[0],
                        Message2 = dos.GetEtapDossier(230)[3],
                        Message = dos.GetEtapDossier(230)[2],
                        Etape = 230
                    });
                    db.GetStatuts.Add(new Statut()
                    {
                        CouleurSt = dos.GetEtapDossier(-230)[1],
                        Status1 = dos.GetEtapDossier(-230)[0],
                        Message2 = dos.GetEtapDossier(-230)[3],
                        Message = dos.GetEtapDossier(-230)[2],
                        Etape = -230
                    });
                    db.GetStatuts.Add(new Statut()
                    {
                        CouleurSt = dos.GetEtapDossier(231)[1],
                        Status1 = dos.GetEtapDossier(231)[0],
                        Message2 = dos.GetEtapDossier(231)[3],
                        Message = dos.GetEtapDossier(231)[2],
                        Etape = 231
                    }); 
                    db.GetStatuts.Add(new Statut()
                    {
                        CouleurSt = dos.GetEtapDossier(-231)[1],
                        Status1 = dos.GetEtapDossier(-231)[0],
                        Message2 = dos.GetEtapDossier(-231)[3],
                        Message = dos.GetEtapDossier(-231)[2],
                        Etape = -231
                    }); 
                    db.GetStatuts.Add(new Statut()
                    {
                        CouleurSt = dos.GetEtapDossier(232)[1],
                        Status1 = dos.GetEtapDossier(232)[0],
                        Message2 = dos.GetEtapDossier(232)[3],
                        Message = dos.GetEtapDossier(232)[2],
                        Etape = 232
                    }); 
                    db.GetStatuts.Add(new Statut()
                    {
                        CouleurSt = dos.GetEtapDossier(-232)[1],
                        Status1 = dos.GetEtapDossier(-232)[0],
                        Message2 = dos.GetEtapDossier(-232)[3],
                        Message = dos.GetEtapDossier(-232)[2],
                        Etape = -232
                    });
                    db.GetStatuts.Add(new Statut()
                    {
                        CouleurSt = dos.GetEtapDossier(250)[1],
                        Status1 = dos.GetEtapDossier(250)[0],
                        Message2 = dos.GetEtapDossier(250)[3],
                        Message = dos.GetEtapDossier(250)[2],
                        Etape = 250
                    });
                    db.GetStatuts.Add(new Statut()
                    {
                        CouleurSt = dos.GetEtapDossier(-250)[1],
                        Status1 = dos.GetEtapDossier(-250)[0],
                        Message2 = dos.GetEtapDossier(-250)[3],
                        Message = dos.GetEtapDossier(-250)[2],
                        Etape = -250
                    });
                }
                catch (Exception)
                { }
            }

            //Actions
            var actions = db.Actions.ToList();
            if (actions.Count()==0)
            {
                db.Actions.Add(new ActionIHM()
                {
                   Url= "~/dossiers_banque/create",
                   Intitule="Nouveau dossier",
                   Recherche="nd",
                   TypeObjet=TypeObjet.dossier,
                   IconName= "newDoc.png",
                   Icon =VariablGlobales.ImageToBytes("newDoc.png")
                }); 
                db.Actions.Add(new ActionIHM()
                {
                   Url= "~/dossiers_banque/edit",
                   Intitule="Editer dossier",
                   Recherche="ed",
                    TypeObjet = TypeObjet.dossier,
                    IconName = "newDoc.png",
                    Icon =VariablGlobales.ImageToBytes("newDoc.png")
                });
                db.Actions.Add(new ActionIHM()
                {
                   Url= "~/dossiers_banque/details",
                   Intitule="Détails du dossier",
                    Recherche = "dd",
                    TypeObjet = TypeObjet.dossier,
                    IconName = "newDoc.png",
                    Icon =VariablGlobales.ImageToBytes("newDoc.png")
                });
                db.Actions.Add(new ActionIHM()
                {
                   Url= "~/dossiers_banque/delete",
                   Intitule="Supprimer le dossier",
                    Recherche = "sd",
                    TypeObjet = TypeObjet.dossier,
                    IconName = "newDoc.png",
                    Icon =VariablGlobales.ImageToBytes("newDoc.png")
                });
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/dossiers_banque/liste",
                    Intitule = "Liste de dossiers",
                    Recherche = "ltd",
                    TypeObjet = TypeObjet.dossier,
                    IconName = "allDoc.png",
                    Icon = VariablGlobales.ImageToBytes("allDoc.png")
                }); 
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/Dossiers_banque/Index?st=recu",
                    Intitule = "Dossiers reçus",
                    Recherche = "ldrec",
                    TypeObjet = TypeObjet.dossier,
                    IconName = "recus.png",
                    Icon = VariablGlobales.ImageToBytes("recus.png")
                });
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/Dossiers_banque/Index?st=encours",
                    Intitule = "Dossiers en cours de traitement agence",
                    Recherche = "ldenc",
                    TypeObjet = TypeObjet.dossier,
                    IconName = "encours2.png",
                    Icon = VariablGlobales.ImageToBytes("encours2.png")
                }); 
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/Dossiers_banque/Index?st=conform",
                    Intitule = "Dossiers en cours de vérification à la conformité",
                    Recherche = "ldconf",
                    TypeObjet = TypeObjet.dossier,
                    IconName = "encours2.png",
                    Icon = VariablGlobales.ImageToBytes("encours2.png")
                }); 
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/Dossiers_banque/Index?st=aapurer",
                    Intitule = "Dossiers à apurer",
                    Recherche = "ldaa",
                    TypeObjet = TypeObjet.dossier,
                    IconName = "aapurer.png",
                    Icon = VariablGlobales.ImageToBytes("aapurer.png")
                });
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/Dossiers_banque/Index?st=echu",
                    Intitule = "Dossiers échus",
                    Recherche = "ldech",
                    TypeObjet = TypeObjet.dossier,
                    IconName = "echus.png",
                    Icon = VariablGlobales.ImageToBytes("echus.png")
                }); 
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/Dossiers_banque/Index?st=encourstf",
                    Intitule = "Dossiers en cours de traitement service transfert",
                    Recherche = "ldecst",
                    TypeObjet = TypeObjet.dossier,
                    IconName = "encours1.png",
                    Icon = VariablGlobales.ImageToBytes("encours1.png")
                });
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/Dossiers_banque/Index?st=aenv_bac",
                    Intitule = "Dossiers à envoyer à la BEAC",
                    Recherche = "ldaenbeac",
                    TypeObjet = TypeObjet.dossier,
                    IconName = "aenvoyer.png",
                    Icon = VariablGlobales.ImageToBytes("aenvoyer.png")
                }); 
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/Dossiers_banque/Index?st=dfx",
                    Intitule = "DFX",
                    Recherche = "dfx",
                    TypeObjet = TypeObjet.dossier,
                    IconName = "dfx.png",
                    Icon = VariablGlobales.ImageToBytes("dfx.png")
                });
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/Dossiers_banque/Index?st=atten_couv",
                    Intitule = "Dossiers envoyés à la BEAC",
                    Recherche = "ldenbeac",
                    TypeObjet = TypeObjet.dossier,
                    IconName = "envoye.png",
                    Icon = VariablGlobales.ImageToBytes("envoye.png")
                });
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/Dossiers_banque/Index?st=saisie_encour",
                    Intitule = "Validés",
                    Recherche = "ldvalide",
                    TypeObjet = TypeObjet.dossier,
                    IconName = "saisie.png",
                    Icon = VariablGlobales.ImageToBytes("saisie.png")
                }); 
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/Dossiers_banque/Index?st=accord",
                    Intitule = "Dossiers accordés",
                    Recherche = "ldac",
                    TypeObjet = TypeObjet.dossier,
                    IconName = "accord.png",
                    Icon = VariablGlobales.ImageToBytes("accord.png")
                }); 
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/Dossiers_banque/Index?st=naccord",
                    Intitule = "Dossiers non accordés",
                    Recherche = "ldnac",
                    TypeObjet = TypeObjet.dossier,
                    IconName = "accord.png",
                    Icon = VariablGlobales.ImageToBytes("accord.png")
                }); 
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/Dossiers_banque/Index?st=port",
                    Intitule = "Mon portefeuille",
                    Recherche = "ldport",
                    TypeObjet = TypeObjet.dossier,
                    IconName = "portefeuille.png",
                    Icon = VariablGlobales.ImageToBytes("portefeuille.png")
                }); 
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/Dossiers_banque/DossiersGroupes",
                    Intitule = "Dossiers groupés",
                    Recherche = "ldgrp",
                    TypeObjet = TypeObjet.dossier,
                    IconName = "docGroupe.png",
                    Icon = VariablGlobales.ImageToBytes("docGroupe.png")
                });
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/Dossiers_banque/Index?st=apure",
                    Intitule = "Dossiers apurés",
                    Recherche = "ldap",
                    TypeObjet = TypeObjet.dossier,
                    IconName = "apure.png",
                    Icon = VariablGlobales.ImageToBytes("apure.png")
                });
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/Dossiers_banque/Index?st=supp",
                    Intitule = "Dossiers rappelés",
                    Recherche = "ldrap",
                    TypeObjet = TypeObjet.dossier,
                    IconName = "docRappele.png",
                    Icon = VariablGlobales.ImageToBytes("docRappele.png")
                });
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/Dossiers_banque/Index?st=tr",
                    Intitule = "Dossiers traités",
                    Recherche = "ldtr",
                    TypeObjet = TypeObjet.dossier,
                    IconName = "apure.png",
                    Icon = VariablGlobales.ImageToBytes("apure.png")
                }); 
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/Dossiers_banque/Index?st=untr",
                    Intitule = "Dossiers non traités",
                    Recherche = "ldntr",
                    TypeObjet = TypeObjet.dossier,
                    IconName = "echus1.png",
                    Icon = VariablGlobales.ImageToBytes("echus1.png")
                }); 
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/Dossiers_banque/Index?st=archive",
                    Intitule = "Dossiers archivés",
                    Recherche = "ldarc",
                    TypeObjet = TypeObjet.dossier,
                    IconName = "archive.png",
                    Icon = VariablGlobales.ImageToBytes("archive.png")
                });
                
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/structures/create",
                    Intitule = "Créer une structure",
                    Recherche = "nstr",
                    TypeObjet = TypeObjet.structure,
                    IconName = "structure.png",
                    Icon = VariablGlobales.ImageToBytes("structure.png")
                });
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/structures/edit",
                    Intitule = "Editer la structure",
                    Recherche = "edstr",
                    TypeObjet = TypeObjet.structure,
                    IconName = "structure.png",
                    Icon = VariablGlobales.ImageToBytes("structure.png")
                });
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/structures/details",
                    Intitule = "Details de la structure",
                    Recherche = "dstr",
                    TypeObjet = TypeObjet.structure,
                    IconName = "structure.png",
                    Icon = VariablGlobales.ImageToBytes("structure.png")
                });
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/structures/delete",
                    Intitule = "Supprimer la structure",
                    Recherche = "sstr",
                    TypeObjet = TypeObjet.structure,
                    IconName = "structure.png",
                    Icon = VariablGlobales.ImageToBytes("structure.png")
                });
                
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/agences/create",
                    Intitule = "Créer une agence",
                    Recherche = "nag",
                    TypeObjet = TypeObjet.agence,
                    IconName = "structure.png",
                    Icon = VariablGlobales.ImageToBytes("structure.png")
                });
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/agences/edit",
                    Intitule = "Editer l'agence",
                    Recherche = "edag",
                    TypeObjet = TypeObjet.agence,
                    IconName = "structure.png",
                    Icon = VariablGlobales.ImageToBytes("structure.png")
                });
                 db.Actions.Add(new ActionIHM()
                {
                    Url = "~/agences/details",
                    Intitule = "Details de l'agence",
                     Recherche = "dag",
                     TypeObjet = TypeObjet.agence,
                     IconName = "structure.png",
                     Icon = VariablGlobales.ImageToBytes("structure.png")
                });
                 db.Actions.Add(new ActionIHM()
                {
                    Url = "~/agences/delete",
                    Intitule = "Supprimer l'agence",
                     Recherche = "sag",
                     TypeObjet = TypeObjet.agence,
                     IconName = "structure.png",
                     Icon = VariablGlobales.ImageToBytes("structure.png")
                });
                
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/directionmetiers/create",
                    Intitule = "Créer une direction metier",
                    Recherche = "ndm",
                    TypeObjet = TypeObjet.directionMetier,
                    IconName = "structure.png",
                    Icon = VariablGlobales.ImageToBytes("structure.png")
                });
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/directionmetiers/edit",
                    Intitule = "Editer la direction metier",
                    Recherche = "eddm",
                    TypeObjet = TypeObjet.directionMetier,
                    IconName = "structure.png",
                    Icon = VariablGlobales.ImageToBytes("structure.png")
                });
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/directionmetiers/details",
                    Intitule = "Détails de la direction metier",
                    Recherche = "ddm",
                    TypeObjet = TypeObjet.directionMetier,
                    IconName = "structure.png",
                    Icon = VariablGlobales.ImageToBytes("structure.png")
                });
                 db.Actions.Add(new ActionIHM()
                {
                    Url = "~/directionmetiers/delete",
                    Intitule = "Supprimer la direction metier",
                     Recherche = "sdm",
                     TypeObjet = TypeObjet.directionMetier,
                     IconName = "structure.png",
                     Icon = VariablGlobales.ImageToBytes("structure.png")
                });
                
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/structures/index",
                    Intitule = "Structures",
                    Recherche = "lst",
                    TypeObjet = TypeObjet.structure,
                    IconName = "structures.png",
                    Icon = VariablGlobales.ImageToBytes("structures.png")
                });
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/agences/index",
                    Intitule = "Agences",
                    Recherche = "lag",
                    TypeObjet = TypeObjet.agence,
                    IconName = "structures.png",
                    Icon = VariablGlobales.ImageToBytes("structures.png")
                }); 
                
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/directionmetiers/index",
                    Intitule = "Directions metier",
                    Recherche = "ldm",
                    TypeObjet = TypeObjet.directionMetier,
                    IconName = "structures.png",
                    Icon = VariablGlobales.ImageToBytes("structures.png")
                }); 
                
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/clients/create",
                    Intitule = "Créer un client",
                    Recherche = "ncl",
                    TypeObjet = TypeObjet.client,
                    IconName = "client.png",
                    Icon = VariablGlobales.ImageToBytes("client.png")
                });
                
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/clients/editer",
                    Intitule = "Editer le client",
                    Recherche = "edcl",
                    TypeObjet = TypeObjet.client,
                    IconName = "client.png",
                    Icon = VariablGlobales.ImageToBytes("client.png")
                });
                
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/clients/details",
                    Intitule = "Détails du client",
                    Recherche = "dcl",
                    TypeObjet = TypeObjet.client,
                    IconName = "client.png",
                    Icon = VariablGlobales.ImageToBytes("client.png")
                });
                
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/clients/delete",
                    Intitule = "Supprimer le client",
                    Recherche = "scl",
                    TypeObjet = TypeObjet.client,
                    IconName = "client.png",
                    Icon = VariablGlobales.ImageToBytes("client.png")
                });
                
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/clients/index",
                    Intitule = "liste des clients",
                    Recherche = "lcl",
                    TypeObjet = TypeObjet.client,
                    IconName = "client.png",
                    Icon = VariablGlobales.ImageToBytes("client.png")
                }); 
                
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/referencebanques/create",
                    Intitule = "Créer une référence",
                    Recherche = "nref",
                    TypeObjet = TypeObjet.structure,
                    IconName = "ref.png",
                    Icon = VariablGlobales.ImageToBytes("ref.png")
                });
                
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/referencebanques/edit",
                    Intitule = "Editer la référence",
                    Recherche = "edref",
                    TypeObjet = TypeObjet.structure,
                    IconName = "ref.png",
                    Icon = VariablGlobales.ImageToBytes("ref.png")
                });
                
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/referencebanques/details",
                    Intitule = "Détails de la référence",
                    Recherche = "dref",
                    TypeObjet = TypeObjet.structure,
                    IconName = "ref.png",
                    Icon = VariablGlobales.ImageToBytes("ref.png")
                });
                
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/referencebanques/delete",
                    Intitule = "Supprimer la référence",
                    Recherche = "sref",
                    TypeObjet = TypeObjet.structure,
                    IconName = "ref.png",
                    Icon = VariablGlobales.ImageToBytes("ref.png")
                });
                
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/referencebanques/index?st=list",
                    Intitule = "Références banque",
                    Recherche = "lref",
                    TypeObjet = TypeObjet.structure,
                    IconName = "ref.png",
                    Icon = VariablGlobales.ImageToBytes("ref.png")
                });
                
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/referencebanques/index?st=lbre",
                    Intitule = "Liste de références non associées",
                    Recherche = "lrefnas",
                    TypeObjet = TypeObjet.structure,
                    IconName = "ref.png",
                    Icon = VariablGlobales.ImageToBytes("ref.png")
                });
                
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/referencebanques/index?st=liee",
                    Intitule = "Références ssociées",
                    Recherche = "lreas",
                    TypeObjet = TypeObjet.structure,
                    IconName = "ref.png",
                    Icon = VariablGlobales.ImageToBytes("ref.png")
                });
                
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/referencebanques/attache",
                    Intitule = "Attacher la réference",
                    Recherche = "aref",
                    TypeObjet = TypeObjet.structure,
                    IconName = "docAssocie.png",
                    Icon = VariablGlobales.ImageToBytes("docAssocie.png")
                });
                
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/devisemonetaires/create",
                    Intitule = "Ajouter une devise",
                    Recherche = "ndev",
                    TypeObjet = TypeObjet.structure,
                    IconName = "devise.png",
                    Icon = VariablGlobales.ImageToBytes("devise.png")
                });
                
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/devisemonetaires/edit",
                    Intitule = "Editer la devise",
                    Recherche = "eddev",
                    TypeObjet = TypeObjet.structure,
                    IconName = "devise.png",
                    Icon = VariablGlobales.ImageToBytes("devise.png")
                });
                
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/devisemonetaires/details",
                    Intitule = "Détails de la devise",
                    Recherche = "ddev",
                    TypeObjet = TypeObjet.structure,
                    IconName = "devise.png",
                    Icon = VariablGlobales.ImageToBytes("devise.png")
                });
                
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/devisemonetaires/delete",
                    Intitule = "Supprimer la devise",
                    Recherche = "sdev",
                    TypeObjet = TypeObjet.structure,
                    IconName = "devise.png",
                    Icon = VariablGlobales.ImageToBytes("devise.png")
                });

                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/devisemonetaires/index",
                    Intitule = "Liste de devises",
                    Recherche = "ldev",
                    TypeObjet = TypeObjet.structure,
                    IconName = "devise.png",
                    Icon = VariablGlobales.ImageToBytes("devise.png")
                });
                
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/banques/configurations",
                    Intitule = "Configuration de la banque",
                    Recherche = "cb",
                    TypeObjet = TypeObjet.structure,
                    IconName = "client.png",
                    Icon = VariablGlobales.ImageToBytes("client.png")
                });
                
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/dossiers_banque/etats",
                    Intitule = "Les états du dossier",
                    Recherche = "etd",
                    TypeObjet = TypeObjet.structure,
                    IconName = "docState.png",
                    Icon = VariablGlobales.ImageToBytes("docState.png")
                });
                
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/indexBanque/index",
                    Intitule = "Tableau de bord",
                    Recherche = "tb",
                    TypeObjet = TypeObjet.structure,
                    IconName = "home.png",
                    Icon = VariablGlobales.ImageToBytes("home.png")
                });
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/Mails/index",
                    Intitule = "Mails",
                    Recherche = "m",
                    TypeObjet = TypeObjet.notification,
                    IconName = "mail.png",
                    Icon = VariablGlobales.ImageToBytes("mail.png")
                });
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/Mails/create",
                    Intitule = "Nouveau mails",
                    Recherche = "nm",
                    TypeObjet = TypeObjet.notification,
                    IconName = "mail.png",
                    Icon = VariablGlobales.ImageToBytes("mail.png")
                });
                
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/chat/index",
                    Intitule = "Chat",
                    Recherche = "ch",
                    TypeObjet = TypeObjet.notification,
                    IconName = "chat.png",
                    Icon = VariablGlobales.ImageToBytes("chat.png")
                });
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/chat/create",
                    Intitule = "Nouveau message",
                    Recherche = "nch",
                    TypeObjet = TypeObjet.notification,
                    IconName = "chat.png",
                    Icon = VariablGlobales.ImageToBytes("chat.png")
                });
                db.Actions.Add(new ActionIHM()
                {
                    Url = "#",
                    Intitule = "Courriers",
                    Recherche = "courr",
                    TypeObjet = TypeObjet.notification,
                    IconName = "mail.png",
                    Icon = VariablGlobales.ImageToBytes("mail.png")
                });
                db.Actions.Add(new ActionIHM()
                {
                    Url = "~/dossiers_banque/index?st=conf_st",
                    Intitule = "Transmis",
                    Recherche = "dconf_st",
                    TypeObjet = TypeObjet.notification,
                    IconName = "encours.png",
                    Icon = VariablGlobales.ImageToBytes("encours.png")
                });

            }

            //GroupeComposant
            var groupeComposants = db.GroupeComposants;
            if (groupeComposants.Count()==0)
            {
                db.GroupeComposants.Add(new GroupeComposant()
                {
                    Nom = "Gestion de dossiers",
                    Priorité=0
                });
                db.GroupeComposants.Add(new GroupeComposant()
                {
                    Nom = "Gestion de clients",
                    Priorité=1
                });
                db.GroupeComposants.Add(new GroupeComposant()
                {
                    Nom = "Gestion d'utilisateurs",
                    Priorité=2
                });
                db.GroupeComposants.Add(new GroupeComposant()
                {
                    Nom = "Gestion d'agences",
                    Priorité=3
                }); 
                db.GroupeComposants.Add(new GroupeComposant()
                {
                    Nom = "Gestion de direction",
                    Priorité=4
                });
                db.GroupeComposants.Add(new GroupeComposant()
                {
                    Nom = "Gestion de la banque",
                    Priorité=5
                });
                db.GroupeComposants.Add(new GroupeComposant()
                {
                    Nom = "Notifications et messageries",
                    Priorité=6
                });
            }

            //TypeStructure
            var typeStructures = db.GetTypeStructures;
            if (typeStructures.Count()==0)
            {
                db.GetTypeStructures.Add(new TypeStructure()
                {
                    Intitule="Agence"
                });
                db.GetTypeStructures.Add(new TypeStructure()
                {
                    Intitule="Conformité"
                });
                db.GetTypeStructures.Add(new TypeStructure()
                {
                    Intitule="Service transfert"
                });
                db.GetTypeStructures.Add(new TypeStructure()
                {
                    Intitule= "DirectionMetier"
                });
                db.GetTypeStructures.Add(new TypeStructure()
                {
                    Intitule= "Banque"
                });
                db.GetTypeStructures.Add(new TypeStructure()
                {
                    Intitule= "Back-office"
                });
            }

            //Les XRôles
            var _roles = db.XtraRoles;
            if (_roles.Count()==0)
            {
                //Creation de tous les xroles (Banque)
                db.XtraRoles.AddRange(new List<XtraRole>()
                {
                    new XtraRole()
                    {
                        Nom="Admin",
                        EstAdmin=true
                    },
                    new XtraRole()
                    {
                        Nom="Gestionnaire"
                    },
                    new XtraRole()
                    {
                        Nom="Chef d'agence"
                    },
                    new XtraRole()
                    {
                        Nom="Back office"
                    },
                    new XtraRole()
                    {
                        Nom="Direction metier"
                    },
                    new XtraRole()
                    {
                        Nom="Conformite"
                    },
                     new XtraRole()
                    {
                        Nom="Service transfert"
                    }
                });
            }
            else
            {
                if (_roles.FirstOrDefault(r => r.Nom == "Admin") == null) db.XtraRoles.Add(new XtraRole()
                {
                    Nom = "Admin",
                    EstAdmin = true
                });
                if (_roles.FirstOrDefault(r => r.Nom == "Gestionnaire") == null) db.XtraRoles.Add(new XtraRole()
                {
                    Nom = "Gestionnaire"
                });
                if (_roles.FirstOrDefault(r => r.Nom == "Chef d'agence") == null) db.XtraRoles.Add(new XtraRole()
                {
                    Nom = "Chef d'agence"
                });
                if (_roles.FirstOrDefault(r => r.Nom == "Back office") == null) db.XtraRoles.Add(new XtraRole()
                {
                    Nom = "Back office"
                });
                if (_roles.FirstOrDefault(r => r.Nom == "Direction metier") == null) db.XtraRoles.Add(new XtraRole()
                {
                    Nom = "Direction metier"
                });
                if (_roles.FirstOrDefault(r => r.Nom == "Conformite") == null) db.XtraRoles.Add(new XtraRole()
                {
                    Nom = "Conformite"
                });
                if (_roles.FirstOrDefault(r => r.Nom == "Service transfert") == null) db.XtraRoles.Add(new XtraRole()
                {
                    Nom = "Service transfert"
                });
            }

            db.SaveChanges();

            //Les composants
            var _comp = db.GetComposants;
            if (_comp.Count() == 0)
            {
                //Creation de tous les composants (Banque)
                ///Accueil et navigation
                
                int Idgroupr = db.GroupeComposants.FirstOrDefault(g=>g.Priorité==0).Id;
                
                actions = db.Actions.ToList();

                //Composants dossiers 
                // dossiers reçus agence
                var action = actions.FirstOrDefault(a => a.Recherche == "ldrec");
                if(action!=null)
                db.GetComposants.Add(new Composant()
                {
                    EstActif = true,
                    IdAction = action.Id,
                    Recherche = "0" + action.Recherche,
                    Description = action.Intitule,
                    Localistion = Localistion.accueil,
                    IdGroupe = Idgroupr,
                    Info= " dossiers reçus agence",
                    Numero = 1,
                    NumeroMin = 1,
                    NumeroMax = 1,
                    Type = Models.Type.lien_bouton
                });
                // dossiers en cours de traitement agence
                action = actions.FirstOrDefault(a => a.Recherche == "ldenc");
                if(action!=null)
                db.GetComposants.Add(new Composant()
                {
                    EstActif = true,
                    IdAction = action.Id,
                    Recherche = "0" + action.Recherche,
                    Description = action.Intitule,
                    Localistion = Localistion.accueil,
                    IdGroupe = Idgroupr,
                    Info = " dossiers en cours de traitement agence",
                    Numero = 2,
                    NumeroMin = 2,
                    NumeroMax = 5,
                    Type = Models.Type.lien_bouton
                });

                // dossiers reçus à la conformité
                action = actions.FirstOrDefault(a => a.Recherche == "ldrec");
                if(action!=null)
                db.GetComposants.Add(new Composant()
                {
                    EstActif = true,
                    IdAction = action.Id,
                    Recherche = "0" + action.Recherche,
                    Description = action.Intitule,
                    Localistion = Localistion.accueil,
                    IdGroupe = Idgroupr,
                    Info = "dossiers reçus à la conformité",
                    Numero = 6,
                    NumeroMin = 6,
                    NumeroMax = 6,
                    Type = Models.Type.lien_bouton
                }); 
                 // dossiers en cours de traitement conformité
                action = actions.FirstOrDefault(a => a.Recherche == "ldconf");
                if(action!=null)
                db.GetComposants.Add(new Composant()
                {
                    EstActif = true,
                    IdAction = action.Id,
                    Recherche = "0" + action.Recherche,
                    Description = action.Intitule,
                    Localistion = Localistion.accueil,
                    IdGroupe = Idgroupr,
                    Info = "dossiers en cours de traitement conformité",
                    Numero = 7,
                    NumeroMin = 7,
                    NumeroMax = 8,
                    Type = Models.Type.lien_bouton
                }); 
                
                 // dossiers reçus service de transfert
                action = actions.FirstOrDefault(a => a.Recherche == "ldrec");
                if(action!=null)
                db.GetComposants.Add(new Composant()
                {
                    EstActif = true,
                    IdAction = action.Id,
                    Recherche = "0" + action.Recherche,
                    Description = action.Intitule,
                    Localistion = Localistion.accueil,
                    IdGroupe = Idgroupr,
                    Info = "dossiers reçus service transfert",
                    Numero = 9,
                    NumeroMin = 9,
                    NumeroMax = 9,
                    Type = Models.Type.lien_bouton
                });
                // dossiers en cours de traitement service de transfert
                action = actions.FirstOrDefault(a => a.Recherche == "ldecst");
                if(action!=null)
                db.GetComposants.Add(new Composant()
                {
                    EstActif = true,
                    IdAction = action.Id,
                    Recherche = "0" + action.Recherche,
                    Description = action.Intitule,
                    Localistion = Localistion.accueil,
                    IdGroupe = Idgroupr,
                    Info = "dossiers en cours de traitement",
                    Numero = 10,
                    NumeroMin = 10,
                    NumeroMax = 10,
                    Type = Models.Type.lien_bouton
                });
                //DFX
                action = actions.FirstOrDefault(a => a.Recherche == "dfx");
                if (action != null)
                    db.GetComposants.Add(new Composant()
                    {
                        EstActif = true,
                        IdAction = action.Id,
                        Recherche = "dfx",
                        Description = "DFX",
                        Localistion = Localistion.accueil,
                        IdGroupe = Idgroupr,
                        Numero = 10,
                        NumeroMin = 10,
                        NumeroMax = 22,
                        Type = Models.Type.lien_bouton
                    });
                //DFX
                action = actions.FirstOrDefault(a => a.Recherche == "dfx");
                if (action != null)
                    db.GetComposants.Add(new Composant()
                    {
                        EstActif = true,
                        IdAction = action.Id,
                        Recherche = "ref",
                        Description = "Refinancemnts",
                        Localistion = Localistion.accueil,
                        IdGroupe = Idgroupr,
                        Numero = 10,
                        NumeroMin = 10,
                        NumDiscontinue="-2",
                        NumeroMax = 22,
                        Type = Models.Type.lien_bouton
                    });

                // dossiers à envoyer à la BEAC
                action = actions.FirstOrDefault(a => a.Recherche == "ldaenbeac");
                if (action != null)
                {
                    db.GetComposants.Add(new Composant()
                    {
                        EstActif = true,
                        IdAction = action.Id,
                        Recherche = "0" + action.Recherche,
                        Description = action.Intitule,
                        Localistion = Localistion.accueil,
                        IdGroupe = Idgroupr,
                        Numero = 14,
                        NumeroMin = 14,
                        NumeroMax = 14,
                        Type = Models.Type.lien_bouton
                    });
                }
               
                // dossiers envoyés à la BEAC
                action = actions.FirstOrDefault(a => a.Recherche == "ldenbeac");
                if(action!=null)
                db.GetComposants.Add(new Composant()
                {
                    EstActif = true,
                    IdAction = action.Id,
                    Recherche = "0" + action.Recherche,
                    Description = action.Intitule,
                    Localistion = Localistion.accueil,
                    IdGroupe = Idgroupr,
                    Numero = 8,
                    NumeroMin = 8,
                    NumeroMax = 10,
                    Type = Models.Type.lien_bouton
                });
                // dossiers en cours de saisie
                action = actions.FirstOrDefault(a => a.Recherche == "ldvalide");
                if(action!=null)
                db.GetComposants.Add(new Composant()
                {
                    EstActif = true,
                    IdAction = action.Id,
                    Recherche = "0" + action.Recherche,
                    Description = action.Intitule,
                    Localistion = Localistion.accueil,
                    IdGroupe = Idgroupr,
                    Numero = 9,
                    NumeroMin = 9,
                    NumeroMax = 9,
                    Type = Models.Type.lien_bouton
                });
                // dossiers accordés
                action = actions.FirstOrDefault(a => a.Recherche == "ldac");
                if(action!=null)
                db.GetComposants.Add(new Composant()
                {
                    EstActif = true,
                    IdAction = action.Id,
                    Recherche = "0" + action.Recherche,
                    Description = action.Intitule,
                    Localistion = Localistion.accueil,
                    IdGroupe = Idgroupr,
                    Numero = 10,
                    NumeroMin = 10,
                    NumeroMax = 10,
                    Type = Models.Type.lien_bouton
                });
                
                 // dossiers à apurer
                action = actions.FirstOrDefault(a => a.Recherche == "ldaa");
                if(action!=null)
                db.GetComposants.Add(new Composant()
                {
                    EstActif = true,
                    IdAction = action.Id,
                    Recherche = "0" + action.Recherche,
                    Description = action.Intitule,
                    Localistion = Localistion.accueil,
                    IdGroupe = Idgroupr,
                    Numero = 11,
                    NumeroMin = 11,
                    NumeroMax = 11,
                    Type = Models.Type.lien_bouton
                });
                // dossiers apurés
                action = actions.FirstOrDefault(a => a.Recherche == "ldap");
                if(action!=null)
                db.GetComposants.Add(new Composant()
                {
                    EstActif = true,
                    IdAction = action.Id,
                    Recherche = "0" + action.Recherche,
                    Description = action.Intitule,
                    Localistion = Localistion.accueil,
                    IdGroupe = Idgroupr,
                    Numero = 12,
                    NumeroMin = 12,
                    NumeroMax = 12,
                    Type = Models.Type.lien_bouton
                });
                // dossiers echus
                action = actions.FirstOrDefault(a => a.Recherche == "ldech");
                if(action!=null)
                db.GetComposants.Add(new Composant()
                {
                    EstActif = true,
                    IdAction = action.Id,
                    Recherche = "0" + action.Recherche,
                    Description = action.Intitule,
                    Localistion = Localistion.accueil,
                    IdGroupe = Idgroupr,
                    Numero = 13,
                    NumeroMin = 13,
                    NumeroMax = 13,
                    Type = Models.Type.lien_bouton
                });
                // dossiers archivés
                action = actions.FirstOrDefault(a => a.Recherche == "ldarc");
                if(action!=null)
                db.GetComposants.Add(new Composant()
                {
                    EstActif = true,
                    IdAction = action.Id,
                    Recherche = "0" + action.Recherche,
                    Description = action.Intitule,
                    Localistion = Localistion.accueil,
                    IdGroupe = Idgroupr,
                    Numero = 14,
                    NumeroMin =14,
                    NumeroMax =14,
                    Type = Models.Type.lien_bouton
                });

                /// dossiers rejetés
                action = db.Actions.FirstOrDefault(a => a.Recherche == "ldntr");

                db.GetComposants.Add(new Composant()
                {
                    EstActif = true,
                    IdAction = action.Id,
                    Recherche = "0" + action.Recherche,
                    Description = "Dossiers rejetés",
                    Localistion = Localistion.accueil,
                    Info="Dossiers rejetés service trasfert vers conformité",
                    IdGroupe = Idgroupr,
                    Numero = 16,
                    NumeroMin = -3,
                    NumeroMax = -3,
                    Type = Models.Type.lien_bouton
                });
                db.GetComposants.Add(new Composant()
                {
                    EstActif = true,
                    IdAction = action.Id,
                    Recherche = "0" + action.Recherche,
                    Description = "Dossiers rejetés",
                    Info = "Dossiers rejetés vers agence",
                    Localistion = Localistion.accueil,
                    IdGroupe = Idgroupr,
                    Numero = 17,
                    NumeroMin = -2,
                    NumeroMax = -2,
                    Type = Models.Type.lien_bouton
                });
                db.GetComposants.Add(new Composant()
                {
                    EstActif = true,
                    IdAction = action.Id,
                    Recherche = "0" + action.Recherche,
                    Description = "Dossiers rejetés vers client",
                    Localistion = Localistion.accueil,
                    IdGroupe = Idgroupr,
                    Numero = 18,
                    NumeroMin = -1,
                    NumeroMax = -1,
                    Type = Models.Type.lien_bouton
                });

                action = actions.FirstOrDefault(a => a.Recherche == "dconf_st");
                db.GetComposants.Add(new Composant()
                {
                    EstActif = true,
                    IdAction = action.Id,
                    Recherche = "0" + action.Recherche,
                    Info = "Dossiers transmis agence",
                    Description = "Dossiers transmis",
                    Localistion = Localistion.accueil,
                    IdGroupe = Idgroupr,
                    Numero = 19,
                    NumeroMin = 3,
                    NumeroMax = 10,
                    Type = Models.Type.lien_bouton
                });
                db.GetComposants.Add(new Composant()
                {
                    EstActif = true,
                    IdAction = action.Id,
                    Recherche = "0" + action.Recherche,
                    Info = "Dossiers traités conformité",
                    Description = "Dossiers validés",
                    Localistion = Localistion.accueil,
                    IdGroupe = Idgroupr,
                    Numero = 20,
                    NumeroMin = 3,
                    NumeroMax = 8,
                    Type = Models.Type.lien_bouton
                });
                db.GetComposants.Add(new Composant()
                {
                    EstActif = true,
                    IdAction = action.Id,
                    Recherche = "dlenvbeac",
                    Info = "Dossiers envoyés à la BEAC",
                    Description = "Dossiers envoyés à la BEAC",
                    Localistion = Localistion.accueil,
                    IdGroupe = Idgroupr,
                    Numero = 21,
                    NumeroMin = 8,
                    NumeroMax = 10,
                    NumDiscontinue="-2",
                    Type = Models.Type.lien_bouton
                });
                db.GetComposants.Add(new Composant()
                {
                    EstActif = true,
                    IdAction = action.Id,
                    Recherche = "dfxaccorde",
                    Info = "Dossiers accordés",
                    Description = "Dossiers accordés",
                    Localistion = Localistion.accueil,
                    IdGroupe = Idgroupr,
                    Numero = 22,
                    NumeroMin = 8,
                    NumeroMax = 10,
                    NumDiscontinue="-2",
                    Type = Models.Type.lien_bouton
                });
            }

            var entitees = db.GetEntitees.ToList();
            //Creation de toutes les entitées
            if (entitees.Count() == 0)
            {
                db.GetEntitees.Add(new Entitee()
                {
                    Type = "dossier"
                });

                db.GetEntitees.Add(new Entitee()
                {
                    Type = "client"
                });

                db.GetEntitees.Add(new Entitee()
                {
                    Type = "agent"
                });

                db.GetEntitees.Add(new Entitee()
                {
                    Type = "banque"
                });

                db.GetEntitees.Add(new Entitee()
                {
                    Type = "structure"
                });

                db.GetEntitees.Add(new Entitee()
                {
                    Type = "pays"
                });

                db.GetEntitees.Add(new Entitee()
                {
                    Type = "ville"
                }); 
                
                db.GetEntitees.Add(new Entitee()
                {
                    Type = "domiciliation d'importation"
                }); 
                db.GetEntitees.Add(new Entitee()
                {
                    Type = "déclaration d'importation"
                }); 
                db.GetEntitees.Add(new Entitee()
                {
                    Type = "facture"
                }); db.GetEntitees.Add(new Entitee()
                {
                    Type = "instruction"
                }); 
                db.GetEntitees.Add(new Entitee()
                {
                    Type = "lettre d'engament"
                });
                db.GetEntitees.Add(new Entitee()
                {
                    Type = "quittance de paiement"
                });
                db.GetEntitees.Add(new Entitee()
                {
                    Type = "documents de transport"
                }); 
                db.GetEntitees.Add(new Entitee()
                {
                    Type = "document de douane"
                });
                db.GetEntitees.Add(new Entitee()
                {
                    Type = "autres documents"
                });
            }
            else
            {
                if (entitees.FirstOrDefault(r => r.Type == "dossier") == null) db.GetEntitees.Add(new Entitee()
                {
                    Type = "dossier"
                }); 
                
                if (entitees.FirstOrDefault(r => r.Type == "client") == null) db.GetEntitees.Add(new Entitee()
                {
                    Type = "client"
                }); if (entitees.FirstOrDefault(r => r.Type == "agent") == null) db.GetEntitees.Add(new Entitee()
                {
                    Type = "agent"
                }); if (entitees.FirstOrDefault(r => r.Type == "banque") == null) db.GetEntitees.Add(new Entitee()
                {
                    Type = "banque"
                }); if (entitees.FirstOrDefault(r => r.Type == "structure") == null) db.GetEntitees.Add(new Entitee()
                {
                    Type = "structure"
                }); if (entitees.FirstOrDefault(r => r.Type == "pays") == null) db.GetEntitees.Add(new Entitee()
                {
                    Type = "pays"
                });
                if (entitees.FirstOrDefault(r => r.Type == "ville") == null) db.GetEntitees.Add(new Entitee()
                {
                    Type = "ville"
                }); 
                
                if (entitees.FirstOrDefault(r => r.Type == "domiciliation d'importation") == null) db.GetEntitees.Add(new Entitee()
                {
                    Type = "domiciliation d'importation",
                    Niveau = 1
                });
                if (entitees.FirstOrDefault(r => r.Type == "déclaration d'importation") == null) db.GetEntitees.Add(new Entitee()
                {
                    Type = "déclaration d'importation",
                    Niveau = 1
                });
                if (entitees.FirstOrDefault(r => r.Type == "facture") == null) db.GetEntitees.Add(new Entitee()
                {
                    Type = "facture",
                    Niveau = 1
                });
                if (entitees.FirstOrDefault(r => r.Type == "instruction") == null) db.GetEntitees.Add(new Entitee()
                {
                    Type = "instruction",
                    Niveau = 1
                });
                if (entitees.FirstOrDefault(r => r.Type == "lettre d'engament") == null) db.GetEntitees.Add(new Entitee()
                {
                    Type = "lettre d'engament",
                    Niveau = 1
                });
                if (entitees.FirstOrDefault(r => r.Type == "quittance de paiement") == null) db.GetEntitees.Add(new Entitee()
                {
                    Type = "quittance de paiement",
                    Niveau = 1
                });
                if (entitees.FirstOrDefault(r => r.Type == "documents de transport") == null) db.GetEntitees.Add(new Entitee()
                {
                    Type = "documents de transport",
                    Niveau = 1
                });
                if (entitees.FirstOrDefault(r => r.Type == "autres documents") == null) db.GetEntitees.Add(new Entitee()
                {
                    Type = "autres documents",
                    Niveau = 1
                });
                if (entitees.FirstOrDefault(r => r.Type == "document de douane") == null) db.GetEntitees.Add(new Entitee()
                {
                    Type = "document de douane",
                    Niveau = 1
                });

            }

            try
            {
                db.SaveChanges();
            }
            catch (Exception ee)
            {}
        }

        private void createRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            
            // In Startup iam creating first Admin Role and creating a default Admin User     
            if (!roleManager.RoleExists("Admin"))
            {

                // first we create Admin rool    
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);

                //Here we create a Admin super user who will maintain the website                   

                var user = new ApplicationUser();
                user.UserName = "shanu";
                user.Email = "adminc01@gmail.com";

                string userPWD = "123456";

                var chkUser = UserManager.Create(user, userPWD);

                //Add default User to Role Admin    
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Admin");

                }
            }

            // creating Creating Manager role     
            if (!roleManager.RoleExists("Manager"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Manager";
                roleManager.Create(role);

            }

            // creating Creating banque role     
            if (!roleManager.RoleExists("Gestionnaire"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Gestionnaire";
                roleManager.Create(role);
            }

            // creating Creating banque role     
            if (!roleManager.RoleExists("Responsable"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Responsable";
                roleManager.Create(role);
            }

            // creating Creating banque role     
            if (!roleManager.RoleExists("STP"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "STP";
                roleManager.Create(role);

            }
        }


    }
}