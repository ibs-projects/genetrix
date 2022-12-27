using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using e_apurement.Models;

namespace eApurement.Controllers
{
    public class MailsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Mails
        public async Task<ActionResult> Index(string st="")
        {
            var adres = (Session["user"] as ApplicationUser).Email;
            var id = (Session["user"] as ApplicationUser).Id;
            var dd = db.GetMails.ToList();
            st = string.IsNullOrEmpty(st)? "recus":st;
            ViewBag.st = st;
            //ViewBag.navigation = "_mail";
            ViewBag.navigation = "mailing";
            ViewBag.navigation_msg = "liste mails";
            switch (st)
            {
                case "recus":
                    return View(await db.GetMails.Where(m => m.AdresseDest == adres && m.AdresseEmeteur !=adres && m.DestinataireId != id && !m.Lu).ToListAsync());
                case "envoi": return View(await db.GetMails.Where(m => m.AdresseDest != adres && m.AdresseEmeteur == adres && m.Envoyé).ToListAsync());
                case "brouil": return View(await db.GetMails.Where(m => m.AdresseDest == adres  && !m.Envoyé).ToListAsync());
                case "corb": return View(await db.GetMails.Where(m => (m.AdresseDest == adres && m.AdresseEmeteur == adres) && m.Corbeille).ToListAsync());
                default:
                    //return View(await db.GetMails.Where(m => m.AdresseDest == adres && !m.Envoyé && !m.Corbeille).ToListAsync());
                    return View(await db.GetMails.Where(m => !m.Corbeille).ToListAsync());
            }
            
        }

        // GET: Mails/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mail mail = await db.GetMails.FindAsync(int.Parse(id));
            if (mail == null)
            {
                return HttpNotFound();
            }

            if (!mail.Lu)
            {
                mail.Lu = true;
                db.SaveChanges();
            }
            ViewBag.navigation = "mailing";
            ViewBag.navigation_msg = "Details mail";
            return View(mail);
        }

        // GET: Mails/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Mails/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Objet,Message,Date,AdresseEmeteur,AdresseDest")] Mail mail)
        {
            if (ModelState.IsValid)
            {
                db.GetMails.Add(mail);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(mail);
        }

        // GET: Mails/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mail mail = await db.GetMails.FindAsync(id);
            if (mail == null)
            {
                return HttpNotFound();
            }
            return View(mail);
        }

        // POST: Mails/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Objet,Message,Date,AdresseEmeteur,AdresseDest")] Mail mail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mail).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(mail);
        }

        [HttpPost]
        public ActionResult MultipleDelete(FormCollection form)
        {
            if (form.Keys.Count > 0)
                foreach (var k in form.Keys)
                {
                    try
                    {
                        var id = int.Parse(k.ToString().Split('_')[1]);
                        var mail = db.GetMails.Find(id);
                        db.GetMails.Remove(mail);
                        db.SaveChanges();
                    }
                    catch (Exception ee)
                    { }
                }

            return RedirectToAction("Index");
        }

        // GET: Mails/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mail mail = await db.GetMails.FindAsync(int.Parse(id));
            if (mail == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "mailing";
            ViewBag.navigation_msg = "suppression mail";
            return View(mail);
        }

        // POST: Mails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            Mail mail = await db.GetMails.FindAsync(int.Parse(id));
            db.GetMails.Remove(mail);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
