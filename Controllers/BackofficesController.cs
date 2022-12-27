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
    public class BackofficesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Backoffices
        public async Task<ActionResult> Index()
        {
            if(Session == null)
                return RedirectToAction("login", "auth");
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Liste back-offices";
            int min = 0;
            try
            {
                min = Convert.ToInt32(Session["userSIteMinNiveau"]);
            }
            catch (Exception)
            { }
            IQueryable<Backoffice> structures = null;
            if (Convert.ToBoolean(Session["EstAdmin"]))
            {
                structures = db.GetBackoffices.Include(b => b.Responsable).Include(b => b.TypeStructure).Include(b => b.Agence);
            }
            else if(min==1)
            {
                var idAgence = Convert.ToInt32(Session["IdStructure"]);
                structures = db.GetBackoffices.Include(b => b.Responsable).Include(b => b.TypeStructure).Include(b => b.Agence).Where(b=>b.IdAgence== idAgence);
            }
            return View(await structures.ToListAsync());
        }

        // GET: Backoffices/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Backoffice backoffice = await db.GetBackoffices.FindAsync(id);
            if (backoffice == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Details back-office";
            return View(backoffice);
        }

        // GET: Backoffices/Create
        public ActionResult Create()
        {
            var banque = Session["IdStructure"];

            ViewBag.IdResponsable = new SelectList(db.Users, "Id", "Nom");
            ViewBag.IdTypeStructure = new SelectList(db.GetTypeStructures, "Id", "Intitule");
      
            ViewBag.IdAgence = new SelectList(db.Agences, "Id", "Nom");
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Creation back-office";

            return View(new Backoffice() { 
                NiveauDossier=4,
                NiveauH=4,
                NiveauMaxDossier=5,
                NiveauMaxManager=4
            });
        }

        // POST: Backoffices/Create
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Nom,Adresse,Ville,Pays,Telephone,Telephone2,NiveauDossier,NiveauMaxManager,NiveauMaxDossier,VoirDossiersAutres,VoirUsersAutres,VoirClientAutres,IdTypeStructure,EstAgence,NiveauH,IdResponsable,LireTouteReference,IdAgence")] Backoffice backoffice)
        {
            if (ModelState.IsValid)
            {
                db.GetBackoffices.Add(backoffice);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.IdResponsable = new SelectList(db.Users, "Id", "Nom", backoffice.IdResponsable);
            ViewBag.IdTypeStructure = new SelectList(db.GetTypeStructures, "Id", "Intitule", backoffice.IdTypeStructure);
            ViewBag.IdAgence = new SelectList(db.GetBackoffices, "Id", "Nom", backoffice.IdAgence);
            return View(backoffice);
        }

        // GET: Backoffices/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (id != null)
            {
                return RedirectToAction("Edit", "Structures", new { id = id });
            }
            Backoffice backoffice = await db.GetBackoffices.FindAsync(id);
            if (backoffice == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Edition back-office";
            ViewBag.IdResponsable = new SelectList(db.Users, "Id", "NomComplet", backoffice.IdResponsable);
            ViewBag.IdTypeStructure = new SelectList(db.GetTypeStructures, "Id", "Intitule", backoffice.IdTypeStructure);
            ViewBag.IdAgence = new SelectList(db.GetBackoffices, "Id", "Nom", backoffice.IdAgence);
            return View(backoffice);
        }

        // POST: Backoffices/Edit/5
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Nom,Adresse,Ville,Pays,Telephone,Telephone2,NiveauDossier,NiveauMaxManager,NiveauMaxDossier,VoirDossiersAutres,VoirUsersAutres,VoirClientAutres,IdTypeStructure,EstAgence,NiveauH,IdResponsable,LireTouteReference,IdAgence")] Backoffice backoffice)
        {
            if (ModelState.IsValid)
            {
                db.Entry(backoffice).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.IdResponsable = new SelectList(db.Users, "Id", "Nom", backoffice.IdResponsable);
            ViewBag.IdTypeStructure = new SelectList(db.GetTypeStructures, "Id", "Intitule", backoffice.IdTypeStructure);
            ViewBag.IdAgence = new SelectList(db.GetBackoffices, "Id", "Nom", backoffice.IdAgence);
            return View(backoffice);
        }

        // GET: Backoffices/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Backoffice backoffice = await db.GetBackoffices.FindAsync(id);
            if (backoffice == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Suppression back-office";
            return View(backoffice);
        }

        // POST: Backoffices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Backoffice backoffice = await db.GetBackoffices.FindAsync(id);
            db.GetBackoffices.Remove(backoffice);
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
