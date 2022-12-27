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
using System.IO;

namespace genetrix.Controllers
{
    public class FournisseursController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Fournisseurs
        /// <summary>
        /// /
        /// </summary>
        /// <param name="cl">Client id</param>
        /// <returns></returns>
        public async Task<ActionResult> Index(int? cl)
        {
            if ((string)Session["userType"] == "CompteClient")
                cl = Convert.ToInt32(Session["clientId"]);
            if (cl == null || cl == 0)
                return View(new List<Fournisseurs>());
            var getFournisseurs = db.GetFournisseurs.Where(f=>f.ClientId==cl).Include(f => f.Client);
              return View(await getFournisseurs.ToListAsync());
        }


        public FileStreamResult GetPDF(int idDoc = 0,int idFournisseur=0)
        {
            #region MyRegion
            var doss = db.GetFournisseurs.FirstOrDefault(d => d.Id == idFournisseur);
            var path = "";
            var estPdf = false;
            try
            {
                var doc = db.GetDocumentAttaches.Include(d => d.GetImageDocumentAttaches).FirstOrDefault(d => d.Id == idDoc);
                estPdf = doc.EstPdf;
                path = doc.GetImageDocumentAttache().Url;
            }
            catch (Exception)
            { }

            #endregion
            FileStream fs = null;
            try
            {
                fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            }
            catch (Exception)
            {
                path = null;
            }
            if (string.IsNullOrEmpty(path))
            {
                path = Path.Combine(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), @"assets\images\loading-error.jpg");
                try
                {
                    fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                }
                catch (Exception e)
                {
                }
            }

            return File(fs, "application/pdf");
        }


        // GET: Fournisseurs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fournisseurs fournisseurs = await db.GetFournisseurs.FindAsync(id);
            if (fournisseurs == null)
            {
                return RedirectToAction("Index");
                //return HttpNotFound();
            }
            fournisseurs.RCCM = db.GetDocumentations.FirstOrDefault(d => d.Nom == "RCCM" && d.IdFournisseur==id);
            fournisseurs.ListeGerants = db.GetDocumentations.FirstOrDefault(d => d.Nom == "ListeGerants" && d.IdFournisseur==id);
            fournisseurs.CopieStatuts = db.GetDocumentations.FirstOrDefault(d => d.Nom == "CopieStatuts" && d.IdFournisseur==id);
            fournisseurs.ListeAyantDroits = db.GetDocumentations.FirstOrDefault(d => d.Nom == "ListeAyantDroits" && d.IdFournisseur==id);
            fournisseurs.ProcesVerbalNommantDirigeants = db.GetDocumentations.FirstOrDefault(d => d.Nom == "ProcesVerbalNommantDirigeants" && d.IdFournisseur==id);
            fournisseurs.CarteIdentiteDirigeants = db.GetDocumentations.FirstOrDefault(d => d.Nom == "CarteIdentiteDirigeants" && d.IdFournisseur==id);
            fournisseurs.FicheKYCBenefi = db.GetDocumentations.FirstOrDefault(d => d.Nom == "FicheKYCBenefi" && d.IdFournisseur==id);
            fournisseurs.PriseActeDeclarationCompte = db.GetDocumentations.FirstOrDefault(d => d.Nom == "PriseActeDeclarationCompte" && d.IdFournisseur==id);
            fournisseurs.JustifDomicileBenefi = db.GetDocumentations.FirstOrDefault(d => d.Nom == "JustifDomicileBenefi" && d.IdFournisseur==id);

            return View(fournisseurs);
        }

        // GET: Fournisseurs/Create
        public ActionResult Create()
        {
            if (Session is null)
                RedirectToAction("Login", "Account");

            var clientId = Convert.ToInt32(Session["clientId"]);
          ViewBag.ClientId = new SelectList(db.GetClients.Where(c=>c.Id==clientId), "Id", "Nom");
            return View();
        }

        [HttpPost]
        public ActionResult Addfiles(int idFournisseur,HttpPostedFileBase RCCM, HttpPostedFileBase ListeGerants
            ,HttpPostedFileBase PriseActeDeclarationCompte, HttpPostedFileBase CopieStatuts, HttpPostedFileBase ListeAyantDroits
            , HttpPostedFileBase ProcesVerbalNommantDirigeants, HttpPostedFileBase CarteIdentiteDirigeants
            , HttpPostedFileBase JustifDomicileBenefi, HttpPostedFileBase FicheKYCBenefi)
        {
            var clientId = Convert.ToInt32(Session["clientId"]);
            var fournisseur = db.GetFournisseurs
                .Include(f => f.RCCM)
                .Include(f => f.ListeGerants)
                .Include(f => f.CopieStatuts)
                .Include(f => f.ListeAyantDroits)
                .Include(f => f.ProcesVerbalNommantDirigeants)
                .Include(f => f.CarteIdentiteDirigeants)
                .Include(f => f.FicheKYCBenefi)
                .Include(f => f.PriseActeDeclarationCompte)
                .Include(f => f.JustifDomicileBenefi)
                .FirstOrDefault(f => f.Id == idFournisseur);
            //Extrait RCCM ou autre document tenant lieu
            if (RCCM != null)
            {
                if (!string.IsNullOrEmpty(RCCM.FileName))
                {
                    try
                    {
                        var dada = fournisseur.RCCM.GetImageDocumentAttache();
                        if (dada != null || fournisseur.Get_RCCM != "#")
                        {
                            //Supprime l'ancienne image
                            try
                            {
                                var imges = fournisseur.RCCM.GetImageDocumentAttaches;
                                var nbr = imges.Count;

                                for (int i = 0; i < nbr; i++)
                                {
                                    try
                                    {
                                        var item = imges.ToList()[i];
                                        System.IO.File.Delete(item.Url);
                                        item.Url = "";
                                        //if (i > 0)
                                        db.GetImageDocumentAttaches.Remove(item);
                                    }
                                    catch (Exception)
                                    { }
                                }
                                var dd = db.GetDocumentations.Where(d => d.IdFournisseur == fournisseur.Id && d.Nom == fournisseur.RCCM.Nom).ToList();
                                fournisseur.RCCM = null;
                                foreach (var item in dd)
                                {
                                    db.GetDocumentations.Remove(item);
                                }
                                dd = null;
                            }
                            catch (Exception e)
                            { }
                        }
                        dada = null;
                        db.SaveChanges();
                    }
                    catch (Exception)
                    { }
                    var imageModel = new genetrix.Models.ImageDocumentAttache();
                    imageModel.Titre = "Extrait RCCM ou autre document tenant lieu";

                    string chemin = Path.GetFileNameWithoutExtension(RCCM.FileName);
                    string extension = Path.GetExtension(RCCM.FileName);
                    chemin = imageModel.Titre + extension;
                    imageModel.Url = Path.Combine(CreateNewFolderDossier(clientId.ToString(), ""+idFournisseur), chemin);

                    imageModel.NomCreateur = User.Identity.Name;

                    //Document attaché
                    var docAttache = new Documentation();
                    docAttache.Nom = "RCCM";
                    docAttache.ClientId = clientId;
                    docAttache.IdFournisseur = idFournisseur;
                    docAttache.GetImageDocumentAttaches = new List<ImageDocumentAttache>();
                    docAttache.GetImageDocumentAttaches.Add(imageModel);

                    chemin = null;
                    extension = null;

                    db.GetDocumentations.Add(docAttache);
                    db.GetImageDocumentAttaches.Add(imageModel);
                    db.SaveChanges();
                    RCCM.SaveAs(imageModel.Url);

                }
            }

            //Liste gérants de la société
            if (ListeGerants != null)
            {
                if (!string.IsNullOrEmpty(ListeGerants.FileName))
                {
                    try
                    {
                        var dada = fournisseur.ListeGerants.GetImageDocumentAttache();
                        if (dada != null || fournisseur.Get_ListeGerants != "#")
                        {
                            //Supprime l'ancienne image
                            try
                            {
                                var imges = fournisseur.ListeGerants.GetImageDocumentAttaches;
                                var nbr = imges.Count;

                                for (int i = 0; i < nbr; i++)
                                {
                                    try
                                    {
                                        var item = imges.ToList()[i];
                                        System.IO.File.Delete(item.Url);
                                        item.Url = "";
                                        //if (i > 0)
                                        db.GetImageDocumentAttaches.Remove(item);
                                    }
                                    catch (Exception)
                                    { }
                                }
                                var dd = db.GetDocumentations.Where(d => d.IdFournisseur == fournisseur.Id && d.Nom == fournisseur.ListeGerants.Nom).ToList();
                                fournisseur.ListeGerants = null;
                                foreach (var item in dd)
                                {
                                    db.GetDocumentations.Remove(item);
                                }
                                dd = null;
                            }
                            catch (Exception e)
                            { }
                        }
                        dada = null;
                        db.SaveChanges();
                    }
                    catch (Exception)
                    { }
                    var imageModel = new genetrix.Models.ImageDocumentAttache();
                    imageModel.Titre = "Liste gérants de la société";

                    string chemin = Path.GetFileNameWithoutExtension(ListeGerants.FileName);
                    string extension = Path.GetExtension(ListeGerants.FileName);
                    chemin = imageModel.Titre + extension;
                    imageModel.Url = Path.Combine(CreateNewFolderDossier(clientId.ToString(), ""+idFournisseur), chemin);

                    imageModel.NomCreateur = User.Identity.Name;

                    //Document attaché
                    var docAttache = new Documentation();
                    docAttache.Nom = "ListeGerants";
                    docAttache.ClientId = clientId;
                    docAttache.IdFournisseur = idFournisseur;
                    docAttache.GetImageDocumentAttaches = new List<ImageDocumentAttache>();
                    docAttache.GetImageDocumentAttaches.Add(imageModel);

                    chemin = null;
                    extension = null;

                    db.GetDocumentations.Add(docAttache);
                    db.GetImageDocumentAttaches.Add(imageModel);
                    db.SaveChanges();
                    ListeGerants.SaveAs(imageModel.Url);

                }
            }

            //JustifDomicileBenefi
            if (JustifDomicileBenefi != null)
            {
                if (!string.IsNullOrEmpty(JustifDomicileBenefi.FileName))
                {
                    try
                    {
                        var dada = fournisseur.JustifDomicileBenefi.GetImageDocumentAttache();
                        if (dada != null || fournisseur.Get_JustifDomicileBenefi != "#")
                        {
                            //Supprime l'ancienne fichier
                            try
                            {
                                var imges = fournisseur.JustifDomicileBenefi.GetImageDocumentAttaches;
                                var nbr = imges.Count;

                                for (int i = 0; i < nbr; i++)
                                {
                                    try
                                    {
                                        var item = imges.ToList()[i];
                                        System.IO.File.Delete(item.Url);
                                        item.Url = "";
                                        //if (i > 0)
                                        db.GetImageDocumentAttaches.Remove(item);
                                    }
                                    catch (Exception)
                                    { }
                                }
                                var dd = db.GetDocumentations.Where(d => d.IdFournisseur == fournisseur.Id && d.Nom == fournisseur.JustifDomicileBenefi.Nom).ToList();
                                fournisseur.JustifDomicileBenefi = null;
                                foreach (var item in dd)
                                {
                                    db.GetDocumentations.Remove(item);
                                }
                                dd = null;
                            }
                            catch (Exception e)
                            { }
                        }
                        dada = null;
                        db.SaveChanges();
                    }
                    catch (Exception)
                    { }
                    var imageModel = new genetrix.Models.ImageDocumentAttache();
                    imageModel.Titre = "Justificatifsn de domicile (facture d'eau ou d'électricité)";

                    string chemin = Path.GetFileNameWithoutExtension(JustifDomicileBenefi.FileName);
                    string extension = Path.GetExtension(JustifDomicileBenefi.FileName);
                    chemin = imageModel.Titre + extension;
                    imageModel.Url = Path.Combine(CreateNewFolderDossier(clientId.ToString(), "" + idFournisseur), chemin);

                    imageModel.NomCreateur = User.Identity.Name;

                    //Document attaché
                    var docAttache = new Documentation();
                    docAttache.Nom = "JustifDomicileBenefi";
                    docAttache.ClientId = clientId;
                    docAttache.IdFournisseur = idFournisseur;
                    docAttache.GetImageDocumentAttaches = new List<ImageDocumentAttache>();
                    docAttache.GetImageDocumentAttaches.Add(imageModel);

                    chemin = null;
                    extension = null;

                    db.GetDocumentations.Add(docAttache);
                    db.GetImageDocumentAttaches.Add(imageModel);
                    db.SaveChanges();
                    JustifDomicileBenefi.SaveAs(imageModel.Url);

                }
            }

            //PriseActeDeclaratioJustifDomicileBenefinCompte
            if (PriseActeDeclarationCompte != null)
            {
                if (!string.IsNullOrEmpty(PriseActeDeclarationCompte.FileName))
                {
                    try
                    {
                        var dada = fournisseur.PriseActeDeclarationCompte.GetImageDocumentAttache();
                        if (dada != null || fournisseur.Get_PriseActeDeclarationCompte != "#")
                        {
                            //Supprime l'ancienne fichier
                            try
                            {
                                var imges = fournisseur.PriseActeDeclarationCompte.GetImageDocumentAttaches;
                                var nbr = imges.Count;

                                for (int i = 0; i < nbr; i++)
                                {
                                    try
                                    {
                                        var item = imges.ToList()[i];
                                        System.IO.File.Delete(item.Url);
                                        item.Url = "";
                                        //if (i > 0)
                                        db.GetImageDocumentAttaches.Remove(item);
                                    }
                                    catch (Exception)
                                    { }
                                }
                                var dd = db.GetDocumentations.Where(d => d.IdFournisseur == fournisseur.Id && d.Nom == fournisseur.PriseActeDeclarationCompte.Nom).ToList();
                                fournisseur.PriseActeDeclarationCompte = null;
                                foreach (var item in dd)
                                {
                                    db.GetDocumentations.Remove(item);
                                }
                                dd = null;
                            }
                            catch (Exception e)
                            { }
                        }
                        dada = null;
                        db.SaveChanges();
                    }
                    catch (Exception)
                    { }
                    var imageModel = new genetrix.Models.ImageDocumentAttache();
                    imageModel.Titre = "Prise d'acte sur la déclaration du compte à la banque centrale pour les bénéficiaires resident de la CEMAC";

                    string chemin = Path.GetFileNameWithoutExtension(PriseActeDeclarationCompte.FileName);
                    string extension = Path.GetExtension(PriseActeDeclarationCompte.FileName);
                    chemin = imageModel.Titre + extension;
                    imageModel.Url = Path.Combine(CreateNewFolderDossier(clientId.ToString(), "" + idFournisseur), chemin);

                    imageModel.NomCreateur = User.Identity.Name;

                    //Document attaché
                    var docAttache = new Documentation();
                    docAttache.Nom = "PriseActeDeclarationCompte";
                    docAttache.ClientId = clientId;
                    docAttache.IdFournisseur = idFournisseur;
                    docAttache.GetImageDocumentAttaches = new List<ImageDocumentAttache>();
                    docAttache.GetImageDocumentAttaches.Add(imageModel);

                    chemin = null;
                    extension = null;

                    db.GetDocumentations.Add(docAttache);
                    db.GetImageDocumentAttaches.Add(imageModel);
                    db.SaveChanges();
                    PriseActeDeclarationCompte.SaveAs(imageModel.Url);

                }
            }
            
            //FicheKYCBenefi
            if (FicheKYCBenefi != null)
            {
                if (!string.IsNullOrEmpty(FicheKYCBenefi.FileName))
                {
                    try
                    {
                        var dada = fournisseur.FicheKYCBenefi.GetImageDocumentAttache();
                        if (dada != null || fournisseur.Get_FicheKYCBenefi != "#")
                        {
                            //Supprime l'ancienne fichier
                            try
                            {
                                var imges = fournisseur.FicheKYCBenefi.GetImageDocumentAttaches;
                                var nbr = imges.Count;

                                for (int i = 0; i < nbr; i++)
                                {
                                    try
                                    {
                                        var item = imges.ToList()[i];
                                        System.IO.File.Delete(item.Url);
                                        item.Url = "";
                                        //if (i > 0)
                                        db.GetImageDocumentAttaches.Remove(item);
                                    }
                                    catch (Exception)
                                    { }
                                }
                                var dd = db.GetDocumentations.Where(d => d.IdFournisseur == fournisseur.Id && d.Nom == fournisseur.FicheKYCBenefi.Nom).ToList();
                                fournisseur.FicheKYCBenefi = null;
                                foreach (var item in dd)
                                {
                                    db.GetDocumentations.Remove(item);
                                }
                                dd = null;
                            }
                            catch (Exception e)
                            { }
                        }
                        dada = null;
                        db.SaveChanges();
                    }
                    catch (Exception)
                    { }
                    var imageModel = new genetrix.Models.ImageDocumentAttache();
                    imageModel.Titre = "Fiche ou attestation KYC (connaissance du client) ou tout document en tenant lieu établie par la banque du bénéficiaire de l'existant et de la regularité du compte au regard des dispositions relatives à la lutte contre le blanchissement des capitaux";

                    string chemin = Path.GetFileNameWithoutExtension(FicheKYCBenefi.FileName);
                    string extension = Path.GetExtension(FicheKYCBenefi.FileName);
                    chemin = imageModel.Titre + extension;
                    imageModel.Url = Path.Combine(CreateNewFolderDossier(clientId.ToString(), "" + idFournisseur), chemin);

                    imageModel.NomCreateur = User.Identity.Name;

                    //Document attaché
                    var docAttache = new Documentation();
                    docAttache.Nom = "FicheKYCBenefi";
                    docAttache.ClientId = clientId;
                    docAttache.IdFournisseur = idFournisseur;
                    docAttache.GetImageDocumentAttaches = new List<ImageDocumentAttache>();
                    docAttache.GetImageDocumentAttaches.Add(imageModel);

                    chemin = null;
                    extension = null;

                    db.GetDocumentations.Add(docAttache);
                    db.GetImageDocumentAttaches.Add(imageModel);
                    db.SaveChanges();
                    FicheKYCBenefi.SaveAs(imageModel.Url);

                }
            }
            
            //CarteIdentiteDirigeants
            if (CarteIdentiteDirigeants != null)
            {
                if (!string.IsNullOrEmpty(CarteIdentiteDirigeants.FileName))
                {
                    try
                    {
                        var dada = fournisseur.CarteIdentiteDirigeants.GetImageDocumentAttache();
                        if (dada != null || fournisseur.Get_CarteIdentiteDirigeants != "#")
                        {
                            //Supprime l'ancienne fichier
                            try
                            {
                                var imges = fournisseur.CarteIdentiteDirigeants.GetImageDocumentAttaches;
                                var nbr = imges.Count;

                                for (int i = 0; i < nbr; i++)
                                {
                                    try
                                    {
                                        var item = imges.ToList()[i];
                                        System.IO.File.Delete(item.Url);
                                        item.Url = "";
                                        //if (i > 0)
                                        db.GetImageDocumentAttaches.Remove(item);
                                    }
                                    catch (Exception)
                                    { }
                                }
                                var dd = db.GetDocumentations.Where(d => d.IdFournisseur == fournisseur.Id && d.Nom == fournisseur.CarteIdentiteDirigeants.Nom).ToList();
                                fournisseur.CarteIdentiteDirigeants = null;
                                foreach (var item in dd)
                                {
                                    db.GetDocumentations.Remove(item);
                                }
                                dd = null;
                            }
                            catch (Exception e)
                            { }
                        }
                        dada = null;
                        db.SaveChanges();
                    }
                    catch (Exception)
                    { }
                    var imageModel = new genetrix.Models.ImageDocumentAttache();
                    imageModel.Titre = "Copie de la carte nationale d'identité ou passeport des dirigeants";

                    string chemin = Path.GetFileNameWithoutExtension(CarteIdentiteDirigeants.FileName);
                    string extension = Path.GetExtension(CarteIdentiteDirigeants.FileName);
                    chemin = imageModel.Titre + extension;
                    imageModel.Url = Path.Combine(CreateNewFolderDossier(clientId.ToString(), "" + idFournisseur), chemin);

                    imageModel.NomCreateur = User.Identity.Name;

                    //Document attaché
                    var docAttache = new Documentation();
                    docAttache.Nom = "CarteIdentiteDirigeants";
                    docAttache.ClientId = clientId;
                    docAttache.IdFournisseur = idFournisseur;
                    docAttache.GetImageDocumentAttaches = new List<ImageDocumentAttache>();
                    docAttache.GetImageDocumentAttaches.Add(imageModel);

                    chemin = null;
                    extension = null;

                    db.GetDocumentations.Add(docAttache);
                    db.GetImageDocumentAttaches.Add(imageModel);
                    db.SaveChanges();
                    CarteIdentiteDirigeants.SaveAs(imageModel.Url);

                }
            }

            //ProcesVerbalNommantDirigeants
            if (ProcesVerbalNommantDirigeants != null)
            {
                if (!string.IsNullOrEmpty(ProcesVerbalNommantDirigeants.FileName))
                {
                    try
                    {
                        var dada = fournisseur.ProcesVerbalNommantDirigeants.GetImageDocumentAttache();
                        if (dada != null || fournisseur.Get_ProcesVerbalNommantDirigeants != "#")
                        {
                            //Supprime l'ancienne fichier
                            try
                            {
                                var imges = fournisseur.ProcesVerbalNommantDirigeants.GetImageDocumentAttaches;
                                var nbr = imges.Count;

                                for (int i = 0; i < nbr; i++)
                                {
                                    try
                                    {
                                        var item = imges.ToList()[i];
                                        System.IO.File.Delete(item.Url);
                                        item.Url = "";
                                        //if (i > 0)
                                        db.GetImageDocumentAttaches.Remove(item);
                                    }
                                    catch (Exception)
                                    { }
                                }
                                var dd = db.GetDocumentations.Where(d => d.IdFournisseur == fournisseur.Id && d.Nom == fournisseur.ProcesVerbalNommantDirigeants.Nom).ToList();
                                fournisseur.ProcesVerbalNommantDirigeants = null;
                                foreach (var item in dd)
                                {
                                    db.GetDocumentations.Remove(item);
                                }
                                dd = null;
                            }
                            catch (Exception e)
                            { }
                        }
                        dada = null;
                        db.SaveChanges();
                    }
                    catch (Exception)
                    { }
                    var imageModel = new genetrix.Models.ImageDocumentAttache();
                    imageModel.Titre = "Copie du procès-verbal nommant les dirigeants ou tout autre document en tenant lieu";

                    string chemin = Path.GetFileNameWithoutExtension(ProcesVerbalNommantDirigeants.FileName);
                    string extension = Path.GetExtension(ProcesVerbalNommantDirigeants.FileName);
                    chemin = imageModel.Titre + extension;
                    imageModel.Url = Path.Combine(CreateNewFolderDossier(clientId.ToString(), "" + idFournisseur), chemin);

                    imageModel.NomCreateur = User.Identity.Name;

                    //Document attaché
                    var docAttache = new Documentation();
                    docAttache.Nom = "ProcesVerbalNommantDirigeants";
                    docAttache.ClientId = clientId;
                    docAttache.IdFournisseur = idFournisseur;
                    docAttache.GetImageDocumentAttaches = new List<ImageDocumentAttache>();
                    docAttache.GetImageDocumentAttaches.Add(imageModel);

                    chemin = null;
                    extension = null;

                    db.GetDocumentations.Add(docAttache);
                    db.GetImageDocumentAttaches.Add(imageModel);
                    db.SaveChanges();
                    ProcesVerbalNommantDirigeants.SaveAs(imageModel.Url);

                }
            }
            
            //ListeAyantDroits
            if (ListeAyantDroits != null)
            {
                if (!string.IsNullOrEmpty(ListeAyantDroits.FileName))
                {
                    try
                    {
                        var dada = fournisseur.ListeAyantDroits.GetImageDocumentAttache();
                        if (dada != null || fournisseur.Get_ListeAyantDroits != "#")
                        {
                            //Supprime l'ancienne fichier
                            try
                            {
                                var imges = fournisseur.ListeAyantDroits.GetImageDocumentAttaches;
                                var nbr = imges.Count;

                                for (int i = 0; i < nbr; i++)
                                {
                                    try
                                    {
                                        var item = imges.ToList()[i];
                                        System.IO.File.Delete(item.Url);
                                        item.Url = "";
                                        //if (i > 0)
                                        db.GetImageDocumentAttaches.Remove(item);
                                    }
                                    catch (Exception)
                                    { }
                                }
                                var dd = db.GetDocumentations.Where(d => d.IdFournisseur == fournisseur.Id && d.Nom == fournisseur.ListeAyantDroits.Nom).ToList();
                                fournisseur.ListeAyantDroits = null;
                                foreach (var item in dd)
                                {
                                    db.GetDocumentations.Remove(item);
                                }
                                dd = null;
                            }
                            catch (Exception e)
                            { }
                        }
                        dada = null;
                        db.SaveChanges();
                    }
                    catch (Exception)
                    { }
                    var imageModel = new genetrix.Models.ImageDocumentAttache();
                    imageModel.Titre = "Liste des ayant droits finaux personnes physiques";

                    string chemin = Path.GetFileNameWithoutExtension(ListeAyantDroits.FileName);
                    string extension = Path.GetExtension(ListeAyantDroits.FileName);
                    chemin = imageModel.Titre + extension;
                    imageModel.Url = Path.Combine(CreateNewFolderDossier(clientId.ToString(), "" + idFournisseur), chemin);

                    imageModel.NomCreateur = User.Identity.Name;

                    //Document attaché
                    var docAttache = new Documentation();
                    docAttache.Nom = "ListeAyantDroits";
                    docAttache.ClientId = clientId;
                    docAttache.IdFournisseur = idFournisseur;
                    docAttache.GetImageDocumentAttaches = new List<ImageDocumentAttache>();
                    docAttache.GetImageDocumentAttaches.Add(imageModel);

                    chemin = null;
                    extension = null;

                    db.GetDocumentations.Add(docAttache);
                    db.GetImageDocumentAttaches.Add(imageModel);
                    db.SaveChanges();
                    ListeAyantDroits.SaveAs(imageModel.Url);

                }
            }
            
            //CopieStatuts
            if (CopieStatuts != null)
            {
                if (!string.IsNullOrEmpty(CopieStatuts.FileName))
                {
                    try
                    {
                        var dada = fournisseur.CopieStatuts.GetImageDocumentAttache();
                        if (dada != null || fournisseur.Get_CopieStatuts != "#")
                        {
                            //Supprime l'ancienne image
                            try
                            {
                                var imges = fournisseur.CopieStatuts.GetImageDocumentAttaches;
                                var nbr = imges.Count;

                                for (int i = 0; i < nbr; i++)
                                {
                                    try
                                    {
                                        var item = imges.ToList()[i];
                                        System.IO.File.Delete(item.Url);
                                        item.Url = "";
                                        //if (i > 0)
                                        db.GetImageDocumentAttaches.Remove(item);
                                    }
                                    catch (Exception)
                                    { }
                                }
                                var dd = db.GetDocumentations.Where(d => d.IdFournisseur == fournisseur.Id && d.Nom == fournisseur.CopieStatuts.Nom).ToList();
                                fournisseur.CopieStatuts = null;
                                foreach (var item in dd)
                                {
                                    db.GetDocumentations.Remove(item);
                                }
                                dd = null;
                            }
                            catch (Exception e)
                            { }
                        }
                        dada = null;
                        db.SaveChanges();
                    }
                    catch (Exception)
                    { }
                    var imageModel = new genetrix.Models.ImageDocumentAttache();
                    imageModel.Titre = "Copie des statuts authetifiés par une autorité abilité";

                    string chemin = Path.GetFileNameWithoutExtension(CopieStatuts.FileName);
                    string extension = Path.GetExtension(CopieStatuts.FileName);
                    chemin = imageModel.Titre + extension;
                    imageModel.Url = Path.Combine(CreateNewFolderDossier(clientId.ToString(), "" + idFournisseur), chemin);

                    imageModel.NomCreateur = User.Identity.Name;

                    //Document attaché
                    var docAttache = new Documentation();
                    docAttache.Nom = "CopieStatuts";
                    docAttache.ClientId = clientId;
                    docAttache.IdFournisseur = idFournisseur;
                    docAttache.GetImageDocumentAttaches = new List<ImageDocumentAttache>();
                    docAttache.GetImageDocumentAttaches.Add(imageModel);

                    chemin = null;
                    extension = null;

                    db.GetDocumentations.Add(docAttache);
                    db.GetImageDocumentAttaches.Add(imageModel);
                    db.SaveChanges();
                    CopieStatuts.SaveAs(imageModel.Url);

                }
            }

            return RedirectToAction("Details", new { id = idFournisseur });
        }

        [HttpPost]
        public ActionResult AddAutresDocs(DocumentAttache document)
        {
            if (Request.Files != null)
            {
                try
                {
                    var autreDoc = Request.Files[0];

                    var imageModel = new genetrix.Models.ImageDocumentAttache();
                    imageModel.Titre = document.Nom;

                    string chemin = Path.GetFileNameWithoutExtension(autreDoc.FileName);
                    string extension = Path.GetExtension(autreDoc.FileName);
                    chemin = imageModel.Titre + extension;
                    imageModel.Url = Path.Combine(CreateNewFolderDossier("" + Session["clientId"], document.Id+""), chemin);
                    imageModel.NomCreateur = User.Identity.Name;
                    imageModel.DocumentAttache = document;
                    db.GetImageDocumentAttaches.Add(imageModel);
                    db.GetFournisseurs.Find(document.Id).AutresDocuments.Add(document);
                    db.SaveChanges();
                    autreDoc.SaveAs(imageModel.Url);
                    chemin = null;
                    extension = null;
                }
                catch (Exception ee)
                {}
            }
            return RedirectToAction("Details", new { id = 1 });
        }

        public async Task<ActionResult> GetCompte(int fourId,string numCmpt="")
        {
            if (string.IsNullOrEmpty(numCmpt))
            {
                var dd = (await db.GetFournisseurs.FindAsync(fourId)).CompteBeneficiaires;
                return Json(from d in dd select new { d.Id, d.Cle, d.AdresseBenf, d.NomBanque, d.Numero, d.CodeAgence, d.CodeEts, d.Nom, d.Adresse }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var d = await db.CompteBeneficiaires.FirstOrDefaultAsync(n=>n.IdFournisseur==fourId && n.Numero==numCmpt);
                return Json(new { d.Id, d.Cle, d.AdresseBenf, d.NomBanque, d.Numero, d.CodeAgence, d.CodeEts, d.Nom, d.Adresse }, JsonRequestBehavior.AllowGet);
            }
        }


        private string CreateNewFolderDossier(string clientId, string intituleDossier)
        {
            //intituleDossier: fournisseurId
            try
            {
                string projectPath = "~/EspaceClient/" + clientId + "/Ressources/Fournisseurs";
                string folderName = Path.Combine(Server.MapPath(projectPath), intituleDossier);
                System.IO.Directory.CreateDirectory(folderName);
                return folderName;
            }
            catch (Exception ee)
            { }
            return "";
        }


        // POST: Fournisseurs/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Nom,CodeEts,Tel2,Tel1,ClientId,Pays,Adresse,Ville,Email")] Fournisseurs fournisseurs)
        {
            if (ModelState.IsValid)
            {
                db.GetFournisseurs.Add(fournisseurs);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom", fournisseurs.ClientId);
            return View(fournisseurs);
        }

        // GET: Fournisseurs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fournisseurs fournisseurs = await db.GetFournisseurs.FindAsync(id);
            if (fournisseurs == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom", fournisseurs.ClientId);
            return View(fournisseurs);
        }

        // POST: Fournisseurs/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "CodeEts,Id,Pays,Nom,Tel2,Tel1,ClientId,Adresse,Ville,Email")] Fournisseurs fournisseurs)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fournisseurs).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom", fournisseurs.ClientId);
            return View(fournisseurs);
        }

        // GET: Fournisseurs/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fournisseurs fournisseurs = await db.GetFournisseurs.FindAsync(id);
            if (fournisseurs == null)
            {
                return HttpNotFound();
            }
            return View(fournisseurs);
        }

        [ActionName("document-fournisseur")]
        public async Task<ActionResult> DeleteDoc(int? id, int? fourid)
        {
            try
            {
                DocumentAttache doc = await db.GetDocumentAttaches.FindAsync(id);
                db.GetDocumentAttaches.Remove(doc);
                db.SaveChanges();
            }
            catch (Exception)
            { }
            return RedirectToAction("Details", new { id = fourid });
        }


        // POST: Fournisseurs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Fournisseurs fournisseurs = await db.GetFournisseurs.FindAsync(id);
            db.GetFournisseurs.Remove(fournisseurs);
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
