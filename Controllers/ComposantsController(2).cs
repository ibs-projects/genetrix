using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using eApurement.Models;
using e_apurement.Models;

namespace eApurement.Controllers
{
    public class ComposantsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Composants1
        public async Task<ActionResult> Index()
        {
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Liste composants";
            var getComposants = db.GetComposants.Include(c => c.Action).Include(c => c.Groupe).Include(c => c.GroupeDisposionAccueil);
            return View(await getComposants.ToListAsync());
        }

        // GET: Composants1/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Composant composant = await db.GetComposants.FindAsync(id);
            if (composant == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Details composant";
            return View(composant);
        }

        // GET: Composants1/Create
        public ActionResult Create()
        {
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Creation composant";
            ViewBag.IdAction = new SelectList(db.Actions, "Id", "Intitule");
            ViewBag.IdGroupe = new SelectList(db.GroupeComposants, "Id", "Nom");
            ViewBag.IdGroupeDisposionAccueil = new SelectList(db.GroupeDisposionAccueilss, "Id", "ClasseFille");
            return View();
        }

        // POST: Composants1/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "NumeroMax,NumeroMin,Numero,EstActif,Description,Localistion,Type,IdGroupe,IdAction,IdGroupeDisposionAccueil,Recherche")] Composant composant)
        {
            if (ModelState.IsValid)
            {
                db.GetComposants.Add(composant);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.IdAction = new SelectList(db.Actions, "Id", "Intitule", composant.IdAction);
            ViewBag.IdGroupe = new SelectList(db.GroupeComposants, "Id", "Nom", composant.IdGroupe);
            ViewBag.IdGroupeDisposionAccueil = new SelectList(db.GroupeDisposionAccueilss, "Id", "ClasseFille", composant.IdGroupeDisposionAccueil);
            return View(composant);
        }

        // GET: Composants1/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Composant composant = await db.GetComposants.FindAsync(id);
            if (composant == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Edition composant";
            ViewBag.IdAction = new SelectList(db.Actions, "Id", "Intitule", composant.IdAction);
            ViewBag.IdGroupe = new SelectList(db.GroupeComposants, "Id", "Nom", composant.IdGroupe);
            ViewBag.IdGroupeDisposionAccueil = new SelectList(db.GroupeDisposionAccueilss, "Id", "ClasseFille", composant.IdGroupeDisposionAccueil);
            return View(composant);
        }

        // POST: Composants1/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,NumeroMin,NumeroMax,Numero,EstActif,Description,Localistion,Type,IdGroupe,IdAction,IdGroupeDisposionAccueil,Recherche")] Composant composant)
        {
            if (ModelState.IsValid)
            {
                db.Entry(composant).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.IdAction = new SelectList(db.Actions, "Id", "Intitule", composant.IdAction);
            ViewBag.IdGroupe = new SelectList(db.GroupeComposants, "Id", "Nom", composant.IdGroupe);
            ViewBag.IdGroupeDisposionAccueil = new SelectList(db.GroupeDisposionAccueilss, "Id", "ClasseFille", composant.IdGroupeDisposionAccueil);
            return View(composant);
        }

        // GET: Composants1/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Composant composant = await db.GetComposants.FindAsync(id);
            if (composant == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Suppression composant";
            return View(composant);
        }

        // POST: Composants1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Composant composant = await db.GetComposants.FindAsync(id);
            db.GetComposants.Remove(composant);
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
