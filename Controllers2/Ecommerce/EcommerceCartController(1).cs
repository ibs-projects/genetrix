using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Minible5.Controllers.Ecommerce
{
    public class EcommerceCartController : Controller
    {
        // GET: EcommerceCart
        public ActionResult Index()
        {
            return View();
        }
    }
}