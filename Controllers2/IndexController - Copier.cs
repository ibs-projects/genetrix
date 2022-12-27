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
using genetrix.Models;
using genetrix;
using System.Collections.Generic;

namespace genetrix.Controllers
{
    [Authorize]
    public class IndexController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

        public IndexController()
        {
            
        }

        public IndexController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
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

        public async Task<ApplicationUser> GetUser()
        {
            ApplicationUser _user = null;
            try
            {
                _user =await UserManager.FindByIdAsync(User.Identity.GetUserId()) as ApplicationUser;
            }
            catch (Exception)
            { }
            return _user;
        }

        //[Authorize(Roles = "CompteAdmin")]
        public ActionResult IndexAdmin()
        {
            return View();
        }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="panel"></param>
       /// <param name="acc"></param>
       /// <param name="ges"></param>
       /// <param name="struc"></param>
       /// <param name="interf">pour le service transfert accueil 0</param>
       /// <param name="port">id utilisateur: pour afficher son tableau de bord</param>
       /// <returns></returns>
        public ActionResult IndexBanque(string panel="",string acc="",string ges="",string struc="",string interf="",string port="",string page="")
        {
            if (Session["user"] == null) return RedirectToAction("Connexion", "Auth");

            ViewBag.pages = string.IsNullOrEmpty(page)?"tb": page;
            struc = (Session["user"] as CompteBanqueCommerciale).Structure.Nom;
            var banqueID = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);
            var banqueName = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueName(db);
            if (string.IsNullOrEmpty(banqueName))
            {
                try
                {
                    banqueName = db.Structures.Find(banqueID).Nom;
                }
                catch (Exception)
                {}
            }

           /// if (!string.IsNullOrEmpty(acc))
             if(!string.IsNullOrEmpty(ges))
                Session["ges"] = ges;

            ViewBag.navigation = "tab1";
            ViewBag.User = GetUser();
            ViewBag.userName = (Session["user"] as CompteBanqueCommerciale).NomComplet;
            ViewBag.banqueName = banqueName;
            ViewBag.struc = struc;
            Session["structure"] = struc;

            VariablGlobales model = new VariablGlobales(db, db.GetCompteBanqueCommerciales.Include("Structure").FirstOrDefault(u => u.UserName == User.Identity.Name) as ApplicationUser);
            var site = db.Structures.Find((Session["user"] as CompteBanqueCommerciale).IdStructure);

            if (string.IsNullOrEmpty(panel) && string.IsNullOrEmpty(interf) || panel=="pr")
                return View("Principal");

            if (ges == "Archivage")
                return RedirectToAction("BindingToHierarchicalStructure", "FileManage");

            if (panel == "wf" && site.NiveauDossier == 1)
            {
                ///return RedirectToAction("Panel");
                panel = "1,2,17,19";
                acc = "Accueil agence";
                //struc = "agence";
            }
            else
            {
                if (site.NiveauDossier == 3)
                {
                    panel = "3,4,16,20";
                    acc = "Accueil conformité";
                   // struc = "Conformité";
                }
                else if (site.NiveauDossier == 5)
                {
                    panel = "5,6,7,8,9,10";
                    acc = "Accueil service transfert";
                    //struc = "Service transfert";
                }
            }

            if (!string.IsNullOrEmpty(struc))
            {
                Session["acc"] = struc; 
            }
            Session["str3"] = panel;
            string[] tab = panel.Split(',');
            Session["menu"] = tab[0];

            //moteur d'obtension des dossiers de l'utilisateur

            IDictionary<int, InfoDocAcueil> _model = null;
            if(string.IsNullOrEmpty(port))
                 _model = model.infoDocBanque(Session["user"] as CompteBanqueCommerciale,7, getDossiersList:true,getDossiersListNum:8);
            else
            {
                try
                {
                    var autre = db.GetCompteBanqueCommerciales.Find(port);
                    _model = model.infoDocBanque(autre, 7, getDossiersList: true,getDossiersListNum:8);
                    ViewBag.autreUtilisateur = " /"+autre.NomComplet;
                }
                catch (Exception)
                {}
            }

            var dd = db.GetComposants.Include("Groupe").Include("Action").Where(c=>c.Localistion==Localistion.accueil && (c.Type==Models.Type.lien_bouton || c.Type==Models.Type.liste)).OrderBy(c=>c.IdGroupe).ToList();
            var idRole = (Session["user"] as CompteBanqueCommerciale).IdXRole;
            var role = db.XtraRoles.FirstOrDefault(x=>x.RoleId==idRole);
            if (role == null)
                return RedirectToAction("Login", "auth");

            //dictionnaire des url
            if (Session["urls"]==null)
            {
                //initialisation dictionnaire d'urls
               // Session["urls"] = new Dictionary<string, string>();
            }


            int _nbr = 0; double _percent = 0; DateTime date = default;
            int traite = 0, traite_dfx = 0, traite_ref = 0, rejet = 0, rejet_dfx = 0,
                rejet_ref = 0, dfx = 0, dfx_recu = 0,
                refinancement = 0, refinancement_recu = 0;

            List<Composant> composants = new List<Composant>();
            InfoDocAcueil infoDocAcueil = null;
            
            if (!(Session["user"] as CompteBanqueCommerciale).EstAdmin)
            {
               
                foreach (var elt in tab)
                //foreach (var item in site.Composants)
                {
                    try
                    {
                        //etapes = elt.Split(',');

                        // var item = site.Composants.FirstOrDefault(c => c.Composant.Numero == int.Parse(etapes[0]));
                        int eta = int.Parse(elt);
                        //var item = db.GetComposants.FirstOrDefault(c => c.Numero == eta && c.NumeroMax>=site.NiveauDossier);
                        foreach (var item in db.GetComposants.Where(c => c.EstActif &&  c.Numero == eta && c.NumeroMax >= site.NiveauDossier))
                        {
                            if (item != null)
                            {
                                //try
                                //{
                                //    if (role.GetIHMs != null || role.GetIHMs.Count() > 0 || role.GetIHMs.FirstOrDefault(i => i.ComposantId == item.Id) != null)
                                //        continue;
                                //}
                                //catch (Exception)
                                //{ }
                                //if (item.Composant.Type == genetrix.Models.Type.lien_bouton)
                                if (item.Type == genetrix.Models.Type.lien_bouton)
                                {
                                    infoDocAcueil = new InfoDocAcueil();
                                    //composants.Add(item.Composant);
                                    composants.Add(item);
                                    try
                                    {
                                         _nbr = 0;  _percent = 0; date = default;
                                         traite = 0; traite_dfx = 0; traite_ref = 0; rejet = 0; rejet_dfx = 0;
                                            rejet_ref = 0; dfx = 0; dfx_recu = 0;
                                            refinancement = 0; refinancement_recu = 0;

                                        //for (int i = item.NumeroMin; i <= item.NumeroMax; i++)
                                        foreach (var i in item.DiscontinusListe)
                                        {
                                            if (_model.Keys != null && _model.Keys.Contains(i))
                                            {
                                                _nbr += _model[i].Nbr;
                                                _percent += _model[i].GetPercentage();
                                                date = _model[i].Date;

                                                rejet = _model[i].NbrRejets;
                                                rejet_dfx = _model[i].NbrRejets_DFX;
                                                rejet_ref = _model[i].NbrRejets_Refinancement;
                                                traite = _model[i].NbrValidés;
                                                traite_dfx = _model[i].NbrValidés_DFX;
                                                traite_ref = _model[i].NbrValidés_Refinancement;
                                                dfx = _model[i].NbrDFX;
                                                dfx_recu = _model[i].NbrDFX_recu;
                                                refinancement = _model[i].NbrRefinanacement;
                                                refinancement_recu = _model[i].NbrRefinanacement_recu;
                                            }
                                        }
                                        infoDocAcueil = new InfoDocAcueil()
                                        {
                                            Date = date,
                                            Nbr = _nbr,
                                            Percentage = _percent,

                                            NbrRejets = rejet,
                                            NbrRejets_DFX = rejet_dfx,
                                            NbrRejets_Refinancement = rejet_ref,
                                            NbrValidés = traite,
                                            NbrValidés_DFX = traite_dfx,
                                            NbrValidés_Refinancement = traite_ref,
                                            NbrDFX = dfx,
                                            NbrDFX_recu = dfx_recu,
                                            NbrRefinanacement = refinancement,
                                            NbrRefinanacement_recu = refinancement_recu,
                                        };
                                    }
                                    catch (Exception)
                                    { }
                                    //ViewData["e" + item.Composant.Numero + "_" + item.Composant.NumeroMax] = infoDocAcueil;
                                    ViewData["e_" + item.NumeroMin + "_" + item.NumeroMax] = infoDocAcueil;
                                }
                            } 
                        }
                    }
                    catch (Exception)
                    {}
                }
            }
            else
            {
                //composants.AddRange(db.GetComposants);
                 traite = 0; traite_dfx = 0; traite_ref = 0; rejet = 0; rejet_dfx = 0;
                    rejet_ref = 0; dfx = 0; dfx_recu = 0;
                    refinancement = 0; refinancement_recu = 0;
                foreach (var item in db.GetComposants.Where(c=>c.EstActif))
                {
                    infoDocAcueil = new InfoDocAcueil();
                    try
                    {
                        composants.Add(item);
                        if (item.NumeroMax > item.Numero)
                        {
                            _nbr = 0; _percent = 0; date = default;
                            for (int i = item.Numero; i < item.NumeroMax; i++)
                            {
                                if (_model.Keys != null && _model.Keys.Contains(i))
                                {
                                    _nbr += _model[i].Nbr;
                                    _percent += _model[12].GetPercentage();
                                    date = _model[i].Date;

                                    rejet = _model[i].NbrRejets;
                                    rejet_dfx = _model[i].NbrRejets_DFX;
                                    rejet_ref = _model[i].NbrRejets_Refinancement;
                                    traite = _model[i].NbrValidés;
                                    traite_dfx = _model[i].NbrValidés_DFX;
                                    traite_ref = _model[i].NbrValidés_Refinancement;
                                    dfx = _model[i].NbrDFX;
                                    dfx_recu = _model[i].NbrDFX_recu;
                                    refinancement = _model[i].NbrRefinanacement;
                                    refinancement_recu = _model[i].NbrRefinanacement_recu;
                                }
                            }
                            infoDocAcueil= new InfoDocAcueil()
                            {
                                Date = date,
                                Nbr = _nbr,
                                Percentage = _percent,

                                NbrRejets = rejet,
                                NbrRejets_DFX = rejet_dfx,
                                NbrRejets_Refinancement = rejet_ref,
                                NbrValidés = traite,
                                NbrValidés_DFX = traite_dfx,
                                NbrValidés_Refinancement = traite_ref,
                                NbrDFX = dfx,
                                NbrDFX_recu = dfx_recu,
                                NbrRefinanacement = refinancement,
                                NbrRefinanacement_recu = refinancement_recu,

                            };
                        }
                        else
                        {
                            if (_model.Keys != null && _model.Keys.Contains(item.Numero))
                            {
                                infoDocAcueil= new InfoDocAcueil()
                                {
                                    Date = _model[item.Numero].Date,
                                    Nbr = _model[item.Numero].Nbr,
                                    Percentage = _model[item.Numero].GetPercentage(),

                                    NbrRejets = _model[item.Numero].NbrRejets,
                                    NbrRejets_DFX = _model[item.Numero].NbrRejets_DFX,
                                    NbrRejets_Refinancement = _model[item.Numero].NbrRejets_Refinancement,
                                    NbrValidés = _model[item.Numero].NbrValidés,
                                    NbrValidés_DFX = _model[item.Numero].NbrValidés_DFX,
                                    NbrValidés_Refinancement = _model[item.Numero].NbrValidés_Refinancement,
                                    NbrDFX = _model[item.Numero].NbrDFX,
                                    NbrDFX_recu = _model[item.Numero].NbrDFX_recu,
                                    NbrRefinanacement = _model[item.Numero].NbrRefinanacement,
                                    NbrRefinanacement_recu = _model[item.Numero].NbrRefinanacement_recu
                            };
                            }
                        }
                    }
                    catch (Exception)
                    { }
                    ViewData["e" + item.Numero + "_" + item.NumeroMax] = infoDocAcueil;
                }
            }

            ViewData["composants"] = composants.OrderBy(c=>c.IdGroupe);
            ViewBag.nbrComposant= composants.Count;

            //affichage section
            ViewBag.panel = panel;
            dd = null;

            try
            {
                ViewBag.interf=  interf;
                //model.Banque = db.GetBanques.Find(banqueID);
            }
            catch (Exception)
            { }

            ViewData["page1"] = "acueil principal /"+";indexbanque?panel=";
            if ((Session["user"] as CompteBanqueCommerciale).EstAdmin)
            {
                return RedirectToAction("configurations","Banques",new { id=banqueID});
            }
            return View(model);
        }

        public ActionResult Panel()
        {
            try
            {
                ViewBag.banqueName = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueName(db);
                ViewBag.userEtape = (Session["user"] as CompteBanqueCommerciale).Structure.NiveauDossier;
            }
            catch (Exception)
            {}
            return View();
        }

        // GET: Index
        //[Authorize(Roles = "CompteClient")]
        public ActionResult Index()
        {
            if (Session["user"] == null) return RedirectToAction("Login", "Account");
            try
            {
                if (Session["Profile"].ToString()!="client") 
                    return RedirectToAction("Login", "Account");

            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.User= GetUser();
            VariablGlobales model = new VariablGlobales(db,db.Users.FirstOrDefault(u=>u.UserName==User.Identity.Name) as ApplicationUser);

            var _model = model.infoDoc2();

            //en cours
            InfoDocAcueil _encours = new InfoDocAcueil();
            try
            {
                int nbr = 0;double percent = 0;DateTime date = default;
                if (_model.Keys != null &&( _model.Keys.Contains(2) || _model.Keys.Contains(3)
                    || _model.Keys.Contains(4) || _model.Keys.Contains(5) || _model.Keys.Contains(6)
                    || _model.Keys.Contains(7)|| _model.Keys.Contains(-1)))
                {
                    nbr += _model[-1].Nbr;
                    for (int i = 2; i <=7; i++)
                    {
                        try
                        {
                            nbr += _model[i].Nbr;
                            percent += _model[i].GetPercentage();
                            date = _model[i].Date;
                        }
                        catch (Exception)
                        {}
                    }


                    _encours = new InfoDocAcueil()
                    {
                        Date = date,
                        Nbr = nbr,
                        //EtapeDossier = _model[2].EtapeDossier,
                        Percentage =nbr==0?0: percent /// nbr
                    };
                   
                }
            }
            catch (Exception)
            { }
            ViewData["_encours"] = _encours;
            _encours = null;
            //AApurer
            InfoDocAcueil _AApurer = new InfoDocAcueil();
            try
            {
                if (_model.Keys != null && _model.Keys.Contains(50))
                {
                    _AApurer = new InfoDocAcueil()
                    {
                        Date = _model[50].Date,
                        Nbr = _model[50].Nbr,
                        Percentage = _model[50].GetPercentage()
                    };
                }
            }
            catch (Exception)
            { }
            ViewData["_AApurer"] = _AApurer;
            _AApurer = null;

            //dossiers apurés
            InfoDocAcueil _Apuré = new InfoDocAcueil();
            try
            {
                if (_model.Keys != null && _model.Keys.Contains(51))
                {
                    _Apuré = new InfoDocAcueil()
                    {
                        Date = _model[51].Date,
                        Nbr = _model[51].Nbr,
                        Percentage = _model[51].GetPercentage()
                    };
                }
            }
            catch (Exception)
            { }

            ViewData["_Apuré"] = _Apuré;
            _Apuré = null;

            //dossiers achus
            InfoDocAcueil _Echus = new InfoDocAcueil();
            try
            {
                if (_model.Keys != null && _model.Keys.Contains(40))
                {
                    _Echus = new InfoDocAcueil()
                    {
                        Date = _model[40].Date,
                        Nbr = _model[40].Nbr,
                        Percentage = _model[40].GetPercentage()
                    };
                }
            }
            catch (Exception)
            { }
            ViewData["_Echus"] = _Echus;
            _Echus = null;

            //dossiers soumis
            InfoDocAcueil Soumis = new InfoDocAcueil();
            try
            {
                if (_model.Keys != null && _model.Keys.Contains(1))
                {
                    Soumis = new InfoDocAcueil()
                    {
                        Date = _model[1].Date,
                        Nbr = _model[1].Nbr,
                        Percentage = _model[1].GetPercentage()
                    };
                }
            }
            catch (Exception)
            { }
            ViewData["_Emission"] = Soumis;
            Soumis = null;

            //dossiers à soumettre
            InfoDocAcueil ASoum = new InfoDocAcueil();
            try
            {
                if (_model.Keys != null && _model.Keys.Contains(0))
                {
                    ASoum = new InfoDocAcueil()
                    {
                        Date = _model[0].Date,
                        Nbr = _model[0].Nbr,
                        Percentage = _model[0].GetPercentage()
                    }
                }
            }
            catch (Exception)
            { }

            ViewData["_ASoum"] = ASoum;
            ASoum = null;

            //brouillon
            InfoDocAcueil Bouillon = new InfoDocAcueil();
            try
            {
                if (_model.Keys != null && _model.Keys.Contains(null))
                {
                    Bouillon = new InfoDocAcueil()
                    {
                        Date = _model[null].Date,
                        Nbr = _model[null].Nbr,
                        Percentage = _model[null].GetPercentage()
                    };
                }
            }
            catch (Exception)
            { }

            ViewData["_Bouillon"] = Bouillon;
            Bouillon = null;
            try
            {
                var idclient = (Session["user"] as CompteClient).Client.Id;
                Session["nbr_add"] = db.GetClients.Include("Adresses").FirstOrDefault(c=>c.Id== idclient).Adresses.Count;
            }
            catch (Exception)
            {}

            return View(model);
        }
    }
}