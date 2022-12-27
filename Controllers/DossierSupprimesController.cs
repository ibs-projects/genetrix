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
    public class DossierSupprimesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DossierSupprimes
        public async Task<ActionResult> Index()
        {
            return View(await db.DossierSupprimes.ToListAsync());
        }

        // GET: DossierSupprimes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DossierSupprime dossierSupprime = await db.DossierSupprimes.FindAsync(id);
            if (dossierSupprime == null)
            {
                return HttpNotFound();
            }
            return View(dossierSupprime);
        }

        // GET: DossierSupprimes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DossierSupprimes/Create
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Agence,Gestionnaire,DonneurDordre,Benefic,Devise,MontantDev,MontantXaf,DateTraitement,DateDepot,DateSupp,UserName")] DossierSupprime dossierSupprime)
        {
            if (ModelState.IsValid)
            {
                db.DossierSupprimes.Add(dossierSupprime);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(dossierSupprime);
        }

        // GET: DossierSupprimes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DossierSupprime dossierSupprime = await db.DossierSupprimes.FindAsync(id);
            if (dossierSupprime == null)
            {
                return HttpNotFound();
            }
            return View(dossierSupprime);
        }

        // POST: DossierSupprimes/Edit/5
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Agence,Gestionnaire,DonneurDordre,Benefic,Devise,MontantDev,MontantXaf,DateTraitement,DateDepot,DateSupp,UserName")] DossierSupprime dossierSupprime)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dossierSupprime).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(dossierSupprime);
        }

        // GET: DossierSupprimes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DossierSupprime dossierSupprime = await db.DossierSupprimes.FindAsync(id);
            if (dossierSupprime == null)
            {
                return HttpNotFound();
            }
            return View(dossierSupprime);
        }

        // POST: DossierSupprimes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            DossierSupprime dossierSupprime = await db.DossierSupprimes.FindAsync(id);
            db.DossierSupprimes.Remove(dossierSupprime);
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
