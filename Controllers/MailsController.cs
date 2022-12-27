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
using genetrix.Models.Fonctions;
using System.Configuration;
using System.Net.Mail;
using System.IO;
using System.IO.Compression;

namespace genetrix.Controllers
{
    public class MailsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        DateTime dateNow;

        public MailsController()
        {
            dateNow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "W. Central Africa Standard Time");
        }
        // GET: Mails
        public async Task<ActionResult> IndexDossier(string st="",int? dos_filtre=null)
        {
            var adres = User.Identity.Name;
            var id = (string)Session["userId"];
            var dd = db.GetMails.ToList();
            st = string.IsNullOrEmpty(st)? "recus":st;
            ViewBag.st = st;
            //ViewBag.navigation = "_mail";
            ViewBag.navigation = "_mail";
            ViewBag.navigation_msg = "liste mails";
            List<Mail> mm = new List<Mail>();
            if(dos_filtre>0)
                mm = await db.GetMails.Where(m=>m.DossierId==dos_filtre).ToListAsync();
            else
                mm = await db.GetMails.ToListAsync();

            switch (st)
            {
                case "new":
                    return View(new List<Mail>()); 
                case "recus":
                    return View(mm.Where(m => m.AdresseDest == adres && m.AdresseEmeteur !=adres && m.DestinataireId != id && !m.Lu));
                case "envoi": 
                    return View(mm.Where(m => m.AdresseDest != adres && m.AdresseEmeteur == adres && m.Envoyé));
                case "brouil": 
                    return View(mm.Where(m => m.AdresseDest == adres  && !m.Envoyé));
                case "corb": 
                    return View(mm.Where(m => (m.AdresseDest == adres && m.AdresseEmeteur == adres) && m.Corbeille));
                default:
                    //return View(await db.GetMails.Where(m => m.AdresseDest == adres && !m.Envoyé && !m.Corbeille).ToListAsync());
                    return View(mm.Where(m => !m.Corbeille));
            }
            
        }   

        public async Task<ActionResult> Index(string st="",int? dos_filtre=null)
        {
            var adres = User.Identity.Name;
            var id = (string)Session["userId"];
            var dd = db.GetMails.ToList();
            st = string.IsNullOrEmpty(st)? "recus":st;
            ViewBag.st = st;
            foreach (var item in db.GetMails.ToList())
            {
                var vv = item;
                var hh = item.AdresseEmeteur;
            }
            //ViewBag.navigation = "_mail";
            ViewBag.navigation = "_mail";
            ViewBag.navigation_msg = "liste mails";
            List<Mail> mm = new List<Mail>();
            if(dos_filtre>0)
                mm = await db.GetMails.Include(m=>m.GetSuppressions).Where(m=>m.DossierId==dos_filtre).ToListAsync();
            else
                mm = await db.GetMails.Include(m => m.GetSuppressions).ToListAsync();
            //verification des mails supprimés par cet utilisateur
            var tmp = mm.ToList();
            foreach (var m in tmp)
                foreach (var item in m.GetSuppressions)
                    if (item.UserId == (string)Session["userId"] || item.UserEmail ==User.Identity.Name)
                        try
                        {
                            mm.Remove(m);
                        }
                        catch (Exception)
                        {}

            tmp = null;

            switch (st)
            {
                case "recus":
                    return View(mm.Where(m => m.AdresseDest == adres));
                    //return View(mm.Where(m => m.AdresseDest == adres && m.AdresseEmeteur !=adres && m.DestinataireId != id && !m.Lu));
                case "envoi": 
                    return View(mm.Where(m => m.AdresseDest != adres && m.AdresseEmeteur == adres));
                case "brouil": 
                    return View(mm.Where(m => m.AdresseDest == adres  && !m.Envoyé));
                case "corb": 
                    return View(mm.Where(m => (m.AdresseDest == adres && m.AdresseEmeteur == adres) && m.Corbeille));
                default:
                    //return View(await db.GetMails.Where(m => m.AdresseDest == adres && !m.Envoyé && !m.Corbeille).ToListAsync());
                    return View(mm.Where(m => (m.AdresseDest != adres || m.AdresseEmeteur == adres) && !m.Corbeille));
            }
            
        }   
        
        public async Task<ActionResult> IndexBanque(string st="",string dos_filtre="")
        {
            var adres = User.Identity.Name;
            var id = (string)Session["userId"];
            var dd = db.GetMails.ToList();
            st = string.IsNullOrEmpty(st)? "recus":st;
            ViewBag.st = st;
            //ViewBag.navigation = "_mail";
            ViewBag.navigation = "_mail";
            ViewBag.navigation_msg = "liste mails";
            switch (st)
            {
                case "recus":
                    return View(await db.GetMails.Where(m => m.AdresseDest == adres && m.AdresseEmeteur !=adres && m.DestinataireId != id && !m.Lu).ToListAsync());
                case "envoi": 
                    return View(await db.GetMails.Where(m => m.AdresseDest != adres && m.AdresseEmeteur == adres && m.Envoyé).ToListAsync());
                case "brouil": 
                    return View(await db.GetMails.Where(m => m.AdresseDest == adres  && !m.Envoyé).ToListAsync());
                case "corb": 
                    return View(await db.GetMails.Where(m => (m.AdresseDest == adres && m.AdresseEmeteur == adres) && m.Corbeille).ToListAsync());
                default:
                    //return View(await db.GetMails.Where(m => m.AdresseDest == adres && !m.Envoyé && !m.Corbeille).ToListAsync());
                    return View(await db.GetMails.Where(m => !m.Corbeille).ToListAsync());
            }
            
        }

        // GET: Mails/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mail mail = await db.GetMails.Include(m=>m.GetSuppressions).Include(m=>m.GetFichierMails).FirstOrDefaultAsync(m=>m.Id==id);
            if (mail == null)
            {
                return HttpNotFound();
            }

            if (mail.GetSuppressions!=null)
                foreach (var item in mail.GetSuppressions)
                    if (item.UserId == (string)Session["userId"] || item.UserEmail == User.Identity.Name)
                        return HttpNotFound();

            if (!mail.Lu)
            {
                mail.Lu = true;
                db.SaveChanges();
            }
            ViewBag.navigation = "_mail";
            ViewBag.navigation_msg = "Details mail";
            return View(mail);
        }

        // GET: Mails/Create
        public ActionResult Create()
        {
            ViewBag.navigation = "_mail";
            ViewBag.navigation_msg = "Details mail";
            return View();
        }

        // POST: Mails/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("envoi")]
        public async Task<ActionResult> Create(Mail model)
        {
            var dateNow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "W. Central Africa Standard Time");
            if (ModelState.IsValid)
            {
                // Send Email with zip as attachment.
                string emailsender = ConfigurationManager.AppSettings["emailsender"].ToString();
                string emailSenderPassword = ConfigurationManager.AppSettings["emailSenderPassword"].ToString();
                string destinataireAdres = model.AdresseDest;
                model.Date = dateNow;
                model.EmetteurNom = (string)Session["userName"];
                model.AdresseEmeteur = User.Identity.Name;
                Dossier doss =null;
                if (model.DossierId != null & model.DossierId != 0)
                    doss = db.GetDossiers.Find(model.DossierId);
                try
                {
                    model.Message = Fonctions.DecodingJS(model.Message);
                }
                catch (Exception)
                { }

                if (string.IsNullOrEmpty(model.AdresseDest))
                {
                    try
                    {
                        destinataireAdres = db.Users.Find(model.DestinataireId).Email;
                    }
                    catch (Exception)
                    {}
                }

                //Enregistrement du mail
                try
                {
                    model=db.GetMails.Add(model);
                    await db.SaveChangesAsync();
                }
                catch (Exception ee)
                { }
                bool estclient = false; int siteId = 0;
                try
                {
                    if (Session["Profile"].ToString() == "client")
                    {
                        estclient = true;
                        siteId =Convert.ToInt32(Session["clientId"]);
                    }
                    else
                        siteId = Convert.ToInt32(Session["IdStructure"]);

                    if (Request.Files != null && model.Id>0)
                        for (int i = 0; i < Request.Files.Count; i++)
                        {
                            var fichier = Request.Files[i];

                            var fichierMail = new genetrix.Models.FichierMail();
                            fichierMail.IdMail = model.Id;
                            fichierMail.Name = fichier.FileName;

                            //string chemin = Path.GetFileNameWithoutExtension(fichier.FileName);
                            //string extension = Path.GetExtension(fichier.FileName);
                            string chemin = "mail_"+fichierMail.IdMail+"_"+ fichier.FileName;
                            fichierMail.Url = Path.Combine(CreateNewFolderDossier(siteId.ToString(), estclient), chemin);
                            db.FichierMails.Add(fichierMail);
                            db.SaveChanges();
                            fichier.SaveAs(fichierMail.Url);
                            chemin = null;
                        }
                }
                catch (Exception)
                {}
                bool sendOk = false;
                model =await db.GetMails.Include(m => m.GetFichierMails).FirstOrDefaultAsync(m=>m.Id==model.Id);
                using (var memoryStream = new MemoryStream())
                {
                    using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        try
                        {
                            foreach (var file in model.GetFichierMails)
                            {
                                try
                                {
                                    ziparchive.CreateEntryFromFile(file.Url, file.Name);

                                }
                                catch (Exception e1)
                                { }
                            }
                        }
                        catch (Exception e)
                        { }
                    }

                    using (MailMessage mail = new MailMessage(emailsender, destinataireAdres))
                    {
                        try
                        {
                            if (doss != null)
                                mail.Subject = model.Objet + " : " + doss.GetClient + $"; dossier [{doss.GetFournisseur}, {doss.MontantStringDevise}]";
                            else
                                mail.Subject = model.Objet + " : " + model.EmetteurNom;
                            model.Message = MailFunctions.DecodingJS(model.Message);
                            mail.Body = model.Message;

                            mail.Attachments.Add(new Attachment(new MemoryStream(memoryStream.ToArray()), "Fichiers.zip"));
                            //mail.Attachments.Add(new Attachment(new MemoryStream(memoryStream.ToArray()), ""));

                            if (!string.IsNullOrEmpty(model.CC))
                            {
                                try
                                {
                                    foreach (var item in model.CC.Split(';'))
                                    {
                                        try
                                        {
                                            if (!string.IsNullOrEmpty(item) && item.Contains('@'))
                                                //if(nomEailCorrespondant[item].Contains('@'))
                                                mail.CC.Add(item.Trim());
                                        }
                                        catch (Exception)
                                        { }
                                    }
                                }
                                catch (Exception)
                                { }

                            }
                            mail.IsBodyHtml = true;
                            SmtpClient smtp = new SmtpClient();
                            smtp.Host = ConfigurationManager.AppSettings["smtpserver"].ToString();
                            //smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSSL"]);
                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = new NetworkCredential(emailsender, emailSenderPassword);
                            smtp.Port = Convert.ToInt16(ConfigurationManager.AppSettings["portnumber"]);
                            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                            smtp.Send(mail);
                            sendOk = true;
                        }
                        catch (Exception e)
                        { }

                    }
                    if (!sendOk)
                    {
                        try
                        {
                            string projectPath = "~/BanqueEspace/" + siteId + "/Mails";
                            if (estclient)
                                projectPath = "~/EspaceClient/" + siteId + "/Ressources/Mails";
                            string folderName = Server.MapPath(projectPath);
                            foreach (var item in model.GetFichierMails)
                            {
                                try
                                {
                                    Directory.Delete(folderName, true);
                                }
                                catch (Exception)
                                { }    
                            }
                            db.GetMails.Remove(model);
                            await db.SaveChangesAsync();
                        }
                        catch (Exception)
                        { }
                    }
                }
            }

            return RedirectToAction("Index");
        }


        public FileStreamResult GetPDF(int idDoc = 0)
        {
            #region MyRegion
            var path = "";
            var estPdf = false;
            try
            {
                FichierMail doc = db.FichierMails.Find(idDoc);
                estPdf = doc.EstPdf;
                path = doc.Url;
            }
            catch (Exception)
            { }
            #endregion
            FileStream fs = null;
            try
            {
                fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            }
            catch (Exception)
            {
                path = null;
            }
            if (string.IsNullOrEmpty(path))
            {
                path = Path.Combine(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), @"assets\images\loading-error.jpg");
                try
                {
                    fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                }
                catch (Exception e)
                {
                }
            }
            if (string.IsNullOrEmpty(path)) return null;
            if (!estPdf)
                return File(fs, "image/jpeg");
            return File(fs, "application/pdf");
        }

        private string CreateNewFolderDossier(string structuteId,bool estclient)
        {
            try
            {
                string projectPath = "~/BanqueEspace/" + structuteId + "/Mails";
                if (estclient)
                    projectPath = "~/EspaceClient/" + structuteId + "/Ressources/Mails";
                string folderName = Server.MapPath(projectPath);
                System.IO.Directory.CreateDirectory(folderName);
                return folderName;
            }
            catch (Exception ee)
            { }
            return "";
        }


        // GET: Mails/Edit/5
        public async Task<ActionResult> Edit(int? id)
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
            if (mail.GetSuppressions != null)
                foreach (var item in mail.GetSuppressions)
                    if (item.UserId == (string)Session["userId"] || item.UserEmail ==User.Identity.Name)
                        return HttpNotFound();

            ViewBag.navigation = "_mail";
            ViewBag.navigation_msg = "Edit mail";
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
            ViewBag.navigation = "_mail";
            ViewBag.navigation_msg = "Edit mail";
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
                        if ((string)Session["userType"]=="CompteAdmin")
                        {
                            db.GetMails.Remove(mail);
                        }
                        else
                        {
                            if (mail.GetSuppressions == null)
                                mail.GetSuppressions = new List<MailSupprime>();
                            mail.GetSuppressions.Add(new MailSupprime()
                            {
                                UserId = (string)Session["userId"],
                                UserEmail = User.Identity.Name,
                                Date = dateNow,
                                MailId=mail.Id
                            }) ;
                        }
                        db.SaveChanges();
                    }
                    catch (Exception ee)
                    { }
                }

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> NewMail()
        {
            if ((string)Session["userType"] == "CompteClient")
            {
                List<UserVM> userVMs = new List<UserVM>();
                var clientId=Convert.ToInt32(Session["clientId"]);
                var client = await db.GetClients.Include(c=>c.Contacts).Include(c=>c.Banques).FirstOrDefaultAsync(c=>c.Id==clientId);
                try
                {
                    foreach (var c in client.GetDefaultContacts())
                    {
                        userVMs.Add(new UserVM()
                        {
                            Id = c.Id.ToString(),
                            Email = c.Email,
                            NomComplet = c.NomComplet,
                            Phone = c.Telephone,
                            Groupe = c.Groupe
                        });
                    }foreach (var c in client.Contacts)
                    {
                        userVMs.Add(new UserVM()
                        {
                            Id = c.Id.ToString(),
                            Email = c.Email,
                            NomComplet = c.NomComplet,
                            Phone = c.Telephone,
                            Groupe = c.Groupe
                        });
                    }
                    //ViewData["corespondants"] = userVMs.GroupBy(c => c.Groupe);
                    ViewData["corespondants"] = userVMs.ToList();
                }
                catch (Exception)
                {}
            }
            else if ((string)Session["userType"] == "CompteBanqueCommerciale")
            {
                var gesId = (string)Session["userId"];
                List<UserVM> userVMs = new List<UserVM>();
                try
                {
                    
                    foreach (var c in db.GetCompteBanqueCommerciales.Find(gesId).GetDefaultContacts())
                    {
                        userVMs.Add(new UserVM()
                        {
                            Id = c.Id.ToString(),
                            Email = c.Email,
                            NomComplet = c.NomComplet,
                            Phone = c.Telephone,
                            Groupe = c.Groupe
                        });
                    }foreach (var c in db.GetContacts.Where(d=>d.IdGestionnaire== gesId))
                    {
                        userVMs.Add(new UserVM()
                        {
                            Id = c.Id.ToString(),
                            Email = c.Email,
                            NomComplet = c.NomComplet,
                            Phone = c.Telephone,
                            Groupe = c.Groupe
                        });
                    }
                    ViewData["corespondants"] = userVMs.ToList();
                }
                catch (Exception)
                { }
            }
            
            return PartialView();
        }

        // GET: Mails/Delete/5
        public async Task<ActionResult> Delete(int? id)
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
            if (mail.GetSuppressions != null)
                foreach (var item in mail.GetSuppressions)
                    if (item.UserId == (string)Session["userId"] || item.UserEmail == User.Identity.Name)
                        return HttpNotFound();

            ViewBag.navigation = "_mail";
            ViewBag.navigation_msg = "suppression mail";
            return View(mail);
        }

        // POST: Mails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int? id)
        {
            Mail mail = await db.GetMails.FindAsync(id);
            if ((string)Session["userType"]=="CompteAdmin")
            {
                db.GetMails.Remove(mail);
            }
            else
            {
                if (mail.GetSuppressions == null)
                    mail.GetSuppressions = new List<MailSupprime>();
                mail.GetSuppressions.Add(new MailSupprime()
                {
                    UserId = (string)Session["userId"],
                    UserEmail = User.Identity.Name,
                    Date = dateNow,
                    MailId = mail.Id
                });
            }
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
