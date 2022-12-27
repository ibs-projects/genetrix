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

namespace eApurement.Controllers
{
    public class XtraRolesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: XtraRoles
        public async Task<ActionResult> Index()
        {
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Liste roles";
            return View(await db.XtraRoles.ToListAsync());
        }

        // GET: XtraRoles/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            XtraRole xtraRole = await db.XtraRoles.FindAsync(id);
            if (xtraRole == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Details role";
            return View(xtraRole);
        }

        // GET: XtraRoles/Create
        public ActionResult Create()
        {
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Creation role";
            return View();
        }

        // POST: XtraRoles/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Nom,NiveauDossier,IdStructure,NiveauDossier,VoirDossiersAutres,VoirUsersAutres,VoirClientAutres")] XtraRole xtraRole)
        {
            if (ModelState.IsValid)
            {
                db.XtraRoles.Add(xtraRole);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(xtraRole);
        }

        // GET: XtraRoles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            XtraRole xtraRole = db.XtraRoles.FirstOrDefault(x=>x.RoleId==id);
            if (xtraRole == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Edition role";
            var ddd = db.GetIHMs.Where(i=>i.XRoleId==id).ToList();
            var idComp = (from d in ddd select d.ComposantId+"_"+d.XRoleId).ToList();
            if (idComp == null) idComp = new List<string>();

            List<Composant> composants = new List<Composant>();
            foreach (var item in db.GetComposants.ToList())
            {
                if (idComp.Contains(item.Id+"_"+id)) continue;
                    composants.Add(item);
            }
            ViewBag.ihms = ddd;
            ddd=null;
            ViewBag.Composants = composants;
            var banqueId = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);
            List<CompteBanqueCommerciale> _users = new List<CompteBanqueCommerciale>();
            var dd= VariablGlobales.GetUsersByBanque(banqueId, db); 

            foreach (var item in dd.ToList())
            {
                try
                {
                    if (xtraRole.Users.FirstOrDefault(u => u.Id == item.Id) == null)
                        _users.Add(item);
                }
                catch (Exception)
                {
                    _users.Add(item);
                }
            }
            dd = null;
            ViewBag.users = _users;

            List<Entitee> _entitees = new List<Entitee>();
            foreach (var item in db.GetEntitees.ToList())
            {
                if (xtraRole.GetEntitee_Roles.FirstOrDefault(u => u.IdEntitee == item.Id) == null)
                    _entitees.Add(item);
            }

            ViewBag.Entitee=_entitees;
            ViewBag.IdStructure = new SelectList(db.Structures, "Id", "Nom");

            _entitees = null;
            _users = null;
            composants = null;
            return View(xtraRole);
        }

        // POST: XtraRoles/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "RoleId,Nom,NiveauDossier,IdStructure,NiveauDossier,VoirDossiersAutres,VoirUsersAutres,VoirClientAutres")] XtraRole xtraRole)
        {
            if (ModelState.IsValid)
            {
                db.Entry(xtraRole).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(xtraRole);
        }

        [HttpPost]
        public ActionResult AddComposant(FormCollection form)
        {
            var id = form["RoleId"];
            if (form.Keys.Count > 0)
                foreach (var k in form.Keys)
                {
                    try
                    {
                        if (k.ToString() != "RoleId")
                        {
                            db.GetIHMs.Add(new IHM()
                            {
                                ComposantId = int.Parse(form[k.ToString()]),
                                XRoleId = int.Parse(id),
                                Lire=true
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
           return RedirectToAction("Edit",new { id=id});
        }

        [HttpPost]
        public ActionResult AddUser(FormCollection form)
        {
            var id = form["RoleId"];
            CompteBanqueCommerciale user = null;
            if (form.Keys.Count > 0)
                foreach (var k in form.Keys)
                {
                    try
                    {
                       user= db.Users.Find(form[k.ToString()]) as CompteBanqueCommerciale;
                       user.IdXRole = int.Parse(id);
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
        public ActionResult AddEntitee(FormCollection form)
        {
            var id = form["RoleId"];
            if (form.Keys.Count > 0)
                foreach (var k in form.Keys)
                {
                    try
                    {
                        if (k.ToString() != "RoleId")
                        {
                            db.GetEntitee_Roles.Add(new Entitee_Role()
                            {
                                IdEntitee = int.Parse(form[k.ToString()]),
                                IdXRole = int.Parse(id),
                                Lire=true,
                                Ecrire=true
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


        // GET: XtraRoles/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            XtraRole xtraRole = await db.XtraRoles.FindAsync(id);
            if (xtraRole == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Suppression role";
            return View(xtraRole);
        }

        [ActionName("edit-ihm")]
        public async Task<ActionResult> Edit_Ihm(string id)
        {
            IHM ihm = await db.GetIHMs.FindAsync(id);
            db.GetIHMs.Remove(ihm);
            await db.SaveChangesAsync();
            return RedirectToAction("Edit", "IHMs", new { id=id.Split('_')[1] });
        }
        
        [ActionName("del-ihm")]
        public async Task<ActionResult> Delete_Ihm(string id)
        {
            try
            {
                int comId = int.Parse(id.Split('_')[0]);
                int rolId = int.Parse(id.Split('_')[1]);
                IHM ihm = db.GetIHMs.FirstOrDefault(i => i.ComposantId == comId && i.XRoleId == rolId);
                db.GetIHMs.Remove(ihm);
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {}
            return RedirectToAction("Edit", "XtraRoles", new { id=id.Split('_')[1] });
        }

        // POST: XtraRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                XtraRole xtraRole = await db.XtraRoles.FindAsync(id);
                db.XtraRoles.Remove(xtraRole);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
            
            }
            return RedirectToAction("Delete", new { id = id });
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
