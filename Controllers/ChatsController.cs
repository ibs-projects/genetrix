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
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using genetrix.Models.Fonctions;

namespace genetrix.Controllers
{
    public class ChatsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        DateTime dateNow;

        public ChatsController()
        {
            dateNow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "W. Central Africa Standard Time");
        }

        // GET: Chats
        public async Task<ActionResult> Index()
        {
            var chats = db.Chats.Include(c => c.Emetteur);
            return View(await chats.ToListAsync());
        }

        public ActionResult Demarrer()
        {
            var userid = Session["userId"];
            var _user = db.Users.Find(userid);
            Chat model = db.Chats.Find(_user.ChatId);
            if (model!=null)
            {
                //if ((string)Session["userType"] == "CompteBanqueCommerciale")
                if ((string)Session["userType"] == "CompteBanqueCommerciale")
                {
                    return RedirectToAction("Details", new { id = model.ChatId });
                }
                else if ((string)Session["userType"] == "CompteClient")
                {
                    return View("Chat", model);
                }
            }
            return View();
        }

        public ActionResult Ferme(int? id,string situation="",int vueLarge=0)
        {
            var chat = db.Chats.Find(id);
            if (situation == "resolu")
                chat.Situation = Situation.Resolu;
            else if(situation == "nonresolu")
                chat.Situation = Situation.NonResolu;
            else
                chat.Situation = Situation.AucuneReponse;

            chat.DateFermeture = dateNow;
            chat.Statut = StatutChat.Fermé;
            chat.Emetteur.ChatId = null;
            db.SaveChanges();
            if (vueLarge==1)
                return RedirectToAction("Details",new { id=id});
            return null;
        }

        public ActionResult Nouveau(int? id,string topic)
        {

            var userid = (string)Session["userId"];
            var _user=db.Users.Find(userid);
            Chat model = db.Chats.Find(_user.ChatId);
            if (_user.ChatId==null || model==null)
            {
                model = db.Chats.Add(new Models.Chat()
                {
                    EmetteurId = userid,
                    DateHeure = dateNow,
                    Token = Guid.NewGuid(),
                    Situation = Situation.NonResolu,
                    Statut = StatutChat.Attente,
                    Sujet=topic
                });
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                { }
                _user.ChatId = model.ChatId;
                model.Destinataires = new List<ApplicationUser>();
                model.Destinataires.Add(_user);
                db.SaveChanges(); 
            }
            if ((string)Session["userType"] == "CompteBanqueCommerciale")
            {
                return RedirectToAction("Details",new {id=model.ChatId });
            }
            else if ((string)Session["userType"] == "CompteClient")
            {
                return View("Chat", model);
            }
            return View();
        }

        public ActionResult Suivre(int suivrepxk)
        {
            var userid = (string)Session["userId"];
            var user = db.Users.Find(userid);
            var chat = db.Chats.Find(suivrepxk);
            if(chat.Destinataires == null)chat.Destinataires=new List<ApplicationUser>();
            chat.Destinataires.Add(user);
            if(chat.Statut==StatutChat.Attente)
            chat.Statut = StatutChat.Encours;
            user.ChatId = chat.ChatId;
            db.SaveChanges();
            return RedirectToAction("Details", new {id=chat.ChatId});
        }

        public ActionResult Chat()
        {
            int? id = db.Users.Find((string)Session["userId"]).ChatId;
            var chat = db.Chats.Find(id);
            if (chat == null || chat.Statut == StatutChat.Fermé)
                return RedirectToAction("Demarrer");
            return View(chat);
        }

        [HttpPost]
        public ActionResult TelechargeFichierClient(int idchat)
        {
            int imgId = 0;string url = "";
            if (Request.Files != null)
            {
                var userId = User.Identity.GetUserId();
                //var clientId = db.GetCompteClients.Find(userId).Id;
                var fichier = Request.Files[0];
                var imageChat = new genetrix.Models.ImageChat();

                string chemin = Path.GetFileNameWithoutExtension(fichier.FileName);
                string extension = Path.GetExtension(fichier.FileName);
                chemin += extension;
                imageChat.Url = Path.Combine(CreateNewFolderDossier(idchat.ToString()), chemin);
                imageChat.IdChat = idchat;
                db.GetImageChats.Add(imageChat);
                db.SaveChanges();
                imgId = imageChat.Id;
                //url = imageChat.Url;
                fichier.SaveAs(imageChat.Url);
                chemin = null;
                extension = null;
                imageChat = null;
            }
            //url = Fonctions.ImageBase64ImgSrc(url);
            //url = $"<img src='{url}' alt='' height='60px' width='60px'/>";
            return Json(new { chatId=idchat,imageId= imgId,url},JsonRequestBehavior.AllowGet);
        }

        public ActionResult OpenImage(int idImage)
        {
            ViewBag.imagePath = Fonctions.ImageBase64ImgSrc(db.GetImageChats.Find(idImage).Url);
            
            return PartialView(); 
        }
        
        private string CreateNewFolderDossier(string idchat)
        {
            try
            {
                string projectPath = "~/EspaceClient/Chats"+idchat;
                var datejour = DateTime.Now.Ticks;
                string folderName = Path.Combine(Server.MapPath(projectPath),"_"+ datejour);
                System.IO.Directory.CreateDirectory(folderName);
                return folderName;
            }
            catch (Exception ee)
            { }
            return "";
        }

        // GET: Chats/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chat chat = await db.Chats.Include(c=>c.Emetteur).FirstOrDefaultAsync(c=>c.ChatId==id);
            if (chat == null)   
            {
                return HttpNotFound();
            }
            int entreId = 0;
            if(chat.Emetteur is CompteClient)entreId=(chat.Emetteur as CompteClient).ClientId;
            else if(chat.Emetteur is CompteBanqueCommerciale)entreId=(int)(chat.Emetteur as CompteBanqueCommerciale).IdStructure;
            var useId = (string)Session["userId"];
            var existe = chat.Destinataires.FirstOrDefault(c => c.Id == useId);
            ViewBag.estSuivi = false;
            if (existe != null)
                ViewBag.estSuivi = true;
            List<ChatVM> chatVMs = new List<ChatVM>();
            var cc = db.Chats.ToList();
            string situaName = "", situaClas = "", statuName = "", statuClas = "";
            await db.Chats.Where(c => c.EntrepriseId == entreId || c.EmetteurId==chat.EmetteurId).ForEachAsync(c =>
            {
                situaClas = c.GetSituationColor[1];
                situaName= c.GetSituationColor[0];
                statuName= c.GetStatutColor[0];
                statuClas = c.GetStatutColor[1];

                chatVMs.Add(new ChatVM() { Id = c.ChatId, Sujet = c.Sujet,StatutChat=c.Statut,Situation=c.Situation,StatutClass=statuClas,StatutName=statuName,SituationClass=situaClas,SituationName=situaName });
            });
            ViewBag.histClient = chatVMs.ToList();
            chatVMs = null;
            return View(chat);
        }

        // GET: Chats/Create
        public ActionResult Create()
        {
            ViewBag.EmetteurId = new SelectList(db.ApplicationUsers, "Id", "Nom");
            return View();
        }

        // POST: Chats/Create
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "NumeroTopic,Sujet,DateHeure,DateFermeture,Token,Statut,Situation,EmetteurId")] Chat chat)
        {
            if (ModelState.IsValid)
            {
                db.Chats.Add(chat);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.EmetteurId = new SelectList(db.ApplicationUsers, "Id", "Nom", chat.EmetteurId);
            return View(chat);
        }

        // GET: Chats/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chat chat = await db.Chats.FindAsync(id);
            if (chat == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmetteurId = new SelectList(db.ApplicationUsers, "Id", "Nom", chat.EmetteurId);
            return View(chat);
        }

        // POST: Chats/Edit/5
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,NumeroTopic,Sujet,DateHeure,DateFermeture,Token,Statut,Situation,EmetteurId")] Chat chat)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chat).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.EmetteurId = new SelectList(db.ApplicationUsers, "Id", "Nom", chat.EmetteurId);
            return View(chat);
        }

        // GET: Chats/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chat chat = await db.Chats.FindAsync(id);
            if (chat == null)
            {
                return HttpNotFound();
            }
            return View(chat);
        }

        // POST: Chats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                Chat chat = await db.Chats.FindAsync(id);
                chat.Emetteur.ChatId = null;
                chat.Emetteur = null;
                foreach (var d in chat.Destinataires)
                {
                    d.ChatId = null;
                    d.Chats = null;
                }
                chat.Destinataires = null;
                db.Chats.Remove(chat);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {}
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

                filterContext.Result = RedirectToAction("DefaultPage","Archivage2");
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
