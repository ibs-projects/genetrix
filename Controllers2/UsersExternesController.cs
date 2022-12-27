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
    public class UsersExternesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: UsersExternes
        public async Task<ActionResult> Index()
        {
            var getUsersExternes = db.GetUsersExternes.Include(u => u.Banque);
            return View(await getUsersExternes.ToListAsync());
        }

        // GET: UsersExternes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsersExterne usersExterne = await db.GetUsersExternes.FindAsync(id);
            if (usersExterne == null)
            {
                return HttpNotFound();
            }
            return View(usersExterne);
        }

        // GET: UsersExternes/Create
        public ActionResult Create()
        {
            ViewBag.BanqueId = new SelectList(db.Structures, "Id", "Nom");
            return View();
        }

        // POST: UsersExternes/Create
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,NomComplet,Telephone,Email,BanqueId")] UsersExterne usersExterne)
        {
            if (ModelState.IsValid)
            {
                db.GetUsersExternes.Add(usersExterne);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.BanqueId = new SelectList(db.Structures, "Id", "Nom", usersExterne.BanqueId);
            return View(usersExterne);
        }

        // GET: UsersExternes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsersExterne usersExterne = await db.GetUsersExternes.FindAsync(id);
            if (usersExterne == null)
            {
                return HttpNotFound();
            }
            ViewBag.BanqueId = new SelectList(db.Structures, "Id", "Nom", usersExterne.BanqueId);
            return View(usersExterne);
        }

        // POST: UsersExternes/Edit/5
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,NomComplet,Telephone,Email,BanqueId")] UsersExterne usersExterne)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usersExterne).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.BanqueId = new SelectList(db.Structures, "Id", "Nom", usersExterne.BanqueId);
            return View(usersExterne);
        }

        // GET: UsersExternes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsersExterne usersExterne = await db.GetUsersExternes.FindAsync(id);
            if (usersExterne == null)
            {
                return HttpNotFound();
            }
            return View(usersExterne);
        }

        // POST: UsersExternes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            UsersExterne usersExterne = await db.GetUsersExternes.FindAsync(id);
            db.GetUsersExternes.Remove(usersExterne);
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
