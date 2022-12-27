using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using genetrix.Models;
using genetrix.Models.Fonctions;
using System.IO;
using System.IO.Compression;
using Microsoft.AspNet.Identity;
using System.Net.Mail;
using System.Configuration;

namespace genetrix.Controllers
{
    public class ReferenceBanquesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ReferenceBanques
        public ActionResult Index(bool reference=false,string st="",string comp="",string type="")
        {
            if (Session == null)
                return RedirectToAction("login", "auth");
            ViewBag.navigation = "ref";
            ViewBag.navigation_msg = "liste references";
            ViewBag.navigation = string.IsNullOrEmpty(st) ? "aenv_bac" : st;
            //if (string.IsNullOrEmpty(st))
            //    return View(new List<ReferenceBanque>());
            ICollection<ReferenceBanque> getReferenceBanques = null;
            var structure = db.Structures.Find(Session["IdStructure"]);

            var banqueID = structure.BanqueId(db);
            genetrix.Models.Banque baqnue = null;
            try
            {
                baqnue = db.GetBanques.Find(banqueID);
            }
            catch (Exception)
            { }
            var role = db.XtraRoles.Find(Session["IdXRole"]);
            var site = db.Structures.Find(Session["IdStructure"]);
            var gesId = User.Identity.GetUserId();
            if (role == null)
                return RedirectToAction("Login", "auth");
            //var rr = VariablGlobales.UserBanqueReferences(db, site, role, banqueID, gesId);
            var getReferences = db.GetReferenceBanques.Include(d => d.Banque).ToList();
            List<ReferenceBanque> rr = new List<ReferenceBanque>();
            getReferences.ForEach(d =>
            {
                if (d.EtapesDosier != 26 && d.EtapesDosier != 27 && d.UserAsPermition(db, site, role, banqueID, gesId))
                {
                    d.UserAsPermition(db, site, role, banqueID, gesId);
                    rr.Add(d);
                }
            });
            var ref_doss = db.GetDossiers.Include("Client")
                          .Include("ReferenceExterne")
                          .Include("Site")
                          .Include("StatusDossiers")
                          .Include("DeviseMonetaire")
                          .Where(d => d.ReferenceExterneId > 0).ToList();
            var dossiers = VariablGlobales.UserBanqueDossiers(db,ref_doss, site, role, banqueID: banqueID, agentId: gesId, montantDfx: baqnue.MontantDFX, dfx_ref:2);

            if (!reference)
            {
                getReferenceBanques = rr.ToList();
            }
            else
            {
                getReferenceBanques = new List<ReferenceBanque>();
                foreach (var re in rr)
                {
                    if (re.Dossiers.Count > 0)
                        getReferenceBanques.Add(re);
                }
            }
            rr = null;
            var dd = getReferenceBanques.ToList();
            getReferenceBanques = new List<ReferenceBanque>();
            if(string.IsNullOrEmpty(comp))
            switch (st)
            {
                case "aapurer":
                    foreach (var r in dd)
                        if (r.AApurer)
                            getReferenceBanques.Add(r);
                    ViewBag.info = " à apurer";
                    ViewBag.st = "apurement";
                    break;
                case "aapurer-acc":
                    foreach (var r in dd)
                        if (r.AApurer_Ac)
                            getReferenceBanques.Add(r);
                    ViewBag.info = "à apurer (à completer)";
                    ViewBag.st = "apurement";
                    break;
                case "aapurer-av":
                    foreach (var r in dd)
                        if (r.AApurer_Av)
                            getReferenceBanques.Add(r);
                    ViewBag.info = "à apurer (attente validation)";
                    ViewBag.st = "apurement";
                    break;
                case "echu":
                    foreach (var r in dd)
                        if (r.Echus)
                            getReferenceBanques.Add(r);
                    ViewBag.info = "echu(s)";
                    ViewBag.st = "apurement";
                    break;
                case "aapurer-beac":
                    foreach (var r in dd)
                        if (r.EtapesDosier==232)
                            getReferenceBanques.Add(r);
                    ViewBag.info = "à apurer (en cours d'analyse BEAC)";
                    ViewBag.st = "apurement";
                    break;
                case "apure":
                    foreach (var r in dd)
                        if (r.EtapesDosier==24)
                            getReferenceBanques.Add(r);
                    ViewBag.info = "apuré";
                    ViewBag.st = "apurement";
                    break;
                case "arch":
                    foreach (var r in dd)
                    {
                        if (r.EtapesDosier == 26)
                            getReferenceBanques.Add(r);
                    }
                    ViewBag.info = " archivées";
                    break;
                case "list":
                    foreach (var r in dd)
                    {
                        if (r.EtapesDosier <=22)
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
                    ViewBag.info = " Refinancement";
                    break; 
                case "env_bac":
                    foreach (var r in dd)
                    {
                        try
                        {
                            if (r.EtapesDosier > 15)
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
                            if (r.EtapesDosier ==13 || r.EtapesDosier ==14)
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
                            if (r.EtapesDosier ==20)
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
                            if (r.EtapesDosier <=22)
                                getReferenceBanques.Add(r);
                        }
                        catch (Exception)
                        {}
                    }
                    ViewBag.info = " refinancement";
                    break;
            }
            else
            {
                int compNum = int.Parse(comp.Split('_')[0]);
                int compMin = int.Parse(comp.Split('_')[1]);
                int compMax = int.Parse(comp.Split('_')[2]);
                var composant = db.GetComposants.FirstOrDefault(c => c.Numero == compNum);
                st = composant.Description;
                foreach (var r in dd)
                {
                    try
                    {   if(r.Dossiers.Count==0)
                        {
                            r.DateCredit = default;
                            r.NATURE = default;
                            r.NOTIFICATION = default;
                            r.DateReception = default;
                            r.DepotBEAC = default;
                            continue;
                        }
                        if (r.EtapesDosier >= compMin && r.EtapesDosier <= compMax)
                            getReferenceBanques.Add(r);
                    }
                    catch (Exception)
                    {}
                }

                try
                {
                    db.SaveChanges();
                }
                catch (Exception)
                {}

                var _dossiers = new List<Dossier>();
                foreach (var item in dossiers.Where(d => d.EtapesDosier >= compMin && d.EtapesDosier <= compMax))
                {
                    if (item.ReferenceExterneId == null)
                        _dossiers.Add(item);
                }
                ViewData["dossiers"] = _dossiers;
                _dossiers = null;
            }
            //ViewBag.statut = st.ToString();
            dd = null;
            dossiers = null;
            return View(getReferenceBanques.ToList());
        }

        public async Task<ActionResult> Reflesh()
        {

            var user =await db.GetCompteBanqueCommerciales.Include(d => d.Structure).FirstOrDefaultAsync(u=>u.Id== User.Identity.GetUserId());
            var idbanque = user.Structure.BanqueId(db);
            user = null;
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

        [HttpGet]
        public async Task<ActionResult> GenereCourier(int id)
        {
            ReferenceBanque reference =await db.GetReferenceBanques.Include(r=>r.Dossiers).FirstOrDefaultAsync(r=>r.Id==id);
            var structure =await db.Structures.FindAsync(Session["IdStructure"]);
            var idbanque = structure.BanqueId(db);

            ViewBag.compteXaf = (from c in db.GetCompteXAFs
                                 where c.BanqueId == idbanque
                                 select c).ToList(); 
            
            ViewBag.correspondants = (from c in db.GetCorrespondants
                                 where c.BanqueId == idbanque
                                 select c).ToList();
            
            ViewBag.signataires = db.GetSignataires.Where(s=>s.BanqueId==idbanque).ToList();
            try
            {
                var idBanqueXaf = Convert.ToInt32(reference.CompteBEACEditer);
                reference.CompteBEACEditer = db.GetCompteXAFs.Find(idBanqueXaf).Libellé;
            }
            catch (Exception)
            {}
            try
            {
                ViewBag.logo = (await db.GetBanques.FindAsync(idbanque)).Image;
            }
            catch (Exception)
            {}
            try
            {
                var idCorresp = Convert.ToInt32(reference.BanqueDomiciliaire);
                reference.BanqueDomiciliaire = db.GetCorrespondants.Find(idCorresp).NomBanque;
            }
            catch (Exception)
            {}

            try
            {
                var _id = Convert.ToInt32(reference.Signataire3);
                reference.Signataire3 = db.GetSignataires.Find(_id).NomComplet;
                reference.fonction3 = db.GetSignataires.Find(_id).Fonction;
            }
            catch (Exception)
            {}

             
            try
            {
                var _id = Convert.ToInt32(reference.Signataire4);
                reference.Signataire4 = db.GetSignataires.Find(_id).NomComplet;
                reference.fonction4 = db.GetSignataires.Find(_id).Fonction;
            }
            catch (Exception)
            {}
            try
            {
                var _id = Convert.ToInt32(reference.IdCompteAcrediter);
                reference.CompteanqueACrediter = db.CompteNostroes.Find(_id).Libellé;
            }
            catch (Exception)
            {}

            try
            {
                reference.GetBanque = db.Structures.Find(idbanque).Nom;
            }
            catch (Exception)
            {}

            try
            {
                idbanque = 0;
                idbanque = reference.Get_IdBanqueBen;
                reference.BanqueBeneficiaire = db.CompteBeneficiaires.Find(idbanque);
            }
            catch (Exception)
            {}
            return PartialView("EditCourrier", reference);
        }

        [HttpPost]
        public async Task<ActionResult> EditCourrier(ReferenceBanque model)
        {
            try
            {
                ReferenceBanque reference =await db.GetReferenceBanques.FindAsync(model.Id);
                reference.CompteBEACEditer = model.CompteBEACEditer;
                reference.BanqueDomiciliaire = model.BanqueDomiciliaire;
                reference.IdCompteAcrediter = model.IdCompteAcrediter;
                reference.Ville = model.Ville;
                reference.Pays = model.Pays;
                reference.CodeSwift = model.CodeSwift;
                reference.Signataire1 = model.Signataire1;
                reference.Signataire2 = model.Signataire2;
                reference.Signataire3 = model.Signataire3;
                reference.Signataire4 = model.Signataire4;
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {}
            return RedirectToAction("Details",new { model.Id});
        }

        public ActionResult GetElets(int id,string type)
        {
            var bankid = Session["IdStructure"];
            var structure = db.Structures.Find(bankid);
            var idbanque = structure.BanqueId(db);
            try
            {
                switch (type)
                {
                    case "sing":
                        //signataires
                        var sign=db.GetSignataires.Find(id);
                        return Json(new { sign.Rang, sign.NomComplet, sign.Fonction,type= type }, JsonRequestBehavior.AllowGet);
                    case "bd":
                        var corresp = db.GetCorrespondants.Find(id);
                        var comptes = from c in corresp.GetCompteNostros
                                      select new { c.Id,c.Libellé,c.Numero,c.Cle };
                        return Json(new { corresp.NomBanque, comptes, corresp.SwiftCode, corresp.Pays, corresp.Ville,type= type }, JsonRequestBehavior.AllowGet);
                    default:
                        break;
                }
            }
            catch (Exception)
            {}
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetNostro(int idCoresspond)
        {
            var coress = db.GetCorrespondants.Include(c => c.GetCompteNostros).FirstOrDefault(c => c.Id == idCoresspond);

            return Json(coress.GetCompteNostros.ToList(), JsonRequestBehavior.AllowGet);
        }

        // GET: ReferenceBanques/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (Session == null)
                return RedirectToAction("login", "auth");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReferenceBanque referenceBanque = await db.GetReferenceBanques.Include(r=>r.Dossiers).FirstOrDefaultAsync(r=>r.Id==id);
            if (referenceBanque == null)
            {
                return HttpNotFound();
            }

            if(string.IsNullOrEmpty(referenceBanque.NumeroRef))
            {
                try
                {
                    var dos = referenceBanque.Dossiers.ToList();
                    foreach (var item in dos)
                    {
                        referenceBanque.Dossiers.Remove(item);
                        item.ReferenceExterneId = null;
                    }
                    db.SaveChanges();
                }
                catch (Exception)
                {}
            }
            var userId = User.Identity.GetUserId();
            var user =await db.GetCompteBanqueCommerciales.Include(u=>u.Structure).FirstOrDefaultAsync(u=>u.Id==userId);
            var site = db.Structures.Find(user.IdStructure);
            var dd = new List<Agence>();
            var agenceID = Convert.ToInt32(Session["IdStructure"]);
            var banqueID = site.BanqueId(db);
            var agentId = (string)Session["userId"];
            var role = db.XtraRoles.Find(Session["IdXRole"]);

            db.Agences.ToList().ForEach(c =>
            {
                if (c.BanqueId(db) == site.BanqueId(db) && c.Id != user.IdStructure)
                    dd.Add(c);
            });

            ViewBag.sites = new SelectList(dd, "Id", "Nom");
            dd = null;
            try
            {
                site = db.Structures.Find(user.IdStructure);
                ViewBag.userSIteMinNiveau = site.NiveauDossier;
                ViewBag.userSIteMaxNiveau = site.NiveauMaxDossier;
            }
            catch (Exception)
            { }
            referenceBanque.DossiersPermitions(db, site, role, banqueID, agentId);
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
                var structure = db.Structures.Find(Session["IdStructure"]);
                var banqueId = structure.BanqueId(db);
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

            var siteID = Convert.ToInt32(Session["IdStructure"]);
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

        public ActionResult DownloadZipFile(int referenceId)
        {
            var reference = db.GetReferenceBanques.Include(r => r.Dossiers).FirstOrDefault(r => r.Id == referenceId);
            var dossiers = reference.Dossiers;

            string nomClientzip = reference.GetClient+"_"+ reference.GetFournisseur,nomFournzip = reference.GetFournisseur;


            //var uploads = (from u in files.GetFiles()
            //               select u.FullName).FirstOrDefault();
            //var files = Directory.GetParent(GetClientDossierFolder(doss.Client.Id, doss.Dossier_Id));
           
            nomClientzip = nomClientzip.Replace(" ", "");
            nomFournzip = nomFournzip.Replace(" ", "_");
            //DirectoryInfo diSource = null;
           // string sourceDirectory = "";

            var sourceDirectory = GetFournisseurDocuments(reference.ClientId, reference.FournisseurId);
            DirectoryInfo fournisseurDocsSource = new DirectoryInfo(sourceDirectory);

            sourceDirectory = GetClientDocuments(reference.ClientId);
            DirectoryInfo clientDocsSource = new DirectoryInfo(sourceDirectory);

            using (var memoryStream = new MemoryStream())
            {
                using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    //dossiers
                    try
                    {
                        foreach (var doss in dossiers)
                        {
                            var docSource = GetClientDossierFolder(doss.Client.Id, doss);
                            var diSource = new DirectoryInfo(docSource);
                            try
                            {
                                //ziparchive.CreateEntry(nomClientzip+"/");
                                foreach (var file in diSource.GetFiles())
                                {
                                    try
                                    {
                                        ziparchive.CreateEntryFromFile(file.FullName, nomClientzip + "/Dossier_" + doss.Montant + "_" + doss.DeviseToString + "/" + file.Name);

                                    }
                                    catch (Exception e1)
                                    { }
                                }
                            }
                            catch (Exception e)
                            {}

                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }

                    try
                    {
                        //Client documents
                        foreach (var file in clientDocsSource.GetFiles())
                        {
                            try
                            {
                                ziparchive.CreateEntryFromFile(file.FullName, nomClientzip +  "/Documents_Client" + "/" + file.Name);

                            }
                            catch (Exception e1)
                            { }
                        }
                    }
                    catch (Exception e)
                    { }

                    try
                    {
                        //Fournisseur documents
                        foreach (var file in fournisseurDocsSource.GetFiles())
                        {
                            try
                            {
                                ziparchive.CreateEntryFromFile(file.FullName, nomClientzip + "/Fournisseur_" + nomFournzip + "/" + file.Name);

                            }
                            catch (Exception e1)
                            { }
                        }
                    }
                    catch (Exception e)
                    { }
                }

                reference.NbrTelechargement++;
                try
                {
                    db.SaveChanges();
                }
                catch (Exception)
                {}
                return File(memoryStream.ToArray(), "application/zip", nomClientzip + ".zip");
            }
        }
        
        public async Task<ActionResult> ExtraireDossier(int referenceId, string agtemail, bool includ_f = true, bool includ_cl = true,string subject="",string body="")
        {
            var reference = await db.GetReferenceBanques.Include(r => r.Dossiers).FirstOrDefaultAsync(r => r.Id == referenceId);
            var dossiers = reference.Dossiers;

            string nomClientzip = reference.GetClient+"_"+ reference.GetFournisseur,nomFournzip = reference.GetFournisseur;


            //var uploads = (from u in files.GetFiles()
            //               select u.FullName).FirstOrDefault();
            //var files = Directory.GetParent(GetClientDossierFolder(doss.Client.Id, doss.Dossier_Id));
           
            nomClientzip = nomClientzip.Replace(" ", "");
            nomFournzip = nomFournzip.Replace(" ", "_");
            //DirectoryInfo diSource = null;
           // string sourceDirectory = "";

            var sourceDirectory = GetFournisseurDocuments(reference.ClientId, reference.FournisseurId);
            DirectoryInfo fournisseurDocsSource = new DirectoryInfo(sourceDirectory);

            sourceDirectory = GetClientDocuments(reference.ClientId);
            DirectoryInfo clientDocsSource = new DirectoryInfo(sourceDirectory);
            var idAgent = User.Identity.GetUserId();
            var agentEmail = db.GetCompteBanqueCommerciales.Find(idAgent).Email;

            using (var memoryStream = new MemoryStream())
            {
                using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    //dossiers
                    try
                    {
                        foreach (var doss in dossiers)
                        {
                            var docSource = GetClientDossierFolder(doss.Client.Id, doss);
                            var diSource = new DirectoryInfo(docSource);
                            try
                            {
                                //ziparchive.CreateEntry(nomClientzip+"/");
                                foreach (var file in diSource.GetFiles())
                                {
                                    try
                                    {
                                        ziparchive.CreateEntryFromFile(file.FullName, nomClientzip + "/Dossier_" + doss.Montant + "_" + doss.DeviseToString + "/" + file.Name);

                                    }
                                    catch (Exception e1)
                                    { }
                                }
                            }
                            catch (Exception e)
                            {}

                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }

                    try
                    {
                        //Client documents
                        if(includ_cl)
                            foreach (var file in clientDocsSource.GetFiles())
                            {
                                try
                                {
                                    ziparchive.CreateEntryFromFile(file.FullName, nomClientzip +  "/Documents_Client" + "/" + file.Name);

                                }
                                catch (Exception e1)
                                { }
                            }
                    }
                    catch (Exception e)
                    { }

                    try
                    {
                        //Fournisseur documents
                        if(includ_f)
                            foreach (var file in fournisseurDocsSource.GetFiles())
                            {
                                try
                                {
                                    ziparchive.CreateEntryFromFile(file.FullName, nomClientzip + "/Fournisseur_" + nomFournzip + "/" + file.Name);

                                }
                                catch (Exception e1)
                                { }
                            }
                    }
                    catch (Exception e)
                    { }
                }

                reference.NbrTelechargement++;
                try
                {
                   await db.SaveChangesAsync();
                }
                catch (Exception)
                {}
                // Send Email with zip as attachment.
                string emailsender = ConfigurationManager.AppSettings["emailsender"].ToString();
                string emailSenderPassword = ConfigurationManager.AppSettings["emailSenderPassword"].ToString();

               // using (MailMessage mail = new MailMessage(emailsender, agentEmail))
                using (MailMessage mail = new MailMessage())
                {
                    try
                    {
                        if (string.IsNullOrEmpty(subject))
                            mail.Subject = "Extraction du transfert " + reference.GetClient + $" ({reference.GetFournisseur}, {reference.MontantStringDevise})";
                        else
                            mail.Subject = subject;

                        if (string.IsNullOrEmpty(body))
                            mail.Body = "Vous avez extrait le dossier " + reference.GetClient + $" {reference.MontantStringDevise} du fournisseur {reference.GetFournisseur}";
                        else
                            mail.Body = body;

                        mail.Attachments.Add(new Attachment(new MemoryStream(memoryStream.ToArray()), "Document.zip"));
                        mail.From = new MailAddress(emailsender);
                        foreach (var item in agtemail.Split(';'))
                        {
                            if(item.Contains("@"))
                            mail.To.Add(item);
                        }

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

                return Json("ok", JsonRequestBehavior.AllowGet);
            }
        }

        private string GetFournisseurDocuments(object clientId, object fournisseurId)
        {
            throw new NotImplementedException();
        }

        public string GetFournisseurDocuments(int clientId, int fournisseurId)
        {
            try
            {
                string projectPath = "~/EspaceClient/" + clientId + "/Ressources/Fournisseurs";
                string folderName = Path.Combine(Server.MapPath(projectPath), "" + fournisseurId);

                return folderName;
            }
            catch (Exception ee)
            { }
            return "";
        }


        private string GetClientDocuments(int clientId)
        {
            try
            {
                string projectPath = "~/EspaceClient/" + clientId + "/Ressources";
                string folderName = Path.Combine(Server.MapPath(projectPath), "DocumentsEntreprise");

                return folderName;
            }
            catch (Exception ee)
            { }
            return "";
        }


        [HttpGet]
        public ActionResult ModifEtatDossierJS(int? etat = null, int? idreference = null,int? idOpSwft=null, bool Estgroupe = true, string message = "", string date = "")
        {
            var result = "Opération effectuée avec succes";
            ReferenceBanque referenceBanque = null;
            try
            {
                var structure = db.Structures.Find(Session["IdStructure"]);
                var idbanque = structure.BanqueId(db);
                genetrix.Models.Banque banque = new Models.Banque();
                try
                {
                    banque = db.GetBanques.Find(idbanque);
                }
                catch (Exception)
                { }
                if (etat > 0)
                {
                    
                    if (idreference > 0)
                    {
                        string fournisseur = "", msg = "";
                        var nomclient = "";
                        int count = 0;

                        referenceBanque = db.GetReferenceBanques.Include(d => d.Dossiers).FirstOrDefault(r => r.Id == idreference);
                        if (etat == 15)
                        {
                            referenceBanque.DepotBEAC = DateTime.Now;
                        }
                        if (referenceBanque.Dossiers.Count > 0)
                        {
                            fournisseur = referenceBanque.Dossiers.FirstOrDefault().Fournisseur.Nom;
                            nomclient = referenceBanque.Dossiers.FirstOrDefault().Client.Nom;
                            if (referenceBanque.Dossiers.FirstOrDefault().Client.Adresses.Count == 0)
                            {
                                result = "Erreur: L'adresse mail du client " + nomclient + " est nulle. Le processus s'est arreté, veillez corriger cette erreur!";
                                var d = 0;
                                var dd = 1 / d;
                            }
                            try
                            {
                                var etatmp = etat;
                                MailFunctions.ChangeEtapeDossier2((int)idbanque, User.Identity.GetUserId(), (int)etat, referenceBanque, db, structure, rejet: true, msg: message, idOpSwft);

                                try
                                {
                                    if ((etatmp == 19 || etatmp == 20) && idOpSwft > 0)
                                    {
                                        var op = db.GetOperateurSwifts.Find(idOpSwft);
                                        var details = MailFunctions.DetailsReference(referenceBanque);

                                        var subject = $"Dossier à traiter: {referenceBanque.GetClient}; {referenceBanque.MontantStringDevise}; {referenceBanque.GetFournisseur}";
                                        var To = op.Email;
                                        var body = $"Madame, Monsieur; <br /><br /> Nous vous transmettons ci-joint le dossier du client {referenceBanque.GetClient} pour traitement ce jour via Swift selon les caracteristiques suivantes:"
                                           + "<br /> <br /> <div class=\"card\">" + details + "</ div>";

                                        //rediction vers extraction 
                                        ExtraireDossier(referenceBanque.Id, To, false, false, subject, body);
                                    }
                                }
                                catch (Exception)
                                { }

                            }
                            catch (Exception)
                            { }
                        }

                    }
                }
                else
                {
                    if (Estgroupe)
                    {
                        if (idreference > 0)
                        {
                            string fournisseur = "", msg = "";
                            var nomclient = "";
                            int count = 0;

                            referenceBanque = db.GetReferenceBanques.Include(d => d.Dossiers).FirstOrDefault(r => r.Id == idreference);
                            foreach (var d in referenceBanque.Dossiers)
                            {
                                d.EtapesDosier = 2;
                                //msg = d.GetEtapDossier()[0];
                                count++;
                            }
                            if (count > 0)
                            {
                                fournisseur = referenceBanque.Dossiers.FirstOrDefault().Fournisseur.Nom;
                                nomclient = referenceBanque.Dossiers.FirstOrDefault().Client.Nom;
                            }
                            //if (string.IsNullOrEmpty(re.Dossiers.FirstOrDefault().Client.Email))
                            if (referenceBanque.Dossiers.FirstOrDefault().Client.Adresses.Count == 0)
                            {
                                result = "Erreur: L'adresse mail du client " + nomclient + " est nulle. Le processus s'est arreté, veillez corriger cette erreur!";
                                return Json(result + "_" + etat, JsonRequestBehavior.AllowGet);
                            }
                            try
                            {
                                db.SaveChanges();
                                MailFunctions.ChangeEtapeDossier2((int)idbanque, User.Identity.GetUserId(), -2, referenceBanque, db, structure, rejet: true, msg: message);
                            }
                            catch (Exception)
                            { }
                        }
                    }
                    //return RedirectToAction("Index");
                }

                //db.SaveChanges();
            }
            catch (Exception)
            {
                //result = "erreur";
            }
            if(idreference!=null)
                return RedirectToAction("Details",new { id= idreference });
            var url = Session["composant"].ToString();
            //return RedirectToAction("IndexBanque", "Index", new { panel = "ref", interf = "refinancement" }) ;
            return Redirect("~/Dossiers_banque/Index?"+url) ;
        }


        private string GetClientDossierFolder(int clientId, Dossier dossier,bool entreprise=false)
        {
            try
            {
                string projectPath = "", folderName="";
                if (!entreprise)
                {
                     projectPath = "~/EspaceClient/" + clientId + "/Ressources/Transferts";
                     folderName = Path.Combine(Server.MapPath(projectPath), dossier.Intitulé);
                }
                else
                {
                     projectPath = "~/EspaceClient/" + clientId + "/Ressources";
                     folderName = Path.Combine(Server.MapPath(projectPath), "DocumentsEntreprise");
                }

                return folderName;
            }
            catch (Exception ee)
            { }
            return "";
        }


        public async Task<JsonResult> AttributionRef(int DossierId,string Reference,bool groupe=false)
        {
            Reference = Reference.Trim();
            Dossier dossier = db.GetDossiers.Find(DossierId);
            Reference _reference = null;
           if (dossier == null)
                throw new Exception("Erreur inattendue liée au dossier.");
            if(dossier.DFX6FP6BEAC==3)
                _reference = db.GetReferenceBanques.FirstOrDefault(r => r.NumeroRef.ToLower() == Reference.ToLower());
            else if(dossier.DFX6FP6BEAC==1)
                _reference = db.GetDFXs.FirstOrDefault(r => r.NumeroRef.ToLower() == Reference.ToLower());

            if (string.IsNullOrEmpty(Reference))
                throw new Exception("La référence ne peut pas être nulle...");
            if (_reference != null && !groupe)
                throw new Exception("Cette référence a déjà été attribuée. Vueillez selectionnez l'option associer le dossier pour utiliser cette la référence");

            var structure = db.Structures.Find(Session["IdStructure"]);

            var banqueId = structure.BanqueId(db);
            //Création de la référence
            if (_reference == null)
            {
                if (dossier.DFX6FP6BEAC==3)
                {
                    _reference = new ReferenceBanque()
                    {
                        NATURE = dossier.NatureOperation.ToString(),
                        BanqueId = banqueId,
                        NumeroRef = Reference
                    };
                    db.GetReferenceBanques.Add(_reference as ReferenceBanque);
                    dossier.ReferenceExterneId = _reference.Id;
                }
                else if (dossier.DFX6FP6BEAC==1)
                {
                    _reference = new DFX()
                    {
                        BanqueId = banqueId,
                        NumeroRef = Reference,
                        DateDebut=DateTime.Now.AddDays(-7),
                        DateFin=DateTime.Now
                    };
                    db.GetDFXs.Add(_reference as DFX);
                    dossier.DfxId = _reference.Id;
                }
            }
            db.SaveChanges();
            _reference.Dossiers.Add(dossier);
            await MailFunctions.ChangeEtapeDossier((int)banqueId, User.Identity.GetUserId(), 13, dossier, db, structure);

            db.SaveChanges();

            dossier = null;
            _reference = null;
            return Json(new string[] { "Réference attribuée avec succès...",Reference },JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> AddCourier(FormCollection form,
      HttpPostedFileBase fileCourrier = null, HttpPostedFileBase fileRecap = null)
        {
            var IdRef = int.Parse(form["IdRef"]);
            var mt=0;

            if (IdRef > 0)
            {
                try
                {
                    mt = int.Parse(form["MT"]);
                }
                catch (Exception)
                {}
                var reference = db.GetReferenceBanques.Include(r=>r.Dossiers).FirstOrDefault(r=>r.Id==IdRef);

                var structure = db.Structures.Find(Session["IdStructure"]);
                var idbanque = structure.BanqueId(db);
                genetrix.Models.Banque banque = new Models.Banque();

                try
                {
                    banque = db.GetBanques.Find(idbanque);
                }
                catch (Exception)
                { }
                try
                {
                    foreach (var d in reference.Dossiers)
                    {
                        if (d.EtapesDosier==13)
                        {
                            try
                            {
                               await MailFunctions.ChangeEtapeDossier((int)idbanque, User.Identity.GetUserId(), 14, d, db, structure);
                            }
                            catch (Exception)
                            {}                        
                        }
                    }
                }
                catch (Exception)
                {}
                //Courrier
                if (mt==0 && fileCourrier != null)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(fileCourrier.FileName))
                        {
                            var imageModel = new genetrix.Models.ImageDocumentAttache();
                            imageModel.Titre = "Courrier";

                            string chemin = Path.GetFileNameWithoutExtension(fileCourrier.FileName);
                            string extension = Path.GetExtension(fileCourrier.FileName);
                            chemin = imageModel.Titre + extension;
                            imageModel.Url = Path.Combine(CreateNewFolderDossier(reference.ClientId.ToString(), reference.Dossiers.FirstOrDefault().Intitulé), chemin);
                            if (reference.Courrier != null && reference.Get_Courrier != "#")
                            {
                                //Supprime l'ancienne image
                                try
                                {
                                    System.IO.File.Delete(reference.Courrier.GetImageDocumentAttache().Url);
                                }
                                catch (Exception)
                                { }
                            }

                            imageModel.NomCreateur = User.Identity.Name;

                            //Document attaché
                            var docAttache = new DocumentAttache();
                            docAttache.Nom = "Courrier";
                            //docAttache.ClientId = User.Identity.GetUserId();
                            docAttache.GetImageDocumentAttaches = new List<ImageDocumentAttache>();
                            docAttache.GetImageDocumentAttaches.Add(imageModel);
                            reference.Courrier = docAttache;
                            docAttache.IdReference = reference.Id;
                            docAttache.DateCreation = DateTime.Now;

                            chemin = null;
                            extension = null;

                            db.GetDocumentAttaches.Add(docAttache);
                            db.GetImageDocumentAttaches.Add(imageModel);
                            try
                            {
                                db.SaveChanges();
                                fileCourrier.SaveAs(imageModel.Url);
                            }
                            catch (Exception ee)
                            { }

                        }
                    }
                    catch (Exception)
                    {}
                }
                //Tableau recapitulatif des transferts
                 if (mt == 0 && fileRecap != null)
                {
                    if (!string.IsNullOrEmpty(fileRecap.FileName))
                    {
                        var imageModel = new genetrix.Models.ImageDocumentAttache();
                        imageModel.Titre = "Tableau recapitulatif des transferts";

                        string chemin = Path.GetFileNameWithoutExtension(fileRecap.FileName);
                        string extension = Path.GetExtension(fileRecap.FileName);
                        chemin = imageModel.Titre + extension;
                        imageModel.Url = Path.Combine(CreateNewFolderDossier(reference.ClientId.ToString(), reference.Dossiers.FirstOrDefault().Intitulé), chemin);
                        if (reference.RecapTransfert != null && reference.Get_Recap != "#")
                        {
                            //Supprime l'ancienne image
                            try
                            {
                                System.IO.File.Delete(reference.RecapTransfert.GetImageDocumentAttache().Url);
                            }
                            catch (Exception)
                            { }
                        }

                        imageModel.NomCreateur = User.Identity.Name;

                        //Document attaché
                        var docAttache = new DocumentAttache();
                        docAttache.Nom = "Recap des transfert";
                        //docAttache.ClientId = User.Identity.GetUserId();
                        docAttache.GetImageDocumentAttaches = new List<ImageDocumentAttache>();
                        docAttache.GetImageDocumentAttaches.Add(imageModel);
                        reference.RecapTransfert = docAttache;
                        docAttache.IdReference = reference.Id;
                        docAttache.DateCreation = DateTime.Now;

                        chemin = null;
                        extension = null;

                        db.GetDocumentAttaches.Add(docAttache);
                        db.GetImageDocumentAttaches.Add(imageModel);
                        try
                        {
                            db.SaveChanges();
                            fileRecap.SaveAs(imageModel.Url);
                        }
                        catch (Exception ee)
                        { }

                    }
                }
                //MT
                if (mt==1 && fileCourrier != null)
                {
                    if (!string.IsNullOrEmpty(fileCourrier.FileName))
                    {
                        var imageModel = new genetrix.Models.ImageDocumentAttache();
                        imageModel.Titre = "MT 298";

                        string chemin = Path.GetFileNameWithoutExtension(fileCourrier.FileName);
                        string extension = Path.GetExtension(fileCourrier.FileName);
                        chemin = imageModel.Titre + extension;
                        imageModel.Url = Path.Combine(CreateNewFolderDossier(reference.ClientId.ToString(), reference.Dossiers.FirstOrDefault().Intitulé), chemin);
                        if (reference.MT != null && reference.Get_MT != "#")
                        {
                            //Supprime l'ancienne image
                            try
                            {
                                System.IO.File.Delete(reference.MT.GetImageDocumentAttache().Url);
                            }
                            catch (Exception)
                            { }
                        }

                        imageModel.NomCreateur = User.Identity.Name;

                        //Document attaché
                        var docAttache = new DocumentAttache();
                        docAttache.Nom = "MT298";
                        //docAttache.ClientId = User.Identity.GetUserId();
                        docAttache.GetImageDocumentAttaches = new List<ImageDocumentAttache>();
                        docAttache.GetImageDocumentAttaches.Add(imageModel);
                        reference.MT = docAttache;
                        docAttache.IdReference = reference.Id;
                        docAttache.DateCreation = DateTime.Now;

                        chemin = null;
                        extension = null;

                        db.GetDocumentAttaches.Add(docAttache);
                        db.GetImageDocumentAttaches.Add(imageModel);
                        try
                        {
                            db.SaveChanges();
                            fileCourrier.SaveAs(imageModel.Url);
                        }
                        catch (Exception ee)
                        { }

                    }
                }
                return RedirectToAction("Details", new { id=IdRef });
            }
            var param = Session["composant"];
            return RedirectToAction("index", new { param });
        }

        private string CreateNewFolderDossier(string v, object intitulé)
        {
            throw new NotImplementedException();
        }

        private string CreateNewFolderDossier(string clientId, string intituleDossier)
        {
            try
            {
                string projectPath = "~/EspaceClient/" + clientId + "/Ressources/Transferts";
                string folderName = Path.Combine(Server.MapPath(projectPath), intituleDossier);
                System.IO.Directory.CreateDirectory(folderName);
                return folderName;
            }
            catch (Exception ee)
            { }
            return "";
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
                var structure = db.Structures.Find(Session["IdStructure"]);
                var idBanque = structure.BanqueId(db);
                try
                {
                    var referenceBanque = db.GetReferenceBanques.Include(r=>r.Dossiers).FirstOrDefault(r=>r.Id==model.Id);
                    referenceBanque.BanqueId= idBanque;

                    if (!referenceBanque.Apuré && false)
                    {
                        referenceBanque.Apuré = model.Apuré;
                        try
                        {
                            foreach (var item in referenceBanque.Dossiers)
                            {
                                try
                                {
                                    MailFunctions.ChangeEtapeDossier(idBanque, User.Identity.GetUserId(), 12, item, db, structure);
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

                    if (referenceBanque.EnvoieBEAC)
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
                                To = ges != null ? ges.Email : "",
                                CC = (from a in dossier.Client.Adresses select a.Email).ToList(),
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
                                    To = ges != null ? ges.Email : "",
                                    CC = (from a in dossier.Client.Adresses select a.Email).ToList(),
                                    Subject ="Référence "+model.NumeroRef+" "+model.NOTIFICATION,
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
                                    To = ges != null ? ges.Email : "",
                                    CC = (from a in dossier.Client.Adresses select a.Email).ToList(),
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

                    //Si non à la bEAC
                    if(referenceBanque.EtapesDosier<15)
                        referenceBanque.NumeroRef = model.NumeroRef;
                    referenceBanque.ObsBEAC = model.ObsBEAC;
                    referenceBanque.ObsTransmission = model.ObsTransmission;
                    //referenceBanque.Echus = model.Echus;

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
            try
            {
                ReferenceBanque referenceBanque = await db.GetReferenceBanques.FindAsync(id);
                var courrierId = referenceBanque.Courrier!=null? referenceBanque.Courrier.Id:0;
                if(courrierId>0)
                {
                    try
                    {
                        var cour = db.GetDocumentAttaches.Find(courrierId);
                        cour.Remove(db);
                        db.GetDocumentAttaches.Remove(cour);
                    }
                    catch (Exception)
                    {}
                }
                var miseEndemeureId = referenceBanque.MiseEnDemeure != null ? referenceBanque.MiseEnDemeure.Id : 0;
                if (miseEndemeureId > 0)
                {
                    try
                    {
                        var endemeure = db.GetDocumentAttaches.Find(miseEndemeureId);
                        endemeure.Remove(db);
                        db.GetDocumentAttaches.Remove(endemeure);
                    }
                    catch (Exception)
                    {}
                }
                var recapId = referenceBanque.RecapTransfert!=null? referenceBanque.RecapTransfert.Id:0;
                if(recapId>0)
                {
                    try
                    {
                        var cour = db.GetDocumentAttaches.Find(recapId);
                        cour.Remove(db);
                        db.GetDocumentAttaches.Remove(cour);
                    }
                    catch (Exception)
                    {}
                }
                var mtId = referenceBanque.MT!=null?referenceBanque.MT.Id:0;
                if (mtId > 0)
                {
                    try
                    {
                        var mt = db.GetDocumentAttaches.Find(mtId);
                        mt.Remove(db);
                        db.GetDocumentAttaches.Remove(mt);
                    }
                    catch (Exception)
                    { }
                }
                try
                {
                    int idbanque = 0;
                    Structure structure = null;
                    try
                    {
                        structure = db.Structures.Find(Session["IdStructure"]);
                        idbanque = structure.BanqueId(db);
                    }
                    catch (Exception)
                    { }
                    foreach (var item in referenceBanque.Dossiers)
                    {
                        await MailFunctions.ChangeEtapeDossier((int)idbanque, (string)Session["userId"], 11, item, db, structure, msg: "", itemsRejet: "");
                        item.ReferenceExterneId = null;
                    }
                    db.SaveChanges();
                }
                catch (Exception)
                {}
                referenceBanque.Courrier = null;
                referenceBanque.RecapTransfert = null;
                referenceBanque.MT = null;

                db.GetReferenceBanques.Remove(referenceBanque);
                await db.SaveChangesAsync();
            }
            catch (Exception ee)
            {}
            return RedirectToAction("Index");
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
