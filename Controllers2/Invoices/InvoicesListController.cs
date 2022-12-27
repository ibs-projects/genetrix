using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Minible5.Controllers.Invoices
{
    public class InvoicesListController : Controller
    {
        // GET: InvoicesList
        public ActionResult Index()
        {
            return View();
        }
    }
}