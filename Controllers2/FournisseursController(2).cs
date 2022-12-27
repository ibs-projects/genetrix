using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using eApurement.Models;
using e_apurement.Models;
using System.IO;

namespace eApurement.Controllers
{
    public class FournisseursController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Fournisseurs
        /// <summary>
        /// /
        /// </summary>
        /// <param name="cl">Client id</param>
        /// <returns></returns>
        public async Task<ActionResult> Index(int? cl)
        {
            if (Session["user"] is CompteClient)
                cl = (Session["user"] as CompteClient).ClientId;
            if (cl == null || cl == 0)
                return View(new List<Fournisseurs>());
            var getFournisseurs = db.GetFournisseurs.Where(f=>f.ClientId==cl).Include(f => f.Client);
              return View(await getFournisseurs.ToListAsync());
        }

        // GET: Fournisseurs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fournisseurs fournisseurs = await db.GetFournisseurs.FindAsync(id);
            if (fournisseurs == null)
            {
                return RedirectToAction("Index");
                //return HttpNotFound();
            }
            fournisseurs.RCCM = db.GetDocumentations.FirstOrDefault(d => d.Nom == "RCCM" && d.IdFournisseur==id);
            fournisseurs.ListeGerants = db.GetDocumentations.FirstOrDefault(d => d.Nom == "ListeGerants" && d.IdFournisseur==id);

            return View(fournisseurs);
        }

        // GET: Fournisseurs/Create
        public ActionResult Create()
        {
            if (Session["user"] is null)
                RedirectToAction("Login", "Account");

            var clientId = (Session["user"] as CompteClient).ClientId;
          ViewBag.ClientId = new SelectList(db.GetClients.Where(c=>c.Id==clientId), "Id", "Nom");
            return View();
        }

        [HttpPost]
        public ActionResult Addfiles(int idFournisseur,HttpPostedFileBase RCCM, HttpPostedFileBase ListeGerants)
        {
            var clientId = (Session["user"] as CompteClient).ClientId;

            //Extrait RCCM ou autre document tenant lieu
            if (RCCM != null)
            {
                if (!string.IsNullOrEmpty(RCCM.FileName))
                {
                    var imageModel = new eApurement.Models.ImageDocumentAttache();
                    imageModel.Titre = "Extrait RCCM ou autre document tenant lieu";

                    string chemin = Path.GetFileNameWithoutExtension(RCCM.FileName);
                    string extension = Path.GetExtension(RCCM.FileName);
                    chemin = imageModel.Titre + extension;
                    imageModel.Url = Path.Combine(CreateNewFolderDossier(clientId.ToString(), ""+idFournisseur), chemin);

                    imageModel.NomCreateur = User.Identity.Name;

                    //Document attaché
                    var docAttache = new Documentation();
                    docAttache.Nom = "RCCM";
                    docAttache.ClientId = clientId;
                    docAttache.IdFournisseur = idFournisseur;
                    docAttache.GetImageDocumentAttaches = new List<ImageDocumentAttache>();
                    docAttache.GetImageDocumentAttaches.Add(imageModel);

                    chemin = null;
                    extension = null;

                    db.GetDocumentations.Add(docAttache);
                    db.GetImageDocumentAttaches.Add(imageModel);
                    db.SaveChanges();
                    RCCM.SaveAs(imageModel.Url);

                }
            }

            //Liste gérants de la société
            if (ListeGerants != null)
            {
                if (!string.IsNullOrEmpty(ListeGerants.FileName))
                {
                    var imageModel = new eApurement.Models.ImageDocumentAttache();
                    imageModel.Titre = "Liste gérants de la société";

                    string chemin = Path.GetFileNameWithoutExtension(ListeGerants.FileName);
                    string extension = Path.GetExtension(ListeGerants.FileName);
                    chemin = imageModel.Titre + extension;
                    imageModel.Url = Path.Combine(CreateNewFolderDossier(clientId.ToString(), ""+idFournisseur), chemin);

                    imageModel.NomCreateur = User.Identity.Name;

                    //Document attaché
                    var docAttache = new Documentation();
                    docAttache.Nom = "ListeGerants";
                    docAttache.ClientId = clientId;
                    docAttache.IdFournisseur = idFournisseur;
                    docAttache.GetImageDocumentAttaches = new List<ImageDocumentAttache>();
                    docAttache.GetImageDocumentAttaches.Add(imageModel);

                    chemin = null;
                    extension = null;

                    db.GetDocumentations.Add(docAttache);
                    db.GetImageDocumentAttaches.Add(imageModel);
                    db.SaveChanges();
                    ListeGerants.SaveAs(imageModel.Url);

                }
            }


            return RedirectToAction("Details", new { id = idFournisseur });
        }


        private string CreateNewFolderDossier(string clientId, string intituleDossier)
        {
            //intituleDossier: fournisseurId
            try
            {
                string projectPath = "~/EspaceClient/" + clientId + "/Ressources/Fournisseurs";
                string folderName = Path.Combine(Server.MapPath(projectPath), intituleDossier);
                System.IO.Directory.CreateDirectory(folderName);
                return folderName;
            }
            catch (Exception ee)
            { }
            return "";
        }


        // POST: Fournisseurs/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Nom,Tel2,Tel1,ClientId,Pays")] Fournisseurs fournisseurs)
        {
            if (ModelState.IsValid)
            {
                db.GetFournisseurs.Add(fournisseurs);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom", fournisseurs.ClientId);
            return View(fournisseurs);
        }

        // GET: Fournisseurs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fournisseurs fournisseurs = await db.GetFournisseurs.FindAsync(id);
            if (fournisseurs == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom", fournisseurs.ClientId);
            return View(fournisseurs);
        }

        // POST: Fournisseurs/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Nom,Tel2,Tel1,ClientId")] Fournisseurs fournisseurs)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fournisseurs).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom", fournisseurs.ClientId);
            return View(fournisseurs);
        }

        // GET: Fournisseurs/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fournisseurs fournisseurs = await db.GetFournisseurs.FindAsync(id);
            if (fournisseurs == null)
            {
                return HttpNotFound();
            }
            return View(fournisseurs);
        }

        // POST: Fournisseurs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Fournisseurs fournisseurs = await db.GetFournisseurs.FindAsync(id);
            db.GetFournisseurs.Remove(fournisseurs);
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
