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
    [Authorize]
    public class Dossier_StatutDossierController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Dossier_StatutDossier
        public async Task<ActionResult> Index()
        {
            return View(await db.GetDossier_StatutDossiers.ToListAsync());
        }

        // GET: Dossier_StatutDossier/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dossier_StatutDossier dossier_StatutDossier = await db.GetDossier_StatutDossiers.FindAsync(id);
            if (dossier_StatutDossier == null)
            {
                return HttpNotFound();
            }
            return View(dossier_StatutDossier);
        }

        // GET: Dossier_StatutDossier/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Dossier_StatutDossier/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Date,Motif")] Dossier_StatutDossier dossier_StatutDossier)
        {
            if (ModelState.IsValid)
            {
                db.GetDossier_StatutDossiers.Add(dossier_StatutDossier);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(dossier_StatutDossier);
        }

        // GET: Dossier_StatutDossier/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dossier_StatutDossier dossier_StatutDossier = await db.GetDossier_StatutDossiers.FindAsync(id);
            if (dossier_StatutDossier == null)
            {
                return HttpNotFound();
            }
            return View(dossier_StatutDossier);
        }

        // POST: Dossier_StatutDossier/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Date,Motif")] Dossier_StatutDossier dossier_StatutDossier)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dossier_StatutDossier).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(dossier_StatutDossier);
        }

        // GET: Dossier_StatutDossier/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dossier_StatutDossier dossier_StatutDossier = await db.GetDossier_StatutDossiers.FindAsync(id);
            if (dossier_StatutDossier == null)
            {
                return HttpNotFound();
            }
            return View(dossier_StatutDossier);
        }

        // POST: Dossier_StatutDossier/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            Dossier_StatutDossier dossier_StatutDossier = await db.GetDossier_StatutDossiers.FindAsync(id);
            db.GetDossier_StatutDossiers.Remove(dossier_StatutDossier);
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
