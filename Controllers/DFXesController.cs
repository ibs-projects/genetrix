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

namespace genetrix.Controllers
{
    public class DFXesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DFXes
        public async Task<ActionResult> Index(bool reference = false, string st = "", string comp = "", string type = "")
        {
            List<DFX> getReferences = new List<DFX>();
            List<DFX> Dfxes = new List<DFX>();
            var agenceID = Convert.ToInt32(Session["IdStructure"]);
            var site = db.Structures.Find(agenceID);
            var banqueID = site.BanqueId(db);
            var role = db.XtraRoles.Find(Session["IdXRole"]);
            var gesId = (string)Session["userId"];

            db.GetDFXs.Include(d => d.Banque).ToList().ForEach(d =>
            {
                if (d.EtapesDosier != 26 && d.EtapesDosier != 27 && d.UserAsPermition(db, site, role, banqueID, gesId))
                {
                    d.DossiersPermitions(db, site, role, banqueID, gesId);
                    Dfxes.Add(d);
                }
            });
            switch (st)
            {
                case "aapurer":
                    foreach (var r in Dfxes)
                        if (r.AApurer)
                            getReferences.Add(r);
                    ViewBag.info = " à apurer";
                    ViewBag.st = "apurement";
                    break;
                case "aapurer-acc":
                    foreach (var r in Dfxes)
                        if (r.AApurer_Ac)
                            getReferences.Add(r);
                    ViewBag.info = "à apurer (à completer)";
                    ViewBag.st = "apurement";
                    break;
                case "aapurer-av":
                    foreach (var r in Dfxes)
                        if (r.AApurer_Av)
                            getReferences.Add(r);
                    ViewBag.info = "à apurer (attente validation)";
                    ViewBag.st = "apurement";
                    break;
                case "echu":
                    foreach (var r in Dfxes)
                        if (r.Echus)
                            getReferences.Add(r);
                    ViewBag.info = "echu(s)";
                    ViewBag.st = "apurement";
                    break;
                case "aapurer-beac":
                    foreach (var r in Dfxes)
                        if (r.EtapesDosier == 232)
                        {
                            getReferences.Add(r);
                        }
                    ViewBag.info = "à apurer (en cours d'analyse BEAC)";
                    ViewBag.st = "apurement";
                    break;
                case "apure":
                    foreach (var r in Dfxes)
                        if (r.EtapesDosier == 24)
                            getReferences.Add(r);
                    ViewBag.info = "apuré";
                    ViewBag.st = "apurement";
                    break;
                case "arch":
                    foreach (var r in Dfxes)
                    {
                        if (r.EtapesDosier == 26)
                            getReferences.Add(r);
                    }
                    ViewBag.info = " archivées";
                    break;
                case "list":
                    foreach (var r in Dfxes)
                    {
                        if (r.EtapesDosier <= 22)
                            getReferences.Add(r);
                    }
                    ViewBag.info = "";
                    break;
                case "lbre":
                    foreach (var re in Dfxes)
                        try
                        {
                            if (re.Dossiers.Count == 0)
                                getReferences.Add(re);
                        }
                        catch (Exception)
                        { }
                    ViewBag.info = " Refinancement";
                    break;
                case "env_bac":
                    foreach (var r in Dfxes)
                    {
                        try
                        {
                            if (r.EtapesDosier > 15)
                                getReferences.Add(r);
                        }
                        catch (Exception)
                        { }
                    }
                    ViewBag.info = " envoyés à la BEAC";
                    break;
                case "à envoyer BEAC":
                case "aenv_bac":
                    foreach (var r in Dfxes)
                    {
                        try
                        {
                            if (r.EtapesDosier == 13 || r.EtapesDosier == 14)
                                getReferences.Add(r);
                        }
                        catch (Exception)
                        { }
                    }
                    ViewBag.info = " à envoyer à la BEAC";
                    break;
                case "accord":
                    foreach (var r in Dfxes)
                    {
                        try
                        {
                            if (r.EtapesDosier == 20)
                                getReferences.Add(r);
                        }
                        catch (Exception)
                        { }
                    }
                    ViewBag.info = " accordés";
                    break;
                case "liee":
                    foreach (var re in Dfxes)
                        try
                        {
                            if (re.Dossiers.Count > 0)
                                getReferences.Add(re);
                        }
                        catch (Exception)
                        { }
                    ViewBag.info = " liées";
                    break;
                default:
                    foreach (var r in Dfxes)
                    {
                        try
                        {
                            if (r.EtapesDosier <= 22)
                                getReferences.Add(r);
                        }
                        catch (Exception)
                        { }
                    }
                    ViewBag.info = " refinancement";
                    break;
            }
            Dfxes = null;
            return View(getReferences.ToList());
        }

        public ActionResult DownloadZipFile(int referenceId)
        {
            var reference = db.GetDFXs.Include(r => r.Dossiers).FirstOrDefault(r => r.Id == referenceId);
            var dossiers = reference.Dossiers;

            string nomClientzip = reference.GetClient + "_" + reference.GetFournisseur, nomFournzip = reference.GetFournisseur;

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
                            { }

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
                                ziparchive.CreateEntryFromFile(file.FullName, nomClientzip + "/Documents_Client" + "/" + file.Name);

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
                { }
                return File(memoryStream.ToArray(), "application/zip", nomClientzip + ".zip");
            }
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


        private string GetClientDossierFolder(int clientId, Dossier dossier, bool entreprise = false)
        {
            try
            {
                string projectPath = "", folderName = "";
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

        // GET: DFXes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DFX dFX = await db.GetDFXs.FindAsync(id);
            if (dFX == null)
            {
                return HttpNotFound();
            }

            try
            {
                var agenceID = Convert.ToInt32(Session["IdStructure"]);
                Structure site = db.Structures.Find(agenceID);
                var banqueID = site.BanqueId(db);
                var agentId = (string)Session["userId"];
                var role = db.XtraRoles.Find(Session["IdXRole"]);

                //dFX.Dossiers.ToList().ForEach(d =>
                //{
                //    if (Fonctions.DroitSurDossier(db, d, site, role, banqueID, agentId))
                //    {
                //        _dossiers.Add(d);nbrdoss++;
                //    }
                //});
                dFX.DossiersPermitions(db, site, role, banqueID, agentId);
            }
            catch (Exception)
            { }
            ViewBag._dossiers = dFX.DossiersPermi!=null?dFX.DossiersPermi:new List<Dossier>();
            ViewBag.nbrdoss = dFX.DossiersPermi != null ? dFX.DossiersPermi.Count :0 ;
            return View(dFX);
        }

        // GET: DFXes/Create
        public ActionResult Create(string ids)
        {

            DFX dFX = new DFX();
            dFX.GetIdsDossiers = ids;
            var lesId = ids.Split(';');
            foreach (var item in lesId)
            {
                try
                {
                    int id = Convert.ToInt32(item);
                    var doss = db.GetDossiers.Find(id);
                    if (doss != null)
                        dFX.Dossiers.Add(doss);
                }
                catch (Exception)
                { }
            }
            int idbanque = 0;
            try
            {
                var structure = db.Structures.Find(Session["IdStructure"]);
                idbanque = structure.BanqueId(db);
                dFX.BanqueId = idbanque;
            }
            catch (Exception)
            { }
            ViewBag.Correspondants = new SelectList(db.GetCorrespondants.Where(c=>c.BanqueId==idbanque), "Id", "NomBanque");
            return View(dFX);
        }

        // POST: DFXes/Create
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(DFX dFX)
        {
            if (ModelState.IsValid)
            {
                int idbanque = 0;
                Structure structure = null;
                try
                {
                    structure = db.Structures.Find(Session["IdStructure"]);
                    idbanque = structure.BanqueId(db);
                    dFX.BanqueId = idbanque;
                }
                catch (Exception)
                { }

                dFX =db.GetDFXs.Add(dFX);
                try
                {
                    await db.SaveChangesAsync();
                    if (!string.IsNullOrEmpty(dFX.GetIdsDossiers))
                    {
                        try
                        {
                            var lesId = dFX.GetIdsDossiers.Split(';');
                            foreach (var item in lesId)
                            {
                                try
                                {
                                    int id = Convert.ToInt32(item);
                                    var doss = db.GetDossiers.Find(id);
                                    if (doss != null)
                                    {
                                        doss.DfxId = dFX.Id;
                                        doss.NumDFX = dFX.Numero;
                                        doss.RefDFX = dFX.NumeroRef;
                                        await MailFunctions.ChangeEtapeDossier((int)idbanque, (string)Session["userId"], 14, doss, db, structure, msg:"", itemsRejet: "");
                                        dFX.Dossiers.Add(doss);
                                    }
                                }
                                catch (Exception)
                                { }
                            }
                            await db.SaveChangesAsync();
                        }
                        catch (Exception)
                        { }
                    }
                    return RedirectToAction("Details",new { id=dFX.Id});
                }
                catch (Exception ee)
                {}
            }

            ViewBag.BanqueId = new SelectList(db.Structures, "Id", "Nom", dFX.BanqueId);
            return View(dFX);
        }

        // GET: DFXes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DFX dFX = await db.GetDFXs.FindAsync(id);
            if (dFX == null)
            {
                return HttpNotFound();
            }
            var structure = db.Structures.Find(Session["IdStructure"]);
            var idbanque = structure.BanqueId(db);
            ViewBag.BanqueId = new SelectList(db.Structures, "Id", "Nom", dFX.BanqueId);
            ViewBag.Correspondants = new SelectList(db.GetCorrespondants.Where(c => c.BanqueId == idbanque), "Id", "NomBanque");
            return View(dFX);
        }

        // POST: DFXes/Edit/5
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit( DFX dFX)
        {
            if (ModelState.IsValid)
            {
                var model=await db.GetDFXs.FindAsync(dFX.Id);
                model.NumeroRef = dFX.NumeroRef;
                model.DateReception = dFX.DateReception;
                model.NumeroAnnexe = dFX.NumeroAnnexe;
                model.Numero = dFX.Numero;
                model.NumeroRef = dFX.NumeroRef;
                model.DateDebut = dFX.DateDebut;
                model.DateFin = dFX.DateFin;
                model.Pays = dFX.Pays;
                model.CorrespondantB = dFX.CorrespondantB;
                db.Entry(model).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.BanqueId = new SelectList(db.Structures, "Id", "Nom", dFX.BanqueId);
            return View(dFX);
        }

        [HttpGet]
        public async Task<ActionResult> ModifEtatDossierJS(int? etat = null, int? idDfx = null, int? idOpSwft = null, string message = "", string date = "")
        {
            var result = "Opération effectuée avec succes";
            DFX dfx = null;
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
                if (idDfx > 0)
                {
                    dfx = db.GetDFXs.Include(d => d.Dossiers).FirstOrDefault(r => r.Id == idDfx);

                    if (dfx.Dossiers.Count > 0)
                    {
                        try
                        {
                            var etatmp = etat;
                           await MailFunctions.ChangeEtapeDossierDFX((int)idbanque, (string)Session["userId"], (int)etat, dfx, db, structure, rejet: true, msg: message, idOpSwft);
                        }
                        catch (Exception)
                        { }
                    }

                }
            }
            catch (Exception)
            {
                //result = "erreur";
            }
            if (idDfx != null) return RedirectToAction("Details",new { id=idDfx});
            return Redirect("~/dfxes/Index?");
        }

        [HttpGet]
        public async Task<ActionResult> GenereCourier(int id)
        {
            DFX reference =await db.GetDFXs.Include(r => r.Dossiers).FirstOrDefaultAsync(r => r.Id == id);
            var structure = db.Structures.Find(Session["IdStructure"]);
            var idbanque = structure.BanqueId(db);

            ViewBag.compteXaf = (from c in db.GetCompteXAFs
                                 where c.BanqueId == idbanque
                                 select c).ToList();

            ViewBag.correspondants = (from c in db.GetCorrespondants
                                      where c.BanqueId == idbanque
                                      select c).ToList();

            ViewBag.signataires = db.GetSignataires.Where(s => s.BanqueId == idbanque).ToList();
            try
            {
                var idBanqueXaf = Convert.ToInt32(reference.CompteBEACEditer);
                reference.CompteBEACEditer = db.GetCompteXAFs.Find(idBanqueXaf).Libellé;
            }
            catch (Exception)
            { }

            try
            {
                var idCorresp = Convert.ToInt32(reference.BanqueDomiciliaire);
                reference.BanqueDomiciliaire = db.GetCorrespondants.Find(idCorresp).NomBanque;
            }
            catch (Exception)
            { }
            try
            {
                ViewBag.logo = (await db.GetBanques.FindAsync(idbanque)).Image;
            }
            catch (Exception)
            { }
            try
            {
                var _id = Convert.ToInt32(reference.Signataire3);
                reference.Signataire3 = db.GetSignataires.Find(_id).NomComplet;
                reference.fonction3 = db.GetSignataires.Find(_id).Fonction;
            }
            catch (Exception)
            { }


            try
            {
                var _id = Convert.ToInt32(reference.Signataire4);
                reference.Signataire4 = db.GetSignataires.Find(_id).NomComplet;
                reference.fonction4 = db.GetSignataires.Find(_id).Fonction;
            }
            catch (Exception)
            { }
            try
            {
                var _id = Convert.ToInt32(reference.IdCompteAcrediter);
                reference.CompteanqueACrediter = db.CompteNostroes.Find(_id).Libellé;
            }
            catch (Exception)
            { }

            try
            {
                reference.GetBanque = db.Structures.Find(idbanque).Nom;
            }
            catch (Exception)
            { }

            return PartialView("EditCourrier", reference);
        }

        [HttpPost]
        public ActionResult EditCourrier(DFX model)
        {
            try
            {
                DFX reference = db.GetDFXs.Find(model.Id);
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
                db.SaveChanges();
            }
            catch (Exception)
            { }
            return RedirectToAction("Details", new { model.Id });
        }

        public ActionResult GetElets(int id, string type)
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
                        var sign = db.GetSignataires.Find(id);
                        return Json(new { sign.Rang, sign.NomComplet, sign.Fonction, type = type }, JsonRequestBehavior.AllowGet);
                    case "bd":
                        var corresp = db.GetCorrespondants.Find(id);
                        var comptes = from c in corresp.GetCompteNostros
                                      select new { c.Id, c.Libellé };
                        return Json(new { corresp.NomBanque, comptes, corresp.SwiftCode, corresp.Pays, corresp.Ville, type = type }, JsonRequestBehavior.AllowGet);
                    default:
                        break;
                }
            }
            catch (Exception)
            { }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetNostro(int idCoresspond)
        {
            var coress = db.GetCorrespondants.Include(c => c.GetCompteNostros).FirstOrDefault(c => c.Id == idCoresspond);

            return Json(coress.GetCompteNostros.ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddCourier(FormCollection form,
     HttpPostedFileBase fileCourrier = null)
        {
            var IdRef = int.Parse(form["IdRef"]);
            var mt = 0;

            if (IdRef > 0)
            {
                try
                {
                    mt = int.Parse(form["MT"]);
                }
                catch (Exception)
                { }
                var reference = db.GetDFXs.Include(r => r.Dossiers).FirstOrDefault(r => r.Id == IdRef);

                var structure = db.Structures.Find(Session["IdStructure"]);
                var idbanque = structure.BanqueId(db);
                genetrix.Models.Banque banque = new Models.Banque();

                try
                {
                    banque = db.GetBanques.Find(idbanque);
                }
                catch (Exception)
                { }
                
                //Courrier
                if (mt == 0 && fileCourrier != null)
                {
                    if (!string.IsNullOrEmpty(fileCourrier.FileName))
                    {
                        var imageModel = new genetrix.Models.ImageDocumentAttache();
                        imageModel.Titre = "Courrier "+reference.Numero;

                        string chemin = Path.GetFileNameWithoutExtension(fileCourrier.FileName);
                        string extension = Path.GetExtension(fileCourrier.FileName);
                        chemin = imageModel.Titre + extension;
                        imageModel.Url = Path.Combine(CreateNewFolderDossier(reference.Numero, reference.Numero), chemin);
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
                return RedirectToAction("Details", new { id = IdRef });
            }
            return RedirectToAction("index");
        }

        private string CreateNewFolderDossier(string clientId, string intituleDossier)
        {
            try
            {
                string projectPath = "~/EspaceClient/" + clientId + "/Ressources/Transferts/DFX";
                string folderName = Path.Combine(Server.MapPath(projectPath), intituleDossier);
                System.IO.Directory.CreateDirectory(folderName);
                return folderName;
            }
            catch (Exception ee)
            { }
            return "";
        }

        // GET: DFXes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DFX dFX = await db.GetDFXs.FindAsync(id);
            if (dFX == null)
            {
                return HttpNotFound();
            }
            return View(dFX);
        }

        // POST: DFXes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            DFX dFX = await db.GetDFXs.FindAsync(id);
            int idbanque = 0;
            Structure structure = null;
            try
            {
                structure = db.Structures.Find(Session["IdStructure"]);
                idbanque = structure.BanqueId(db);
                dFX.BanqueId = idbanque;
            }
            catch (Exception)
            { }

            var courrierId = dFX.Courrier != null ? dFX.Courrier.Id : 0;
            if (courrierId > 0)
            {
                try
                {
                    var cour = db.GetDocumentAttaches.Find(courrierId);
                    cour.Remove(db);
                    db.GetDocumentAttaches.Remove(cour);
                }
                catch (Exception)
                { }
            }
            var miseEndemeureId = dFX.MiseEnDemeure != null ? dFX.MiseEnDemeure.Id : 0;
            if (miseEndemeureId > 0)
            {
                try
                {
                    var endemeure = db.GetDocumentAttaches.Find(miseEndemeureId);
                    endemeure.Remove(db);
                    db.GetDocumentAttaches.Remove(endemeure);
                }
                catch (Exception)
                { }
            }

            try
            {
                foreach (var dos in dFX.Dossiers)
                {
                    try
                    {
                        if(dos.EtapesDosier>0 &&dos.EtapesDosier<22 )
                        await MailFunctions.ChangeEtapeDossier((int)idbanque, (string)Session["userId"], 11, dos, db, structure, msg: "", itemsRejet: "");
                        dos.DfxId = null;
                    }
                    catch (Exception)
                    { }
                }
            }
            catch (Exception)
            { }
            dFX.Dossiers.Clear();
            db.GetDFXs.Remove(dFX);
            await db.SaveChangesAsync();
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
