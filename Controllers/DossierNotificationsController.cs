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
    public class DossierNotificationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DossierNotifications
        public async Task<ActionResult> Index()
        {
            var getDossierNotifications = db.GetDossierNotifications.Include(d => d.Dossier);
            return View(await getDossierNotifications.ToListAsync());
        }

        // GET: DossierNotifications/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DossierNotification dossierNotification = await db.GetDossierNotifications.FindAsync(id);
            if (dossierNotification == null)
            {
                return HttpNotFound();
            }
            return View(dossierNotification);
        }

        // GET: DossierNotifications/Create
        public ActionResult Create()
        {
            ViewBag.DossierId = new SelectList(db.GetDossiers, "Id", "Intitule");
            return View();
        }

        // POST: DossierNotifications/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,DossierId,Date,Desactive,Lue,Titre,Detail")] DossierNotification dossierNotification)
        {
            if (ModelState.IsValid)
            {
                db.GetDossierNotifications.Add(dossierNotification);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.DossierId = new SelectList(db.GetDossiers, "Id", "Intitule", dossierNotification.DossierId);
            return View(dossierNotification);
        }

        // GET: DossierNotifications/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DossierNotification dossierNotification = await db.GetDossierNotifications.FindAsync(id);
            if (dossierNotification == null)
            {
                return HttpNotFound();
            }
            ViewBag.DossierId = new SelectList(db.GetDossiers, "Id", "Intitule", dossierNotification.DossierId);
            return View(dossierNotification);
        }

        // POST: DossierNotifications/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,DossierId,Date,Desactive,Lue,Titre,Detail")] DossierNotification dossierNotification)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dossierNotification).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.DossierId = new SelectList(db.GetDossiers, "Id", "Intitule", dossierNotification.DossierId);
            return View(dossierNotification);
        }

        // GET: DossierNotifications/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DossierNotification dossierNotification = await db.GetDossierNotifications.FindAsync(id);
            if (dossierNotification == null)
            {
                return HttpNotFound();
            }
            return View(dossierNotification);
        }

        // POST: DossierNotifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            DossierNotification dossierNotification = await db.GetDossierNotifications.FindAsync(id);
            db.GetDossierNotifications.Remove(dossierNotification);
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
