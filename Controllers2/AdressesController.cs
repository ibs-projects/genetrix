using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using genetrix.Models;

namespace genetrix.Controllers
{
    public class AdressesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Adresses
        public ActionResult Index()
        {

            var clientId = Convert.ToInt32(Session["ClientId"]);
            return View(db.GetAdresses.Where(a=>a.ClientId==clientId).ToList());
        }

        // GET: Adresses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Adresse adresse = db.GetAdresses.Find(id);
            if (adresse == null)
            {
                return HttpNotFound();
            }
            return View(adresse);
        }

        // GET: Adresses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Adresses/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Tel1,ClientId,Email,CodePostal")] Adresse adresse)
        {
            if (ModelState.IsValid)
            {
                db.GetAdresses.Add(adresse);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(adresse);
        }

        // GET: Adresses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Adresse adresse = db.GetAdresses.Find(id);
            if (adresse == null)
            {
                return HttpNotFound();
            }
            return View(adresse);
        }

        // POST: Adresses/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Tel1,ClientId,Email,CodePostal")] Adresse adresse)
        {
            if (ModelState.IsValid)
            {
                db.Entry(adresse).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(adresse);
        }

        // GET: Adresses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Adresse adresse = db.GetAdresses.Find(id);
            if (adresse == null)
            {
                return HttpNotFound();
            }
            return View(adresse);
        }

        // POST: Adresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            Adresse adresse = db.GetAdresses.Find(id);
            db.GetAdresses.Remove(adresse);
            db.SaveChanges();
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
