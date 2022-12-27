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
    public class FormatRefIndernesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: FormatRefIndernes
        public async Task<ActionResult> Index()
        {
            return View(await db.GetFormatRefIndernes.ToListAsync());
        }

        // GET: FormatRefIndernes/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FormatRefInderne formatRefInderne = await db.GetFormatRefIndernes.FindAsync(id);
            if (formatRefInderne == null)
            {
                return HttpNotFound();
            }
            return View(formatRefInderne);
        }

        // GET: FormatRefIndernes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FormatRefIndernes/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,CodeFormat,CodeFormatTaile")] FormatRefInderne formatRefInderne)
        {
            if (ModelState.IsValid)
            {
                db.GetFormatRefIndernes.Add(formatRefInderne);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(formatRefInderne);
        }

        // GET: FormatRefIndernes/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FormatRefInderne formatRefInderne = await db.GetFormatRefIndernes.FindAsync(id);
            if (formatRefInderne == null)
            {
                return HttpNotFound();
            }
            return View(formatRefInderne);
        }

        // POST: FormatRefIndernes/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,CodeFormat,CodeFormatTaile")] FormatRefInderne formatRefInderne)
        {
            if (ModelState.IsValid)
            {
                db.Entry(formatRefInderne).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(formatRefInderne);
        }

        // GET: FormatRefIndernes/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FormatRefInderne formatRefInderne = await db.GetFormatRefIndernes.FindAsync(id);
            if (formatRefInderne == null)
            {
                return HttpNotFound();
            }
            return View(formatRefInderne);
        }

        // POST: FormatRefIndernes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            FormatRefInderne formatRefInderne = await db.GetFormatRefIndernes.FindAsync(id);
            db.GetFormatRefIndernes.Remove(formatRefInderne);
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
