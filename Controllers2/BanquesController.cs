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
using System.IO;

namespace genetrix.Controllers
{
    public class BanquesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Banques1
        public ActionResult Index(string msg="")
        {
            var cc = db.GetBanques.Count();
            foreach (var item in db.GetBanques)
            {

            }
            ViewBag.msg = msg;
            return View( db.GetBanques.ToList());
        }
         public ActionResult Orga()
        {
            ViewBag.navigation = "param";
            ViewBag.navigation_msg = "Organigramme";
            var idStruct = Session["IdStructure"];
            var st = db.Structures.Find(idStruct);
            genetrix.Models.Banque banque = new genetrix.Models.Banque(); ;
            if (st != null)
            {
                try
                {
                    banque = db.GetBanques.Include(b => b.DirectionMetiers).FirstOrDefault(b => b.Id == st.BanqueId(db));

                }
                catch (Exception)
                {
                    var strId = st.BanqueId(db);
                    banque = db.GetBanques.FirstOrDefault(b => b.Id == strId);
                }
            }

            //var dd = db.Structures.GroupBy(s=>s.NiveauDossier).ToList();
            try
            {
                ViewData["structures"] = db.Structures.GroupBy(s => s.NiveauDossier).ToList();
            }
            catch (Exception)
            {}
            return View(banque);
        }

        // GET: Banques1/Details/5
        [ActionName("configurations")]
        public async Task<ActionResult> Details(int? id)
        {
            if (Session == null) return RedirectToAction("Connextion", "auth");

            if ((string)Session["userType"] == "CompteBanqueCommerciale")
            {
                var user = db.GetCompteBanqueCommerciales.Find((string)Session["userId"]);
                int strId = user.Structure.Id;
                var str = db.Structures.Find(strId);
                id = str.BanqueId(db);
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var banque = await db.GetBanques.FindAsync(id);
            if (banque == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdTypeStructure = new SelectList(db.GetTypeStructures, "Id", "Intitule");

            return View("Details",banque);
        }

        /// <summary>
        /// Selection opérateur de swift
        /// </summary>
        /// <returns></returns>
        public ActionResult SelOp()
        {
            var structure = db.Structures.Find(Session["IdStructure"]);
            var idbanque = structure.BanqueId(db);
            var ops = (from op in db.GetOperateurSwifts
                       where op.BanqueId == idbanque
                       select new { Nom = op.NomComplet, Id = op.Id }).ToList();

            return Json(ops, JsonRequestBehavior.AllowGet);
        }

        // GET: Banques1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Banques1/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Nom,Adresse,Telephone")] genetrix.Models.Banque banque)
        {
            if (ModelState.IsValid)
            {
                var _bexiste = db.GetBanques.FirstOrDefault(b => b.Nom.ToLower() == banque.Nom.ToLower());
                if (_bexiste!=null)
                {
                    ViewBag.msg = "La banque du même nom existe déjà!";
                    return RedirectToAction("Create", banque);
                }

                db.GetBanques.Add(banque);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(banque);
        }

        // GET: Banques1/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var banque = await db.GetBanques.FindAsync(id);
            if (banque == null)
            {
                return HttpNotFound();
            }
            return View(banque);
        }

        // POST: Banques1/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "MontantDFX,ConfigPersonnel,Id,Nom,Adresse,Telephone")] genetrix.Models.Banque model, HttpPostedFileBase Logo = null)
        {
            if (ModelState.IsValid)
            {
                genetrix.Models.Banque banque = db.GetBanques.Find(model.Id);
                banque.MontantDFX = model.MontantDFX;
                banque.ConfigPersonnel = model.ConfigPersonnel;
                banque.Nom = model.Nom;
                banque.Adresse = model.Adresse;
                banque.Telephone = model.Telephone;

                db.Entry(banque).State = EntityState.Modified;
                if (Logo != null)
                {
                    try
                    {
                        string chemin = Path.GetFileNameWithoutExtension(Logo.FileName);
                        string extension = Path.GetExtension(Logo.FileName);
                        chemin += extension;
                        var img = Path.Combine(CreateNewFolderDossier(banque.Id + ""), chemin);
                        if (!string.IsNullOrEmpty(Logo.FileName))
                        {
                            if (!string.IsNullOrEmpty(banque.Image))
                            {
                                //Supprime l'ancienne image
                                try
                                {
                                    System.IO.File.Delete(banque.Image);
                                }
                                catch (Exception e)
                                { }
                            }
                        }
                        banque.Image = "~/BanqueEspace/Ressources/"+banque.Id+"/"+chemin;
                        Logo.SaveAs(img);
                        chemin = null;
                        extension = null;
                    }
                    catch (Exception)
                    {}
                }
                await db.SaveChangesAsync();
                //return RedirectToAction("Index");
            }
            return View(model);
        }

        private string CreateNewFolderDossier(string banqueId)
        {
            try
            {
                string projectPath = "~/BanqueEspace/Ressources";
                string folderName = Path.Combine(Server.MapPath(projectPath), banqueId);
                System.IO.Directory.CreateDirectory(folderName);
                return folderName;
            }
            catch (Exception ee)
            { }
            return "";
        }

        public JsonResult GetAgence(int idBanque)
        {
            var agences = from a in db.Agences.Where(s => s.BanqueId(db) == idBanque && s.EstAgence).ToList()
                          select new { a.Id, a.Nom };

            return Json(agences, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGestionnaire(int idAgence)
        {
            var gestion = from a in db.GetCompteBanqueCommerciales.Where(s => s.IdStructure == idAgence && s.EstGestionnaire).ToList()
                          select new { a.Id, Nom = a.NomComplet };

            return Json(gestion, JsonRequestBehavior.AllowGet);
        }

        // GET: Banques1/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var banque = await db.GetBanques.FindAsync(id);
            if (banque == null)
            {
                return HttpNotFound();
            }
            return View(banque);
        }

        // POST: Banques1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var banque = await db.GetBanques.FindAsync(id);
            db.GetBanques.Remove(banque);
            var msg = "";
            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                msg = "Impossible de supprimer l'objet. Il est référence par plusieurs autres objets.";
            }
            return RedirectToAction("Index",new { msg=msg});
        }

        public ActionResult DataAssync()
        {
            var structure = db.Structures.Find(Session["IdStructure"]);
            var idbanque = structure.BanqueId(db);
            var banque = db.GetBanques.Find(idbanque);
            try
            {
                banque.TempsPassageArchivage=banque.TempsPassageArchivage == null ? banque.TempsPassageArchivage = 7:null;
                banque.DureeArchivage = banque.DureeArchivage == null ? banque.DureeArchivage = 365:null;
                banque.DureeRappelMiseEnDemaure = banque.DureeRappelMiseEnDemaure == null ? banque.DureeRappelMiseEnDemaure = 2:null;
                banque.RelanceDossierEchu = banque.RelanceDossierEchu == null ? banque.RelanceDossierEchu = 2:null;
            }
            catch (Exception)
            {}
            return View(banque);
        }

        [HttpPost]
        public async Task<ActionResult> DataAssync(genetrix.Models.Banque banque)
        {
            try
            {
                var model = db.GetBanques.Find(banque.Id);
                model.Activetimer = banque.Activetimer;
                model.Interval = banque.Interval;
                model.Epub = banque.Epub;
                model.HeureExecuteDebut = banque.HeureExecuteDebut;
                model.HeureExecuteFin = banque.HeureExecuteFin;
                model.TempsPassageArchivage = banque.TempsPassageArchivage;
                model.DureeArchivage = banque.DureeArchivage;
                model.DureeRappelMiseEnDemaure = banque.DureeRappelMiseEnDemaure;
                db.Entry(model).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("configurations", new { id=model.Id});
            }
            catch (Exception)
            {}
            return View(banque);
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
