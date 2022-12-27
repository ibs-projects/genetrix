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
using eApurement;
using System.Collections.Generic;

namespace eApurement.Controllers
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

        //[Authorize(Roles = "CompteBanqueCommerciale")]
        public ActionResult IndexBanque(string panel="",string acc="",string ges="",string struc="")
        {
            //var composants1 = db.XtraRoles.ToList();
            //foreach (var item in composants1)
            //{
            //    db.XtraRoles.Remove(item);
            //}
            //db.SaveChanges();

            //var actions = db.Actions.ToList();
            //foreach (var item in actions)
            //{
            //    db.Actions.Remove(item);
            //}
            //db.SaveChanges();

            if(struc=="agence")
                struc= (Session["user"] as CompteBanqueCommerciale).Structure.Nom;

           /// if (!string.IsNullOrEmpty(acc))
             if(!string.IsNullOrEmpty(ges))
                Session["ges"] = ges;

            ViewBag.navigation = "tab1";
            if (Session["user"] == null) return RedirectToAction("Connexion", "Auth");
            var banqueID = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);
            ViewBag.User = GetUser();
            ViewBag.userName = (Session["user"] as CompteBanqueCommerciale).NomComplet;

            VariablGlobales model = new VariablGlobales(db, db.GetCompteBanqueCommerciales.Include("Structure").FirstOrDefault(u => u.UserName == User.Identity.Name) as ApplicationUser);
            var site = db.Structures.Find((Session["user"] as CompteBanqueCommerciale).IdStructure);

            if (string.IsNullOrEmpty(panel) || panel=="pr")
                return View("Principal");

            if (ges == "Archivage")
                return RedirectToAction("BindingToHierarchicalStructure", "FileManage");

            if (panel=="wf" && site.NiveauDossier<3)
                return RedirectToAction("Panel");
            else
            {
                if (site.NiveauDossier==3)
                {
                    panel = "3,4,16,20";
                    acc = "Accueil conformité";
                    struc = "Conformité";
                }else if (site.NiveauDossier==5)
                {
                    panel = "5,6,7,8,9,10";
                    acc = "Accueil service transfert";
                    struc = "Service transfert";
                }
            }

            if (!string.IsNullOrEmpty(struc))
            {
                Session["acc"] = struc; 
            }
            Session["str3"] = panel;
            string[] tab = panel.Split(',');
            Session["menu"] = tab[0];

            var _model = model.infoDocBanque(7);
            
            #region Old
            //// dossiers reçcu
            //InfoDocAcueil Soumis = new InfoDocAcueil();
            //try
            //{
            //    if (_model.Keys != null && _model.Keys.Contains(1))
            //    {
            //        Soumis = new InfoDocAcueil()
            //        {
            //            Date = _model[1].Date,
            //            Nbr = _model[1].Nbr,
            //            Percentage = _model[1].GetPercentage()
            //        };
            //    }
            //}
            //catch (Exception)
            //{ }
            //ViewData["e1"] = Soumis;
            //Soumis = null;

            ////en cours
            //InfoDocAcueil _encours = new InfoDocAcueil();
            //try
            //{
            //    if (Session["Profile"].ToString() == "banque")
            //    {
            //        int nbr = 0; double percent = 0; DateTime date = default;


            //        int i = 2;
            //        //for ( i = 2; i <= 6; i++)
            //        {
            //            try
            //            {
            //                nbr += _model[i].Nbr;
            //                percent += _model[i].GetPercentage();
            //                date = _model[i].Date;
            //            }
            //            catch (Exception)
            //            { }
            //        }


            //        _encours = new InfoDocAcueil()
            //        {
            //            Date = date,
            //            Nbr = nbr,
            //            //EtapeDossier = _model[2].EtapeDossier,
            //            Percentage = nbr == 0 ? 0 : percent / nbr
            //        };

            //        ViewData["e2"] = _encours;
            //        _encours = null;



            //        // dossiers en cours d'analyse conformité
            //        InfoDocAcueil encoourconformite = new InfoDocAcueil();
            //        try
            //        {
            //            if (_model.Keys != null && _model.Keys.Contains(3))
            //            {
            //                encoourconformite = new InfoDocAcueil()
            //                {
            //                    Date = _model[3].Date,
            //                    Nbr = _model[3].Nbr,
            //                    Percentage = _model[3].GetPercentage()
            //                };
            //            }
            //        }
            //        catch (Exception)
            //        { }
            //        ViewData["e3"] = encoourconformite;
            //        encoourconformite = null;

            //        // dossiers en cours d'analyse stf
            //        InfoDocAcueil encours_stf = new InfoDocAcueil();
            //        try
            //        {
            //            if (_model.Keys != null && _model.Keys.Contains(4))
            //            {
            //                encours_stf = new InfoDocAcueil()
            //                {
            //                    Date = _model[4].Date,
            //                    Nbr = _model[4].Nbr,
            //                    Percentage = _model[4].GetPercentage()
            //                };
            //            }
            //        }
            //        catch (Exception)
            //        { }
            //        ViewData["e4"] = encours_stf;
            //        encours_stf = null;

            //        // dossiers en attente transmission beac
            //        InfoDocAcueil attentetransmissionbeac = new InfoDocAcueil();
            //        try
            //        {
            //            if (_model.Keys != null && _model.Keys.Contains(5))
            //            {
            //                attentetransmissionbeac = new InfoDocAcueil()
            //                {
            //                    Date = _model[5].Date,
            //                    Nbr = _model[5].Nbr,
            //                    Percentage = _model[5].GetPercentage()
            //                };
            //            }
            //        }
            //        catch (Exception)
            //        { }
            //        ViewData["e5"] = attentetransmissionbeac;
            //        attentetransmissionbeac = null;


            //        // dossiers attente couverture beac
            //        InfoDocAcueil atten_couv = new InfoDocAcueil();
            //        try
            //        {
            //            if (_model.Keys != null && _model.Keys.Contains(6))
            //            {
            //                atten_couv = new InfoDocAcueil()
            //                {
            //                    Date = _model[6].Date,
            //                    Nbr = _model[6].Nbr,
            //                    Percentage = _model[6].GetPercentage()
            //                };
            //            }
            //        }
            //        catch (Exception)
            //        { }
            //        ViewData["e6"] = atten_couv;
            //        atten_couv = null;

            //        // Saisie en cours
            //        InfoDocAcueil saisieencours = new InfoDocAcueil();
            //        try
            //        {
            //            if (_model.Keys != null && _model.Keys.Contains(7))
            //            {
            //                saisieencours = new InfoDocAcueil()
            //                {
            //                    Date = _model[7].Date,
            //                    Nbr = _model[7].Nbr,
            //                    Percentage = _model[7].GetPercentage()
            //                };
            //            }
            //        }
            //        catch (Exception)
            //        { }
            //        ViewData["e7"] = saisieencours;
            //        saisieencours = null;



            //        // dossiers accordés
            //        InfoDocAcueil execute = new InfoDocAcueil();
            //        try
            //        {
            //            if (_model.Keys != null && _model.Keys.Contains(8))
            //            {
            //                execute = new InfoDocAcueil()
            //                {
            //                    Date = _model[8].Date,
            //                    Nbr = _model[8].Nbr,
            //                    Percentage = _model[8].GetPercentage()
            //                };
            //            }
            //        }
            //        catch (Exception)
            //        { }
            //        ViewData["e8"] = execute;
            //        execute = null;

            //        // dossiers non accordés
            //        InfoDocAcueil nonaccordes = new InfoDocAcueil();
            //        try
            //        {
            //            if (_model.Keys != null && _model.Keys.Contains(9))
            //            {
            //                nonaccordes = new InfoDocAcueil()
            //                {
            //                    Date = _model[9].Date,
            //                    Nbr = _model[9].Nbr,
            //                    Percentage = _model[9].GetPercentage()
            //                };
            //            }
            //        }
            //        catch (Exception)
            //        { }
            //        ViewData["e9"] = nonaccordes;
            //        execute = null;
            //    }
            //    else
            //    {
            //        if (_model.Keys != null && (_model.Keys.Contains(2) || _model.Keys.Contains(3)
            //        || _model.Keys.Contains(4) || _model.Keys.Contains(5) || _model.Keys.Contains(6)
            //        || _model.Keys.Contains(7) || _model.Keys.Contains(8)))//_model.Keys.Contains(2))
            //        {
            //            _encours = new InfoDocAcueil()
            //            {
            //                Date = _model[2].Date,
            //                Nbr = _model[2].Nbr,
            //                Percentage = _model[2].GetPercentage()
            //            };

            //        }
            //    }
            //}
            //catch (Exception)
            //{ }
            ////AApurer
            //InfoDocAcueil _AApurer = new InfoDocAcueil();
            //try
            //{
            //    if (_model.Keys != null && _model.Keys.Contains(10))
            //    {
            //        _AApurer = new InfoDocAcueil()
            //        {
            //            Date = _model[10].Date,
            //            Nbr = _model[10].Nbr,
            //            Percentage = _model[10].GetPercentage()
            //        };
            //    }
            //}
            //catch (Exception)
            //{ }
            //ViewData["e10"] = _AApurer;
            //_AApurer = null;

            //// apuré
            //InfoDocAcueil _Apuré = new InfoDocAcueil();
            //try
            //{
            //    if (_model.Keys != null && _model.Keys.Contains(11))
            //    {
            //        _Apuré = new InfoDocAcueil()
            //        {
            //            Date = _model[11].Date,
            //            Nbr = _model[11].Nbr,
            //            Percentage = _model[11].GetPercentage()
            //        };
            //    }
            //}
            //catch (Exception)
            //{ }

            //ViewData["e11"] = _Apuré;
            //_Apuré = null;

            //// dossiers echus
            //InfoDocAcueil _Echus = new InfoDocAcueil();
            //try
            //{
            //    if (_model.Keys != null && _model.Keys.Contains(12))
            //    {
            //        _Echus = new InfoDocAcueil()
            //        {
            //            Date = _model[12].Date,
            //            Nbr = _model[12].Nbr,
            //            Percentage = _model[12].GetPercentage()
            //        };
            //    }
            //}
            //catch (Exception)
            //{ }
            //ViewData["e12"] = _Echus;
            //_Echus = null;

            //// dossiers conf st
            //InfoDocAcueil conf_st = new InfoDocAcueil();
            //try
            //{
            //    int _nbr = 0; double _percent = 0; DateTime date = default;
            //    for (int i = 2; i < 12; i++)
            //    {
            //        if (_model.Keys != null && _model.Keys.Contains(i))
            //        {
            //            _nbr += _model[i].Nbr;
            //            _percent += _model[12].GetPercentage();
            //            date = _model[i].Date;
            //        }
            //    }
            //    conf_st = new InfoDocAcueil()
            //    {
            //        Date = date,
            //        Nbr = _nbr,
            //        Percentage = _percent
            //    };
            //}
            //catch (Exception)
            //{ }
            //ViewData["e16"] = conf_st;
            //conf_st = null;
            #endregion

            var dd = db.GetComposants.Include("Groupe").Include("Action").Where(c=>c.Localistion==Localistion.accueil && (c.Type==Models.Type.lien_bouton || c.Type==Models.Type.liste)).OrderBy(c=>c.IdGroupe).ToList();
            var idRole = (Session["user"] as CompteBanqueCommerciale).IdXRole;
            var role = db.XtraRoles.FirstOrDefault(x=>x.RoleId==idRole);
            if (role == null)
                return RedirectToAction("Login", "auth");

            //exemple donnée panel=40,4,2,3:15,4,5,6:16,1,2,4
            // string[] tab=panel.split(':');
            //tab[0]=40,4,2,3 avec 40 n° composant, 4, 2, 3 l'etape des dossiers prise en charge

            //string[] etapes = null;

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
                                //if (item.Composant.Type == eApurement.Models.Type.lien_bouton)
                                if (item.Type == eApurement.Models.Type.lien_bouton)
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
            dd = null;

            try
            {
                Session["userSIteMinNiveau"] = site.NiveauDossier;
                Session["userSIteMaxNiveau"] = site.NiveauMaxDossier;
            }
            catch (Exception)
            { }

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

            var _model = model.infoDoc();

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
                        Percentage = _model[1].GetPercentage()
                    };
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

            return View(model);
        }
    }
}