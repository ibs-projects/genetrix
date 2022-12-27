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

namespace eApurement.Controllers.Banque
{
    public class StructuresController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Structures
        public ActionResult Index(string tp="",string msg="")
        {
            switch (tp)
            {
                case "Agences": return RedirectToAction("index", tp);
                case "Banques": return RedirectToAction("index", tp);
                case "Conformités": return RedirectToAction("index", "conformites");
                case "DirectionMetiers": return RedirectToAction("index", tp);
                case "Service transferts": return RedirectToAction("index", "servicetransferts");
                default:
                    break;
            }

            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Liste de structures";
            var dd = db.Structures.Include(s => s.Responsable).Include(s => s.TypeStructure);
            var banqueId = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);
            List<Structure> structures = VariablGlobales.GetStructureByBanque(banqueId, db);
            ViewBag.msg = msg;
            return View(structures.ToList());
        }

        // GET: Structures/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Structure structure = await db.Structures.FindAsync(id);
            if (structure == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Details structure";
            return View(structure);
        }

        // GET: Structures/Create
        public ActionResult Create()
        {
            var banqueId = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);
            List<CompteBanqueCommerciale> users = VariablGlobales.GetUsersByBanque(banqueId, db);
            ViewBag.IdResponsable = new SelectList(users, "Id", "Nom");
            ViewBag.IdTypeStructure = new SelectList(db.GetTypeStructures, "Id", "Intitule");
            return View();
        }

        // POST: Structures/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "LireTouteReference,Nom,Adresse,NiveauMaxDossier,Ville,Pays,Telephone,Telephone2,NiveauDossier,IdTypeStructure,EstAgence,IdResponsable")] Structure structure)
        {
            var banqueId = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);

            if (ModelState.IsValid)
            {
                db.Structures.Add(structure);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            List<CompteBanqueCommerciale> users = VariablGlobales.GetUsersByBanque(banqueId, db);
            ViewBag.IdResponsable = new SelectList(users, "Id", "Nom", structure.IdResponsable);
            ViewBag.IdTypeStructure = new SelectList(db.GetTypeStructures, "Id", "Intitule", structure.IdTypeStructure);
            users = null;
            return View(structure);
        }

        // GET: Structures/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Structure structure = await db.Structures.Include(s=>s.Agents).Include(s=>s.Agents).FirstOrDefaultAsync(s=>s.Id==id);
            if (structure == null)
            {
                return HttpNotFound();
            }

            var ddd = db.GetIHMStructures.Where(i => i.IdStructure == id).ToList();
            var idComp = (from d in ddd select d.ComposantId + "_" + d.IdStructure).ToList();
            if (idComp == null) idComp = new List<string>();

            List<Composant> composants = new List<Composant>();
            foreach (var item in db.GetComposants.ToList())
            {
                if (idComp.Contains(item.Id + "_" + id)) continue;
                composants.Add(item);
            }
            ViewBag.ihms = ddd;
            ddd = null;
            ViewBag.Composants = composants;

            int banqueId = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);
            List<CompteBanqueCommerciale> _users = new List<CompteBanqueCommerciale>();
            List<CompteBanqueCommerciale> ow_users = new List<CompteBanqueCommerciale>();
            foreach (var item in db.GetCompteBanqueCommerciales.Include(c=>c.Structure).ToList())
            {
                try
                {
                    var _id = item.Structure.BanqueId(db);
                    if (_id == banqueId && structure.Id!=item.IdStructure)
                    {
                        //if (structure.Agents.Count()>0 && structure.Agents.FirstOrDefault(u => u.Id == item.Id) == null)
                        //    _users.Add(item);
                        //else ow_users.Add(item);
                        _users.Add(item);
                    }
                    else
                    {
                        if(structure.Id == item.IdStructure)
                        ow_users.Add(item);
                    }
                
                }
                catch (Exception)
                {
                    _users.Add(item);
                }
            }

            ViewBag.users = _users.ToList();
            ViewBag.ow_users = ow_users.ToList();
            _users = null;
            ow_users = null;
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Edition structure";
            List<CompteBanqueCommerciale> users = new List<CompteBanqueCommerciale>();
            foreach (var item in db.GetCompteBanqueCommerciales.Include(b => b.Structure))
            {
                try
                {
                    if (item.Structure.BanqueId(db) != banqueId) continue;
                    users.Add(item);
                }
                catch (Exception)
                { }
            }
            ViewBag.IdResponsable = new SelectList(users, "Id", "Nom", structure.IdResponsable);
            ViewBag.IdTypeStructure = new SelectList(db.GetTypeStructures, "Id", "Intitule", structure.IdTypeStructure);
            return View(structure);
        }

        [HttpPost]
        public ActionResult AddComposant(FormCollection form)
        {
            var id = form["StructureId"];
            if (form.Keys.Count > 0)
                foreach (var k in form.Keys)
                {
                    try
                    {
                        if (k.ToString() != "StructureId")
                        {
                            db.GetIHMStructures.Add(new IHMStructure()
                            {
                                ComposantId = int.Parse(form[k.ToString()]),
                                IdStructure = int.Parse(id),
                                Lire = true
                            });
                            db.SaveChanges();
                        }
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
            return RedirectToAction("Edit", new { id = id });
        }

        [HttpPost]
        public ActionResult AddUser(FormCollection form)
        {
            var id = form["StructureId"];
            CompteBanqueCommerciale user = null;
            if (form.Keys.Count > 0)
                foreach (var k in form.Keys)
                {
                    try
                    {
                        user = db.Users.Find(form[k.ToString()]) as CompteBanqueCommerciale;
                        user.IdStructure = int.Parse(id);
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
            return RedirectToAction("Edit", new { id = id });
        }

        // POST: Structures/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,LireTouteReference,Nom,Adresse,NiveauMaxDossier,Ville,Pays,Telephone,Telephone2,NiveauDossier,IdTypeStructure,EstAgence,IdResponsable")] Structure structure)
        {
            var banqueId = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);

            if (ModelState.IsValid)
            {
                db.Entry(structure).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            List<CompteBanqueCommerciale> users = VariablGlobales.GetUsersByBanque(banqueId, db);
            ViewBag.IdResponsable = new SelectList(users, "Id", "Nom", structure.IdResponsable);
            ViewBag.IdTypeStructure = new SelectList(db.GetTypeStructures, "Id", "Intitule", structure.IdTypeStructure);
            return View(structure);
        }

        // GET: Structures/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Structure structure = await db.Structures.FindAsync(id);
            if (structure == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Suppression structure";
            return View(structure);
        }

        // POST: Structures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Structure structure = await db.Structures.FindAsync(id);
            string msg = "";
            try
            {
                db.Structures.Remove(structure);
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                msg = "Erreur code 2563. Impossible de supprimer de l'entitée structure.";
            }
            return RedirectToAction("Index",new { msg=msg});
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
