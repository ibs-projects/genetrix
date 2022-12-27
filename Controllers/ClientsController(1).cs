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
using eApurement.Models;

namespace eApurement.Controllers
{
    public class ClientsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Clients
        public async Task<ActionResult> Index()
        {
            if ((Session["user"] is CompteClient))
            {
                var id = (Session["user"] as CompteClient).ClientId;
                return View(await db.GetClients.Where(c=>c.Id==id).ToListAsync());
            }
            if ((Session["user"] is CompteBanqueCommerciale))
            {
                var id=(Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);
                var role = db.XtraRoles.Find((Session["user"] as CompteBanqueCommerciale).IdXRole);
                var cls = new List<Client>();

                if (role == null)
                    return RedirectToAction("Login", "auth");
                if (role.Nom !="Admin")
                {
                    var site = db.Agences.Find((Session["user"] as CompteBanqueCommerciale).IdStructure);
                    db.GetBanqueClients.ToList().ForEach(c =>
                    {
                        if (!site.VoirClientAutres)//permission structure
                    {
                            if (c.IdSite == site.Id)
                            {
                                if (!role.VoirClientAutres && c.IdGestionnaire == (Session["user"] as CompteBanqueCommerciale).Id)//permission de role
                            {
                                    cls.Add(c.Client);
                                }
                            }
                        }
                        else
                        {
                            cls.Add(c.Client);
                        }
                    });
                }
                else
                {
                    cls =(from c in db.GetBanqueClients.ToList()
                         where c.Site.BanqueId(db) == id
                         select c.Client).ToList();
                }


                if (cls == null) cls = new List<Client>();
                return View(cls.ToList());
            }else if (Session["user"] is CompteAdmin)
                return View(await db.GetClients.ToListAsync());
            else return View(new List<Client>());
        }

        // GET: Clients/Details/5
        public ActionResult Details(int? id,string msg="")
        {
            if((Session["user"] is CompteClient))
                id = (Session["user"] as CompteClient).ClientId;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.GetClients
                .Include(c=>c.Banques)
                .Include(c=>c.PlanLSS)
                .Include(c=>c.FicheKYC)
                .Include(c=>c.Fournisseurs)
                .Include(c=>c.RCCM)
                .Include(c=>c.Statut)
                .Include(c=>c.ProcesVerbal)
                .Include(c=>c.EtatFinanciers)
                .Include(c=>c.AtestationHinneur)
                .FirstOrDefault(c=>c.Id==id);
            if (client == null)
            {
                return HttpNotFound();
            }
            client.PlanLSS = db.GetDocumentations.FirstOrDefault(d =>d.ClientId==client.Id && d.Nom == "PlanLSS");
            client.FicheKYC = db.GetDocumentations.FirstOrDefault(d => d.ClientId == client.Id && d.Nom == "FicheKYC");
            client.RCCM = db.GetDocumentations.FirstOrDefault(d => d.ClientId == client.Id && d.Nom == "RCCM");
            client.Statut = db.GetDocumentations.FirstOrDefault(d => d.ClientId == client.Id && d.Nom == "Statut");
            client.ProcesVerbal = db.GetDocumentations.FirstOrDefault(d => d.ClientId == client.Id && d.Nom == "ProcesVerbal");
            client.EtatFinanciers = db.GetDocumentations.FirstOrDefault(d => d.ClientId == client.Id && d.Nom == "EtatFinanciers");
            client.AtestationHinneur = db.GetDocumentations.FirstOrDefault(d => d.ClientId == client.Id && d.Nom == "AtestationHinneur");

            ViewBag.info = msg;
            if ((Session["Profile"].ToString()=="banque"))
            {
                var dd =new List<BanqueClient>();
                var banqueId= (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);
                foreach (var b in client.Banques)
                {
                    if (b.Site.BanqueId(db) == banqueId)
                        dd.Add(b);
                }
                ViewBag.Gestionnaires = dd;
                
                var ges =new List<CompteBanqueCommerciale>();
                db.GetCompteBanqueCommerciales.ToList().ForEach(c =>
                {
                    if (c.Structure.BanqueId(db) == banqueId)
                        ges.Add(c);
                });
                ViewBag.AllBankGestionnaires = ges;
               
                var agences =new List<Agence>();
                db.Agences.ToList().ForEach(c =>
                {
                    if (c.BanqueId(db) == banqueId)
                        agences.Add(c);
                });
                ViewBag.allAgences = agences;

                dd = null;ges = null;agences = null;
            }
            return View(client);
        }
        
        // GET: Clients/Create
        public ActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Addfiles(HttpPostedFileBase FicheKYC, HttpPostedFileBase PlanLSS
            , HttpPostedFileBase RCCM, HttpPostedFileBase Statut, HttpPostedFileBase ProcesVerbal
            , HttpPostedFileBase EtatFinanciers, HttpPostedFileBase AtestationHinneur)
        {
            
            if(!(Session["user"] is CompteClient))
                return RedirectToAction("Details", new { id = 000 });
            var clientId = (Session["user"] as CompteClient).ClientId;
            //FicheKYC
            if (FicheKYC != null)
            {
                if (!string.IsNullOrEmpty(FicheKYC.FileName))
                {
                    var imageModel = new eApurement.Models.ImageDocumentAttache();
                    imageModel.Titre = "FicheKYC";

                    string chemin = Path.GetFileNameWithoutExtension(FicheKYC.FileName);
                    string extension = Path.GetExtension(FicheKYC.FileName);
                    chemin = imageModel.Titre + extension;
                    imageModel.Url = Path.Combine(CreateNewFolderDossier(clientId.ToString(), "FicheKYC"), chemin);

                    imageModel.NomCreateur = User.Identity.Name;

                    //Document attaché
                    var docAttache = new Documentation();
                    docAttache.Nom = "FicheKYC";
                    docAttache.ClientId = clientId;
                    docAttache.GetImageDocumentAttaches = new List<ImageDocumentAttache>();
                    docAttache.GetImageDocumentAttaches.Add(imageModel);

                    chemin = null;
                    extension = null;

                    db.GetDocumentations.Add(docAttache);
                    db.GetImageDocumentAttaches.Add(imageModel);
                    db.SaveChanges();
                    FicheKYC.SaveAs(imageModel.Url);

                }
            }

            //PlanLSS
            if (PlanLSS != null)
            {
                if (!string.IsNullOrEmpty(PlanLSS.FileName))
                {
                    var imageModel = new eApurement.Models.ImageDocumentAttache();
                    imageModel.Titre = "Plan localisation du siège social";

                    string chemin = Path.GetFileNameWithoutExtension(PlanLSS.FileName);
                    string extension = Path.GetExtension(PlanLSS.FileName);
                    chemin = imageModel.Titre + extension;
                    imageModel.Url = Path.Combine(CreateNewFolderDossier(clientId.ToString(), "PlanLSS"), chemin);

                    imageModel.NomCreateur = User.Identity.Name;

                    //Document attaché
                    var docAttache = new Documentation();
                    docAttache.Nom = "PlanLSS";
                    docAttache.ClientId = clientId;
                    docAttache.GetImageDocumentAttaches = new List<ImageDocumentAttache>();
                    docAttache.GetImageDocumentAttaches.Add(imageModel);

                    chemin = null;
                    extension = null;

                    db.GetDocumentations.Add(docAttache);
                    db.GetImageDocumentAttaches.Add(imageModel);
                    db.SaveChanges();
                    PlanLSS.SaveAs(imageModel.Url);

                }
            }

            //RCCM
            if (RCCM != null)
            {
                if (!string.IsNullOrEmpty(RCCM.FileName))
                {
                    var imageModel = new eApurement.Models.ImageDocumentAttache();
                    imageModel.Titre = "Extrait RCCM ou autre document tenant lieu";

                    string chemin = Path.GetFileNameWithoutExtension(RCCM.FileName);
                    string extension = Path.GetExtension(RCCM.FileName);
                    chemin = imageModel.Titre + extension;
                    imageModel.Url = Path.Combine(CreateNewFolderDossier(clientId.ToString(), "RCCM"), chemin);

                    imageModel.NomCreateur = User.Identity.Name;

                    //Document attaché
                    var docAttache = new Documentation();
                    docAttache.Nom = "RCCM";
                    docAttache.ClientId = clientId;
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

            //Statut
            if (Statut != null)
            {
                if (!string.IsNullOrEmpty(Statut.FileName))
                {
                    var imageModel = new eApurement.Models.ImageDocumentAttache();
                    imageModel.Titre = "Copie des statuts";

                    string chemin = Path.GetFileNameWithoutExtension(Statut.FileName);
                    string extension = Path.GetExtension(Statut.FileName);
                    chemin = imageModel.Titre + extension;
                    imageModel.Url = Path.Combine(CreateNewFolderDossier(clientId.ToString(), "Statut"), chemin);

                    imageModel.NomCreateur = User.Identity.Name;

                    //Document attaché
                    var docAttache = new Documentation();
                    docAttache.Nom = "Statut";
                    docAttache.ClientId = clientId;
                    docAttache.GetImageDocumentAttaches = new List<ImageDocumentAttache>();
                    docAttache.GetImageDocumentAttaches.Add(imageModel);

                    chemin = null;
                    extension = null;

                    db.GetDocumentations.Add(docAttache);
                    db.GetImageDocumentAttaches.Add(imageModel);
                    db.SaveChanges();
                    Statut.SaveAs(imageModel.Url);

                }
            }

            //ProcesVerbal
            if (ProcesVerbal != null)
            {
                if (!string.IsNullOrEmpty(ProcesVerbal.FileName))
                {
                    var imageModel = new eApurement.Models.ImageDocumentAttache();
                    imageModel.Titre = "Proces-verbal nommant les dirigeants";

                    string chemin = Path.GetFileNameWithoutExtension(ProcesVerbal.FileName);
                    string extension = Path.GetExtension(ProcesVerbal.FileName);
                    chemin = imageModel.Titre + extension;
                    imageModel.Url = Path.Combine(CreateNewFolderDossier(clientId.ToString(), "ProcesVerbal"), chemin);

                    imageModel.NomCreateur = User.Identity.Name;

                    //Document attaché
                    var docAttache = new Documentation();
                    docAttache.Nom = "ProcesVerbal";
                    docAttache.ClientId = clientId;
                    docAttache.GetImageDocumentAttaches = new List<ImageDocumentAttache>();
                    docAttache.GetImageDocumentAttaches.Add(imageModel);

                    chemin = null;
                    extension = null;

                    db.GetDocumentations.Add(docAttache);
                    db.GetImageDocumentAttaches.Add(imageModel);
                    db.SaveChanges();
                    ProcesVerbal.SaveAs(imageModel.Url);

                }
            }

            //EtatFinanciers
            if (EtatFinanciers != null)
            {
                if (!string.IsNullOrEmpty(EtatFinanciers.FileName))
                {
                    var imageModel = new eApurement.Models.ImageDocumentAttache();
                    imageModel.Titre = "Les états financiers des deux derniers exercices";

                    string chemin = Path.GetFileNameWithoutExtension(EtatFinanciers.FileName);
                    string extension = Path.GetExtension(EtatFinanciers.FileName);
                    chemin = imageModel.Titre + extension;
                    imageModel.Url = Path.Combine(CreateNewFolderDossier(clientId.ToString(), "EtatFinanciers"), chemin);

                    imageModel.NomCreateur = User.Identity.Name;

                    //Document attaché
                    var docAttache = new Documentation();
                    docAttache.Nom = "EtatFinanciers";
                    docAttache.ClientId = clientId;
                    docAttache.GetImageDocumentAttaches = new List<ImageDocumentAttache>();
                    docAttache.GetImageDocumentAttaches.Add(imageModel);

                    chemin = null;
                    extension = null;

                    db.GetDocumentations.Add(docAttache);
                    db.GetImageDocumentAttaches.Add(imageModel);
                    db.SaveChanges();
                    EtatFinanciers.SaveAs(imageModel.Url);

                }
            }

            //AtestationHinneur
            if (AtestationHinneur != null)
            {
                if (!string.IsNullOrEmpty(AtestationHinneur.FileName))
                {
                    var imageModel = new eApurement.Models.ImageDocumentAttache();
                    imageModel.Titre = "Attestation sur honneur";

                    string chemin = Path.GetFileNameWithoutExtension(AtestationHinneur.FileName);
                    string extension = Path.GetExtension(AtestationHinneur.FileName);
                    chemin = imageModel.Titre + extension;
                    imageModel.Url = Path.Combine(CreateNewFolderDossier(clientId.ToString(), "AtestationHinneur"), chemin);

                    imageModel.NomCreateur = User.Identity.Name;

                    //Document attaché
                    var docAttache = new Documentation();
                    docAttache.Nom = "AtestationHinneur";
                    docAttache.ClientId = clientId;
                    docAttache.GetImageDocumentAttaches = new List<ImageDocumentAttache>();
                    docAttache.GetImageDocumentAttaches.Add(imageModel);

                    chemin = null;
                    extension = null;

                    db.GetDocumentations.Add(docAttache);
                    db.GetImageDocumentAttaches.Add(imageModel);
                    db.SaveChanges();
                    AtestationHinneur.SaveAs(imageModel.Url);

                }
            }


            return RedirectToAction("Details",new { id=000});
        }

        private string CreateNewFolderDossier(string clientId, string intituleDossier)
        {
            try
            {
                string projectPath = "~/EspaceClient/" + clientId + "/Ressources";
                string folderName = Path.Combine(Server.MapPath(projectPath), "DocumentsEntreprise");
                System.IO.Directory.CreateDirectory(folderName);
                return folderName;
            }
            catch (Exception ee)
            { }
            return "";
        }

        // POST: Clients/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Nom,Logo,Email")] Client client)
        {
            if (ModelState.IsValid)
            {
                ///verifie si le client existe deja
                var _bexiste = db.GetClients.FirstOrDefault(b => b.Nom.ToLower() == client.Nom.ToLower() || b.Email.ToLower()==client.Email.ToLower());
                if (_bexiste != null)
                {
                    ViewBag.msg = "Erreur. Un compte client du même nom ou même adresse mail existe existe déjà !";
                    return View("Create", client);
                }
                db.GetClients.Add(client);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(client);
        }

        // GET: Clients/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            id = (Session["user"] as CompteClient).ClientId;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = await db.GetClients.FindAsync(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Nom,Logo,Email,Telephone")] Client client)
        {
            if (ModelState.IsValid)
            {
                db.Entry(client).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = await db.GetClients.FindAsync(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int? id)
        {
            Client client = await db.GetClients.FindAsync(id);
            try
            {
                db.GetClients.Remove(client);
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                //ViewBag.info = "Impossible de supprimer cet élément, d'autres éléments en dépendent";
                return RedirectToAction("Details", new { id = id,msg= "Impossible de supprimer cet élément, d'autres éléments en dépendent" });
            }
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
