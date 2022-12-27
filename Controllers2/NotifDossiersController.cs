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
    public class NotifDossiersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: NotifDossiers
        public async Task<ActionResult> Index()
        {
            return View(await db.GetNotifications.ToListAsync());
        }

        // GET: NotifDossiers/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NotifDossier notifDossier = await db.GetNotifDossiers.FindAsync(id);
            if (notifDossier == null)
            {
                return HttpNotFound();
            }
            return View(notifDossier);
        }

        // GET: NotifDossiers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NotifDossiers/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Message,Objet,LienImage,Image")] NotifDossier notifDossier)
        {
            if (ModelState.IsValid)
            {
                db.GetNotifications.Add(notifDossier);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(notifDossier);
        }

        // GET: NotifDossiers/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NotifDossier notifDossier = await db.GetNotifDossiers.FindAsync(id);
            if (notifDossier == null)
            {
                return HttpNotFound();
            }
            return View(notifDossier);
        }

        // POST: NotifDossiers/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Message,Objet,LienImage,Image")] NotifDossier notifDossier)
        {
            if (ModelState.IsValid)
            {
                db.Entry(notifDossier).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(notifDossier);
        }

        // GET: NotifDossiers/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NotifDossier notifDossier = await db.GetNotifDossiers.FindAsync(id);
            if (notifDossier == null)
            {
                return HttpNotFound();
            }
            return View(notifDossier);
        }

        // POST: NotifDossiers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            NotifDossier notifDossier = await db.GetNotifDossiers.FindAsync(id);
            db.GetNotifications.Remove(notifDossier);
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
