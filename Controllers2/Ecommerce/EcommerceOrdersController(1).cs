using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Minible5.Controllers.Ecommerce
{
    public class EcommerceOrdersController : Controller
    {
        // GET: EcommerceOrders
        public ActionResult Index()
        {
            return View();
        }
    }
}