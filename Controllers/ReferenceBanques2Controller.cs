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
    public class ReferenceBanques2Controller : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ReferenceBanques
        public async Task<ActionResult> Index()
        {
            var getReferenceBanques = db.GetReferenceBanques.Include(r => r.Banque).Include(r => r.Client);
            return View(await getReferenceBanques.ToListAsync());
        }

        // GET: ReferenceBanques/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReferenceBanque referenceBanque = await db.GetReferenceBanques.FindAsync(id);
            if (referenceBanque == null)
            {
                return HttpNotFound();
            }
            return View(referenceBanque);
        }

        // GET: ReferenceBanques/attache
        public ActionResult Attache()
        {
            ViewBag.ReferenceId = new SelectList(db.GetReferenceBanques.ToList(), "Id", "NumeroRef");
            var siteID = (Session["user"] as CompteBanqueCommerciale).IdSite;
            var dossiers = db.GetDossiers.Where(d => d.IdSite == siteID & d.EtapesDosier != 52).Include(d => d.Site).Include(d => d.DeviseMonetaire).Include(d => d.ReferenceExterne);
            ViewBag.statut = "/ Attacher les références";
            return View(dossiers.OrderBy(d=>d.ClientId).ToList());
        }
        
        [HttpPost]
        public ActionResult Attachement(FormCollection form)
        {
            Dossier dossier = null;
            int dossierId = 0;
            if (form.Keys.Count > 0)
                foreach (var k in form.Keys)
                {
                    try
                    {
                        dossierId = int.Parse(k.ToString());
                        dossier = db.GetDossiers.Find(dossierId);
                        dossier.ReferenceExterneId = int.Parse(form[k.ToString()]);
                        db.SaveChanges();
                    }
                    catch (Exception ee)
                    { }
                }
            try
            {
                db.SaveChanges();
            }
            catch (Exception ee)
            { }
            dossier = null;
            return RedirectToAction("Attache");
        }
        
        // GET: ReferenceBanques/Create
        public ActionResult Create()
        {
            ViewBag.BanqueId = new SelectList(db.GetBanques, "Id", "Nom");
            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom");
            return View();
        }

        // POST: ReferenceBanques/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "NumeroRef,DateEmission,DateEcheance,NombreMois")] ReferenceBanque referenceBanque)
        {
            if (ModelState.IsValid)
            {
                referenceBanque.BanqueId = (int)(Session["user"] as CompteBanqueCommerciale).IdBanque;
                db.GetReferenceBanques.Add(referenceBanque);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.BanqueId = new SelectList(db.GetBanques, "Id", "Nom", referenceBanque.BanqueId);
            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom", referenceBanque.ClientId);
            return View(referenceBanque);
        }

        // GET: ReferenceBanques/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReferenceBanque referenceBanque = await db.GetReferenceBanques.FindAsync(id);
            if (referenceBanque == null)
            {
                return HttpNotFound();
            }
            ViewBag.BanqueId = new SelectList(db.GetBanques, "Id", "Nom", referenceBanque.BanqueId);
            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom", referenceBanque.ClientId);
            return View(referenceBanque);
        }

        // POST: ReferenceBanques/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,NumeroRef,DateEmission,DateEcheance,NombreMois,Montant,ClientEntre,ClientId,BanqueId")] ReferenceBanque referenceBanque)
        {
            if (ModelState.IsValid)
            {
                db.Entry(referenceBanque).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.BanqueId = new SelectList(db.GetBanques, "Id", "Nom", referenceBanque.BanqueId);
            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom", referenceBanque.ClientId);
            return View(referenceBanque);
        }

        // GET: ReferenceBanques/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReferenceBanque referenceBanque = await db.GetReferenceBanques.FindAsync(id);
            if (referenceBanque == null)
            {
                return HttpNotFound();
            }
            return View(referenceBanque);
        }

        // POST: ReferenceBanques/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            ReferenceBanque referenceBanque = await db.GetReferenceBanques.FindAsync(id);
            db.GetReferenceBanques.Remove(referenceBanque);
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
