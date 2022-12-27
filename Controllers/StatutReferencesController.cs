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

namespace genetrix.Controllers
{
    public class StatutReferencesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: StatutReferences
        public async Task<ActionResult> Index()
        {
            return View(await db.GetStatutReferences.ToListAsync());
        }

        // GET: StatutReferences/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StatutReference statutReference = await db.GetStatutReferences.FindAsync(id);
            if (statutReference == null)
            {
                return HttpNotFound();
            }
            return View(statutReference);
        }

        // GET: StatutReferences/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StatutReferences/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,EtatDossier,Message,Titre,Image")] StatutReference statutReference)
        {
            if (ModelState.IsValid)
            {
                db.GetStatutReferences.Add(statutReference);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(statutReference);
        }

        // GET: StatutReferences/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StatutReference statutReference = await db.GetStatutReferences.FindAsync(id);
            if (statutReference == null)
            {
                return HttpNotFound();
            }
            return View(statutReference);
        }

        // POST: StatutReferences/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,EtatDossier,Message,Titre,Image")] StatutReference statutReference)
        {
            if (ModelState.IsValid)
            {
                db.Entry(statutReference).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(statutReference);
        }

        // GET: StatutReferences/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StatutReference statutReference = await db.GetStatutReferences.FindAsync(id);
            if (statutReference == null)
            {
                return HttpNotFound();
            }
            return View(statutReference);
        }

        // POST: StatutReferences/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            StatutReference statutReference = await db.GetStatutReferences.FindAsync(id);
            db.GetStatutReferences.Remove(statutReference);
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
