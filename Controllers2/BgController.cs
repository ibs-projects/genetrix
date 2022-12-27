using genetrix.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace genetrix.Controllers
{
    public class BgController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Bg
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bcl">banque ou client: b=banque et cl=client</param>
        /// <returns></returns>
        public async Task<ActionResult> Index(string bcl)
        {
            ePub epub = null;
            try
            {
                epub = await db.GetEPubs.FirstOrDefaultAsync();
            }
            catch (Exception)
            {}
            try
            {
                ViewBag.pubItemsBas = db.PubItems.Where(i => i.eType == 3 && i.Acive).ToList();
                ViewBag.pubItemsHaut = db.PubItems.Where(i => i.eType == 2 && i.Acive).ToList();
            }
            catch (Exception)
            {}
            ViewBag.bcl = bcl;
            return View("Index", epub);
        }
        
        public ActionResult Index2()
        {
            return View("Index2");
        }
        
        public ActionResult IndexPub()
        {
            return View();
        }
    }
}