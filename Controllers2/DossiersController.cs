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
using genetrix.Models;
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

        public async Task<FileResult> DownloadInstruction(int id)
        {
            var dossier = await db.GetDossiers.FindAsync(id);
            dossier.DateSignInst = dateNow;
            db.SaveChanges();
            var projectPath =Server.MapPath("~/InstructionModel.docx");

            ChangeTextInCell(projectPath,"aaa");

            var openSett = new OpenSettings() { AutoSave = false, MaxCharactersInPart = 0 };

            try
            {
               // CopyThemeContent(this.Server.MapPath("~/instruction.docx"), projectPath);

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
            PdfPage page = document.AddPage();
            //Save PDF File
            document.Save(pdfPath);

            ConvertWordToSpecifiedFormat(Server.MapPath("~/InstructionModel.docx"), pdfPath, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatPDF);
            return File(pdfPath, "application/pdf", $"Instruction_{dossier.GetFournisseur}_{dossier.MontantStringDevise}.pdf");
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

        // Change the text in a table in a word processing document.
        public static void ChangeTextInCell(string filepath, string txt)
        {
            // Use the file name and path passed in as an argument to 
            // open an existing document.            
            using (WordprocessingDocument doc =
                WordprocessingDocument.Open(filepath, true))
            {
                try
                {
                    // Find the first table in the document.
                    var elts =
                        doc.MainDocumentPart.Document.Body.Elements();
                    foreach (var item in elts)
                    {
                       var tt= item.InnerText;
                        foreach (var ch in item.ChildElements)
                        {
                            var rr = ch;
                        }
                    }
                    // Find the first table in the document.
                    var tables =
                        doc.MainDocumentPart.Document.Body.Elements<Table>();
                    Table table =
                        doc.MainDocumentPart.Document.Body.Elements<Table>().First();

                    // Find the second row in the table.
                    TableRow row = table.Elements<TableRow>().ElementAt(1);

                    // Find the third cell in the row.
                    TableCell cell = row.Elements<TableCell>().ElementAt(2);

                    // Find the first paragraph in the table cell.
                    Paragraph p = cell.Elements<Paragraph>().First();

                    // Find the first run in the paragraph.
                    Run r = p.Elements<Run>().First();

                    // Set the text for the run.
                    Text t = r.Elements<Text>().First();
                    t.Text = txt;
                }
                catch (Exception ee)
                {

                }
            }
        }

        public async Task<FileStreamResult> InstructionGen(int id)
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
            string pdfPath = Server.MapPath("~/EspaceClient/" + Session["clientId"] + "/Ressources/InstructionMOdel.Pdf");
            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage(); ;
            //Save PDF File
            document.Save(pdfPath);

            ConvertWordToSpecifiedFormat(Server.MapPath("~/InstructionModel.docx"), pdfPath, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatPDF);
            //return File(pdfPath, "application/pdf", fileName);
            FileStream fs = null;
            try
            {
                fs = new FileStream(pdfPath, FileMode.Open, FileAccess.Read);
            }
            catch (Exception e)
            {
            }
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
            var _user = db.Users.Find(User.Identity.GetUserId());
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
                ViewData["comp"] = "aaa";


                if (_user is CompteClient)
                {
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
                }

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
            
            //if(Session["Profile"].ToString()=="banque")
            //    return View("Index_banque",getDossiers.ToList());
            return View(getDossiers.ToList());
        }

        // GET: Dossiers1/Details/5
        public async Task<ActionResult> Details(int? id, string msg)
        {
            if (id == null || id == 0)
            {
                throw new Exception();
            }
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
            int _clientId = Convert.ToInt32(Session["clientId"]);
            ViewBag.BanqueId = (from b in db.GetBanqueClients
                              where b.ClientId==_clientId
                               select b.Site).ToList();
            List<Agence> agences = new List<Agence>();
            var gesSite = "";
            var bankSite = 0;
            Client client = null;
            try
            {
                client = db.GetClients.Find(_clientId);
            }
            catch (Exception)
            {}
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
        public async Task<ActionResult> Create(Dossier dossier, HttpPostedFileBase ImageInstruction=null,
            HttpPostedFileBase ImageDeclarImport = null, HttpPostedFileBase ImageDomicilImport = null, HttpPostedFileBase ImageLettreEngage = null,
             HttpPostedFileBase ImageQuittancePay = null, HttpPostedFileBase ImageDocumentTransport = null)// IEnumerable<HttpPostedFileBase> images)
        {
            IEnumerable<HttpPostedFileBase> images = null;

            try
            {
                //dossier.Montant = Convert.flo(dossier.Montant);
            }
            catch (Exception)
            {}

            //if (ModelState.IsValid)
            {
                try
                {
                    dossier.DateDepotBank = default;
                    dossier.DateModif = dateNow;
                    dossier.DateCreationApp = dateNow;
                    var banqueId = db.Structures.Find(dossier.IdSite).BanqueId(db);
                    var bank = db.GetBanques.Find(banqueId);
                    if (dossier.MontantCV <= bank.MontantDFX)
                        dossier.Categorie = 0;
                    else dossier.Categorie = 1;

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
                }
                catch (Exception)
                {}
                try
                {
                    dossier = db.GetDossiers.Add(dossier);
                    db.SaveChanges();
                }
                catch (Exception ee)
                {}
                #region Numerisation

                try
                {
                    //instructon
                    if (ImageInstruction != null)
                    {
                        /* if (!string.IsNullOrEmpty(ImageInstruction.FileName))
                         {
                             var imageModel = new genetrix.Models.ImageInstruction();
                             imageModel.Titre = "Instruction";

                             string chemin = Path.GetFileNameWithoutExtension(ImageInstruction.FileName);
                             string extension = Path.GetExtension(ImageInstruction.FileName);
                             chemin = imageModel.Titre + extension;
                             //imageModel.Url = Path.Combine(Server.MapPath(CreateNewFolderDossier(dossier.ClientId.ToString(),dossier.Intitulé)), chemin);
                             imageModel.Url = Path.Combine(CreateNewFolderDossier(dossier.ClientId.ToString(), dossier.Intitulé), chemin);
                             imageModel.NomCreateur = User.Identity.Name;
                             dossier.GetImageInstructions = new List<genetrix.Models.ImageInstruction>();
                             dossier.GetImageInstructions.Add(imageModel);
                             imageModel.Dossier = dossier;
                             db.GetImageInstructions.Add(imageModel);
                             //db._GetDossiers(_user).Add(dossier);
                             db.SaveChanges();
                             ImageInstruction.SaveAs(imageModel.Url);
                             chemin = null;
                             extension = null;
                         }*/

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

                            string chemin = Path.GetFileNameWithoutExtension(ImageInstruction.FileName);
                            string extension = Path.GetExtension(ImageInstruction.FileName);
                            chemin = imageModel.Titre + extension;
                            imageModel.Url = Path.Combine(CreateNewFolderDossier(dossier.ClientId.ToString(), dossier.Intitulé), chemin);

                            imageModel.NomCreateur = User.Identity.Name;

                            //Document attaché
                            var docAttache = new DocumentAttache();
                            docAttache.Nom = "Déclaration d'importation";
                            ////docAttache.ClientId = User.Identity.GetUserId();
                            docAttache.GetImageDocumentAttaches = new List<ImageDocumentAttache>();
                            docAttache.GetImageDocumentAttaches.Add(imageModel);
                            dossier.DeclarImport = docAttache;
                            docAttache.Dossier = dossier;

                            chemin = null;
                            extension = null;

                            db.GetDocumentAttaches.Add(docAttache);
                            db.GetImageDocumentAttaches.Add(imageModel);
                            db.SaveChanges();
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

                            imageModel.NomCreateur = User.Identity.Name;

                            //Document attaché
                            var docAttache = new DocumentAttache();
                            docAttache.Nom = "Domiciliation d'importation";
                            ////docAttache.ClientId = User.Identity.GetUserId();
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

                            imageModel.NomCreateur = User.Identity.Name;

                            //Document attaché
                            var docAttache = new DocumentAttache();
                            docAttache.Nom = "Lettre d'engagement";
                            ////docAttache.ClientId = User.Identity.GetUserId();
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

                            imageModel.NomCreateur = User.Identity.Name;

                            //Document attaché
                            var docAttache = new DocumentAttache();
                            docAttache.Nom = "Quittance de paiement";
                            ////docAttache.ClientId = User.Identity.GetUserId();
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

                            imageModel.NomCreateur = User.Identity.Name;

                            //Document attaché
                            var docAttache = new DocumentAttache();
                            docAttache.Nom = "Documents de transport";
                            ////docAttache.ClientId = User.Identity.GetUserId();
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

                }
                catch (Exception)
                { }
                #endregion

                var info = "Le dossier peut être soumis. Les documents obligatoires sont fournis";
                var color = "warning";
                try
                {
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
                }
                catch (Exception)
                {}

                ViewData["msg"] = info;
                ViewData["color"] = color;

                if(dossier.Dossier_Id>0)
                return RedirectToAction("Edit",new { id=dossier.Dossier_Id});
            }

            int _clientId = Convert.ToInt32(Session["clientId"]);
            //ViewBag.BanqueId = (from b in db.GetBanques select b).ToList();

            List<Agence> agences = new List<Agence>();
            var gesSite = "";
            var bankSite = 0;
            db.GetBanqueClients.Where(b => b.ClientId == _clientId).ToList().ForEach(b =>
            {
                try
                {
                    bankSite = b.IdSite;
                    agences.Add(b.Site);
                    gesSite = b.Gestionnaire.Structure.Nom;
                }
                catch (Exception)
                { }
            });

            ViewBag.BanqueId = agences;
            ViewBag.IdBank = bankSite;
            ViewBag.agence = gesSite;
            agences = null;
            ViewBag.DeviseMonetaireId = (from b in db.GetDeviseMonetaires select b).ToList();
            ViewBag.ReferenceExterneId = new SelectList(db.GetReferenceBanques, "Id", "NumeroRef");
            ViewBag.FournisseurId = (from b in db.GetFournisseurs
                                     where b.ClientId == _clientId
                                     select b).ToList();
            ViewBag.ClientId = new SelectList(db.GetClients.Where(c => c.Id == _clientId), "Id", "Nom");

            var model = new Dossier()
            {
                ApplicationUser = User.Identity.Name,
                DateCreationApp = dateNow,
                NbreJustif = 0,
                RefInterne = "XXX",
                ClientId = _clientId
            };

            return View(dossier);
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
        public async Task<ActionResult> Edit(int? id,string msg)
        {
            if (id == null || id==0)
            {
                throw new Exception();
            }
            try
            {
                Dossier dossier = await db.GetDossiers
                        .Include(d => d.DocumentTransport)
                           .Include(d => d.QuittancePay)
                           .Include(d => d.DocumentAttaches)
                           .Include(d => d.Justificatifs)
                           .Include(d => d.Fournisseur)
                           .Include(d => d.LettreEngage)
                           .Include(d => d.DomicilImport)
                           .Include(d => d.ReferenceExterne)
                           .Include(d => d.DeclarImport)
                           .Include(d => d.GetImageInstructions)
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
                //ViewBag.BanqueId = db.GetBanqueClients.Where(b => b.ClientId == _clientId).ToList();
                ViewBag.DeviseMonetaireId = (from b in db.GetDeviseMonetaires select b).ToList();
                ViewBag.ReferenceExterneId = new SelectList(db.GetReferenceBanques, "Id", "NumeroRef");
                //ViewBag.FournisseurId = (from b in db.GetFournisseurs
                //                         where b.ClientId == _clientId
                //                         select b).ToList();
                var bc =(from b in db.GetBanqueClients.Where(b => b.ClientId == _clientId) select b.Site).ToList();
                ViewBag.BanqueId = (from b in db.GetBanqueClients
                                    where b.ClientId == _clientId
                                    select b.Site).ToList();
                bc = null; bc = null;
                ViewBag.DeviseMonetaireId = new SelectList(db.GetDeviseMonetaires, "Id", "Nom", dossier.DeviseMonetaireId);
                //ViewBag.FournisseurId = new SelectList(db.GetFournisseurs, "Id", "Nom", dossier.FournisseurId);

                ViewBag.ClientId = new SelectList(db.GetClients.Where(c => c.Id == _clientId), "Id", "Nom");

                var fourn = new List<Fournisseurs>();

                //foreach (var item in db.GetFournisseurs)
                //{
                //    if (item.ClientId == _clientId)
                //        fourn.Add(item);
                //}
                ViewBag.FournisseurId = (from b in db.GetFournisseurs
                                         where b.ClientId == _clientId
                                         select b).ToList();

                //ViewBag.FournisseurId = new SelectList(fourn, "Id", "Nom");
                ViewBag.ClientId = new SelectList(db.GetClients.Where(c => c.Id == _clientId), "Id", "Nom");
                fourn = null;
                ViewBag.msg_sauvegardeTmp = "Dossier sauvegardé temporairement. Vous pouvez le retrouver dans les brouillons !";
                //ViewBag.InfoPercentage = dossier.InfoPercentage;
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
                    ViewBag.motifs = db.ModifsTransferts.Select(m => m.Intitule);
                }
                catch (Exception)
                { }
                return View(dossier);
            }
            catch (Exception ee)
            {}
            return RedirectToAction("Index");
        }

        // POST: Dossiers1/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Dossier model, HttpPostedFileBase ImageInstruction = null,
            HttpPostedFileBase ImageDeclarImport = null, HttpPostedFileBase ImageDomicilImport = null, HttpPostedFileBase ImageLettreEngage = null,
             HttpPostedFileBase ImageQuittancePay = null, HttpPostedFileBase ImageDocumentTransport = null
             )
        {
            Dossier dossier = model;
            var idDossier = model.Dossier_Id;

            try
            {
                dossier = db.GetDossiers.Include(d => d.Justificatifs).Include(d => d.Client).FirstOrDefault(d => d.Dossier_Id == idDossier);
            }
            catch (Exception)
            {}


            //droits d'edition
            if (dossier.EtapesDosier != null && dossier.EtapesDosier != 0 && dossier.EtapesDosier != -1 && dossier.EtapesDosier != 23 && dossier.EtapesDosier != -230 && dossier.EtapesDosier != -231 && dossier.EtapesDosier != -232 && dossier.EtapesDosier != 25 && dossier.EtapesDosier != -250 || !((string)Session["userType"] == "CompteClient") || (Convert.ToInt32(Session["clientId"]) != dossier.ClientId))
                return RedirectToAction("Details", new { id = dossier.Dossier_Id, msg = "Impossible d'éditer ce dossier! Veuillez contacter votre gestionnaire, car vous n'avez pas les droits d'édition sur le dossier." });

            if (ModelState.IsValid)
            {
                try
                {
                    //[Bind(Include = ",,Dossier_Id,BanqueId,,,,,,,Message,")]
                    dossier.IdSite = model.IdSite;
                    dossier.ContreValeurXAF = model.ContreValeurXAF;
                    dossier.Montant = model.Montant;
                    dossier.DeviseMonetaireId = model.DeviseMonetaireId;
                    if(model.FournisseurId>0)
                        dossier.FournisseurId = model.FournisseurId;
                    dossier.ClientId = model.ClientId;
                    dossier.EtapesDosier = model.EtapesDosier;
                    dossier.Transmis = model.Transmis;
                    dossier.Motif = model.Motif;
                    dossier.NumCompteBenef = model.NumCompteBenef;
                    dossier.NumCompteClient = model.NumCompteClient;
                    dossier.MarchandiseArrivee = model.MarchandiseArrivee;
                    dossier.NatureOperation = model.NatureOperation;
                    dossier.CodeAgence = model.CodeAgence;
                    dossier.Motif = model.Motif;
                    dossier.Cle = model.Cle;
                    dossier.PaysClient = model.PaysClient;
                    dossier.DateSignInst = model.DateSignInst;
                    try
                    {
                        var banqueBen = db.CompteBeneficiaires.Find(model.NumCompteBenef);
                        dossier.PaysBanqueBenf = banqueBen.Pays;
                        dossier.RibBanqueBenf = banqueBen.Cle;
                        dossier.CodeAgence = banqueBen.CodeAgence;
                        dossier.NomBanqueBenf = banqueBen.NomBanque;
                        dossier.NumBanqueBenf = banqueBen.Numero;
                    }
                    catch (Exception)
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
                        _NumberDecimalDigits = dossier.Montant.ToString().Split(',')[1].Length;
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
                    if (dossier.DateCreationApp==new DateTime())
                        dossier.DateCreationApp = dateNow;


                    if(dossier.EtapesDosier==-1)
                    {
                        //dossier.EtapesDosier = 0;
                    }
                    
                    db.Entry(dossier).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    #region Numerisation
                    dossier =  db.GetDossiers
                                    .Include(d => d.DocumentTransport)
                                        .Include(d => d.QuittancePay)
                                        .Include(d => d.LettreEngage)
                                        .Include(d => d.DomicilImport)
                                        .Include(d => d.DeclarImport)
                                        .Include(d => d.ReferenceExterne)
                                        .Include(d => d.StatusDossiers)
                                        .Include(d => d.GetImageInstructions)
                                    .FirstOrDefault(d => d.Dossier_Id == dossier.Dossier_Id);
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
                            {}

                            imageModel.NomCreateur = User.Identity.Name;
                            if(dossier.GetImageInstructions!=null && dossier.GetImageInstructions.Count >0)
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
                                        {}
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
                            {}
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
                                {}
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
                dossier = db.GetDossiers
                                    .Include(d => d.DocumentTransport)
                                        .Include(d => d.QuittancePay)
                                        .Include(d => d.LettreEngage)
                                        .Include(d => d.DomicilImport)
                                        .Include(d => d.DeclarImport)
                                        .Include(d => d.ReferenceExterne)
                                        .Include(d => d.StatusDossiers)
                                        .Include(d => d.GetImageInstructions)
                                    .FirstOrDefault(d => d.Dossier_Id == idDossier);
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

            //int _clientId = Convert.ToInt32(Session["clientId"]);
            //ViewBag.BanqueId = (from b in db.GetBanques select b).ToList();
            ////ViewBag.DeviseMonetaireId = (from b in db.GetDeviseMonetaires select b).ToList();
            //ViewBag.DeviseMonetaireId = new SelectList(db.GetDeviseMonetaires, "Id", "Nom", dossier.DeviseMonetaireId);
            //ViewBag.ReferenceExterneId = new SelectList(db.GetReferenceBanques, "Id", "NumeroRef");
            ////ViewBag.FournisseurId = (from b in db.GetFournisseurs
            ////                         where b.ClientId == _clientId
            ////                         select b).ToList();
            //ViewBag.FournisseurId = new SelectList(db.GetFournisseurs, "Id", "Nom", dossier.FournisseurId);

            //ViewBag.ClientId = new SelectList(db.GetClients.Where(c => c.Id == _clientId), "Id", "Nom");
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

        public async Task<ActionResult> Transmis(int id)
        {
            if (id > 0)
            {
                try
                {
                    var doss = db.GetDossiers.Include(d => d.Site)
                        .Include(d => d.DeclarImport)
                        .Include(d => d.DomicilImport)
                        .FirstOrDefault(d => d.Dossier_Id == id);

                    #region Restrictions
                    //Date de l'instruction <=15 jour
                    if ((dateNow-doss.DateSignInst.Value).TotalDays>15)
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
                _mt = Convert.ToDouble(mt);
            }
            catch (Exception)
            {}
            int _NumberDecimalDigits = 0;
            try
            {
                _NumberDecimalDigits = mt.Split(',')[1].Length;
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
                cvs = cv.ToString("n", nfi);// + " XAF";
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
            var deviseId = db.GetDeviseMonetaires.FirstOrDefault(d => d.Nom == devise).Id;
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
