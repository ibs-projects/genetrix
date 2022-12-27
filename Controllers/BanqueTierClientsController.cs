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

namespace genetrix.Controllers
{
    public class BanqueTierClientsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BanqueTierClients
        public async Task<ActionResult> Index()
        {
            var banqueTierClients = db.banqueTierClients.Include(b => b.Client);
            return View(await banqueTierClients.ToListAsync());
        }

        // GET: BanqueTierClients/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BanqueTierClient banqueTierClient = await db.banqueTierClients.FindAsync(id);
            if (banqueTierClient == null)
            {
                return HttpNotFound();
            }
            return View(banqueTierClient);
        }

        // GET: BanqueTierClients/Create
        public ActionResult Create()
        {
            ViewBag.IdClient = new SelectList(db.GetClients, "Id", "Nom");
            return View();
        }

        public FileStreamResult GetPDF(int? idbanque, int?idclient=null,int? idDoc=null,bool estValide=false)
        {
            #region MyRegion
            if ((string)Session["userType"] == "CompteClient")
            {

            }
            var path = "";
            try
            {
                if (idDoc==null && estValide)
                    foreach (var d in db.banqueTierClients.Include(b => b.AttestationNonDefautAp).FirstOrDefault(b => b.Id == idbanque).AttestationNonDefautAp.ToList())
                    {
                        try
                        {
                            if (d.DateCreation != null && (DateTime.Now - d.DateSignature.Value).TotalDays < 30)
                            {
                                path = d.GetImageDocumentAttache().Url;
                                break;
                            }
                        }
                        catch (Exception e)
                        {}
                    }
                else
                    path = db.GetDocumentAttaches.Include(d => d.GetImageDocumentAttaches).FirstOrDefault(d => d.Id == idDoc).GetImageDocumentAttache().Url;
            }
            catch (Exception e)
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

            if (fs == null) return null;
            return File(fs, "application/pdf");
        }

        [HttpPost]
        public async Task<ActionResult> AddAttest(DocumentAttache document)
        {
            var msg = "";
            BanqueTierClient banqueTier = null;
            //if (Request.Files.Count > 0)
            {
                document.DateCreation = DateTime.Now;
                banqueTier = db.banqueTierClients.Find(document.IdBanqueTierce);
                document.IdBanqueTierce = banqueTier.Id;
                banqueTier.AttestationNonDefautAp = new List<DocumentAttache>();
                banqueTier.AttestationNonDefautAp.Add(document);
                document.TypeDocumentAttaché = TypeDocumentAttaché.AttestationNonDefautApurement;

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
                        imageModel.Url = Path.Combine(CreateNewFolderDossier(banqueTier.IdClient.ToString(), document.Nom), chemin);
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
            return RedirectToAction("Edit", new { id = banqueTier.Id });
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


        // POST: BanqueTierClients/Create
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Nom,Ville,Pays,Adresse,IdClient")] BanqueTierClient banqueTierClient)
        {
            if (ModelState.IsValid)
            {
                db.banqueTierClients.Add(banqueTierClient);
                await db.SaveChangesAsync();
                return RedirectToAction("Details", "Clients",new { id=banqueTierClient.IdClient});
            }

            ViewBag.IdClient = new SelectList(db.GetClients, "Id", "Nom", banqueTierClient.IdClient);
            return View(banqueTierClient);
        }

        // GET: BanqueTierClients/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BanqueTierClient banqueTierClient = await db.banqueTierClients.FindAsync(id);
            if (banqueTierClient == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdClient = new SelectList(db.GetClients, "Id", "Nom", banqueTierClient.IdClient);
            return View(banqueTierClient);
        }

        // POST: BanqueTierClients/Edit/5
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Nom,Ville,Pays,Adresse,IdClient")] BanqueTierClient banqueTierClient)
        {
            if (ModelState.IsValid)
            {
                db.Entry(banqueTierClient).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.IdClient = new SelectList(db.GetClients, "Id", "Nom", banqueTierClient.IdClient);
            return View(banqueTierClient);
        }

        // GET: BanqueTierClients/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BanqueTierClient banqueTierClient = await db.banqueTierClients.FindAsync(id);
            if (banqueTierClient == null)
            {
                return HttpNotFound();
            }
            return View(banqueTierClient);
        }

        // POST: BanqueTierClients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            BanqueTierClient banqueTierClient = await db.banqueTierClients.FindAsync(id);
            db.banqueTierClients.Remove(banqueTierClient);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [ActionName("document-client")]
        public ActionResult DeleteDoc(int? idBanque)
        {
            try
            {
                DocumentAttache document = null;
                foreach (var d in db.banqueTierClients.Include(b => b.AttestationNonDefautAp).FirstOrDefault(b => b.Id == idBanque).AttestationNonDefautAp.ToList())
                {
                    try
                    {
                        if (d.DateCreation != null && (DateTime.Now - d.DateSignature.Value).TotalDays < 30)
                        {
                            document = d;
                            break;
                        }
                    }
                    catch (Exception e)
                    { }
                }
                if (document!=null)
                {
                    db.GetDocumentAttaches.Remove(document);
                    db.SaveChanges(); 
                }
            }
            catch (Exception)
            { }
            return RedirectToAction("Edit", new { id = idBanque });
        }


        protected override void OnException(ExceptionContext filterContext)
        {
            if (Session != null)
            {
                var url = (string)Session["urlaccueil"];
                filterContext.Result = Redirect(url);
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
