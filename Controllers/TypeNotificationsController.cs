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
    public class TypeNotificationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TypeNotifications
        public async Task<ActionResult> Index()
        {
            return View(await db.TypeNotifications.ToListAsync());
        }

        // GET: TypeNotifications/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TypeNotification typeNotification = await db.TypeNotifications.FindAsync(id);
            if (typeNotification == null)
            {
                return HttpNotFound();
            }
            return View(typeNotification);
        }

        // GET: TypeNotifications/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TypeNotifications/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Message,Type")] TypeNotification typeNotification)
        {
            if (ModelState.IsValid)
            {
                db.TypeNotifications.Add(typeNotification);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(typeNotification);
        }

        // GET: TypeNotifications/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TypeNotification typeNotification = await db.TypeNotifications.FindAsync(id);
            if (typeNotification == null)
            {
                return HttpNotFound();
            }
            return View(typeNotification);
        }

        // POST: TypeNotifications/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Message,Type")] TypeNotification typeNotification)
        {
            if (ModelState.IsValid)
            {
                db.Entry(typeNotification).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(typeNotification);
        }

        // GET: TypeNotifications/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TypeNotification typeNotification = await db.TypeNotifications.FindAsync(id);
            if (typeNotification == null)
            {
                return HttpNotFound();
            }
            return View(typeNotification);
        }

        // POST: TypeNotifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            TypeNotification typeNotification = await db.TypeNotifications.FindAsync(id);
            db.TypeNotifications.Remove(typeNotification);
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
