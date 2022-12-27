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
    public class CompteClientsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CompteClients
        public async Task<ActionResult> Index()
        {
            var clientId = Convert.ToInt32(Session["clientId"]);
            var GetCompteClients = db.GetCompteClients.Include(c => c.Client).Where(c=>c.ClientId==clientId);
            var user = db.GetCompteClients.Find(Session["userId"]);
            var client = db.GetClients.Find(user.ClientId);
            try
            {
                if (client.ModeRestraint)
                {
                    ViewBag.CreerUser = user.CreerUser || user.EstAdmin;
                    ViewBag.ModifUser = user.ModifUser || user.EstAdmin;
                    ViewBag.SuppUser = user.SuppUser || user.EstAdmin; 
                }
            }
            catch (Exception)
            { }
            ViewBag.ModeRestraint = client.ModeRestraint;
            //user.EstAdmin = true;
            //db.SaveChanges();
            user = null;
            client = null;
            return View(await GetCompteClients.ToListAsync());
        }

        // GET: CompteClients/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompteClient compteClient = await db.GetCompteClients.FindAsync(id);
            if (compteClient == null)
            {
                return HttpNotFound();
            }
            return View(compteClient);
        }

        // GET: CompteClients/Create
        public ActionResult Create()
        {
            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom");
            return View();
        }

        // POST: CompteClients/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "EstAdmin,Nom,NomUtilisateur,Prenom,Sexe,Tel2,Tel1,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,ClientId")] CompteClient compteClient)
        {
            if (ModelState.IsValid)
            {
                compteClient.ClientId= Convert.ToInt32(Session["clientId"]); 
                db.GetCompteClients.Add(compteClient);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom", compteClient.ClientId);
            return View(compteClient);
        }

        // GET: CompteClients/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Restriction
            var user = db.GetCompteClients.Find(Session["userId"]);
            var client = db.GetClients.Find(user.ClientId);
            try
            {
                if (!user.ModifUser && !user.EstAdmin && client.ModeRestraint)
                    return RedirectToAction("Index", "Index", new { msg = "Vous ne pouvez pas éditer un utilisateur, Veuillez contacter votre administrateur" });
            }
            catch (Exception)
            { }
            CompteClient compteClient = await db.GetCompteClients.FindAsync(id);
            if (compteClient == null)
            {
                return HttpNotFound();
            }
            //ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom", compteClient.ClientId);
            ViewBag.IdUserRole = new SelectList(db.ClientUserRoles, "Id", "Nom");
            ViewBag.ModeRestrait = client.ModeRestraint;
            return View(compteClient);
        }

        // POST: CompteClients/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CompteClient compteClient)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var model = db.GetCompteClients.Find(compteClient.Id);
                    var userRole = db.ClientUserRoles.Find(model.IdUserRole);
                    compteClient.ClientId = Convert.ToInt32(Session["clientId"]);
                    model.ClientId = compteClient.ClientId;
                    model.Email = compteClient.Email;
                    model.EstAdmin = compteClient.EstAdmin;
                    model.Nom = compteClient.Nom;
                    model.NomUtilisateur = compteClient.NomUtilisateur;
                    model.Prenom = compteClient.Prenom;
                    model.Sexe = compteClient.Sexe;
                    model.Tel2 = compteClient.Tel2;
                    model.Tel1 = compteClient.Tel1;
                    model.Email = compteClient.Email;
                    model.IdUserRole = compteClient.IdUserRole;
                    model.UserRole = userRole;
                    var client = db.GetClients.Find(Session["clientId"]);
                    if(client.ModeRestraint)
                        model.SetHabilitations();
                    //model.CreerDossier = compteClient.CreerDossier;
                    //model.SoumettreDossier = compteClient.SoumettreDossier;
                    //model.CreerUser = compteClient.CreerUser;
                    //model.SuppUser = compteClient.SuppUser;
                    //model.ModifUser = compteClient.ModifUser;
                    //model.CreerBenef = compteClient.CreerBenef;
                    //model.SuppBenef = compteClient.SuppBenef;
                    //model.ModifBenef = compteClient.ModifBenef;

                    db.Entry(model).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                catch (Exception ee)
                { }
                return RedirectToAction("Index");
            }
            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom", compteClient.ClientId);
            return View(compteClient);
        }

        // GET: CompteClients/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Restriction
            var user = db.GetCompteClients.Find(Session["userId"]);
            var client = db.GetClients.Find(user.ClientId);
            try
            {
                if (!user.SuppUser && client.ModeRestraint)
                    return RedirectToAction("Index", "Index", new { msg = "Vous ne pouvez pas supprimer un utilisateur, Veuillez contacter votre administrateur" });
            }
            catch (Exception)
            { }
            user = null;
            client = null;
            CompteClient compteClient = await db.GetCompteClients.FindAsync(id);
            if (compteClient == null)
            {
                return HttpNotFound();
            }
            return View(compteClient);
        }

        // POST: CompteClients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            CompteClient compteClient = await db.GetCompteClients.FindAsync(id);
            db.GetCompteClients.Remove(compteClient);
            await db.SaveChangesAsync();
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
