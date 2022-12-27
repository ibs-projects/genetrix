using e_apurement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Minible5.Controllers.Form
{
    public class FormWizardController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: FormWizard
        public ActionResult Index()
        {
            return View();
        }
    }
}