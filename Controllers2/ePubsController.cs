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
    public class ePubsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ePubs
        public async Task<ActionResult> Index()
        {
            return View(await db.GetEPubs.ToListAsync());
        }

        // GET: ePubs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ePub ePub = await db.GetEPubs.FindAsync(id);
            if (ePub == null)
            {
                return HttpNotFound();
            }
            return View(ePub);
        }

        // GET: ePubs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ePubs/Create
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Etat")] ePub ePub)
        {
            if (ModelState.IsValid)
            {
                db.GetEPubs.Add(ePub);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(ePub);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreatePubItem(PubItem pubItem)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var pub = db.PubItems.Find(pubItem.Id);
                    pub.DateDebut = pubItem.DateDebut;
                    pub.HeureDebut = pubItem.HeureDebut;
                    pub.DateFin = pubItem.DateFin;
                    pub.HeureFin = pubItem.HeureFin;
                    pub.ChaqueHeure = pubItem.ChaqueHeure;
                    pub.DureeApp = pubItem.DureeApp;
                    pub.DuréeAtt = pubItem.DuréeAtt;
                    pub.Acive = pubItem.Acive;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Edit", "ePubs", new { id = pub.IdePub });
                }
                pubItem.IdePub = db.GetEPubs.First().Id;
            }
            catch (Exception)
            {}
            return RedirectToAction("Edit", "ePubs", new { id = pubItem.IdePub });
        }

        // GET: ePubs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var strId = Session["IdStructure"];
            var banqueId = db.Structures.FindAsync(strId).Result.BanqueId(db);
            var banque = await db.GetBanques.FindAsync(banqueId);
            if (banque.GetEPub == null)
            {
                banque.GetEPub = new ePub()
                {
                    Etat = 1,
                };
                db.SaveChanges();
            }
            ViewBag.navigation = "epub";
            ViewBag.navigation_msg = "Configuration";
            if (banque.GetEPub.WidhtCD == null || banque.GetEPub.WidhtCD == 0) banque.GetEPub.WidhtCD = 450;
            if (banque.GetEPub.HeigthCD == null || banque.GetEPub.HeigthCD == 0) banque.GetEPub.HeigthCD = 350;
            if (banque.GetEPub.WidhtCG == null || banque.GetEPub.WidhtCG == 0) banque.GetEPub.WidhtCG = 450;
            if (banque.GetEPub.HeigthCG == null || banque.GetEPub.HeigthCG == 0) banque.GetEPub.HeigthCG = 350;
            if (banque.GetEPub.DelaitAttCD == null || banque.GetEPub.DelaitAttCD == 0) banque.GetEPub.DelaitAttCD = 10000;
            if (banque.GetEPub.DelaitAttCG == null || banque.GetEPub.DelaitAttCG == 0) banque.GetEPub.DelaitAttCG = 10000;
            return View(banque.GetEPub);
        }

        // POST: ePubs/Edit/5
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ePub ePub)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var strId = Session["IdStructure"];
                    var banqueId = db.Structures.FindAsync(strId).Result.BanqueId(db);
                    var banque = await db.GetBanques.FindAsync(banqueId);
                    var model = banque.GetEPub;
                    model.Etat = ePub.Etat;
                    model.CardLeft = ePub.CardLeft;
                    model.CardRigth = ePub.CardRigth;
                    model.CardTop = ePub.CardTop;
                    model.PostionCarteLeftV = ePub.PostionCarteLeftV;
                    model.PostionCarteLeftH = ePub.PostionCarteLeftH;
                    model.NbrMaxAffCG = ePub.NbrMaxAffCG;
                    model.WidhtCG = ePub.WidhtCG;
                    model.HeigthCG = ePub.HeigthCG;
                    model.PostionCarteRigthtV = ePub.PostionCarteRigthtV;
                    model.PostionCarteRigthtH = ePub.PostionCarteRigthtH;
                    model.NbrMaxAffCD = ePub.NbrMaxAffCD;
                    model.WidhtCD = ePub.WidhtCD;
                    model.HeigthCD = ePub.HeigthCD;
                    model.DelaitAttCD = ePub.DelaitAttCD;
                    model.DelaitAttCG = ePub.DelaitAttCG;
                    model.NbrPosteAffCG = ePub.NbrPosteAffCG;
                    model.NbrPosteAffCD = ePub.NbrPosteAffCD;

                    model.CardBottom = ePub.CardBottom;
                    db.Entry(model).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    //return RedirectToAction("Index");
                }
                catch (Exception)
                {}
            }
            return View(ePub);
        }

        // GET: ePubs/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ePub ePub = await db.GetEPubs.FindAsync(id);
            if (ePub == null)
            {
                return HttpNotFound();
            }
            return View(ePub);
        }

        // POST: ePubs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ePub ePub = await db.GetEPubs.FindAsync(id);
            db.GetEPubs.Remove(ePub);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (Session != null)
            {
                var url = (string)Session["urlaccueil"];
                filterContext.Result = Redirect(url);
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
