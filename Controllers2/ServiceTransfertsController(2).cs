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
    public class ServiceTransfertsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ServiceTransferts
        public async Task<ActionResult> Index()
        {
            var structures = db.ServiceTransferts.Include(s => s.Banque).Include(s => s.Responsable).Include(s => s.TypeStructure);
            return View(await structures.ToListAsync());
        }

        // GET: ServiceTransferts/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceTransfert serviceTransfert = await db.ServiceTransferts.FindAsync(id);
            if (serviceTransfert == null)
            {
                return HttpNotFound();
            }
            return View(serviceTransfert);
        }

        // GET: ServiceTransferts/Create
        public ActionResult Create()
        {
            ViewBag.IdBanque = new SelectList(db.ServiceTransferts, "Id", "Nom");
            ViewBag.IdResponsable = new SelectList(db.Users, "Id", "Nom");
            ViewBag.IdTypeStructure = new SelectList(db.GetTypeStructures, "Id", "Intitule");
            var model = new ServiceTransfert();
            model.NiveauDossier = 5;
            model.NiveauMaxDossier = 13;
            return View(model);
        }

        // POST: ServiceTransferts/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "NiveauMaxDossier,LireTouteReference,,Nom,Adresse,Ville,Pays,Telephone,Telephone2,NiveauDossier,VoirDossiersAutres,VoirUsersAutres,VoirClientAutres,IdTypeStructure,EstAgence,NiveauH,IdBanque,IdResponsable")] ServiceTransfert serviceTransfert)
        {
            var banqueId = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);
            if (ModelState.IsValid)
            {
                serviceTransfert.IdBanque = banqueId;
                db.ServiceTransferts.Add(serviceTransfert);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.IdBanque = new SelectList(db.ServiceTransferts, "Id", "Nom", serviceTransfert.IdBanque);
            ViewBag.IdResponsable = new SelectList(db.Users, "Id", "Nom", serviceTransfert.IdResponsable);
            ViewBag.IdTypeStructure = new SelectList(db.GetTypeStructures, "Id", "Intitule", serviceTransfert.IdTypeStructure);
            return View(serviceTransfert);
        }

        // GET: ServiceTransferts/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var banqueId = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);
            ServiceTransfert serviceTransfert = await db.ServiceTransferts.FirstOrDefaultAsync(c => c.IdBanque == banqueId);
            if (serviceTransfert == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdBanque = new SelectList(db.ServiceTransferts, "Id", "Nom", serviceTransfert.IdBanque);
            ViewBag.IdResponsable = new SelectList(db.Users, "Id", "Nom", serviceTransfert.IdResponsable);
            ViewBag.IdTypeStructure = new SelectList(db.GetTypeStructures, "Id", "Intitule", serviceTransfert.IdTypeStructure);
            return View(serviceTransfert);
        }

        // POST: ServiceTransferts/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,LireTouteReference,,NiveauMaxDossier,Nom,Adresse,Ville,Pays,Telephone,Telephone2,NiveauDossier,VoirDossiersAutres,VoirUsersAutres,VoirClientAutres,IdTypeStructure,EstAgence,NiveauH,IdBanque,IdResponsable")] ServiceTransfert serviceTransfert)
        {
            var banqueId = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);
            if (ModelState.IsValid)
            {
                serviceTransfert.IdBanque = banqueId;
                db.Entry(serviceTransfert).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.IdBanque = new SelectList(db.ServiceTransferts, "Id", "Nom", serviceTransfert.IdBanque);
            ViewBag.IdResponsable = new SelectList(db.Users, "Id", "Nom", serviceTransfert.IdResponsable);
            ViewBag.IdTypeStructure = new SelectList(db.GetTypeStructures, "Id", "Intitule", serviceTransfert.IdTypeStructure);
            return View(serviceTransfert);
        }

        // GET: ServiceTransferts/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceTransfert serviceTransfert = await db.ServiceTransferts.FindAsync(id);
            if (serviceTransfert == null)
            {
                return HttpNotFound();
            }
            return View(serviceTransfert);
        }

        // POST: ServiceTransferts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ServiceTransfert serviceTransfert = await db.ServiceTransferts.FindAsync(id);
            db.ServiceTransferts.Remove(serviceTransfert);
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
