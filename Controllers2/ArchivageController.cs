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
    public class ArchivageController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        Dictionary<string, string> fname;
        Dictionary<string, string> DosName;
        int clientID;
        Client cl;

        public ArchivageController()
        {
            
        }
        // GET: Archivage
        public ActionResult Index()
        {
            ViewBag.medium = "medium";
            return View();
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

        public ActionResult Donnees(string adx="",string _id="",bool isabsolute=false)
        {
            IniteData();
            string[] tab = new string[4];
            ViewBag.navigation =!string.IsNullOrEmpty(adx)?adx: "archives";

            if (isabsolute) ViewBag.navigation = _id;

            bool am = false;
            try
            {
                if (string.IsNullOrEmpty(adx))
                {
                    //tab[0] = "root";
                    //tab[1] = "s";
                    //tab[2] = "all";
                }
                else
                {
                    tab = adx.Split('-');
                    adx = "";
                    foreach (var item in tab)
                    {
                        if (item == "gm" || item == "ga")
                        {
                            am = true;
                            break;
                        }
                        adx += item + "/";
                    }
                }

            }
            catch (Exception)
            {}

            List<FileItemModel> model = new List<FileItemModel>();
            model = Data();
            try
            {
                ViewData["sidebar"] = model.ToList();
            }
            catch (Exception)
            {}
            model = new List<FileItemModel>();
            model = Data(adx, isabsolute);
            if (am)
            {
                try
                {
                    int annee = Convert.ToInt32(tab[2]);
                    if (tab[1] == "gm")
                    {
                        int mois = Convert.ToInt32(tab[3]);
                        model = model.Where(d => d.DateArchivage.Year == annee && d.DateArchivage.Month == mois).ToList();
                    }
                    else
                    {
                        model = model.Where(d => d.DateArchivage.Year == annee).ToList();
                    }
                }
                catch (Exception)
                {}
            }
            return PartialView(model);
        }

        private List<FileItemModel> Data(string adx="",bool isabsolute=false)
        {
            List<FileItemModel> model = new List<FileItemModel>();

            try
            {
                ViewData["fname"] = fname;

                string projectPath = "~/EspaceClient/" + clientID + "/Ressources/";
                string folderName = "";
                try
                {
                    if (!isabsolute)
                    {
                        folderName = Path.Combine(Server.MapPath(projectPath), adx + "");
                    }
                    else
                    {
                        folderName = adx;// Server.MapPath(adx);
                    }
                }
                catch (Exception e)
                { }
                //var directories = Directory.GetDirectories(folderName);
                DirectoryInfo di = new DirectoryInfo(folderName);
                DirectoryInfo[] directories = null;
                try
                {
                    directories = di.GetDirectories("*");
                }
                catch (Exception e)
                { }
                var files = di.GetFiles();
                string idf = "",name="",path="",fourn="";
                long taille = 0;
                foreach (var item in directories)
                {
                    try
                    {
                        taille = item.GetFiles().Sum(fi => fi.Length) +
                                 item.GetDirectories().Sum(d => DirSize(d));
                        try
                        {
                            name = adx == "fournisseurs/" ? fname[item.Name] : DosName[idf];
                        }
                        catch (Exception)
                        {}
                        try
                        {
                            path= adx == "fournisseurs/" ? "Fournisseurs/1" : item.Name;
                        }
                        catch (Exception)
                        {}
                        try
                        {
                            fourn= item.Parent.Name == "Transferts" && fname.ContainsKey(idf) ? fname[idf] : "";
                        }
                        catch (Exception)
                        {}
                        try
                        { 
                            idf = "";
                            if (item.Parent.Name == "Transferts")
                            {
                                idf = item.Name.Split('_')[2];
                            }
                        }
                        catch (Exception)
                        {}

                        model.Add(new FileItemModel()
                        {
                            DateArchivage = item.CreationTime,
                            IsFolder = true,
                            MDate = item.LastWriteTime,
                            //Name = adx == "fournisseurs/" ? fname[item.Name] : item.Name,
                            Name = name,
                            Path = item.FullName,
                            SousRepertoires = item.GetDirectories(),
                            Fichiers = item.GetFiles(),
                            ParentPath = path,
                            Taille = taille,
                            ClientName = cl.Nom,
                            FournisseurName = fourn,
                            Parent = item.Parent.Name
                        }) ;
                    }
                    catch (Exception e)
                    { }
                }
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
                            Taille = item.Length
                        });
                    }
                    catch (Exception)
                    { }
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
    }
}