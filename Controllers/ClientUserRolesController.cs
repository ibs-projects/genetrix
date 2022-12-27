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
    public class ClientUserRolesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ClientUserRoles
        public async Task<ActionResult> Index()
        {
            return View(await db.ClientUserRoles.ToListAsync());
        }

        // GET: ClientUserRoles/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientUserRole clientUserRole = await db.ClientUserRoles.FindAsync(id);
            if (clientUserRole == null)
            {
                return HttpNotFound();
            }
            return View(clientUserRole);
        }

        // GET: ClientUserRoles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ClientUserRoles/Create
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Nom,CreerDossier,SoumettreDossier,CreerUser,SuppUser,ModifUser,CreerBenef,SuppBenef,ModifBenef")] ClientUserRole clientUserRole)
        {
            if (ModelState.IsValid)
            {
                db.ClientUserRoles.Add(clientUserRole);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(clientUserRole);
        }

        // GET: ClientUserRoles/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientUserRole clientUserRole = await db.ClientUserRoles.FindAsync(id);
            if (clientUserRole == null)
            {
                return HttpNotFound();
            }
            return View(clientUserRole);
        }

        // POST: ClientUserRoles/Edit/5
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Nom,CreerDossier,SoumettreDossier,CreerUser,SuppUser,ModifUser,CreerBenef,SuppBenef,ModifBenef")] ClientUserRole clientUserRole)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clientUserRole).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(clientUserRole);
        }

        // GET: ClientUserRoles/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientUserRole clientUserRole = await db.ClientUserRoles.FindAsync(id);
            if (clientUserRole == null)
            {
                return HttpNotFound();
            }
            return View(clientUserRole);
        }

        // POST: ClientUserRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ClientUserRole clientUserRole = await db.ClientUserRoles.FindAsync(id);
            db.ClientUserRoles.Remove(clientUserRole);
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
