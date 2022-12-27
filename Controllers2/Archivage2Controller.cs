using genetrix.Models;
using genetrix.Models.Fonctions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace genetrix.Controllers
{
    public class Archivage2Controller : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        Dictionary<string, string> fname;
        Dictionary<string, string> DosName;
        int clientID;
        Client cl;

        public Archivage2Controller()
        {
            
        }
        // GET: Archivage
        public ActionResult Index(int idDossier = 0,int idFournisseur = 0
            ,int idDevise = 0,double montant_min=0,double montant_max=0, string estRecherche=""
            ,int agence = 0,string gestionnaire="",int client=0
            , string _id="", int annee_arch=0,int mois_arch=0,int jour_arch=0
            ,int annee_depot=0,int mois_depot=0,int jour_depot=0,string vue= "large"
            ,int annee_trait1 = 0,int mois_trait1 = 0,int jour_trait1=0
            ,int annee_trait2 = 0,int mois_trait2 = 0,int jour_trait2=0,string num_ref=null,
            string tablist = "box",string cat="",bool inp_dfx=false,bool inp_fp = false,bool inp_ref = false)
        {
            ViewBag.vue = vue;
            ViewBag.tablist = tablist;
            if (Session == null)
                return RedirectToAction("login", "auth");
            //ViewBag.navigation = string.IsNullOrEmpty(_id)? "archives" : _id;
            ViewBag.navigation =  _id;

            ViewBag.DevisesMonetaire = db.GetDeviseMonetaires.ToList();

            string serachString = "";int siteId = 0;

            //sidebar data
            var clientId = client;
            if ((string)Session["userType"] == "CompteClient")
                clientId = Convert.ToInt32(Session["clientId"]);
            else
                siteId = (int)Session["IdStructure"];

            var data = GetDossiersFolder(clientId,fournisseurId:idFournisseur,siteId:siteId);
            List<int> annees = new List<int>();
            try
            {
                var cl = db.GetClients.Find(clientId);
                var ann = DateTime.UtcNow.Year-1;
                if (cl != null && cl.DateCreation != null) ann = cl.DateCreation.Value.Year;
                for (int i = ann; i <= DateTime.UtcNow.Year; i++)
                    annees.Add(i);
            }
            catch (Exception e)
            {}
            ViewBag.annees=annees;

            if ((string)Session["userType"] == "CompteBanqueCommerciale")
            {
                ViewData["clients0"] = data.GroupBy(d => d.ClientName);
                ViewData["agences"] = data.GroupBy(d => d.SiteName);
            }
            else
            {
                ViewData["fournisseur"] = data.GroupBy(d => d.FournisseurName);
            }

            ViewData["date"] = data.GroupBy(d => d.AnneeArchivage);
            ViewData["devise"] = data.GroupBy(d => d.Devise);
            ViewBag.idFournisseur = idFournisseur;
            ViewBag.idDevise = idDevise;
            List<FileItemModel> models = new List<FileItemModel>();
            try
            {
                if (idDevise != 0)
                    ViewBag.DeviseName = db.GetDeviseMonetaires.Find(idDevise).Nom;
            }
            catch (Exception)
            {}
            string[] mois = new string[] { "Jeanvier", "Févier", "Mars", "Avril", "Mai", "Juin", "Juillet", "Aoùt", "Septembre", "Octobre", "Novembre", "Decembre" };
            try
            {
                if(!string.IsNullOrEmpty(cat) || ViewBag.navigation!="archives" && ViewBag.navigation!="fournisseurs"  && ViewBag.navigation!="devises" && ViewBag.navigation!="clients" && idDossier==0)
                {
                    if (client != 0)
                    {
                        data = data.Where(d => d.ClientId == client).ToList();
                        var clientName = "";
                        try
                        {
                            clientName = db.GetClients.Find(client).Nom;
                        }
                        catch (Exception)
                        {}
                        serachString += $"Client: <a href=\"#\">{clientName} </a> /";
                    }
                    if (agence != 0)
                    {
                        data = data.Where(d => d.SiteId == agence).ToList();
                        var agenceName = "";
                        try
                        {
                            agenceName = db.Structures.Find(agence).Nom;
                        }
                        catch (Exception)
                        {}
                        serachString += $"Agance: <a href=\"#\">{agenceName} </a> /";
                    }
                    if (!string.IsNullOrEmpty(gestionnaire))
                    {
                        data = data.Where(d => d.GestionnaireId == gestionnaire).ToList();
                        var gestName = "";
                        try
                        {
                            gestName = db.Users.Find(gestionnaire).NomComplet;
                        }
                        catch (Exception)
                        { }
                        serachString += $"Gestionnaire: <a href=\"#\">{gestName} </a> /";
                    }
                    
                    if (annee_trait1 != 0)
                    {
                        if(annee_trait2==0)
                            data = data.Where(d => d.Date_Etape22.Value.Year == annee_trait1).ToList();
                        else
                            data = data.Where(d => d.Date_Etape22.Value.Year >= annee_trait1).ToList();
                        serachString += $"Année traitement: <a href=\"#\">{annee_trait1 } </a> /";
                    }
                    if (mois_trait1 != 0)
                    {
                        if(mois_trait2==0)
                            data = data.Where(d => d.Date_Etape22.Value.Month == mois_trait1).ToList();
                        else 
                            data = data.Where(d => d.Date_Etape22.Value.Month >= mois_trait1).ToList();
                        serachString += $"Mois traitement: <a href=\"#\">{mois[mois_trait1- 1]} </a> /";
                    }
                    if (jour_trait1 != 0)
                    {
                        if(jour_trait2==0)
                            data = data.Where(d => d.Date_Etape22.Value.Day == jour_trait1).ToList();
                        else
                            data = data.Where(d => d.Date_Etape22.Value.Day >= jour_trait1).ToList();
                        serachString += $"Jour traitement: <a href=\"#\">{jour_trait1 } </a> /";
                    }
                     
                    if (annee_trait2 != 0)
                    {
                        data = data.Where(d => d.Date_Etape22.Value.Year <= annee_trait2).ToList();
                        serachString += $"Année traitement: <a href=\"#\">{annee_trait2 } </a> /";
                    }
                    if (mois_trait2 != 0)
                    {
                        //string[] mois = new string[] { "Janvier", "Février", "Mars", "Avril", "Mai", "Juin", "Juillet", "Août", "Septembre", "Octobre", "Novembre", "Decembre" };
                        data = data.Where(d => d.Date_Etape22.Value.Month <= mois_trait2).ToList();
                        serachString += $"Mois traitement: <a href=\"#\">{mois[mois_trait2- 1]} </a> /";
                    }
                    if (jour_trait2 != 0)
                    {
                        data = data.Where(d => d.Date_Etape22.Value.Day <= jour_trait2).ToList();
                        serachString += $"Jour traitement: <a href=\"#\">{jour_trait2 } </a> /";
                    }
                     
                    if (annee_arch != 0)
                    {
                        data = data.Where(d => d.AnneeArchivage == annee_arch).ToList();
                        serachString += $"Année archivage: <a href=\"#\">{annee_arch} </a> /";
                    }
                    if (mois_arch != 0)
                    {
                        //string[] mois = new string[] { "Janvier", "Février", "Mars", "Avril", "Mai", "Juin", "Juillet", "Août", "Septembre", "Octobre", "Novembre", "Decembre" };
                        data = data.Where(d => d.MoisArchivage == mois_arch).ToList();
                        serachString += $"Mois archivage: <a href=\"#\">{mois[mois_arch-1]} </a> /";
                    }
                    if (jour_arch != 0)
                    {
                        data = data.Where(d => d.JourArchivage == jour_arch).ToList();
                        serachString += $"Jour archivage: <a href=\"#\">{jour_arch} </a> /";
                    }
                    
                    if (annee_depot != 0)
                    {
                        data = data.Where(d => d.AnneeDepot == annee_depot).ToList();
                        serachString += $"Année dépot à la banque: <a href=\"#\">{annee_depot} </a> /";
                    }
                    if (mois_depot != 0)
                    {
                        //string[] mois = new string[] { "Janvier", "Février", "Mars", "Avril", "Mai", "Juin", "Juillet", "Août", "Septembre", "Octobre", "Novembre", "Decembre" };
                        data = data.Where(d => d.MoisDepot == mois_depot).ToList();
                        serachString += $"Mois dépot à la banque: <a href=\"#\">{mois[mois_depot-1]} </a> /";
                    }
                    if (jour_depot != 0)
                        {
                        data = data.Where(d => d.JourDepot == jour_depot).ToList();
                        serachString += $"Jour dépot à la banque: <a href=\"#\">{jour_depot} </a> /";
                    }
                    if (idFournisseur != 0)
                    {
                        data = data.Where(d => d.FournisseurId == idFournisseur).ToList();
                        var fournisseurName = "";
                        try
                        {
                            fournisseurName = db.GetFournisseurs.Find(idFournisseur).Nom;
                        }
                        catch (Exception)
                        {}                        
                        serachString += $"Fournisseur: <a href=\"#\">{fournisseurName} </a> /";
                    }
                    if (idDevise != 0)
                    {
                        var devise = "";
                        try
                        {
                            devise = db.GetDeviseMonetaires.Find(idDevise).Nom;
                        }
                        catch (Exception)
                        { }
                        data = data.Where(d => d.IdDevise == idDevise).ToList();
                        serachString += $"Devise: <a href=\"#\">{devise} </a> /";
                    }
                    if (montant_min != 0)
                    {
                        if(montant_max==0)
                            data = data.Where(d => d.Montant == montant_min).ToList();
                        else
                            data = data.Where(d => d.Montant >= montant_min).ToList();
                        serachString += $"Montant minimum: <a href=\"#\">{montant_min} </a> /";
                    }
                    if (montant_max != 0)
                    {
                        data = data.Where(d => d.Montant <= montant_max).ToList();
                        serachString += $"Montant maximum: <a href=\"#\">{montant_max} </a> /";
                    }
                    if (inp_dfx)
                    {
                        data = data.Where(d => d.DFX6FP6BEAC == 1).ToList();
                        serachString += $"Catégorie: <a href=\"#\">DFX</a> /";
                    }
                    if (inp_fp)
                    {
                        data = data.Where(d => d.DFX6FP6BEAC == 2).ToList();
                        serachString += $"Catégorie: <a href=\"#\">Fonds propres</a> /";
                    } 
                    if (inp_ref)
                    {
                        data = data.Where(d => d.DFX6FP6BEAC == 3).ToList();
                        serachString += $"Catégorie: <a href=\"#\">Refinancement</a> /";
                    }
                    if (!string.IsNullOrEmpty(num_ref))
                    {
                        data = data.Where(d => (d.DFX6FP6BEAC == 3 || d.DFX6FP6BEAC == 1) && d.GetReference==num_ref).ToList();
                        serachString += $"N° référence: <a href=\"#\">{num_ref}</a> /";
                    }
                    if(!string.IsNullOrEmpty(estRecherche))
                        ViewBag.serachString = !string.IsNullOrEmpty(serachString)?$"Rechercher par : {serachString}": "";
                    if (!string.IsNullOrEmpty(cat))
                    {
                        switch (cat)
                        {
                            case "dfx":
                                data = data.Where(d => d.DFX6FP6BEAC==1).ToList();
                                break;
                            case "dfx-g":
                                data = data.Where(d => d.DFX6FP6BEAC==1 && d.DfxId!=null && d.DfxId!=0 ).ToList();
                                break;
                            case "dfx-s":
                                data = data.Where(d => d.DFX6FP6BEAC==1 && (d.DfxId==null || d.DfxId!=0) ).ToList();
                                break;
                            case "ref":
                                data = data.Where(d => d.DFX6FP6BEAC==3).ToList();
                                break;
                            case "ref-usd":
                                data = data.Where(d => d.DFX6FP6BEAC == 3 && d.Devise=="USD").ToList();
                                break; 
                            case "ref-eur":
                                data = data.Where(d => d.DFX6FP6BEAC == 3 && d.Devise=="EUR").ToList();
                                break;
                            case "ref-aur":
                                data = data.Where(d => d.DFX6FP6BEAC == 3 && d.Devise!="EUR" && d.Devise!="USD").ToList();
                                break;
                            case "fp":
                                data = data.Where(d => d.DFX6FP6BEAC == 2).ToList();
                                break;
                            case "fp-usd":
                                data = data.Where(d => d.DFX6FP6BEAC == 2 && d.Devise == "USD").ToList();
                                break;
                            case "fp-eur":
                                data = data.Where(d => d.DFX6FP6BEAC == 2 && d.Devise == "EUR").ToList();
                                break;
                            case "fp-aur":
                                data = data.Where(d => d.DFX6FP6BEAC == 2 && d.Devise != "EUR" && d.Devise != "USD").ToList();
                                break;
                            default:
                                break;
                        }
                    }
                    models.AddRange(data);
                    //ViewBag.navigation = "archives";
                }
                else
                {
                    if(_id=="clients")
                    {

                    }
                    
                }

            }
            catch (Exception)
            {}
            ViewBag.mois = mois;
            if (idDossier!=0)
            {
                ViewBag.idDossier = idDossier;
                var dos = db.GetDossiers.Find(idDossier);
                models.AddRange(GetDossierFiles(dos,clientId));
            }
            if ((string)Session["userType"] == "CompteClient")
                return PartialView("~/Views/Archivage/Donnees.cshtml", models);
            else
            {
                var idSite = Convert.ToInt32(Session["IdStructure"]);
                List<AgentVM> agentVMs = new List<AgentVM>();
                try
                {
                    foreach (var item in db.GetCompteBanqueCommerciales.Where(d => d.IdStructure == idSite))
                        agentVMs.Add(new AgentVM() { Id = item.Id, NomComplet = item.NomComplet });
                }
                catch (Exception)
                {}
                ViewBag.agents = agentVMs.ToList();
                agentVMs = null; 
                
                List<ClientVM> clientVMs = new List<ClientVM>();
                foreach (var item in db.GetBanqueClients.Where(c=>c.IdSite==idSite))
                    try
                    {
                        clientVMs.Add(new ClientVM() { Id = item.ClientId, Nom = item.Client.Nom });
                    }
                    catch (Exception)
                    {}
                ViewBag.clients = clientVMs.ToList();
                clientVMs = null;
                return PartialView("~/Views/Archivage/Donnees_banque.cshtml", models);
            }
        }
        
        private void IniteData()
        {
            clientID = Convert.ToInt32(Session["clientId"]);
            cl = db.GetClients.Include("Fournisseurs").FirstOrDefault(f => f.Id == clientID);
            //get all client' fournisseurs
            fname = new Dictionary<string, string>();
            DosName = new Dictionary<string, string>();
            cl.Fournisseurs.ToList().ForEach(f =>
            {
                try
                {
                    fname.Add(f.Id.ToString(), f.Nom);
                }
                catch (Exception)
                { }
            });
            cl.Dossiers.Where(d => d.EtapesDosier == 26).ToList().ForEach(d =>
            {
                try
                {
                    DosName.Add(d.Dossier_Id.ToString(), d.Intitulé2);
                }
                catch (Exception)
                { }
            });
        }


        public ActionResult GetFoldersData(int clientId, int fournisseurId = 0, double montant = 0, int deviseId = 0, int siteId = 0, string gestionnaireId = ""
            , DateTime0 dateDepot = null, DateTime0 dateEnvoiBeac = null, DateTime0 dateObtensionDevise = null, DateTime0 dateArchivage = null)
        {
            IniteData();
      
            return PartialView(GetDossiersFolder(clientId,fournisseurId,montant,deviseId,siteId,gestionnaireId,dateDepot,dateEnvoiBeac,dateObtensionDevise,dateArchivage));
        }

        public ActionResult GetFilesData(int dossierId)
        {
            try
            {
                var dossier = db.GetDossiers.Find(dossierId);
                return PartialView(GetDossierFiles(dossier, dossier.ClientId));
            }
            catch (Exception)
            {}
            return PartialView(new List<FileItemModel>());
        }

        DateTime DateCreationClient;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dossier"></param>
        /// <returns></returns>
        public List<FileItemModel> GetDossierFiles(Dossier dossier,int clientId)
        {
            List<FileItemModel> model = new List<FileItemModel>();
            string projectPath = "~/EspaceClient/" + clientId + "/Ressources/Transferts";
            string folderName = "";
            try
            {
                folderName = Path.Combine(Server.MapPath(projectPath), dossier.Intitulé);
            }
            catch (Exception e)
            { }
            try
            {
                DirectoryInfo di = new DirectoryInfo(folderName);
                var files = di.GetFiles();
                
                foreach (var item in files)
                {
                    try
                    {
                        model.Add(new FileItemModel()
                        {
                            DateArchivage = item.CreationTime,
                            IsFolder = false,
                            MDate = item.LastWriteTime,
                            Name = item.Name,
                            Path = item.FullName,
                            MimeType = item.Extension,
                            Taille = item.Length,
                            ClientName=dossier.Client.Nom,
                            FournisseurName=dossier.GetFournisseur,
                            MontantString=dossier.MontantString,
                            Devise=dossier.DeviseToString,
                            DateDepot= dossier.DateDepotBank
                        });
                    }
                    catch (Exception e)
                    { }
                }
            }
            catch (Exception)
            {}
            return model;
        }

      
        public List<FileItemModel> GetDossiersFolder(int clientId, int fournisseurId = 0,double montant=0, int deviseId = 0, int siteId = 0, string gestionnaireId = ""
            , DateTime0 dateDepot=null,DateTime0 dateEnvoiBeac=null, DateTime0 dateObtensionDevise=null, DateTime0 dateArchivage=null)
        {
            var dossiers = new List<Dossier>();

            // vérifie si l'archive a été supprimé par le client et la banque
            var client_supp = true;
            var banque_supp = true;

            if ((string)Session["userType"] == "CompteClient")
            { client_supp = false; banque_supp = false; }
            else
            { banque_supp = false; }

            if (clientId!=0)
                dossiers = db.GetDossiers.Where(d => d.ClientId == clientId && d.EtapesDosier==26 && (d.SupprimeClient==client_supp || d.SupprimeBanque==banque_supp)).ToList();
            else
                dossiers = db.GetDossiers.Where(d => d.IdSite== siteId && (d.SupprimeClient == client_supp || d.SupprimeBanque == banque_supp)).ToList();

            List<FileItemModel> model = new List<FileItemModel>();
            if (fournisseurId != 0)
                dossiers = dossiers.Where(d => d.FournisseurId == fournisseurId).ToList();
            montant = Math.Round(montant, 2);
            if (montant != 0)
                dossiers = dossiers.Where(d => d.Montant == montant).ToList(); 
            if (deviseId != 0)
                dossiers = dossiers.Where(d => d.DeviseMonetaireId == deviseId).ToList();
            if (siteId != 0)
                dossiers = dossiers.Where(d => d.IdSite == siteId).ToList();
            if (!string.IsNullOrEmpty(gestionnaireId))
                dossiers = dossiers.Where(d => d.GestionnaireId == gestionnaireId).ToList();
            if (dateDepot != new DateTime0()) //determine la valeur defaut de la date
            {
                try
                {
                    var annee = dateDepot.Year;
                    var mois = dateDepot.Month;
                    var jour = dateDepot.Day;
                    if (annee != 0)
                        dossiers = dossiers.Where(d => d.DateDepotBank.Value.Year == annee).ToList();
                    if (mois != 0)
                        dossiers = dossiers.Where(d => d.DateDepotBank.Value.Month == mois).ToList();
                    if (jour != 0)
                        dossiers = dossiers.Where(d => d.DateDepotBank.Value.Day == jour).ToList();
                }
                catch (Exception)
                { }
            }
            if (dateEnvoiBeac != null)
            {
                var annee = dateEnvoiBeac.Year;
                var mois = dateEnvoiBeac.Month;
                var jour = dateEnvoiBeac.Day;
                if(annee !=0)
                    dossiers = dossiers.Where(d => ((DateTime)d.DateEnvoiBeac).Year == annee).ToList();
                if(mois !=0)
                    dossiers = dossiers.Where(d => ((DateTime)d.DateEnvoiBeac).Month == mois).ToList();
                if(jour !=0)
                    dossiers = dossiers.Where(d => ((DateTime)d.DateEnvoiBeac).Day == jour).ToList();
            }
            if (dateObtensionDevise != null)
            {
                var annee = dateObtensionDevise.Year;
                var mois = dateObtensionDevise.Month;
                var jour = dateObtensionDevise.Day;
                if(annee !=0)
                    dossiers = dossiers.Where(d => ((DateTime)d.ObtDevise).Year == annee).ToList();
                if(mois !=0)                       
                    dossiers = dossiers.Where(d => ((DateTime)d.ObtDevise).Month == mois).ToList();
                if(jour !=0)                       
                    dossiers = dossiers.Where(d => ((DateTime)d.ObtDevise).Day == jour).ToList();
            }
            if (dateArchivage != null)
            {
                var annee = dateArchivage.Year;
                var mois = dateArchivage.Month;
                var jour = dateArchivage.Day;
                if(annee !=0)
                    dossiers = dossiers.Where(d => ((DateTime)d.DateArchivage).Year == annee).ToList();
                if(mois !=0)                       
                    dossiers = dossiers.Where(d => ((DateTime)d.DateArchivage).Month == mois).ToList();
                if(jour !=0)                       
                    dossiers = dossiers.Where(d => ((DateTime)d.DateArchivage).Day == jour).ToList();
            }

            try
            {
                byte cl_bq = 0;
                if (Session["Profile"].ToString()=="client")
                    cl_bq = 1;
                else
                    cl_bq = 2;

                foreach (var item in dossiers)
                {
                    try
                    {
                        model.Add(
                           new FileItemModel()
                           {
                               DateArchivage = (DateTime)item.DateArchivage,
                               AnneeArchivage = ((DateTime)item.DateArchivage).Year,
                               MoisArchivage = ((DateTime)item.DateArchivage).Month,
                               JourArchivage = ((DateTime)item.DateArchivage).Day, 
                               DateDepot = (DateTime)item.DateDepotBank,
                               AnneeDepot= ((DateTime)item.DateDepotBank).Year,
                               MoisDepot = ((DateTime)item.DateDepotBank).Month,
                               JourDepot = ((DateTime)item.DateDepotBank).Day,
                               IsFolder = true,
                               Montant = item.Montant,
                               MDate = (DateTime)item.DateArchivage,
                               Name = item.ToString(cl_bq),
                               Taille = item.TailleFiles,
                               ClientName = item.Client!=null? item.Client.Nom:"",
                               ClientId=item.ClientId,
                               FournisseurName = item.GetFournisseur,
                               FournisseurId = item.FournisseurId,
                               GestionnaireId = item.GestionnaireId,
                               GestionnaireName = item.GestionnaireName,
                               SiteId = item.IdSite,
                               SiteName = item.Site!=null? item.Site.Nom:"",
                               Id=item.Dossier_Id,
                               Devise=item.DeviseToString,
                               MontantString = item.MontantString,
                               Date_Etape24 = item.Date_Etape24,
                               Date_Etape22 = item.Date_Etape22!=null? item.Date_Etape22: item.Date_Etape23,
                               GetCategorie = item.GetCategorie,
                               GetReference = item.GetReference,
                               GetAgenceName = item.GetAgenceName,
                               IdDevise=item.DeviseMonetaireId,
                               DFX6FP6BEAC = item.DFX6FP6BEAC,
                               DfxId = item.DfxId,
                               Annee_Mois= ((DateTime)item.DateArchivage).Year+"-"+ ((DateTime)item.DateArchivage).Month
                           }
                        ) ;
                    }
                    catch (Exception)
                    {}
                }
            }
            catch (Exception)
            {}
            return model;
        }

        public static long DirSize(DirectoryInfo dir)
        {
            return dir.GetFiles().Sum(fi => fi.Length) +
                   dir.GetDirectories().Sum(di => DirSize(di));
        }

        public ActionResult OpenView(string path)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            }
            catch (Exception)
            {
                path = "#";
            }
            if(path.Contains(".pdf"))
                return File(fs, "application/pdf");
            var tab = path.Split('.');
            var ext = "";
            try
            {
                ext = tab[tab.Length - 1];
            }
            catch (Exception)
            {}
            return base.File(path, "image/jpeg");
        }

        public ActionResult Image(string id)
        {
            return base.File(id, "image/jpeg");
        }

        public ActionResult ImageViewer(string path)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            }
            catch (Exception)
            {
                //path = "#";
            }
            return PartialView("ImageViewer", path);
        }

        public ActionResult Delete(string path,string adx)
        {
            try
            {
                if (path.Contains('.'))
                {
                    System.IO.File.Delete(path);
                }
                else
                {
                    Directory.Delete(path, true);
                }
            }
            catch (Exception)
            {}
            
            return RedirectToAction("Donnees",new { adx=adx});
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (Session != null)
            {                
                filterContext.Result = RedirectToAction("Index");
            }
            else
            {
                filterContext.Result = RedirectToAction("DefaultPage");
            }
        }

        public ActionResult DefaultPage()
        {
            return View();
        }


        public class DateTime0
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public int Day { get; set; }
        }
    }
}