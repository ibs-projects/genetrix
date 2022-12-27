using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;
using genetrix.Models;
using genetrix.Models.Fonctions;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;

namespace genetrix.Controllers
{
    public class DemoController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Demo
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> EditionDossier(int dossierId, int? apEtap=null,bool sendRapel=false)
        {
            var dossier =await db.GetDossiers
                             .Include(d => d.Site)
                            .Include(d => d.Fournisseur)
                            .Include(d => d.Client)
                            .Include(d => d.DeviseMonetaire)
                            .FirstOrDefaultAsync(d => d.Dossier_Id == dossierId);
            string duree = "", date = "",docs="";

            try
            {
                docs = dossier.GetDocsMonquants.Replace("Documents manquants:  - ", "");
                docs = duree.Replace("-", "<b/>");
            }
            catch (Exception)
            { }
            switch (apEtap)
            {
                case 0:
                    dossier.Date_Etape22 = DateTime.Now;
                    dossier.ObtDevise = DateTime.Now;
                    dossier.EtapesDosier = 23;
                    break;
                case 1:
                    if (dossier.NatureOperation == NatureOperation.Bien)
                    {
                        dossier.Date_Etape22 = DateTime.Now.AddMonths(-1);
                        dossier.ObtDevise = DateTime.Now.AddMonths(-1);
                        duree = "deux mois";
                        date = DateTime.Now.AddMonths(2).ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        dossier.Date_Etape22 = DateTime.Now.AddDays(-19);
                        dossier.ObtDevise = DateTime.Now.AddDays(-19);
                        duree = "deux semaines";
                        date = DateTime.Now.AddDays(14).ToString("dd/MM/yyyy");
                    }
                    dossier.EtapesDosier = 23;
                    //envoie un mail au client
                    SendMail(dossier.GetClientEmail,"", $"Nou vous informons de l'échéance dans {duree} ({date}) du transfert de {dossier.MontantStringDevise} en faveur du fournisseur {dossier.GetFournisseur}. Nous vous rappelons à toutes fins utiles, les documents manquants à savoir: "+docs);
                    break;
                case 2:
                    duree = ""; date="";
                    if (dossier.NatureOperation == NatureOperation.Bien)
                    {
                        dossier.Date_Etape22 = DateTime.Now.AddMonths(-2);
                        dossier.ObtDevise = DateTime.Now.AddMonths(-2);
                        duree = "un mois";
                        date = DateTime.Now.AddMonths(1).ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        dossier.Date_Etape22 = DateTime.Now.AddDays(-24);
                        dossier.ObtDevise = DateTime.Now.AddDays(-24);
                        duree = "une semaine";
                        date = DateTime.Now.AddDays(7).ToString("dd/MM/yyyy");
                    }
                    dossier.EtapesDosier = 23;
                    //envoie un mail au client
                    SendMail(dossier.GetClientEmail,"", $"Nou vous informons de l'échéance imminente ({date}) du transfert de {dossier.MontantStringDevise} en faveur du fournisseur {dossier.GetFournisseur}. Nous vous rappelons à toutes fins utiles, les documents manquants à savoir: "+docs);
                    break; 
                case 3:
                    duree = ""; date="";
                    if (dossier.NatureOperation == NatureOperation.Bien)
                    {
                        dossier.Date_Etape22 = DateTime.Now.AddMonths(-3);
                        dossier.ObtDevise = DateTime.Now.AddMonths(-3);
                        date = dossier.Date_Etape22.Value.AddMonths(3).ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        dossier.Date_Etape22 = DateTime.Now.AddDays(-31);
                        dossier.ObtDevise = DateTime.Now.AddDays(-31);
                        date = dossier.Date_Etape22.Value.AddDays(30).ToString("dd/MM/yyyy");
                    }
                    dossier.EtapesDosier = 25;
                    //envoie un mail au client
                   
                    break;
                default:
                    break;
            }
            if (sendRapel || apEtap==3)
            {
                if (dossier.Get_EnDemeure2 == "#")
                {
                    if (docs == "Aucun") docs = "";
                    docs += "<br/> Accusé de reception de la mise en demeure";
                }
                SendMail(dossier.GetClientEmail,"", $"Nou vous informons que les délais d'apurement du transfert de {dossier.MontantStringDevise} en faveur du fournisseur {dossier.GetFournisseur} sont largement dépassés. Veuillez télécharger et accuser reception le courrier de Mise en demeure. Nous vous rappelons à toutes fins utiles, les documents manquants à savoir: " + docs);

                if(dossier.Get_EnDemeure=="#")
                    SendMail(dossier.GestionnaireEmail,"", $"Nous vous informons que les délais d'apurement du transfert du client {dossier.GetClient} d'un montant de {dossier.MontantStringDevise} sont largement dépassés. Veuillez télécharger et signer le courrier de Mise en demeure.",miseendemeure:true);
            }
            db.SaveChanges();
            return RedirectToAction("Details","Dossiers", new { id=dossierId});
        }

        private void SendMail(string emailDest, string objet,string body, bool miseendemeure = false)
        {
            // Send Email with zip as attachment.
            string emailsender = ConfigurationManager.AppSettings["emailsender"].ToString();
            string emailSenderPassword = ConfigurationManager.AppSettings["emailSenderPassword"].ToString();

            var exportFilePath = this.Server.MapPath("~/mise_en_demeure.pdf");
            using (var memoryStream = new MemoryStream())
            {
                if(miseendemeure)
                    using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    try
                    {
                        //CSV
                        ziparchive.CreateEntryFromFile(exportFilePath, ".pdf");
                    }
                    catch (Exception e)
                    { }
                }
                using (MailMessage mail = new MailMessage())
                {
                    try
                    {
                        mail.Subject = objet;
                        mail.Body = body;
                        if (miseendemeure)
                            mail.Attachments.Add(new Attachment(new MemoryStream(memoryStream.ToArray()), "Document.zip"));
                        mail.From = new MailAddress(emailsender);
                        mail.To.Add(emailDest);
                        
                        mail.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = ConfigurationManager.AppSettings["smtpserver"].ToString();
                        //smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSSL"]);
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(emailsender, emailSenderPassword);
                        smtp.Port = Convert.ToInt16(ConfigurationManager.AppSettings["portnumber"]);
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.Send(mail);
                    }
                    catch (Exception e)
                    { }

                }
            }
        }
    }
}