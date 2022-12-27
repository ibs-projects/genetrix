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
using genetrix;
using System.Collections.Generic;
using WordToPDF;
using System.IO;
using GroupDocs.Editor.Options;
using genetrix.Models.Fonctions;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System.Timers;
using System.Net.Mail;
using System.Configuration;
using System.Net;
using SelectPdf;
using PdfDocument = SelectPdf.PdfDocument;

namespace genetrix.Controllers
{
    [Authorize]
    public class IndexController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // Send Email with zip as attachment.
        string emailsender = ConfigurationManager.AppSettings["emailsender"].ToString();
        string emailSenderPassword = ConfigurationManager.AppSettings["emailSenderPassword"].ToString();
        private ApplicationUserManager _userManager;

        DateTime dateNow;
        public IndexController()
        {
            dateNow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "W. Central Africa Standard Time");
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

        /// <summary>
        /// Mise à jour automatique des views
        /// </summary>
        /// <param name="typeData">Type de données</param>
        /// <param name="vc">Type de la view</param>
        /// <param name="idComponent">Id du composant à modifier</param>
        /// <returns></returns>
        public async Task<ActionResult> RealTimeData(string typeData, ViewComponent vc,string idComponent)
        {
            var data=VariablGlobales.UserInfos[User.Identity.GetUserId()].FirstOrDefault(s=>s.Type== typeData);
            if(data !=null)
            {
                data.ComponentId=idComponent;
                data.ComponentType=vc.ToString();
            }
            return Json(data,JsonRequestBehavior.AllowGet);
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

        public ActionResult Page500()
        {
            return View();
        }

        public ActionResult Page400()
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
        public ActionResult IndexBanque(string panel="",string acc="",string ges="",string struc="",string interf="",string port="",string page="",string navigation="")
        {
            if (Session == null) return RedirectToAction("Connexion", "Auth");
            Session["urlaccueil"] = Request.Url.AbsoluteUri;
            //acces workflow apurement
            if (!string.IsNullOrEmpty(panel) && panel == "apurement")
                ViewBag.comp = "apurement";

            ViewBag.pages = string.IsNullOrEmpty(page)?"tb": page;

            var strucId = Session["IdStructure"];
            var structure = db.Structures.Find(strucId);
            struc = structure.Nom;
            var banqueID = structure.BanqueId(db);
            var banque = db.GetBanques.Find(banqueID);
            var banqueName =structure.BanqueName(db);
            if (string.IsNullOrEmpty(banqueName))
            {
                try
                {
                    banqueName = db.Structures.Find(banqueID).Nom;
                }
                catch (Exception)
                {}
            }

            try
            {
                ViewBag.minSite = Convert.ToInt32(Session["userSIteMinNiveau"]);
                ViewBag.maxSite = Convert.ToInt32(Session["userSIteMaxNiveau"]);
            }
            catch (Exception)
            {}

            ViewBag.navigation =string.IsNullOrEmpty(navigation)? "tab1":navigation;
            ViewBag.User = GetUser();
            ViewBag.userName = Session["userName"];
            ViewBag.banqueName = banqueName;
            ViewBag.struc = struc;
            Session["structure"] = struc;

            VariablGlobales model = new VariablGlobales(db, db.GetCompteBanqueCommerciales.Include("Structure").FirstOrDefault(u => u.UserName == User.Identity.Name) as ApplicationUser);
            var site = db.Structures.Find(Session["IdStructure"]);

            //if (string.IsNullOrEmpty(panel) && string.IsNullOrEmpty(interf) || panel=="pr")
            if(structure.NiveauDossier==1 && string.IsNullOrEmpty(panel) && string.IsNullOrEmpty(interf)&& !Convert.ToBoolean(Session["EstBackOff"]) || (page=="principale" && structure.NiveauDossier == 1 && !Convert.ToBoolean(Session["EstBackOff"])))
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
                if (site.NiveauDossier == 6)
                {
                    panel = "3,4,16,20";
                    acc = "Accueil conformité";
                    ges = "Gestion de transfert";
                   // struc = "Conformité";
                }
                else if (site.NiveauDossier == 9)
                {
                    panel = "5,6,7,8,9,10";
                    acc = "Accueil service transfert";
                    ges = "Gestion de transfert";
                    //struc = "Service transfert";
                }
            }

            if (!string.IsNullOrEmpty(ges))
                Session["ges"] = ges;

            if (!string.IsNullOrEmpty(struc))
            {
                Session["acc"] = struc; 
            }
            Session["str3"] = panel;
            string[] tab = panel.Split(',');
            Session["menu"] = tab[0];

            //moteur d'obtension des dossiers de l'utilisateur

            IDictionary<int, InfoDocAcueil> _model = null;
            if (string.IsNullOrEmpty(port))
            {
                var user = db.GetCompteBanqueCommerciales.Find(Session["userId"]);
                //model.infoDocBanque2(user, banque, 7);
                if (ViewBag.comp == "apurement")
                    model.infoDocBanque3(user,db.GetDossiers.ToList(),banque: banque,id: 7);
                else
                    model.infoDocBanque2(user, db.GetDossiers.ToList(), banque, 7);
                user = null;
                model.DossiersBEAC = new List<ReferenceBanque>();
                try
                {
                    var refs = model.Dossiers.Where(d=>d.EtapesDosier<22 &&d.EtapesDosier> 14 ).GroupBy(r => r.ReferenceExterneId);
                    foreach (var r in refs)
                    {
                        try
                        {
                            model.DossiersBEAC.Add(db.GetReferenceBanques.Find((int)r.Key));
                        }
                        catch (Exception)
                        { }
                    }
                }
                catch (Exception)
                {}
            }
            else
            {
                try
                {
                    var autre = db.GetCompteBanqueCommerciales.Find(port);
                    if (ViewBag.comp == "apurement")
                        model.infoDocBanque3(autre, db.GetDossiers.ToList(), banque, 7);
                    else
                    model.infoDocBanque2(autre, db.GetDossiers.ToList(), banque, 7);
                    ViewBag.autreUtilisateur = " /" + autre.NomComplet;
                }
                catch (Exception)
                { }
            }

            var dd = db.GetComposants.Include("Groupe").Include("Action").Where(c=>c.Localistion==Localistion.accueil && (c.Type==Models.Type.lien_bouton || c.Type==Models.Type.liste)).OrderBy(c=>c.IdGroupe).ToList();
            var idRole = Convert.ToInt32(Session["IdXRole"]);
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
            
            if (!Convert.ToBoolean(Session["EstAdmin"]))
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
                                        if(_model!=null)
                                        foreach (var i in item.DiscontinusListe)
                                        {
                                            if (_model.Keys != null && _model.Keys.Contains(i))
                                            {
                                                _nbr += _model[i].Nbr;
                                                _percent += _model[i].GetPercentage();
                                                date = _model[i].Date.Value;

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
                                    date = _model[i].Date.Value;

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
            if (Convert.ToBoolean(Session["EstAdmin"]))
            {
                return RedirectToAction("configurations","Banques",new { id=banqueID});
            }
            //redirection de la page index
            switch (interf)
            {
                case "recu-eur"://redirection vers la page des euros reçus
                    interf = "Dossiers reçu (EURO)";
                    ViewBag.interf = interf;
                    ViewBag.etat = "recu";
                    return View("~/Views/Index/IndexBanque_euro.cshtml", model); 
                case "encours-eur"://redirection vers la page des euros reçus
                    interf = "Dossiers en cours (EURO)";
                    ViewBag.interf = interf;
                    ViewBag.etat = "encours";
                    return View("~/Views/Index/IndexBanque_euro.cshtml", model);
                case "recu-autr"://redirection vers la page autres devises
                    interf = "Dossiers reçu (Autres devises)";
                    ViewBag.interf = interf;
                    ViewBag.etat = "recu";
                    return View("~/Views/Index/IndexBanque_autre.cshtml", model);
                case "encours-autr"://redirection vers la page autres devises
                    interf = "Dossiers en cours (Autres devises)";
                    ViewBag.interf = interf;
                    ViewBag.etat = "encours";
                    return View("~/Views/Index/IndexBanque_autre.cshtml", model);
                default:
                    break;
            }
            if (ViewBag.comp == "apurement")
            {
                return View("~/Views/Index/ApurementView.cshtml", model) ;
            }

            return View(model);
        }

        public ActionResult Panel()
        {
            try
            {
                var user = db.GetCompteBanqueCommerciales.Find(Session["userId"]);
                ViewBag.banqueName = user.Structure.BanqueName(db);
                ViewBag.userEtape = user.Structure.NiveauDossier;
                user = null;
            }
            catch (Exception)
            {}
            return View();
        }

        // GET: Index
        //[Authorize(Roles = "CompteClient")]
        public ActionResult Index(string st="",string msg="",bool groupe=false)
        {
            if (Session == null) return RedirectToAction("Login", "Account");
            var idclient = Convert.ToInt32(Session["ClientId"]);
            Session["urlaccueil"] = Request.Url.AbsoluteUri;
            ViewBag.pages = "indexclient";
            if(!string.IsNullOrEmpty(msg))
            ViewBag.msg = msg;
            try
            {
                if (Session["Profile"].ToString()!="client") 
                    return RedirectToAction("Login", "Account");
                var cl= db.GetClients.Include("Adresses").FirstOrDefault(c=>c.Id== idclient);
                
                var addres= cl.Adresses.Count;
                try
                {
                    if (!string.IsNullOrEmpty(cl.Adresse))
                        addres++;
                }
                catch (Exception)
                { }
                Session["nbr_add"] = addres;
                cl = null;
            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Account");
            }
            var client= db.GetClients.Find(idclient);
            try
            {
                Session["client"] = client.Nom;
            }
            catch (Exception)
            {}
            try
            {
                var idsite= client.Banques.FirstOrDefault().IdSite;
                var idbanque = db.Structures.Find(idsite).BanqueId(db);
                Session["banqueName"] = db.GetBanques.Find(idbanque).Nom;
            }
            catch (Exception)
            {}
            ViewBag.navigation = "tb-principal";
            ViewBag.navigation_msg = "tb";
            ViewBag.User = GetUser();
            VariablGlobales model = new VariablGlobales(db, db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name) as ApplicationUser);
            ViewBag.comp = "transfert";
            if (!string.IsNullOrEmpty(st) && st == "apurement")
            {
                ViewBag.comp = "apurement"; 
                if(groupe)
                    model.infoDoc3(apurement:true);
                else
                    model.infoDoc2();
            }
            else
                model.infoDoc2();
            return View(model);
        }

        private static System.Timers.Timer aTimer;
        private int Interval= 14400000;
        private short HeureExecuteDebut = 0;
        private short HeureExecuteFin = 4;

        public ActionResult MiseDemeureAction(bool stop=false,int interval=43200,short heureExecuteDebut=0, short heureExecuteFin=4)
        {
            var structure = db.Structures.Find(Session["IdStructure"]);
            var idbanque = structure.BanqueId(db);
            var banque = db.GetBanques.Find(idbanque);
            var msg = "";
            if (aTimer==null)
            {
                try
                {

                    // Create a timer with a two second interval.
                    aTimer = new System.Timers.Timer(interval);//4H
                    // Hook up the Elapsed event for the timer. 
                    aTimer.Elapsed += OnTimedEvent;
                    aTimer.AutoReset = true;
                }
                catch (Exception)
                { }
            }
            //if (aTimer!=null)
            if (!banque.Activetimer)
            {
                banque.StopDataSynchrone = true;
                banque.DateExecuteFin = null;
                banque.DateExecuteDebut = dateNow;
                banque.Activetimer = true;
                aTimer.Interval = 10000;// interval;
                HeureExecuteDebut = heureExecuteDebut;
                HeureExecuteFin = heureExecuteFin;
                db.SaveChanges();
                aTimer.Enabled = true;
                msg = "Lecture données d\'arrière plan activée";
            }
            else
            {
                banque.StopDataSynchrone = false;
                banque.DateExecuteFin = dateNow;
                banque.DateExecuteDebut = null;
                banque.Activetimer = false;
                db.SaveChanges();
                aTimer.Enabled = false;
                aTimer.Stop();
                msg = "Lecture données d\'arrière plan arrée";
            }

            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        private string TimeSinceAsync;
        public async Task<ActionResult> GetTimeSinceAsync()
        {
            return Json(TimeSinceAsync, JsonRequestBehavior.AllowGet);
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            TimeSinceAsync = e.SignalTime.ToString();
            //foreach (var item in db.GetDossiers.ToList().OrderBy(d=>d.ClientId))
            //{
            //    GenerateMiseEnDemeure(item);
            //    break;
            //}
            //apurement echu
            if (DateTime.Now.Hour >= HeureExecuteDebut && DateTime.Now.Hour< HeureExecuteFin)
            {
                foreach (var item in db.GetDossiers.Where(d=>true || (d.EtapesDosier==23 || d.EtapesDosier==25 )&& 
                (d.NatureOperation==NatureOperation.Bien && d.Date_Etape23.Value.Day<30 
                ||d.NatureOperation==NatureOperation.Service && d.Date_Etape23.Value.Day<14)).ToList().OrderBy(d=>d.ClientId))
                {
                    GenerateMiseEnDemeure(item);
                }
            }
        }


        private void GenerateMiseEnDemeure(Dossier dossier)
        {
            if (dossier!=null)
            {
                
                using (MemoryStream stream = new System.IO.MemoryStream())
                {
                    string FromLocation = "", destination = "";
                    string ToLocation = "";
                    FromLocation = this.Server.MapPath("~/mise_en_demeure.doc");
                    destination = this.Server.MapPath("mise_en_demeure2.pdf");
                    ToLocation = this.Server.MapPath("mise_en_demeure.pdf");

                    Dictionary<string, string> texts = new Dictionary<string, string>();
                    texts.Add("gesName", dossier.GestionnaireName);
                    texts.Add("agenceName", dossier.GetAgenceName);
                    texts.Add("agenceAdresse", dossier.GetAgenceAdresse);
                    texts.Add("gesTel", dossier.GestionnairePhone);
                    texts.Add("gesMail", dossier.GestionnaireEmail);
                    texts.Add("gesVille", dossier.GestionnaireVille);
                    texts.Add("dateNow", DateTime.Now.ToString("dd/MM/yyyy"));

                    texts.Add("clientName", dossier.GetClient);
                    texts.Add("clientAdresse", dossier.AdresseClient);
                    texts.Add("clientPays", dossier.PaysClient);
                    texts.Add("clientMail", dossier.GetClientEmail);
                    texts.Add("clientTel", dossier.TelClient);

                    texts.Add("mntTrans", dossier.MontantString);
                    texts.Add("deviseTrans", dossier.DeviseToString);
                    texts.Add("fourniTrans", dossier.GetFournisseur);
                    var GridHtml = Fonctions.HtmlToPdf(texts, destination);
                    StringReader sr = new StringReader(GridHtml);
                    Document pdfDoc = new Document(PageSize.A4, 50f, 50f, 100f, 0f);
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                    pdfDoc.Open();
                    XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                    pdfDoc.Close();
                    //return File(stream.ToArray(), "application/pdf", "Grid.pdf");
                    //using (MailMessage mail = new MailMessage(emailsender, User.Identity.Name))
                    ///using (MailMessage mail = new MailMessage(emailsender, dossiers.First().GestionnaireEmail))
                    using (MailMessage mail = new MailMessage(emailsender, "bokokibitim@gmail.com"))
                    {
                        try
                        {
                            mail.Subject = "Transferts échus";
                            mail.Body = $"Le transfert du client NomClient est echu";

                            mail.Attachments.Add(new Attachment(new MemoryStream(stream.ToArray()), "MiseEndemeure.pdf"));

                            mail.IsBodyHtml = true;
                            SmtpClient smtp = new SmtpClient();
                            smtp.Host = ConfigurationManager.AppSettings["smtpserver"].ToString();
                            smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSSL"]);
                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = new NetworkCredential(emailsender, emailSenderPassword);
                            smtp.Port = Convert.ToInt16(ConfigurationManager.AppSettings["portnumber"]);
                            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                            smtp.Send(mail);
                        }
                        catch (Exception e)
                        { }

                    }

                } 
            }
        }

        public ActionResult BlenkPage()
        {
            return View();
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (Session != null)
            {
                if ((string)Session["userType"] == "CompteBanqueCommerciale")
                {
                    var url = (string)Session["urlaccueil"];
                    filterContext.Result = Redirect(url);
                }

                filterContext.Result = RedirectToAction("Index", "Index");
            }
        }


    }
}