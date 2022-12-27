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

namespace eApurement.Controllers
{
    public class SitesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Sites
        public async Task<ActionResult> Index(string tp="")
        {

            try
            {
                if (!string.IsNullOrEmpty(tp))
                {
                    var type = db.GetTypeStructures.FirstOrDefault(t => t.Intitule.ToLower() == tp.ToLower());

                    if (type != null)
                    {
                        ViewBag.navigation = "param";
                        ViewBag.navigation_msg = "Liste "+type.Intitule.ToLower()+"s";
                        ViewBag.tp = tp.ToLower() + "s";
                        return View(db.Agences.Where(a => a.IdTypeStructure == type.Id).Include(a => a.DirectionMetier).Include(a => a.Responsable).ToList());
                    }
                }
            }
            catch (Exception)
            { }
            var sites = db.Agences.Include(s => s.DirectionMetier).Include(s => s.Responsable).Include(s => s.TypeStructure);
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Liste structures";

            return View(await sites.ToListAsync());
        }

        // GET: Sites/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Agence site = await db.Agences.FindAsync(id);
            if (site == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Details site";
            return View(site);
        }

        // GET: Sites/Create
        public ActionResult Create()
        {
            var banqueId = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);
            ViewBag.idBank = banqueId;
            ViewBag.BanqueId = new SelectList(db.GetBanques, "Id", "Nom");
            ViewBag.ChefId = new SelectList(db.GetCompteBanqueCommerciales.Where(c=>c.Structure.BanqueId(db) == banqueId), "Id", "NomComplet");
            ViewBag.IdTypeStructure = new SelectList(db.GetTypeStructures, "Id", "Intitule");
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Creation site";
            return View();
        }

        // POST: Sites/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Nom,NiveauDossier,Adresse,Ville,Pays,Telephone,Telephone2,ChefId,BanqueId,IdTypeStructure,EstAgence")] Agence site)
        {
            if (ModelState.IsValid)
            {
                db.Agences.Add(site);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            var banqueId = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);
            ViewBag.idBank = banqueId;
            ViewBag.BanqueId = new SelectList(db.GetBanques, "Id", "Nom", site.BanqueId(db));
            ViewBag.ChefId = new SelectList(db.GetCompteBanqueCommerciales.Where(c => c.Structure.BanqueId(db) == banqueId), "Id", "NomComplet");
            ViewBag.IdTypeStructure = new SelectList(db.GetTypeStructures, "Id", "Intitule", site.IdTypeStructure);
            return View(site);
        }

        // GET: Sites/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Agence site = await db.Agences.FindAsync(id);
            if (site == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Edition site";
            ViewBag.BanqueId = new SelectList(db.GetBanques, "Id", "Nom", site.BanqueId(db));
            ViewBag.ChefId = new SelectList(db.Users, "Id", "Nom", site.IdResponsable);
            ViewBag.IdTypeStructure = new SelectList(db.GetTypeStructures, "Id", "Intitule", site.IdTypeStructure);
            return View(site);
        }

        // POST: Sites/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,NiveauDossier,Nom,Adresse,Ville,Pays,Telephone,Telephone2,ChefId,BanqueId,IdTypeStructure,EstAgence")] Agence site)
        {
            if (ModelState.IsValid)
            {
                db.Entry(site).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.BanqueId = new SelectList(db.GetBanques, "Id", "Nom", site.BanqueId(db));
            ViewBag.ChefId = new SelectList(db.Users, "Id", "Nom", site.IdResponsable);
            ViewBag.IdTypeStructure = new SelectList(db.GetTypeStructures, "Id", "Intitule", site.IdTypeStructure);
            return View(site);
        }

        // GET: Sites/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Agence site = await db.Agences.FindAsync(id);
            if (site == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Suppression site";
            return View(site);
        }

        // POST: Sites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Agence site = await db.Agences.FindAsync(id);
            db.Agences.Remove(site);
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
