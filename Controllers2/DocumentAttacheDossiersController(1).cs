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

namespace eApurement.Controllers
{
    public class DocumentAttacheDossiersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DocumentAttacheDossiers
        public async Task<ActionResult> Index()
        {
            var GetDocumentAttacheDossiers = db.GetDocumentAttacheDossiers.Include(d => d.Client);
            return View(await GetDocumentAttacheDossiers.ToListAsync());
        }

        // GET: DocumentAttacheDossiers/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocumentAttacheDossier documentAttacheDossier = await db.GetDocumentAttacheDossiers.FindAsync(id);
            if (documentAttacheDossier == null)
            {
                return HttpNotFound();
            }
            return View(documentAttacheDossier);
        }

        // GET: DocumentAttacheDossiers/Create
        public ActionResult Create()
        {
            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom");
            return View();
        }

        // POST: DocumentAttacheDossiers/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Url,FichierImage,DateCreaApp,DateModif,ClientId")] DocumentAttacheDossier documentAttacheDossier)
        {
            if (ModelState.IsValid)
            {
                db.GetDocumentAttacheDossiers.Add(documentAttacheDossier);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom", documentAttacheDossier.ClientId);
            return View(documentAttacheDossier);
        }

        // GET: DocumentAttacheDossiers/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocumentAttacheDossier documentAttacheDossier = await db.GetDocumentAttacheDossiers.FindAsync(id);
            if (documentAttacheDossier == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom", documentAttacheDossier.ClientId);
            return View(documentAttacheDossier);
        }

        // POST: DocumentAttacheDossiers/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Url,FichierImage,DateCreaApp,DateModif,ClientId")] DocumentAttacheDossier documentAttacheDossier)
        {
            if (ModelState.IsValid)
            {
                db.Entry(documentAttacheDossier).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom", documentAttacheDossier.ClientId);
            return View(documentAttacheDossier);
        }

        // GET: DocumentAttacheDossiers/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocumentAttacheDossier documentAttacheDossier = await db.GetDocumentAttacheDossiers.FindAsync(id);
            if (documentAttacheDossier == null)
            {
                return HttpNotFound();
            }
            return View(documentAttacheDossier);
        }

        // POST: DocumentAttacheDossiers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            DocumentAttacheDossier documentAttacheDossier = await db.GetDocumentAttacheDossiers.FindAsync(id);
            db.GetDocumentAttacheDossiers.Remove(documentAttacheDossier);
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
