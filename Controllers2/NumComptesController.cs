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
    public class NumComptesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: NumComptes
        public async Task<ActionResult> Index()
        {
            var numComptesClients = db.NumComptesClients.Include(n => n.BanqueClient);
            return View(await numComptesClients.ToListAsync());
        }

        // GET: NumComptes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NumCompte numCompte = await db.NumComptesClients.FindAsync(id);
            if (numCompte == null)
            {
                return HttpNotFound();
            }
            return View(numCompte);
        }

        // GET: NumComptes/Create
        public ActionResult Create()
        {
            ViewBag.IdBanqueClient = new SelectList(db.GetBanqueClients, "Id", "IdGestionnaire");
            return View();
        }

        // POST: NumComptes/Create
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Cle,Numero,Nom,CodeAgence,IdBanqueClient")] NumCompte numCompte)
        {
            //if (ModelState.IsValid)
            {
                try
                {
                    numCompte.BanqueClient = db.GetClients.Find(Session["clientId"]).Banques.FirstOrDefault();
                    db.NumComptesClients.Add(numCompte);
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {}
            }
            return RedirectToAction("Details", "Clients", new { id = 1 });
        }

        // GET: NumComptes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NumCompte numCompte = await db.NumComptesClients.FindAsync(id);
            if (numCompte == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdBanqueClient = new SelectList(db.GetBanqueClients, "Id", "IdGestionnaire", numCompte.IdBanqueClient);
            return View(numCompte);
        }

        // POST: NumComptes/Edit/5
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Cle,CodeAgence,Numero,Nom,IdBanqueClient")] NumCompte numCompte)
        {
            if (ModelState.IsValid)
            {
                db.Entry(numCompte).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Details", "Clients", new { id = 1 });
        }

        // GET: NumComptes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NumCompte numCompte = await db.NumComptesClients.FindAsync(id);
            if (numCompte == null)
            {
                return HttpNotFound();
            }
            return View(numCompte);
        }

        // POST: NumComptes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            NumCompte numCompte = await db.NumComptesClients.FindAsync(id);
            db.NumComptesClients.Remove(numCompte);
            await db.SaveChangesAsync();
            return RedirectToAction("Details", "Clients", new { id = 1 });
        }
        protected override void OnException(ExceptionContext filterContext)
        {
            if (Session != null)
            {
                if ((string)Session["userType"]== "CompteBanqueCommerciale")
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
