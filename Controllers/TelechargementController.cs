using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace genetrix.Controllers
{
    public class TelechargementController : Controller
    {
        // GET: Telechargement
        public ActionResult Index()
        {
            return View();
        }

        public FileResult Download(int iddossier)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(@"c:\folder\myfile.ext");
            string fileName = "myfile.ext";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        //public FileActionResult Downloads()
        //{
        //    var dir = new System.IO.DirectoryInfo(Server.MapPath("~/App_Data/Images/"));
        //    System.IO.FileInfo[] fileNames = dir.GetFiles("*.*");
        //    List<string> items = new List<string>();

        //    foreach (var file in fileNames)
        //    {
        //        items.Add(file.Name);
        //    }

        //    return View(items);
        //}

    }
}