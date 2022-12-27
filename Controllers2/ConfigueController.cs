using e_apurement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace eApurement.Controllers
{
    public class ConfigueController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Configue
        public ActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> RefleshClientInterface()
        {

            return Json("", JsonRequestBehavior.AllowGet);
        }
        
        public async Task<JsonResult> UpdateClientData()
        {
            //declenche le processus d'apurement
            foreach (var item in db.GetDossiers.Where(d=>d.EtapesDosier<10).ToList())//dossiers à apurer
            {
                if ((DateTime.Now- item.DateModif).Days>1)
                {

                }
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}