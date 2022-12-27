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
    public class NotifReferencesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: NotifReferences
        public async Task<ActionResult> Index()
        {
            return View(await db.GetNotifReferences.ToListAsync());
        }

        // GET: NotifReferences/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NotifReference notifReference = await db.GetNotifReferences.FindAsync(id);
            if (notifReference == null)
            {
                return HttpNotFound();
            }
            return View(notifReference);
        }

        // GET: NotifReferences/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NotifReferences/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Message,Objet,LienImage,Image")] NotifReference notifReference)
        {
            if (ModelState.IsValid)
            {
                db.GetNotifReferences.Add(notifReference);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(notifReference);
        }

        // GET: NotifReferences/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NotifReference notifReference = await db.GetNotifReferences.FindAsync(id);
            if (notifReference == null)
            {
                return HttpNotFound();
            }
            return View(notifReference);
        }

        // POST: NotifReferences/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Message,Objet,LienImage,Image")] NotifReference notifReference)
        {
            if (ModelState.IsValid)
            {
                db.Entry(notifReference).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(notifReference);
        }

        // GET: NotifReferences/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NotifReference notifReference = await db.GetNotifReferences.FindAsync(id);
            if (notifReference == null)
            {
                return HttpNotFound();
            }
            return View(notifReference);
        }

        // POST: NotifReferences/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            NotifReference notifReference = await db.GetNotifReferences.FindAsync(id);
            db.GetNotifReferences.Remove(notifReference);
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
