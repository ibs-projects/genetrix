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
using System.IO;
using System.Data.Entity.Validation;

namespace eApurement.Controllers
{
    public class JustificatifsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Justificatifs
        public async Task<ActionResult> Index()
        {
            var getJustificatifs = db.GetJustificatifs.Include(j => j.Dossier);
            return View(await getJustificatifs.ToListAsync());
        }

        // GET: Justificatifs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Justificatif justificatif = await db.GetJustificatifs.FindAsync(id);
            if (justificatif == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "rien";
            ViewBag.navigation_msg = "Details facture " + justificatif.Id;
            return View(justificatif);
        }

        // GET: Justificatifs/Create
        public ActionResult Create()
        {
            ViewBag.DossierId = new SelectList(db.GetDossiers, "Dossier_Id", "Intitule");
            return View();
        }

        // POST: Justificatifs/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Libellé,FournisseurJustif,BanqueJustif,MontantJustif,NumeroJustif,DateEmissioJustif,DateCreaAppJustif,DateModifJustif,NbrePieces,UtilisateurId,DeviseJustif,DossierId")] Justificatif justificatif)
        {
            if (ModelState.IsValid)
            {
                db.GetJustificatifs.Add(justificatif);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.DossierId = new SelectList(db.GetDossiers, "Dossier_Id", "Intitule", justificatif.DossierId);
            return View(justificatif);
        }

        // GET: Justificatifs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Justificatif justificatif = await db.GetJustificatifs.FindAsync(id);
            if (justificatif == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "rien";
            ViewBag.navigation_msg = "Edition facture " + justificatif.Id;
            ViewBag.DossierId = new SelectList(db.GetDossiers, "Dossier_Id", "Intitule", justificatif.DossierId);
            return View(justificatif);
        }

        [HttpPost]
        public async Task<JsonResult> CreateJs([Bind(Include = "CompteurFactures,Libellé,FournisseurJustif,MontantJustif,NumeroJustif,NbrePieces,DeviseJustif,DossierId,BanqueJustif,NbreJustif")] Justificatif justificatif/*, HttpPostedFileBase ImagesFacture1, HttpPostedFileBase ImagesFacture2,
            HttpPostedFileBase ImagesFacture3, HttpPostedFileBase ImagesFacture5, HttpPostedFileBase ImagesFacture6, HttpPostedFileBase ImagesFacture7*/)
        {
            var msg = ""; var rest_just = 0;
            //HttpPostedFileBase ImagesFacture = null;
            //Justificatif justificatif = null;
            //IEnumerable<HttpPostedFileBase> ImagesFacture2 = null;
            try
            {
                //if (ModelState.IsValid)
                //if (Request.Files.Count > 0)
                {
                    justificatif.DateCreaAppJustif = DateTime.Now;
                    justificatif.DateEmissioJustif = DateTime.Now;
                    justificatif.DateModifJustif = DateTime.Now;
                    db.GetJustificatifs.Add(justificatif);
                    var dossier = db.GetDossiers.Find(justificatif.DossierId);

                    if (dossier.NbreJustif < dossier.Justificatifs.Count
                        && justificatif.CompteurFactures <= dossier.Justificatifs.Count && dossier.NbreJustif > 0)
                        return Json(new object[] { new { justificatif.NumeroJustif, justificatif.FournisseurJustif, justificatif.MontantJustif, justificatif.DeviseJustif, justificatif.NbrePieces }, 0, "Le nombre de facture est atteint !", rest_just }, JsonRequestBehavior.AllowGet);

                    dossier.NbreJustif = justificatif.CompteurFactures;
                    await db.SaveChangesAsync();
                    if (dossier.NbreJustif > dossier.Justificatifs.Count)
                        rest_just = dossier.Justificatifs.Count + 1;
                    if (Request.Files != null)
                        for (int i = 0; i < Request.Files.Count; i++)
                        {
                            var imgJustif = Request.Files[i];

                            //var fileName = Path.GetFileName(file.FileName);

                            //var path = Path.Combine(Server.MapPath("~/App_Data/"), fileName);
                            //file.SaveAs(path);

                            var imageModel = new eApurement.Models.ImageJustificatif();
                            imageModel.Titre = "Page_" + justificatif.Intitulé;

                            string chemin = Path.GetFileNameWithoutExtension(imgJustif.FileName);
                            string extension = Path.GetExtension(imgJustif.FileName);
                            chemin = imageModel.Titre + DateTime.Now.ToString("yymmssff") + extension;
                            //imageModel.Url = Path.Combine(Server.MapPath(CreateNewFolderDossier(dossier.ClientId.ToString(),dossier.Intitulé)), chemin);
                            imageModel.Url = Path.Combine(CreateNewFolderDossier(justificatif.Dossier.ClientId.ToString(), justificatif.Dossier.Intitulé), chemin);
                            imageModel.NomCreateur = User.Identity.Name;
                            imageModel.Justificatif = justificatif;
                            db.GetImageJustificatifs.Add(imageModel);
                            //db.GetDossiers.Add(dossier);
                            db.SaveChanges();
                            imgJustif.SaveAs(imageModel.Url);
                            chemin = null;
                            extension = null;
                        }
                    if (false)
                        foreach (var tmp in Request.Files)
                        {
                            var imgJustif = tmp as HttpPostedFileBase;
                            if (!string.IsNullOrEmpty(imgJustif.FileName))
                            {
                                var imageModel = new eApurement.Models.ImageJustificatif();
                                imageModel.Titre = "Page_" + justificatif.Intitulé;

                                string chemin = Path.GetFileNameWithoutExtension(imgJustif.FileName);
                                string extension = Path.GetExtension(imgJustif.FileName);
                                chemin = imageModel.Titre + DateTime.Now.ToString("yymmssff") + extension;
                                //imageModel.Url = Path.Combine(Server.MapPath(CreateNewFolderDossier(dossier.ClientId.ToString(),dossier.Intitulé)), chemin);
                                imageModel.Url = Path.Combine(CreateNewFolderDossier(justificatif.Dossier.ClientId.ToString(), justificatif.Dossier.Intitulé), chemin);
                                imageModel.NomCreateur = User.Identity.Name;
                                imageModel.Justificatif = justificatif;
                                db.GetImageJustificatifs.Add(imageModel);
                                //db.GetDossiers.Add(dossier);
                                db.SaveChanges();
                                imgJustif.SaveAs(imageModel.Url);
                                chemin = null;
                                extension = null;
                            }
                        }
                    return Json(new object[] { new { justificatif.NumeroJustif, justificatif.FournisseurJustif, justificatif.MontantJustif, justificatif.DeviseJustif, justificatif.NbrePieces,justificatif.Id }, 1, "success", rest_just }, JsonRequestBehavior.AllowGet);
                }
                if (justificatif != null) { if (justificatif.DossierId == 0) msg = "Vueillez enrégistrer le dossier avant d'ajouter une facture"; }
                else if (justificatif == null)
                    msg = "Impossible d'ajouter une facture nulle !";
                else
                    msg = "Erreur de validation";
            }
            catch (DbEntityValidationException ee)
            {
                foreach (var error in ee.EntityValidationErrors)
                {
                    foreach (var thisError in error.ValidationErrors)
                    {
                        msg += thisError.ErrorMessage + ";\n";
                    }
                }
            }
            catch (Exception ee)
            {
                msg = ee.Message;
            }
            return Json(new object[] { new { justificatif.NumeroJustif, justificatif.FournisseurJustif, justificatif.MontantJustif, justificatif.DeviseJustif, justificatif.NbrePieces, justificatif.Id }, 0, msg, rest_just }, JsonRequestBehavior.AllowGet);
        }

        private string CreateNewFolderDossier(string clientId, string intituleDossier)
        {
            try
            {
                string projectPath = "~/EspaceClient/" + clientId + "/Ressources/Transferts";
                string folderName = Path.Combine(Server.MapPath(projectPath), intituleDossier);
                System.IO.Directory.CreateDirectory(folderName);
                return folderName;
            }
            catch (Exception ee)
            { }
            return "";
        }


        // POST: Justificatifs/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Libellé,FournisseurJustif,BanqueJustif,MontantJustif,NumeroJustif,DateEmissioJustif,DateCreaAppJustif,DateModifJustif,NbrePieces,UtilisateurId,DeviseJustif,DossierId")] Justificatif justificatif)
        {
            if (ModelState.IsValid)
            {
                db.Entry(justificatif).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.DossierId = new SelectList(db.GetDossiers, "Dossier_Id", "Intitule", justificatif.DossierId);
            return View(justificatif);
        }

        // GET: Justificatifs/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Justificatif justificatif = await db.GetJustificatifs.FindAsync(id);
            if (justificatif == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "rien";
            ViewBag.navigation_msg = "Suppression facture " + justificatif.Id;
            return View(justificatif);
        }

        // POST: Justificatifs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Justificatif justificatif = await db.GetJustificatifs.FindAsync(id);
            db.GetJustificatifs.Remove(justificatif);
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
