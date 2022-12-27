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
    public class DeviseMonetairesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DeviseMonetaires
        public async Task<ActionResult> Index()
        {
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "liste de devises";

            return View(await db.GetDeviseMonetaires.ToListAsync());
        }

        // GET: DeviseMonetaires/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeviseMonetaire deviseMonetaire = await db.GetDeviseMonetaires.FindAsync(id);
            if (deviseMonetaire == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "details devise";
            return View(deviseMonetaire);
        }

        // GET: DeviseMonetaires/Create
        public ActionResult Create()
        {
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "creation devise";
            return View();
        }

        // POST: DeviseMonetaires/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Nom,ImageLogo")] DeviseMonetaire deviseMonetaire)
        {
            if (ModelState.IsValid)
            {
                db.GetDeviseMonetaires.Add(deviseMonetaire);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(deviseMonetaire);
        }

        // GET: DeviseMonetaires/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeviseMonetaire deviseMonetaire = await db.GetDeviseMonetaires.FindAsync(id);
            if (deviseMonetaire == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "edition devise";
            return View(deviseMonetaire);
        }

        // POST: DeviseMonetaires/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Nom,ImageLogo")] DeviseMonetaire deviseMonetaire)
        {
            if (ModelState.IsValid)
            {
                db.Entry(deviseMonetaire).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(deviseMonetaire);
        }

        // GET: DeviseMonetaires/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeviseMonetaire deviseMonetaire = await db.GetDeviseMonetaires.FindAsync(id);
            if (deviseMonetaire == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "suppression devise";
            return View(deviseMonetaire);
        }

        // POST: DeviseMonetaires/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            DeviseMonetaire deviseMonetaire = await db.GetDeviseMonetaires.FindAsync(id);
            db.GetDeviseMonetaires.Remove(deviseMonetaire);
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
