using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using eApurement.Models;
using e_apurement.Models;

namespace eApurement.Controllers
{
    public class IHMsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: IHMs
        public ActionResult Index()
        {
            var getIHMs = db.GetIHMs.Include(i => i.Composant).Include(i => i.XRole);
            return View(getIHMs.ToList());
        }

        // GET: IHMs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IHM iHM = db.GetIHMs.Find(id);
            if (iHM == null)
            {
                return HttpNotFound();
            }
            return View(iHM);
        }

        // GET: IHMs/Create
        public ActionResult Create()
        {
            ViewBag.ComposantId = new SelectList(db.GetComposants, "Id", "Description");
            ViewBag.XRoleId = new SelectList(db.XtraRoles, "RoleId", "Nom");
            return View();
        }

        // POST: IHMs/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "XRoleId,ComposantId,Lire,Ecrire,Supprimer,Créer")] IHM iHM)
        {
            if (ModelState.IsValid)
            {
                db.GetIHMs.Add(iHM);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ComposantId = new SelectList(db.GetComposants, "Id", "Description", iHM.ComposantId);
            ViewBag.XRoleId = new SelectList(db.XtraRoles, "RoleId", "Nom", iHM.XRoleId);
            return View(iHM);
        }

        // GET: IHMs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IHM iHM = db.GetIHMs.Find(id);
            if (iHM == null)
            {
                return HttpNotFound();
            }
            ViewBag.ComposantId = new SelectList(db.GetComposants, "Id", "Description", iHM.ComposantId);
            ViewBag.XRoleId = new SelectList(db.XtraRoles, "RoleId", "Nom", iHM.XRoleId);
            return View(iHM);
        }

        // POST: IHMs/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "XRoleId,ComposantId,Lire,Ecrire,Supprimer,Créer")] IHM iHM)
        {
            if (ModelState.IsValid)
            {
                db.Entry(iHM).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ComposantId = new SelectList(db.GetComposants, "Id", "Description", iHM.ComposantId);
            ViewBag.XRoleId = new SelectList(db.XtraRoles, "RoleId", "Nom", iHM.XRoleId);
            return View(iHM);
        }

        // GET: IHMs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IHM iHM = db.GetIHMs.Find(id);
            if (iHM == null)
            {
                return HttpNotFound();
            }
            return View(iHM);
        }

        // POST: IHMs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            IHM iHM = db.GetIHMs.Find(id);
            db.GetIHMs.Remove(iHM);
            db.SaveChanges();
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
