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
    public class CompteNostroesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CompteNostroes
        public async Task<ActionResult> Index()
        {
            var compteNostroes = db.CompteNostroes.Include(c => c.Correspondant).Include(c => c.Devise);
            return View(await compteNostroes.ToListAsync());
        }

        // GET: CompteNostroes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompteNostro compteNostro = await db.CompteNostroes.FindAsync(id);
            if (compteNostro == null)
            {
                return HttpNotFound();
            }
            return View(compteNostro);
        }

        // GET: CompteNostroes/Create
        public ActionResult Create()
        {
            ViewBag.IdCorrespondant = new SelectList(db.GetCorrespondants, "Id", "NomBanque");
            ViewBag.IdDevise = new SelectList(db.GetDeviseMonetaires, "Id", "Nom");
            return View();
        }

        // POST: CompteNostroes/Create
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Numero,Cle,RIB,Libellé,IdDevise,IdCorrespondant")] CompteNostro compteNostro)
        {
            if (ModelState.IsValid)
            {
                db.CompteNostroes.Add(compteNostro);
                await db.SaveChangesAsync();
                //return RedirectToAction("Index");
                return RedirectToAction("Edit", "Correspondants",new { id=compteNostro.IdCorrespondant});
            }

            ViewBag.IdCorrespondant = new SelectList(db.GetCorrespondants, "Id", "NomBanque", compteNostro.IdCorrespondant);
            ViewBag.IdDevise = new SelectList(db.GetDeviseMonetaires, "Id", "Nom", compteNostro.IdDevise);
            return View(compteNostro);
        }

        // GET: CompteNostroes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompteNostro compteNostro = await db.CompteNostroes.FindAsync(id);
            if (compteNostro == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdCorrespondant = new SelectList(db.GetCorrespondants, "Id", "NomBanque", compteNostro.IdCorrespondant);
            ViewBag.IdDevise = new SelectList(db.GetDeviseMonetaires, "Id", "Nom", compteNostro.IdDevise);
            return View(compteNostro);
        }

        // POST: CompteNostroes/Edit/5
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Cle,RIB,Numero,Libellé,IdDevise,IdCorrespondant")] CompteNostro compteNostro)
        {
            if (ModelState.IsValid)
            {
                db.Entry(compteNostro).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Edit", "Correspondants", new { id = compteNostro.IdCorrespondant });
            }
            ViewBag.IdCorrespondant = new SelectList(db.GetCorrespondants, "Id", "NomBanque", compteNostro.IdCorrespondant);
            ViewBag.IdDevise = new SelectList(db.GetDeviseMonetaires, "Id", "Nom", compteNostro.IdDevise);
            return View(compteNostro);
        }

        // GET: CompteNostroes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompteNostro compteNostro = await db.CompteNostroes.FindAsync(id);
            if (compteNostro == null)
            {
                return HttpNotFound();
            }
            return View(compteNostro);
        }

        // POST: CompteNostroes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            CompteNostro compteNostro = await db.CompteNostroes.FindAsync(id);
            db.CompteNostroes.Remove(compteNostro);
            await db.SaveChangesAsync();
            return RedirectToAction("Edit", "Correspondants", new { id = compteNostro.Id });
            //return RedirectToAction("Index");
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
