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
    public class TypeStructuresController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TypeStructures
        public async Task<ActionResult> Index()
        {
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Liste typestructures";
            return View(await db.GetTypeStructures.ToListAsync());
        }

        // GET: TypeStructures/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TypeStructure typeStructure = await db.GetTypeStructures.FindAsync(id);
            if (typeStructure == null)
            {
                return HttpNotFound();
            }
            return View(typeStructure);
        }

        // GET: TypeStructures/Create
        public ActionResult Create()
        {
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Creation typestructure";
            return View();
        }

        // POST: TypeStructures/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Intitule")] TypeStructure typeStructure)
        {
            if (ModelState.IsValid)
            {
                db.GetTypeStructures.Add(typeStructure);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(typeStructure);
        }

        // GET: TypeStructures/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TypeStructure typeStructure = await db.GetTypeStructures.FindAsync(id);
            if (typeStructure == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Edition typestructure";
            return View(typeStructure);
        }

        // POST: TypeStructures/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Intitule")] TypeStructure typeStructure)
        {
            if (ModelState.IsValid)
            {
                db.Entry(typeStructure).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(typeStructure);
        }

        // GET: TypeStructures/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TypeStructure typeStructure = await db.GetTypeStructures.FindAsync(id);
            if (typeStructure == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Suppression typestructure";
            return View(typeStructure);
        }

        // POST: TypeStructures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            TypeStructure typeStructure = await db.GetTypeStructures.FindAsync(id);
            db.GetTypeStructures.Remove(typeStructure);
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
