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
using System.Web.Helpers;
using genetrix.Models;

namespace genetrix.Controllers
{
    public class ApplicationUsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ApplicationUsers
        public async Task<ActionResult> Index()
        {
            IQueryable<ApplicationUser> applicationUsers = null;

            if ((string)Session["userType"] == "CompteBanqueCommerciale")
            {
                var structure = db.Structures.Find(Session["IdStructure"]);
                var idb = structure.BanqueId(db);
                structure = null;
                applicationUsers = db.Users.Where(u => u
                is CompteBanqueCommerciale
                && (u as CompteBanqueCommerciale).Structure.BanqueId(db) == idb);//.Include(a => a.Role);
            }
            else if ((string)Session["userType"] == "CompteClient")
            {
                var idc = Convert.ToInt32(Session["clientId"]);
                applicationUsers = db.Users.Where(u => u
                is CompteBanqueCommerciale
                && (u as CompteBanqueCommerciale).Structure.BanqueId(db) == idc);//.Include(a => a.Role);
            }
            else if ((string)Session["userType"] == "CompteAdmin")
                applicationUsers = db.Users;//.Include(a => a.Role);

            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Liste utilisateurs";
            return View(await applicationUsers.ToListAsync());
        }

        // GET: ApplicationUsers/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Details utilisateur " + applicationUser.NomUtilisateur;
            return View(applicationUser);
        }

        // GET: ApplicationUsers/Create
        public ActionResult Create()
        {
            ViewBag.RoleId = new SelectList(db.XtraRoles, "RoleId", "Nom");
            ViewBag.Agences = new SelectList(db.Agences, "Id", "Nom");

            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Creation utilisateur ";

            var structure = db.Structures.Find(Session["IdStructure"]);
            var banqueId = structure.BanqueId(db);
            structure = null;
            var banque = db.GetBanques.Find(banqueId);
            var model = new RegisterViewModel()
            {
                NomEntreprise = banque.Nom,
            };
            model.Roles = new List<XtraRole>();
            model.Sites = new List<Structure>();

            model.Roles.AddRange(db.XtraRoles);
            model.Sites.AddRange(db.Agences.Where(a=>a.BanqueId(db) == banque.Id));

            return View("_adduser", model);
        }

        // POST: ApplicationUsers/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Nom,Prenom,Sexe,Tel2,Tel1,RoleId,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,NomUtilisateur")] CompteBanqueCommerciale applicationUser)
        {
            if (ModelState.IsValid)
            {
                applicationUser.UserName = applicationUser.Email;
                db.Users.Add(applicationUser);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.RoleId = new SelectList(db.XtraRoles, "RoleId", "Nom", applicationUser.IdXRole);
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Création utilisateur";
            return View(applicationUser);
        }

        // GET: ApplicationUsers/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompteBanqueCommerciale applicationUser = db.GetCompteBanqueCommerciales.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            ViewBag.RoleId = new SelectList(db.XtraRoles, "RoleId", "Nom", applicationUser.IdXRole);
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Edition utilisateur " + applicationUser.NomUtilisateur;
            return View(applicationUser);
        }

        // POST: ApplicationUsers/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,NomUtilisateur,Nom,Prenom,Sexe,Tel2,Tel1,RoleId,Email,EmailConfirmed,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,PassWordTmp")] CompteBanqueCommerciale applicationUser)
        {
            if (ModelState.IsValid) 
            { 
                if (!string.IsNullOrEmpty(applicationUser.PassWordTmp))
                applicationUser.PasswordHash = Crypto.HashPassword(applicationUser.PassWordTmp);
                applicationUser.UserName = applicationUser.Email;
                db.Entry(applicationUser).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.RoleId = new SelectList(db.XtraRoles, "RoleId", "Nom", applicationUser.IdXRole);
            return View(applicationUser);
        }

        // GET: ApplicationUsers/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Suppression utilisateur " + applicationUser.NomComplet;
            return View(applicationUser);
        }

        // POST: ApplicationUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            
            try
            {
                ApplicationUser applicationUser = db.Users.Find(id);
                (applicationUser as CompteBanqueCommerciale).IdXRole = null;
                (applicationUser as CompteBanqueCommerciale).IdStructure = null;
                //(applicationUser as CompteBanqueCommerciale).IdBanque = 0;
                //db.SaveChanges();
                db.GetCompteBanqueCommerciales.Remove((applicationUser as CompteBanqueCommerciale));

                db.SaveChanges();
            }
            catch (Exception ee)
            {}
            return RedirectToAction("Index");
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (Session != null)
            {
                if ((string)Session["userType"] == "CompteBanqueCommerciale")
                {
                    var url = (string)Session["urlaccueil"];
                    filterContext.Result = Redirect(url);
                }

                filterContext.Result = RedirectToAction("Index", "Index");
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
