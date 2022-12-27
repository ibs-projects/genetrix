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

namespace genetrix.Controllers
{
    public class CompteBanqueCommercialesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CompteBanqueCommerciales
        public ActionResult Index(string coll="")
        {
            var role = db.XtraRoles.Find(Session["IdXRole"]);
            var site = db.Structures.Find(Session["IdStructure"]);
            var banqueID = (site.BanqueId(db));

            if (role == null)
                return RedirectToAction("Login", "auth");
            bool estCollaborateurs = false;
            var userId = (string)Session["userId"];
            if (coll == "1") estCollaborateurs = true;

            List<CompteBanqueCommerciale> users = new List<CompteBanqueCommerciale>();
            if (!Convert.ToBoolean(Session["EstAdmin"]))
            {
                db.GetCompteBanqueCommerciales.Include(c => c.Structure).Include(c => c.XRole).ToList().ForEach(c =>
                    {
                        if (!site.VoirUsersAutres)//permission structure
                        {
                            if (c.IdStructure == site.Id)
                            {
                                if(userId==site.IdResponsable)
                                    users.Add(c);
                                else if (!role.VoirDossiersAutres && c.GetGestionnaireId(site.Id) == userId)//permission de role
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
                    try
                    {
                        if (u.Structure.BanqueId(db) != banqueID) continue;
                        users.Add(u);
                    }
                    catch (Exception)
                    {}
                }

            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Liste utilisateurs";
            return View(users);
        }

        public new ActionResult Profile()
        {
            return View();
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
            try
            {
                ViewBag.Historisations = db.GetHistorisations.Where(h => h.IdAgant == compteBanqueCommerciale.Id).ToList();
            }
            catch (Exception)
            { }
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
            model.Sites.AddRange(VariablGlobales.GetStructureByBanque(banque.BanqueId(db),db));
            model.Password = Fonctions.RandomPassword(6);
            model.ConfirmPassword = Fonctions.RandomPassword(6);
            return View("~/Views/ApplicationUsers/_adduser.cshtml", model);
        }

        // POST: CompteBanqueCommerciales/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "EstBackOff,EstGestionnaire,EstAdmin,Nom,NomUtilisateur,Prenom,Sexe,Tel2,Tel1,RoleId,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,Categorie,Structure,IdXRole,IdSite")] CompteBanqueCommerciale compteBanqueCommerciale)
        {
            if (ModelState.IsValid)
            {
                var structure = db.Structures.Find(Session["IdStructure"]);
                var banqueId = structure.BanqueId(db);
                if(compteBanqueCommerciale.IdStructure==null || compteBanqueCommerciale.IdStructure==0)
                    compteBanqueCommerciale.IdStructure = banqueId;
                structure = null;
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
            CompteBanqueCommerciale compteBanqueCommerciale = null;
            try
            {
               compteBanqueCommerciale = await db.GetCompteBanqueCommerciales.FindAsync(id);
            }
            catch (Exception)
            {}            
            if (compteBanqueCommerciale == null)
            {
                return HttpNotFound();
            }
            //ViewBag.RoleId = new SelectList(db.XtraRoles, "RoleId", "Nom", compteBanqueCommerciale.RoleId);
            ViewBag.IdBanque = new SelectList(db.GetBanques, "Id", "Nom", compteBanqueCommerciale.Structure.BanqueId(db));
            ViewBag.IdStructure = new SelectList(db.Structures, "Id", "Nom", compteBanqueCommerciale.IdStructure);
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
        public async Task<ActionResult> Edit([Bind(Include = "EstBackOff,Sexe,EstGestionnaire,Id,EstAdmin,Nom,NomUtilisateur,Prenom,Sexe,Tel2,Tel1,Email,Structure,IdXRole,IdSite")] CompteBanqueCommerciale compteBanqueCommerciale)
        {
            if (ModelState.IsValid && false)
            {
                bool modifPwd = false;
                if (!string.IsNullOrEmpty(compteBanqueCommerciale.PassWordTmp))
                {
                    compteBanqueCommerciale.PasswordHash = compteBanqueCommerciale.PassWordTmp;
                    modifPwd = true;
                }
                compteBanqueCommerciale.UserName = compteBanqueCommerciale.Email;
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
            ViewBag.IdStructure = new SelectList(db.Structures, "Id", "Nom", compteBanqueCommerciale.IdStructure);
            ViewBag.IdXRole = new SelectList(db.XtraRoles, "RoleId", "Nom", compteBanqueCommerciale.IdXRole);
            return View(compteBanqueCommerciale);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edition(CompteBanqueCommerciale _cb)
        {
            var cb = db.GetCompteBanqueCommerciales.Find(_cb.Id);
            try
            {
                cb.Nom = _cb.Nom;
                cb.EstAdmin = _cb.EstAdmin;
                cb.Categorie = _cb.Categorie;
                cb.SuitChat = _cb.SuitChat;
                cb.Email = _cb.Email;
                cb.EstGestionnaire = _cb.EstGestionnaire;
                cb.IdStructure = _cb.IdStructure;
                cb.NomUtilisateur = _cb.NomUtilisateur;
                cb.PhoneNumber = _cb.PhoneNumber;
                cb.Prenom = _cb.Prenom;
                cb.Sexe = _cb.Sexe;
                cb.Tel1 = _cb.Tel1;
                cb.Tel2 = _cb.Tel2;
                cb.EstBackOff = _cb.EstBackOff;
             
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ee)
            { }

            ViewBag.IdBanque = new SelectList(db.GetBanques, "Id", "Nom", _cb.Structure.BanqueId(db));
            ViewBag.IdStructure = new SelectList(db.Structures, "Id", "Nom", _cb.IdStructure);
            ViewBag.IdXRole = new SelectList(db.XtraRoles, "RoleId", "Nom", _cb.IdXRole);
            return View(_cb);
        }

        public JsonResult GetAgents()
        {
            var strucId = Convert.ToInt32(Session["IdStructure"]);
            try
            {
                var agents = (from c in db.GetCompteBanqueCommerciales
                              where c.IdStructure == strucId
                              select new { Nom =c.Prenom+" "+ c.Nom, Id = c.Id }).ToList();
                return Json(agents, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ee)
            {}
            return Json(new { },JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetByStructure(int strucId)
        {
            
            try
            {
                var agents = (from c in db.GetCompteBanqueCommerciales
                              where c.IdStructure == strucId
                              select new { Nom =c.Prenom+" "+ c.Nom, Id = c.Id }).ToList();
                return Json(agents, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ee)
            {}
            return Json(new { },JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> Attribute(int doss,string agent)
        {
            var dossier =await db.GetDossiers.FindAsync(doss);
            var _agent = db.GetCompteBanqueCommerciales.Find(agent);
            var idStructure = Convert.ToInt32(Session["IdStructure"]);
            var structure = db.Structures.Find(idStructure);
            var idbanque = structure.BanqueId(db);
            //creation dossier-structure
            var ds =await db.GetDossierStructures.FirstOrDefaultAsync(d => d.IdDossier == dossier.Dossier_Id && d.IdStructure == idStructure);
            if (ds==null)
            {
                var doss_struct = new DossierStructure()
                {
                    IdDossier = dossier.Dossier_Id,
                    IdStructure = idStructure,
                    AttribuerPar = (string)Session["userName"],
                    Date = DateTime.Now,
                    IdResponsable = _agent.Id,
                    NomResponsable = _agent.NomComplet
                };
                db.GetDossierStructures.Add(doss_struct);
            }
            else
            {
                ds.AttribuerPar = (string)Session["userName"];
                ds.IdResponsable = _agent.Id;
                ds.NomResponsable = _agent.NomComplet;
                ds.Date = DateTime.Now;
            }

            //Envoyer d'un mail de notification aux responsables
            //// Le gestionnaire principal
            ///
            var chef = (string)Session["userName"];
            MailFunctions.SendMail(new MailModel()
            {
                To = dossier.GestionnaireEmail,
                Subject = "Réaffectation du dossier " + dossier.GetClient + $" ({dossier.MontantStringDevise} de {dossier.GetFournisseur})",
                Body = $"Le dossier ({dossier.MontantStringDevise}, fournisseur {dossier.GetFournisseur}) du client {dossier.GetClient}  a été réaffecté à M(me) {_agent.NomComplet}.<br/>Pour plus d'information, veuillez contacter votre chef d'agence M(me) {chef}"
            },db,persiste:true);
            //// Gestionnaire interinaire
            MailFunctions.SendMail(new MailModel()
            {
                To = _agent.Email,
                Subject = "Réaffectation du dossier " + dossier.GetClient + $" ({dossier.MontantStringDevise} de {dossier.GetFournisseur})",
                Body = $"Le dossier ({dossier.MontantStringDevise}, fournisseur {dossier.GetFournisseur}) du client {dossier.GetClient} vous a été affecté ce jour.<br/>Pour plus d'information, veuillez contacter votre chef d'agence M(me) {chef}"
            }, db, persiste: true);

            //responsable du dossier
            dossier.IdAgentResponsableDossier = agent;

            //Responsable pour chaque site: 1 agence, 2 conformite, 3 service transfert
            switch (structure.NiveauDossier)
            {
                case 1:
                    dossier.IdResponsableAgence = agent;
                    dossier.AgentResponsableAgence = _agent;
                    break; 
                case 4:
                    dossier.IdResponsableBackOffice = agent;
                    dossier.AgentResponsableBackOffice = _agent;
                    break;
                case 6:
                   dossier.IdResponsableConformite = agent;
                   dossier.AgentResponsableConformite = _agent;
                    break;
                case 9:
                    dossier.IdResponsableTransfert = agent;
                    dossier.AgentResponsableTransfert = _agent;
                    break;
                default:
                    break;
            }
            try
            {
                Fonctions.Histiriser(db, new Historisation()
                {
                    DateDebut = DateTime.Now,
                    TypeHistorique = 2,
                    Cible = "dossier" + dossier.Dossier_Id,
                    Dossier = dossier.MontantStringDevise + " - fournisseur: " + dossier.GetFournisseur,
                    IdAgant = agent,
                    Agent = _agent.NomComplet,
                    Client = dossier.GetClient,
                    IdClient = dossier.ClientId
                });
            }
            catch (Exception)
            {}
           await db.SaveChangesAsync();
            return Json(_agent.NomComplet, JsonRequestBehavior.AllowGet);
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
