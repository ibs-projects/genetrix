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
    public class CompteClientsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CompteClients
        public async Task<ActionResult> Index()
        {
            var clientId = (Session["user"] as CompteClient).ClientId;
            var GetCompteClients = db.GetCompteClients.Include(c => c.Client).Where(c=>c.ClientId==clientId);
            return View(await GetCompteClients.ToListAsync());
        }

        // GET: CompteClients/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompteClient compteClient = await db.GetCompteClients.FindAsync(id);
            if (compteClient == null)
            {
                return HttpNotFound();
            }
            return View(compteClient);
        }

        // GET: CompteClients/Create
        public ActionResult Create()
        {
            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom");
            return View();
        }

        // POST: CompteClients/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "EstAdmin,Nom,NomUtilisateur,Prenom,Sexe,Tel2,Tel1,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,ClientId")] CompteClient compteClient)
        {
            if (ModelState.IsValid)
            {
                compteClient.ClientId= (Session["user"] as CompteClient).ClientId; 
                db.GetCompteClients.Add(compteClient);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom", compteClient.ClientId);
            return View(compteClient);
        }

        // GET: CompteClients/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompteClient compteClient = await db.GetCompteClients.FindAsync(id);
            if (compteClient == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom", compteClient.ClientId);
            return View(compteClient);
        }

        // POST: CompteClients/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,EstAdmin,Nom,NomUtilisateur,Prenom,Sexe,Tel2,Tel1,Email")] CompteClient compteClient)
        {
            if (ModelState.IsValid)
            {
                compteClient.ClientId = (Session["user"] as CompteClient).ClientId;
                db.Entry(compteClient).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom", compteClient.ClientId);
            return View(compteClient);
        }

        // GET: CompteClients/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompteClient compteClient = await db.GetCompteClients.FindAsync(id);
            if (compteClient == null)
            {
                return HttpNotFound();
            }
            return View(compteClient);
        }

        // POST: CompteClients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            CompteClient compteClient = await db.GetCompteClients.FindAsync(id);
            db.GetCompteClients.Remove(compteClient);
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
