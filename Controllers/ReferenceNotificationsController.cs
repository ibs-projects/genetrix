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
    public class ReferenceNotificationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ReferenceNotifications
        public async Task<ActionResult> Index()
        {
            return View(await db.GetReferenceNotifications.ToListAsync());
        }

        // GET: ReferenceNotifications/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReferenceNotification referenceNotification = await db.GetReferenceNotifications.FindAsync(id);
            if (referenceNotification == null)
            {
                return HttpNotFound();
            }
            return View(referenceNotification);
        }

        // GET: ReferenceNotifications/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ReferenceNotifications/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Date,Desactive,Lue,Titre,Detail")] ReferenceNotification referenceNotification)
        {
            if (ModelState.IsValid)
            {
                db.GetReferenceNotifications.Add(referenceNotification);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(referenceNotification);
        }

        // GET: ReferenceNotifications/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReferenceNotification referenceNotification = await db.GetReferenceNotifications.FindAsync(id);
            if (referenceNotification == null)
            {
                return HttpNotFound();
            }
            return View(referenceNotification);
        }

        // POST: ReferenceNotifications/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Date,Desactive,Lue,Titre,Detail")] ReferenceNotification referenceNotification)
        {
            if (ModelState.IsValid)
            {
                db.Entry(referenceNotification).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(referenceNotification);
        }

        // GET: ReferenceNotifications/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReferenceNotification referenceNotification = await db.GetReferenceNotifications.FindAsync(id);
            if (referenceNotification == null)
            {
                return HttpNotFound();
            }
            return View(referenceNotification);
        }

        // POST: ReferenceNotifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            ReferenceNotification referenceNotification = await db.GetReferenceNotifications.FindAsync(id);
            db.GetReferenceNotifications.Remove(referenceNotification);
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
