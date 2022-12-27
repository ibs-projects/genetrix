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
    public class PaysController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Pays
        public async Task<ActionResult> Index()
        {
            return View(await db.GetPays.ToListAsync());
        }

        // GET: Pays/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pays pays = await db.GetPays.FindAsync(id);
            if (pays == null)
            {
                return HttpNotFound();
            }
            return View(pays);
        }

        // GET: Pays/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Pays/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Code,Indicatif")] Pays pays)
        {
            if (ModelState.IsValid)
            {
                db.GetPays.Add(pays);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(pays);
        }

        // GET: Pays/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pays pays = await db.GetPays.FindAsync(id);
            if (pays == null)
            {
                return HttpNotFound();
            }
            return View(pays);
        }

        // POST: Pays/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Code,Indicatif")] Pays pays)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pays).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(pays);
        }

        // GET: Pays/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pays pays = await db.GetPays.FindAsync(id);
            if (pays == null)
            {
                return HttpNotFound();
            }
            return View(pays);
        }

        // POST: Pays/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            Pays pays = await db.GetPays.FindAsync(id);
            db.GetPays.Remove(pays);
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
