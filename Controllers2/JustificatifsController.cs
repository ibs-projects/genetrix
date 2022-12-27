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
using System.Data.Entity.Validation;

namespace genetrix.Controllers
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
            Justificatif justificatif = await db.GetJustificatifs.Include(j=>j.Dossier).FirstOrDefaultAsync(j=>j.Id==id);
            if (justificatif == null)
            {
                return HttpNotFound();
            }

            //if (justificatif.EstPartielle)
            //{
            //    justificatif.GetImages.ToList().AddRange(justificatif.GetImages);
            //    //foreach (var item in justificatif.FacturePieces)
            //    //{
            //    //}
            //}

            ViewBag.navigation = "rien";
            ViewBag.navigation_msg = "Details facture ";
            return View(justificatif);
        }
        
        public async Task<ActionResult> DeleteFile(int? idFile,int? idJustific)
        {
            string msg = "";
            try
            {
                ImageJustificatif file = await db.GetImageJustificatifs.FindAsync(idFile);
                idJustific = file.JustificatifId!=null?file.JustificatifId: idJustific;

                if (file.JustificatifId==idJustific)
                {
                    try
                    {
                        //remove physical file
                        Directory.Delete(file.Url);
                    }
                    catch (Exception)
                    { }
                    db.GetImageJustificatifs.Remove(file);
                    db.SaveChanges();
                }
                else
                {
                    msg = "Impossible de supprimer ce fichier, il est référencé par plusieurs factures !";
                }
            }
            catch (Exception)
            {}
            return RedirectToAction("Edit", new { id = idJustific,msg=msg });
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
                justificatif.IdClient = Convert.ToInt32(Session["clientId"]);
                db.GetJustificatifs.Add(justificatif);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.DossierId = new SelectList(db.GetDossiers, "Dossier_Id", "Intitule", justificatif.DossierId);
            return View(justificatif);
        }

        // GET: Justificatifs/Edit/5
        public async Task<ActionResult> Edit(int? id,string msg)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Justificatif justificatif = await db.GetJustificatifs.Include(j => j.Dossier).FirstOrDefaultAsync(j => j.Id == id);
            if (justificatif == null)
            {
                return HttpNotFound();
            }
            if (justificatif.Dossier.EtapesDosier == null || justificatif.Dossier.EtapesDosier == -1 || justificatif.Dossier.EtapesDosier == 0)
            { }
            else
            {
                return RedirectToAction("Edit", "Dossiers", new { justificatif.DossierId });
            }

            if (justificatif.EstPartielle)
            {
                //justificatif.GetImages.ToList().AddRange(justificatif.GetImages);
            }

            ViewBag.navigation = "rien";
            ViewBag.navigation_msg = "Edition facture ";
            ViewBag.DossierId = new SelectList(db.GetDossiers, "Dossier_Id", "Intitule", justificatif.DossierId);
            ViewBag.msg = msg;
            return View(justificatif);
        }

        [HttpPost]
        //public async Task<JsonResult> CreateJs([Bind(Include = "CompteurFactures,MontantTotalDossierString2,MontantTotalDossierString,Libellé,FournisseurJustif,MontantJustif,NumeroJustif,NbrePieces,DeviseJustif,DossierId,BanqueJustif,NbreJustif")] Justificatif justificatif)
        public async Task<JsonResult> CreateJs(Justificatif justificatif)
        {
            var msg = ""; var rest_just = 0;
            //HttpPostedFileBase ImagesFacture = null;
            //Justificatif justificatif = null;
            //IEnumerable<HttpPostedFileBase> ImagesFacture2 = null;
            justificatif.MontantJustif = Convert.ToDouble(justificatif.MontantJustif);
            justificatif.MontantPartiel = Convert.ToDouble(justificatif.MontantPartiel);
            Dossier dossier = null;
            try
            {
                //if (ModelState.IsValid)
                //if (Request.Files.Count > 0)
                {
                    try
                    {
                        justificatif.MontantJustif = Convert.ToDouble(justificatif.MontantTotalDossierString2);
                        if(justificatif.MontantJustif==0)
                            return Json(new object[] { new { justificatif.NumeroJustif, justificatif.NbrePieces, justificatif.MontantString, justificatif.MontantPartielString, justificatif.MontantRestantString }, 0, $"Le montant de la facture ne peut pas être nul !", rest_just }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception)
                    {
                        return Json(new object[] { new { justificatif.NumeroJustif, justificatif.NbrePieces, justificatif.MontantString, justificatif.MontantPartielString, justificatif.MontantRestantString }, 0, $"Montant {justificatif.MontantTotalDossierString} invalide !", rest_just }, JsonRequestBehavior.AllowGet);
                    }

                    if (justificatif.DateEmissioJustif>DateTime.Now)
                    {
                        return Json(new object[] { new { justificatif.NumeroJustif, justificatif.NbrePieces, justificatif.MontantString, justificatif.MontantPartielString, justificatif.MontantRestantString }, 0, "La date d'émission de la facture ne peut être superieure à la date du jour !", rest_just }, JsonRequestBehavior.AllowGet);
                    }
                   
                    justificatif.DateCreaAppJustif = DateTime.Now;
                    justificatif.DateEmissioJustif = DateTime.Now;
                    justificatif.DateModifJustif = DateTime.Now;
                    justificatif.IdClient = Convert.ToInt32(Session["clientId"]);

                    dossier = db.GetDossiers.Find(justificatif.DossierId);

                    if (dossier.Justificatifs.FirstOrDefault(j=>j.NumeroJustif==justificatif.NumeroJustif)!=null)
                    {
                        return Json(new object[] { new { justificatif.NumeroJustif, justificatif.NbrePieces, justificatif.MontantString, justificatif.MontantPartielString, justificatif.MontantRestantString }, 0, $"L'opération contient déjà une facture de numéro : {justificatif.NumeroJustif} !", rest_just }, JsonRequestBehavior.AllowGet);
                    }
                    int _NumberDecimalDigits = 0;
                    try
                    {
                        _NumberDecimalDigits = dossier.Montant.ToString().Split(',')[1].Length;
                    }
                    catch (Exception)
                    { }
                    //var mfacture = Math.Round(dossier.Justificatifs.Sum(j => j.MontantJustif) + justificatif.MontantJustif, _NumberDecimalDigits);
                    //var mop = Math.Round(dossier.Montant, _NumberDecimalDigits);
                    //if (mfacture < mop)
                    //{
                    //    return Json(new object[] { new { justificatif.NumeroJustif, justificatif.NbrePieces, justificatif.MontantString, justificatif.MontantPartielString, justificatif.MontantRestantString }, 0, $"Le montant de l'opération ne peut pas être supérieur au montant total des factures !", rest_just }, JsonRequestBehavior.AllowGet);
                    //}

                    db.GetJustificatifs.Add(justificatif);

                    if (dossier.NbreJustif < dossier.Justificatifs.Count
                        && justificatif.CompteurFactures <= dossier.Justificatifs.Count && dossier.NbreJustif > 0)
                        return Json(new object[] { new { justificatif.NumeroJustif, justificatif.NbrePieces, justificatif.MontantString, justificatif.MontantPartielString, justificatif.MontantRestantString }, 0, "Le nombre de facture est atteint !", rest_just }, JsonRequestBehavior.AllowGet);

                    dossier.NbreJustif = justificatif.CompteurFactures;
                    await db.SaveChangesAsync();

                    if (justificatif.EstAncienneFactMoins6mois == 1)
                    {
                        try
                        {
                            justificatif.EstPartielle = true;
                            await db.GetJustificatifs.Include(j=>j.Dossier).Include(j=>j.GetImages).Where(j => j.NumeroJustif == justificatif.NumeroJustif && j.Id!=justificatif.Id && j.IdClient == justificatif.IdClient).ForEachAsync(j =>
                            {
                                if (j.FournisseurId == justificatif.FournisseurId && j.Id != justificatif.Id)// && !j.EstPartielle)
                                {
                                    if (j.AncienneFacture == null)
                                    {
                                        justificatif.AncienneFacture = j;
                                        j.MontantRestant -= justificatif.MontantPartiel;
                                    }
                                }
                            });
                        }
                        catch (Exception)
                        { }
                    }
                    else 
                    {
                        justificatif.MontantRestant = justificatif.MontantRestant;
                    }

                    await db.SaveChangesAsync();

                    //if (dossier.NbreJustif > dossier.Justificatifs.Count)
                    if (justificatif.CompteurFactures > dossier.Justificatifs.Count)
                        rest_just = dossier.Justificatifs.Count + 1;
                    if (Request.Files != null)
                        for (int i = 0; i < Request.Files.Count; i++)
                        {
                            var imgJustif = Request.Files[i];
                            var imageModel = new genetrix.Models.ImageJustificatif();
                            int nbrPage = justificatif.GetImages == null ? 1 : justificatif.GetImages.Count + 1;
                            imageModel.Titre = $"Facture n°{justificatif.NumeroJustif} - page {nbrPage}";

                            string chemin = Path.GetFileNameWithoutExtension(imgJustif.FileName);
                            string extension = Path.GetExtension(imgJustif.FileName);
                            chemin = imageModel.Titre + extension;
                            imageModel.Url = Path.Combine(CreateNewFolderDossier(justificatif.Dossier.ClientId.ToString(), justificatif.Dossier.Intitulé), chemin);
                            imageModel.NomCreateur = User.Identity.Name;
                            imageModel.Justificatif = justificatif;
                            db.GetImageJustificatifs.Add(imageModel);
                            db.SaveChanges();
                            imgJustif.SaveAs(imageModel.Url);
                            chemin = null;
                            extension = null;
                        }
                    double montantreste = 0;
                    try
                    {
                        montantreste = Convert.ToDouble(dossier.MontantPassif.Replace('.',','));
                    }
                    catch (Exception)
                    {}
                    return Json(new object[] { new { justificatif.NumeroJustif, justificatif.NbrePieces, justificatif.MontantString, justificatif.MontantPartielString, justificatif.MontantRestantString, justificatif.Id,dossier.MontantTpmp }, 1, "success", rest_just, dossier.MontantPassif, montantreste }, JsonRequestBehavior.AllowGet);
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
            return Json(new object[] { new { justificatif.NumeroJustif, justificatif.FournisseurJustif, justificatif.MontantJustif, justificatif.DeviseJustif, justificatif.NbrePieces, justificatif.Id}, 0, msg, rest_just }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetfactureCorrespondants(int DossierId, int fournisseurId, int deviseId,string num)
        {
            Dossier doss=null;
            Justificatif jus=null;
            var clientId=Convert.ToInt32(Session["clientId"]);
            var fff = db.GetJustificatifs.Where(c => c.IdClient == clientId && c.NumeroJustif == num).ToList();
            //foreach (var d in db.GetClients.Include(c => c.Dossiers).FirstOrDefault(c => c.Id == clientId).Dossiers.ToList())
            foreach (var j in db.GetJustificatifs.Where(c => c.IdClient == clientId && c.NumeroJustif == num).ToList())
            {
                if (j.FournisseurId != fournisseurId)
                    continue;

                //foreach (var j in d.Justificatifs)
                {
                    //try
                    //{
                    //    if (j.NumeroJustif.Trim() != num.Trim())
                    //        continue;
                    //}
                    //catch (Exception)
                    //{}
                    //Si la facture est un règlement partiel
                    doss = j.Dossier;
                    jus = j;
                    break;
                }
               //break;
            }
            string msg = "";
            if(doss==null)
                return Json(new { estPartiel = "-1", reste = "", devise = "", montant = "",msg="" }, JsonRequestBehavior.AllowGet);

            var files = from im in jus.GetImages select im.GetImage();
            if (jus.MontantRestant2 > 0)
            {
                //var ddd = jus.GetImages;
                msg = "Il existe une facture de même numéro (" + num + ") ayant fait l'objet d'un règlement partiel. Voulez-vous completer cette facture?";
                return Json(new { estPartiel = "1", reste = jus.MontantRestant.ToString(), devise = jus.Devise, montant = jus.MontantJustif, msg = msg, files =from g in jus.GetImages select g.Id, dossierId = doss.Dossier_Id, anneeFact=jus.DateEmissioJustif.Year, moisFact=jus.DateEmissioJustif.Month, jourFact=jus.DateEmissioJustif.Day, nbrePieces = jus.NbrePieces}, JsonRequestBehavior.AllowGet) ;
            }
            else
            {
                return Json(new { estPartiel = "0", reste = jus.MontantRestant.ToString(), devise = jus.Devise, montant = jus.MontantJustif, msg = msg, files = from g in jus.GetImages select g.Id, dossierId = doss.Dossier_Id, anneeFact = jus.DateEmissioJustif.Year, moisFact = jus.DateEmissioJustif.Month, jourFact = jus.DateEmissioJustif.Day, nbrePieces = jus.NbrePieces }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetDetailJS(int id)
        {
            var model = db.GetJustificatifs.Include(j => j.GetImages).FirstOrDefault(j => j.Id == id);
            return PartialView("ViewDetails", model);
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

        public ActionResult DeletePieceJointe(int id)
        {
            return Json("", JsonRequestBehavior.AllowGet);
        }


        // POST: Justificatifs/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Justificatif model)
        {
            try
            {
                if(model.MontantPartiel>model.MontantJustif)
                {
                    ViewBag.msg = "Le montant payé ne peut être superieur au montant de la facture !";
                    throw new Exception();
                }

                var justificatif = db.GetJustificatifs.Find(model.Id);
                justificatif.MontantJustif = Convert.ToDouble(model.MontantJustif);
                justificatif.MontantPartiel = Convert.ToDouble(model.MontantPartiel);
                justificatif.BanqueJustif = model.BanqueJustif;
                justificatif.DateModifJustif = DateTime.Now;
                justificatif.NumeroJustif = model.NumeroJustif;

                db.Entry(justificatif).State = EntityState.Modified;
                try
                {
                    if (justificatif.EstAncienneFactMoins6mois == 1)
                    {
                        try
                        {
                            justificatif.EstPartielle = true;
                            await db.GetJustificatifs.Include(j => j.Dossier).Include(j => j.GetImages).Where(j => j.NumeroJustif == justificatif.NumeroJustif && j.Id != justificatif.Id && j.IdClient == justificatif.IdClient).ForEachAsync(j =>
                            {
                                if (j.FournisseurId == justificatif.FournisseurId && j.Id != justificatif.Id)// && !j.EstPartielle)
                                {
                                    if (j.AncienneFacture == null)
                                    {
                                        justificatif.AncienneFacture = j;
                                        j.MontantRestant = justificatif.MontantRestant;
                                    }
                                }
                            });
                        }
                        catch (Exception)
                        { }
                    }
                    else
                    {
                        justificatif.MontantRestant = justificatif.MontantRestant;
                    }
                }
                catch (Exception)
                { }
                try
                {
                    justificatif.EstPartielle = true;
                    await db.GetJustificatifs.Include(j => j.Dossier).Include(j => j.GetImages).Where(j => j.NumeroJustif == justificatif.NumeroJustif && j.Id != justificatif.Id && j.IdClient == justificatif.IdClient).ForEachAsync(j =>
                    {
                        if (j.FournisseurId == justificatif.FournisseurId && j.Id != justificatif.Id)// && !j.EstPartielle)
                        {
                            if (j.AncienneFacture == null)
                            {
                                justificatif.AncienneFacture = j;
                                j.MontantRestant = justificatif.MontantRestant;
                            }
                        }
                    });
                }
                catch (Exception)
                { }
                await db.SaveChangesAsync();
                if (Request.Files != null)
                {
                    ImageJustificatif imageModel = null;
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        try
                        {
                            var imgJustif = Request.Files[i];
                            imageModel = new genetrix.Models.ImageJustificatif();
                            imageModel.Titre = $"Facture n°{justificatif.NumeroJustif} - page {justificatif.GetImages.Count + 1}";

                            string chemin = Path.GetFileNameWithoutExtension(imgJustif.FileName);
                            string extension = Path.GetExtension(imgJustif.FileName);
                            chemin = imageModel.Titre + extension;
                            imageModel.Url = Path.Combine(CreateNewFolderDossier(justificatif.Dossier.ClientId.ToString(), justificatif.Dossier.Intitulé), chemin);
                            imageModel.NomCreateur = User.Identity.Name;
                            imageModel.Justificatif = justificatif;
                            db.GetImageJustificatifs.Add(imageModel);
                            db.SaveChanges();
                            imgJustif.SaveAs(imageModel.Url);
                            chemin = null;
                            extension = null;
                        }
                        catch (Exception)
                        {}
                    }
                }
                if (justificatif.DossierId==0)
                    return View(model);
                return RedirectToAction("Edit",new { id=justificatif.Id});
            }
            catch (Exception)
            {}
            ViewBag.DossierId = new SelectList(db.GetDossiers, "Dossier_Id", "Intitule", model.DossierId);
            return View(model);
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
            if (justificatif.Dossier.EtapesDosier == null || justificatif.Dossier.EtapesDosier == -1 || justificatif.Dossier.EtapesDosier == 0)
            { }
            else
            {
                return RedirectToAction("Edit", "Dossiers", new { justificatif.DossierId });
            }

            ViewBag.navigation = "rien";
            ViewBag.navigation_msg = "Suppression facture ";
            return View(justificatif);
        }

        // POST: Justificatifs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Justificatif justificatif = await db.GetJustificatifs.Include(j=>j.GetImages).FirstOrDefaultAsync(j=>j.Id==id);
            //suppression des fichiers
            string msg = "";
            var files = justificatif.GetImages.ToList();
            foreach (var f in files)
            {
                if (f.JustificatifId==justificatif.Id)
                {
                    try
                    {
                        System.IO.File.Delete(f.Url);
                        db.GetImageJustificatifs.Remove(f);
                    }
                    catch (Exception e)
                    { } 
                }
            }
            var iddoss = justificatif.DossierId;
            try
            {
                db.GetJustificatifs.Remove(justificatif);
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                msg = "Impossible de supprimer cette facture, car elle est référencée par une autre !";
            }
            return RedirectToAction("Edit","Dossiers",new { id=iddoss,msg=msg});
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
