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
    public class NumCompteBeneficiairesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: NumCompteBeneficiaires
        public async Task<ActionResult> Index()
        {
            var compteBeneficiaires = db.CompteBeneficiaires.Include(n => n.Fournisseur);
            return View(await compteBeneficiaires.ToListAsync());
        }

        // GET: NumCompteBeneficiaires/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NumCompteBeneficiaire numCompteBeneficiaire = await db.CompteBeneficiaires.FindAsync(id);
            if (numCompteBeneficiaire == null)
            {
                return HttpNotFound();
            }
            return View(numCompteBeneficiaire);
        }

        // GET: NumCompteBeneficiaires/Create
        public ActionResult Create()
        {
            ViewBag.IdFournisseur = new SelectList(db.GetFournisseurs, "Id", "Nom");
            return View();
        }

        // POST: NumCompteBeneficiaires/Create
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Cle,CodeSwift,Adresse,NomBanque,CodeAgence,Numero,Nom,Pays,Ville,IdFournisseur")] NumCompteBeneficiaire numCompteBeneficiaire)
        {
            if (ModelState.IsValid)
            {
                db.CompteBeneficiaires.Add(numCompteBeneficiaire);
                await db.SaveChangesAsync();
                //return RedirectToAction("Index");
            }

            ViewBag.IdFournisseur = new SelectList(db.GetFournisseurs, "Id", "Nom", numCompteBeneficiaire.IdFournisseur);
            return RedirectToAction("Details","Fournisseurs",new { id=numCompteBeneficiaire.IdFournisseur});
        }

        // GET: NumCompteBeneficiaires/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NumCompteBeneficiaire numCompteBeneficiaire = await db.CompteBeneficiaires.FindAsync(id);
            if (numCompteBeneficiaire == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdFournisseur = new SelectList(db.GetFournisseurs, "Id", "Nom", numCompteBeneficiaire.IdFournisseur);
            return View(numCompteBeneficiaire);
        }

        // POST: NumCompteBeneficiaires/Edit/5
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,CodeSwift,Cle,Adresse,NomBanque,CodeAgence,Numero,Nom,Pays,Ville,IdFournisseur")] NumCompteBeneficiaire numCompteBeneficiaire)
        {
            if (ModelState.IsValid)
            {
                db.Entry(numCompteBeneficiaire).State = EntityState.Modified;
                await db.SaveChangesAsync();
                //return RedirectToAction("Index");
            }
            ViewBag.IdFournisseur = new SelectList(db.GetFournisseurs, "Id", "Nom", numCompteBeneficiaire.IdFournisseur);
            return RedirectToAction("Details", "Fournisseurs", new { id = numCompteBeneficiaire.IdFournisseur });
        }

        // GET: NumCompteBeneficiaires/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NumCompteBeneficiaire numCompteBeneficiaire = await db.CompteBeneficiaires.FindAsync(id);
            if (numCompteBeneficiaire == null)
            {
                return HttpNotFound();
            }
            return View(numCompteBeneficiaire);
        }

        // POST: NumCompteBeneficiaires/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            NumCompteBeneficiaire numCompteBeneficiaire = await db.CompteBeneficiaires.FindAsync(id);
            db.CompteBeneficiaires.Remove(numCompteBeneficiaire);
            await db.SaveChangesAsync();
            return RedirectToAction("Details", "Fournisseurs", new { id = numCompteBeneficiaire.IdFournisseur });
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
