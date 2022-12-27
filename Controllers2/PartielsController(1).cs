using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eApurement.Controllers
{
    public class PartielsController : Controller
    {
        // GET: Partiels
        public ActionResult PdfViewier(string file_to_upload="", HttpPostedFileBase file=null)
        {
            ViewBag.inputName = file_to_upload;
            return View("~/Views/_Shared/PartialViews/Pdf_Viewer.cshtml");//,file_to_upload);
        }
    }
}