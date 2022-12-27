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
    public class SignatairesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Signataires
        public async Task<ActionResult> Index()
        {
            ViewBag.navigation = "tiers_lab";
            ViewBag.navigation_msg = "Liste signataires";
            var GetSignataires = db.GetSignataires.Include(s => s.Banque);
            return View(await GetSignataires.ToListAsync());
        }

        // GET: Signataires/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Signataire signataire = await db.GetSignataires.FindAsync(id);
            if (signataire == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "tiers_lab";
            ViewBag.navigation_msg = "Details signataire";
            return View(signataire);
        }

        // GET: Signataires/Create
        public ActionResult Create()
        {
            ViewBag.navigation = "tiers_lab";
            ViewBag.navigation_msg = "Ajout signataire";
            return View();
        }

        // POST: Signataires/Create
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "NomComplet,Telephone,Email,Rang,Fonction")] Signataire signataire)
        {
            if (ModelState.IsValid)
            {
                var structure = db.Structures.Find(Session["IdStructure"]);
                var idbanque = structure.BanqueId(db);
                signataire.BanqueId = idbanque;

                db.GetSignataires.Add(signataire);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(signataire);
        }

        // GET: Signataires/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Signataire signataire = await db.GetSignataires.FindAsync(id);
            if (signataire == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "tiers_lab";
            ViewBag.navigation_msg = "Edition signataire";
            return View(signataire);
        }

        // POST: Signataires/Edit/5
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,NomComplet,Telephone,Email,BanqueId,Rang,Fonction")] Signataire signataire)
        {
            if (ModelState.IsValid)
            {
                db.Entry(signataire).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.BanqueId = new SelectList(db.Structures, "Id", "Nom", signataire.BanqueId);
            return View(signataire);
        }

        // GET: Signataires/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Signataire signataire = await db.GetSignataires.FindAsync(id);
            if (signataire == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "tiers_lab";
            ViewBag.navigation_msg = "Suppression signataire";
            return View(signataire);
        }

        // POST: Signataires/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Signataire signataire = await db.GetSignataires.FindAsync(id);
            db.GetSignataires.Remove(signataire);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (Session != null)
            {
                if ((string)Session["userType"] == "CompteBanqueCommerciale")
                {
                    var url = (string)Session["urlaccueil"];
                    filterContext.Result = Redirect(url);
                }

                filterContext.Result = RedirectToAction("Index", "Index");
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
