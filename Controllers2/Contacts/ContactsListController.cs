using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Minible5.Controllers.Contacts
{
    public class ContactsListController : Controller
    {
        // GET: ContactsList
        public ActionResult Index()
        {
            return View();
        }
    }
}