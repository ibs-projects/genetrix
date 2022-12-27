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
using System.IO;
using genetrix.Models.Fonctions;

namespace genetrix.Controllers
{
    public class PubItemsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: PubItems
        public async Task<ActionResult> Index()
        {
            var pubItems = db.PubItems.Include(p => p.EPub);
            ViewBag.navigation = "epub";
            ViewBag.navigation_msg = "Publications";
            return View(await pubItems.ToListAsync());
        }

        // GET: PubItems/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PubItem pubItem = await db.PubItems.FindAsync(id);
            if (pubItem == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "epub";
            ViewBag.navigation_msg = "Details publication";
            return View(pubItem);
        }

        // GET: PubItems/Create
        public ActionResult Create()
        {
            ViewBag.navigation = "epub";
            ViewBag.navigation_msg = "Ajout publication";
            ViewBag.IdePub = new SelectList(db.GetEPubs, "Id", "Id");
            ViewBag.Themes = db.GetThemes.ToList();
            return View();
        }

        // POST: PubItems/Create
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Theme,DureeApp,Nom,Titre,Description,Image,IsHtml,Left,Rigth,Top,Bottom,TitreColor,DescriptionColor,IdePub,ePubItemType,Acive")] PubItem pubItem)
        {
            if (ModelState.IsValid)
            {
                var strId = Session["IdStructure"];
                var banqueId = db.Structures.FindAsync(strId).Result.BanqueId(db);
                var banque = await db.GetBanques.FindAsync(banqueId);
                pubItem.eType = pubItem.GetEtype();
                db.PubItems.Add(pubItem);
                await db.SaveChangesAsync();
                //return RedirectToAction("Edit","ePubs",new { id=0});
                return RedirectToAction("Index");
            }

            ViewBag.IdePub = new SelectList(db.GetEPubs, "Id", "Id", pubItem.IdePub);
            return View(pubItem);
        }

        // GET: PubItems/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PubItem pubItem = await db.PubItems.FindAsync(id);
            if (pubItem == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdePub = new SelectList(db.GetEPubs, "Id", "Id", pubItem.IdePub);
            ViewBag.navigation = "epub";
            ViewBag.navigation_msg = "Edition publication";
            ViewBag.Themes = db.GetThemes.ToList();
            return View(pubItem);
        }

        // POST: PubItems/Edit/5
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(PubItem pubItem)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var model = db.PubItems.Find(pubItem.Id);
                    if (Request.Files != null && !string.IsNullOrEmpty(pubItem.Image))
                    {
                        var fichier = Request.Files[0];
                        string chemin = Path.GetFileNameWithoutExtension(fichier.FileName);
                        string extension = Path.GetExtension(fichier.FileName);
                        chemin += extension;
                        model.Image = Path.Combine(CreateNewFolderDossier(pubItem.Id.ToString()), chemin);
                        fichier.SaveAs(model.Image);
                        model.Image = "~/BanqueEspace/ePub/"+model.Id+"/"+chemin;
                        //pubItem.Image = Fonctions.ImageBase64ImgSrc(pubItem.Image);
                        //db.SaveChanges();
                    }
                    model.DescriptionColor = pubItem.DescriptionColor;
                    model.ePubItemType = pubItem.ePubItemType;
                    model.eType = model.GetEtype();
                    model.IdePub = pubItem.IdePub;
                    model.TitreColor = pubItem.TitreColor;
                    model.Bottom = pubItem.Bottom;
                    model.Top = pubItem.Top;
                    model.Rigth = pubItem.Rigth;
                    model.Left = pubItem.Left;
                    model.IsHtml = pubItem.IsHtml;
                    model.Theme = pubItem.Theme;
                    model.Nom = pubItem.Nom;
                    model.Titre = pubItem.Titre;
                    model.DureeApp = pubItem.DureeApp;
                    model.DuréeAtt = pubItem.DuréeAtt;
                    model.DateDebut = pubItem.DateDebut;
                    model.DateFin = pubItem.DateFin;
                    model.HeureDebut = pubItem.HeureDebut;
                    model.HeureFin = pubItem.HeureFin;
                    model.Description = pubItem.Description;
                    model.Acive = pubItem.Acive;
                    db.Entry(model).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                catch (Exception ee)
                {}
                //return RedirectToAction("Index");
            }
            ViewBag.IdePub = new SelectList(db.GetEPubs, "Id", "Id", pubItem.IdePub);
            ViewBag.Themes = db.GetThemes.ToList();
            return View(pubItem);
        }

        private string CreateNewFolderDossier(string id)
        {
            try
            {
                string projectPath = "~/BanqueEspace/ePub";
                string folderName = Path.Combine(Server.MapPath(projectPath),""+id);
                System.IO.Directory.CreateDirectory(folderName);
                return folderName;
            }
            catch (Exception ee)
            { }
            return "";
        }

        // GET: PubItems/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PubItem pubItem = await db.PubItems.FindAsync(id);
            if (pubItem == null)
            {
                return HttpNotFound();
            }
            return View(pubItem);
        }

        // POST: PubItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            PubItem pubItem = await db.PubItems.FindAsync(id);
            db.PubItems.Remove(pubItem);
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
