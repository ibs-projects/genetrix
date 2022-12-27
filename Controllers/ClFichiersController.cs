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
    public class ClFichiersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ClFichiers
        public async Task<ActionResult> Index()
        {
            var getClFichiers = db.GetClFichiers.Include(c => c.Client);
            return View(await getClFichiers.ToListAsync());
        }

        // GET: ClFichiers/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClFichier clFichier = await db.GetClFichiers.FindAsync(id);
            if (clFichier == null)
            {
                return HttpNotFound();
            }
            return View(clFichier);
        }

        // GET: ClFichiers/Create
        public ActionResult Create()
        {
            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom");
            return View();
        }

        // POST: ClFichiers/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Url,FichierImage,DateCreaApp,DateModif,ClientId")] ClFichier clFichier)
        {
            if (ModelState.IsValid)
            {
                db.GetClFichiers.Add(clFichier);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom", clFichier.ClientId);
            return View(clFichier);
        }

        // GET: ClFichiers/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClFichier clFichier = await db.GetClFichiers.FindAsync(id);
            if (clFichier == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom", clFichier.ClientId);
            return View(clFichier);
        }

        // POST: ClFichiers/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Url,FichierImage,DateCreaApp,DateModif,ClientId")] ClFichier clFichier)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clFichier).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom", clFichier.ClientId);
            return View(clFichier);
        }

        // GET: ClFichiers/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClFichier clFichier = await db.GetClFichiers.FindAsync(id);
            if (clFichier == null)
            {
                return HttpNotFound();
            }
            return View(clFichier);
        }

        // POST: ClFichiers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            ClFichier clFichier = await db.GetClFichiers.FindAsync(id);
            db.GetClFichiers.Remove(clFichier);
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
