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
        public async Task<ActionResult> Details(string id)
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

        // GET: DocumentAttaches/Edit/5
        public async Task<ActionResult> Edit(string id)
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

        // GET: DocumentAttaches/Delete/5
        public async Task<ActionResult> Delete(string id)
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
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            DocumentAttache documentAttache = await db.GetDocumentAttaches.FindAsync(id);
            db.GetDocumentAttaches.Remove(documentAttache);
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
