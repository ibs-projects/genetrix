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
    public class MotifsTransfertsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ModifsTransferts
        public async Task<ActionResult> Index()
        {
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "liste de motifs";
            return View(await db.ModifsTransferts.ToListAsync());
        }

        [HttpPost]
        public JsonResult Index(string Prefix)
        {
            //Searching records from list using LINQ query  
            var Name = (from m in db.ModifsTransferts
                        where m.Intitule.StartsWith(Prefix)
                        select new { m.Intitule });
            return Json(Name, JsonRequestBehavior.AllowGet);
        }

        // GET: ModifsTransferts/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MotifsTransfert modifsTransfert = await db.ModifsTransferts.FindAsync(id);
            if (modifsTransfert == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Details du motif";
            return View(modifsTransfert);
        }

        // GET: ModifsTransferts/Create
        public ActionResult Create()
        {
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Ajout d'un modif";
            return View();
        }

        // POST: ModifsTransferts/Create
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Intitule")] MotifsTransfert modifsTransfert)
        {
            if (ModelState.IsValid)
            {
                db.ModifsTransferts.Add(modifsTransfert);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(modifsTransfert);
        }

        // GET: ModifsTransferts/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MotifsTransfert modifsTransfert = await db.ModifsTransferts.FindAsync(id);
            if (modifsTransfert == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Edition du motif";
            return View(modifsTransfert);
        }

        // POST: ModifsTransferts/Edit/5
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Intitule")] MotifsTransfert modifsTransfert)
        {
            if (ModelState.IsValid)
            {
                db.Entry(modifsTransfert).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(modifsTransfert);
        }

        // GET: ModifsTransferts/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MotifsTransfert modifsTransfert = await db.ModifsTransferts.FindAsync(id);
            if (modifsTransfert == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Suppression du motif";
            return View(modifsTransfert);
        }

        // POST: ModifsTransferts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            MotifsTransfert modifsTransfert = await db.ModifsTransferts.FindAsync(id);
            db.ModifsTransferts.Remove(modifsTransfert);
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
