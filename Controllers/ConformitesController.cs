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
using genetrix.Models;

namespace genetrix.Controllers
{
    public class ConformitesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Conformites
        public async Task<ActionResult> Index()
        {
            var structures = db.Conformites.Include(c => c.Responsable).Include(c => c.TypeStructure);//.Include(c => c.DirectionMetier);
            return View(await structures.ToListAsync());
        }

        // GET: Conformites/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Conformite conformite = await db.Conformites.FindAsync(id);
            if (conformite == null)
            {
                if(id==100)
                {
                    if (db.Conformites.Count() > 0)
                        return View(await db.Conformites.FirstAsync());
                    else
                        return RedirectToAction("create");
                }
                return HttpNotFound();
            }
            return View(conformite);
        }

        // GET: Conformites/Create
        public ActionResult Create()
        {
            var structure = db.Structures.Find(Session["IdStructure"]);
            var banqueId = structure.BanqueId(db);
            structure = null;
            int min = 0;
            try
            {
                min = Convert.ToInt32(Session["userSIteMinNiveau"]);
            }
            catch (Exception)
            { }
            List<CompteBanqueCommerciale> users = VariablGlobales.GetUsersByBanque(banqueId, db, min, Session["role"].ToString());

            ViewData["users"] = users;// from u in users select new {Nom=u.NomComplet,Value=u.Id };
            users = null;
            ViewBag.IdTypeStructure = new SelectList(db.GetTypeStructures, "Id", "Intitule");
            ViewBag.IdDirectionMetier = new SelectList(db.Conformites, "Id", "Nom");
            var model = new Conformite();
            model.NiveauDossier = 6;
            model.NiveauMaxManager = 7;
            model.NiveauMaxDossier = 21;
            return View(model);
        }

        // POST: Conformites/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "NiveauMaxDossier,NiveauMaxManager,NiveauMaxDossier,LireTouteReference,,Nom,Adresse,Ville,Pays,Telephone,Telephone2,NiveauDossier,VoirDossiersAutres,VoirUsersAutres,VoirClientAutres,IdTypeStructure,EstAgence,NiveauH,IdResponsable,IdDirectionMetier")] Conformite conformite)
        {
            var structure = db.Structures.Find(Session["IdStructure"]);
            var banqueId = structure.BanqueId(db);
            structure = null;
            if (ModelState.IsValid)
            {
                try
                {
                    conformite.IdBanque = banqueId;
                    db.Conformites.Add(conformite);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception ee)
                {}
            }

            ViewBag.IdResponsable = new SelectList(db.Users, "Id", "Nom", conformite.IdResponsable);
            ViewBag.IdTypeStructure = new SelectList(db.GetTypeStructures, "Id", "Intitule", conformite.IdTypeStructure);
            //ViewBag.IdDirectionMetier = new SelectList(db.Conformites, "Id", "Nom", conformite.IdDirectionMetier);
            return View(conformite);
        }

        // GET: Conformites/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            var structure = db.Structures.Find(Session["IdStructure"]);
            var banqueId = structure.BanqueId(db);
            structure = null;

            if (id == null)
            {
                return View("Erreur");
            }
            if (id != null)
            {
                return RedirectToAction("Edit", "Structures", new { id = id });
            }
            Conformite conformite = await db.Conformites.FirstOrDefaultAsync(c=>c.IdBanque==banqueId);
            if (conformite == null)
            {
                return View("Erreur");
            }
            ViewBag.IdResponsable = new SelectList(db.Users, "Id", "Nom", conformite.IdResponsable);
            ViewBag.IdTypeStructure = new SelectList(db.GetTypeStructures, "Id", "Intitule", conformite.IdTypeStructure);
            //ViewBag.IdDirectionMetier = new SelectList(db.Conformites, "Id", "Nom", conformite.IdDirectionMetier);
            return View(conformite);
        }

        // POST: Conformites/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "NiveauMaxDossier,NiveauMaxManager,Id,LireTouteReference,NiveauMaxDossier,Nom,Adresse,Ville,Pays,Telephone,Telephone2,NiveauDossier,VoirDossiersAutres,VoirUsersAutres,VoirClientAutres,IdTypeStructure,EstAgence,NiveauH,IdResponsable,IdDirectionMetier")] Conformite conformite)
        {
            var structure = db.Structures.Find(Session["IdStructure"]);
            var banqueId = structure.BanqueId(db);
            structure = null;
            if (ModelState.IsValid)
            {
                conformite.IdBanque = banqueId;
                db.Entry(conformite).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.IdResponsable = new SelectList(db.Users, "Id", "Nom", conformite.IdResponsable);
            ViewBag.IdTypeStructure = new SelectList(db.GetTypeStructures, "Id", "Intitule", conformite.IdTypeStructure);
            //ViewBag.IdDirectionMetier = new SelectList(db.Conformites, "Id", "Nom", conformite.IdDirectionMetier);
            return View(conformite);
        }

        // GET: Conformites/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Conformite conformite = await db.Conformites.FindAsync(id);
            if (conformite == null)
            {
                return HttpNotFound();
            }
            return View(conformite);
        }

        // POST: Conformites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Conformite conformite = await db.Conformites.FindAsync(id);
            db.Conformites.Remove(conformite);
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
