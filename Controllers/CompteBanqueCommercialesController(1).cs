using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using e_apurement.Models;
using eApurement.Models;

namespace eApurement.Controllers
{
    public class CompteBanqueCommercialesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CompteBanqueCommerciales
        public ActionResult Index()
        {
            var banqueID = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);
            var role = db.XtraRoles.Find((Session["user"] as CompteBanqueCommerciale).IdXRole);
            var site = db.Structures.Find((Session["user"] as CompteBanqueCommerciale).IdStructure);
            if (role == null)
                return RedirectToAction("Login", "auth");

            List<CompteBanqueCommerciale> users = new List<CompteBanqueCommerciale>();
            if (!(Session["user"] as CompteBanqueCommerciale).EstAdmin)
            {
                db.GetCompteBanqueCommerciales.Include(c => c.Structure).Include(c => c.XRole).ToList().ForEach(c =>
                    {
                        if (!site.VoirUsersAutres)//permission structure
                        {
                            if (c.IdStructure == site.Id)
                            {
                                if (!role.VoirDossiersAutres && c.GetGestionnaireId(site.Id) == (Session["user"] as CompteBanqueCommerciale).Id)//permission de role
                                {
                                    users.Add(c);
                                }
                            }
                        }
                        else
                        {
                            users.Add(c);
                        }
                    });

            }
            else
                foreach (var u in db.GetCompteBanqueCommerciales.Include(b=>b.Structure).ToList())
                {
                    if (u.Structure.BanqueId(db) != banqueID) continue;
                    users.Add(u);
                }

            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Liste utilisateurs";
            return View(users);
        }

        // GET: CompteBanqueCommerciales/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompteBanqueCommerciale compteBanqueCommerciale = await db.GetCompteBanqueCommerciales.FindAsync(id);
            if (compteBanqueCommerciale == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Details utilisateur";
            return View(compteBanqueCommerciale);
        }

        // GET: CompteBanqueCommerciales/Create
        public ActionResult Create()
        {
            //ViewBag.RoleId = new SelectList(db.XtraRoles, "RoleId", "Nom");
            //ViewBag.IdBanque = new SelectList(db.GetBanques, "Id", "Nom");
            //ViewBag.IdSite = new SelectList(db.Sites, "Id", "Nom");
            //ViewBag.IdXRole = new SelectList(db.XtraRoles, "RoleId", "Nom");
            //ViewBag.navigation = "param";
            //ViewBag.navigation_msg = "Creation utilisateur";
            //return RedirectToAction("Create","ApplicationUsers");

            ViewBag.RoleId = new SelectList(db.XtraRoles, "RoleId", "Nom");
            ViewBag.Agences = new SelectList(db.Structures, "Id", "Nom");

            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Creation utilisateur ";

            var banque = db.GetBanques.Find((Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db));
            var model = new RegisterViewModel()
            {
                NomEntreprise = banque.Nom,
            };
            model.Roles = new List<XtraRole>();
            model.Sites = new List<Structure>();

            model.Roles.AddRange(db.XtraRoles);
            model.Sites.AddRange(VariablGlobales.GetStructureByBanque(banque.BanqueId(db),db));

            return View("~/Views/ApplicationUsers/_adduser.cshtml", model);
        }

        // POST: CompteBanqueCommerciales/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "EstGestionnaire,EstAdmin,Nom,NomUtilisateur,Prenom,Sexe,Tel2,Tel1,RoleId,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,Categorie,Structure,IdXRole,IdSite")] CompteBanqueCommerciale compteBanqueCommerciale)
        {
            if (ModelState.IsValid)
            {
                ///compteBanqueCommerciale.IdBanque = (Session["user"] as CompteBanqueCommerciale).IdBanque;
                db.Users.Add(compteBanqueCommerciale);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            //ViewBag.RoleId = new SelectList(db.XtraRoles, "RoleId", "Nom", compteBanqueCommerciale.IdXRole);
            ViewBag.IdBanque = new SelectList(db.GetBanques, "Id", "Nom", compteBanqueCommerciale.Structure.BanqueId(db));
            ViewBag.IdSite = new SelectList(db.Agences, "Id", "Nom", compteBanqueCommerciale.IdStructure);
            ViewBag.IdXRole = new SelectList(db.XtraRoles, "RoleId", "Nom", compteBanqueCommerciale.IdXRole);
            return View(compteBanqueCommerciale);
        }

        // GET: CompteBanqueCommerciales/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompteBanqueCommerciale compteBanqueCommerciale = await db.GetCompteBanqueCommerciales.FindAsync(id);
            if (compteBanqueCommerciale == null)
            {
                return HttpNotFound();
            }
            //ViewBag.RoleId = new SelectList(db.XtraRoles, "RoleId", "Nom", compteBanqueCommerciale.RoleId);
            ViewBag.IdBanque = new SelectList(db.GetBanques, "Id", "Nom", compteBanqueCommerciale.Structure.BanqueId(db));
            ViewBag.IdSite = new SelectList(db.Agences, "Id", "Nom", compteBanqueCommerciale.IdStructure);
            ViewBag.IdXRole = new SelectList(db.XtraRoles, "RoleId", "Nom", compteBanqueCommerciale.IdXRole);

            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Edition utilisateur";
            return View(compteBanqueCommerciale);
        }

        // POST: CompteBanqueCommerciales/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "EstGestionnaire,Id,EstAdmin,Nom,NomUtilisateur,Prenom,Sexe,Tel2,Tel1,Email,Structure,IdXRole,IdSite")] CompteBanqueCommerciale compteBanqueCommerciale)
        {
            if (ModelState.IsValid)
            {
                bool modifPwd = false;
                if (!string.IsNullOrEmpty(compteBanqueCommerciale.PassWordTmp))
                {
                    compteBanqueCommerciale.PasswordHash = compteBanqueCommerciale.PassWordTmp;
                    modifPwd = true;
                }
                compteBanqueCommerciale.UserName = compteBanqueCommerciale.Email;
                //compteBanqueCommerciale.IdBanque = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId;
                db.Entry(compteBanqueCommerciale).State = EntityState.Modified;
                try
                {
                    await db.SaveChangesAsync();
                    if (modifPwd) { }
                }
                catch (Exception ee)
                {}
                return RedirectToAction("Index");
            }
            //ViewBag.RoleId = new SelectList(db.XtraRoles, "RoleId", "Nom", compteBanqueCommerciale.RoleId);
            ViewBag.IdBanque = new SelectList(db.GetBanques, "Id", "Nom", compteBanqueCommerciale.Structure.BanqueId(db));
            ViewBag.IdSite = new SelectList(db.Agences, "Id", "Nom", compteBanqueCommerciale.IdStructure);
            ViewBag.IdXRole = new SelectList(db.XtraRoles, "RoleId", "Nom", compteBanqueCommerciale.IdXRole);
            return View(compteBanqueCommerciale);
        }

        // GET: CompteBanqueCommerciales/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompteBanqueCommerciale compteBanqueCommerciale = await db.GetCompteBanqueCommerciales.FindAsync(id);
            if (compteBanqueCommerciale == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Suppression utilisateur";
            return View(compteBanqueCommerciale);
        }

        // POST: CompteBanqueCommerciales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            CompteBanqueCommerciale compteBanqueCommerciale = await db.GetCompteBanqueCommerciales.FindAsync(id);
            db.Users.Remove(compteBanqueCommerciale);
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
