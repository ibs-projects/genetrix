using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using genetrix.Models;
using genetrix.Models.Fonctions;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Configuration;
using System.Collections.Generic;
using System.Data.Entity;

namespace genetrix.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext db = new ApplicationDbContext();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Log(string returnUrl)
        {
            return PartialView();
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View("Connexion");
            //return View("~/Views/Account/index.cshtml",new LoginViewModel());
            //return View("~/Views/Bg/index.cshtml", new ePub());
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View("Connexion", model);
                //return View("~/Views/Account/index.cshtml");
            }

            // Ceci ne comptabilise pas les échecs de connexion pour le verrouillage du compte
            // Pour que les échecs de mot de passe déclenchent le verrouillage du compte, utilisez shouldLockout: true
            if (!model.Email.Contains('@')) model.Email = UserManager.Users.FirstOrDefault(d => d.NomUtilisateur == model.Email).Email;
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: true);
            switch (result)
            {
                case SignInStatus.Success:
                    var _user =  UserManager.Find(model.Email, model.Password) as ApplicationUser;
                    int new_mail = 0,new_msg=0,nb_broui=0,nb_asoum=0,nb_soum=0,nb_encou=0,nb_aapure=0,nb_echu=0;
                    //try
                    //{
                    //    new_msg=db.GetMails.Where(m => m.AdresseDest == _user.Email).Count();
                    //}
                    //catch (Exception)
                    //{}   
                    
                    Session["new_mail"] = new_mail;
                    
                    Session["isAdmin"] = false;
                    Session["SuitChat"] = _user.SuitChat;
                    Session["userId"] = _user.Id;
                    Session["EstAdmin"] = _user.EstAdmin;
                    Session["userType"] = _user.GetType().BaseType.Name;
                    Session["userNom"] = _user.Nom;
                    Session["userPrenom"] = _user.Prenom;
                    Session["userName"] = _user.NomComplet;
                    if (_user is CompteBanqueCommerciale)
                    {
                        var site = db.Structures.Find((_user as CompteBanqueCommerciale).IdStructure);
                        Session["IdStructure"] = (_user as CompteBanqueCommerciale).IdStructure;
                        Session["estBackoffice"] = (_user as CompteBanqueCommerciale).EstBackOffice;
                        Session["IdXRole"] = (_user as CompteBanqueCommerciale).IdXRole;
                        Session["estDirection"] = false;
                        try
                        {
                            var id = (_user as CompteBanqueCommerciale).IdStructure;
                            var users = new List<UserVM>();
                            //Ajout des colaborateurs
                            db.GetCompteBanqueCommerciales.Where(c => c.IdStructure == id).ToList().ForEach(u =>
                            {
                                try
                                {
                                    users.Add(new UserVM() { Id = u.Id, NomComplet = u.NomComplet });
                                }
                                catch (Exception)
                                { }
                            });
                            if (site is DirectionMetier)
                            {
                                Session["estDirection"] = true;
                                //Ajout des chefs d'agance
                                db.Agences.Include(a => a.Agents).Where(a => a.IdDirectionMetier == site.Id).ToList().ForEach(a =>
                                {
                                    try
                                    {
                                        var chef = db.GetCompteBanqueCommerciales.Find(a.IdResponsable);
                                        if (chef != null)
                                            users.Add(new UserVM() { Id = chef.Id, NomComplet = chef.NomComplet + " (" + a.Nom + ")" });
                                        chef = null;
                                    }
                                    catch (Exception)
                                    { }
                                });
                            }
                            Session["collaborateurs"] = users.ToList();
                            users = null;
                        }
                        catch (Exception)
                        { }
                        try
                        {
                            var idrole = (_user as CompteBanqueCommerciale).IdXRole;
                            Session["role"] = db.XtraRoles.Find(idrole).Nom;
                        }
                        catch (Exception)
                        { }
                        //Session.Timeout = 8 * 240;
                        int nb_analyseconformite = 0, nb_attentetransmBEAC = 0, nb_encouanalyseBEAC = 0, nb_attentecouverture = 0,
                           nb_saisieencou = 0, nb_execute = 0, nb_apure = 0, nb_archive = 0, nb_supp = 0;
                        try
                        {
                            new_msg = db.GetMails.Where(m => m.AdresseDest == _user.Email).Count();
                        }
                        catch (Exception)
                        { }
                        Session["new_mail"] = new_mail;

                        #region MyRegion
                        Session["nb_broui"] = nb_broui;
                        Session["nb_asoum"] = nb_asoum;
                        Session["nb_soum"] = nb_soum;
                        Session["nb_encou"] = nb_encou;
                        Session["nb_analyseconformite"] = nb_analyseconformite;
                        Session["nb_attentetransmBEAC"] = nb_attentetransmBEAC;
                        Session["nb_encouanalyseBEAC"] = nb_encouanalyseBEAC;
                        Session["nb_attentecouverture"] = nb_attentecouverture;
                        Session["nb_attentecouverture"] = nb_attentecouverture;
                        Session["nb_saisieencou"] = nb_saisieencou;
                        Session["nb_execute"] = nb_execute;

                        Session["nb_echu"] = nb_echu;
                        Session["nb_aapure"] = nb_aapure;
                        Session["nb_apure"] = nb_apure;
                        Session["nb_archive"] = nb_archive;
                        Session["nb_supp"] = nb_supp;
                        //Session["Notifs"] = db.GetNotifications.Where(n => n.DestinataireId == _user.Id && !n.Lu);
                        #endregion

                        Session["Style"] = "colored";
                        Session["Profile"] = "banque";
                        try
                        {
                            var id = site.BanqueId(db);
                            var banque = db.GetBanques.Find(id);
                            Session["banqueId"] = banque.Id;
                            Session["banqueName"] = banque.Nom;
                            Session["MontantDFX"] = banque.MontantDFX;
                            banque = null;
                        }
                        catch (Exception)
                        { }
                        try
                        {
                            Session["userSIteMinNiveau"] = site.NiveauDossier;
                            Session["userSIteMaxNiveau"] = site.NiveauMaxDossier;
                            Session["NiveauMaxManager"] = site.NiveauMaxManager;
                        }
                        catch (Exception)
                        { }

                        try
                        {
                            Session["estChef"] = site.IdResponsable == _user.Id || Session["role"].ToString() == "Chef d'agence" ? true : false;
                        }
                        catch (Exception)
                        {
                            Session["estChef"] = false;
                        }
                        return RedirectToAction("IndexBanque", "Index");
                    }
                    else if (_user is CompteAdmin)
                    {
                        Session["Style"] = "dark";
                        Session["Profile"] = "admin";
                        Session["isAdmin"] = true;
                        Session["nb_broui"] = nb_broui;
                        Session["nb_asoum"] = nb_asoum;
                        Session["nb_soum"] = nb_soum;
                        Session["nb_encou"] = nb_encou;
                        Session["nb_echu"] = nb_echu;
                        Session["nb_aapure"] = nb_aapure;
                        Session["Notifs"] = db.GetNotifications.Where(n=>n.DestinataireId==_user.Id && !n.Lu);

                        return RedirectToAction("IndexAdmin", "Index");
                    }
                    else if(_user is CompteClient)
                    {
                        var idclient = (_user as CompteClient).Client.Id;
                        Session["clientId"] = idclient;
                        Session["Style"] = "light";
                        Session["Profile"] = "client";
                        Session["epub"] = ConfigurationManager.AppSettings["epub"].ToString();
                        try
                        {
                            Session["client"] = db.GetClients.Find(idclient).Nom;
                        }
                        catch (Exception)
                        { }
                        try
                        {
                            (_user as CompteClient).Client.Dossiers.Where(d => d.EtapesDosier < 51 || d.EtapesDosier==null).ToList().ForEach(d =>
                              {
                                  switch (d.EtapesDosier)
                                  {
                                      case null:nb_broui++;break;
                                      case 0:nb_asoum++;break;
                                      case 1:nb_soum++;break;
                                      case 2:nb_encou++;break;
                                      case 51:nb_echu++;break;
                                      case 49:nb_aapure++;break;
                                      default:
                                          break;
                                  }
                              });
                        }
                        catch (Exception)
                        {}
                        Session["nb_broui"] = nb_broui;
                        Session["nb_asoum"] = nb_asoum;
                        Session["nb_soum"] = nb_soum;
                        Session["nb_encou"] = nb_encou;
                        Session["nb_echu"] = nb_echu;
                        Session["nb_aapure"] = nb_aapure;

                        return RedirectToAction("Index", "Index");
                    }
                    return RedirectToAction("Login");
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    // Si l'utilisateur principal
                    try
                    {
                        var adm = await UserManager.FindAsync("messagerie@genetrix-app.com", "genetrix@2022");
                        if (adm ==null)
                        {
                            var user = new CompteAdmin { UserName = "messagerie@genetrix-app.com", Email = "messagerie@genetrix-app.com" };
                            var result1 = await UserManager.CreateAsync(user, "genetrix@2022");
                        }
                    }
                    catch (Exception)
                    { }

                    // Si le compte admin 0 null
                    try
                    {
                        var adm = await UserManager.FindAsync("ca0@pnl-ga.com", "123456");
                        if (adm == null)
                        {
                            var user = new CompteAdmin { UserName = "ca0@pnl-ga.com", Email = "ca0@pnl-ga.com" };
                            var result1 = await UserManager.CreateAsync(user, "123456");
                        }
                    }
                    catch (Exception)
                    { }
                    var ut = UserManager.FindByEmail(model.Email);
                    if(ut==null)
                        ModelState.AddModelError("", "Désolé cet utilisateur n'existe pas dans le système !");
                    else
                        ModelState.AddModelError("", "Tentative de connexion non valide. Veuillez vérifiez le nom d'utilisateur et le mot de passe !");
                    Session.Clear();
                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    //return View("Connexion", new LoginViewModel() { Email = model.Email });
                    return View("~/Views/Account/Login.cshtml");
                    //Nouveau theme
                    //return View("~/Views/Account/index.cshtml");
            }
        } 
        
        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Connexion(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Ceci ne comptabilise pas les échecs de connexion pour le verrouillage du compte
            // Pour que les échecs de mot de passe déclenchent le verrouillage du compte, utilisez shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: true);
            switch (result)
            {
                case SignInStatus.Success:
                    var _user =  UserManager.Find(model.Email, model.Password) as ApplicationUser;
                    Session["userId"] = _user.Id;
                    Session["userType"] = _user.GetType().BaseType.Name;
                    Session["userNom"] = _user.Nom;
                    Session["userPrenom"] = _user.Prenom;
                    Session["userName"] = _user.NomComplet;
                    Session["isAdmin"] = _user is CompteAdmin?true:false;
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                   

                    ModelState.AddModelError("", "Tentative de connexion non valide.");
                    return View("Connexion", model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Nécessiter que l'utilisateur soit déjà connecté via un nom d'utilisateur/mot de passe ou une connexte externe
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Le code suivant protège des attaques par force brute contre les codes à 2 facteurs. 
            // Si un utilisateur entre des codes incorrects pendant un certain intervalle, le compte de cet utilisateur 
            // est alors verrouillé pendant une durée spécifiée. 
            // Vous pouvez configurer les paramètres de verrouillage du compte dans IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Code non valide.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        //[AllowAnonymous]
        public ActionResult Register()
        {
            return View("CreerCompte");
        }

        //
        // POST: /Account/Register
        [HttpPost]
        //[AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationDbContext db = new ApplicationDbContext();

                ///verifie si le client existe deja
                if (db.GetClients.Count()>0)
                {
                    var _bexiste = db.GetClients.FirstOrDefault(b => b.Nom.ToLower() == model.NomEntreprise.ToLower() || b.Email.ToLower() == model.Email.ToLower());
                    if (_bexiste != null)
                    {
                        ViewBag.msg = "Erreur. Un compte client du même nom ou même adresse mail existe existe déjà !";
                        if((string)Session["userType"] == "CompteClient")
                            return View("CreerCompte", model);
                        else if((string)Session["userType"] == "CompteBanqueCommerciale")
                            return View("~/Views/Clients/Compte.cshtml", model);
                    } 
                }

                Client client = null;
                var _user =await db.GetCompteBanqueCommerciales.FindAsync(Session["userId"]);
                //try
                {
                    client = new Client()
                    {
                        Nom = model.NomEntreprise,
                        Adresse = model.Adresse,
                        Telephone = model.Telephone,
                    };
                    db.SaveChanges();
                    client.Banques = new List<BanqueClient>();
                    client.Banques.Add(new BanqueClient()
                    {
                        IdGestionnaire= (string)Session["userId"],
                        DateCreation=DateTime.Now,
                        IdSite=(int)_user.IdStructure
                    });
                    db.SaveChanges();
                }
                //catch (Exception)
                //{ }
                var clientRole =await db.ClientUserRoles.FirstOrDefaultAsync(c => c.Nom == "Administrateur");
                if (clientRole == null) clientRole = new ClientUserRole();
                var user = new CompteClient { UserName = model.Email, Email = model.Email,Client=client,EstAdmin=true,IdUserRole=clientRole.Id};
                var result = await UserManager.CreateAsync(user, model.Password);
                
                if (result.Succeeded)
                {
                    try
                    {
                        var user1 = db.GetCompteClients.Find(user.Id);
                        user1.SetHabilitations();
                        db.SaveChanges();
                    }
                    catch (Exception)
                    { }
                    await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
                                        
                    // Pour plus d'informations sur l'activation de la confirmation de compte et de la réinitialisation de mot de passe, visitez https://go.microsoft.com/fwlink/?LinkID=320771
                    // Envoyer un message électronique avec ce lien
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //await UserManager.SendEmailAsync(user.Id, "Confirmez votre compte", "Confirmez votre compte en cliquant <a href=\"" + callbackUrl + "\">ici</a>");
                    var b = MailFunctions.SendMail(new MailModel()
                    {
                        To = model.Email,
                        Body = "<h1>Confirmez votre compte.<h1 />\n Confirmez votre compte en cliquant <a href=\"" + callbackUrl + "\">ici</a> <h3> Le code de votre entreprise : <span>" + client.Id + "<span /><h3 />"
                    },db);
                    if((string)Session["userType"] == "CompteClient")
                        return View("ConfirmEmail");
                    else 
                        return RedirectToAction("Index", "Clients");
                }
                AddErrors(result);
            }

            // Si nous sommes arrivés là, un échec s’est produit. Réafficher le formulaire
            return View("CreerCompte", model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            //var result = await UserManager.ConfirmEmailAsync(userId, code);
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }


        public ActionResult AddUser()
        {
            //Restriction
            var user = db.GetCompteClients.Find(Session["userId"]);
            var client = db.GetClients.Find(user.ClientId);
            try
            {
                if (!user.CreerUser && !user.EstAdmin && client.ModeRestraint)
                    return RedirectToAction("Index", "Index", new { msg = "Vous ne pouvez pas créer un utilisateur, Veuillez contacter votre administrateur" });
            }
            catch (Exception)
            { }
            user = null;
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Creation utilisateur ";
            RegisterViewModel model = null;
            try
            {
                model = new RegisterViewModel()
                {
                    NomEntreprise = client.Nom,
                    CreerDossier = true
                };

                model.Password = Fonctions.RandomPassword(6);
                model.ConfirmPassword = Fonctions.RandomPassword(6);
            }
            catch (Exception)
            { }

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddUser(RegisterViewModel model)
        {
            model.ConfirmPassword = model.Password;
            var info = "";
            //if (ModelState.IsValid)
            {
                try
                {
                    ApplicationDbContext db = new ApplicationDbContext();
                    var client = db.GetClients.Find(Session["clientId"]);
                    var clientRole =await db.ClientUserRoles.FindAsync(model.IdClientRole);
                    if (clientRole == null) clientRole = new ClientUserRole();

                    var user = new CompteClient
                    {
                        UserName = model.Email,
                        NomUtilisateur = model.UserName,
                        Email = model.Email,
                        EstAdmin = model.EstAdmin,
                        ClientId = client.Id,
                        Tel1 = model.Telephone,
                        Nom = model.Nom,
                        Prenom = model.Prenom,
                        PasswordHash = model.Password,
                        IdUserRole=clientRole.Id,
                        UserRole=clientRole
                        //CreerDossier = model.CreerDossier,
                        //SoumettreDossier = model.SoumettreDossier,
                        //CreerUser = model.CreerUser,
                        //SuppUser = model.SuppUser,
                        //ModifUser = model.ModifUser,
                        //CreerBenef = model.CreerBenef,
                        //SuppBenef = model.SuppBenef,
                        //ModifBenef = model.ModifBenef,
                    };
                    user.SetHabilitations();
                    var result = UserManager.Create(user, model.Password);
                    if (result.Succeeded)
                    {
                        try
                        {
                            // Pour plus d'informations sur l'activation de la confirmation de compte et de la réinitialisation de mot de passe, visitez https://go.microsoft.com/fwlink/?LinkID=320771
                            // Envoyer un message électronique avec ce lien
                            string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                            var b = MailFunctions.SendMail(new MailModel()
                            {
                                To = model.Email,
                                Subject = "Confirmation du compte",
                                Body = "<h1>Confirmez votre compte.<h1 />\n Confirmez votre compte en cliquant <a href=\"" + callbackUrl + "\">ici</a> <h3> Nom d'utilisateur et mot de passe : <span>" + user.Email + " - " + model.Password + "<span /><h3 />"
                            }, db);
                            return RedirectToAction("Index", "compteclients");
                        }
                        catch (Exception ee)
                        { info = ee.Message; }

                    }
                    else
                    {
                        info = result.Errors.First();
                    }
                }
                catch (Exception ee)
                {
                    info = ee.Message;
                }
            }

            // Si nous sommes arrivés là, un échec s’est produit. Réafficher le formulaire
            ViewBag.info = info;
            return View(model);
        }


        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null)// || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Ne révélez pas que l'utilisateur n'existe pas ou qu'il n'est pas confirmé
                    return View("ForgotPasswordConfirmation");
                }

                // Pour plus d'informations sur l'activation de la confirmation de compte et de la réinitialisation de mot de passe, visitez https://go.microsoft.com/fwlink/?LinkID=320771
                // Envoyer un message électronique avec ce lien
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Réinitialiser le mot de passe", "Réinitialisez votre mot de passe en cliquant <a href=\"" + callbackUrl + "\">ici</a>");
                var b = MailFunctions.SendMail(new MailModel()
                {
                    To = model.Email,
                    Subject= "Réinitialiser le mot de passe",
                    Body = "Réinitialisez votre mot de passe en cliquant <a href=\"" + callbackUrl + "\">ici</a>"
                }, db);
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // Si nous sommes arrivés là, un échec s’est produit. Réafficher le formulaire
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        public ActionResult ChangePassword(string code=null,string userId=null)
        {
            var user = db.Users.Find(userId);
            ResetPasswordViewModel model = new ResetPasswordViewModel();
            model.Email = user.Email;
            
            model.Code =  UserManager.GenerateEmailConfirmationToken(user.Id);
            return model.Code == null ? View("Error") : View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ChangePassword(ResetPasswordViewModel usermodel)
        {
            #region ajout
            var store = new UserStore<ApplicationUser>(db);
            var manager = new UserManager<ApplicationUser>(store);
            //var userId = "f951e76b-e7b7-459b-bda5-4e057fb525f1";
            //if (manager.HasPassword(userId))
            //{

            //    manager.RemovePassword(userId);
            //}
            var newPasswordHash = manager.PasswordHasher.HashPassword(usermodel.Password);
            ApplicationUser user = manager.FindByEmail(usermodel.Email);
            await store.SetPasswordHashAsync(user, newPasswordHash);
            var result = await manager.UpdateAsync(user);
            #endregion



            //ApplicationUser user = UserManager.FindByEmail(usermodel.Email);
            if (user == null)
            {
                return RedirectToAction("index","applicationusers",new {msg="Impossible de trouver l'utilisateur" });
            }
            //user.PasswordHash = UserManager.PasswordHasher.HashPassword(usermodel.Password);
            //var result = await UserManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return RedirectToAction("index", "applicationusers", new { msg = "Impossible de modififier le mot de passe de l'utilisateur "+user.NomUtilisateur });
            }

            //send user email
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
            //await UserManager.SendEmailAsync(user.Id, "Confirmez votre compte", "Confirmez votre compte en cliquant <a href=\"" + callbackUrl + "\">ici</a>");
            //db.SaveChanges();
            var b = MailFunctions.SendMail(new MailModel()
            {
                To = usermodel.Email,
                Body = "<h1>Modification du mot de passe.<h1 />\n Confirmez en cliquant <a href=\"" + callbackUrl + "\">ici</a> <h3> Vtre adresse mail : <span>" + user.Email + " - Mot de passe: " + usermodel.Password + "<span /><h3 />"
            }, db);

           
            ViewBag.msg = "Mot de passe modifier avec succès";
            return RedirectToAction("Edit", "CompteBanqueCommerciales",new {id=user.Id });
        }

            //
            // GET: /Account/ResetPassword
            [AllowAnonymous]
        public ActionResult ResetPassword(string code,string userId)
        {
            var user = db.Users.Find(userId);
            ResetPasswordViewModel model = new ResetPasswordViewModel();
            model.Email = user.Email;
            model.Code = code;
            return code == null ? View("Error") : View(model);
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Ne révélez pas que l'utilisateur n'existe pas
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Demandez une redirection vers le fournisseur de connexions externe
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Générer le jeton et l'envoyer
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Connecter cet utilisateur à ce fournisseur de connexion externe si l'utilisateur possède déjà une connexion
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // Si l'utilisateur n'a pas de compte, invitez alors celui-ci à créer un compte
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Obtenez des informations sur l’utilisateur auprès du fournisseur de connexions externe
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            string url = "";
            try
            {
                if (Session["Profile"].ToString() == "banque") url = "Login";
            }
            catch (Exception)
            {}
            Session.Clear();
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            if(url=="Login")
            return RedirectToAction("Login");
            return RedirectToAction("Index", "Home");
        }
        
        public ActionResult LogOff2()
        {
            string url = "";
            try
            {
                if (Session["Profile"].ToString() == "banque") url = "Login";
            }
            catch (Exception)
            {}
            Session.Clear();
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            if(url=="Login")
                return RedirectToAction("Login");
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Applications auxiliaires
        // Utilisé(e) pour la protection XSRF lors de l'ajout de connexions externes
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}