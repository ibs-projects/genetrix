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
using System.IO;
using System.Data.Entity.Validation;

namespace genetrix.Controllers
{
    public class DocumentAttachesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DocumentAttaches
        public async Task<ActionResult> Index()
        {
            var getDocumentAttaches = db.GetDocumentAttaches;//.Include(d => d.Client);
            return View(await getDocumentAttaches.ToListAsync());
        }

        // GET: DocumentAttaches/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocumentAttache documentAttache = await db.GetDocumentAttaches.Include(d=>d.Dossier).FirstOrDefaultAsync(d=>d.Id==id);
            if (documentAttache == null)
            {
                return HttpNotFound();
            }
            return View(documentAttache);
        }
        
        public async Task<ActionResult> GetDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocumentAttache documentAttache = await db.GetDocumentAttaches.FindAsync(id);
            if (documentAttache == null)
            {
                return HttpNotFound();
            }
            return PartialView(documentAttache);
        }

        // GET: DocumentAttaches/Create
        public ActionResult Create()
        {
            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom");
            return View();
        }

        // POST: DocumentAttaches/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Nom,NomBreDoc,ClientId,TypeDocumentAttaché")] DocumentAttache documentAttache)
        {
            if (ModelState.IsValid)
            {
                db.GetDocumentAttaches.Add(documentAttache);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            //ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom", documentAttache.ClientId);
            return View(documentAttache);
        }

        [HttpPost]
        public async Task<JsonResult> CreateJs([Bind(Include = "Nom,DossierId,typeDocument,Id,NomBreDoc,ClientId,TypeDocumentAttaché")] DocumentAttache document)
        {
            var msg = ""; 
            Dossier dossier = null;
            //if (Request.Files.Count > 0)
            {
                document.DateCreation = DateTime.Now;
                dossier = db.GetDossiers.Find(document.DossierId);
                document.DossierId = dossier.Dossier_Id;
                if (document.typeDocument == 0)
                {
                    document.TypeDocumentAttaché = TypeDocumentAttaché.DocumentTransport;
                    dossier.DocumentsTransport.Add(document);
                }
                else
                {
                    document.TypeDocumentAttaché = TypeDocumentAttaché.AutreDoc;
                    dossier.DocumentAttaches.Add(document);
                }
                var typeDoc = document.typeDocument;
                db.GetDocumentAttaches.Add(document);

                await db.SaveChangesAsync();
                if (Request.Files != null)
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        var imgJustif = Request.Files[i];

                        var imageModel = new genetrix.Models.ImageDocumentAttache();
                        imageModel.Titre = document.Nom;

                        string chemin = Path.GetFileNameWithoutExtension(imgJustif.FileName);
                        string extension = Path.GetExtension(imgJustif.FileName);
                        chemin = imageModel.Titre + extension;
                        imageModel.Url = Path.Combine(CreateNewFolderDossier(document.Dossier.ClientId.ToString(), document.Dossier.Intitulé), chemin);
                        imageModel.NomCreateur = User.Identity.Name;
                        imageModel.DocumentAttache = document;
                        db.GetImageDocumentAttaches.Add(imageModel);
                        //db.GetDossiers.Add(dossier);
                        db.SaveChanges();
                        imgJustif.SaveAs(imageModel.Url);
                        chemin = null;
                        extension = null;
                    }
                if (false)
                    foreach (var tmp in Request.Files)
                    {
                        var imgJustif = tmp as HttpPostedFileBase;
                        if (!string.IsNullOrEmpty(imgJustif.FileName))
                        {
                            var imageModel = new genetrix.Models.ImageDocumentAttache();
                            imageModel.Titre = document.Nom;

                            string chemin = Path.GetFileNameWithoutExtension(imgJustif.FileName);
                            string extension = Path.GetExtension(imgJustif.FileName);
                            chemin = imageModel.Titre + extension;
                            //imageModel.Url = Path.Combine(Server.MapPath(CreateNewFolderDossier(dossier.ClientId.ToString(),dossier.Intitulé)), chemin);
                            imageModel.Url = Path.Combine(CreateNewFolderDossier(document.Dossier.ClientId.ToString(), document.Dossier.Intitulé), chemin);
                            imageModel.NomCreateur = User.Identity.Name;
                            imageModel.DocumentAttache = document;
                            db.GetImageDocumentAttaches.Add(imageModel);
                            //db.GetDossiers.Add(dossier);
                            db.SaveChanges();
                            imgJustif.SaveAs(imageModel.Url);
                            chemin = null;
                            extension = null;
                        }
                    }
                return Json(new { document.Nom,document.Id, typeDoc }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetDetailJS(int id)
        {
            var model = db.GetDocumentAttaches.Include(j => j.GetImageDocumentAttaches).FirstOrDefault(j => j.Id == id);
            return PartialView("ViewDetails", model);
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

        // GET: DocumentAttaches/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocumentAttache documentAttache = await db.GetDocumentAttaches.FindAsync(id);
            if (documentAttache == null)
            {
                return HttpNotFound();
            }
            //ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom", documentAttache.ClientId);
            return View(documentAttache);
        }

        // POST: DocumentAttaches/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Nom,NomBreDoc,ClientId,TypeDocumentAttaché")] DocumentAttache documentAttache)
        {
            if (ModelState.IsValid)
            {
                db.Entry(documentAttache).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            //ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom", documentAttache.ClientId);
            return View(documentAttache);
        }

        public FileStreamResult GetPDF(int idDoc = 0)
        {
            #region MyRegion
            var path = "";
            var estPdf = false;
            try
            {
                DocumentAttache doc = db.GetDocumentAttaches.Include(d => d.GetImageDocumentAttaches).FirstOrDefault(d => d.Id == idDoc);
                estPdf = doc.EstPdf;
                path = doc.GetImageDocumentAttache().Url;
            }
            catch (Exception)
            { }
            #endregion
            FileStream fs = null;
            try
            {
                fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            }
            catch (Exception)
            {
                path = null;
            }
            if (string.IsNullOrEmpty(path))
            {
                path = Path.Combine(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), @"assets\images\loading-error.jpg");
                try
                {
                    fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                }
                catch (Exception e)
                {
                }
            }
            if (string.IsNullOrEmpty(path)) return null;
            if (!estPdf)
                return File(fs, "image/jpeg");
            return File(fs, "application/pdf");
        }

        // GET: DocumentAttaches/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocumentAttache documentAttache = await db.GetDocumentAttaches.FindAsync(id);
            if (documentAttache == null)
            {
                return HttpNotFound();
            }
            return View(documentAttache);
        }

        // POST: DocumentAttaches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int? id)
        {
            DocumentAttache documentAttache = await db.GetDocumentAttaches.FindAsync(id);
            db.GetDocumentAttaches.Remove(documentAttache);
            await db.SaveChangesAsync();
            if(documentAttache.DossierId!=null)
                return RedirectToAction("Edit","Dossiers",new { id= documentAttache.DossierId, msg="" });
            return RedirectToAction("Index", "Index");
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (Session == null)
            {
                filterContext.Result = RedirectToAction("DefaultPage", "Archivage2");
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
