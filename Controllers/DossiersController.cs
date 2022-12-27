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
using System.Drawing;
using System.IO;
using genetrix;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using genetrix.Models.Fonctions;
using System.Web.UI;
using System.Globalization;
using DocumentFormat.OpenXml.Packaging;
using System.Text.RegularExpressions;
using System.Reflection;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Diagnostics;
using DocumentFormat.OpenXml.Drawing;
using Path = System.IO.Path;
using OpenXmlPowerTools;
using System.Xml.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfDocument = PdfSharp.Pdf.PdfDocument;
using PdfObject = iTextSharp.text.pdf.PdfObject;
using System.Drawing.Imaging;
using iTextSharp.tool.xml;
using SelectPdf;
//A reference to the Generate PDF service  

namespace genetrix.Controllers
{
    [Authorize]
    public class DossiersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        DateTime dateNow;
        public string msg { get; set; }
        public DossiersController()
        {
            dateNow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "W. Central Africa Standard Time");
        }

        public async Task<ActionResult> RecapTransfert(int id,bool pdf=false,int? print=0)
        {
            if (!pdf && false)
            {
                var exportFilePath = this.Server.MapPath("~/instruction.docx");
                var dossier = await db.GetDossiers.FindAsync(id);
                SearchAndReplace(exportFilePath, dossier); 
            }
            var _dossier = await db.GetDossiers.FindAsync(id);
            ViewBag.dossier = _dossier;
            ViewBag.datejour = dateNow;
            ViewBag.impression = print;
            return View(_dossier.TransfertResume());
        }

        public async Task<FileResult> DownloadInstruction1(int id)
        {
            var dossier = await db.GetDossiers.FindAsync(id);
            dossier.DateSignInst = dateNow;
            db.SaveChanges();
            string destPath =Server.MapPath("~/InstructionModel.docx"),sourcePath=this.Server.MapPath("~/instruction.docx");

            //ChangeTextInCell(projectPath,"aaa");

            var openSett = new OpenSettings() { AutoSave = false, MaxCharactersInPart = 0 };

            try
            {
               // CopyThemeContent(this.Server.MapPath("~/instruction.docx"), projectPath);

                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(sourcePath, false, openSett))
                {
                    using (WordprocessingDocument wordDocModel = WordprocessingDocument.Open(destPath, true, openSett))
                    {
                        string docText = null;
                        using (StreamReader sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                        {
                            docText = sr.ReadToEnd();
                        }
                        //DocumentFormat.OpenXml.Wordprocessing.Body body = wordDoc.MainDocumentPart.Document.Body.Clone() as DocumentFormat.OpenXml.Wordprocessing.Body;
                        //wordDocModel.MainDocumentPart.Document.Body.Remove();
                        //wordDocModel.MainDocumentPart.Document.AddChild(body);
                        CopyThemeContent(sourcePath,destPath);

                        string val = ""; int i = 0;
                        foreach (var item in dossier.GetItemGenerationInstruction())
                        {
                            i++;
                            try
                            {
                                val = item.Value;
                                if (string.IsNullOrEmpty(item.Value))
                                    val = "";
                                //Regex regexText = new Regex(item.Key);
                                //docText = regexText.Replace(docText, val);
                                docText = docText.Replace(item.Key, val);
                            }
                            catch (Exception)
                            { }
                        }

                        using (StreamWriter sw = new StreamWriter(wordDocModel.MainDocumentPart.GetStream(FileMode.Create)))
                        {
                            sw.Write(docText);
                        }
                    }
                }

            }
            catch (Exception)
            { }
            //CReate pdf is not exist
            string pdfPath = Server.MapPath("~/EspaceClient/" + Session["clientId"] + "/Ressources/InstructionMOdel.Pdf");
            PdfDocument document = new PdfDocument();
            PdfSharp.Pdf.PdfPage page = document.AddPage();
            //Save PDF File
            document.Save(pdfPath);

            try
            {
                ConvertWordToSpecifiedFormat(Server.MapPath("~/InstructionModel.docx"), pdfPath, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatPDF);
                return File(pdfPath, "application/pdf", $"Instruction_{dossier.GetFournisseur}_{dossier.MontantStringDevise}.pdf");
            }
            catch (Exception)
            { }
            return File(pdfPath, "application/pdf", $"Instruction_{dossier.GetFournisseur}_{dossier.MontantStringDevise}.pdf");
        }

        public async Task<FileResult> DownloadInstruction2(int id)
        {
            var dossier = await db.GetDossiers.FindAsync(id);
            dossier.DateSignInst = dateNow;
            db.SaveChanges();
            string destPath =this.Server.MapPath("~/instruction.docx");
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                InstructPdfViwModel model = new InstructPdfViwModel();
                model.InitData(dossier);
                StringReader reader = new StringReader(Fonctions.HtmlToPdf(model));
                Document PdfFile = new Document(PageSize.A4);
                PdfWriter writer = PdfWriter.GetInstance(PdfFile, stream);
                PdfFile.Open();
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, PdfFile, reader);
                PdfFile.Close();
                return File(stream.ToArray(), "application/pdf", $"Instruction_{dossier.GetFournisseur}_{dossier.MontantStringDevise}.pdf");
            }
        }

        public async Task<ActionResult> DownloadInstruction(int id)
        {
            var dossier = await db.GetDossiers.FindAsync(id);
            dossier.DateSignInst = dateNow;
            db.SaveChanges();
            InstructPdfViwModel model = new InstructPdfViwModel();
            model.InitData(dossier);
            string pdf_page_size = "A4";
            PdfPageSize pageSize =
                (PdfPageSize)Enum.Parse(typeof(PdfPageSize), pdf_page_size, true);

            string pdf_orientation = "Portrait";
            PdfPageOrientation pdfOrientation =
                (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation),
                pdf_orientation, true);

            int webPageWidth = 1024;

            int webPageHeight = 0;

            // instantiate a html to pdf converter object
            HtmlToPdf converter = new HtmlToPdf();

            // set converter options
            converter.Options.PdfPageSize = pageSize;
            converter.Options.PdfPageOrientation = pdfOrientation;
            converter.Options.WebPageWidth = webPageWidth;
            converter.Options.WebPageHeight = webPageHeight;

            // create a new pdf document converting an url
            //var doc = converter.ConvertUrl(url);
            var doc = converter.ConvertHtmlString(Fonctions.HtmlToPdf(model));

            // save pdf document
            byte[] pdf = doc.Save();

            // close pdf document
            doc.Close();

            // return resulted pdf document
            FileResult fileResult = new FileContentResult(pdf, "application/pdf");
            fileResult.FileDownloadName = $"Instruction_{dossier.GetFournisseur}_{dossier.MontantStringDevise}.pdf";
            return fileResult;
        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="id">id du dossier</param>
        /// <param name="i_l_de_do">i=instruction; l=lettre d'engagement; de= déclaration; do=domiciliation</param>
        /// <returns></returns>
        public async Task<ActionResult> DownloadDocuments(int id, string i_l_de_do = ""
            , string genre = ""
            , string NonComplet = ""
            , string fonction = ""
            , string entreprise = ""
            , string banque = ""
            , string vilePays_fournisseur = ""
            , string dateLivraison = ""
            , string ville = ""
            , string dateJour = "")
        {
            var dossier = await db.GetDossiers.FindAsync(id);
            string pdf_page_size = "A4";
            PdfPageSize pageSize =
                (PdfPageSize)Enum.Parse(typeof(PdfPageSize), pdf_page_size, true);

            string pdf_orientation = "Portrait";
            PdfPageOrientation pdfOrientation =
                (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation),
                pdf_orientation, true);

            int webPageWidth = 1024;

            int webPageHeight = 0;

            // instantiate a html to pdf converter object
            HtmlToPdf converter = new HtmlToPdf();

            // set converter options
            converter.Options.PdfPageSize = pageSize;
            converter.Options.PdfPageOrientation = pdfOrientation;
            converter.Options.WebPageWidth = webPageWidth;
            converter.Options.WebPageHeight = webPageHeight;

            // create a new pdf document converting an url
            SelectPdf.PdfDocument doc = null;
            string nomdoc = "";
            switch (i_l_de_do)
            {
                case "i"://Instruction
                    nomdoc = "Instruction";
                    dossier.DateSignInst = dateNow;
                    db.SaveChanges();
                    InstructPdfViwModel model = new InstructPdfViwModel();
                    model.InitData(dossier);
                    doc = converter.ConvertHtmlString(Fonctions.HtmlToPdf(model));
                    break;
                case "l"://lettre d'engagement
                    nomdoc = "Lettre";
                    doc = converter.ConvertHtmlString(dossier.PrintLettreEngagement(genre, NonComplet, fonction, entreprise, banque, vilePays_fournisseur, dateLivraison, ville, dateJour));
                    break;
                case "de"://déclaration d'importation
                    nomdoc = "Déclaration";
                    doc = converter.ConvertHtmlString(dossier.PrintDeclaration());
                    break;
                case "do"://domiciliation d'importation
                    nomdoc = "Domiciliation";
                    doc = converter.ConvertHtmlString(dossier.PrintDomiciliation());
                    break;
                default:break;
            }
            // save pdf document
            byte[] pdf = doc.Save();

            // close pdf document
            doc.Close();

            // return resulted pdf document
            FileResult fileResult = new FileContentResult(pdf, "application/pdf");
            fileResult.FileDownloadName = $"{nomdoc}_{dossier.GetFournisseur}_{dossier.MontantStringDevise}.pdf";
            return fileResult;
        }

        // To copy contents of one package part.
        public static void CopyThemeContent(string fromDocument1, string toDocument2)
        {
            using (WordprocessingDocument wordDoc1 = WordprocessingDocument.Open(fromDocument1, false))
            using (WordprocessingDocument wordDoc2 = WordprocessingDocument.Open(toDocument2, true))
            {
                ThemePart themePart1 = wordDoc1.MainDocumentPart.ThemePart;
                ThemePart themePart2 = wordDoc2.MainDocumentPart.ThemePart;
                wordDoc2.MainDocumentPart.Document.Body.Remove();
                using (StreamReader streamReader = new StreamReader(themePart1.GetStream()))
                using (StreamWriter streamWriter = new StreamWriter(themePart2.GetStream(FileMode.Create)))
                {
                    streamWriter.Write(streamReader.ReadToEnd());
                }
            }
        }

        public async Task<FileResult> InstructionGen(int id)
        {
            MemoryStream stream = new MemoryStream();
            Dossier dossier = null;
            string clientpdfPath = "";
            try
            {
                dossier = await db.GetDossiers.FindAsync(id);
                dossier.DateSignInst = dateNow;
                db.SaveChanges();
                var projectPath = Server.MapPath("~/Instruction.pdf");
                clientpdfPath = Server.MapPath("~/EspaceClient/" + Session["clientId"] + "/Ressources/InstructionModel.pdf");

                string val = "";
                using (PdfReader reader = new PdfReader(projectPath))
                {
                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        byte[] contentBytes = reader.GetPageContent(i);
                        string contentString = PdfEncodings.ConvertToString(contentBytes, PdfObject.TEXT_UNICODE);
                        foreach (var item in dossier.GetItemGenerationInstruction())
                        {
                            try
                            {
                                val = item.Value;
                                if (string.IsNullOrEmpty(item.Value))
                                    val = "";
                                //docText = docText.Replace(item.Key, val);
                                contentString = contentString.Replace(item.Key, val);
                            }
                            catch (Exception)
                            { }
                        }
                        reader.SetPageContent(i, PdfEncodings.ConvertToBytes(contentString, PdfObject.TEXT_PDFDOCENCODING));
                    }
                    new PdfStamper(reader, new FileStream(clientpdfPath, FileMode.Create, FileAccess.Write)).Close();
                }
            }
            catch (Exception)
            {}


            //PdfDocument document = new PdfDocument();
            //PdfSharp.Pdf.PdfPage page = document.AddPage();
            ////Save PDF File
            //document.Save(clientpdfPath);
            //return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "servedFilename.docx");
            return File(clientpdfPath, "application/pdf", $"Instruction_{dossier.GetFournisseur}_{dossier.MontantStringDevise}.pdf");
        }
        
        public async Task<FileResult> InstructionGen00(int id)
        {
            MemoryStream stream = new MemoryStream();
            Dossier dossier = null;
            try
            {
                dossier = await db.GetDossiers.FindAsync(id);
                dossier.DateSignInst = dateNow;
                db.SaveChanges();
                var projectPath = Server.MapPath("~/InstructionModel.docx");

                // Copy file content to MemeoryStream via byte array
                byte[] fileBytesArray = System.IO.File.ReadAllBytes(projectPath);
                stream.Write(fileBytesArray, 0, fileBytesArray.Length);// copy file content to MemoryStream
                stream.Position = 0;
                var openSett = new OpenSettings() { AutoSave = false, MaxCharactersInPart = 0 };

                //using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(this.Server.MapPath("~/instruction.docx"), false, openSett))
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(stream, true))
                {
                    string docText = null;
                    using (StreamReader sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                    {
                        docText = sr.ReadToEnd();
                    }
                    string val = ""; int i = 0;
                    foreach (var item in dossier.GetItemGenerationInstruction())
                    {
                        i++;
                        try
                        {
                            val = item.Value;
                            if (string.IsNullOrEmpty(item.Value))
                                val = "";
                            docText = docText.Replace(item.Key, val);
                        }
                        catch (Exception)
                        { }
                    }

                    using (StreamWriter sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                    {
                        sw.Write(docText);
                    }
                    // Save changes to reflect on stream object
                    wordDoc.MainDocumentPart.Document.Save();
                    HtmlConverterSettings settings = new HtmlConverterSettings();
                    XElement html = HtmlConverter.ConvertToHtml(wordDoc, settings);

                }
            }
            catch (Exception)
            {}


            //string pdfPath = Server.MapPath("~/EspaceClient/" + Session["clientId"] + "/Ressources/InstructionMOdel.Pdf");
            //PdfDocument document = new PdfDocument();
            //Document document1 = new Document();
            //document.Save(stream);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "servedFilename.docx");
            //return File(stream, "application/pdf", $"Instruction_{dossier.GetFournisseur}_{dossier.MontantStringDevise}.pdf");
        }

        void VerySimpleReplaceText(string OrigFile, string ResultFile, string origText, string replaceText)
        {
            using (PdfReader reader = new PdfReader(OrigFile))
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    byte[] contentBytes = reader.GetPageContent(i);
                    string contentString = PdfEncodings.ConvertToString(contentBytes, PdfObject.TEXT_PDFDOCENCODING);
                    contentString = contentString.Replace(origText, replaceText);
                    reader.SetPageContent(i, PdfEncodings.ConvertToBytes(contentString, PdfObject.TEXT_PDFDOCENCODING));
                }
                new PdfStamper(reader, new FileStream(ResultFile, FileMode.Create, FileAccess.Write)).Close();
            }
        }

        public async Task<FileStreamResult> InstructionGen0(int id)
        {
            string pdfPath = "";
            try
            {
                var dossier = await db.GetDossiers.FindAsync(id);
                dossier.DateSignInst = dateNow;
                db.SaveChanges();
                var projectPath = Server.MapPath("~/InstructionModel.docx");

                var openSett = new OpenSettings() { AutoSave = false, MaxCharactersInPart = 0 };

                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(this.Server.MapPath("~/instruction.docx"), false, openSett))
                {
                    using (WordprocessingDocument wordDocModel = WordprocessingDocument.Open(projectPath, true, openSett))
                    {
                        string docText = null;
                        using (StreamReader sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                        {
                            docText = sr.ReadToEnd();
                        }
                        DocumentFormat.OpenXml.Wordprocessing.Body body = wordDoc.MainDocumentPart.Document.Body.Clone() as DocumentFormat.OpenXml.Wordprocessing.Body;
                        wordDocModel.MainDocumentPart.Document.Body.Remove();
                        wordDocModel.MainDocumentPart.Document.AddChild(body);

                        foreach (var item in dossier.GetItemGenerationInstruction())
                        {
                            try
                            {
                                Regex regexText = new Regex(item.Key);
                                docText = regexText.Replace(docText, item.Value);
                            }
                            catch (Exception)
                            { }
                        }

                        using (StreamWriter sw = new StreamWriter(wordDocModel.MainDocumentPart.GetStream(FileMode.Create)))
                        {
                            sw.Write(docText);
                        }
                    }
                }

                //CReate pdf is not exist
                pdfPath = Server.MapPath("~/EspaceClient/" + Session["clientId"] + "/Ressources/InstructionMOdel.Pdf");
                PdfDocument document = new PdfDocument();
                PdfSharp.Pdf.PdfPage page = document.AddPage();
                //Save PDF File
                document.Save(pdfPath);

            }
            catch (Exception)
            {}
            try
            {
                ConvertWordToSpecifiedFormat(Server.MapPath("~/InstructionModel.docx"), pdfPath, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatPDF);
            }
            catch (Exception)
            { }            //return File(pdfPath, "application/pdf", fileName);
            FileStream fs = null;
            try
            {
                fs = new FileStream(pdfPath, FileMode.Open, FileAccess.Read);
            }
            catch (Exception e)
            {
            }
            if (fs == null) return null;
            return File(fs, "application/pdf");
        }

        private static void ConvertWordToSpecifiedFormat(object input, object output, object format)
        {
            Microsoft.Office.Interop.Word._Application application = new Microsoft.Office.Interop.Word.Application();
            application.Visible = false;
            object missing = Missing.Value;
            object isVisible = true;
            object readOnly = false;
            Microsoft.Office.Interop.Word._Document document = application.Documents.Open(ref input, ref missing, ref readOnly, ref missing, ref missing, ref missing, ref missing,
                                    ref missing, ref missing, ref missing, ref missing, ref isVisible, ref missing, ref missing, ref missing, ref missing);

            document.Activate();
            document.SaveAs(ref output, ref format, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing);
            application.Quit(ref missing, ref missing, ref missing);
        }

        public async Task<ActionResult> InstructionGen2(int id)
        {
            var exportFilePath = this.Server.MapPath("~/instruction.docx");
            var dossier =await db.GetDossiers.FindAsync(id);
            SearchAndReplace(exportFilePath,dossier);
            //var text = GetCommentsFromDocument(exportFilePath);
            
            return View(await db.GetDossiers.FindAsync(id));
        }

        public void SearchAndReplace(string document,Dossier dossier)
        {
            var openSett = new OpenSettings() { AutoSave = false, MaxCharactersInPart = 0 };
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(document, true, openSett))
            {
                string docText = null;
                using (StreamReader sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                {
                    docText = sr.ReadToEnd();
                }

                foreach (var item in dossier.GetItemGenerationInstruction())
                {
                    try
                    {
                        Regex regexText = new Regex(item.Key);
                        docText = regexText.Replace(docText, item.Value);
                    }
                    catch (Exception)
                    {}
                }

                using (StreamWriter sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                {
                    sw.Write(docText);
                }
            }
        }

        public static string GetCommentsFromDocument(string document)
        {
            string comments = null;

            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(document, true))
            {
                MainDocumentPart mainPart = wordDoc.MainDocumentPart;
                WordprocessingCommentsPart WordprocessingCommentsPart = mainPart.WordprocessingCommentsPart;
                var WordprocessingCommentsPart2 = mainPart.WordprocessingCommentsExPart;
                var doc = mainPart.Document;
                var body = doc.Body;
                //using (StreamReader streamReader = new StreamReader(WordprocessingCommentsPart.GetStream()))
                using (StreamReader streamReader = new StreamReader(mainPart.GetStream()))
                {
                    comments = streamReader.ReadToEnd();
                }
            }
            return comments;
        }


        // GET: Dossiers1
        public ActionResult Index(string st="",string par="")
        {
            if (Session == null)
                return RedirectToAction("login", "auth");

            ViewBag.pages = "indexdosclient";

            List<Dossier> getDossiers = new List<Dossier>();
            var _user = db.GetCompteClients.Find(User.Identity.GetUserId());
            //if (VariablGlobales.Access(_user,"dossier"))
            {
                int new_mail = 0, new_msg = 0, nb_broui = 0, nb_asoum = 0, nb_soum = 0, nb_encou = 0, nb_aapure = 0, nb_echu = 0,
                    nb_analyseconformite = 0, nb_attentetransmBEAC = 0, nb_encouanalyseBEAC = 0, nb_attentecouverture = 0,
                    nb_saisieencou = 0, nb_execute = 0, nb_apure = 0, nb_archive = 0, nb_supp = 0;
                try
                {
                    new_msg = db.GetMails.Where(m => m.AdresseDest == _user.Email).Count();
                }
                catch (Exception)
                { }

                //var profileff=Session["Profile"] ;
                //var useriddd= Session["userId"];
                Session["new_mail"] = new_mail;
                ViewData["comp"] = "transfert";


                try
                {
                    var _clientId = (_user as CompteClient).ClientId;
                    db._GetDossiers(_user).Where(d => (d.EtapesDosier < 52 || d.EtapesDosier == null) && d.ClientId == _clientId).ToList().ForEach(d =>
                    {
                        switch (d.EtapesDosier)
                        {
                            case null: nb_broui++; break;
                            case 0: nb_asoum++; break;
                            case 1: nb_soum++; break;
                            case 2: nb_encou++; break;
                            case 51: nb_echu++; break;
                            case 49: nb_aapure++; break;
                            default:
                                break;
                        }
                    });
                }
                catch (Exception)
                { }

                #region MyRegion
                Session["nb_broui"] = nb_broui;
                Session["nb_asoum"] = nb_asoum;
                Session["nb_soum"] = nb_soum;
                Session["nb_encou"] = nb_encou;
                Session["nb_analyseconformite"] = nb_analyseconformite;
                Session["nb_attentetransmBEAC"] = nb_attentetransmBEAC;
                Session["nb_encouanalyseBEAC"] = nb_encouanalyseBEAC;
                Session["nb_attentecouverture"] = nb_attentecouverture;
                Session["nb_attentecouverture"] = nb_attentecouverture;
                Session["nb_saisieencou"] = nb_saisieencou;
                Session["nb_execute"] = nb_execute;

                Session["nb_echu"] = nb_echu;
                Session["nb_aapure"] = nb_aapure;
                Session["nb_apure"] = nb_apure;
                Session["nb_archive"] = nb_archive;
                Session["nb_supp"] = nb_supp;
                Session["Notifs"] = db.GetNotifications.Where(n => n.DestinataireId == _user.Id && !n.Lu);
                #endregion

                IQueryable<Dossier> dd = null;
                ViewBag.navigation = string.IsNullOrEmpty(st) ? "brouil" : st;
                try
                {
                    //1=client; 2=banque;3=admin
                    byte profile = 0;

                    if ((string)Session["userType"] == "CompteClient")
                    {
                        int clientid = Convert.ToInt32(Session["ClientId"]);
                        dd = db._GetDossiers(_user).Where(d => d.ClientId == clientid).Include(d => d.Site).Include(d => d.StatusDossiers).Include(d => d.DeviseMonetaire).Include(d => d.ReferenceExterne);//.ToList();
                    }
                    else if ((string)Session["userType"] == " CompteAdmin")
                    {
                        dd = db._GetDossiers(_user).Include(d => d.Site).Include(d => d.DeviseMonetaire).Include(d => d.ReferenceExterne);
                    }


                    switch (st)
                    {
                        case "encours":
                        case "en cours":
                            getDossiers.AddRange(dd.Where(d => d.EtapesDosier >= 1 && d.EtapesDosier<19 || d.EtapesDosier<-1).ToList());
                            st = "En cours de traitement à la banque";
                            break;
                        case "local": 
                            getDossiers.AddRange(dd.Where(d => d.EtapesDosier == null || d.EtapesDosier == 0 || d.EtapesDosier == 1).ToList());
                            st = " en attente";
                            break;
                        case "traite": 
                            getDossiers.AddRange(dd.Where(d => d.EtapesDosier>=19 && d.EtapesDosier < 22).ToList());
                            st = " traités";
                            break;
                        case "assoum":
                        case "à soumettre":
                            getDossiers.AddRange(dd.Where(d => d.EtapesDosier == null || d.EtapesDosier == 0).ToList());
                            st = "à soumettre";
                            break;
                        case "ap":
                            getDossiers.AddRange(dd.Where(d => d.EtapesDosier >=23 || d.EtapesDosier <=25).ToList());
                            st = "Les apurements";
                            ViewData["comp"] = "apurement";
                            break;
                        case "env-encours":
                            getDossiers.AddRange(dd.Where(d => d.EtapesDosier ==231 || d.EtapesDosier== 232 || d.EtapesDosier== 250).ToList());
                            st = "en cours de traitement (apurement)";
                            ViewData["comp"] = "apurement";
                            break;
                        case "app-encours":
                            getDossiers.AddRange(dd.Where(d => d.EtapesDosier==230 || d.EtapesDosier==231 || d.EtapesDosier==232 || d.EtapesDosier==250).ToList());
                            st = "en cours (apurement)";
                            ViewData["comp"] = "apurement";
                            break;
                        case "aapurer":
                        case "à apurer":
                            getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 23 ||d.EtapesDosier == 230 ||d.EtapesDosier == 231 ||d.EtapesDosier == 232 ||d.EtapesDosier == -230 || d.EtapesDosier == -231 ||  d.EtapesDosier == -232 || d.EtapesDosier == -250 || d.EtapesDosier == 22 || d.EtapesDosier == 25|| d.EtapesDosier == 250|| d.EtapesDosier == -250).ToList());
                            st = "à apurer";
                            ViewData["comp"] = "apurement";
                            break;
                        case "loc-aapurer":
                            getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 23 || d.EtapesDosier == 22).ToList());
                            ViewData["comp"] = "apurement";
                            st = "à apurer (local)";
                            break;
                        case "env-aapurer":
                            getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 230).ToList());
                            st = "à apurer (envoyés)";
                            ViewData["comp"] = "apurement";
                            break;
                        case "rej-aapurer":
                            getDossiers.AddRange(dd.Where(d => d.EtapesDosier == -230 || d.EtapesDosier == -231|| d.EtapesDosier == -232).ToList());
                            st = "à apurer (rejetés)";
                            ViewData["comp"] = "apurement";
                            break;
                        case "apure":
                        case "apuré":
                            getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 24).ToList());
                            st = "apurés";
                            ViewData["comp"] = "apurement";
                            break;
                        case "archive":
                        case "archivé":
                            getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 52 || d.EtapesDosier==50).ToList());
                            st = "archivés";
                            break;
                        case "echu":
                        case "échu":
                            getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 25).ToList());
                            st = "échus";
                            ViewData["comp"] = "apurement";
                            break; 
                        case "loc-echu":
                            getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 25).ToList());
                            st = "échus (local)";
                            ViewData["comp"] = "apurement";
                            break;
                        case "env-echu":
                            getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 25).ToList());
                            st = "échus (envoyés)";
                            ViewData["comp"] = "apurement";
                            break;  
                        case "rej-echu":
                            getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 25).ToList());
                            st = "échus (rejetés)";
                            ViewData["comp"] = "apurement";
                            break;
                        case "soumis":
                        case "recu":
                        case "reçus":
                            getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 1).ToList());
                            st = st == "recu" ? "reçus" : "soumis";
                            break;
                        case "supp":
                        case "supprimés":
                            getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 53).ToList());
                            st = "supprimés";
                            break;
                        case "env_bac":
                        case "envoyés BEAC":
                            getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 5).ToList());
                            st = "envoyés BEAC";
                            break;
                        case "aenv_bac":
                        case "à envoyer BEAC":
                            getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 8).ToList());
                            st = "à envoyer BEAC";
                            break;
                        case "accord":
                        case "accordés":
                            getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 6).ToList());
                            st = "accordés";
                            break;
                        case "encoourstf":
                        case "en cours de traitement service transfert":
                            getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 4).ToList());
                            st = "en cours de traitement service transfert";
                            break;
                        case "rej":
                        case "rejetés":
                            getDossiers.AddRange(dd.Where(d => d.EtapesDosier == -1).ToList());
                            st = "rejetés";
                            break;
                        case "all":
                            getDossiers.AddRange(db._GetDossiers(_user).ToList());
                            st = "tous";
                            break;
                        default:
                            var docc = dd.ToList();
                            if ((string)Session["userType"] != "CompteAdmin")
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier == null).ToList());
                            else getDossiers.AddRange(dd);
                            st = "au brouillon";
                            break;
                    }
                }
                catch (Exception e)
                { }
                dd = null;
                if (!string.IsNullOrEmpty(par))
                    try
                    {
                        switch (par)
                        {
                            case "bank":
                                getDossiers = getDossiers.OrderBy(b => b.Site).ToList();
                                break;
                            case "ddepot":
                                getDossiers = getDossiers.OrderBy(b => b.DateDepotBank).ToList();
                                break;
                            case "fourn":
                                getDossiers = getDossiers.OrderBy(b => b.Fournisseur).ToList();
                                break;
                            default:
                                break;
                        }
                    }
                    catch (Exception)
                    { } 
            }
            ViewBag.statut = st.ToString();
            var client = db.GetClients.Find(_user.ClientId);
            ViewBag.ModeRestraint = client.ModeRestraint;
            client = null;
            return View(getDossiers.ToList());
        }

        // GET: Dossiers1/Details/5
        public async Task<ActionResult> Details(int? id, string msg)
        {
            if (id == null || id == 0)
            {
                throw new Exception();
            }
            var user1 = db.GetCompteClients.Find(Session["userId"]);
            try
            {
                ViewBag.SoumettreDossier = user1.SoumettreDossier;
                ViewBag.CreerDossier = user1.CreerDossier;
            }
            catch (Exception)
            { }
            var client = db.GetClients.Find(user1.ClientId);
            ViewBag.ModeRestraint = client.ModeRestraint;
            client = null;
            try
            {
                Dossier dossier = null; ;
                dossier = await db.GetDossiers
                            .Include(d => d.StatusDossiers)
                            .Include(d => d.DocumentTransport)
                               .Include(d => d.QuittancePay)
                               .Include(d => d.LettreEngage)
                               .Include(d => d.DomicilImport)
                               .Include(d => d.DeclarImport)
                               .Include(d => d.ReferenceExterne)
                               .Include(d => d.StatusDossiers)
                               .Include(d => d.GetImageInstructions)
                           .FirstAsync(d => d.Dossier_Id == id);

                if (dossier == null)
                {
                    return RedirectToAction("Index");
                }
                ViewBag.navigation = "rien";
                ViewBag.navigation_msg = "Details dossier ";

                if (Session== null) return RedirectToAction("indexbanque", "index");
                if ((Session["Profile"].ToString()=="banque") && dossier.EtapesDosier==1)
                {
                    var structure = db.Structures.Find(Session["IdStructure"]);
                    var idbanque = structure.BanqueId(db);
                    await MailFunctions.ChangeEtapeDossier((int)idbanque, (string)Session["userId"], 2, dossier, db, structure);
                }
                if (dossier.EtapesDosier==22)
                {
                    var ges = db.GetCompteBanqueCommerciales.Find(dossier.GestionnaireId);
                    var structure = db.Structures.Find(ges.IdStructure);

                    var idbanque = structure.BanqueId(db);
                    await MailFunctions.ChangeEtapeDossier((int)idbanque, dossier.GestionnaireId, 23, dossier, db, structure);
                }
                
                ViewBag.msg = msg;
                try
                {
                    Dictionary<int, string> structures = new Dictionary<int, string>();
                    foreach (var item in db.Structures)
                    {
                        try
                        {
                            structures.Add(item.Id, item.Nom);
                        }
                        catch (Exception)
                        {}
                    }
                    ViewBag.structures = structures;
                }
                catch (Exception)
                {}
                ViewBag.pages = "detailsdosclient";
                ViewBag.comp = dossier.Apurement ? "Apurement" : "Transfert";
                return View(dossier);
            }
            catch (Exception ee)
            { }
            
            return RedirectToAction("Index",new { msg=msg});
        }

        // GET: Dossiers1/Create
        public ActionResult Create()
        {
            if (Session == null) return RedirectToAction("Login", "Account");

            //Restriction
            var user = db.GetCompteClients.Find(Session["userId"]);
            var client = db.GetClients.Find(user.ClientId);
            try
            {
                ViewBag.SoumettreDossier = user.SoumettreDossier;
                if (!user.CreerDossier && client.ModeRestraint)
                    return RedirectToAction("Index", "Index",new { msg= "Vous ne pouvez pas créer un dossier, Veuillez contacter votre administrateur" });
            }
            catch (Exception)
            {}
            user = null;
            int _clientId = Convert.ToInt32(Session["clientId"]);
            ViewBag.BanqueId = (from b in db.GetBanqueClients
                              where b.ClientId==_clientId
                               select b.Site).ToList();
            List<Agence> agences = new List<Agence>();
            var gesSite = "";
            var bankSite = 0;
            //Client client = null;
            //try
            //{
            //    client = db.GetClients.Find(_clientId);
            //}
            //catch (Exception)
            //{}
            if (client == null) client = new Client();
            db.GetBanqueClients.Where(b=> b.ClientId == _clientId).ToList().ForEach(b =>
            {
                try
                {
                    bankSite = b.IdSite;
                    agences.Add(b.Site);
                    gesSite = b.Gestionnaire.Structure.Nom;
                }
                catch (Exception)
                {}
            });

            ViewBag.BanqueId = agences;
            ViewBag.IdBank = bankSite;
            ViewBag.agence = gesSite;
            agences = null;

            ViewBag.DeviseMonetaireId = (from b in db.GetDeviseMonetaires select b).ToList();
            ViewBag.ReferenceExterneId = new SelectList(db.GetReferenceBanques, "Id", "NumeroRef");
            ViewBag.FournisseurId = (from b in db.GetFournisseurs
                                     where b.ClientId ==_clientId
                                     select b).ToList();
           // ViewBag.ClientId = new SelectList(db.GetClients.Where(c=>c.Id==_clientId), "Id", "Nom");
            ViewBag.ClientId = new SelectList(new List<Client>(){client }, "Id", "Nom");

            var model = new Dossier()
            {
                ApplicationUser=User.Identity.Name,
                DateCreationApp=dateNow,
                NbreJustif=0,
                RefInterne ="XXX",
                ClientId= _clientId
            };
            ViewBag.pages = "createdosclient";
            try
            {
                //ViewBag.NumCompteClient = db.GetClients.Find(_clientId).GetNumComptes();
                ViewBag.NumCompteClient = client.GetNumComptes();
            }
            catch (Exception)
            {}
            try
            {
                ViewBag.motifs = db.ModifsTransferts.Select(m=>m.Intitule);
            }
            catch (Exception)
            {}
            try
            {
                ViewBag.pays = client.Pays ;
            }
            catch (Exception)
            {}
            return View(model);
        }

        // POST: Dossiers1/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<ActionResult> Create([Bind(Include = "IdSite,ContreValeurXAF,Montant,FournisseurId,DeviseMonetaireId,NbreJustif,EtapesDosier,ClientId")] Dossier dossier, HttpPostedFileBase ImageInstruction=null,
        public async Task<ActionResult> Create(FormCollection form)
        {
            Dossier dossier = null;
            //if (ModelState.IsValid)
            {
                try
                {
                    switch (form["nature"])
                    {
                        case "bien":
                            dossier = new Bien();
                            break;
                        case "service":
                            dossier = new Service();
                            break;
                        default:
                            dossier.NatureOperation = NatureOperation.Nulle;
                            break;
                    }
                    dossier.DateDepotBank = default;
                    dossier.DateModif = dateNow;
                    dossier.DateSignInst = dateNow;
                    dossier.DateCreationApp = dateNow;
                    if (dossier.Montant % 1 > 0)
                        dossier.Montant = Math.Round(dossier.Montant,2);
                    int idSite = Convert.ToInt32(form["IdSite"]);
                    var banqueId = db.Structures.Find(idSite).BanqueId(db);
                    //var idbank00 = form.GetValue("IdSite");
                    var bank = db.GetBanques.Find(banqueId);
                    if (dossier.MontantCV <= bank.MontantDFX)
                        dossier.Categorie = 0;
                    else dossier.Categorie = 1;
                    dossier.IdSite = idSite;
                    dossier.Client = (db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name) as CompteClient).Client;

                }
                catch (Exception e)
                {}
                try
                {
                    var banqueBen = db.CompteBeneficiaires.Find(dossier.NumCompteBenef);
                    dossier.PaysBanqueBenf = banqueBen.Pays;
                    dossier.CodeAgence = banqueBen.CodeAgence;
                    dossier.Cle = banqueBen.Cle;
                    dossier.NomBanqueBenf = banqueBen.NomBanque;
                    dossier.NumBanqueBenf = banqueBen.Numero;
                    dossier.NumBanqueBenf = banqueBen.Numero;
                }
                catch (Exception)
                {}
                try
                {
                    if(form["nature"]=="bien")
                        dossier = db.Biens.Add(dossier as Bien);
                    else
                        dossier = db.Services.Add(dossier as Service);
                    db.SaveChanges();
                }
                catch (Exception ee)
                {}

                if(dossier.Dossier_Id>0)
                return RedirectToAction("Edit",new { id=dossier.Dossier_Id});
            }

            return RedirectToAction("Create");
        }
        
        public ActionResult Carte(int id)
        {
            Dossier doss = null;
            try
            {
                doss = db.GetDossiers.Find(id);
            }
            catch (Exception)
            {}
            try
            {
                if(doss==null)
                doss = new Dossier();
            }
            catch (Exception)
            { }
            if(doss.Apurement)
                return PartialView("~/Views/Dossiers/Carte_apurement.cshtml",doss);
            return PartialView(doss);
        }
        
        private string CreateNewFolderDossier(string clientId,string intituleDossier)
        {
            try
            {
                string projectPath = "~/EspaceClient/" + clientId + "/Ressources/Transferts";
                string folderName = System.IO.Path.Combine(Server.MapPath(projectPath), intituleDossier);
                System.IO.Directory.CreateDirectory(folderName);
                return folderName;
            }
            catch (Exception ee)
            { }
            return "";
        }

        [HttpPost]
        public async Task<ActionResult> ModifEtatDossier(FormCollection form)
        {
            var idDossier = int.Parse(form["IdDossier"]);
            var etat = int.Parse(form["EtapesDosier"]);
            if (idDossier>0)
            {
                var structure = db.Structures.Find(Session["IdStructure"]);
                var idbanque = structure.BanqueId(db);

                var doss = db.GetDossiers.Find(idDossier);
                var user = db.GetCompteBanqueCommerciales.Include(u=>u.Structure).FirstOrDefault(u=>u.Id==(string)Session["userId"]);
                await MailFunctions.ChangeEtapeDossier((int)idbanque, (string)Session["userId"], etat,doss,db, user.Structure);
                doss.DateModif = dateNow;
                user = null;
            }
            
            return RedirectToAction("Details", new { id= idDossier });
        }
        
        [HttpPost]
        public ActionResult AddDouane(FormCollection form, HttpPostedFileBase ImageDouane = null)
        {
            var idDossier = int.Parse(form["IdDossier"]);

            if (idDossier>0)
            {
                var dossier = db.GetDossiers.Find(idDossier);
                dossier.DateModif = dateNow;

                //Douane
                if (ImageDouane != null)
                {
                    if (!string.IsNullOrEmpty(ImageDouane.FileName))
                    {
                        var imageModel = new genetrix.Models.ImageDocumentAttache();
                        imageModel.Titre = "Douane_" + dossier.Intitulé;

                        string chemin = System.IO.Path.GetFileNameWithoutExtension(ImageDouane.FileName);
                        string extension = System.IO.Path.GetExtension(ImageDouane.FileName);
                        chemin = imageModel.Titre + extension;
                        imageModel.Url = System.IO.Path.Combine(CreateNewFolderDossier(dossier.ClientId.ToString(), dossier.Intitulé), chemin);
                        if (dossier.GetImageInstructions != null && dossier.GetImageInstructions.Count > 0)
                        {
                            //Supprime l'ancienne image
                            System.IO.File.Delete(dossier.GetImageInstructions.ToList()[0].Url);
                        }
                        else
                            dossier.GetImageInstructions = new List<genetrix.Models.ImageInstruction>();

                        imageModel.NomCreateur = User.Identity.Name;

                        //Document attaché
                        var docAttache = new DocumentAttache();
                        docAttache.Nom = "Douane";
                        ////docAttache.ClientId = User.Identity.GetUserId();
                        docAttache.GetImageDocumentAttaches = new List<ImageDocumentAttache>();
                        docAttache.GetImageDocumentAttaches.Add(imageModel);
                        dossier.DeclarImport = docAttache;
                        docAttache.Dossier = dossier;

                        chemin = null;
                        extension = null;

                        db.GetDocumentAttaches.Add(docAttache);
                        db.GetImageDocumentAttaches.Add(imageModel);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception)
                        { }
                        ImageDouane.SaveAs(imageModel.Url);

                    }
                }
            }
            
            return RedirectToAction("Details", new { id= idDossier });
        }

        // GET: Dossiers1/Edit/5
        public async Task<ActionResult> Edit(int? id,string msg,int etapeData = 1 )
        {
            if (id == null || id==0)
            {
                throw new Exception();
            }
            var user = db.GetCompteClients.Find(Session["userId"]);
            try
            {
                ViewBag.SoumettreDossier = user.SoumettreDossier;
                ViewBag.CreerDossier = user.CreerDossier;
            }
            catch (Exception)
            { }
            user = null;
            try
            {
                Dossier dossier = await db.GetDossiers
                       .FirstAsync(d => d.Dossier_Id == id);
                if (dossier == null)
                {
                    return HttpNotFound();
                }

                //droits d'edition
                if (dossier.EtapesDosier != null && dossier.EtapesDosier != 0 && dossier.EtapesDosier != -1 && dossier.EtapesDosier != 23 && dossier.EtapesDosier != -230 && dossier.EtapesDosier != -231 && dossier.EtapesDosier != -232 && dossier.EtapesDosier != 25 && dossier.EtapesDosier != -250 || !((string)Session["userType"] == "CompteClient") || (Convert.ToInt32(Session["clientId"]) !=dossier.ClientId))
                    return RedirectToAction("Details", new { id = dossier.Dossier_Id, msg = "Impossible d'éditer ce dossier! Veuillez contacter votre gestionnaire, car vous n'avez pas les droits d'édition sur le dossier." });

                if (Session == null) return RedirectToAction("Login", "Account");
                int _clientId = 0;
                if (Session["Profile"].ToString()=="banque")
                    _clientId = dossier.ClientId;
                else
                    _clientId = Convert.ToInt32(Session["clientId"]);
                //ViewBag.DeviseMonetaireId = (from b in db.GetDeviseMonetaires select b).ToList();
                ViewBag.BanqueId = db.GetBanqueClients.FirstOrDefault(b => b.ClientId == _clientId).Id;
                ViewBag.DeviseMonetaireId = (from b in db.GetDeviseMonetaires select b).ToList();

                ViewBag.FournisseurId = (from b in db.GetFournisseurs.ToList()
                                         where b.ClientId == _clientId
                                         select b).ToList();

                ViewBag.ImportateurId = (from b in db.Importateurs.ToList()
                                         where b.IdClient == _clientId
                                         select b).ToList();

                ViewBag.VendeurId = (from b in db.GetContacts.ToList()
                                         where b.IdClient == _clientId && b.Groupe==Groupe.Vendeur
                                         select b).ToList();

                ViewBag.msg_sauvegardeTmp = "Dossier sauvegardé temporairement. Vous pouvez le retrouver dans les brouillons !";
                ViewBag.navigation = "rien";
                ViewBag.navigation_msg = "Edition dossier ";
                ViewBag.msg = msg;
                try
                {
                    ViewBag.NumCompteClient = db.GetClients.Find(_clientId).GetNumComptes();
                }
                catch (Exception)
                { }
                try
                {
                    ViewBag.motifs = db.ModifsTransferts.Select(m => m.Intitule).ToList();
                }
                catch (Exception)
                { }

                var client = db.GetClients.Find(_clientId);
                ViewBag.ModeRestraint = client.ModeRestraint;
                dossier.BanqueToString = client.GetBanque(db);
                client = null;
                ViewBag.comp = dossier.Apurement ? "Apurement" : "Transfert";
                if (dossier is Bien)
                {
                    if(dossier.InEtapeNum && etapeData == 1)
                        return View("Bien_NumeriseDocs", dossier);
                    return View("Bien_SaisieDonnees", dossier);
                }
                else
                {
                    if (dossier.InEtapeNum && etapeData == 1)
                        return View("Service_NumeriseDocs", dossier);
                    return View("Service_SaisieDonnees", dossier);
                }
            }
            catch (Exception ee)
            {}
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditBien(Bien model)
        {
            Bien dossier = model;
            var idDossier = model.Dossier_Id;

            try
            {
                dossier = db.Biens.Include(d => d.Justificatifs).Include(d => d.Client).FirstOrDefault(d => d.Dossier_Id == idDossier);
                dossier.InEtapeNum = model.InEtapeNum;
            }
            catch (Exception)
            {}


            //droits d'edition
            if (dossier.EtapesDosier != null && dossier.EtapesDosier != 0 && dossier.EtapesDosier != -1 && dossier.EtapesDosier != 23 && dossier.EtapesDosier != -230 && dossier.EtapesDosier != -231 && dossier.EtapesDosier != -232 && dossier.EtapesDosier != 25 && dossier.EtapesDosier != -250 || !((string)Session["userType"] == "CompteClient") || (Convert.ToInt32(Session["clientId"]) != dossier.ClientId))
                return RedirectToAction("Details", new { id = dossier.Dossier_Id, msg = "Impossible d'éditer ce dossier! Veuillez contacter votre gestionnaire, car vous n'avez pas les droits d'édition sur le dossier." });

            if (ModelState.IsValid || true)
            {
                try
                {
                    if (!dossier.InEtapeNum)
                    {
                        dossier.InEtapeNum = false;
                        dossier.ModeTransport = (model as Bien).ModeTransport;
                        dossier.TypeExpedition = (model as Bien).TypeExpedition;
                        dossier.Qte = (model as Bien).Qte;
                        dossier.Unite = (model as Bien).Unite;
                        dossier.Obtention = (model as Bien).Obtention;
                        dossier.Peremption = (model as Bien).Peremption;
                        dossier.ImportateurCode = (model as Bien).ImportateurCode;
                        dossier.ServiceConnexe = (model as Bien).ServiceConnexe;
                        dossier.BureauEmbarquement = (model as Bien).BureauEmbarquement;
                        dossier.CAF = (model as Bien).CAF;
                        dossier.NomenClatureDouane = (model as Bien).NomenClatureDouane;
                        dossier.EcheancePaiement = (model as Bien).EcheancePaiement;

                        //dossier.IdSite = model.IdSite;
                        dossier.ContreValeurXAF = model.ContreValeurXAF;
                        dossier.Montant = model.Montant;
                        if (dossier.Montant % 1 > 0)
                            dossier.Montant = Math.Round(dossier.Montant, 2);
                        dossier.DeviseMonetaireId = model.DeviseMonetaireId;
                        if (model.FournisseurId > 0)
                            dossier.FournisseurId = model.FournisseurId;
                        if (model.ImportateurId > 0)
                            dossier.ImportateurId = model.ImportateurId;
                        dossier.ClientId = model.ClientId;
                        dossier.EtapesDosier = model.EtapesDosier;
                        dossier.Transmis = model.Transmis;
                        dossier.Motif = model.Motif;
                        dossier.CodeSwiftBic = model.CodeSwiftBic;
                        dossier.CodeEtsBenf = model.CodeEtsBenf;
                        dossier.MontantEnLettre = model.MontantEnLettre;
                        dossier.NumCompteBenef = model.NumCompteBenef;
                        dossier.NumCompteClient = model.NumCompteClient;
                        dossier.MarchandiseArrivee = model.MarchandiseArrivee;
                        dossier.CodeAgence = model.CodeAgence;
                        dossier.Motif = model.Motif;
                        dossier.Cle = model.Cle;
                        dossier.PaysClient = model.PaysClient;
                        //dossier.DateSignInst = model.DateSignInst;
                        dossier.EtapeValidationClient = model.EtapeValidationClient;

                        dossier.Description = model.Description;
                        dossier.ImportaAdresse = model.ImportaAdresse;
                        dossier.ImportaImmatri = model.ImportaImmatri;
                        dossier.ImportaNom = model.ImportaNom;
                        dossier.ImportaNumInscri = model.ImportaNumInscri;
                        dossier.ImportaPays = model.ImportaPays;
                        dossier.ImportaProfession = model.ImportaProfession;
                        dossier.ImportateurCodeAgr = model.ImportateurCodeAgr;
                        dossier.ImportateurMail = model.ImportateurMail;
                        dossier.ImportateurPhone = model.ImportateurPhone;
                        dossier.ImportaVille = model.ImportaVille;
                        dossier.TermeVente = model.TermeVente;
                        dossier.LieuDedouagnement = model.LieuDedouagnement;
                        dossier.PaysProv = model.PaysProv;
                        dossier.PaysOrig = model.PaysOrig;
                        dossier.RefDomiciliation = model.RefDomiciliation;
                        dossier.DateDomiciliation = model.DateDomiciliation;
                        dossier.BanqueDomi = model.BanqueDomi;
                        dossier.NumFacturePro = model.NumFacturePro;
                        dossier.DateFacturePro = model.DateFacturePro;
                        dossier.ModalReglement = model.ModalReglement;
                        dossier.ValeurFOB = model.ValeurFOB;
                        dossier.TauxChange = model.TauxChange;
                        dossier.ValeurCFA = model.ValeurCFA;
                        dossier.ValeurDevise = model.ValeurDevise;
                        dossier.PosTarif = model.PosTarif;
                        dossier.FOBDevise = model.FOBDevise;
                        dossier.TaxeInsp = model.TaxeInsp;
                        dossier.ChequeNum = model.ChequeNum;
                        dossier.ChequeDate = model.ChequeDate;
                        dossier.ChequeBanque = model.ChequeBanque;
                        dossier.ChequeMontantCFA = model.ChequeMontantCFA;
                        dossier.NumDeclaration = model.NumDeclaration;
                        dossier.DateDeclaration = model.DateDeclaration;
                        dossier.DateImport = model.DateImport;
                        try
                        {
                            db.SaveChanges();
                            var banqueBen = db.CompteBeneficiaires.FirstOrDefault(c => c.Numero == model.NumCompteBenef && c.IdFournisseur == dossier.FournisseurId);
                            dossier.PaysBanqueBenf = banqueBen.Pays;
                            dossier.RibBanqueBenf = banqueBen.Cle;
                            dossier.CodeAgence = banqueBen.CodeAgence;
                            dossier.NomBanqueBenf = banqueBen.NomBanque;
                            dossier.NumBanqueBenf = banqueBen.Numero;
                        }
                        catch (Exception e)
                        { }
                        double montantDfx = 0;
                        try
                        {
                            montantDfx = Convert.ToDouble(Session["MontantDFX"]);
                        }
                        catch (Exception)
                        { }

                        #region Restrictions
                        //if (dossier.TotalPayeFactures != dossier.Montant && dossier.Justificatifs.Count>0)
                        //{
                        //    msg = "Le montant de l'instruction doit être inférieur ou égal au montant de la facture";
                        //    return RedirectToAction("Edit", new { id = dossier.Dossier_Id, msg = msg });
                        //}
                        int _NumberDecimalDigits = 0;
                        try
                        {
                            _NumberDecimalDigits = 2;
                        }
                        catch (Exception)
                        { }
                        var mfacture = Math.Round(dossier.Justificatifs.Sum(j => j.MontantJustif), _NumberDecimalDigits);
                        var mop = Math.Round(dossier.Montant, _NumberDecimalDigits);
                        if (mfacture < mop && dossier.Justificatifs.Count > 0)
                        {
                            msg = "Le montant de l'instruction doit être inférieur ou égal au montant de la facture";
                            return RedirectToAction("Edit", new { id = dossier.Dossier_Id, msg = msg });
                        }
                        #endregion

                        //var clientname = db.GetClients.Find(dossier.ClientId).Nom;
                        if (!string.IsNullOrEmpty(model.MessageTmp))
                        {
                            string tmp = "";
                            tmp = "<h5> Client " + $"{dossier.Client.Nom} {dateNow}</h5>" + model.MessageTmp;
                            if (!string.IsNullOrEmpty(model.ItemsRaison))
                            {
                                string eltsmodif = "";
                                try
                                {
                                    foreach (var item in model.ItemsRaison.Split(';'))
                                    {
                                        eltsmodif += $"<li> {item}</li>";
                                    }
                                }
                                catch (Exception)
                                { }
                                tmp += $"<h6 style=\"margin-top:15px\">Eléménts affectés</h6>"
                                                + $"<ol>{eltsmodif}</ol>";
                            }
                            dossier.Message = tmp + "<hr/>" + dossier.Message;
                        }

                        dossier.DateModif = dateNow;
                        if (dossier.DateCreationApp == new DateTime())
                            dossier.DateCreationApp = dateNow;


                        if (dossier.EtapesDosier == -1)
                        {
                            //dossier.EtapesDosier = 0;
                        }

                        db.Entry(dossier).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                    }
                    else
                    {
                        dossier.InEtapeNum = true;
                        await db.SaveChangesAsync();
                    }
                }
                catch (Exception e)
                {}

                //action transmis
                if (dossier.Transmis == 1)
                    return RedirectToAction("Transmis",new { id=dossier.Dossier_Id});

                if (dossier==null)
                dossier = db.Biens.FirstOrDefault(d => d.Dossier_Id == idDossier);
                var info = "Le dossier peut être soumis. Les documents obligatoires sont fournis";
                var color = "warning";
                //Test si le bouton terminer a été cliqué
                if (dossier.EtapesDosier == 0)
                {
                    // Toutes les étapes doivent etre satisfait
                    if (string.IsNullOrEmpty(dossier.GetImageInstruction()) || string.IsNullOrEmpty(dossier.Get_DeclarImport)
                        || string.IsNullOrEmpty(dossier.Get_DomicilImport) || string.IsNullOrEmpty(dossier.Get_LettreEngage))
                    {
                        dossier.EtapesDosier = null;
                        db.SaveChanges();
                        color = "danger";
                        info = "Le dossier ne peut pas être soumis. Les documents obligatoires ne sont pas tous fournis";
                    }
                }
                else if (dossier.EtapesDosier == 1)
                {
                    info = $"Le dossier {dossier.ClientId} a été soumis avec succès...";
                    color = "success";
                }

                ViewData["msg"] = info;
                ViewData["color"] = color;

                if (dossier.EtapesDosier == 1 || dossier.EtapesDosier == 0)
                    return RedirectToAction("Index");
            }

            if (dossier != null && dossier.Dossier_Id > 0)
                idDossier = dossier.Dossier_Id;
            return RedirectToAction("Edit",new { id= idDossier });
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditService(Service model)
        {
            Service dossier = model;
            var idDossier = model.Dossier_Id;

            try
            {
                dossier = db.Services.Include(d => d.Justificatifs).Include(d => d.Client).FirstOrDefault(d => d.Dossier_Id == idDossier);
                dossier.InEtapeNum = model.InEtapeNum;
            }
            catch (Exception)
            {}


            //droits d'edition
            if (dossier.EtapesDosier != null && dossier.EtapesDosier != 0 && dossier.EtapesDosier != -1 && dossier.EtapesDosier != 23 && dossier.EtapesDosier != -230 && dossier.EtapesDosier != -231 && dossier.EtapesDosier != -232 && dossier.EtapesDosier != 25 && dossier.EtapesDosier != -250 || !((string)Session["userType"] == "CompteClient") || (Convert.ToInt32(Session["clientId"]) != dossier.ClientId))
                return RedirectToAction("Details", new { id = dossier.Dossier_Id, msg = "Impossible d'éditer ce dossier! Veuillez contacter votre gestionnaire, car vous n'avez pas les droits d'édition sur le dossier." });

            if (ModelState.IsValid || true)
            {
                try
                {
                    if (!dossier.InEtapeNum)
                    {
                        dossier.InEtapeNum = false;

                        //dossier.IdSite = model.IdSite;
                        dossier.DescriptionService = model.DescriptionService;
                        dossier.Chapitre = model.Chapitre;
                        dossier.ContreValeurXAF = model.ContreValeurXAF;
                        dossier.Montant = model.Montant;
                        if (dossier.Montant % 1 > 0)
                            dossier.Montant = Math.Round(dossier.Montant, 2);
                        dossier.DeviseMonetaireId = model.DeviseMonetaireId;
                        if (model.FournisseurId > 0)
                            dossier.FournisseurId = model.FournisseurId;
                        if (model.ImportateurId > 0)
                            dossier.ImportateurId = model.ImportateurId;
                        dossier.ClientId = model.ClientId;
                        dossier.EtapesDosier = model.EtapesDosier;
                        dossier.Transmis = model.Transmis;
                        dossier.Motif = model.Motif;
                        dossier.CodeSwiftBic = model.CodeSwiftBic;
                        dossier.CodeEtsBenf = model.CodeEtsBenf;
                        dossier.NumCompteBenef = model.NumCompteBenef;
                        dossier.NumCompteClient = model.NumCompteClient;
                        dossier.MarchandiseArrivee = model.MarchandiseArrivee;
                        dossier.MontantEnLettre = model.MontantEnLettre;
                        dossier.CodeAgence = model.CodeAgence;
                        dossier.Motif = model.Motif;
                        dossier.Cle = model.Cle;
                        dossier.PaysClient = model.PaysClient;
                        //dossier.DateSignInst = model.DateSignInst;
                        dossier.EtapeValidationClient = model.EtapeValidationClient;

                        dossier.Description = model.Description;
                        dossier.ImportaAdresse = model.ImportaAdresse;
                        dossier.ImportaImmatri = model.ImportaImmatri;
                        dossier.ImportaNom = model.ImportaNom;
                        dossier.ImportaNumInscri = model.ImportaNumInscri;
                        dossier.ImportaPays = model.ImportaPays;
                        dossier.ImportaProfession = model.ImportaProfession;
                        dossier.ImportateurCodeAgr = model.ImportateurCodeAgr;
                        dossier.ImportateurMail = model.ImportateurMail;
                        dossier.ImportateurPhone = model.ImportateurPhone;
                        dossier.ImportaVille = model.ImportaVille;
                        dossier.VendeurAdresse = model.VendeurAdresse;
                        dossier.VendeurFax = model.VendeurFax;
                        dossier.VendeurNom = model.VendeurNom;
                        dossier.VendeurPaysHorsCemac = model.VendeurPaysHorsCemac;
                        dossier.VendeurPhone = model.VendeurPhone;
                        dossier.VendeurVille = model.VendeurVille;
                        dossier.TermeVente = model.TermeVente;
                        dossier.LieuDedouagnement = model.LieuDedouagnement;
                        dossier.PaysProv = model.PaysProv;
                        dossier.PaysOrig = model.PaysOrig;
                        dossier.RefDomiciliation = model.RefDomiciliation;
                        dossier.DateDomiciliation = model.DateDomiciliation;
                        dossier.BanqueDomi = model.BanqueDomi;
                        dossier.NumFacturePro = model.NumFacturePro;
                        dossier.DateFacturePro = model.DateFacturePro;
                        dossier.ModalReglement = model.ModalReglement;
                        dossier.ValeurFOB = model.ValeurFOB;
                        dossier.TauxChange = model.TauxChange;
                        dossier.ValeurCFA = model.ValeurCFA;
                        dossier.ValeurDevise = model.ValeurDevise;
                        dossier.PosTarif = model.PosTarif;
                        dossier.FOBDevise = model.FOBDevise;
                        dossier.TaxeInsp = model.TaxeInsp;
                        dossier.ChequeNum = model.ChequeNum;
                        dossier.ChequeDate = model.ChequeDate;
                        dossier.ChequeBanque = model.ChequeBanque;
                        dossier.ChequeMontantCFA = model.ChequeMontantCFA;
                        dossier.NomenClatureDouane = model.NomenClatureDouane;
                        dossier.NumDeclaration = model.NumDeclaration;
                        dossier.DateDeclaration = model.DateDeclaration;
                        dossier.DateImport = model.DateImport;

                        try
                        {
                            db.SaveChanges();
                            var banqueBen = db.CompteBeneficiaires.FirstOrDefault(c => c.Numero == model.NumCompteBenef && c.IdFournisseur == dossier.FournisseurId);
                            dossier.PaysBanqueBenf = banqueBen.Pays;
                            dossier.RibBanqueBenf = banqueBen.Cle;
                            dossier.CodeAgence = banqueBen.CodeAgence;
                            dossier.NomBanqueBenf = banqueBen.NomBanque;
                            dossier.NumBanqueBenf = banqueBen.Numero;
                        }
                        catch (Exception e)
                        { }
                        double montantDfx = 0;
                        try
                        {
                            montantDfx = Convert.ToDouble(Session["MontantDFX"]);
                        }
                        catch (Exception)
                        { }

                        #region Restrictions
                        //if (dossier.TotalPayeFactures != dossier.Montant && dossier.Justificatifs.Count>0)
                        //{
                        //    msg = "Le montant de l'instruction doit être inférieur ou égal au montant de la facture";
                        //    return RedirectToAction("Edit", new { id = dossier.Dossier_Id, msg = msg });
                        //}
                        int _NumberDecimalDigits = 0;
                        try
                        {
                            _NumberDecimalDigits = 2;
                        }
                        catch (Exception)
                        { }
                        var mfacture = Math.Round(dossier.Justificatifs.Sum(j => j.MontantJustif), _NumberDecimalDigits);
                        var mop = Math.Round(dossier.Montant, _NumberDecimalDigits);
                        if (mfacture < mop && dossier.Justificatifs.Count > 0)
                        {
                            msg = "Le montant de l'instruction doit être inférieur ou égal au montant de la facture";
                            return RedirectToAction("Edit", new { id = dossier.Dossier_Id, msg = msg });
                        }
                        #endregion

                        //var clientname = db.GetClients.Find(dossier.ClientId).Nom;
                        if (!string.IsNullOrEmpty(model.MessageTmp))
                        {
                            string tmp = "";
                            tmp = "<h5> Client " + $"{dossier.Client.Nom} {dateNow}</h5>" + model.MessageTmp;
                            if (!string.IsNullOrEmpty(model.ItemsRaison))
                            {
                                string eltsmodif = "";
                                try
                                {
                                    foreach (var item in model.ItemsRaison.Split(';'))
                                    {
                                        eltsmodif += $"<li> {item}</li>";
                                    }
                                }
                                catch (Exception)
                                { }
                                tmp += $"<h6 style=\"margin-top:15px\">Eléménts affectés</h6>"
                                                + $"<ol>{eltsmodif}</ol>";
                            }
                            dossier.Message = tmp + "<hr/>" + dossier.Message;
                        }

                        dossier.DateModif = dateNow;
                        if (dossier.DateCreationApp == new DateTime())
                            dossier.DateCreationApp = dateNow;


                        if (dossier.EtapesDosier == -1)
                        {
                            //dossier.EtapesDosier = 0;
                        }

                        db.Entry(dossier).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                    }
                    else
                    {
                        dossier.InEtapeNum = true;
                        await db.SaveChangesAsync();
                    }
                }
                catch (Exception e)
                {}

                //action transmis
                if (dossier.Transmis == 1)
                    return RedirectToAction("Transmis",new { id=dossier.Dossier_Id});

                if (dossier==null)
                dossier = db.Services.FirstOrDefault(d => d.Dossier_Id == idDossier);
                var info = "Le dossier peut être soumis. Les documents obligatoires sont fournis";
                var color = "warning";
                //Test si le bouton terminer a été cliqué
                if (dossier.EtapesDosier == 0)
                {
                    // Toutes les étapes doivent etre satisfait
                    if (string.IsNullOrEmpty(dossier.GetImageInstruction()) || string.IsNullOrEmpty(dossier.Get_DeclarImport)
                        || string.IsNullOrEmpty(dossier.Get_DomicilImport) || string.IsNullOrEmpty(dossier.Get_LettreEngage))
                    {
                        dossier.EtapesDosier = null;
                        db.SaveChanges();
                        color = "danger";
                        info = "Le dossier ne peut pas être soumis. Les documents obligatoires ne sont pas tous fournis";
                    }
                }
                else if (dossier.EtapesDosier == 1)
                {
                    info = $"Le dossier {dossier.ClientId} a été soumis avec succès...";
                    color = "success";
                }

                ViewData["msg"] = info;
                ViewData["color"] = color;

                if (dossier.EtapesDosier == 1 || dossier.EtapesDosier == 0)
                    return RedirectToAction("Index");
            }

            if (dossier != null && dossier.Dossier_Id > 0)
                idDossier = dossier.Dossier_Id;
            return RedirectToAction("Edit",new { id= idDossier });
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Dossier model, HttpPostedFileBase ImageInstruction = null,
            HttpPostedFileBase ImageDeclarImport = null, HttpPostedFileBase ImageDomicilImport = null, HttpPostedFileBase ImageLettreEngage = null,
             HttpPostedFileBase ImageQuittancePay = null, HttpPostedFileBase ImageDocumentTransport = null
             )
        {
            Dossier dossier = null;
            var idDossier = model.Dossier_Id;

            try
            {
                dossier = db.GetDossiers.Include(d => d.Justificatifs).Include(d => d.Client).FirstOrDefault(d => d.Dossier_Id == idDossier);
                dossier.Transmis = model.Transmis;
                db.SaveChanges();
            }
            catch (Exception)
            {}


            //droits d'edition
            if (dossier.EtapesDosier != null && dossier.EtapesDosier != 0 && dossier.EtapesDosier != -1 && dossier.EtapesDosier != 23 && dossier.EtapesDosier != -230 && dossier.EtapesDosier != -231 && dossier.EtapesDosier != -232 && dossier.EtapesDosier != 25 && dossier.EtapesDosier != -250 || !((string)Session["userType"] == "CompteClient") || (Convert.ToInt32(Session["clientId"]) != dossier.ClientId))
                return RedirectToAction("Details", new { id = dossier.Dossier_Id, msg = "Impossible d'éditer ce dossier! Veuillez contacter votre gestionnaire, car vous n'avez pas les droits d'édition sur le dossier." });

            if (ModelState.IsValid || true)
            {
                try
                {
                    #region Numerisation

                    //instructon
                    if (ImageInstruction != null)
                    {
                        if (!string.IsNullOrEmpty(ImageInstruction.FileName))
                        {
                            var imageModel = new genetrix.Models.ImageInstruction();
                            imageModel.Titre = "Instruction";

                            string chemin = Path.GetFileNameWithoutExtension(ImageInstruction.FileName);
                            string extension = Path.GetExtension(ImageInstruction.FileName);
                            chemin = imageModel.Titre + extension;
                            imageModel.Url = Path.Combine(CreateNewFolderDossier(dossier.ClientId.ToString(), dossier.Intitulé), chemin);
                            if (dossier.GetImageInstruction() != "#" && dossier.GetImageInstructions != null)
                            {
                                //Supprime l'ancienne image
                                try
                                {
                                    var imges = dossier.GetImageInstructions;
                                    var nbr = imges.Count;

                                    for (int i = 0; i < nbr; i++)
                                    {
                                        try
                                        {
                                            var item = imges.ToList()[i];
                                            System.IO.File.Delete(item.Url);
                                            item.Url = "";
                                            if (i > 0)
                                                db.GetImageInstructions.Remove(item);
                                        }
                                        catch (Exception)
                                        { }
                                    }
                                }
                                catch (Exception e)
                                { }
                            }
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (Exception)
                            { }

                            imageModel.NomCreateur = User.Identity.Name;
                            if (dossier.GetImageInstructions != null && dossier.GetImageInstructions.Count > 0)
                            {
                                //Supprime l'ancienne image
                                try
                                {
                                    var imges = dossier.GetImageInstructions;
                                    var nbr = imges.Count;
                                    for (int i = 0; i < nbr; i++)
                                    {
                                        var item = imges.ToList()[i];
                                        try
                                        {
                                            System.IO.File.Delete(item.Url);
                                            item.Url = "";
                                        }
                                        catch (Exception)
                                        { }
                                        try
                                        {
                                            //if (i > 0)
                                            db.GetImageInstructions.Remove(item);
                                        }
                                        catch (Exception)
                                        { }
                                    }

                                }
                                catch (Exception)
                                {
                                    //forcer la suppression
                                    try
                                    {
                                        var img = db.GetImageInstructions.Find(dossier.GetImageInstructions.ToList()[0].Id);
                                        img.Url = "";
                                        db.SaveChanges();
                                    }
                                    catch (Exception)
                                    { }

                                }
                            }
                            else
                                dossier.GetImageInstructions = new List<genetrix.Models.ImageInstruction>();
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (Exception)
                            { }
                            dossier.GetImageInstructions.Add(imageModel);
                            imageModel.Dossier = dossier;
                            db.GetImageInstructions.Add(imageModel);
                            db.SaveChanges();
                            ImageInstruction.SaveAs(imageModel.Url);
                            chemin = null;
                            extension = null;
                        }
                    }

                    //Declaration d'importation
                    if (ImageDeclarImport != null)
                    {
                        if (!string.IsNullOrEmpty(ImageDeclarImport.FileName))
                        {
                            var imageModel = new genetrix.Models.ImageDocumentAttache();
                            imageModel.Titre = "Déclaration d'importation";

                            string chemin = Path.GetFileNameWithoutExtension(ImageDeclarImport.FileName);
                            string extension = Path.GetExtension(ImageDeclarImport.FileName);
                            chemin = imageModel.Titre + extension;
                            imageModel.Url = Path.Combine(CreateNewFolderDossier(dossier.ClientId.ToString(), dossier.Intitulé), chemin);
                            if (dossier.DeclarImport != null && dossier.Get_DeclarImport != "#")
                            {
                                //Supprime l'ancienne image
                                try
                                {
                                    System.IO.File.Delete(dossier.DeclarImport.GetImageDocumentAttache().Url);
                                }
                                catch (Exception)
                                { }
                            }

                            imageModel.NomCreateur = User.Identity.Name;

                            //Document attaché
                            var docAttache = new DocumentAttache();
                            docAttache.Nom = "Déclaration d'importation";
                            //docAttache.ClientId = User.Identity.GetUserId();
                            docAttache.GetImageDocumentAttaches = new List<ImageDocumentAttache>();
                            docAttache.GetImageDocumentAttaches.Add(imageModel);
                            dossier.DeclarImport = docAttache;
                            docAttache.Dossier = dossier;

                            chemin = null;
                            extension = null;

                            db.GetDocumentAttaches.Add(docAttache);
                            db.GetImageDocumentAttaches.Add(imageModel);
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (Exception)
                            { }
                            ImageDeclarImport.SaveAs(imageModel.Url);

                        }
                    }

                    //Domiciliation d'importation
                    if (ImageDomicilImport != null)
                    {
                        if (!string.IsNullOrEmpty(ImageDomicilImport.FileName))
                        {
                            var imageModel = new genetrix.Models.ImageDocumentAttache();
                            imageModel.Titre = "Domiciliation d'importation";

                            string chemin = Path.GetFileNameWithoutExtension(ImageDomicilImport.FileName);
                            string extension = Path.GetExtension(ImageDomicilImport.FileName);
                            chemin = imageModel.Titre + extension;
                            imageModel.Url = Path.Combine(CreateNewFolderDossier(dossier.ClientId.ToString(), dossier.Intitulé), chemin);
                            if (dossier.DomicilImport != null && dossier.Get_DomicilImport != "#")
                            {
                                //Supprime l'ancienne image
                                try
                                {
                                    System.IO.File.Delete(dossier.DomicilImport.GetImageDocumentAttache().Url);
                                }
                                catch (Exception)
                                { }
                            }

                            imageModel.NomCreateur = User.Identity.Name;

                            //Document attaché
                            var docAttache = new DocumentAttache();
                            docAttache.Nom = "Domiciliation d'importation";
                            //docAttache.ClientId = User.Identity.GetUserId();
                            docAttache.GetImageDocumentAttaches = new List<ImageDocumentAttache>();
                            docAttache.GetImageDocumentAttaches.Add(imageModel);
                            dossier.DomicilImport = docAttache;
                            docAttache.Dossier = dossier;

                            chemin = null;
                            extension = null;
                            db.GetDocumentAttaches.Add(docAttache);
                            db.GetImageDocumentAttaches.Add(imageModel);
                            db.SaveChanges();
                            ImageDomicilImport.SaveAs(imageModel.Url);
                        }
                    }

                    //Lettre d'engagement
                    if (ImageLettreEngage != null)
                    {
                        if (!string.IsNullOrEmpty(ImageLettreEngage.FileName))
                        {
                            var imageModel = new genetrix.Models.ImageDocumentAttache();
                            imageModel.Titre = "Lettre d'engagement";

                            string chemin = Path.GetFileNameWithoutExtension(ImageLettreEngage.FileName);
                            string extension = Path.GetExtension(ImageLettreEngage.FileName);
                            chemin = imageModel.Titre + extension;
                            imageModel.Url = Path.Combine(CreateNewFolderDossier(dossier.ClientId.ToString(), dossier.Intitulé), chemin);
                            if (dossier.LettreEngage != null && dossier.Get_LettreEngage != "#")
                            {
                                //Supprime l'ancienne image
                                try
                                {
                                    System.IO.File.Delete(dossier.LettreEngage.GetImageDocumentAttache().Url);

                                }
                                catch (Exception)
                                {
                                }
                            }

                            imageModel.NomCreateur = User.Identity.Name;

                            //Document attaché
                            var docAttache = new DocumentAttache();
                            docAttache.Nom = "Lettre d'engagement";
                            //docAttache.ClientId = User.Identity.GetUserId();
                            docAttache.GetImageDocumentAttaches = new List<ImageDocumentAttache>();
                            docAttache.GetImageDocumentAttaches.Add(imageModel);
                            dossier.LettreEngage = docAttache;
                            docAttache.Dossier = dossier;

                            chemin = null;
                            extension = null;

                            db.GetDocumentAttaches.Add(docAttache);
                            db.GetImageDocumentAttaches.Add(imageModel);
                            db.SaveChanges();
                            ImageLettreEngage.SaveAs(imageModel.Url);
                        }
                    }

                    //Quittance de paiement
                    if (ImageQuittancePay != null)
                    {
                        if (!string.IsNullOrEmpty(ImageQuittancePay.FileName))
                        {
                            var imageModel = new genetrix.Models.ImageDocumentAttache();
                            imageModel.Titre = "Quittance de paiement";

                            string chemin = Path.GetFileNameWithoutExtension(ImageQuittancePay.FileName);
                            string extension = Path.GetExtension(ImageQuittancePay.FileName);
                            chemin = imageModel.Titre + extension;
                            imageModel.Url = Path.Combine(CreateNewFolderDossier(dossier.ClientId.ToString(), dossier.Intitulé), chemin);
                            if (dossier.QuittancePay != null && dossier.Get_QuittancePay != "#")
                            {
                                //Supprime l'ancienne image
                                try
                                {
                                    System.IO.File.Delete(dossier.QuittancePay.GetImageDocumentAttache().Url);
                                }
                                catch (Exception)
                                { }
                            }

                            imageModel.NomCreateur = User.Identity.Name;

                            //Document attaché
                            var docAttache = new DocumentAttache();
                            docAttache.Nom = "Quittance de paiement";
                            //docAttache.ClientId = User.Identity.GetUserId();
                            docAttache.GetImageDocumentAttaches = new List<ImageDocumentAttache>();
                            docAttache.GetImageDocumentAttaches.Add(imageModel);
                            dossier.QuittancePay = docAttache;
                            docAttache.Dossier = dossier;

                            chemin = null;
                            extension = null;

                            db.GetDocumentAttaches.Add(docAttache);
                            db.GetImageDocumentAttaches.Add(imageModel);
                            db.SaveChanges();
                            ImageQuittancePay.SaveAs(imageModel.Url);
                        }
                    }

                    //Documents de transport
                    if (ImageDocumentTransport != null)
                    {
                        if (!string.IsNullOrEmpty(ImageDocumentTransport.FileName))
                        {
                            var imageModel = new genetrix.Models.ImageDocumentAttache();
                            imageModel.Titre = "Documents de transport";

                            string chemin = Path.GetFileNameWithoutExtension(ImageDocumentTransport.FileName);
                            string extension = Path.GetExtension(ImageDocumentTransport.FileName);
                            chemin = imageModel.Titre + extension;
                            imageModel.Url = Path.Combine(CreateNewFolderDossier(dossier.ClientId.ToString(), dossier.Intitulé), chemin);
                            if (dossier.DocumentTransport != null && dossier.Get_DocumentTransport != "#")
                            {
                                //Supprime l'ancienne image
                                System.IO.File.Delete(dossier.Get_DocumentTransport);
                                try
                                {
                                    //System.IO.File.Delete(dossier.DocumentsTransport.GetImageDocumentAttache().Url);
                                }
                                catch (Exception)
                                { }
                            }

                            imageModel.NomCreateur = User.Identity.Name;

                            //Document attaché
                            var docAttache = new DocumentAttache();
                            docAttache.Nom = "Documents de transport";
                            //docAttache.ClientId = User.Identity.GetUserId();
                            docAttache.GetImageDocumentAttaches = new List<ImageDocumentAttache>();
                            docAttache.GetImageDocumentAttaches.Add(imageModel);
                            dossier.DocumentTransport = docAttache;
                            docAttache.Dossier = dossier;

                            chemin = null;
                            extension = null;

                            db.GetDocumentAttaches.Add(docAttache);
                            db.GetImageDocumentAttaches.Add(imageModel);
                            db.SaveChanges();
                            ImageDocumentTransport.SaveAs(imageModel.Url);
                        }
                    }

                    #endregion

                }
                catch (Exception e)
                {}

                //action transmis
                if (dossier.Transmis == 1)
                    return RedirectToAction("Transmis",new { id=dossier.Dossier_Id});

                if (dossier==null)
                dossier = db.Services.FirstOrDefault(d => d.Dossier_Id == idDossier);
                var info = "Le dossier peut être soumis. Les documents obligatoires sont fournis";
                var color = "warning";
                //Test si le bouton terminer a été cliqué
                if (dossier.EtapesDosier == 0)
                {
                    // Toutes les étapes doivent etre satisfait
                    if (string.IsNullOrEmpty(dossier.GetImageInstruction()) || string.IsNullOrEmpty(dossier.Get_DeclarImport)
                        || string.IsNullOrEmpty(dossier.Get_DomicilImport) || string.IsNullOrEmpty(dossier.Get_LettreEngage))
                    {
                        dossier.EtapesDosier = null;
                        db.SaveChanges();
                        color = "danger";
                        info = "Le dossier ne peut pas être soumis. Les documents obligatoires ne sont pas tous fournis";
                    }
                }
                else if (dossier.EtapesDosier == 1)
                {
                    info = $"Le dossier {dossier.ClientId} a été soumis avec succès...";
                    color = "success";
                }

                ViewData["msg"] = info;
                ViewData["color"] = color;

                if (dossier.EtapesDosier == 1 || dossier.EtapesDosier == 0)
                    return RedirectToAction("Index");
            }

            if (dossier != null && dossier.Dossier_Id > 0)
                idDossier = dossier.Dossier_Id;
            return RedirectToAction("Edit",new { id= idDossier });
        }


        [HttpPost]
        public async Task<ActionResult> SetMiseEnDemeure(Dossier dossier,HttpPostedFileBase EnDemeure2 = null)
        {
            try
            {
                 dossier =await db.GetDossiers
                                        .Include(d => d.EnDemeure2)
                                      .FirstOrDefaultAsync(d => d.Dossier_Id == dossier.Dossier_Id);
  
                //Mise en demeure: accusé de reception
                if (EnDemeure2 != null)
                {
                    if (!string.IsNullOrEmpty(EnDemeure2.FileName))
                    {
                        var imageModel = new genetrix.Models.ImageDocumentAttache();
                        imageModel.Titre = "Accusé de reception mise en demeure ";

                        string chemin = Path.GetFileNameWithoutExtension(EnDemeure2.FileName);
                        string extension = Path.GetExtension(EnDemeure2.FileName);
                        chemin = imageModel.Titre + extension;
                        imageModel.Url = Path.Combine(CreateNewFolderDossier(dossier.ClientId.ToString(), dossier.Intitulé), chemin);
                        if (dossier.EnDemeure2 != null && dossier.Get_EnDemeure2 != "#")
                        {
                            //Supprime l'ancienne image
                            System.IO.File.Delete(dossier.Get_EnDemeure2);
                            try
                            {
                                //System.IO.File.Delete(dossier.DocumentsTransport.GetImageDocumentAttache().Url);
                            }
                            catch (Exception)
                            { }
                        }

                        imageModel.NomCreateur = User.Identity.Name;

                        //Document attaché
                        var docAttache = new DocumentAttache();
                        docAttache.Nom = "Accusé de reception mise en demeure";
                        //docAttache.ClientId = User.Identity.GetUserId();
                        docAttache.GetImageDocumentAttaches = new List<ImageDocumentAttache>();
                        docAttache.GetImageDocumentAttaches.Add(imageModel);
                        dossier.EnDemeure2 = docAttache;
                        docAttache.Dossier = dossier;

                        chemin = null;
                        extension = null;

                        db.GetDocumentAttaches.Add(docAttache);
                        db.GetImageDocumentAttaches.Add(imageModel);
                        db.SaveChanges();
                        EnDemeure2.SaveAs(imageModel.Url);
                    }
                }
            }
            catch (Exception)
            { }
            return RedirectToAction("Details",new { id=dossier.Dossier_Id});
        }

        public FileStreamResult GetPDF(int? idDossier=null,bool estDocAttache=false,bool estJustif = false,int idDoc=0)
        {
            #region MyRegion
            Dossier doss = null;
            var path = "";
            try
            {
                if(idDoc!=null)
                doss = db.GetDossiers.Include(d => d.GetImageInstructions).FirstOrDefault(d => d.Dossier_Id == idDossier);
                if (estDocAttache || idDossier == null)
                    try
                    {
                        path = db.GetDocumentAttaches.Include(d => d.GetImageDocumentAttaches).FirstOrDefault(d => d.Id == idDoc).GetImageDocumentAttache().Url;
                    }
                    catch (Exception)
                    { }
                else if (estJustif)
                    try
                    {
                        //path = db.GetJustificatifs.Include(d => d.GetImages).FirstOrDefault(d => d.Id == idDoc).GetImages.ToList()[0].Url;
                        path = db.GetImageJustificatifs.Find(idDoc).Url;
                    }
                    catch (Exception e)
                    { }
                else
                    path = doss.GetImageInstructions.ToList()[0].Url;
            }
            catch (Exception)
            {}
            //FIX ROOT PATH TO APP ROOT PATH
            //if (path.StartsWith("/"))
            //    path = path.Insert(0, "~");

            //if (!path.StartsWith("~/"))
            //    path = path.Insert(0, "~/");
            //path = VirtualPathUtility.ToAbsolute(path);

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

            return File(fs, "application/pdf");
        }

        public ActionResult Archiver(int id)
        {
            try
            {
                var doss = db.GetDossiers.Find(id);
                var clientId = Convert.ToInt32(Session["clientId"]);
                if (doss.ClientId==clientId)
                {
                    var structure = db.Structures.Find(doss.IdSite);
                    var idbanque = structure.BanqueId(db);

                    MailFunctions.ChangeEtapeDossier((int)idbanque, doss.IdGestionnaire, 26, doss, db, structure);
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {}
            return RedirectToAction("Details", new { id = id });
        }

        /// <summary>
        /// Modification de EtapeValidationClient
        /// </summary>
        /// <param name="id"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task<ActionResult> Etatx3cValid(int id,int value)
        {
            var dossier =await db.GetDossiers.FindAsync(id);
            dossier.EtapeValidationClient = (byte)value;
            db.SaveChanges();
            if (dossier.EtapeValidationClient == 2)
                return RedirectToAction("Transmis",new { id=dossier.Dossier_Id});
            return RedirectToAction("Index","Index");
        }

        public async Task<ActionResult> Transmis(int id)
        {
            if (id > 0)
            {
                //Restriction
                var user = db.GetCompteClients.Find(Session["userId"]);
                var client = db.GetClients.Find(user.ClientId);
                try
                {
                    if (!user.CreerDossier && client.ModeRestraint)
                        return RedirectToAction("Index", "Index", new { msg = "Vous ne pouvez pas soumettre un dossier, Veuillez contacter votre administrateur." });
                }
                catch (Exception)
                { }
                client = null;
                try
                {
                    var doss = db.GetDossiers.Include(d => d.Site)
                        .Include(d => d.DeclarImport)
                        .Include(d => d.DomicilImport)
                        .FirstOrDefault(d => d.Dossier_Id == id);

                    #region Restrictions
                    //Date de l'instruction <=15 jour
                    if (!doss.Apurement && (dateNow-doss.DateSignInst.Value).TotalDays>15)
                    {
                        return RedirectToAction("Edit",new { id=id,msg= "Veillez noter que l'instruction doit dater de moins de 15 jours. Ainsi nous vous invitons à insérer une version actualisée de celle-ci." });
                    }

                    //DIB obligatoire - Domiciliation
                    if (doss.DomicilImport==null)
                    {
                        return RedirectToAction("Edit",new { id=id,msg= "La domiciliation d'importation est obligatoire pour continuer l'opération." });
                    }
                    //DIB obligatoire - Declaration
                    if (doss.DeclarImport==null)
                    {
                        return RedirectToAction("Edit",new { id=id,msg= "La déclaration d'importation est obligatoire pour continuer l'opération." });
                    }

                    #endregion

                    var Banqueclient = db.GetBanqueClients.FirstOrDefault(d => d.IdSite == doss.IdSite && d.ClientId==doss.ClientId);

                    var site = db.Structures.FirstOrDefault(d => d.Id == doss.IdSite);
                    var idbanque = site.BanqueId(db);
                    doss.DateModif = dateNow;
                    doss.DateDepotBank = dateNow;
                    doss.IdGestionnaire = Banqueclient.IdGestionnaire;
                    doss.IdAgentResponsableDossier = Banqueclient.IdGestionnaire;
                    doss.IdResponsableAgence= Banqueclient.IdGestionnaire;
                    if (doss.EtapesDosier==23 ||doss.EtapesDosier==25 ||doss.EtapesDosier==-230 ||doss.EtapesDosier==-231 ||doss.EtapesDosier==-232 ||doss.EtapesDosier==230)
                    {
                       await MailFunctions.ChangeEtapeDossier((int)idbanque, (string)Session["userId"], 230, doss, db, site);
                    }
                    else
                    {
                       await MailFunctions.ChangeEtapeDossier((int)idbanque, (string)Session["userId"], 1, doss, db, site);
                    }
                }
                catch (Exception ee)
                {}
            }
            return RedirectToAction("Index","Index");
        }
        
        public ActionResult getpath(string path)
        {
            return Json(path, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Parite(string mt,int idDevise)
        {
            double _mt = 0;
            try
            {
                NumberFormatInfo provider = new NumberFormatInfo();
                provider.NumberDecimalSeparator = ".";
                provider.NumberGroupSeparator = " ";
                _mt = Convert.ToDouble(mt,provider);
            }
            catch (Exception e)
            {}
            int _NumberDecimalDigits = 0;
            try
            {
                _NumberDecimalDigits = mt.Split('.')[1].Length;
            }
            catch (Exception)
            {}
            var devise = db.GetDeviseMonetaires.Find(idDevise);
            var cv =0;
            try
            {
                cv = Convert.ToInt32(_mt * devise.ParitéXAF);
            }
            catch (Exception)
            {}
            string mts = "", cvs = "";
            NumberFormatInfo nfi = new NumberFormatInfo { NumberGroupSeparator = " ", NumberDecimalDigits= _NumberDecimalDigits };
            try
            {
                mts= _mt.ToString("n", nfi) + " " + devise.Nom;
            }
            catch (Exception)
            { }
            try
            {
                //cvs = cv.ToString("n", nfi);// + " XAF";
                cvs = cv.ToString("n");// + " XAF";
            }
            catch (Exception)
            { }
            if (devise.Nom !="EUR")
            {
                cv = 0;
                cvs = "Le cours vous sera communiqué par votre gestionnaire";
            }
            return Json(new { cv,mts,cvs}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GrapheInit(int annee,string devise)
        {
            List<GrapheVM> donneesAnnulles = new List<GrapheVM>();
            var dev = db.GetDeviseMonetaires.FirstOrDefault(d => d.Nom == devise);
            int deviseId = 0;
            if (dev == null)
                deviseId = db.GetDeviseMonetaires.FirstOrDefault().Id;
            var id = Convert.ToInt32(Session["clientId"]);
            var client = db.GetClients.Include(c => c.Banques).FirstOrDefault(c => c.Id == id);
            
            foreach (var b in client.Banques)
            {
                try
                {
                    var bank = b.Site.BanqueName(db);
                    donneesAnnulles.Append(new GrapheVM()
                    {
                        NomBanque = bank,
                        Donnees = b.GetDossierAllMonths(annee, deviseId,db)
                    });
                }
                catch (Exception)
                {}
            }
            return Json(donneesAnnulles, JsonRequestBehavior.AllowGet);
        }


        // GET: Dossiers1/Delete/5

         public async Task<ActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                throw new Exception();
            }
            Dossier dossier = await db.GetDossiers.FindAsync(id);
            if (dossier == null)
            {
                return HttpNotFound();
            }

            //Droit d'accès
            if (dossier.EtapesDosier != null && dossier.EtapesDosier != 0 && dossier.EtapesDosier != -1)
                return RedirectToAction("Details", new { id = dossier.Dossier_Id, msg = "Impossible de supprimer le dossier" });

            ViewBag.navigation = "rien";
            ViewBag.navigation_msg = "suppresion du dossier ";
            return View(dossier);
        }

        // POST: Dossiers1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                Dossier dossier = db.GetDossiers
                    .Include(d => d.DocumentTransport)
                    .Include(d => d.QuittancePay)
                    .Include(d => d.LettreEngage)
                    .Include(d => d.DomicilImport)
                    .Include(d => d.DeclarImport)
                    .FirstOrDefault(d => d.Dossier_Id == id);

                //Droit d'accès
                if (dossier.EtapesDosier != null && dossier.EtapesDosier != 0 && dossier.EtapesDosier != -1)
                    return RedirectToAction("Details", new { id = dossier.Dossier_Id, msg = "Impossible de supprimer le dossier" });


                //if (dossier.EtapesDosier<1 || dossier.EtapesDosier==null )
                {
                    //dossier.Remove(db);
                    dossier.DomicilImport = null;
                    dossier.DeclarImport = null;
                    dossier.LettreEngage = null;
                    dossier.QuittancePay = null;
                    //dossier.EtapesDosier = 27;
                    db.SaveChanges();
                    try
                    {
                        if (dossier.DeclarImport != null)
                        {
                            dossier.DeclarImport.Remove(db);
                            db.GetDocumentAttaches.Remove(dossier.DeclarImport);
                        }
                        if (dossier.DomicilImport != null)
                        {
                            dossier.DomicilImport.Remove(db);
                            db.GetDocumentAttaches.Remove(dossier.DomicilImport);
                        }
                        if (dossier.LettreEngage != null)
                        {
                            dossier.LettreEngage.Remove(db);
                            db.GetDocumentAttaches.Remove(dossier.LettreEngage);
                        }
                        if (dossier.QuittancePay != null)
                        {
                            dossier.QuittancePay.Remove(db);
                            db.GetDocumentAttaches.Remove(dossier.QuittancePay);
                        }
                        if (dossier.DocumentTransport != null)
                        {
                            dossier.DocumentTransport.Remove(db);
                            db.GetDocumentAttaches.Remove(dossier.DocumentTransport);
                        }
                        try
                        {
                            foreach (var j in dossier.Justificatifs.ToList())
                            {
                                j.Remove(db);
                                db.GetJustificatifs.Remove(j);
                            }
                            db.SaveChanges();
                        }
                        catch (Exception ee)
                        { }


                        try
                        {
                            foreach (var d in dossier.DocumentAttaches)
                            {
                                d.Remove(db);
                            }
                        }
                        catch (Exception)
                        { }

                        try
                        {
                            foreach (var d in db.GetDocumentAttaches.Where(d=>d.DossierId==dossier.Dossier_Id).ToList())
                            {
                                // d.Remove(db);
                                try
                                {

                                    db.GetDocumentAttaches.Remove(d);
                                    db.SaveChanges();
                                }
                                catch (Exception ee)
                                { }
                            }
                        }
                        catch (Exception)
                        { }

                    }
                    catch (Exception ee)
                    { }

                    db.GetDossiers.Remove(dossier);
                    await db.SaveChangesAsync();
                    string projectPath = "~/EspaceClient/" + dossier.ClientId + "/Ressources/Transferts";
                    string folderName = Path.Combine(Server.MapPath(projectPath), dossier.Intitulé);
                    Directory.Delete(folderName, true);

                    //Supprime tous les documents attachés
                    try
                    {
                        var docAtts = db.GetDocumentAttaches.Where(d => d.DossierId == id).ToList();
                        for (int i = 0; i < docAtts.Count; i++)
                        {
                            
                            try
                            {
                                docAtts[i].Remove(db);
                                db.GetDocumentAttaches.Remove(docAtts[i]);
                            }
                            catch (Exception)
                            { }
                        }
                        db.SaveChanges();
                    }
                    catch (Exception)
                    {}
                    
                    //status du dossier
                    try
                    {
                        var docStats= db.GetStatutDossiers.Where(d => d.IdDossier == id).ToList();
                        for (int i = 0; i < docStats.Count; i++)
                        {
                            try
                            {
                                docStats[i].Statut1=null;
                                db.GetStatutDossiers.Remove(docStats[i]);
                            }
                            catch (Exception)
                            { }
                        }
                        db.SaveChanges();
                    }
                    catch (Exception)
                    {}
                    
                    //mails et notifications du dossier
                    try
                    {
                        var docMails= db.GetNotifications.Where(d => d.DossierId == id).ToList();
                        for (int i = 0; i < docMails.Count; i++)
                        {
                            try
                            {
                                db.GetNotifications.Remove(docMails[i]);
                            }
                            catch (Exception)
                            { }
                        }
                        db.SaveChanges();
                    }
                    catch (Exception)
                    {}
                    
                    //dossier structures
                    try
                    {
                        var docStrucs= db.GetDossierStructures.Where(d => d.IdDossier == id).ToList();
                        for (int i = 0; i < docStrucs.Count; i++)
                        {
                            try
                            {
                                db.GetDossierStructures.Remove(docStrucs[i]);
                            }
                            catch (Exception)
                            { }
                        }
                        db.SaveChanges();
                    }
                    catch (Exception)
                    {}
                }
                db.DossierSupprimes.Add(new DossierSupprime()
                {
                    Agence = dossier.GetAgenceName,
                    DonneurDordre = dossier.GetClient,
                    Gestionnaire = dossier.GestionnaireName,
                    Benefic = dossier.GetFournisseur,
                    DateDepot = dossier.DateDepotBankToString,
                    DateSupp = dateNow.ToString("dd/MM/yyyy"),
                    DateTraitement = dossier.Date_Etape22ToString,
                    Devise = dossier.DeviseToString,
                    MontantDev = dossier.Montant,
                    MontantXaf = dossier.MontantCV,
                    UserName = (string)Session["userName"],
                });
                db.GetDossiers.Remove(dossier);
                await db.SaveChangesAsync();
            }
            catch (Exception ee)
            { }
            return RedirectToAction("Index");
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (Session != null){
                //var url = (string)Session["urlaccueil"];
                filterContext.Result = RedirectToAction("Index", "Index");
                //filterContext.Result = Redirect(url);
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
