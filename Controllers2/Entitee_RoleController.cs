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
    public class Entitee_RoleController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Entitee_Role
        public async Task<ActionResult> Index()
        {
            var getEntitee_Roles = db.GetEntitee_Roles.Include(e => e.Entitee).Include(e => e.XRole);
            return View(await getEntitee_Roles.ToListAsync());
        }

        // GET: Entitee_Role/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Entitee_Role entitee_Role = await db.GetEntitee_Roles.FindAsync(id);
            if (entitee_Role == null)
            {
                return HttpNotFound();
            }
            return View(entitee_Role);
        }

        // GET: Entitee_Role/Create
        public ActionResult Create()
        {
            ViewBag.IdEntitee = new SelectList(db.GetEntitees, "Id", "Type");
            ViewBag.IdXRole = new SelectList(db.XtraRoles, "RoleId", "Nom");
            return View();
        }

        // POST: Entitee_Role/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IdXRole,IdEntitee,Lire,Ecrire,Supprimer,Créer")] Entitee_Role entitee_Role)
        {
            if (ModelState.IsValid)
            {
                db.GetEntitee_Roles.Add(entitee_Role);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.IdEntitee = new SelectList(db.GetEntitees, "Id", "Type", entitee_Role.IdEntitee);
            ViewBag.IdXRole = new SelectList(db.XtraRoles, "RoleId", "Nom", entitee_Role.IdXRole);
            return View(entitee_Role);
        }

        // GET: Entitee_Role/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Entitee_Role entitee_Role = await db.GetEntitee_Roles.FindAsync(id);
            if (entitee_Role == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdEntitee = new SelectList(db.GetEntitees, "Id", "Type", entitee_Role.IdEntitee);
            ViewBag.IdXRole = new SelectList(db.XtraRoles, "RoleId", "Nom", entitee_Role.IdXRole);
            return View(entitee_Role);
        }

        // POST: Entitee_Role/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IdXRole,IdEntitee,Lire,Ecrire,Supprimer,Créer")] Entitee_Role entitee_Role)
        {
            if (ModelState.IsValid)
            {
                db.Entry(entitee_Role).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.IdEntitee = new SelectList(db.GetEntitees, "Id", "Type", entitee_Role.IdEntitee);
            ViewBag.IdXRole = new SelectList(db.XtraRoles, "RoleId", "Nom", entitee_Role.IdXRole);
            return View(entitee_Role);
        }

        // GET: Entitee_Role/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Entitee_Role entitee_Role = await db.GetEntitee_Roles.FindAsync(id);
            if (entitee_Role == null)
            {
                return HttpNotFound();
            }
            return View(entitee_Role);
        }

        // POST: Entitee_Role/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Entitee_Role entitee_Role = await db.GetEntitee_Roles.FindAsync(id);
            db.GetEntitee_Roles.Remove(entitee_Role);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (Session != null)
            {
                var url = (string)Session["urlaccueil"];
                filterContext.Result = Redirect(url);
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
