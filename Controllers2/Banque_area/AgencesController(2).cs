using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using eApurement.Models;
using e_apurement.Models;

namespace eApurement.Controllers.Banque_area
{
    public class AgencesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Agences
        public ActionResult Index(string msg="")
        {
            if (Session == null)
                return RedirectToAction("login", "auth");
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Liste agences";
            var str = db.Structures.Find((Session["user"] as CompteBanqueCommerciale).IdStructure);
            var banqueId = str.BanqueId(db);
            //var dd = db.Agences.Include(a => a.Responsable).Include(a => a.TypeStructure).Include(a => a.DirectionMetier);
            List<Agence> structures = VariablGlobales.GetAgenceByBanque(banqueId, db);
            ViewBag.msg = msg;
            return View(structures);
        }

        // GET: Agences/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Agence agence = await db.Agences.FindAsync(id);
            if (agence == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Details agence";
            return View(agence);
        }

        // GET: Agences/Create
        public ActionResult Create()
        {
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Creation agence";
            var banqueId = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);
            List<CompteBanqueCommerciale> users = VariablGlobales.GetUsersByBanque(banqueId, db);
            ViewData["IdResponsable"] = users;// from u in users select new {Nom=u.NomComplet,Value=u.Id };
            users = null;
            ViewBag.IdTypeStructure = new SelectList(db.GetTypeStructures, "Id", "Intitule");
            ViewBag.IdDirectionMetier = new SelectList(db.Agences, "Id", "Nom");
            List<DirectionMetier> direct = VariablGlobales.GetDirectionMetierByBanque(banqueId, db);
            ViewData["IdDirectionMetier"] = direct;// from u in users select new {Nom=u.NomComplet,Value=u.Id };
            users = null;
            var model = new Agence() { EstAgence = true, LireTouteReference = false };
            model.NiveauDossier = 1;
            model.NiveauMaxDossier = 2;
            return View(model);
        }

        // POST: Agences/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "LireTouteReference,Nom,NiveauMaxDossier,Adresse,Ville,Pays,Telephone,Telephone2,NiveauDossier,IdTypeStructure,EstAgence,IdResponsable,IdDirectionMetier")] Agence agence)
        {
            var banqueId = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);

            if (ModelState.IsValid)
            {
                agence.IdTypeStructure = db.GetTypeStructures.FirstOrDefault(t => t.Intitule.ToLower().Contains("agence")).Id;
                db.Agences.Add(agence);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            var users = VariablGlobales.GetUsersByBanque(banqueId, db);
            var agences = VariablGlobales.GetAgenceByBanque(banqueId, db);
            ViewBag.IdResponsable = new SelectList(users, "Id", "Nom", agence.IdResponsable);
            ViewBag.IdTypeStructure = new SelectList(db.GetTypeStructures, "Id", "Intitule", agence.IdTypeStructure);
            ViewBag.IdDirectionMetier = new SelectList(agences, "Id", "Nom", agence.IdDirectionMetier);
            users = null;
            agences = null;
            return View(agence);
        }

        // GET: Agences/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Agence agence = await db.Agences.FindAsync(id);
            if (agence == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Edition agence";

            var banqueId = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);
            var users = VariablGlobales.GetUsersByBanque(banqueId, db);
            var agences = VariablGlobales.GetDirectionMetierByBanque(banqueId, db);

            ViewBag.IdResponsable = new SelectList(users, "Id", "Nom", agence.IdResponsable);
            ViewBag.IdTypeStructure = new SelectList(db.GetTypeStructures, "Id", "Intitule", agence.IdTypeStructure);
            ViewBag.IdDirectionMetier = new SelectList(agences, "Id", "Nom", agence.IdDirectionMetier);
            users = null;
            agences = null;
            return View(agence);
        }

        // POST: Agences/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "LireTouteReference,Id,NiveauMaxDossier,Nom,Adresse,Ville,Pays,Telephone,Telephone2,NiveauDossier,IdTypeStructure,EstAgence,IdResponsable,IdDirectionMetier")] Agence agence)
        {
            var banqueId = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);

            if (ModelState.IsValid)
            {
                db.Entry(agence).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            var users = VariablGlobales.GetUsersByBanque(banqueId, db);
            var agences = VariablGlobales.GetAgenceByBanque(banqueId, db);

            ViewBag.IdResponsable = new SelectList(users, "Id", "Nom", agence.IdResponsable);
            ViewBag.IdTypeStructure = new SelectList(db.GetTypeStructures, "Id", "Intitule", agence.IdTypeStructure);
            ViewBag.IdDirectionMetier = new SelectList(agences, "Id", "Nom", agence.IdDirectionMetier);
            users = null;
            agences = null;
            return View(agence);
        }

        // GET: Agences/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Agence agence = await db.Agences.FindAsync(id);
            if (agence == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Suppression agence";
            return View(agence);
        }

        // POST: Agences/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            string msg = "";
            Agence agence = await db.Agences.FindAsync(id);
            try
            {
                db.Agences.Remove(agence);
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                msg = "Erreur code 2563. Impossible de supprimer de l'entitée structure.";
            }
            return RedirectToAction("Index", new { msg = msg });
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
