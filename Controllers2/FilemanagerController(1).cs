using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eApurement.Controllers
{
    public class FilemanagerController : Controller
    {
        // GET: Filemanager
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult FileManager1Partial()
        {
            return PartialView("_FileManager1Partial", FilemanagerControllerFileManager1Settings.Model);
        }

        public FileStreamResult FileManager1PartialDownload()
        {
            return FileManagerExtension.DownloadFiles(FilemanagerControllerFileManager1Settings.DownloadSettings, FilemanagerControllerFileManager1Settings.Model);
        }
    }
    public class FilemanagerControllerFileManager1Settings
    {
        public const string RootFolder = @"~\EspaceClient";

        public static string Model { get { return RootFolder; } }
        public static DevExpress.Web.Mvc.FileManagerSettings DownloadSettings
        {
            get
            {
                var settings = new DevExpress.Web.Mvc.FileManagerSettings { Name = "FileManager1" };
                settings.SettingsEditing.AllowDownload = true;
                return settings;
            }
        }
    }

}