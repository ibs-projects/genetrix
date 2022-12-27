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
    public class StatutDossiersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: StatutDossiers
        public async Task<ActionResult> Index()
        {
            return View(await db.GetStatutDossiers.ToListAsync());
        }

        // GET: StatutDossiers/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StatutDossier statutDossier = await db.GetStatutDossiers.FindAsync(id);
            if (statutDossier == null)
            {
                return HttpNotFound();
            }
            return View(statutDossier);
        }

        // GET: StatutDossiers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StatutDossiers/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "EtatDossier,Message,Titre,Image,Couleur")] StatutDossier statutDossier)
        {
            if (ModelState.IsValid)
            {
                db.GetStatutDossiers.Add(statutDossier);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(statutDossier);
        }

        // GET: StatutDossiers/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StatutDossier statutDossier = await db.GetStatutDossiers.FindAsync(id);
            if (statutDossier == null)
            {
                return HttpNotFound();
            }
            return View(statutDossier);
        }

        // POST: StatutDossiers/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Statut_Id,EtatDossier,Message,Titre,Image,Couleur")] StatutDossier statutDossier)
        {
            if (ModelState.IsValid)
            {
                db.Entry(statutDossier).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(statutDossier);
        }

        // GET: StatutDossiers/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StatutDossier statutDossier = await db.GetStatutDossiers.FindAsync(id);
            if (statutDossier == null)
            {
                return HttpNotFound();
            }
            return View(statutDossier);
        }

        // POST: StatutDossiers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            StatutDossier statutDossier = await db.GetStatutDossiers.FindAsync(id);
            db.GetStatutDossiers.Remove(statutDossier);
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
