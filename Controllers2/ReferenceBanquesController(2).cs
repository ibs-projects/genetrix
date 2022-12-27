using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using e_apurement.Models;
using eApurement.Models.Fonctions;
using eApurement.Models;

namespace eApurement.Controllers
{
    public class ReferenceBanquesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ReferenceBanques
        public ActionResult Index(bool reference=false,string st="")
        {

            ViewBag.navigation = "ref";
            ViewBag.navigation_msg = "liste references";
            ViewBag.navigation = string.IsNullOrEmpty(st) ? "aenv_bac" : st;
            //if (string.IsNullOrEmpty(st))
            //    return View(new List<ReferenceBanque>());
            ICollection<ReferenceBanque> getReferenceBanques = null;

            var banqueID = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);
            var role = db.XtraRoles.Find((Session["user"] as CompteBanqueCommerciale).IdXRole);
            var site = db.Agences.Find((Session["user"] as CompteBanqueCommerciale).IdStructure);
            var gesId = (Session["user"] as CompteBanqueCommerciale).Id;
            if (role == null)
                return RedirectToAction("Login", "auth");
            var rr = VariablGlobales.UserBanqueReferences(db, site, role, banqueID, gesId);

            if (!reference)
            {
                //getReferenceBanques = db.GetReferenceBanques.Include(r => r.Dossiers).Include(r => r.Banque).ToList();
                getReferenceBanques = rr.ToList();
            }
            else
            {
                getReferenceBanques = new List<ReferenceBanque>();
                //foreach (var re in db.GetReferenceBanques.Include(r => r.Dossiers).Include(r => r.Banque))
                foreach (var re in rr)
                {
                    if (re.Dossiers.Count > 0)
                        getReferenceBanques.Add(re);
                }
            }
            rr = null;
            var dd = getReferenceBanques.ToList();
            getReferenceBanques = new List<ReferenceBanque>();
            switch (st)
            {
                case "arch":
                    foreach (var r in dd)
                    {
                        if (r.EtapesDosier == 13)
                            getReferenceBanques.Add(r);
                    }
                    ViewBag.info = " archivées";
                    break;
                case "list":
                    foreach (var r in dd)
                    {
                        if (r.EtapesDosier != 13)
                            getReferenceBanques.Add(r);
                    }
                    ViewBag.info = "";
                    break;
                case "lbre":
                    foreach (var re in dd)
                        try
                        {
                            if (re.Dossiers.Count == 0)
                                getReferenceBanques.Add(re);
                        }
                        catch (Exception)
                        {}
                    ViewBag.info = " libres";
                    break; 
                case "env_bac":
                    foreach (var r in dd)
                    {
                        try
                        {
                            if (r.EtapesDosier == 6)
                                getReferenceBanques.Add(r);
                        }
                        catch (Exception)
                        {}
                    }
                    ViewBag.info = " envoyés à la BEAC";
                    break; 
                case "à envoyer BEAC":
                case "aenv_bac":
                    foreach (var r in dd)
                    {
                        try
                        {
                            if (r.EtapesDosier == 5)
                                getReferenceBanques.Add(r);
                        }
                        catch (Exception)
                        {}
                    }
                    ViewBag.info = " à envoyer à la BEAC";
                    break;
                case "accord":
                    foreach (var r in dd)
                    {
                        try
                        {
                            if (r.EtapesDosier == 7 || r.EtapesDosier == 8)
                                getReferenceBanques.Add(r);
                        }
                        catch (Exception)
                        {}
                    }
                    ViewBag.info = " accordés";
                    break;
                case "liee":
                    foreach (var re in dd)
                        try
                        {
                            if (re.Dossiers.Count > 0)
                                getReferenceBanques.Add(re);
                        }
                        catch (Exception)
                        {}
                    ViewBag.info = " liées";
                    break;
                default:
                    foreach (var r in dd)
                    {
                        try
                        {
                            if (r.EtapesDosier != 13)
                                getReferenceBanques.Add(r);
                        }
                        catch (Exception)
                        {}
                    }
                    ViewBag.info = " libres";
                    break;
            }
            //ViewBag.statut = st.ToString();
            dd = null;
            return View(getReferenceBanques.ToList());
        }

        public async Task<ActionResult> Reflesh()
        {

            var idbanque = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);
            var fournisseur = "";
            var nomclient = "";
            int count = 0;
            var dd = await db.GetReferenceBanques.Where(r => r.BanqueId == idbanque).Include(r => r.Dossiers).ToListAsync();
            foreach (var re in dd)
            {
                if (re.EtapesDosier == 8)
                {
                    foreach (var d in re.Dossiers)
                    {
                        d.EtapesDosier = 49;
                        count++;
                    }
                    if (count > 0)
                    {
                        fournisseur = re.Dossiers.FirstOrDefault().Fournisseur.Nom;
                        nomclient = re.Dossiers.FirstOrDefault().Client.Nom;
                    }
                    try
                    {
                        db.SaveChanges();
                        MailFunctions.EnvoiMail(re, fournisseur, nomclient, (int)idbanque, "Référence en attente d'apurement", " est en attente d'apurement depuis le "+re.DateReception,db);
                    }
                    catch (Exception)
                    { }

                }
                else if (re.EtapesDosier == 11)
                {
                    switch (re.NATURE.ToLower())
                    {
                        case "service":
                            if((DateTime.Now- (DateTime)re.DateReception).Days>=30)
                                foreach (var d in re.Dossiers)
                                {
                                    d.EtapesDosier = 12;
                                }
                            MailFunctions.EnvoiMail(re, fournisseur, nomclient, (int)idbanque,"Dossier(s) échus"," est à arrivée à echéance",db);
                            break;
                        case "bien":
                            if ((DateTime.Now - (DateTime)re.DateReception).Days >= 90)
                                foreach (var d in re.Dossiers)
                                {
                                    d.EtapesDosier = 12;
                                }
                            MailFunctions.EnvoiMail(re, fournisseur, nomclient, (int)idbanque, "Dossier(s) échus", " est à arrivée à echéance",db);
                            break;
                        default:
                            break;
                    }
                }
            }
            return RedirectToAction("Index");
        }

        // GET: ReferenceBanques/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReferenceBanque referenceBanque = await db.GetReferenceBanques.Include(r=>r.Dossiers).FirstOrDefaultAsync(r=>r.Id==id);
            if (referenceBanque == null)
            {
                return HttpNotFound();
            }
            //if (referenceBanque.EtapesDosier ==null )
            //{
            //    var dd = db.GetDossiers;
            //    foreach (var d in dd)
            //    {
            //        d.ReferenceExterneId = null;
            //        d.ReferenceExterne = null;
            //        d.EtapesDosier = 2;
            //    }
            //    db.SaveChanges();
            //}
            ViewBag.navigation = "ref";
            ViewBag.navigation_msg = "details reference";
            return View(referenceBanque);
        }

        // GET: ReferenceBanques/Create
        public ActionResult Create()
        {
            ViewBag.BanqueId= new SelectList(db.GetBanques, "Id", "Nom");
            ViewBag.navigation = "ref";
            ViewBag.navigation_msg = "creation reference";
            return View();
        }

        // POST: ReferenceBanques/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "NumeroRef,DateReception,DepotBEAC,DélaitT,NOTIFICATION,DateNotif,MT99,DateMT99,DateMT202,DateCredit,DateTraité,ObsTransmission,ObsBEAC,NATURE,CvEURO")] ReferenceBanque referenceBanque)
        {
            if (ModelState.IsValid)
            {
                var banqueId = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);
                referenceBanque.BanqueId= (int)banqueId;
                db.GetReferenceBanques.Add(referenceBanque);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            string desc = "";
            foreach (ModelState modelState in ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    desc = desc + " " + error.ErrorMessage.ToString();
                }
            }
            ViewBag.msg = desc;
            ViewBag.BanqueId= new SelectList(db.GetBanques, "Id", "Nom", referenceBanque.BanqueId);
            return View(referenceBanque);
        }

        // GET: ReferenceBanques/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReferenceBanque referenceBanque = await db.GetReferenceBanques.FindAsync(id);
            if (referenceBanque == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "ref";
            ViewBag.navigation_msg = "edition reference";
            ViewBag.BanqueId = new SelectList(db.GetBanques, "Id", "Nom", referenceBanque.BanqueId);
            return View(referenceBanque);
        }


        // GET: ReferenceBanques/attache
        public ActionResult Attache()
        {
            ViewBag.ReferenceId = new SelectList(db.GetReferenceBanques.ToList(), "Id", "NumeroRef");

            var siteID = (Session["user"] as CompteBanqueCommerciale).IdStructure;
            var dossiers = new List<Dossier>();
            db.GetDossiers.Include(d => d.Site).Include(d => d.DeviseMonetaire).Include(d => d.ReferenceExterne).ToList().ForEach(d =>
            {
                if (d.IdSite == siteID && d.EtapesDosier != 13 && d.EtapesDosier >= 4)
                    dossiers.Add(d);
            });

            ViewBag.statut = "/ Attacher les références";
            ViewBag.navigation = "ref";
            ViewBag.navigation_msg = "attache reference";
            return View(dossiers.OrderBy(d => d.ClientId).ToList());
        }

        [HttpPost]
        public async Task<ActionResult> Attachement(FormCollection form)
        {
            Dossier dossier = null;
            int dossierId = 0,i=0;
            string content = "",clientmail="";
            ReferenceBanque referenceBanque = null;
            Dictionary<ReferenceBanque, string> refContent = new Dictionary<ReferenceBanque, string>();

            if (form.Keys.Count > 0)
            {
                foreach (var k in form.Keys)
                {
                    try
                    {
                        dossierId = int.Parse(k.ToString());
                        dossier = db.GetDossiers.Find(dossierId);
                        if (dossier.EtapesDosier == 4)
                        {
                            clientmail = dossier.Client.Email;
                            dossier.ReferenceExterneId = int.Parse(form[k.ToString()]);
                            dossier.EtapesDosier = 5;
                            await db.SaveChangesAsync();
                            i++;
                            content = "";
                            content = "<tr> "
                                + "<td> "
                                + dossier.NumeroDossier
                                + "</td>"
                                + "<td> "
                                + dossier.Montant
                                + "</td>"
                                + "<td> "
                                + dossier.DeviseMonetaire.Nom
                                + "</td>"
                                + "<td> "
                                + dossier.Fournisseur.Nom
                                + "</td>"
                                + "<td> "
                                + dossier.DateDepotBank
                                + "</td>"
                                + "<td> "
                                + dossier.Traité
                                + "</td>"
                                + " "
                                + "<td> "
                                + dossier.Site.Nom
                                + "</td>"
                                + " "
                                + "<td> "
                                + dossier.Site.DirectionMetier.Banque.Nom
                                + "</td>"
                                + "</tr>";
                            if (refContent.Keys.Contains(dossier.ReferenceExterne))
                            {
                                refContent[dossier.ReferenceExterne] += content;
                            }
                            else
                            {
                                refContent.Add(dossier.ReferenceExterne, content);
                            }

                        }
                    }
                    catch (Exception ee)
                    { }
                }
                try
                {
                    foreach (var key in refContent.Keys)
                    {

                        var msg = "<h3 style=\"background-color:blue;color:white;padding:8px;text-align:center\"> Attribution de référence </h3>"
                                    + "<p>Les dossiers suivants ont été liés à la référence <span style=\"color:black;font:bold\">" + key.NumeroRef + " </span> à la date de " + DateTime.Now + "</p>"
                                    + "<table class=\"table\">"
                                    + "<tr>"
                                    + "<th>"
                                    + "N°"
                                    + "</th>"
                                    + "<th> "
                                    + "Montant"
                                    + "</th>"
                                    + "<th>Devise</th>"
                                    + "<th> "
                                    + "Fournisseur"
                                    + "</th>"
                                    + "<th> "
                                    + "Depot banque "
                                    + "</th>"
                                    + "<th> "
                                    + "Traité"
                                    + "</th>"
                                    + "<th> "
                                    + "Agence"
                                    + "</th>"
                                    + "<th> "
                                    + "Banque"
                                    + "</th>"
                                    + "</tr>"
                                    + " "
                                    + refContent[key]
                                    + " "
                                    + "</table>";

                        MailFunctions.SendMail(new Models.MailModel()
                        {
                            Subject = "Attribution de référence",
                            To = clientmail,
                            Body =msg
                        },db);
                    }

                   await db.SaveChangesAsync();
                }
                catch (Exception ee)
                { }
            }
            dossier = null;
            refContent = null;
            referenceBanque = null;
            return RedirectToAction("Attache");
        }
        

        public JsonResult AttributionRef(int DossierId,string Reference,bool groupe=false)
        {
            Reference = Reference.Trim();
            Dossier dossier = db.GetDossiers.Find(DossierId);
            var _reference = db.GetReferenceBanques.FirstOrDefault(r => r.NumeroRef.ToLower() == Reference.ToLower());
            if (dossier == null)
                throw new Exception("Erreur inattendue liée au dossier.");
            if (string.IsNullOrEmpty(Reference))
                throw new Exception("La référence ne peut pas être nulle...");
            if (_reference != null && !groupe)
                throw new Exception("Cette référence a déjà été attribuée. Vueillez selectionnez l'option associer le dossier pour utiliser cette la référence");

            var banqueId = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);
            //Création de la référence
            if (_reference == null)
            {
                _reference = new ReferenceBanque()
                {
                    NATURE = dossier.NatureOperation.ToString(),
                    BanqueId = banqueId,
                    NumeroRef = Reference
                };
                db.GetReferenceBanques.Add(_reference);
            }
            db.SaveChanges();
            _reference.Dossiers.Add(dossier);
            dossier.ReferenceExterneId = _reference.Id;
            dossier.EtapesDosier = 5;
            db.SaveChanges();

            dossier = null;
            _reference = null;
            return Json(new string[] { "Réference attribuée avec succès...",Reference },JsonRequestBehavior.AllowGet);
        }


        // POST: ReferenceBanques/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit([Bind(Include = "Id,NumeroRef,DateReception,DepotBEAC,DélaitT,NOTIFICATION,DateNotif,MT99,DateMT99,DateMT202,DateCredit,DateTraité,ObsTransmission,ObsBEAC,NATURE,CvEURO,BanqueId")] ReferenceBanque referenceBanque)
        public async Task<ActionResult> Edit(ReferenceBanque model)
        {
            if (ModelState.IsValid)
            {
                var idBanque = (int)(Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);
                try
                {
                    var referenceBanque = db.GetReferenceBanques.Include(r=>r.Dossiers).FirstOrDefault(r=>r.Id==model.Id);
                    referenceBanque.BanqueId= idBanque;

                    if (model.Apuré && !referenceBanque.Apuré)
                    {
                        referenceBanque.Apuré = model.Apuré;
                        try
                        {
                            foreach (var item in referenceBanque.Dossiers)
                            {
                                try
                                {
                                    item.EtapesDosier = 11;
                                }
                                catch (Exception)
                                { }
                            }
                            db.SaveChanges();
                            var dossier = referenceBanque.Dossiers.FirstOrDefault();
                            var fournisseur = dossier != null ? dossier.Fournisseur != null ? dossier.Fournisseur.Nom : "" : "";
                            var nomclient = dossier != null ? dossier.Client != null ? dossier.Client.Nom : "" : "";
                            MailFunctions.EnvoiMail(referenceBanque, fournisseur, nomclient,(int)idBanque, "Référence apurée", " a bien été apuré le " + DateTime.Now,db);
                        }
                        catch (Exception)
                        { }
                    }

                    if (model.EnvoieBEAC)
                    {
                        referenceBanque.EnvoieBEAC = model.EnvoieBEAC;
                        referenceBanque.DepotBEAC = DateTime.Now;
                        try
                        {
                            foreach (var item in referenceBanque.Dossiers)
                            {
                                try
                                {
                                    item.EtapesDosier = 6;
                                }
                                catch (Exception)
                                { }
                            }
                            db.SaveChanges();
                        }
                        catch (Exception)
                        { }

                        try
                        {
                            var dossier = referenceBanque.Dossiers.FirstOrDefault();
                            var fournisseur = dossier != null ? dossier.Fournisseur != null ? dossier.Fournisseur.Nom : "" : "";
                            var nomclient = dossier != null ? dossier.Client != null ? dossier.Client.Nom : "" : "";
                            var txt = MailFunctions.TabReference(model, fournisseur, referenceBanque.Montant, (referenceBanque.Devise != null ? referenceBanque.Devise.Nom : ""), nomclient,idBanque,db);
                            var body = "La référence " + model.NumeroRef + " a été transmise à la BEAC le " + model.DepotBEAC
                                      + "<hr />"
                                      + txt;
                            var client = referenceBanque.Dossiers.ToList()[0].Client;
                            var ges = client.Banques.FirstOrDefault(b => b.Site.BanqueId(db) == idBanque).Gestionnaire;
                            MailFunctions.SendMail(new Models.MailModel()
                            {
                                To = client.Email,
                                CC = ges != null ? ges.Email : "",
                                Subject = "Envoie du dossier à la BEAC",
                                Body = body
                            }, db);
                            ges = null;
                        }
                        catch (Exception)
                        { }
                    }

                    referenceBanque.Apuré = model.Apuré;
                    if(model.DepotBEAC!=null && model.DepotBEAC!=new DateTime())
                        referenceBanque.DepotBEAC = model.DepotBEAC;
                    if (model.DateCredit != null && model.DateCredit != new DateTime())
                        referenceBanque.DateCredit = model.DateCredit;
                    if (model.DateMT202 != null && model.DateMT202 != new DateTime())
                        referenceBanque.DateMT202 = model.DateMT202;
                    if (model.DateMT99 != null && model.DateMT99 != new DateTime())
                        referenceBanque.DateMT99 = model.DateMT99;
                    if (model.DateNotif != null && model.DateNotif != new DateTime())
                    {
                        if (referenceBanque.DateNotif != model.DateNotif)
                        {
                            if (model.NOTIFICATION == NOTIFICATION.Accordé)
                            {
                                referenceBanque.Accordé = true;
                                try
                                {
                                    foreach (var item in referenceBanque.Dossiers)
                                    {
                                        try
                                        {
                                            item.EtapesDosier = 7;
                                        }
                                        catch (Exception)
                                        { }
                                    }
                                    db.SaveChanges();
                                }
                                catch (Exception)
                                { }
                            }
                            else
                            {
                                referenceBanque.Accordé = false;
                                try
                                {
                                    foreach (var item in referenceBanque.Dossiers)
                                    {
                                        try
                                        {
                                            item.EtapesDosier = 5;
                                        }
                                        catch (Exception)
                                        { }
                                    }
                                    db.SaveChanges();
                                }
                                catch (Exception)
                                { }
                            }
                            referenceBanque.DateNotif = model.DateNotif;

                            var dossier = referenceBanque.Dossiers.FirstOrDefault();
                            var fournisseur = dossier != null ? dossier.Fournisseur != null ? dossier.Fournisseur.Nom : "" : "";
                            var nomclient = dossier != null ? dossier.Client != null ? dossier.Client.Nom : "" : "";
                            var txt=MailFunctions.TabReference(model, fournisseur,referenceBanque.Montant,(referenceBanque.Devise!=null? referenceBanque.Devise.Nom:""), nomclient, idBanque, db);
                            var body = "La référence " + model.NumeroRef + " est " + model.NOTIFICATION + "e le " + model.DateNotif
                                      + "<hr />"
                                      + txt;
                            dossier = null; 
                            try
                            {
                                var client = referenceBanque.Dossiers.ToList()[0].Client;
                                var ges = client.Banques.FirstOrDefault(b=>b.Site.BanqueId(db)== idBanque).Gestionnaire;
                                MailFunctions.SendMail(new Models.MailModel()
                                {
                                    To=client.Email,
                                    CC=ges!=null?ges.Email:"",
                                    Subject="Référence "+model.NumeroRef+" "+model.NOTIFICATION,
                                    Body=body
                                },db);
                                ges = null;
                            }
                            catch (Exception)
                            {}
                        }
                    }
                    if (model.DateReception != null && model.DateReception != new DateTime())
                    {
                        if (referenceBanque.DateReception != model.DateReception)
                        {
                            referenceBanque.DateReception = model.DateReception;
                            try
                            {
                                foreach (var item in referenceBanque.Dossiers)
                                {
                                    try
                                    {
                                        item.EtapesDosier = 8;
                                    }
                                    catch (Exception)
                                    { }
                                }
                                db.SaveChanges();
                            }
                            catch (Exception)
                            { }

                            var dossier = referenceBanque.Dossiers.FirstOrDefault();
                            var fournisseur = dossier != null ? dossier.Fournisseur != null ? dossier.Fournisseur.Nom : "" : "";
                            var nomclient = dossier != null ? dossier.Client != null ? dossier.Client.Nom : "" : "";
                            var txt =MailFunctions.TabReference(model, fournisseur, referenceBanque.Montant, (referenceBanque.Devise != null ? referenceBanque.Devise.Nom : ""),nomclient,idBanque,db);
                            var delai = referenceBanque.NATURE == "Service" ? "30 jours " : "3 mois ";
                            var body = "La dévise pour la référence " + model.NumeroRef + " du client " + nomclient + " a été reçue le " + model.DateReception
                                      + "<hr />"
                                      + txt
                                      +"<p></ p>"
                                      + "<mark>Remarque: </ mark> Le dossier doit être apuré dans un delai de "+delai+" à patir du "+((DateTime)model.DateReception).AddDays(1);
                            dossier = null;
                            try
                            {
                                var client = referenceBanque.Dossiers.ToList()[0].Client;
                                var ges = client.Banques.FirstOrDefault(b => b.Site.BanqueId(db) == idBanque).Gestionnaire;
                                MailFunctions.SendMail(new Models.MailModel()
                                {
                                    To = client.Email,
                                    CC = ges != null ? ges.Email : "",
                                    Subject = "Reception de la devise pour la référence " + model.NumeroRef,
                                    Body = body
                                }, db);
                                ges = null;
                            }
                            catch (Exception)
                            { }
                        }
                    }
                    if (model.DateTraité != null && model.DateTraité != new DateTime())
                        referenceBanque.DateTraité = model.DateTraité;
                    if (model.DepotBEAC != null && model.DepotBEAC != new DateTime())
                        referenceBanque.DepotBEAC = model.DepotBEAC;
                    referenceBanque.DélaitT = model.DélaitT;
                    referenceBanque.Apuré = model.Apuré;
                    referenceBanque.CvEURO = model.CvEURO;
                    referenceBanque.MT99 = model.MT99;
                    referenceBanque.NATURE = model.NATURE;
                    referenceBanque.NOTIFICATION = model.NOTIFICATION;
                    referenceBanque.NumeroRef = model.NumeroRef;
                    referenceBanque.ObsBEAC = model.ObsBEAC;
                    referenceBanque.ObsTransmission = model.ObsTransmission;
                    referenceBanque.Echus = model.Echus;

                    db.Entry(referenceBanque).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                catch (Exception)
                { }
            }
            if(model.Id==0)
               return RedirectToAction("Index");

                ViewBag.BanqueId = new SelectList(db.GetBanques, "Id", "Nom", model.BanqueId);
            return RedirectToAction("Details",new { id= model.Id });
        }

        // GET: ReferenceBanques/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReferenceBanque referenceBanque = await db.GetReferenceBanques.FindAsync(id);
            if (referenceBanque == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "ref";
            ViewBag.navigation_msg = "suppression reference";
            return View(referenceBanque);
        }

        // POST: ReferenceBanques/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ReferenceBanque referenceBanque = await db.GetReferenceBanques.FindAsync(id);
            db.GetReferenceBanques.Remove(referenceBanque);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
