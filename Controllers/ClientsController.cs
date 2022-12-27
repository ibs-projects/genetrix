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
using genetrix.Models;

namespace genetrix.Controllers
{
    public class ClientsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        DateTime dateNow;

        public ClientsController()
        {
            dateNow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "W. Central Africa Standard Time");
        }

        // GET: Clients
        public async Task<ActionResult> Index()
        {
            //if (((string)Session["userType"] == "CompteClient"))
            if (((string)Session["userType"] == "CompteClient"))
            {
                return HttpNotFound();
            }
            if (((string)Session["userType"] == "CompteBanqueCommerciale"))
            {
                var structure = db.Structures.Find(Session["IdStructure"]);
                var id = structure.BanqueId(db);
                structure = null;

                var role = db.XtraRoles.Find(Convert.ToInt32(Session["IdXRole"]));
                var cls = new List<Client>();

                if (role == null)
                    return RedirectToAction("Login", "auth");
                if (role.Nom !="Admin")
                {
                    var site = db.Agences.Find(Session["IdStructure"]);
                    db.GetBanqueClients.ToList().ForEach(c =>
                    {
                        if (!site.VoirClientAutres)//permission structure
                    {
                            if (c.IdSite == site.Id)
                            {
                                if (Session["estChef"].ToString() == "True" || Session["role"].ToString() == "Chef d'agence")
                                    cls.Add(c.Client);
                                else if (!role.VoirClientAutres && c.IdGestionnaire == (string)Session["userId"])//permission de role
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
            }
            else if ((string)Session["userType"] == "CompteAdmin")
                return View(await db.GetClients.ToListAsync());
            else return View(new List<Client>());
        }
          
        public async Task<ActionResult> Compte()
        {
            try
            {

            }
            catch (Exception)
            {}
            return View();
        }

        // GET: Clients/Details/5
        public ActionResult Details(int? id,string msg="")
        {
            if(((string)Session["userType"] == "CompteClient"))
                id = Convert.ToInt32(Session["clientId"]);

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.GetClients
                .Include(c=>c.Banques)
                .Include(c=>c.PlanLSS)
                .Include(c=>c.FicheKYC)
                .Include(c=>c.Fournisseurs)
                .Include(c=>c.RCCM_Cl)
                .Include(c=>c.Statut)
                .Include(c=>c.ProcesVerbal)
                .Include(c=>c.EtatFinanciers)
                .Include(c=>c.AtestationHinneur)
                .Include(c=>c.PlanLOcalisationDomi)
                .Include(c=>c.JustifDomicile)
                .Include(c=>c.CarteIdentie)
                .FirstOrDefault(c=>c.Id==id);
            if (client == null)
            {
                return HttpNotFound();
            }
            try
            {
                if (client.Adresses.Count == 0) client.Adresses.Add(new Adresse()
                {
                    Email = client.Email,
                    Tel1 = client.Telephone
                });
                db.SaveChanges();
            }
            catch (Exception)
            {}

            client.PlanLSS = db.GetDocumentations.FirstOrDefault(d =>d.ClientId==client.Id && d.Nom == "PlanLSS");
            client.FicheKYC = db.GetDocumentations.FirstOrDefault(d => d.ClientId == client.Id && d.Nom == "FicheKYC");
            client.RCCM_Cl = db.GetDocumentations.FirstOrDefault(d => d.ClientId == client.Id && d.Nom == "RCCM");
            client.Statut = db.GetDocumentations.FirstOrDefault(d => d.ClientId == client.Id && d.Nom == "Statut");
            client.ProcesVerbal = db.GetDocumentations.FirstOrDefault(d => d.ClientId == client.Id && d.Nom == "ProcesVerbal");
            client.EtatFinanciers = db.GetDocumentations.FirstOrDefault(d => d.ClientId == client.Id && d.Nom == "EtatFinanciers");
            client.AtestationHinneur = db.GetDocumentations.FirstOrDefault(d => d.ClientId == client.Id && d.Nom == "AtestationHinneur");
            client.PlanLOcalisationDomi = db.GetDocumentations.FirstOrDefault(d => d.ClientId == client.Id && d.Nom == "PlanLOcalisationDomi");
            client.JustifDomicile = db.GetDocumentations.FirstOrDefault(d => d.ClientId == client.Id && d.Nom == "JustifDomicile");
            client.CarteIdentie = db.GetDocumentations.FirstOrDefault(d => d.ClientId == client.Id && d.Nom == "CarteIdentie");
           
            if(string.IsNullOrEmpty(msg))
                ViewBag.info = msg;
            if ((Session["Profile"].ToString()=="banque"))
            {
                var dd =new List<BanqueClient>();
                var structure = db.Structures.Find(Session["IdStructure"]);
                var banqueId = structure.BanqueId(db);
                foreach (var b in client.Banques)
                {
                    if (b.Site.BanqueId(db) == banqueId)
                        dd.Add(b);
                }
                ViewBag.Gestionnaires = dd;
                
                var ges =new List<CompteBanqueCommerciale>();
                db.GetCompteBanqueCommerciales.ToList().ForEach(c =>
                {
                    try
                    {
                        if (c.Structure.BanqueId(db) == banqueId)
                            ges.Add(c);
                    }
                    catch (Exception)
                    {}
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
            try
            {
                ViewBag.Historisations = db.GetHistorisations.Where(h => h.TypeHistorique == 1 && h.IdClient == client.Id).ToList();
            }
            catch (Exception)
            { }
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
            , HttpPostedFileBase EtatFinanciers, HttpPostedFileBase AtestationHinneur
            , HttpPostedFileBase PlanLOcalisationDomi, HttpPostedFileBase JustifDomicile, HttpPostedFileBase CarteIdentie)
        {
            
            if(!((string)Session["userType"] == "CompteClient"))
                return RedirectToAction("Details", new { id = 000 });
            var clientId = Convert.ToInt32(Session["clientId"]);
            var client = db.GetClients
                        .Include(c=>c.FicheKYC)
                        .Include(c=>c.PlanLSS)
                        .Include(c=>c.RCCM_Cl)
                        .Include(c=>c.Statut)
                        .Include(c=>c.ProcesVerbal)
                        .Include(c=>c.EtatFinanciers)
                        .Include(c=>c.AtestationHinneur)
                        .Include(c=>c.PlanLOcalisationDomi)
                        .Include(c=>c.JustifDomicile)
                        .Include(c=>c.CarteIdentie)
                        .FirstOrDefault(c=>c.Id==clientId);
            //FicheKYC
            if (FicheKYC != null)
            {
                if (!string.IsNullOrEmpty(FicheKYC.FileName))
                {
                    try
                    {
                        var dada = client.FicheKYC.GetImageDocumentAttache();
                        if (dada != null || client.Get_FicheKYC != "#")
                        {
                            //Supprime l'ancienne image
                            try
                            {
                                var imges = client.FicheKYC.GetImageDocumentAttaches;
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
                                var dd = db.GetDocumentations.Where(d=>d.ClientId==client.Id && d.Nom==client.FicheKYC.Nom).ToList();
                                client.FicheKYC = null;
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
                    client.FicheKYC = docAttache;
                    db.GetDocumentations.Add(docAttache);
                    docAttache.GetImageDocumentAttaches = new List<ImageDocumentAttache>();
                    db.SaveChanges();
                    imageModel.DocumentAttacheId = docAttache.Id;
                    docAttache.GetImageDocumentAttaches.Add(imageModel);
                    var doccc = db.GetDocumentations.Where(d => d.ClientId == client.Id).ToList();
                    chemin = null;
                    extension = null;

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
                    try
                    {
                        var dada = client.PlanLSS.GetImageDocumentAttache();
                        if (dada != null || client.Get_PlanLSS != "#")
                        {
                            //Supprime l'ancienne image
                            try
                            {
                                var imges = client.PlanLSS.GetImageDocumentAttaches;
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
                                var dd = db.GetDocumentations.Where(d => d.ClientId == client.Id && d.Nom == client.PlanLSS.Nom).ToList();
                                client.FicheKYC = null;
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

            try
            {
                //RCCM
                if (RCCM != null)
                {
                    if (!string.IsNullOrEmpty(RCCM.FileName))
                    {
                        try
                        {
                            var dada = client.RCCM_Cl.GetImageDocumentAttache();
                            if (dada != null || client.Get_RCCM != "#")
                            {
                                //Supprime l'ancienne image
                                try
                                {
                                    var imges = client.RCCM_Cl.GetImageDocumentAttaches;
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
                                    var dd = db.GetDocumentations.Where(d => d.ClientId == client.Id && d.Nom == client.RCCM_Cl.Nom).ToList();
                                    client.RCCM_Cl = null;
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
                        try
                        {
                            var dada = client.Statut.GetImageDocumentAttache();
                            if (dada != null || client.Get_Statut != "#")
                            {
                                //Supprime l'ancienne image
                                try
                                {
                                    var imges = client.Statut.GetImageDocumentAttaches;
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
                                    var dd = db.GetDocumentations.Where(d => d.ClientId == client.Id && d.Nom == client.Statut.Nom).ToList();
                                    client.Statut = null;
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
                        try
                        {
                            var dada = client.ProcesVerbal.GetImageDocumentAttache();
                            if (dada != null || client.Get_ProcesVerbal != "#")
                            {
                                //Supprime l'ancienne image
                                try
                                {
                                    var imges = client.ProcesVerbal.GetImageDocumentAttaches;
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
                                    var dd = db.GetDocumentations.Where(d => d.ClientId == client.Id && d.Nom == client.ProcesVerbal.Nom).ToList();
                                    client.ProcesVerbal = null;
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
                        try
                        {
                            var dada = client.EtatFinanciers.GetImageDocumentAttache();
                            if (dada != null || client.Get_EtatFinanciers != "#")
                            {
                                //Supprime l'ancienne image
                                try
                                {
                                    var imges = client.EtatFinanciers.GetImageDocumentAttaches;
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
                                    var dd = db.GetDocumentations.Where(d => d.ClientId == client.Id && d.Nom == client.EtatFinanciers.Nom).ToList();
                                    client.EtatFinanciers = null;
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
                        try
                        {
                            var dada = client.AtestationHinneur.GetImageDocumentAttache();
                            if (dada != null || client.Get_AtestationHinneur != "#")
                            {
                                //Supprime l'ancienne image
                                try
                                {
                                    var imges = client.AtestationHinneur.GetImageDocumentAttaches;
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
                                    var dd = db.GetDocumentations.Where(d => d.ClientId == client.Id && d.Nom == client.AtestationHinneur.Nom).ToList();
                                    client.AtestationHinneur = null;
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
               
            }
            catch (Exception)
            {}

            //JustifDomicile
            if (PlanLOcalisationDomi != null)
            {
                if (!string.IsNullOrEmpty(PlanLOcalisationDomi.FileName))
                {
                    try
                    {
                        var dada = client.PlanLOcalisationDomi.GetImageDocumentAttache();
                        if (dada != null || client.Get_PlanLOcalisationDomi != "#")
                        {
                            //Supprime l'ancienne fichier
                            try
                            {
                                var imges = client.PlanLOcalisationDomi.GetImageDocumentAttaches;
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
                                var dd = db.GetDocumentations.Where(d => d.ClientId == client.Id && d.Nom == client.PlanLOcalisationDomi.Nom).ToList();
                                client.PlanLOcalisationDomi = null;
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
                    imageModel.Titre = "Plan de localisation du domicile";

                    string chemin = Path.GetFileNameWithoutExtension(PlanLOcalisationDomi.FileName);
                    string extension = Path.GetExtension(PlanLOcalisationDomi.FileName);
                    chemin = imageModel.Titre + extension;
                    imageModel.Url = Path.Combine(CreateNewFolderDossier(clientId.ToString(), "PlanLOcalisationDomi"), chemin);

                    imageModel.NomCreateur = User.Identity.Name;

                    //Document attaché
                    var docAttache = new Documentation();
                    docAttache.Nom = "PlanLOcalisationDomi";
                    docAttache.ClientId = clientId;
                    client.PlanLOcalisationDomi = docAttache;
                    db.GetDocumentations.Add(docAttache);
                    docAttache.GetImageDocumentAttaches = new List<ImageDocumentAttache>();
                    db.SaveChanges();
                    imageModel.DocumentAttacheId = docAttache.Id;
                    docAttache.GetImageDocumentAttaches.Add(imageModel);
                    var doccc = db.GetDocumentations.Where(d => d.ClientId == client.Id).ToList();
                    chemin = null;
                    extension = null;

                    db.GetImageDocumentAttaches.Add(imageModel);
                    db.SaveChanges();
                    PlanLOcalisationDomi.SaveAs(imageModel.Url);

                }
            }
            
            //JustifDomicile
            if (JustifDomicile != null)
            {
                if (!string.IsNullOrEmpty(JustifDomicile.FileName))
                {
                    try
                    {
                        var dada = client.JustifDomicile.GetImageDocumentAttache();
                        if (dada != null || client.Get_JustifDomicile != "#")
                        {
                            //Supprime l'ancienne fichier
                            try
                            {
                                var imges = client.JustifDomicile.GetImageDocumentAttaches;
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
                                var dd = db.GetDocumentations.Where(d => d.ClientId == client.Id && d.Nom == client.JustifDomicile.Nom).ToList();
                                client.JustifDomicile = null;
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

                    string chemin = Path.GetFileNameWithoutExtension(JustifDomicile.FileName);
                    string extension = Path.GetExtension(JustifDomicile.FileName);
                    chemin = imageModel.Titre + extension;
                    imageModel.Url = Path.Combine(CreateNewFolderDossier(clientId.ToString(), "JustifDomicile"), chemin);

                    imageModel.NomCreateur = User.Identity.Name;

                    //Document attaché
                    var docAttache = new Documentation();
                    docAttache.Nom = "JustifDomicile";
                    docAttache.ClientId = clientId;
                    client.JustifDomicile = docAttache;
                    db.GetDocumentations.Add(docAttache);
                    docAttache.GetImageDocumentAttaches = new List<ImageDocumentAttache>();
                    db.SaveChanges();
                    imageModel.DocumentAttacheId = docAttache.Id;
                    docAttache.GetImageDocumentAttaches.Add(imageModel);
                    var doccc = db.GetDocumentations.Where(d => d.ClientId == client.Id).ToList();
                    chemin = null;
                    extension = null;

                    db.GetImageDocumentAttaches.Add(imageModel);
                    db.SaveChanges();
                    JustifDomicile.SaveAs(imageModel.Url);

                }
            }
            
            //CarteIdentie
            if (CarteIdentie != null)
            {
                if (!string.IsNullOrEmpty(CarteIdentie.FileName))
                {
                    try
                    {
                        var dada = client.CarteIdentie.GetImageDocumentAttache();
                        if (dada != null || client.Get_CarteIdentie != "#")
                        {
                            //Supprime l'ancienne image
                            try
                            {
                                var imges = client.CarteIdentie.GetImageDocumentAttaches;
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
                                var dd = db.GetDocumentations.Where(d => d.ClientId == client.Id && d.Nom == client.CarteIdentie.Nom).ToList();
                                client.CarteIdentie = null;
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
                    imageModel.Titre = "Copie de la carte d'identité ou du passeport";

                    string chemin = Path.GetFileNameWithoutExtension(CarteIdentie.FileName);
                    string extension = Path.GetExtension(CarteIdentie.FileName);
                    chemin = imageModel.Titre + extension;
                    imageModel.Url = Path.Combine(CreateNewFolderDossier(clientId.ToString(), "CarteIdentie"), chemin);

                    imageModel.NomCreateur = User.Identity.Name;

                    //Document attaché
                    var docAttache = new Documentation();
                    docAttache.Nom = "CarteIdentie";
                    docAttache.ClientId = clientId;
                    client.CarteIdentie = docAttache;
                    db.GetDocumentations.Add(docAttache);
                    docAttache.GetImageDocumentAttaches = new List<ImageDocumentAttache>();
                    db.SaveChanges();
                    imageModel.DocumentAttacheId = docAttache.Id;
                    docAttache.GetImageDocumentAttaches.Add(imageModel);
                    var doccc = db.GetDocumentations.Where(d => d.ClientId == client.Id).ToList();
                    chemin = null;
                    extension = null;

                    db.GetImageDocumentAttaches.Add(imageModel);
                    db.SaveChanges();
                    CarteIdentie.SaveAs(imageModel.Url);

                }
            }

            return RedirectToAction("Details",new { id=000});
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
                    imageModel.Url = Path.Combine(CreateNewFolderDossier(Session["clientId"] + "", document.Nom), chemin);
                    imageModel.NomCreateur = User.Identity.Name;
                    imageModel.DocumentAttache = document;
                    db.GetImageDocumentAttaches.Add(imageModel);
                    db.GetClients.Find(Session["clientId"]).AutresDocuments.Add(document);
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


        [HttpPost]
        public async Task<ActionResult> AddAttest(DocumentAttache document)
        {
            var msg = "";
            Client client = null;
            //if (Request.Files.Count > 0)
            {
                document.DateCreation = DateTime.Now;
                document.AttestSurHonneur = true;
                client = db.GetClients.Find(document.IdClient);
                client.DocumentAttaches = new List<DocumentAttache>();
                client.DocumentAttaches.Add(document);
                document.TypeDocumentAttaché = TypeDocumentAttaché.AttestationNonDefautApurement;

                db.GetDocumentAttaches.Add(document);
                await db.SaveChangesAsync();

                if (Request.Files != null)
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        var imgJustif = Request.Files[i];

                        var imageModel = new genetrix.Models.ImageDocumentAttache();
                        imageModel.Titre = document.Nom;

                        string chemin = Path.GetFileNameWithoutExtension(imgJustif.FileName);
                        string extension = Path.GetExtension(imgJustif.FileName);
                        chemin = imageModel.Titre + extension;
                        imageModel.Url = Path.Combine(CreateNewFolderDossier(client.Id.ToString(), document.Nom), chemin);
                        imageModel.NomCreateur = User.Identity.Name;
                        imageModel.DocumentAttache = document;
                        db.GetImageDocumentAttaches.Add(imageModel);
                        //db.GetDossiers.Add(dossier);
                        db.SaveChanges();
                        imgJustif.SaveAs(imageModel.Url);
                        chemin = null;
                        extension = null;
                    }

            }
            return RedirectToAction("Edit", new { id = client.Id });
        }


        public FileStreamResult GetPDF(int idDoc = 0, int idClient = 0,bool estAttest=false,bool estValide=false)
        {
            #region MyRegion
            //var client = db.GetClients.FirstOrDefault(d => d.Id == idClient);
            var path = "";
            var estPdf = false;
            try
            {
                DocumentAttache doc = db.GetDocumentAttaches.Include(d => d.GetImageDocumentAttaches).FirstOrDefault(d => d.Id == idDoc);
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
            if (string.IsNullOrEmpty(path)) return null;
            if(!estPdf)
                return File(fs, "image/jpeg");
            return File(fs, "application/pdf");
        }


        private string CreateNewFolderDossier(string clientId, string intitule)
        {
            try
            {
                string projectPath = "", folderName = "";
                if (intitule=="logo")
                {
                    projectPath = "~/EspaceClient/" + clientId + "/Ressources";
                    folderName = Path.Combine(Server.MapPath(projectPath), "Logo"); 
                }
                else
                {
                    projectPath = "~/EspaceClient/" + clientId + "/Ressources";
                    folderName = Path.Combine(Server.MapPath(projectPath), "DocumentsEntreprise"); 
                }
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
                client.DateCreation = dateNow;
                var _client=db.GetClients.Add(client);
                await db.SaveChangesAsync();
                if ((string)Session["userType"] == "CompteBanqueCommerciale")
                {
                    var siteId = Session["IdStructure"];
                    var banqueId = db.Structures.Find(siteId).BanqueId(db);
                    //var banque = db.GetBanques.Find(banqueId);
                    client.Banques.Add(new BanqueClient()
                    {
                        ClientId = _client.Id,
                        Client = _client,
                        IdSite = banqueId,
                        DateCreation = dateNow
                    });
                }
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(client);
        }


        [ActionName("Compte-client")]
        public ActionResult CompteClient()
        {
            return PartialView("CompteClient", new RegisterViewModel());
        }

        // GET: Clients/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if ((string)Session["userType"] == "CompteClient")
            {
                id = null;
                id = Convert.ToInt32(Session["clientId"]);
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = await db.GetClients.FindAsync(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            //Restriction
            var user = db.GetCompteClients.Find(Session["userId"]);
            try
            {
                if (!user.EstAdmin && client.ModeRestraint)
                    return RedirectToAction("Index", "Index", new { msg = "Vous ne pouvez pas changer les parametres de l'entreprise, Veuillez contacter votre administrateur" });
            }
            catch (Exception)
            { }
            return View(client);
        }

        // POST: Clients/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Client model, HttpPostedFileBase Logo = null)
        {
            if (ModelState.IsValid)
            {
                if ((string)Session["userType"] == "CompteClient")
                {
                    var id = Convert.ToInt32(Session["clientId"]);
                    if(id!=model.Id)
                        return View(model);
                }
                Client client = db.GetClients.Find(model.Id);
                client.Nom = model.Nom;
                client.Email = model.Email;
                client.Telephone = model.Telephone;
                client.Pays = model.Pays;
                client.Ville = model.Ville;
                client.Adresse = model.Adresse;
                client.ModeRestraint = model.ModeRestraint;
                client.CodeEtablissement = model.CodeEtablissement;
                db.Entry(client).State = EntityState.Modified;
                if (Logo != null)
                {
                    try
                    {
                        string chemin = Path.GetFileNameWithoutExtension(Logo.FileName);
                        string extension = Path.GetExtension(Logo.FileName);
                        chemin += extension;
                        var img = Path.Combine(CreateNewFolderDossier(client.Id + "","logo"), chemin);
                        if (!string.IsNullOrEmpty(Logo.FileName))
                        {
                            if (!string.IsNullOrEmpty(client.Logo))
                            {
                                //Supprime l'ancienne image
                                try
                                {
                                    System.IO.File.Delete(client.Logo);
                                }
                                catch (Exception e)
                                { }
                            }
                        }
                        client.Logo = "~/EspaceClient/" + client.Id + "/Ressources/Logo/"+chemin; 
                        Logo.SaveAs(img);
                        chemin = null;
                        extension = null;
                    }
                    catch (Exception)
                    { }
                }
                await db.SaveChangesAsync();
                return RedirectToAction("Details",new {id=client.Id});
            }
            return View(model);
        }

        public ActionResult GetFournisseurs(int? clienId)
        {
            if (clienId == null)
                clienId = Convert.ToInt32(Session["clientId"]);
            var ff = db.GetFournisseurs.Where(f => f.ClientId == clienId);
            return Json(from f in ff select new { f.Id,f.Nom},JsonRequestBehavior.AllowGet);
        }

         public ActionResult GetClientByGestion(string gestionId)
        {
            IEnumerable<BanqueClient> ff=null;
            if(string.IsNullOrEmpty(gestionId))
                ff = db.GetBanqueClients.Include(c=>c.Client).ToList();
            else
                ff = db.GetBanqueClients.Include(c=>c.Client).Where(f => f.IdGestionnaire == gestionId);
            return Json(from f in ff select new { f.ClientId,f.Client.Nom},JsonRequestBehavior.AllowGet);
        }

        // GET: Clients/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if ((string)Session["userType"] == "CompteClient")
                id = null;
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

        [ActionName("document-client")]
        public async Task<ActionResult> DeleteDoc(int? id,int? clid)
        {
            try
            {
                DocumentAttache doc = await db.GetDocumentAttaches.FindAsync(id);
                db.GetDocumentAttaches.Remove(doc);
                db.SaveChanges();
            }
            catch (Exception)
            {}            
            return RedirectToAction("Details",new { id= clid });
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int? id)
        {
            if ((string)Session["userType"] == "CompteClient")
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
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
