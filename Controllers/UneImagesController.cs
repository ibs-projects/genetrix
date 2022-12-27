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
using genetrix.Models;

namespace genetrix.Controllers
{
    public class UneImagesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: UneImages
        public async Task<ActionResult> Index()
        {
            return View(await db.GetAllImages.ToListAsync());
        }

        // GET: UneImages/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UneImage uneImage = await db.GetAllImages.FindAsync(id);
            if (uneImage == null)
            {
                return HttpNotFound();
            }
            return View(uneImage);
        }

        // GET: UneImages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UneImages/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Url,DateCreation,DerniereModif,Titre,NomCreateur")] UneImage uneImage)
        {
            if (ModelState.IsValid)
            {
                db.GetAllImages.Add(uneImage);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(uneImage);
        }

        // GET: UneImages/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UneImage uneImage = await db.GetAllImages.FindAsync(id);
            if (uneImage == null)
            {
                return HttpNotFound();
            }
            return View(uneImage);
        }

        // POST: UneImages/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Url,DateCreation,DerniereModif,Titre,NomCreateur")] UneImage uneImage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(uneImage).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(uneImage);
        }

        // GET: UneImages/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UneImage uneImage = await db.GetAllImages.FindAsync(id);
            if (uneImage == null)
            {
                return HttpNotFound();
            }
            return View(uneImage);
        }

        // POST: UneImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            UneImage uneImage = await db.GetAllImages.FindAsync(id);
            db.GetAllImages.Remove(uneImage);
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
