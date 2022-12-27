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
    public class CorrespondantsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Correspondants
        public async Task<ActionResult> Index()
        {
            ViewBag.navigation = "param_trans_lab";
            ViewBag.navigation_msg = "Liste Correspondants";
            var getCorrespondants = db.GetCorrespondants.Include(c => c.Banque);
            return View(await getCorrespondants.ToListAsync());
        }

        // GET: Correspondants/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Correspondant correspondant = await db.GetCorrespondants.FindAsync(id);
            if (correspondant == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param_trans_lab";
            ViewBag.navigation_msg = "Details Correspondant";
            return View(correspondant);
        }

        // GET: Correspondants/Create
        public ActionResult Create()
        {
            ViewBag.navigation = "param_trans_lab";
            ViewBag.navigation_msg = "Ajout Correspondant";

            var structure = db.Structures.Find(Session["IdStructure"]);
            var idbanque = structure.BanqueId(db);

            ViewBag.IdCorrespondant = new SelectList(db.GetCorrespondants.Where(c=>c.BanqueId==idbanque), "Id", "NomBanque");
            ViewBag.IdDevise = new SelectList(db.GetDeviseMonetaires, "Id", "Nom");

            return View();
        }

        // POST: Correspondants/Create
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "NomBanque,SwiftCode,Pays,Ville,NumCompte")] Correspondant correspondant)
        {
            if (ModelState.IsValid)
            {
                var structure = db.Structures.Find(Session["IdStructure"]);
                var idbanque = structure.BanqueId(db);
                correspondant.BanqueId = idbanque;

                db.GetCorrespondants.Add(correspondant);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.BanqueId = new SelectList(db.Structures, "Id", "Nom", correspondant.BanqueId);
            return View(correspondant);
        }

        // GET: Correspondants/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Correspondant correspondant = await db.GetCorrespondants.FindAsync(id);
            if (correspondant == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param_trans_lab";
            ViewBag.navigation_msg = "Edition Correspondant";

            var structure = db.Structures.Find(Session["IdStructure"]);
            var idbanque = structure.BanqueId(db);

            ViewBag.comptesNostros = db.CompteNostroes.Where(c => c.IdCorrespondant == id).ToList();
            ViewBag.IdDevise = new SelectList(db.GetDeviseMonetaires, "Id", "Nom");

            return View(correspondant);
        }

        // POST: Correspondants/Edit/5
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,NomBanque,SwiftCode,Pays,Ville,NumCompte,BanqueId")] Correspondant correspondant)
        {
            if (ModelState.IsValid)
            {
                db.Entry(correspondant).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.BanqueId = new SelectList(db.Structures, "Id", "Nom", correspondant.BanqueId);
            return View(correspondant);
        }

        // GET: Correspondants/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Correspondant correspondant = await db.GetCorrespondants.FindAsync(id);
            if (correspondant == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param_trans_lab";
            ViewBag.navigation_msg = "Suppression Correspondant";
            return View(correspondant);
        }

        // POST: Correspondants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Correspondant correspondant = await db.GetCorrespondants.FindAsync(id);
            db.GetCorrespondants.Remove(correspondant);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (Session != null)
            {
                var url = (string)Session["urlaccueil"];
                filterContext.Result = Redirect(url);
            }
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
