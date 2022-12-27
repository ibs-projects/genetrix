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
using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using genetrix.Models;
using genetrix.Models.Fonctions;

namespace genetrix.Controllers
{
    public class BanqueClientsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BanqueClients1
        public ActionResult Index()
        {
            IEnumerable<BanqueClient> getBanqueClients = new List<BanqueClient>();
            try
            {
                var clientId = (db.Users.Find(User.Identity.GetUserId()) as CompteClient).ClientId;
                getBanqueClients = db.GetBanqueClients.Include(b => b.Site).Include(b => b.Site.DirectionMetier).Include(b => b.Client).Where(b => b.ClientId == clientId).ToList();
            }
            catch (Exception)
            {}
            return View(getBanqueClients.ToList());
        }

        // GET: BanqueClients1/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BanqueClient banqueClient = await db.GetBanqueClients.FindAsync(id);
            if (banqueClient == null)
            {
                return HttpNotFound();
            }
            return View(banqueClient);
        }

        // GET: BanqueClients1/Create
        public ActionResult Create()
        {
            ViewBag.BanqueId = new SelectList(db.GetBanques, "Id", "Nom");
            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom");
            return View();
        }

        // POST: BanqueClients1/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IdGestionnaire,ClientId,BanqueId,IdSite")] BanqueClient banqueClient)
        {
            if (ModelState.IsValid)
            {
                banqueClient.Client =(db.Users.Find(User.Identity.GetUserId()) as CompteClient).Client;
                db.GetBanqueClients.Add(banqueClient);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.BanqueId = new SelectList(db.Agences, "Id", "Nom", banqueClient.IdSite);
            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom", banqueClient.ClientId);
            return View(banqueClient);
        }

        // GET: BanqueClients1/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BanqueClient banqueClient = await db.GetBanqueClients.FindAsync(id);
            if (banqueClient == null)
            {
                return HttpNotFound();
            }
            ViewBag.BanqueId = new SelectList(db.GetBanques, "Id", "Nom", banqueClient.IdSite);
            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom", banqueClient.ClientId);
            return View(banqueClient);
        }

        // POST: BanqueClients1/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ClientId,IdSite,IdGestionnaire")] BanqueClient banqueClient)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(banqueClient).State = EntityState.Modified;
                    try
                    {
                        var agent = db.GetCompteBanqueCommerciales.Find(banqueClient.IdGestionnaire);
                        var client = db.GetClients.Find(banqueClient.ClientId);
                        Fonctions.Histiriser(db, new Historisation()
                        {
                            DateDebut = DateTime.Now,
                            TypeHistorique = 1,
                            Cible = "agent_client" + agent.Id + "" +banqueClient.ClientId,
                            IdAgant = agent.Id,
                            Agent = agent.NomComplet,
                            Client = client.Nom,
                            IdClient = client.Id
                        });
                        agent = null;client = null;
                    }
                    catch (Exception)
                    { }
                    await db.SaveChangesAsync();
                }
                catch (Exception)
                {}
                return RedirectToAction("Index");
            }
            ViewBag.BanqueId = new SelectList(db.Agences, "Id", "Nom", banqueClient.IdSite);
            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom", banqueClient.ClientId);
            return View(banqueClient);
        }
        
        public JsonResult EditJS(int banqueclientId,int siteId,string gesId)
        {
            var bc = db.GetBanqueClients.Find(banqueclientId);
            bc.IdGestionnaire = gesId;
            bc.IdSite = siteId;
            db.SaveChanges();
            return Json("Modification réussie",JsonRequestBehavior.AllowGet);
        }

        // GET: BanqueClients1/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BanqueClient banqueClient = await db.GetBanqueClients.FindAsync(id);
            if (banqueClient == null)
            {
                return HttpNotFound();
            }
            return View(banqueClient);
        }

        // POST: BanqueClients1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            BanqueClient banqueClient = await db.GetBanqueClients.FindAsync(id);
            db.GetBanqueClients.Remove(banqueClient);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public JsonResult GetAgence(int idBanque)
        {
            List<Agence> agences = new List<Agence>();
            foreach (var s in db.Agences.ToList())
            {
                try
                {
                    if (s.BanqueId(db) == idBanque)// && s.EstAgence)
                        agences.Add(s);
                }
                catch (Exception)
                {}
            }

            var dd = from a in agences select new { Id = a.Id, Nom = a.Nom };
            agences = null;
            return Json(dd,JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult GetGestionnaire(int idAgence)
        {
            var dd = new List<CompteBanqueCommerciale>();
            foreach (var s in db.GetCompteBanqueCommerciales.Include(c=>c.XRole).ToList())
            {
                if (s.IdStructure == idAgence)// && (s.XRole.Nom=="Gestionnaire" || s.EstGestionnaire))
                {
                    dd.Add(s);
                }
            }
            
            var gestion = (from a in dd.ToList()
                          select new {a.Id,Nom=a.NomComplet }).ToList();

            return Json(gestion, JsonRequestBehavior.AllowGet);
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
