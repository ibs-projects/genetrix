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
    public class PieceJointesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: PieceJointes
        public async Task<ActionResult> Index()
        {
            var GetPieceJointes = db.GetPieceJointes.Include(p => p.Client).Include(p => p.Justificatif);
            return View(await GetPieceJointes.ToListAsync());
        }

        // GET: PieceJointes/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PieceJointe pieceJointe = await db.GetPieceJointes.FindAsync(id);
            if (pieceJointe == null)
            {
                return HttpNotFound();
            }
            return View(pieceJointe);
        }

        // GET: PieceJointes/Create
        public ActionResult Create()
        {
            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom");
            ViewBag.JustificatifId = new SelectList(db.GetJustificatifs, "Id", "Libellé");
            return View();
        }

        // POST: PieceJointes/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Url,FichierImage,DateCreaApp,DateModif,ClientId,JustificatifId")] PieceJointe pieceJointe)
        {
            if (ModelState.IsValid)
            {
                db.GetPieceJointes.Add(pieceJointe);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom", pieceJointe.ClientId);
            ViewBag.JustificatifId = new SelectList(db.GetJustificatifs, "Id", "Libellé", pieceJointe.JustificatifId);
            return View(pieceJointe);
        }

        // GET: PieceJointes/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PieceJointe pieceJointe = await db.GetPieceJointes.FindAsync(id);
            if (pieceJointe == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom", pieceJointe.ClientId);
            ViewBag.JustificatifId = new SelectList(db.GetJustificatifs, "Id", "Libellé", pieceJointe.JustificatifId);
            return View(pieceJointe);
        }

        // POST: PieceJointes/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Url,FichierImage,DateCreaApp,DateModif,ClientId,JustificatifId")] PieceJointe pieceJointe)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pieceJointe).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom", pieceJointe.ClientId);
            ViewBag.JustificatifId = new SelectList(db.GetJustificatifs, "Id", "Libellé", pieceJointe.JustificatifId);
            return View(pieceJointe);
        }

        // GET: PieceJointes/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PieceJointe pieceJointe = await db.GetPieceJointes.FindAsync(id);
            if (pieceJointe == null)
            {
                return HttpNotFound();
            }
            return View(pieceJointe);
        }

        // POST: PieceJointes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            PieceJointe pieceJointe = await db.GetPieceJointes.FindAsync(id);
            db.GetPieceJointes.Remove(pieceJointe);
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
