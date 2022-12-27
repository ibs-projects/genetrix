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
using System.Drawing;
using System.IO;
using e_apurement;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using eApurement.Models;
using eApurement.Models.Fonctions;
using System.Web.UI;

namespace eApurement.Controllers
{
    [Authorize]
    public class DossiersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Dossiers1
        public ActionResult Index(string st="",string par="")
        {
            //if(Session==null)
            //    return RedirectToAction("")
            List<Dossier> getDossiers = new List<Dossier>();
            var _user = Session["user"] as ApplicationUser;
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

                Session["new_mail"] = new_mail;

                if (_user is CompteBanqueCommerciale)
                {
                    try
                    {
                        (_user as CompteBanqueCommerciale).Structure.Dossiers.Where(d => d.EtapesDosier < 51 || d.EtapesDosier == null).ToList().ForEach(d =>
                        {
                            switch (d.EtapesDosier)
                            {
                                case null: nb_broui++; break;
                                case 0: nb_asoum++; break;
                                case 1: nb_soum++; break;
                                case 2: nb_encou++; break;
                                case 3: nb_analyseconformite++; break;
                                case 4: nb_attentetransmBEAC++; break;
                                case 5: nb_encouanalyseBEAC++; break;
                                case 6: nb_attentecouverture++; break;
                                case 7: nb_saisieencou++; break;
                                case 8: nb_execute++; break;

                                case 40: nb_echu++; break;
                                case 50: nb_aapure++; break;
                                case 51: nb_apure++; break;
                                case 52: nb_archive++; break;
                                case 53: nb_supp++; break;
                                default:
                                    break;
                            }
                        });
                    }
                    catch (Exception)
                    { }


                }
                else if (_user is CompteClient)
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

                    if (Session["user"] is CompteClient)
                    {
                        int clientid = (Session["user"] as CompteClient).ClientId;
                        dd = db._GetDossiers(_user).Where(d => d.ClientId == clientid).Include(d => d.Site).Include(d => d.StatusDossiers).Include(d => d.DeviseMonetaire).Include(d => d.ReferenceExterne);//.ToList();
                    }
                    else if (Session["user"] is CompteAdmin)
                    {
                        dd = db._GetDossiers(_user).Include(d => d.Site).Include(d => d.DeviseMonetaire).Include(d => d.ReferenceExterne);
                    }

                    switch (st)
                    {
                        case "encours":
                        case "en cours":
                            getDossiers.AddRange(dd.Where(d => d.EtapesDosier >= 2 && d.EtapesDosier<7).ToList());
                            st = "en cours";
                            break;
                        case "assoum":
                        case "à soumettre":
                            getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 0).ToList());
                            st = "à soumettre";
                            break;
                        case "aapurer":
                        case "à apurer":
                            getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 49).ToList());
                            st = "à apurer";
                            break;
                        case "apure":
                        case "apuré":
                            getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 50).ToList());
                            st = "apuré";
                            break;
                        case "archive":
                        case "archivé":
                            getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 52 || d.EtapesDosier==50).ToList());
                            st = "archivé";
                            break;
                        case "echu":
                        case "échu":
                            getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 40).ToList());
                            st = "échu";
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
                        case "all":
                            getDossiers.AddRange(db._GetDossiers(_user).ToList());
                            st = "tous";
                            break;
                        default:
                            var docc = dd.ToList();
                            if (!(Session["user"] is CompteAdmin))
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
            
            if(Session["Profile"].ToString()=="banque")
                return View("Index_banque",getDossiers.ToList());
            return View(getDossiers.ToList());
        }

        // GET: Dossiers1/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null || id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                Dossier dossier = null; ;
                dossier = await db.GetDossiers
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
                ViewBag.navigation_msg = "Details dossier " + dossier.Dossier_Id;

                if (Session["user"] == null) return RedirectToAction("indexbanque", "index");
                if ((Session["Profile"].ToString()=="banque") && dossier.EtapesDosier==1)
                {
                    var idbanque = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);

                    MailFunctions.ChangeEtapeDossier((int)idbanque,2, dossier, db, (Session["user"] as CompteBanqueCommerciale).Structure);
                }

                return View(dossier);
            }
            catch (Exception ee)
            { }

            return RedirectToAction("Index");
        }

        // GET: Dossiers1/Create
        public ActionResult Create()
        {
            if (Session["user"] == null) return RedirectToAction("Login", "Account");
            int _clientId = (Session["user"] as CompteClient).ClientId;
            ViewBag.BanqueId =(from b in db.GetBanqueClients
                              where b.ClientId==_clientId
                               select b.Site).ToList(); 
            ViewBag.DeviseMonetaireId = (from b in db.GetDeviseMonetaires select b).ToList();
            ViewBag.ReferenceExterneId = new SelectList(db.GetReferenceBanques, "Id", "NumeroRef");
            ViewBag.FournisseurId = (from b in db.GetFournisseurs
                                     where b.ClientId ==_clientId
                                     select b).ToList();
            ViewBag.ClientId = new SelectList(db.GetClients.Where(c=>c.Id==_clientId), "Id", "Nom");

            var model = new Dossier()
            {
                ApplicationUser=User.Identity.Name,
                DateCreationApp=DateTime.Now,
                NbreJustif=0,
                RefInterne ="XXX",
                ClientId= _clientId
            };
            return View(model);
        }

        // POST: Dossiers1/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IdSite,Montant,FournisseurId,DeviseMonetaireId,NbreJustif,EtapesDosier,ClientId")] Dossier dossier, HttpPostedFileBase ImageInstruction=null,
            HttpPostedFileBase ImageDeclarImport = null, HttpPostedFileBase ImageDomicilImport = null, HttpPostedFileBase ImageLettreEngage = null,
             HttpPostedFileBase ImageQuittancePay = null, HttpPostedFileBase ImageDocumentTransport = null)// IEnumerable<HttpPostedFileBase> images)
        {
            IEnumerable<HttpPostedFileBase> images = null;
          
            if (ModelState.IsValid)
            {
                dossier.DateDepotBank = DateTime.Now;
                dossier.DateModif = DateTime.Now;
                dossier.DateCreationApp = DateTime.Now;
                try
                {
                    dossier.Client = (db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name) as CompteClient).Client;
                }
                catch (Exception e)
                {}                
                dossier.GetStatutDossiers = new List<Dossier_StatutDossier>();
                Dossier_StatutDossier ds = null;

                dossier=db.GetDossiers.Add(dossier);

                try
                {
                    //await db.SaveChangesAsync();
                    //foreach (var item in db.GetStatutDossiers)
                    //{
                    //    try
                    //    {
                    //        if (item.EtatDossier == EtatDossier.Brouillon)
                    //        {
                    //            ds = new Dossier_StatutDossier()
                    //            {
                    //                Dossier = dossier,
                    //                DossierId = dossier.Dossier_Id,
                    //                StatutDossier = item,
                    //                StatutDossierId = item.Statut_Id
                    //            };
                    //            break;
                    //        }
                    //    }
                    //    catch (Exception)
                    //    { }
                    //}
                    //if (ds != null)
                    //{
                    //    db.GetDossier_StatutDossiers.Add(ds);
                    //    await db.SaveChangesAsync();
                    //}
                }
                catch (Exception ee)
                {}

                #region Numerisation

                //instructon
                if (ImageInstruction != null)
                {
                    if (!string.IsNullOrEmpty(ImageInstruction.FileName))
                    {
                        var imageModel = new eApurement.Models.ImageInstruction();
                        imageModel.Titre = "Instruction_" + dossier.Intitulé;

                        string chemin = Path.GetFileNameWithoutExtension(ImageInstruction.FileName);
                        string extension = Path.GetExtension(ImageInstruction.FileName);
                        chemin = imageModel.Titre + DateTime.Now.ToString("yymmssff") + extension;
                        //imageModel.Url = Path.Combine(Server.MapPath(CreateNewFolderDossier(dossier.ClientId.ToString(),dossier.Intitulé)), chemin);
                        imageModel.Url = Path.Combine(CreateNewFolderDossier(dossier.ClientId.ToString(), dossier.Intitulé), chemin);
                        imageModel.NomCreateur = User.Identity.Name;
                        dossier.GetImageInstructions = new List<eApurement.Models.ImageInstruction>();
                        dossier.GetImageInstructions.Add(imageModel);
                        imageModel.Dossier = dossier;
                        db.GetImageInstructions.Add(imageModel);
                        //db._GetDossiers(_user).Add(dossier);
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
                        var imageModel = new eApurement.Models.ImageDocumentAttache();
                        imageModel.Titre = "Déclaration d'importation_" + dossier.Intitulé;

                        string chemin = Path.GetFileNameWithoutExtension(ImageInstruction.FileName);
                        string extension = Path.GetExtension(ImageInstruction.FileName);
                        chemin = imageModel.Titre + DateTime.Now.ToString("yymmssff") + extension;
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
                        var imageModel = new eApurement.Models.ImageDocumentAttache();
                        imageModel.Titre = "Domiciliation d'importation_" + dossier.Intitulé;

                        string chemin = Path.GetFileNameWithoutExtension(ImageDomicilImport.FileName);
                        string extension = Path.GetExtension(ImageDomicilImport.FileName);
                        chemin = imageModel.Titre + DateTime.Now.ToString("yymmssff") + extension;
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
                        var imageModel = new eApurement.Models.ImageDocumentAttache();
                        imageModel.Titre = "Lettre d'engagement_" + dossier.Intitulé;

                        string chemin = Path.GetFileNameWithoutExtension(ImageLettreEngage.FileName);
                        string extension = Path.GetExtension(ImageLettreEngage.FileName);
                        chemin = imageModel.Titre + DateTime.Now.ToString("yymmssff") + extension;
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
                        var imageModel = new eApurement.Models.ImageDocumentAttache();
                        imageModel.Titre = "Quittance de paiement_" + dossier.Intitulé;

                        string chemin = Path.GetFileNameWithoutExtension(ImageQuittancePay.FileName);
                        string extension = Path.GetExtension(ImageQuittancePay.FileName);
                        chemin = imageModel.Titre + DateTime.Now.ToString("yymmssff") + extension;
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
                        var imageModel = new eApurement.Models.ImageDocumentAttache();
                        imageModel.Titre = "Documents de transport_" + dossier.Intitulé;

                        string chemin = Path.GetFileNameWithoutExtension(ImageDocumentTransport.FileName);
                        string extension = Path.GetExtension(ImageDocumentTransport.FileName);
                        chemin = imageModel.Titre + DateTime.Now.ToString("yymmssff") + extension;
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

                #endregion
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


                return RedirectToAction("Edit",new { id=dossier.Dossier_Id});
            }

            int _clientId = (Session["user"] as CompteClient).ClientId;
            ViewBag.BanqueId = (from b in db.GetBanques select b).ToList();
            ViewBag.DeviseMonetaireId = (from b in db.GetDeviseMonetaires select b).ToList();
            ViewBag.ReferenceExterneId = new SelectList(db.GetReferenceBanques, "Id", "NumeroRef");
            ViewBag.FournisseurId = (from b in db.GetFournisseurs
                                     where b.ClientId == _clientId
                                     select b).ToList();
            ViewBag.ClientId = new SelectList(db.GetClients.Where(c => c.Id == _clientId), "Id", "Nom");

            return View(dossier);
        }

        
        private string CreateNewFolderDossier(string clientId,string intituleDossier)
        {
            try
            {
                string projectPath = "~/EspaceClient/" + clientId + "/Ressources/Transferts";
                string folderName = Path.Combine(Server.MapPath(projectPath), intituleDossier);
                System.IO.Directory.CreateDirectory(folderName);
                return folderName;
            }
            catch (Exception ee)
            { }
            return "";
        }

        [HttpPost]
        public ActionResult ModifEtatDossier(FormCollection form)
        {
            var idDossier = int.Parse(form["IdDossier"]);
            var etat = int.Parse(form["EtapesDosier"]);
            if (idDossier>0)
            {
                var idbanque = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);

                var doss = db.GetDossiers.Find(idDossier);
                MailFunctions.ChangeEtapeDossier((int)idbanque, etat,doss,db, (Session["user"] as CompteBanqueCommerciale).Structure);
                doss.DateModif = DateTime.Now;
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
                dossier.DateModif = DateTime.Now;

                //Douane
                if (ImageDouane != null)
                {
                    if (!string.IsNullOrEmpty(ImageDouane.FileName))
                    {
                        var imageModel = new eApurement.Models.ImageDocumentAttache();
                        imageModel.Titre = "Douane_" + dossier.Intitulé;

                        string chemin = Path.GetFileNameWithoutExtension(ImageDouane.FileName);
                        string extension = Path.GetExtension(ImageDouane.FileName);
                        chemin = imageModel.Titre + DateTime.Now.ToString("yymmssff") + extension;
                        imageModel.Url = Path.Combine(CreateNewFolderDossier(dossier.ClientId.ToString(), dossier.Intitulé), chemin);
                        if (dossier.GetImageInstructions != null && dossier.GetImageInstructions.Count > 0)
                        {
                            //Supprime l'ancienne image
                            System.IO.File.Delete(dossier.GetImageInstructions.ToList()[0].Url);
                        }
                        else
                            dossier.GetImageInstructions = new List<eApurement.Models.ImageInstruction>();

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
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null || id==0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                Dossier dossier = await db.GetDossiers
                        .Include(d => d.DocumentTransport)
                           .Include(d => d.QuittancePay)
                           .Include(d => d.Justificatifs)
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
                if (Session["user"] == null) return RedirectToAction("Login", "Account");
                int _clientId = 0;
                if (Session["Profile"].ToString()=="banque")
                    _clientId = dossier.ClientId;
                else
                    _clientId = (Session["user"] as CompteClient).ClientId;
                //ViewBag.BanqueId = db.GetBanqueClients.Where(b => b.ClientId == _clientId).ToList();
                ViewBag.DeviseMonetaireId = (from b in db.GetDeviseMonetaires select b).ToList();
                ViewBag.ReferenceExterneId = new SelectList(db.GetReferenceBanques, "Id", "NumeroRef");
                ViewBag.FournisseurId = (from b in db.GetFournisseurs
                                         where b.ClientId == _clientId
                                         select b).ToList();
                var bc =(from b in db.GetBanqueClients.Where(b => b.ClientId == _clientId) select b.Site).ToList();
                ViewBag.BanqueId = (from b in db.GetBanqueClients
                                    where b.ClientId == _clientId
                                    select b.Site).ToList();
                bc = null; bc = null;
                ViewBag.DeviseMonetaireId = new SelectList(db.GetDeviseMonetaires, "Id", "Nom", dossier.DeviseMonetaireId);
                ViewBag.FournisseurId = new SelectList(db.GetFournisseurs, "Id", "Nom", dossier.FournisseurId);

                ViewBag.ClientId = new SelectList(db.GetClients.Where(c => c.Id == _clientId), "Id", "Nom");

                var fourn = new List<Fournisseurs>();

                foreach (var item in db.GetFournisseurs)
                {
                    if (item.ClientId == _clientId)
                        fourn.Add(item);
                }

                ViewBag.FournisseurId = new SelectList(fourn, "Id", "Nom");
                ViewBag.ClientId = new SelectList(db.GetClients.Where(c => c.Id == _clientId), "Id", "Nom");
                fourn = null;
                ViewBag.msg_sauvegardeTmp = "Dossier sauvegardé temporairement. Vous pouvez le retrouver dans les brouillons !";
                //ViewBag.InfoPercentage = dossier.InfoPercentage;
                ViewBag.navigation = "rien";
                ViewBag.navigation_msg = "Edition dossier " + dossier.Dossier_Id;

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
        public async Task<ActionResult> Edit([Bind(Include = "IdSite,Dossier_Id,BanqueId,Montant,DeviseMonetaireId,FournisseurId,ClientId,EtapesDosier")] Dossier dossier, HttpPostedFileBase ImageInstruction = null,
            HttpPostedFileBase ImageDeclarImport = null, HttpPostedFileBase ImageDomicilImport = null, HttpPostedFileBase ImageLettreEngage = null,
             HttpPostedFileBase ImageQuittancePay = null, HttpPostedFileBase ImageDocumentTransport = null)// IEnumerable<HttpPostedFileBase> FactureImages)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    dossier.DateModif = DateTime.Now;
                    if (dossier.DateCreationApp==new DateTime())
                        dossier.DateCreationApp = DateTime.Now;

                    if (dossier.DateDepotBank == new DateTime())
                        dossier.DateDepotBank = DateTime.Now;

                    db.Entry(dossier).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    #region Numerisation

                    //instructon
                    if (ImageInstruction != null)
                    {
                        if (!string.IsNullOrEmpty(ImageInstruction.FileName))
                        {
                            var imageModel = new eApurement.Models.ImageInstruction();
                            imageModel.Titre = "Instruction_" + dossier.Intitulé;

                            string chemin = Path.GetFileNameWithoutExtension(ImageInstruction.FileName);
                            string extension = Path.GetExtension(ImageInstruction.FileName);
                            chemin = imageModel.Titre + DateTime.Now.ToString("yymmssff") + extension;
                            imageModel.Url = Path.Combine(CreateNewFolderDossier(dossier.ClientId.ToString(), dossier.Intitulé), chemin);
                            imageModel.NomCreateur = User.Identity.Name;
                            if(dossier.GetImageInstructions!=null && dossier.GetImageInstructions.Count >0)
                            {
                                //Supprime l'ancienne image
                                System.IO.File.Delete(dossier.GetImageInstructions.ToList()[0].Url);
                            }
                            else
                                dossier.GetImageInstructions = new List<eApurement.Models.ImageInstruction>();
                            
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
                            var imageModel = new eApurement.Models.ImageDocumentAttache();
                            imageModel.Titre = "Déclaration d'importation_" + dossier.Intitulé;

                            string chemin = Path.GetFileNameWithoutExtension(ImageDeclarImport.FileName);
                            string extension = Path.GetExtension(ImageDeclarImport.FileName);
                            chemin = imageModel.Titre + DateTime.Now.ToString("yymmssff") + extension;
                            imageModel.Url = Path.Combine(CreateNewFolderDossier(dossier.ClientId.ToString(), dossier.Intitulé), chemin);
                            if (dossier.DeclarImport != null && dossier.Get_DeclarImport != "#")
                            {
                                //Supprime l'ancienne image
                                System.IO.File.Delete(dossier.Get_DeclarImport);
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
                            var imageModel = new eApurement.Models.ImageDocumentAttache();
                            imageModel.Titre = "Domiciliation d'importation_" + dossier.Intitulé;

                            string chemin = Path.GetFileNameWithoutExtension(ImageDomicilImport.FileName);
                            string extension = Path.GetExtension(ImageDomicilImport.FileName);
                            chemin = imageModel.Titre + DateTime.Now.ToString("yymmssff") + extension;
                            imageModel.Url = Path.Combine(CreateNewFolderDossier(dossier.ClientId.ToString(), dossier.Intitulé), chemin);
                            if (dossier.DomicilImport != null && dossier.Get_DomicilImport != "#")
                            {
                                //Supprime l'ancienne image
                                System.IO.File.Delete(dossier.Get_DomicilImport);
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
                            var imageModel = new eApurement.Models.ImageDocumentAttache();
                            imageModel.Titre = "Lettre d'engagement_" + dossier.Intitulé;

                            string chemin = Path.GetFileNameWithoutExtension(ImageLettreEngage.FileName);
                            string extension = Path.GetExtension(ImageLettreEngage.FileName);
                            chemin = imageModel.Titre + DateTime.Now.ToString("yymmssff") + extension;
                            imageModel.Url = Path.Combine(CreateNewFolderDossier(dossier.ClientId.ToString(), dossier.Intitulé), chemin);
                            if (dossier.LettreEngage != null && dossier.Get_LettreEngage != "#")
                            {
                                //Supprime l'ancienne image
                                System.IO.File.Delete(dossier.Get_LettreEngage);
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
                            var imageModel = new eApurement.Models.ImageDocumentAttache();
                            imageModel.Titre = "Quittance de paiement_" + dossier.Intitulé;

                            string chemin = Path.GetFileNameWithoutExtension(ImageQuittancePay.FileName);
                            string extension = Path.GetExtension(ImageQuittancePay.FileName);
                            chemin = imageModel.Titre + DateTime.Now.ToString("yymmssff") + extension;
                            imageModel.Url = Path.Combine(CreateNewFolderDossier(dossier.ClientId.ToString(), dossier.Intitulé), chemin);
                            if (dossier.QuittancePay != null && dossier.Get_QuittancePay != "#")
                            {
                                //Supprime l'ancienne image
                                System.IO.File.Delete(dossier.Get_QuittancePay);
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
                            var imageModel = new eApurement.Models.ImageDocumentAttache();
                            imageModel.Titre = "Documents de transport_" + dossier.Intitulé;

                            string chemin = Path.GetFileNameWithoutExtension(ImageDocumentTransport.FileName);
                            string extension = Path.GetExtension(ImageDocumentTransport.FileName);
                            chemin = imageModel.Titre + DateTime.Now.ToString("yymmssff") + extension;
                            imageModel.Url = Path.Combine(CreateNewFolderDossier(dossier.ClientId.ToString(), dossier.Intitulé), chemin);
                            if (dossier.DocumentTransport != null && dossier.Get_DocumentTransport != "#")
                            {
                                //Supprime l'ancienne image
                                System.IO.File.Delete(dossier.Get_DocumentTransport);
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


                return RedirectToAction("Index");
            }

            int _clientId = (Session["user"] as CompteClient).ClientId;
            ViewBag.BanqueId = (from b in db.GetBanques select b).ToList();
            ViewBag.DeviseMonetaireId = (from b in db.GetDeviseMonetaires select b).ToList();
            ViewBag.ReferenceExterneId = new SelectList(db.GetReferenceBanques, "Id", "NumeroRef");
            ViewBag.FournisseurId = (from b in db.GetFournisseurs
                                     where b.ClientId == _clientId
                                     select b).ToList();
            ViewBag.ClientId = new SelectList(db.GetClients.Where(c => c.Id == _clientId), "Id", "Nom");

            return View(dossier);
        }

        public FileStreamResult GetPDF(string path)
        {
            #region MyRegion

            //FIX ROOT PATH TO APP ROOT PATH
            if (path.StartsWith("/"))
                path = path.Insert(0, "~");

            if (!path.StartsWith("~/"))
                path = path.Insert(0, "~/");
            path = VirtualPathUtility.ToAbsolute(path);

            #endregion

            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            return File(fs, "application/pdf");
        }
        public ActionResult getpath(string path)
        {
            return Json(path, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GrapheInit(int annee,string devise)
        {
            List<GrapheVM> donneesAnnulles = new List<GrapheVM>();
            var deviseId = db.GetDeviseMonetaires.FirstOrDefault(d => d.Nom == devise).Id;
            var id = (Session["user"] as CompteClient).ClientId;
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
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dossier dossier = await db.GetDossiers.FindAsync(id);
            if (dossier == null)
            {
                return HttpNotFound();
            }
            ViewBag.navigation = "rien";
            ViewBag.navigation_msg = "suppresion dossier " + dossier.Dossier_Id;
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
                    .Include(d=>d.DeclarImport)
                    .FirstOrDefault(d=>d.Dossier_Id==id);

                //dossier.Remove(db);
                try
                {
                    if (dossier.DeclarImport != null)
                        dossier.DeclarImport.Remove(db);
                    if (dossier.DomicilImport != null)
                        dossier.DomicilImport.Remove(db);
                    if (dossier.LettreEngage != null)
                        dossier.LettreEngage.Remove(db);
                    if (dossier.QuittancePay != null)
                        dossier.QuittancePay.Remove(db);
                    if (dossier.DocumentTransport != null)
                        dossier.DocumentTransport.Remove(db);   
                    try
                    {
                        foreach (var j in dossier.Justificatifs.ToList())
                        {
                            j.Remove(db);
                        }
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
                        foreach (var d in db.GetDocumentAttaches.ToList())//.Where(d=>d.DossierId==dossier.Dossier_Id))
                        {
                            d.Remove(db);
                        }
                    }
                    catch (Exception)
                    { }
                    db.SaveChanges();

                }
                catch (Exception ee)
                { }

                db.GetDossiers.Remove(dossier);
                await db.SaveChangesAsync();
                string projectPath = "~/EspaceClient/" + dossier.ClientId + "/Ressources/Transferts";
                string folderName = Path.Combine(Server.MapPath(projectPath), dossier.Intitulé);
                Directory.Delete(folderName, true);
            }
            catch (Exception ee)
            {}
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
