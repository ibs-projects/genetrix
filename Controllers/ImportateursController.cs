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
    public class ImportateursController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Importateurs
        public async Task<ActionResult> Index()
        {
            var Importateurs = db.Importateurs.Include(i => i.Client).Include(i => i.Grestionnaire);
            return View(await Importateurs.ToListAsync());
        }

        // GET: Importateurs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Importateur importateur = await db.Importateurs.FindAsync(id);
            if (importateur == null)
            {
                return HttpNotFound();
            }
            return View(importateur);
        }

        // GET: Importateurs/Create
        public ActionResult Create()
        {
            ViewBag.IdClient = new SelectList(db.GetClients, "Id", "Nom");
            ViewBag.IdGestionnaire = new SelectList(db.Users, "Id", "Nom");
            return View();
        }

        // POST: Importateurs/Create
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,NomComplet,Pays,PaysOrigine,Ville,Telephone,Fax,Profession,Email,Adresse,IdGestionnaire,IdClient,Groupe,CodeAgrement,DateOptention,Code,Immatriculation,NumInscri")] Importateur importateur)
        {
            if (ModelState.IsValid)
            {
                importateur.IdClient = Convert.ToInt32(Session["clientId"]);
                importateur.Groupe = Groupe.Importateur;
                db.Importateurs.Add(importateur);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.IdClient = new SelectList(db.GetClients, "Id", "Nom", importateur.IdClient);
            ViewBag.IdGestionnaire = new SelectList(db.Users, "Id", "Nom", importateur.IdGestionnaire);
            return View(importateur);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateWithDossier([Bind(Include = "IdDossier,Id,NomComplet,Pays,PaysOrigine,Ville,Telephone,Fax,Profession,Email,Adresse,IdGestionnaire,IdClient,Groupe,CodeAgrement,DateOptention,Code,Immatriculation,NumInscri")] Importateur importateur)
        {
            if (ModelState.IsValid)
            {
                importateur.IdClient = Convert.ToInt32(Session["clientId"]);
                importateur.Groupe = Groupe.Importateur;
                db.Importateurs.Add(importateur);
                await db.SaveChangesAsync();
                if(importateur.IdDossier > 0)
                    return RedirectToAction("Edit", "Dossiers", new { id= importateur.IdDossier, etapeData=0});
                return RedirectToAction("Index");
            }

            ViewBag.IdClient = new SelectList(db.GetClients, "Id", "Nom", importateur.IdClient);
            ViewBag.IdGestionnaire = new SelectList(db.Users, "Id", "Nom", importateur.IdGestionnaire);
            return View(importateur);
        }

        // GET: Importateurs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Importateur importateur = await db.Importateurs.FindAsync(id);
            if (importateur == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdClient = new SelectList(db.GetClients, "Id", "Nom", importateur.IdClient);
            ViewBag.IdGestionnaire = new SelectList(db.Users, "Id", "Nom", importateur.IdGestionnaire);
            return View(importateur);
        }

        // POST: Importateurs/Edit/5
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,NomComplet,Pays,PaysOrigine,Ville,Telephone,Fax,Profession,Email,Adresse,IdGestionnaire,IdClient,Groupe,CodeAgrement,DateOptention,Code,Immatriculation,NumInscri")] Importateur importateur)
        {
            if (ModelState.IsValid)
            {
                importateur.IdClient = Convert.ToInt32(Session["clientId"]);
                var cl =await db.GetClients.FindAsync(Convert.ToInt32(Session["clientId"]));
                importateur.IdGestionnaire =cl.GetGestionnaireId;
                cl = null;
                importateur.Groupe = Groupe.Importateur;
                db.Entry(importateur).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.IdClient = new SelectList(db.GetClients, "Id", "Nom", importateur.IdClient);
            ViewBag.IdGestionnaire = new SelectList(db.Users, "Id", "Nom", importateur.IdGestionnaire);
            return View(importateur);
        }

        // GET: Importateurs/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Importateur importateur = await db.Importateurs.FindAsync(id);
            if (importateur == null)
            {
                return HttpNotFound();
            }
            return View(importateur);
        }

        // POST: Importateurs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Importateur importateur = await db.Importateurs.FindAsync(id);
            db.Importateurs.Remove(importateur);
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
