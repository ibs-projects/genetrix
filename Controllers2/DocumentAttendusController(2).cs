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
    public class DocumentAttendusController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DocumentAttendus
        public async Task<ActionResult> Index()
        {
            var getDocumentAttendus = db.GetDocumentAttendus.Include(d => d.Reference);
            return View(await getDocumentAttendus.ToListAsync());
        }

        // GET: DocumentAttendus/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocumentAttendus documentAttendus = await db.GetDocumentAttendus.FindAsync(id);
            if (documentAttendus == null)
            {
                return HttpNotFound();
            }
            return View(documentAttendus);
        }

        // GET: DocumentAttendus/Create
        public ActionResult Create()
        {
            ViewBag.ReferenceBanqueId = new SelectList(db.GetReferenceBanques, "Id", "NumeroRef");
            return View();
        }

        // POST: DocumentAttendus/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Intitulé,ReferenceBanqueId")] DocumentAttendus documentAttendus)
        {
            if (ModelState.IsValid)
            {
                db.GetDocumentAttendus.Add(documentAttendus);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ReferenceBanqueId = new SelectList(db.GetReferenceBanques, "Id", "NumeroRef", documentAttendus.ReferenceBanqueId);
            return View(documentAttendus);
        }

        // GET: DocumentAttendus/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocumentAttendus documentAttendus = await db.GetDocumentAttendus.FindAsync(id);
            if (documentAttendus == null)
            {
                return HttpNotFound();
            }
            ViewBag.ReferenceBanqueId = new SelectList(db.GetReferenceBanques, "Id", "NumeroRef", documentAttendus.ReferenceBanqueId);
            return View(documentAttendus);
        }

        // POST: DocumentAttendus/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Intitulé,ReferenceBanqueId")] DocumentAttendus documentAttendus)
        {
            if (ModelState.IsValid)
            {
                db.Entry(documentAttendus).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ReferenceBanqueId = new SelectList(db.GetReferenceBanques, "Id", "NumeroRef", documentAttendus.ReferenceBanqueId);
            return View(documentAttendus);
        }

        // GET: DocumentAttendus/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocumentAttendus documentAttendus = await db.GetDocumentAttendus.FindAsync(id);
            if (documentAttendus == null)
            {
                return HttpNotFound();
            }
            return View(documentAttendus);
        }

        // POST: DocumentAttendus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            DocumentAttendus documentAttendus = await db.GetDocumentAttendus.FindAsync(id);
            db.GetDocumentAttendus.Remove(documentAttendus);
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
