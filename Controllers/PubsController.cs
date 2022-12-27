using genetrix.Models;
using genetrix.Models.Fonctions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace genetrix.Controllers
{
    public class PubsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Bg
        public ActionResult Index()
        {
            return View("~/Views/Bg/Index.cshtml");
        }

        public ActionResult epub()
        {
            return View("~/Views/Pubs/Index.cshtml");
        }

        public ActionResult IndexThemes()
        {
            ViewBag.navigation = "epub";
            ViewBag.navigation_msg = "Publications";
            return View();
        }

        public ActionResult ThemeInterne(int? rg,string ps)
        {
            var epub = db.GetEPubs.FirstOrDefault();

            if (epub != null && epub.Etat == 1)
            {
                var models = new List<int>();
                try
                {
                    if (epub.CardLeft && epub.CardRigth)
                    {
                        if (epub.CardLeft && ps == "gch")
                        {
                            ViewBag.nbrMax = epub.NbrMaxAffCG;
                            models.Add(db.PubItems.First(p => p.Acive && p.ePubItemType == ePubItemType.CarteGauche).Id);
                        }
                        else if (epub.CardRigth && ps == "drt")
                        {
                            ViewBag.nbrMax = epub.NbrMaxAffCD;
                            models.AddRange(db.PubItems.Where(p => p.Acive && p.ePubItemType == ePubItemType.CarteDroite).Select(p => p.Id));
                        }
                    }
                    else
                    {
                        if (epub.CardLeft && ps == "gch")
                        {
                            models.Add(db.PubItems.First(p => p.Acive).Id);
                            ViewBag.nbrMax = epub.NbrMaxAffCG;
                        }
                        else if (epub.CardRigth && ps == "drt")
                        {
                            models.AddRange(db.PubItems.Where(p => p.Acive).Select(p => p.Id));
                            ViewBag.nbrMax = epub.NbrMaxAffCD;
                        }
                    }
                }
                catch (Exception)
                { }
                ViewBag.ps = ps;
                ViewBag.itemsId = models;
                return View("ThemeInterne" + rg,epub);
            }
            return View("ThemeInterne"+rg);
        }
        
        public ActionResult CardDroite(char ps)
        {
            return PartialView();
        }

        public ActionResult CardGauche(char ps)
        {
            return PartialView();
        }

        public ActionResult GetPubItem(char ps)
        {
            return PartialView("");
        }

        public ActionResult GetPubCard(int? id,int predur=5000)
        {
            PubItem pub = null;
            ViewBag.prt_dur = predur;
            if (id !=null)
                pub = db.PubItems.Find(id);
            else
                pub = db.PubItems.FirstOrDefault(p=>p.EstLibre);
            string nom = "";
            if (pub == null) return null;
            nom = "~/Views/Pubs/Items/" + pub.Theme + ".cshtml";
            return PartialView(nom,pub);
            //return PartialView(nom,new {pub.Image,pub.Description,pub.DescriptionColor,pub.Titre,pub.TitreColor,pub.DuréeAtt});
        }

        public async Task<ActionResult> GetPub1()
        {
            var pubs =await db.PubItems.ToListAsync();
            return Json(from p in pubs select new{p.Id,p.Theme}, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult GetPub()
        {
            var pubs = db.PubItems.Where(p=>p.Acive && p.eType==1).ToList();
            return Json(from p in pubs select new{p.Id,p.Theme,p.DureeApp}, JsonRequestBehavior.AllowGet);
        }
        
        
        public ActionResult Themes(int? idPubItem=null,string themeName="",string cards="",int? width=null,int? heigth=null,string desc="",string titre=""
            ,string titreColor= "",string descriptionColor = "",string fondColor= "",string frameId= "")
        {
            var epub = db.GetEPubs.FirstOrDefault();
            if (epub!=null && epub.Etat==1)
            {
                var model = new PubItem() { };
                try
                {
                    if (idPubItem != null && idPubItem != 0)
                    {
                        model = db.PubItems.Find(idPubItem);
                        model.Heigth = model.Heigth != null && model.Heigth == 0 ? heigth : 0;
                        model.Width = model.Width != null && model.Width == 0 ? width : 0;
                    }
                    else
                    {
                        model.Theme = themeName;
                        model.Width = width;
                        model.Heigth = heigth;
                        model.Description = desc;
                        model.Titre = titre;
                        model.TitreColor = string.IsNullOrEmpty(titreColor) ? "" : titreColor.Replace("-", "#");
                        model.DescriptionColor = string.IsNullOrEmpty(descriptionColor) ? "" : descriptionColor.Replace("-", "#");
                        model.FondColor = fondColor;
                    }
                    model.Cards = cards;
                    model.FrameId = frameId;
                    //if (string.IsNullOrEmpty(cards) && string.IsNullOrEmpty(themeName))
                    //{
                    //    model.Cards = themeName + ";aa";
                    //    return View("~/Views/Pubs/Themes2.cshtml", model);
                    //}
                    //if (string.IsNullOrEmpty(cards))
                       model.Cards = themeName;
                }
                catch (Exception)
                {}
                return View(model); 
            }
            return null;
        }
         public async Task<ActionResult> Themes1(int? idPubItem=null,string themeName="",string cards="",int? width=null,int? heigth=null,string desc="",string titre=""
            ,string titreColor= "",string descriptionColor = "",string fondColor= "")
        {
            var epub =await db.GetEPubs.FirstOrDefaultAsync();
            if (epub!=null && epub.Etat==1)
            {
                var model = new PubItem() { };
                if (idPubItem != null && idPubItem != 0)
                    model =await db.PubItems.FindAsync(idPubItem);
                else
                {
                    model.Theme = themeName;
                    model.Width = width;
                    model.Heigth = heigth;
                    model.Description = desc;
                    model.Titre = titre;
                    model.TitreColor = string.IsNullOrEmpty(titreColor) ? "" : titreColor.Replace("-", "#");
                    model.DescriptionColor = string.IsNullOrEmpty(descriptionColor) ? "" : descriptionColor.Replace("-", "#");
                    model.FondColor = fondColor;
                }
                model.Cards = cards;
                return View(model); 
            }
            return null;
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (Session != null)
            {
                filterContext.Result = RedirectToAction("Index", "BlenkPage");
            }
        }

    }
}