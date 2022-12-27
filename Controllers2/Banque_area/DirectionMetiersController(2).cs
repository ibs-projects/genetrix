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

namespace eApurement.Controllers.Banque
{
    public class DirectionMetiersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DirectionMetiers
        public ActionResult Index(string msg="")
        {
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Liste directions m.";
            var str = db.Structures.Find((Session["user"] as CompteBanqueCommerciale).IdStructure);
            var banqueId = str.BanqueId(db);
            str = null;
            //var dd = db.DirectionMetiers.Include(d => d.Responsable).Include(d => d.TypeStructure).Include(d => d.Banque);
            List<DirectionMetier> structures = VariablGlobales.GetDirectionMetierByBanque(banqueId, db);

            ViewBag.msg = msg;
            return View(structures);
        }

        // GET: DirectionMetiers/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DirectionMetier directionMetier = await db.DirectionMetiers.Include(d=>d.Agences).FirstOrDefaultAsync(d=>d.Id==id);
            if (directionMetier == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Details direction m.";
            return View(directionMetier);
        }

        // GET: DirectionMetiers/Create
        public ActionResult Create()
        {
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Creation direction m.";
            var banqueId = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);
            List<CompteBanqueCommerciale> users  = VariablGlobales.GetUsersByBanque(banqueId, db);

            ViewData["users"] = users;// from u in users select new {Nom=u.NomComplet,Value=u.Id };
            users = null;
            ViewBag.IdTypeStructure = new SelectList(db.GetTypeStructures, "Id", "Intitule");
            List<DirectionMetier> directionMetiers = VariablGlobales.GetDirectionMetierByBanque(banqueId, db);
            ViewBag.IdBanque = new SelectList(directionMetiers, "Id", "Nom");
            var model = new DirectionMetier();
            model.NiveauDossier = 1;
            model.NiveauMaxDossier = 13;
            return View(model);
        }

        // POST: DirectionMetiers/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "LireTouteReference,NiveauMaxDossier,Nom,Adresse,Ville,Pays,Telephone,Telephone2,NiveauDossier,IdTypeStructure,EstAgence,IdResponsable,IdBanque")] DirectionMetier directionMetier)
        {
            var banqueId = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);

            if (ModelState.IsValid)
            {
                directionMetier.IdBanque = banqueId;
                directionMetier.IdTypeStructure = db.GetTypeStructures.FirstOrDefault(t=>t.Intitule.ToLower().Contains("direction")).Id;
                directionMetier.IdBanque = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);
                db.DirectionMetiers.Add(directionMetier);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            var users = VariablGlobales.GetUsersByBanque(banqueId,db);
            var directionsmetiers = VariablGlobales.GetDirectionMetierByBanque(banqueId,db);

            ViewBag.IdResponsable = new SelectList(users, "Id", "Nom", directionMetier.IdResponsable);
            ViewBag.IdTypeStructure = new SelectList(db.GetTypeStructures, "Id", "Intitule", directionMetier.IdTypeStructure);
            ViewBag.IdBanque = new SelectList(directionsmetiers, "Id", "Nom", directionMetier.IdBanque);
            return View(directionMetier);
        }


        // GET: DirectionMetiers/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DirectionMetier directionMetier = await db.DirectionMetiers.FindAsync(id);
            int banqueId = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);
            if (directionMetier == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Edition direction m.";
            List<CompteBanqueCommerciale> users = VariablGlobales.GetUsersByBanque(banqueId, db);
            ViewData["users"] = users;// from u in users select new {Nom=u.NomComplet,Value=u.Id };
            users = null; ViewBag.IdTypeStructure = new SelectList(db.GetTypeStructures, "Id", "Intitule", directionMetier.IdTypeStructure);
            //ViewBag.IdBanque = new SelectList(db.GetBanques, "Id", "Nom", directionMetier.IdBanque);
            return View(directionMetier);
        }

        // POST: DirectionMetiers/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,NiveauMaxDossier,LireTouteReference,Nom,Adresse,Ville,Pays,Telephone,Telephone2,NiveauDossier,IdTypeStructure,EstAgence,IdResponsable,IdBanque")] DirectionMetier directionMetier)
        {
            var banqueId = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);
            if (ModelState.IsValid)
            {
                directionMetier.IdBanque = banqueId;
                directionMetier.IdBanque = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);
                db.Entry(directionMetier).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            var users = VariablGlobales.GetUsersByBanque(banqueId, db);
            var directionMetiers = VariablGlobales.GetDirectionMetierByBanque(banqueId, db);
            
            ViewBag.IdResponsable = new SelectList(users, "Id", "Nom", directionMetier.IdResponsable);
            ViewBag.IdTypeStructure = new SelectList(db.GetTypeStructures, "Id", "Intitule", directionMetier.IdTypeStructure);
            ViewBag.IdBanque = new SelectList(directionMetiers, "Id", "Nom", directionMetier.IdBanque);
            return View(directionMetier);
        }

        // GET: DirectionMetiers/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DirectionMetier directionMetier = await db.DirectionMetiers.FindAsync(id);
            if (directionMetier == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Suppression direction m.";
            return View(directionMetier);
        }

        // POST: DirectionMetiers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            string msg = "";
            DirectionMetier directionMetier = await db.DirectionMetiers.FindAsync(id);
            try
            {
                db.DirectionMetiers.Remove(directionMetier);
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
