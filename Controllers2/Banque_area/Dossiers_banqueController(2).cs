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

namespace eApurement.Controllers
{
    [Authorize]
    public class Dossiers_banqueController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Dossiers1
        public ActionResult Index(string st="",string comp="",string par="",string interf="",int? referenceId=null,string page="")
        {
            if (Session==null)
                return RedirectToAction("connexion", "auth");

            ViewBag.pages = string.IsNullOrEmpty(page) ? "tb" : page;

            var _user = Session["user"]  as CompteBanqueCommerciale;
            List<Dossier> getDossiers = new List<Dossier>();
            bool dossiersGroupes = false;
            //if (VariablGlobales.Access(_user, "dossier"))
            {
                int new_mail = 0, new_msg = 0, nb_broui = 0, nb_asoum = 0, nb_soum = 0, nb_encou = 0, nb_aapure = 0, nb_echu = 0,
                       nb_analyseconformite = 0, nb_attentetransmBEAC = 0, nb_encouanalyseBEAC = 0, nb_attentecouverture = 0,
                       nb_saisieencou = 0, nb_execute = 0, nb_apure = 0, nb_archive = 0, nb_supp = 0;

                if (!(_user is CompteBanqueCommerciale))
                    return RedirectToAction("connexion", "auth");

                try
                {
                    new_msg = db.GetMails.Where(m => m.AdresseDest == _user.Email).Count();
                }
                catch (Exception)
                { }

                Session["new_mail"] = new_mail;

                try
                {
                    (_user as CompteBanqueCommerciale).Structure.Dossiers.Where(d => d.EtapesDosier < 13 || d.EtapesDosier == null).ToList().ForEach(d =>
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

                List<Dossier> dd = new List<Dossier>();
                ViewBag.navigation = string.IsNullOrEmpty(st) ? "brouil" : st;
                try
                {
                    //1=client; 2=banque;3=admin
                    byte profile = 0;

                    if (Session["user"] is CompteBanqueCommerciale)
                    {
                        if ((Session["user"] as CompteBanqueCommerciale).EstAdmin)
                        {
                            var agenceID = (Session["user"] as CompteBanqueCommerciale).IdStructure;
                            dd = db.GetDossiers.Where(d => d.IdSite == agenceID & d.EtapesDosier != 14).Include(d => d.Site).Include(d => d.DeviseMonetaire).Include(d => d.ReferenceExterne).ToList();
                        }
                        else
                        {

                            var banqueID = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);
                            var role = db.XtraRoles.Find((Session["user"] as CompteBanqueCommerciale).IdXRole);
                            //var site = db.Agences.Find((Session["user"] as CompteBanqueCommerciale).IdStructure);
                            var site = db.Structures.Find((Session["user"] as CompteBanqueCommerciale).IdStructure);
                            var gesId = (Session["user"] as CompteBanqueCommerciale).Id;
                            if (role == null)
                                return RedirectToAction("Login", "auth");

                            dd = VariablGlobales.UserBanqueDossiers(db,site,role,banqueID,gesId);
                        }
                    }
                    else if (Session["user"] is CompteAdmin)
                    {
                        dd = db.GetDossiers.Include(d => d.Site).Include(d => d.DeviseMonetaire).Include(d => d.ReferenceExterne).ToList();
                    }
                    if (string.IsNullOrEmpty(comp))
                    {
                        switch (st)
                        {
                            case "rejet":
                            case "rejeté":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier <0).ToList());
                                st ="rejeté";
                                break;
                            case "soumis":
                            case "recu":
                            case "reçus":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 1).ToList());
                                st = st == "recu" ? "reçus" : "soumis";
                                break;
                            case "encours":
                            case "en cours":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 2).ToList());
                                st = "en cours";
                                break;
                            case "conform":
                            case "à la conformité":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 3).ToList());
                                st = "à la conformité";
                                break;
                            case "encoursconf":
                            case "en cours d'analyse conformité":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 4).ToList());
                                st = "en cours d'analyse conformité";
                                break;
                            case "env_stf":
                            case "transmis au service ransfert":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 5).ToList());
                                st = "transmis au service ransfert";
                                break; 
                            case "encourstf":
                            case "en cours service ransfert":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 6).ToList());
                                st = "en cours service ransfert";
                                break;
                            case "aenv_bac":
                            case "à envoyer à la BEAC":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 7).ToList());
                                st = "à envoyer à la BEAC";
                                dossiersGroupes = true;
                                break;
                            case "atten_couv":
                            case "env_bac":
                            case "envoyés BEAC":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 8).ToList());
                                st = "envoyés BEAC";
                                dossiersGroupes = true;
                                break;
                            case "saisie_encour":
                            case "saisie en cours":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 9).ToList());
                                dossiersGroupes = true;
                                st = "saisie en cours";
                                break;
                            case "accord":
                            case "accordés":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 10).ToList());
                                dossiersGroupes = true;
                                st = "accordés";
                                break;
                            case "aapurer":
                            case "à apurer":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 11).ToList());
                                st = "à apurer";
                                break;
                            case "apure":
                            case "apuré":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 12).ToList());
                                st = "apuré";
                                break;
                            case "echu":
                            case "échu":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 13).ToList());
                                st = "échu";
                                break;
                            case "archive":
                            case "archivé":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 14).ToList());
                                st = "archivé";
                                break;
                            case "supp":
                            case "supprimés":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 15).ToList());
                                st = "supprimés";
                                break;
                            case "tr":
                            case "traités":
                                getDossiers.AddRange(dd.Where(d => d.Traité).ToList());
                                st = "traités";
                                break;
                            case "untr":
                            case "non traités":
                                getDossiers.AddRange(dd.Where(d => !d.Traité).ToList());
                                st = "non traités";
                                break;
                            case "port":
                            case "mon portefeuille":
                                var userId = (Session["user"] as CompteBanqueCommerciale).Id;
                                foreach (var item in dd.Where(d => !d.Traité).ToList())
                                {
                                    try
                                    {
                                        if (item.Client.Banques.FirstOrDefault(c => c.IdGestionnaire == userId) != null)
                                        {
                                            getDossiers.Add(item);
                                        }
                                    }
                                    catch (Exception e)
                                    { }
                                }
                                st = "mon portefeuille";
                                break;
                            case "all":
                                getDossiers.AddRange(db.GetDossiers.ToList());
                                st = "tous";
                                break;
                            case "conf_st":
                            case "transmis":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier >= 3).ToList());
                                st = "transmis";
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        int compNum = int.Parse(comp.Split('_')[0]);
                        int compMin = int.Parse(comp.Split('_')[1]);
                        int compMax= int.Parse(comp.Split('_')[2]);
                        var composant = db.GetComposants.FirstOrDefault(c=>c.Numero==compNum && c.NumeroMin==compMin && c.NumeroMax==compMax);
                        string interface_url = "";
                        Session["NumComposant"] = compNum;

                        if (interf == "dfx")
                        {
                            getDossiers.AddRange(dd.Where(d => d.Montant <= 50000000 && d.EtapesDosier >= compMin && d.EtapesDosier <= compMax).ToList());
                            interface_url = "/index/indexbanque?panel=dfx&interf=dfx";
                        }
                        else if (interf == "refinancement")
                        {
                            getDossiers.AddRange(dd.Where(d => d.Montant > 50000000 && d.EtapesDosier >= compMin && d.EtapesDosier <= compMax).ToList());
                            interface_url = "/index/indexbanque?panel=ref&interf=refinancement";
                        }
                        else
                            getDossiers.AddRange(dd.Where(d => d.EtapesDosier >= compMin && d.EtapesDosier <= compMax).ToList());
                        st =$"<a href=\"{interface_url}\">{interf}</a>  / "+ composant.Description;
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
            ViewBag.par = comp;
            ViewBag.statut = st.ToString();
            if (dossiersGroupes)
                return RedirectToAction("Index", "ReferenceBanques", new { st = ViewBag.navigation,comp=comp });

            if (referenceId !=null)
            {
                return View("Index", getDossiers.Where(d => d.ReferenceExterneId == referenceId).ToList());
            }
           return View("Index",getDossiers.ToList());
        }

        public ActionResult Etats()
        {
            return View(new Dossier());
        }

        public ActionResult DossiersGroupes(int? idClient=null)
        {
            List<Dossier> getDossiers = new List<Dossier>();
            IEnumerable<Dossier> dd = new List<Dossier>();
            var banque =db.GetBanques.Find((Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db));
            if (idClient==null || idClient==0)
            {
                dd = db.GetDossiers.Where(d => d.Site.BanqueId(db) == banque.Id && d.EtapesDosier < 14 && d.ReferenceExterneId!=null).Include(d => d.Site).Include(d => d.DeviseMonetaire).Include(d => d.ReferenceExterne);//.ToList();

                //foreach (var item in db.GetDossiers.Include(d => d.Site).Include(d => d.DeviseMonetaire).Include(d => d.ReferenceExterne))
                //{
                //    if (item.IdSite == agence.Id && item.EtapesDosier != 52 && item.ReferenceExterneId != null)
                //        dd.Add(item);
                //}
            }
            else
            {
                dd = db.GetDossiers.Where(d => d.IdSite == banque.Id & d.EtapesDosier <14 && d.ClientId==idClient).Include(d => d.Site).Include(d => d.DeviseMonetaire).Include(d => d.ReferenceExterne);
            }
            
            List<DossierParReference> parReferences = new List<DossierParReference>();

            try
            {
                foreach (var item in dd.Where(d => d.ReferenceExterneId != null && d.ReferenceExterneId != 0).GroupBy(d => d.ReferenceExterneId))
                {
                    parReferences.Add(new DossierParReference()
                    {
                        IdAgence = (int)banque.Id,
                        NomAgence = banque.Nom,
                        NbrDossiers = item.Count(),
                        EtapeDosiier = (item.Count() == 0 ? null : item.ElementAt(0).EtapesDosier),
                        NumeroRef = (item.Count() == 0 ? "null" : item.ElementAt(0).ReferenceExterne.NumeroRef)
                    });
                }
                getDossiers.AddRange(dd.Where(d => !d.Traité).ToList());
            }
            catch (Exception ee)
            {}
            ViewBag.statut = "groupés par référence";
            return View(parReferences);
        }

        // GET: Dossiers1/Details/5
        public async Task<ActionResult> Details(int? id,string unit="")
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
               
                if (dossier.EtapesDosier < 4 && dossier.ReferenceExterneId!=null)
                {
                    try
                    {
                        if (dossier.ReferenceExterne != null)
                        {
                            dossier.ReferenceExterne.DepotBEAC = null;
                            dossier.ReferenceExterne.Accordé = false;
                            dossier.ReferenceExterne.EnvoieBEAC = false;
                        }
                        dossier.ReferenceExterne = null;
                        dossier.ReferenceExterneId = null;
                        db.SaveChanges();
                    }
                    catch (Exception)
                    {}
                }
                else if (dossier.EtapesDosier >= 5 && dossier.ReferenceExterne != null)
                {
                    dossier.EtapesDosier = 6;
                    db.SaveChanges();
                }
                //else if (dossier.EtapesDosier > 4 && dossier.ReferenceExterneId == null)
                //{
                //    dossier.EtapesDosier = 4;
                //    db.SaveChanges();
                //}

                if (dossier.EtapesDosier == 3)
                {
                    try
                    {
                        dossier.EtapesDosier = 4;
                        db.SaveChanges();
                    }
                    catch (Exception)
                    { }
                }


                if (dossier.Montant>50000000 && dossier.ReferenceExterneId != null && dossier.ReferenceExterneId != 0 && unit!="ok")
                    return RedirectToAction("Details", "ReferenceBanques", new { id = dossier.ReferenceExterneId });

                ViewBag.navigation = "rien";
                ViewBag.navigation_msg = "Details dossier " + dossier.Dossier_Id;

                if (Session["user"] == null) return RedirectToAction("indexbanque", "index");
                if ((Session["Profile"].ToString()=="banque") && dossier.EtapesDosier==1 && (dossier.DomicilImport==null && dossier.DeclarImport == null || !dossier.MarchandiseArrivee))
                {
                    var idbanque = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);

                    MailFunctions.ChangeEtapeDossier((int)idbanque,2, dossier, db, (Session["user"] as CompteBanqueCommerciale).Structure);
                }

                var user = Session["user"] as CompteBanqueCommerciale;
                dossier.Permissions = new Dictionary<string, bool>();
                //foreach (var item in user.XRole.GetEntitee_Roles)
                var entitees = db.GetEntitees.ToList();

                foreach (var item in db.GetEntitee_Roles.Where(r=>r.IdXRole==user.IdXRole))
                {
                    try
                    {
                        dossier.Permissions.Add(entitees.FirstOrDefault(e => e.Id == item.IdEntitee).Type, item.Ecrire);

                    }
                    catch (Exception)
                    {}                
                }

                var dd = new List<Agence>();
                db.Agences.ToList().ForEach(c =>
                {
                    if (c.BanqueId(db) == user.Structure.BanqueId(db) && c.Id != user.IdStructure)
                        dd.Add(c);
                });

                ViewBag.sites = new SelectList(dd, "Id", "Nom");
                entitees = null;
                dd = null;
                try
                {
                    var site = db.Structures.Find(user.IdStructure);
                    ViewBag.userSIteMinNiveau = site.NiveauDossier;
                    ViewBag.userSIteMaxNiveau = site.NiveauMaxDossier;
                    ViewBag.niveauSite = site.NiveauDossier;
                }
                catch (Exception)
                {}
                //ViewBag.client=db.GetClients.Include(c=>c.)
                return View(dossier);
            }
            catch (Exception)
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
                RefInterne ="XXX"
            };
            return View(model);
        }

        // POST: Dossiers1/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "BanqueId,Montant,FournisseurId,DeviseMonetaireId,NbreJustif,EtapesDosier")] Dossier dossier, HttpPostedFileBase ImageInstruction=null,
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
                    await db.SaveChangesAsync();
                    foreach (var item in db.GetStatutDossiers)
                    {
                        try
                        {
                            if (item.EtatDossier == EtatDossier.Brouillon)
                            {
                                ds = new Dossier_StatutDossier()
                                {
                                    Dossier = dossier,
                                    DossierId = dossier.Dossier_Id,
                                    StatutDossier = item,
                                    StatutDossierId = item.Statut_Id
                                };
                                break;
                            }
                        }
                        catch (Exception)
                        { }
                    }
                    if (ds != null)
                    {
                        db.GetDossier_StatutDossiers.Add(ds);
                        await db.SaveChangesAsync();
                    }
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
                        //db.GetDossiers.Add(dossier);
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
                        //docAttache.ClientId = User.Identity.GetUserId();
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
                //var idbanque = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);
                //var doss = db.GetDossiers.Find(idDossier);
                //MailFunctions.ChangeEtapeDossier((int)idbanque,etat,doss,db);
                //doss.DateModif = DateTime.Now;
                //db.SaveChanges();
            }
            
            return RedirectToAction("Details", new { id= idDossier });
        } 
        
        [HttpGet]
        public ActionResult ModifEtatDossierJS(int? etat=null,int? idDossier=null,bool Estgroupe=false,int? idRef=null,string message="")
        {
            var result = "Opération effectuée avec succes";
            try
            {
                var idbanque = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);
                if (etat>0)
                {
                    if (Estgroupe)
                    {
                        if (idRef > 0)
                        {
                            string fournisseur = "",msg="";
                            var nomclient = "";
                            int count = 0;

                            var re = db.GetReferenceBanques.Include(d=>d.Dossiers).FirstOrDefault(r => r.Id == idRef);
                            foreach (var d in re.Dossiers)
                            {
                                d.EtapesDosier = etat;
                                msg = d.GetEtapDossier()[0];
                                count++;
                            }
                            if (count > 0)
                            {
                                fournisseur = re.Dossiers.FirstOrDefault().Fournisseur.Nom;
                                nomclient = re.Dossiers.FirstOrDefault().Client.Nom;
                                if (string.IsNullOrEmpty(re.Dossiers.FirstOrDefault().Client.Email))
                                {
                                    result = "Erreur: L'adresse mail du client "+ nomclient + " est nulle. Le processus s'est arreté, veillez corriger cette erreur!";
                                    var d = 0;
                                    var dd = 1 / d;
                                }
                            }
                            try
                            {
                                db.SaveChanges();
                                MailFunctions.EnvoiMail(re, fournisseur, nomclient, (int)idbanque, "Dossier "+msg, "La référence est "+msg+" depuis le " + re.DateReception,db);
                            }
                            catch (Exception)
                            { }
                        }
                    }
                    else if (idDossier > 0)
                    {
                        var doss = db.GetDossiers
                         .Include(d => d.Site)
                        .Include(d => d.Fournisseur)
                        .Include(d => d.Client)
                        .Include(d => d.DeviseMonetaire)
                        .FirstOrDefault(d=>d.Dossier_Id==idDossier);

                        var nomclient = doss.Client.Nom;
                        if (string.IsNullOrEmpty(doss.Client.Email))
                        {
                            result = "Erreur: L'adresse mail du client " + nomclient + " est nulle. Le processus s'est arreté, veillez corriger cette erreur!";
                            var d = 0;
                            var dd = 1 / d;
                        }
                        MailFunctions.ChangeEtapeDossier((int)idbanque,(int)etat, doss, db, (Session["user"] as CompteBanqueCommerciale).Structure);
                    }
                }
                else
                {
                    if (Estgroupe)
                    {
                        if (idRef > 0)
                        {
                            string fournisseur = "", msg = "";
                            var nomclient = "";
                            int count = 0;

                            var re = db.GetReferenceBanques.Include(d => d.Dossiers).FirstOrDefault(r => r.Id == idRef);
                            foreach (var d in re.Dossiers)
                            {
                                d.EtapesDosier = 2;
                                msg = d.GetEtapDossier()[0];
                                count++;
                            }
                            if (count > 0)
                            {
                                fournisseur = re.Dossiers.FirstOrDefault().Fournisseur.Nom;
                                nomclient = re.Dossiers.FirstOrDefault().Client.Nom;
                            }
                            if (string.IsNullOrEmpty(re.Dossiers.FirstOrDefault().Client.Email))
                            {
                                result = "Erreur: L'adresse mail du client " + nomclient + " est nulle. Le processus s'est arreté, veillez corriger cette erreur!";
                                return Json(result + "_" + etat, JsonRequestBehavior.AllowGet);
                            }
                            try
                            {
                                db.SaveChanges();
                                MailFunctions.EnvoiMail(re, fournisseur, nomclient, (int)idbanque, "Dossier rejeté ", "La référence "+re.NumeroRef+"  associée à(aux) dossier(s) suivant(s) a été rejetée suite à " + message + " depuis le " + DateTime.Now, db);
                            }
                            catch (Exception)
                            { }
                        }
                    }
                    else
                    {
                        var doss = db.GetDossiers
                        .Include(d => d.Site)
                        .Include(d => d.Fournisseur)
                        .Include(d => d.Client)
                        .Include(d => d.DeviseMonetaire)
                        .FirstOrDefault(d => d.Dossier_Id == idDossier);
                        var nomclient = doss.Client.Nom;
                        if (string.IsNullOrEmpty(doss.Client.Email))
                        {
                            result = "Erreur: L'adresse mail du client " + nomclient + " est nulle. Le processus s'est arreté, veillez corriger cette erreur!";
                            return Json(result + "_" + etat, JsonRequestBehavior.AllowGet);
                        }
                        if (doss.EtapesDosier > 2)
                        {
                            MailFunctions.ChangeEtapeDossier((int)idbanque, 2, doss, db, (Session["user"] as CompteBanqueCommerciale).Structure, rejet: true, msg: message);
                        }
                        else
                        {
                            int eta = 0;
                            if (doss.EtapesDosier > 2)
                                eta = -2;
                            else
                                eta = -1;
                            MailFunctions.ChangeEtapeDossier((int)idbanque, eta, doss, db, (Session["user"] as CompteBanqueCommerciale).Structure, rejet: true, msg: message);
                        }
                    }

                    //return RedirectToAction("Index");
                }

                db.SaveChanges();
            }
            catch (Exception)
            {
                //result = "erreur";
            }

            
            return Json(result+"_"+ etat, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TraiteDossier(int idDossier,bool traite)
        {
            var result = "erreur";
            if (idDossier > 0)
            {
                try
                {
                    var emailGes = "";
                    var doss = db.GetDossiers.Include(d=>d.Client).FirstOrDefault(d=>d.Dossier_Id==idDossier);
                    var ges = db.GetBanqueClients.FirstOrDefault(d=>d.IdSite==doss.IdSite && d.ClientId==doss.ClientId);
                    if (ges != null)
                        try
                        {
                            emailGes = ges.Gestionnaire.Email;
                        }
                        catch (Exception)
                        {}

                    doss.Traité = traite;
                    db.SaveChanges();
                    MailFunctions.SendMail(new MailModel()
                    {
                        To = doss.Client.Email,
                        Subject = "<h3 style=\"background-color:blue;color:white;padding:8px;text-align:center\"> Situation du dossier n°" + doss.Dossier_Id+"</h3>",
                        Body =doss.Traité? "<p>Le dossier n°" + doss.Dossier_Id + " a été traité le " + DateTime.Now+"</p>": "<p>Le dossier n°" + doss.Dossier_Id + " n'a pas été traité le " + DateTime.Now+"</p>",
                        CC= emailGes
                    }, db);
                    result = "succes";
                    doss =null;
                    ges = null;
                }
                catch (Exception)
                {}
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult Valider(int idDossier, int eta)
        {
            string msg = "";
            var dos = db.GetDossiers.Find(idDossier);
            if (!dos.ValiderConformite)
            {
                dos.EtapesDosier = eta + 1;
                dos.ValiderConformite = true;
                msg = "Dossier valider avec succes...";
            }
            else
            {
                dos.EtapesDosier = eta - 1;
                dos.ValiderConformite = false;
                msg = "Vous avez invalider le dossier...";
            }
            db.SaveChanges();
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TransfertDossier(int idSiteDest)
        {
            if (idSiteDest > 0)
            {
                try
                {
                    var agents = db.GetCompteBanqueCommerciales.Where(c=>c.IdStructure==idSiteDest).ToList();
                    var ges = from gs in agents select new { Id = gs.Id, Nom = gs.NomComplet };

                    return Json(ges, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {}
            }

            return Json(new object[] { "0", "" }, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult GetRefCorrespondantes(int DossierId,int fournisseurId,int deviseId,int clientId)
        {
            var banqueId = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);
            var refs = new List<ReferenceVModel>();
            var dossiers = db.GetDossiers.Include(d=>d.ReferenceExterne).Where(d => d.ClientId == clientId && d.FournisseurId == fournisseurId && d.DeviseMonetaireId == deviseId).ToList();
            dossiers.ForEach(d =>
            {
                try
                {
                    if (refs.Count==0)
                    {
                        refs.Add(new ReferenceVModel()
                        {
                            Id = (int)d.ReferenceExterneId,
                            Numero = d.ReferenceExterne.NumeroRef
                        });
                    }
                    else
                    {
                        if (refs.FirstOrDefault(r => r.Id == d.ReferenceExterneId) == null)
                            refs.Add(new ReferenceVModel()
                            {
                                Id = (int)d.ReferenceExterneId,
                                Numero = d.ReferenceExterne.NumeroRef
                            });
                    }
                }
                catch (Exception)
                {}
            });

            return Json(refs, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDossiersByRef(int RefId)
        {
            var banqueId = (Session["user"] as CompteBanqueCommerciale).Structure.BanqueId(db);
           
            var _ref = db.GetReferenceBanques.Include(r=>r.Dossiers).FirstOrDefault(r=>r.Id==RefId);
            var dossiers = new List<DossierVModel>();
            _ref.Dossiers.ToList().ForEach(d=> {
                dossiers.Add(new DossierVModel()
                {
                    Id=d.Dossier_Id,
                    Devise=d.DeviseMonetaire.Nom,
                    Fournisseur=d.Fournisseur.Nom,
                    Montant=d.Montant
                });
            });

            return Json(dossiers, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult TransfertDossier(FormCollection form)
        {
            var idDossier = int.Parse(form["Dossier_Id"]);
            var IdSiteOrignine = int.Parse(form["IdSiteOrignine"]);
            var IdSiteDestination = int.Parse(form["IdSiteDestination"]);
            var gestionnaireId = form["gestionnaireId"];
            try
            {
                var emailGes = "";
                var doss = db.GetDossiers.Include(d=>d.Client).FirstOrDefault(d=>d.Dossier_Id==idDossier);
                var siteOrg = db.Agences.Find(IdSiteOrignine);
                var siteDes = db.Agences.Find(IdSiteDestination);
                var ges = db.GetCompteBanqueCommerciales.Find(gestionnaireId);

                if (ges != null)
                    try
                    {
                        emailGes = ges.Email;
                    }
                    catch (Exception)
                    { }

                doss.IdSite = siteDes.Id;
                doss.Site = siteDes;
                db.SaveChanges();
                MailFunctions.SendMail(new MailModel()
                {
                    To=doss.Client.Email,
                    Subject = "Transfert de dossier de " +siteOrg.Nom+" vert "+siteDes.Nom,
                    Body = "Le dossier du n°" + doss.Dossier_Id + " du client " + doss.Client.Nom + " a été transféré du site " + siteOrg.Nom + " vert le site " + siteDes.Nom,
                    CC= emailGes
                }, db);
                emailGes = null;
                doss = null;
                siteOrg  =null;
                siteDes = null;
                ges = null;
            }
            catch (Exception ee)
            {}
            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult AddDouane(FormCollection form, 
            HttpPostedFileBase ImageDeclarImport = null, HttpPostedFileBase ImageDomicilImport = null)
        {
            var idDossier = int.Parse(form["Dossier_Id"]);

            if (idDossier>0)
            {
                var dossier = db.GetDossiers.Find(idDossier);
                dossier.DateModif = DateTime.Now;

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
                        { }
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
                           .Include(d => d.DeclarImport)
                           .Include(d => d.ReferenceExterne)
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
                ViewBag.BanqueId = new SelectList(bc, "Id", "Nom", dossier.IdSite);
                bc = null;
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

                var user = Session["user"] as CompteBanqueCommerciale;
                dossier.Permissions = new Dictionary<string, bool>();
                //foreach (var item in user.XRole.GetEntitee_Roles)
                var entitees = db.GetEntitees.ToList();

                foreach (var item in db.GetEntitee_Roles.Where(r => r.IdXRole == user.IdXRole))
                {
                    try
                    {
                        dossier.Permissions.Add(entitees.FirstOrDefault(e => e.Id == item.IdEntitee).Type, item.Ecrire);

                    }
                    catch (Exception)
                    { }
                }
                entitees = null;

                return View(dossier);
            }
            catch (Exception)
            {}


            return RedirectToAction("Index");
        }

        // POST: Dossiers1/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Traité,Dossier_Id,BanqueId,Montant,IdSite,DeviseMonetaireId,FournisseurId,ClientId,EtapesDosier")] Dossier dossier, HttpPostedFileBase ImageInstruction = null,
            HttpPostedFileBase ImageDeclarImport = null, HttpPostedFileBase ImageDomicilImport = null, HttpPostedFileBase ImageLettreEngage = null,
             HttpPostedFileBase ImageQuittancePay = null, HttpPostedFileBase ImageDocumentTransport = null)// IEnumerable<HttpPostedFileBase> FactureImages)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    dossier.DateDepotBank = DateTime.Now;
                    dossier.DateCreationApp = DateTime.Now;
                    dossier.DateModif = DateTime.Now;
                    dossier.IdAgentResponsableDossier = db.GetDossiers.Find(dossier.Dossier_Id).IdAgentResponsableDossier;
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
                           // docAttache.ClientId = User.Identity.GetUserId();
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
