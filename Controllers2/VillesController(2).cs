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
    public class VillesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Villes
        public async Task<ActionResult> Index()
        {
            var getVilles = db.GetVilles.Include(v => v.Pays);
            return View(await getVilles.ToListAsync());
        }

        // GET: Villes/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ville ville = await db.GetVilles.FindAsync(id);
            if (ville == null)
            {
                return HttpNotFound();
            }
            return View(ville);
        }

        // GET: Villes/Create
        public ActionResult Create()
        {
            ViewBag.PaysId = new SelectList(db.GetPays, "Id", "Code");
            return View();
        }

        // POST: Villes/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Nom,PaysId")] Ville ville)
        {
            if (ModelState.IsValid)
            {
                db.GetVilles.Add(ville);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.PaysId = new SelectList(db.GetPays, "Id", "Code", ville.PaysId);
            return View(ville);
        }

        // GET: Villes/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ville ville = await db.GetVilles.FindAsync(id);
            if (ville == null)
            {
                return HttpNotFound();
            }
            ViewBag.PaysId = new SelectList(db.GetPays, "Id", "Code", ville.PaysId);
            return View(ville);
        }

        // POST: Villes/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Nom,PaysId")] Ville ville)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ville).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.PaysId = new SelectList(db.GetPays, "Id", "Code", ville.PaysId);
            return View(ville);
        }

        // GET: Villes/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ville ville = await db.GetVilles.FindAsync(id);
            if (ville == null)
            {
                return HttpNotFound();
            }
            return View(ville);
        }

        // POST: Villes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            Ville ville = await db.GetVilles.FindAsync(id);
            db.GetVilles.Remove(ville);
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
