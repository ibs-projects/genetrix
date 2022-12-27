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
    public class OperateurSwiftsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: OperateurSwifts
        public async Task<ActionResult> Index()
        {
            ViewBag.navigation = "tiers_lab";
            ViewBag.navigation_msg = "Liste operateurs";
            var GetOperateurSwifts = db.GetOperateurSwifts.Include(o => o.Banque);
            return View(await GetOperateurSwifts.ToListAsync());
        }

        // GET: OperateurSwifts/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OperateurSwift operateurSwift = await db.GetOperateurSwifts.FindAsync(id);
            if (operateurSwift == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "tiers_lab";
            ViewBag.navigation_msg = "Details operateur";
            return View(operateurSwift);
        }

        // GET: OperateurSwifts/Create
        public ActionResult Create()
        {
            ViewBag.navigation = "tiers_lab";
            ViewBag.navigation_msg = "Ajout operateur";
            return View();
        }

        // POST: OperateurSwifts/Create
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "NomComplet,Telephone,Email,BanqueId")] OperateurSwift operateurSwift)
        {
            if (ModelState.IsValid)
            {
                var structure = db.Structures.Find(Session["IdStructure"]);
                var idbanque = structure.BanqueId(db);
                operateurSwift.BanqueId = idbanque;

                db.GetOperateurSwifts.Add(operateurSwift);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.BanqueId = new SelectList(db.Structures, "Id", "Nom", operateurSwift.BanqueId);
            return View(operateurSwift);
        }

        // GET: OperateurSwifts/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OperateurSwift operateurSwift = await db.GetOperateurSwifts.FindAsync(id);
            if (operateurSwift == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "tiers_lab";
            ViewBag.navigation_msg = "Edition operateur";
            return View(operateurSwift);
        }

        // POST: OperateurSwifts/Edit/5
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,NomComplet,Telephone,Email,BanqueId")] OperateurSwift operateurSwift)
        {
            if (ModelState.IsValid)
            {
                db.Entry(operateurSwift).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.BanqueId = new SelectList(db.Structures, "Id", "Nom", operateurSwift.BanqueId);
            return View(operateurSwift);
        }

        // GET: OperateurSwifts/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OperateurSwift operateurSwift = await db.GetOperateurSwifts.FindAsync(id);
            if (operateurSwift == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "tiers_lab";
            ViewBag.navigation_msg = "suppression operateur";
            return View(operateurSwift);
        }

        // POST: OperateurSwifts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            OperateurSwift operateurSwift = await db.GetOperateurSwifts.FindAsync(id);
            db.GetOperateurSwifts.Remove(operateurSwift);
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
