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
    public class CompteXAFsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CompteXAFs
        public async Task<ActionResult> Index()
        {
            var getCompteXAFs = db.GetCompteXAFs.Include(c => c.Banque);
            ViewBag.navigation = "param_trans_lab";
            ViewBag.navigation_msg = "Liste Compte Xaf";
            return View(await getCompteXAFs.ToListAsync());
        }

        // GET: CompteXAFs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompteXAF compteXAF = await db.GetCompteXAFs.FindAsync(id);
            if (compteXAF == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param_trans_lab";
            ViewBag.navigation_msg = "Details Compte Xaf";
            return View(compteXAF);
        }

        // GET: CompteXAFs/Create
        public ActionResult Create()
        {
            ViewBag.navigation = "param_trans_lab";
            ViewBag.navigation_msg = "Ajout Compte Xaf";
            return View();
        }

        // POST: CompteXAFs/Create
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "NumCompte,RIB,Libellé")] CompteXAF compteXAF)
        {
            if (ModelState.IsValid)
            {
                var structure = db.Structures.Find(Session["IdStructure"]);
                var idbanque = structure.BanqueId(db);
                compteXAF.BanqueId = idbanque;

                db.GetCompteXAFs.Add(compteXAF);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.BanqueId = new SelectList(db.Structures, "Id", "Nom", compteXAF.BanqueId);
            return View(compteXAF);
        }

        // GET: CompteXAFs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompteXAF compteXAF = await db.GetCompteXAFs.FindAsync(id);
            if (compteXAF == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param_trans_lab";
            ViewBag.navigation_msg = "Edition Compte Xaf";
            return View(compteXAF);
        }

        // POST: CompteXAFs/Edit/5
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,NumCompte,RIB,Libellé,BanqueId")] CompteXAF compteXAF)
        {
            if (ModelState.IsValid)
            {
                db.Entry(compteXAF).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.BanqueId = new SelectList(db.Structures, "Id", "Nom", compteXAF.BanqueId);
            return View(compteXAF);
        }

        // GET: CompteXAFs/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompteXAF compteXAF = await db.GetCompteXAFs.FindAsync(id);
            if (compteXAF == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param_trans_lab";
            ViewBag.navigation_msg = "Suppression Compte Xaf";
            return View(compteXAF);
        }

        // POST: CompteXAFs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            CompteXAF compteXAF = await db.GetCompteXAFs.FindAsync(id);
            db.GetCompteXAFs.Remove(compteXAF);
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
