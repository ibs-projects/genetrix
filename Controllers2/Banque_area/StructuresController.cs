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
using genetrix.Models.Fonctions;

namespace genetrix.Controllers.Banque
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
                case "Back-offices": return RedirectToAction("index", "backoffices");
                default:
                    break;
            }

            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Liste de structures";
            var idStruct = Convert.ToInt32(Session["IdStructure"]);
            var structure = db.Structures.Include(s => s.Responsable).Include(s => s.TypeStructure).FirstOrDefault(s=>s.Id == idStruct);
            var banqueId = structure.BanqueId(db);
            List<Structure> structures = VariablGlobales.GetStructureByBanque(banqueId, db);
            try
            {
                if (!string.IsNullOrEmpty(tp) && structures != null)
                    structures = structures.Where(s => !string.IsNullOrEmpty(s.Nom) && s.Nom.Contains(tp)
                     || s.TypeStructure.Intitule.Contains(tp)
                     || s.TypeStructure.Intitule.Equals(tp, StringComparison.OrdinalIgnoreCase)
                     || s.Nom.Equals(tp, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            catch (Exception)
            {}
            ViewBag.msg = msg;
            structure = null;
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
            return View();
        }

        // POST: Structures/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "SuitChat,NiveauMaxManager,NiveauMaxDossier,LireTouteReference,Nom,Adresse,NiveauMaxDossier,Ville,Pays,Telephone,Telephone2,NiveauDossier,IdTypeStructure,EstAgence,IdResponsable")] Structure structure)
        {
            var str = db.Structures.Find(Session["IdStructure"]);
            var banqueId = str.BanqueId(db);
            str = null;

            if (ModelState.IsValid)
            {
                db.Structures.Add(structure);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            int min = 0;
            try
            {
                min=Convert.ToInt32(Session["userSIteMinNiveau"]);
            }
            catch (Exception)
            {}
            List<CompteBanqueCommerciale> users = VariablGlobales.GetUsersByBanque(banqueId, db, min, Session["role"].ToString());
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
            int banqueId = structure.BanqueId(db);
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
                {}
            }
            ViewBag.IdResponsable = new SelectList(users, "Id", "NomComplet", structure.IdResponsable);
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
        public async Task<ActionResult> AddUser(FormCollection form)
        {
            var id = form["StructureId"];
            Structure strect =null;
            try
            {
                strect = db.Structures.Find(Convert.ToInt32(id));
            }
            catch (Exception)
            { }           
            CompteBanqueCommerciale user = null;
            if (form.Keys.Count > 0)
                foreach (var k in form.Keys)
                {
                    try
                    {
                        user = db.Users.Find(form[k.ToString()]) as CompteBanqueCommerciale;
                        user.IdStructure = int.Parse(id);
                        if (strect.NiveauDossier == 4)
                            user.EstBackOff = true;
                        try
                        {
                            Fonctions.Histiriser(db, new Historisation()
                            {
                                DateDebut = DateTime.Now,
                                TypeHistorique = 0,
                                Cible = "agent_structure" + user.Id+"_"+user.IdStructure,
                                IdAgant = user.Id,
                                Agent = user.NomComplet,
                                IdStructure=user.IdStructure,
                                Structure=strect.Nom
                            });
                        }
                        catch (Exception)
                        { }
                    }
                    catch (Exception ee)
                    { }
                }
            try
            {
                await db.SaveChangesAsync();
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
        public async Task<ActionResult> Edit([Bind(Include = "SuitChat,NiveauMaxManager,NiveauMaxDossier,Id,LireTouteReference,Nom,Adresse,NiveauMaxDossier,Ville,Pays,Telephone,Telephone2,NiveauDossier,IdTypeStructure,EstAgence,IdResponsable")] Structure structure)
        {
            var strc =db.Structures.Find(Session["IdStructure"]);
            var banqueId = strc.BanqueId(db);

            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(structure).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {}
            }
            int min = 0;
            try
            {
                min = Convert.ToInt32(Session["userSIteMinNiveau"]);
            }
            catch (Exception)
            { }
            List<CompteBanqueCommerciale> users = VariablGlobales.GetUsersByBanque(banqueId, db, min, Session["role"].ToString());
            ViewBag.IdResponsable = new SelectList(users, "Id", "Nom", structure.IdResponsable);
            ViewBag.IdTypeStructure = new SelectList(db.GetTypeStructures, "Id", "Intitule", structure.IdTypeStructure);
            return View(structure);
        }

        [ActionName("sup-util")]
        public ActionResult SupprimeUtilisateur(int id_struc,string id_util)
        {
            try
            {
                var user = db.GetCompteBanqueCommerciales.Find(id_util);
                user.IdStructure = null;
                db.SaveChanges();
            }
            catch (Exception)
            {}
            return RedirectToAction("Edit", new { id = id_struc });
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
