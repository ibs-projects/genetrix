using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using genetrix.Models;

namespace genetrix.Controllers
{
    public class ContactsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Contacts
        public async Task<ActionResult> Index()
        {
            var getContacts =new List<Contact>();
            if ((string)Session["userType"] == "CompteClient")
            {
                var clientID = Convert.ToInt32(Session["clientId"]);
                getContacts =await db.GetContacts.Where(c => c.IdClient == clientID).ToListAsync();
            }
            else if ((string)Session["userType"] == "CompteBanqueCommerciale")
            {
                var id = (string)Session["userId"];
                getContacts =await db.GetContacts.Where(c => c.IdGestionnaire == id).ToListAsync();
            }

            return View(getContacts);
        }

        // GET: Contacts/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = await db.GetContacts.FindAsync(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            if ((string)Session["userType"] == "CompteClient" && contact.IdClient!= Convert.ToInt32(Session["clientId"]))
            {
                return HttpNotFound();
            }
            else if ((string)Session["userType"] == "CompteBanqueCommerciale" && contact.IdGestionnaire== (string)Session["userId"])
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        // GET: Contacts/Create
        public ActionResult Create()
        {
            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom");
            ViewBag.GestionnaireId = new SelectList(db.Users, "Id", "Nom");
            return View();
        }

        // POST: Contacts/Create
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Groupe,NomComplet,Pays,Ville,Telephone,Telephone2,Email,GestionnaireId,ClientId")] Contact contact)
        {
            if (ModelState.IsValid)
            {
                if ((string)Session["userType"] == "CompteClient")
                    contact.IdClient = Convert.ToInt32(Session["clientId"]);
                else if ((string)Session["userType"] == "CompteBanqueCommerciale")
                    contact.IdGestionnaire = (string)Session["userId"];

                try
                {
                    db.GetContacts.Add(contact);
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {}
                return RedirectToAction("Index");
            }

            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom", contact.IdClient);
            ViewBag.GestionnaireId = new SelectList(db.Users, "Id", "Nom", contact.IdGestionnaire);
            return View(contact);
        }

        // GET: Contacts/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = await db.GetContacts.FindAsync(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            if ((string)Session["userType"] == "CompteClient" && contact.IdClient != Convert.ToInt32(Session["clientId"]))
            {
                return HttpNotFound();
            }
            else if ((string)Session["userType"] == "CompteBanqueCommerciale" && contact.IdGestionnaire == (string)Session["userId"])
            {
                return HttpNotFound();
            }
            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom", contact.IdClient);
            ViewBag.GestionnaireId = new SelectList(db.Users, "Id", "Nom", contact.IdGestionnaire);
            return View(contact);
        }

        // POST: Contacts/Edit/5
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Contact contact)
        {
            if (ModelState.IsValid)
            {
                var model = db.GetContacts.Find(contact.Id);
                model.Email = contact.Email;
                model.NomComplet = contact.NomComplet;
                model.Telephone = contact.Telephone;
                model.Telephone2 = contact.Telephone2;
                model.Pays = contact.Pays;
                model.Groupe = contact.Groupe;
                model.Ville = contact.Ville;
                db.Entry(model).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ClientId = new SelectList(db.GetClients, "Id", "Nom", contact.IdClient);
            ViewBag.GestionnaireId = new SelectList(db.Users, "Id", "Nom", contact.IdGestionnaire);
            return View(contact);
        }

        // GET: Contacts/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = await db.GetContacts.FindAsync(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            if ((string)Session["userType"] == "CompteClient" && contact.IdClient != Convert.ToInt32(Session["clientId"]))
            {
                return HttpNotFound();
            }
            else if ((string)Session["userType"] == "CompteBanqueCommerciale" && contact.IdGestionnaire == (string)Session["userId"])
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        // POST: Contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int? id)
        {
            Contact contact = await db.GetContacts.FindAsync(id);
            db.GetContacts.Remove(contact);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (Session != null)
            {
                if ((string)Session["userType"] == "CompteBanqueCommerciale")
                {
                    var url = (string)Session["urlaccueil"];
                    filterContext.Result = Redirect(url);
                }

                filterContext.Result = RedirectToAction("Index", "Index");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
