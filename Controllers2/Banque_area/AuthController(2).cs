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
using eApurement.Models;
using e_apurement.Models;
using eApurement.Models.Fonctions;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace eApurement.Controllers
{
    [Authorize]
    public class AuthController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext db = new ApplicationDbContext();

        public AuthController()
        {
        }

        public AuthController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
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
        [ActionName("Connexion")]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;            
            return View("~/Views/Account/Login.cshtml");
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Connexion(LoginViewModel model, string returnUrl)
        {
            var msg = "";
            Session.RemoveAll();
            Session.Clear();
            if (!ModelState.IsValid)
            {
                return View("~/Views/Account/Login.cshtml");
            }

            foreach (var item in db.Users)
            {
                var mail = item.Email;
                var dd = item.PasswordHash;
            }

            // Ceci ne comptabilise pas les échecs de connexion pour le verrouillage du compte
            // Pour que les échecs de mot de passe déclenchent le verrouillage du compte, utilisez shouldLockout: true
            try
            {
                var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: true);
                var _user = UserManager.Find(model.Email, model.Password) as ApplicationUser;
                //var result = await SignInManager.SignInAsync(user:_user,isPersistent:true,rememberBrowser: model.RememberMe);
                //var tt=result;
                switch (result)
                //if(_user!=null)
                {
                    case SignInStatus.Success:
                        Session.Timeout = 8*240;
                        Session["user"] = _user;
                        //if (_user is CompteAdmin)
                        //{
                        //    Session["isAdmin"] = true;
                        //    return RedirectToAction("IndexAdmin", "Index");
                        //}
                        //else 
                        int new_mail = 0, new_msg = 0, nb_broui = 0, nb_asoum = 0, nb_soum = 0, nb_encou = 0, nb_aapure = 0, nb_echu = 0,
                                      nb_analyseconformite = 0, nb_attentetransmBEAC = 0, nb_encouanalyseBEAC = 0, nb_attentecouverture = 0,
                                      nb_saisieencou = 0, nb_execute = 0, nb_apure = 0, nb_archive = 0, nb_supp = 0; try
                        {
                            new_msg = db.GetMails.Where(m => m.AdresseDest == _user.Email).Count();
                        }
                        catch (Exception)
                        { }
                        Session["new_mail"] = new_mail;

                        if (_user is CompteBanqueCommerciale)
                        {
                            //Affichage de prtefeuille utilisateur
                            // et collaborateurs
                            try
                            {
                                var estChef = ((_user as CompteBanqueCommerciale).Structure.IdResponsable == (_user as CompteBanqueCommerciale).Id);
                                Session["estChef"] = estChef;
                                if (estChef)
                                {
                                    var idStructure= (_user as CompteBanqueCommerciale).IdStructure;
                                    Session["collaborateurs"] = db.GetCompteBanqueCommerciales.Where(c=>c.IdStructure==idStructure).ToList();
                                }
                                else
                                {
                                    Session["collaborateurs"] = _user as CompteBanqueCommerciale;
                                }
                            }
                            catch (Exception)
                            { }
                            var site = (_user as CompteBanqueCommerciale).Structure;
                            try
                            {
                               db.GetDossiers.Where(d =>d.IdSite==site.Id && d.EtapesDosier < 51 || d.EtapesDosier == null).ToList().ForEach(d =>
                                {
                                    switch (d.EtapesDosier)
                                    {
                                        case null: nb_broui++; break;
                                        case 0: nb_asoum++; break;
                                        case 1: nb_soum++; break;
                                        case 2: nb_encou++; break;
                                        case 3: nb_analyseconformite++; break;
                                        case 4: nb_attentetransmBEAC++; break;
                                        case 5: nb_encouanalyseBEAC++; break;
                                        case 6: nb_attentecouverture++; break;
                                        case 7: nb_saisieencou++; break;
                                        case 8: nb_execute++; break;

                                        case 40: nb_echu++; break;
                                        case 50: nb_aapure++; break;
                                        case 51: nb_apure++; break;
                                        case 52: nb_archive++; break;
                                        case 53: nb_supp++; break;
                                        default:
                                            break;
                                    }
                                });
                            }
                            catch (Exception)
                            { }
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
                            Session["Notifs"] = db.GetNotifications.Where(n => n.DestinataireId == _user.Id && !n.Lu);
                            #endregion

                            Session["Style"] = "colored";
                            Session["Profile"] = "banque";

                            try
                            {
                                Session["userSIteMinNiveau"] = site.NiveauDossier;
                                Session["userSIteMaxNiveau"] = site.NiveauMaxDossier;
                            }
                            catch (Exception)
                            { }


                            try
                            {
                                if ((_user as CompteBanqueCommerciale).XRole.EstAdmin)
                                {
                                    //for (int i = 0; i <= 111; i++)
                                    //{
                                    //    Session["numero" + i] = i;
                                    //}

                                    Session["composants"] = db.GetComposants.Include(c=>c.Action).Include(c=>c.Groupe).ToList();
                                }
                                else 
                                {
                                    var id = (_user as CompteBanqueCommerciale).IdXRole;
                                    var roles = db.GetIHMs.Where(o=>o.XRoleId==id).ToList();
                                    foreach (var item in roles)
                                    {
                                        try
                                        {
                                            var comp = db.GetComposants.Find(item.ComposantId);
                                            Session[comp.Nom] = comp.Numero;
                                        }
                                        catch (Exception e)
                                        {}
                                    }
                                }
                                
                            }
                            catch (Exception ee)
                            {}

                            return RedirectToAction("IndexBanque", "Index");
                        }
                        else
                        {
                            ViewBag.info= "Utilisateur non authentifié !";
                            return View("~/Views/Account/Login.cshtml", model);
                        }
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    case SignInStatus.Failure:
                    default:


                        ModelState.AddModelError("", "Tentative de connexion non valide.");
                        return View("~/Views/Account/Login.cshtml", model);
                }
                msg = "Adresse mail ou mot de passe incorreste.";
            }
            catch (Exception ee)
            {
                msg = ee.Message;
            }
            ViewBag.info = msg;
            return View("~/Views/Account/Login.cshtml");
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
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View("~/Views/Account/Register.cshtml");
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            var info = "";
            if (ModelState.IsValid)
            {
                try
                {
                    ApplicationDbContext db = new ApplicationDbContext();
                    //var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                    //var transaction = db.Database.BeginTransaction();
                    var cc = db.XtraRoles.FirstOrDefault(x => x.EstAdmin);
                    if (cc == null)
                    {
                        ViewBag.info = "Impossible de créer le compte, l'erreur AUTHCONT-229-" + DateTime.Now.Millisecond + " a été générée.";
                        return View("~/Views/Account/Register.cshtml", model);
                    }

                    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

                    ///verifie si la banque existe deja
                    e_apurement.Models.Banque _bexiste = null;
                    try
                    {
                        _bexiste = db.GetBanques.FirstOrDefault(b => b.Nom.ToLower() == model.NomEntreprise.ToLower());
                    }
                    catch (Exception ee)
                    {}

                    if (_bexiste != null)
                    {
                        ViewBag.msg = "La banque du même nom existe déjà!";
                        return View("~/Views/Account/Register.cshtml", model);
                    }
                    var typeStructure = db.GetTypeStructures.FirstOrDefault(t=>t.Intitule.ToLower()=="banque");
                    e_apurement.Models.Banque banque = null;
                    banque = new e_apurement.Models.Banque()
                    {
                        Nom = model.NomEntreprise,
                        Adresse = model.Adresse,
                        Telephone = model.Telephone,
                        IdTypeStructure=typeStructure.Id
                        //Administrateur=user
                    };

                    db.GetBanques.Add(banque);
                    db.SaveChanges();

                    var user = new CompteBanqueCommerciale { UserName = model.Email, NomUtilisateur = model.UserName, Email = model.Email, EstAdmin = true, IdXRole = cc.RoleId,IdStructure=banque.BanqueId(db) };
                    var result = await UserManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        try
                        {
                            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                            // Pour plus d'informations sur l'activation de la confirmation de compte et de la réinitialisation de mot de passe, visitez https://go.microsoft.com/fwlink/?LinkID=320771
                            // Envoyer un message électronique avec ce lien
                            string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                            //await UserManager.SendEmailAsync(user.Id, "Confirmez votre compte", "Confirmez votre compte en cliquant <a href=\"" + callbackUrl + "\">ici</a>");
                            //db.SaveChanges();
                            var b = MailFunctions.SendMail(new MailModel()
                            {
                                To = model.Email,
                                Body = "<h1>Confirmez votre compte.<h1 />\n Confirmez votre compte en cliquant <a href=\"" + callbackUrl + "\">ici</a> <h3> Le code de votre banque : <span>" + banque.Id + "<span /><h3 />"
                            }, db);
                            return View("~/Views/Account/ConfirmEmail.cshtml");
                        }
                        catch (Exception ee)
                        { info = ee.Message; }

                    }
                    else
                    {
                        db.GetBanques.Remove(banque);
                        db.Users.Remove(user);
                        db.SaveChanges();
                    }
                    AddErrors(result);
                }
                catch (Exception ee)
                {}
            }

            // Si nous sommes arrivés là, un échec s’est produit. Réafficher le formulaire
            ViewBag.info = info;
            return View("~/Views/Account/Register.cshtml", model);
        }

        public ActionResult AddUser()
        {
            ViewBag.RoleId = new SelectList(db.XtraRoles, "RoleId", "Nom");
            ViewBag.Agences = new SelectList(db.Structures, "Id", "Nom");

            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Creation utilisateur ";

            var banque = db.GetBanques.Find((Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db));
            var model = new RegisterViewModel()
            {
                NomEntreprise = banque.Nom,
            };
            model.Roles = new List<XtraRole>();
            model.Sites = new List<Structure>();

            model.Roles.AddRange(db.XtraRoles);
            model.Sites.AddRange(db.Agences.Where(a => a.BanqueId(db) == banque.Id));

            return View("~/Views/ApplicationUsers/_adduser.cshtml", model);
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
                    //var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                    var cc = db.XtraRoles.FirstOrDefault(x => x.RoleId==model.IdRole);
                    if (cc == null)
                    {
                        ViewBag.info = "Impossible de créer le compte le rôle ne peut pas être nul, l'erreur AUTHCONT-229-" + DateTime.Now.Millisecond + " a été générée.";
                        return View("~/Views/ApplicationUsers/Create.cshtml", model);
                    }

                    var agence = db.Agences.FirstOrDefault(x => x.Id == model.IdAgence);

                    //int banqueID = (int)(Session["user"] as CompteBanqueCommerciale).Structure.BanqueId;

                    var user = new CompteBanqueCommerciale { 
                        UserName = model.Email, 
                        NomUtilisateur = model.UserName, 
                        Email = model.Email, 
                        EstAdmin = model.EstAdmin,
                       // IdSite= model.IdAgence/*IdAgence=model.IdAgence*/, 
                        IdStructure= model.IdAgence,
                        //IdBanque= banqueID,
                        IdXRole=model.IdRole,
                        Tel1=model.Telephone,
                        Nom=model.Nom,
                        Prenom=model.Prenom,
                        PasswordHash=model.Password,
                        EstGestionnaire=model.EstGestionnaire
                    };
                    var result = UserManager.Create(user, model.Password);
                    //var result = await UserManager.CreateAsync(user, model.Password);
                    //var result = db.Users.Add(user);
                    //db.SaveChanges();
                    if (result.Succeeded)
                    {
                        try
                        {
                            //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                            // Pour plus d'informations sur l'activation de la confirmation de compte et de la réinitialisation de mot de passe, visitez https://go.microsoft.com/fwlink/?LinkID=320771
                            // Envoyer un message électronique avec ce lien
                            string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                            //await UserManager.SendEmailAsync(user.Id, "Confirmez votre compte", "Confirmez votre compte en cliquant <a href=\"" + callbackUrl + "\">ici</a>");
                            //db.SaveChanges();
                            var b = MailFunctions.SendMail(new MailModel()
                            {
                                To = model.Email,
                                Subject="Confirmation du compte",
                                Body = "<h1>Confirmez votre compte.<h1 />\n Confirmez votre compte en cliquant <a href=\"" + callbackUrl + "\">ici</a> <h3> Nom d'utilisateur et mot de passe : <span>" + user.Email+" - "+model.Password + "<span /><h3 />"
                            }, db);
                            return RedirectToAction("Index", "comptebanquecommerciales");
                        }
                        catch (Exception ee)
                        { info = ee.Message; }

                    }
                    else
                    {
                        info = result.Errors.First();
                        //db.Sites.Remove(banque);
                        //db.SaveChanges();
                    }
                    //AddErrors(result);
                }
                catch (Exception ee)
                {
                    info = ee.Message;
                }
            }

            // Si nous sommes arrivés là, un échec s’est produit. Réafficher le formulaire
            ViewBag.info = info;
            return View("~/Views/ApplicationUsers/_adduser.cshtml", model);
            //return View(model);
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
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View("~/Views/Account/ForgotPassword.cshtml");
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

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
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
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Connexion");
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