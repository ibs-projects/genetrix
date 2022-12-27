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
using System.IO.Compression;
using System.Net.Mail;
using System.Configuration;
using OfficeOpenXml;

namespace genetrix.Controllers
{
    [Authorize]
    public class Dossiers_banqueController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        DateTime dateNow;

        public Dossiers_banqueController()
        {
            dateNow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "W. Central Africa Standard Time");
        }
        // GET: Dossiers1
        public async Task<ActionResult> Index(string st="",string comp="",string par="",int? referenceId=null,string interf="")
        {
            if (Session==null)
                return RedirectToAction("login", "auth");
            //var b1 = 0;
            //var tt = 1 / b1;
            var url = Session["current_url"];
            ViewBag.par = comp;
            Session["composant"] = string.IsNullOrEmpty(comp)?"st="+st:"comp="+comp;
            var _user = db.GetCompteBanqueCommerciales.Find((string)Session["userId"]);
            var structure = db.Structures.Find(_user.IdStructure);
            List<Dossier> getDossiers = new List<Dossier>();
            bool dossiersGroupes = false;
            //if (VariablGlobales.Access(_user, "dossier"))
            {
                int new_mail = 0, new_msg = 0, nb_broui = 0, nb_asoum = 0, nb_soum = 0, nb_encou = 0, nb_aapure = 0, nb_echu = 0,
                       nb_analyseconformite = 0, nb_attentetransmBEAC = 0, nb_encouanalyseBEAC = 0, nb_attentecouverture = 0,
                       nb_saisieencou = 0, nb_execute = 0, nb_apure = 0, nb_archive = 0, nb_supp = 0;

                if (!(_user is CompteBanqueCommerciale))
                    return RedirectToAction("login", "auth");

                try
                {
                    new_msg = db.GetMails.Where(m => m.AdresseDest == _user.Email).Count();
                }
                catch (Exception)
                { }

                Session["new_mail"] = new_mail;

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
                if (Session["Notifs"] ==null)
                {
                    //Session["Notifs"] = db.GetNotifications.Where(n => n.DestinataireId == _user.Id && !n.Lu); 
                }
                #endregion

                genetrix.Models.Banque banque = null;
                List<Dossier> dd = new List<Dossier>();
                ViewBag.navigation = string.IsNullOrEmpty(st) ? "brouil" : st;

                try
                {
                    //1=client; 2=banque;3=admin
                    byte profile = 0;
                    var banqueID = 0;
                    Structure site = null;
                    ViewData["comp"] = "aaa";

                    if ((string)Session["userType"] == "CompteBanqueCommerciale")
                    {
                        var agenceID = Convert.ToInt32(Session["IdStructure"]);

                        site = db.Structures.Find(agenceID);
                        
                        if (Convert.ToBoolean(Session["EstAdmin"]))
                        {
                            dd = db.GetDossiers.Where(d => d.IdSite == agenceID & d.EtapesDosier != 52).Include(d => d.Site).Include(d => d.DeviseMonetaire).Include(d => d.ReferenceExterne).ToList();
                        }
                        else
                        {

                            banqueID = structure.BanqueId(db);
                            try
                            {
                                banque = db.GetBanques.Find(banqueID);
                                ViewData["montantcv"] = banque.MontantDFX;
                            }
                            catch (Exception)
                            { }
                            var role = db.XtraRoles.Find(Session["IdXRole"]);
                            var gesId = (string)Session["userId"];
                            var estbackof = Convert.ToBoolean(Session["EstBackOff"]);
                            if (role == null)
                                return RedirectToAction("Login", "auth");
                            byte dfx_ref = 0;
                            if (st == "dfx") dfx_ref = 1;
                            if (st == "refinancement") dfx_ref = 2;
                             dd = VariablGlobales.UserBanqueDossiers(db,site,role,banqueID:banque.Id,gesId,estBackoffice: estbackof, montantDfx: banque.MontantDFX, dfx_ref:dfx_ref);
                        }
                    }
                    else if ((string)Session["userType"] == "CompteAdmin")
                    {
                        dd = db.GetDossiers.Include(d => d.Site).Include(d => d.DeviseMonetaire).Include(d => d.ReferenceExterne).ToList();
                    }
                    
                    if (string.IsNullOrEmpty(comp))
                    {
                        ViewBag.st = st;
                        switch (st)
                        {
                            case "devise-recu-ref":
                                getDossiers.AddRange(dd.Where(d => (d.EtapesDosier ==19 || d.EtapesDosier ==20) &&  d.DFX6FP6BEAC ==3 ).ToList());
                                st = "Refinancement (devise reçue)";
                                ViewBag.par = "beac=Tout;encours-beac=En cours de traitement BEAC;accord-beac=Accordés;devise-recu-ref=Devise reçue";
                                break;
                            case "usd-recus":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier ==9 && d.DeviseMonetaire.Nom=="USD").ToList());
                                st = "reçus (USD)";
                                ViewBag.par = "autres-recus=Tout;usd-recus=USD;recu-autr=Autres devises";
                                break;
                            case "recu-autr":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier ==9 && d.DeviseMonetaire.Nom !="USD" && d.DeviseMonetaire.Nom !="EUR").ToList());
                                st = "reçus (Autres devise)";
                                ViewBag.par = "autres-recus=Tout;usd-recus=USD;recu-autr=Autres";
                                break;
                            case "autres-recus":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier ==9 && d.DeviseMonetaire.Nom !="EUR").ToList());
                                st = "reçus (autres devises)";
                                ViewBag.par = "autres-recus=Tout;usd-recus=USD;recu-autr=Autres devises";
                                break;
                            case "usd-encours":
                                getDossiers.AddRange(dd.Where(d => d.DFX6FP6BEAC!=3 && d.EtapesDosier >= 10 && d.EtapesDosier < 22 && d.DeviseMonetaire.Nom == "USD").ToList());
                                st = "en cours (USD)";
                                ViewBag.par = "unique";
                                break;
                            case "autres-encours":
                                getDossiers.AddRange(dd.Where(d => d.DFX6FP6BEAC!=3 && d.EtapesDosier >= 10 && d.EtapesDosier < 22 && d.DeviseMonetaire.Nom != "USD").ToList());
                                st = "en cours (autres devises)";
                                ViewBag.par = "unique";
                                break;
                            case "conform":
                            case "à la conformité":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier >=6 && d.EtapesDosier <9).ToList());
                                st = "à la conformité";
                                ViewBag.par = "unique";
                                break;
                            case "transfert":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier >=9 || d.EtapesDosier <=19).ToList());
                                st = "au service transfert";
                                ViewBag.par = "unique";
                                break;
                            case "beac":
                                getDossiers.AddRange(dd.Where(d => d.DFX6FP6BEAC==3 && d.EtapesDosier >=15 && d.EtapesDosier <=21 ).ToList());
                                st = "BEAC";
                                ViewBag.par = "beac=Tout;encours-beac=En cours de traitement BEAC;accord-beac=Accordés;devise-recu-ref=Devise reçue";
                                break;
                            case "ref-encours":
                                getDossiers.AddRange(dd.Where(d =>d.DFX6FP6BEAC ==3 && d.EtapesDosier >=10 && d.EtapesDosier < 15).ToList());
                                st = "en cours (Refinancement)";
                                ViewBag.par = "encours9=Tout;dfx-encours=DFX=gold;ref-encours=Refinancement=cornflowerblue;fp-encours=Fond propre=deeppink";
                                break;
                            case "ref-accord":
                                getDossiers.AddRange(dd.Where(d => (d.DFX6FP6BEAC==3 || d.DeviseToString == "EUR") && d.EtapesDosier >= 16 && d.EtapesDosier <= 21 && banque.MontantDFX < d.MontantCV).ToList());
                                st = "accordés (Refinancement)";
                                ViewBag.par = "unique";
                                break;
                           
                            case "dfx-accord":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier >= 16 && d.EtapesDosier <= 21 && banque.MontantDFX >= d.MontantCV).ToList());
                                st = "accordés (DFX)";
                                ViewBag.par = "unique";
                                break;
                            case "rej":
                            case "rejetés":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier == -2 || d.EtapesDosier == -3).ToList());
                                st = "rejetés";
                                break;
                            case "dfx-recus":
                                //getDossiers.AddRange(dd.Where(d => d.EtapesDosier ==9 && d.MontantCV <=banque.MontantDFX).ToList());
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 9 &&d.DeviseMonetaire.Nom == "EUR" && d.MontantCV <= banque.MontantDFX).ToList());
                                st = "reçus (DFX)";
                                ViewBag.par = "eur-recus=Tout;dfx-recus=DFX=gold;ref-recu=Refinancement=cornflowerblue";
                                break; 
                            case "dfx-encours":
                                //getDossiers.AddRange(dd.Where(d => d.EtapesDosier ==9 && d.MontantCV <=banque.MontantDFX).ToList());
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier >= 10 && d.EtapesDosier<=20 && d.DFX6FP6BEAC!=2 &&d.DeviseMonetaire.Nom == "EUR" && d.MontantCV <= banque.MontantDFX).ToList());
                                st = "en cours (DFX)";
                                ViewBag.par = "encours9=Tout;dfx-encours=DFX=gold;ref-encours=Refinancement=cornflowerblue;fp-encours=Fond propre=deeppink";
                                break;
                            case "fp-encours":
                                //getDossiers.AddRange(dd.Where(d => d.EtapesDosier ==9 && d.MontantCV <=banque.MontantDFX).ToList());
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier >= 10 && d.EtapesDosier<=20 && d.DFX6FP6BEAC==2).ToList());
                                st = "en cours (Fonds propres)";
                                ViewBag.par = "encours9=Tout;dfx-encours=DFX=gold;ref-encours=Refinancement=cornflowerblue;fp-encours=Fond propre=deeppink";
                                break;
                            case "reçus9":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 9).ToList());
                                st = "reçus";
                                ViewBag.par = "eur-recus=Tout;autres-recus=Tout";
                                break;
                            case "encours9":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier >= 10 && d.EtapesDosier <= 20 && (d.EtapesDosier < 15 || d.DFX6FP6BEAC!=3) ).ToList());
                                st = "en cours";
                                ViewBag.par = "encours9=Tout;dfx-encours=DFX=gold;ref-encours=Refinancement=cornflowerblue;fp-encours=Fond propre=deeppink";
                                break;
                            case "eur-recus":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 9 && d.DeviseMonetaire.Nom == "EUR").ToList());
                                st = "reçus (EUR)";
                                ViewBag.par = "eur-recus=Tout;dfx-recus=DFX=gold;ref-recu=Refinancement=cornflowerblue";
                                break; 
                            case "eur-encours":
                                //getDossiers.AddRange(dd.Where(d => d.EtapesDosier ==9 && d.DeviseMonetaire.Nom == "EUR").ToList());
                                getDossiers.AddRange(dd.Where(d =>d.EtapesDosier >= 10 && d.DeviseMonetaire.Nom == "EUR" &&  (d.EtapesDosier < 15 || d.DFX6FP6BEAC != 3)).ToList());
                                st = "en cours (EURO)";
                                ViewBag.par = "unique";
                                break; 
                            case "ref-recu":
                                //getDossiers.AddRange(dd.Where(d => d.EtapesDosier ==9 && d.MontantCV >banque.MontantDFX).ToList());
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 9 && d.DeviseMonetaire.Nom == "EUR" && d.MontantCV > banque.MontantDFX).ToList());

                                st = "reçus (Refinancements)";
                                ViewBag.par = "eur-recus=Tout;dfx-recus=DFX=gold;ref-recu=Refinancement=cornflowerblue";
                                break;
                            case "encours":
                            case "en cours":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier > 5 && d.EtapesDosier < 16).ToList());
                                st = "en cours";
                                break;
                            case "assoum":
                            case "à soumettre":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 0).ToList());
                                st = "à soumettre";
                                break;
                            case "aapurer":
                            case "à apurer":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 22 || d.EtapesDosier == 23|| d.EtapesDosier == 230|| d.EtapesDosier == 231|| d.EtapesDosier == -230|| d.EtapesDosier == -231
                                || d.EtapesDosier == 25|| d.EtapesDosier == 250|| d.EtapesDosier == -250).ToList());
                                foreach (var d in getDossiers.Where(d=>d.EtapesDosier==22))
                                {
                                    try
                                    {
                                       await MailFunctions.ChangeEtapeDossier(banqueID, (string)Session["userId"], 23, d, db, structure);
                                    }
                                    catch (Exception)
                                    {}
                                }
                                st = "à apurer";
                                ViewData["comp"] = "apurement";
                                break;
                            case "aapurer-av":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 250 || d.EtapesDosier == 231 || d.EtapesDosier == 230).ToList());
                                st = "à apurer (attente validation)";
                                ViewData["comp"] = "apurement";
                                break;  
                            case "aapurer-beac":
                                getDossiers.AddRange(dd.Where(d =>d.EtapesDosier == 232).ToList());
                                st = "à apurer (en cours d'analyse BEAC)";
                                ViewData["comp"] = "apurement";
                                break; 
                            case "aapurer-acc":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 22|| d.EtapesDosier == 23|| d.EtapesDosier == 25|| d.EtapesDosier == -230|| d.EtapesDosier == -231||  d.EtapesDosier == -232|| d.EtapesDosier == -250).ToList());
                                st = "à apurer (à completer)";
                                ViewData["comp"] = "apurement";
                                break;
                            case "aapurer-rej":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier == -250|| d.EtapesDosier == -230).ToList());
                                st = "à apurer (rejetés)";
                                ViewData["comp"] = "apurement";
                                break;
                            case "ech-av":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 250).ToList());
                                st = "échus (attente validation)";
                                ViewData["comp"] = "apurement";
                                break; 
                            case "ech-rej":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 250).ToList());
                                st = "échus (rejetés)";
                                ViewData["comp"] = "apurement";
                                break;
                            case "apure":
                            case "apuré":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 24).ToList());
                                st = "apuré";
                                ViewData["comp"] = "apurement";
                                break;
                            case "archive":
                            case "archivé":
                                getDossiers.AddRange(dd.Where(d =>  d.EtapesDosier == 27).ToList());
                                st = "archivé";
                                break;
                            case "echu":
                            case "échu":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 25).ToList());
                                st = "échu";
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
                            case "atten_couv":
                            case "env_bac":
                            case "encours-beac":
                                getDossiers.AddRange(dd.Where(d => d.DFX6FP6BEAC==3 &&d.EtapesDosier == 15).ToList());
                                st = "en cours de traitement BEAC";
                                ViewBag.par = "beac=Tout;encours-beac=En cours de traitement BEAC;accord-beac=Accordés;devise-recu-ref=Devise reçue";
                                dossiersGroupes = true;
                                break;
                            case "accord-beac":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier >= 16 && d.EtapesDosier <19 && d.DFX6FP6BEAC==3).ToList());
                                st = "en cours de traitement BEAC";
                                dossiersGroupes = true;
                                ViewBag.par = "beac=Tout;encours-beac=En cours de traitement BEAC;accord-beac=Accordés;devise-recu-ref=Devise reçue";
                                break;
                            case "rej-beac":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier >= 15 && d.EtapesDosier <= 18 && d.MontantCV>banque.MontantDFX).ToList());
                                st = "rejetés (BEAC)";
                                ViewBag.par = "beac=Tout;encours-beac=En cours de traitement BEAC;accord-beac=Accordés;rej-beac=rejetés";
                                dossiersGroupes = true;
                                break;
                            case "aenv_bac":
                            case "à envoyer BEAC":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 5).ToList());
                                st = "à envoyer BEAC";
                                dossiersGroupes = true;
                                break;
                            case "accord":
                            case "accordés":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 19 || d.EtapesDosier ==20).ToList());
                                //dossiersGroupes = true;
                                st = "accordés";
                                break;
                            case "saisie_encour":
                            case "saisie en cours":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 8).ToList());
                                dossiersGroupes = true;
                                st = "saisie en cours";
                                break;
                            case "encourstf":
                            case "en cours de traitement service transfert":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 4).ToList());
                                st = "en cours de traitement service transfert";
                                //dossiersGroupes = true;
                                break;
                            case "tr":
                            case "traite":
                            case "traités":
                                getDossiers.AddRange(dd.Where(d =>d.EtapesDosier==22 || d.EtapesDosier==23).ToList());
                                st = "traités";
                                ViewBag.par = "traite=Tout;traite-dfx=Traités (DFX);accord-beac=Accordés;traite-fp=Fond proptres";
                                break;
                            case "traite-dfx":
                                getDossiers.AddRange(dd.Where(d => (d.EtapesDosier == 22 || d.EtapesDosier == 23) && d.MontantCV <= banque.MontantDFX && d.DeviseToString=="EUR").ToList());
                                st = "traités (DFX)";
                                ViewBag.par = "traite=Tout;traite-dfx=Traités (DFX);traite-ref=Traités (Refinancement);traite-fp=Fond proptres";
                                break;
                            case "traite-ref":
                                getDossiers.AddRange(dd.Where(d => (d.EtapesDosier == 22 || d.EtapesDosier == 23) && d.DFX6FP6BEAC==3 && (d.MontantCV > banque.MontantDFX || d.DeviseToString != "EUR")).ToList());
                                st = "traités (Refinencement)";
                                ViewBag.par = "traite=Tout;traite-dfx=Traités (DFX);traite-ref=Traités (Refinancement);traite-fp=Fonds proptres";
                                break;
                            case "traite-fp":
                                getDossiers.AddRange(dd.Where(d => (d.EtapesDosier == 22 || d.EtapesDosier == 23) && d.DFX6FP6BEAC!=3 && (d.MontantCV > banque.MontantDFX || d.DeviseToString !="EUR")).ToList());
                                st = "traités (Fonds propres)";
                                ViewBag.par = "traite=Tout;traite-dfx=Traités (DFX);traite-ref=Traités (Refinancement);traite-fp=Fonds propres";
                                break;
                            case "untr":
                            case "non traités":
                                getDossiers.AddRange(dd.Where(d => !d.Traité).ToList());
                                st = "non traités";
                                break;
                            case "port":
                            case "mon portefeuille":
                                var userId = (string)Session["userId"];
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
                                //getDossiers.AddRange(db.GetDossiers.ToList());
                                getDossiers.AddRange(dd.ToList());
                                st = "tous";
                                break;
                            case "conf_st":
                            case "transmis":
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier >= 10 && d.EtapesDosier <= 18).ToList());
                                st = "transmis";
                                break;
                            default:
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier == site.NiveauDossier).ToList());
                                st = "reçus";
                                break;
                            
                        }
                        st = "Dossiers " + st;
                    }
                    else
                    {
                        int compNum = int.Parse(comp.Split('_')[0]);
                        int compMin = int.Parse(comp.Split('_')[1]);
                        int compMax= int.Parse(comp.Split('_')[2]);
                        var composant = db.GetComposants.FirstOrDefault(c=>c.Numero==compNum);
                        st = composant.Description;
                        if ( !Convert.ToBoolean(Session["EstBackOffice"]))
                        {
                            if (comp == "20_9_10" && structure.NiveauDossier==6)
                            {
                                getDossiers.AddRange(dd.Where(d => d.EstPasséConformite && d.EtapesDosier<22 && (dateNow- (DateTime)d.Date_Etape9).Days<=31).ToList());
                            }
                            else
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier >= compMin && d.EtapesDosier <= compMax || (compNum == 19 && d.EtapesDosier < -1)).ToList());
                        }
                        else
                        {
                            if (comp=="1_1_1")
                            {
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 4 && !d.ValiderDouane).ToList());
                            }
                            else if (comp=="2_2_5")
                            {
                                getDossiers.AddRange(dd.Where(d => d.EtapesDosier == 5 && (d.DomicilImport==null || d.DeclarImport==null || !d.ValiderDouane)).ToList());
                            }
                            else if (comp=="19_6_22")
                            {
                                getDossiers.AddRange(dd.Where(d => (d.EtapesDosier == 4 || d.EtapesDosier == 5) && d.DomicilImport !=null && d.DeclarImport !=null && d.ValiderDouane).ToList());
                            }
                        }
                    }
                }
                catch (Exception e)
                { }
                //dd[0].EtapesDosier = 5;
                //db.SaveChanges();
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
            if (dossiersGroupes && false)
                return RedirectToAction("Index", "ReferenceBanques", new { st = ViewBag.navigation });

            if (referenceId !=null)
            {
                return View("Index", getDossiers.Where(d => d.ReferenceExterneId == referenceId).ToList());
            }

            ViewBag.navigation = "rien";
            ViewBag.navigation_msg = "Liste dossiers";
            ViewBag.pages = "dossier_bank_index";
            Session["IndexUrl"] = Request.Url.AbsoluteUri;
            return View("Index",getDossiers.ToList());
        }

        public ActionResult MasterDetails(int iddossier)
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult GetcvsFile(FormCollection form)
        {
            var exportFilePath = this.Server.MapPath("~/dfx_recap.xlsx");

            FileInfo fi = new FileInfo(exportFilePath);
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            using (ExcelPackage excelPackage = new ExcelPackage(fi))
            {
                ExcelWorksheet firstWorksheet = excelPackage.Workbook.Worksheets[1];
                //get data
                ///Entete
                var annexe = form["annexe"];
                var numDFX = form["numDFX"];
                var pays = form["pays"];
                var banque = form["banque"];
                var dateDebut = form["dateDebut"];
                var dateFin = form["dateFin"];
                var reference = form["reference"];

               firstWorksheet.Cells["A1"].Value= annexe;
               firstWorksheet.Cells["A3"].Value= numDFX;
               firstWorksheet.Cells["B7"].Value= pays;
               firstWorksheet.Cells["C8"].Value= banque;
               firstWorksheet.Cells["B9"].Value= dateDebut.ToString() +" AU "+ dateFin;
               firstWorksheet.Cells["A10"].Value= "Référence MT 298 : "+ reference;

                int i = 13;
                foreach (var item in form)
                {
                    try
                    {
                        int iddossier = Convert.ToInt32(item);
                        var doss=db.GetDossiers.Find(iddossier);
                        firstWorksheet.Cells["A"+i].Value = i;
                        firstWorksheet.Cells["B"+i].Value = doss.DateDepotBank.Value.ToString("dd/MM/yyyy");
                        firstWorksheet.Cells["C"+i].Value = doss.GetClient;
                        firstWorksheet.Cells["D"+i].Value = doss.GetFournisseur;
                        firstWorksheet.Cells["E"+i].Value = doss.Motif;
                        firstWorksheet.Cells["F"+i].Value = doss.DeviseToString;
                        firstWorksheet.Cells["G"+i].Value = doss.Montant;
                        firstWorksheet.Cells["H"+i].Value = doss.DeviseMonetaire.ParitéXAF;
                        firstWorksheet.Cells["I"+i].Value = doss.MontantCV;
                        firstWorksheet.Cells["J"+i].Value = form["BIC_Correspondant"];

                    }
                    catch (Exception)
                    {}
                }
                //Save your file
               // excelPackage.Save();
            }

            return File(exportFilePath, "text/csv");
        }
        
        [HttpGet]
        public ActionResult GetcvsFile(int dfx_iden)
        {
            var exportFilePath = this.Server.MapPath("~/dfx_recap.xlsx");
            var dfx = db.GetDFXs.Find(dfx_iden);
            if (dfx == null) return null;
            try
            {
                var _id=Convert.ToInt32(dfx.CorrespondantB);
                dfx.CorrespondantB = db.GetCorrespondants.Find(_id).NomBanque;
            }
            catch (Exception)
            {}
            FileInfo fi = new FileInfo(exportFilePath);
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            string dateFin = "", dateDebut = "";
            using (ExcelPackage excelPackage = new ExcelPackage(fi))
            {
                ExcelWorksheet firstWorksheet = excelPackage.Workbook.Worksheets[1];
                //get data
                ///Entete
                var annexe = dfx.NumeroAnnexe;
                var numDFX = dfx.Numero;
                var pays = dfx.Pays;
                var banque = dfx.Banque.Nom;
                dateDebut = dfx.DateDebut.ToString("dd/MM/yyyy");
                dateFin = dfx.DateFin.ToString("dd/MM/yyyy");
                var reference = dfx.NumeroRef;

               firstWorksheet.Cells["A1"].Value= annexe;
               firstWorksheet.Cells["A3"].Value= numDFX;
               firstWorksheet.Cells["B7"].Value= pays;
               firstWorksheet.Cells["C8"].Value= banque;
               firstWorksheet.Cells["B9"].Value= dateDebut +" AU "+ dateFin;
               firstWorksheet.Cells["A10"].Value= "Référence MT 298 : "+ reference;

                int i = 1,j=13;
                foreach (var doss in dfx.Dossiers)
                {
                    try
                    {
                        firstWorksheet.Cells["A"+j].Value = i;
                        firstWorksheet.Cells["B"+j].Value = doss.DateDepotBank.Value.ToString("dd/MM/yyyy");
                        firstWorksheet.Cells["C"+j].Value = doss.GetClient;
                        firstWorksheet.Cells["D"+j].Value = doss.GetFournisseur;
                        firstWorksheet.Cells["E"+j].Value = doss.Motif;
                        firstWorksheet.Cells["F"+j].Value = doss.DeviseToString;
                        firstWorksheet.Cells["G"+j].Value = doss.Montant;
                        firstWorksheet.Cells["H"+j].Value = doss.DeviseMonetaire.ParitéXAF;
                        firstWorksheet.Cells["I"+j].Value = doss.MontantCV;
                        firstWorksheet.Cells["J"+j].Value = dfx.CorrespondantB;
                        i++;
                        j++;

                    }
                    catch (Exception)
                    {}
                }
                //Save your file
                excelPackage.Save();
            }
            try
            {
                dfx.Telechargements++;
                db.SaveChanges();
            }
            catch (Exception)
            { }
            return File(exportFilePath, "text/csv", dateDebut + " AU " + dateFin + ".csv") ;
        }

        [HttpPost]
        public async Task<ActionResult> SetMiseEnDemeure(Dossier dossier, HttpPostedFileBase EnDemeure = null, HttpPostedFileBase EnDemeure2 = null)
        {
            try
            {
                dossier = await db.GetDossiers
                                     .Include(d => d.EnDemeure)
                                       .Include(d => d.EnDemeure2)
                                     .FirstOrDefaultAsync(d => d.Dossier_Id == dossier.Dossier_Id);
                //Mise en demeure
                if (EnDemeure != null)
                {
                    if (!string.IsNullOrEmpty(EnDemeure.FileName))
                    {
                        var imageModel = new genetrix.Models.ImageDocumentAttache();
                        imageModel.Titre = "Mise en demeure";

                        string chemin = Path.GetFileNameWithoutExtension(EnDemeure.FileName);
                        string extension = Path.GetExtension(EnDemeure.FileName);
                        chemin = imageModel.Titre + extension;
                        imageModel.Url = Path.Combine(CreateNewFolderDossier(dossier.ClientId.ToString(), dossier.Intitulé), chemin);
                        if (dossier.EnDemeure != null && dossier.Get_EnDemeure != "#")
                        {
                            //Supprime l'ancienne image
                            System.IO.File.Delete(dossier.Get_EnDemeure);
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
                        docAttache.Nom = "Mise en demeure";
                        //docAttache.ClientId = User.Identity.GetUserId();
                        docAttache.GetImageDocumentAttaches = new List<ImageDocumentAttache>();
                        docAttache.GetImageDocumentAttaches.Add(imageModel);
                        dossier.EnDemeure = docAttache;
                        docAttache.Dossier = dossier;

                        chemin = null;
                        extension = null;

                        db.GetDocumentAttaches.Add(docAttache);
                        db.GetImageDocumentAttaches.Add(imageModel);
                        db.SaveChanges();
                        EnDemeure.SaveAs(imageModel.Url);
                    }
                }

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
            return RedirectToAction("Details", new { id = dossier.Dossier_Id });
        }

        [HttpGet]
        public ActionResult GetcvsFileByEmail(int dfx_iden,string emails)
        {
            //var structure = db.Structures.Find(Session["IdStructure"]);
            //var idbanque = structure.BanqueId(db);
            //var banque = db.GetBanques.Find(idbanque);

            var exportFilePath = this.Server.MapPath("~/dfx_recap.xlsx");
            var dfx = db.GetDFXs.Find(dfx_iden);
            if (dfx == null) return null;
            try
            {
                var _id=Convert.ToInt32(dfx.CorrespondantB);
                dfx.CorrespondantB = db.GetCorrespondants.Find(_id).NomBanque;
            }
            catch (Exception)
            {}
            FileInfo fi = new FileInfo(exportFilePath);
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            string dateFin = "", dateDebut = "";
            int i = 1, j = 13;
            using (ExcelPackage excelPackage = new ExcelPackage(fi))
            {
                ExcelWorksheet firstWorksheet = excelPackage.Workbook.Worksheets[1];
                //get data
                ///Entete
                ///
                var annexe = dfx.NumeroAnnexe;
                var numDFX = dfx.Numero;
                var pays = dfx.Pays;
                var banque = dfx.Banque.Nom;
                dateDebut = dfx.DateDebut.ToString("dd/MM/yyyy");
                dateFin = dfx.DateFin.ToString("dd/MM/yyyy");
                var reference = dfx.NumeroRef;

                //suppression des precedentes données
                //if (i < dfx.Banque.FileCourierIns)
                {
                    for (int _i = 13; _i < dfx.Banque.FileCourierIns + 30; _i++)
                    {
                        try
                        {
                            firstWorksheet.Cells["A" + _i ].Value = "";
                            firstWorksheet.Cells["B" + _i ].Value = "";
                            firstWorksheet.Cells["C" + _i ].Value = "";
                            firstWorksheet.Cells["D" + _i ].Value = "";
                            firstWorksheet.Cells["E" + _i ].Value = "";
                            firstWorksheet.Cells["F" + _i ].Value = "";
                            firstWorksheet.Cells["G" + _i ].Value = "";
                            firstWorksheet.Cells["H" + _i ].Value = "";
                            firstWorksheet.Cells["I" + _i ].Value = "";
                            firstWorksheet.Cells["J" + _i ].Value = "";
                        }
                        catch (Exception)
                        { }
                    }
                }

                firstWorksheet.Cells["A1"].Value= annexe;
               firstWorksheet.Cells["A3"].Value= numDFX;
               firstWorksheet.Cells["B7"].Value= pays;
               firstWorksheet.Cells["C8"].Value= banque;
               firstWorksheet.Cells["B9"].Value= dateDebut +" AU "+ dateFin;
               firstWorksheet.Cells["A10"].Value= "Référence MT 298 : "+ reference;

                foreach (var doss in dfx.Dossiers)
                {
                    try
                    {
                        firstWorksheet.Cells["A"+j].Value = i;
                        firstWorksheet.Cells["B"+j].Value = doss.DateDepotBank.Value.ToString("dd/MM/yyyy");
                        firstWorksheet.Cells["C"+j].Value = doss.GetClient;
                        firstWorksheet.Cells["D"+j].Value = doss.GetFournisseur;
                        firstWorksheet.Cells["E"+j].Value = doss.Motif;
                        firstWorksheet.Cells["F"+j].Value = doss.DeviseToString;
                        firstWorksheet.Cells["G"+j].Value = doss.Montant;
                        firstWorksheet.Cells["H"+j].Value = doss.DeviseMonetaire.ParitéXAF;
                        firstWorksheet.Cells["I"+j].Value = doss.MontantCV;
                        firstWorksheet.Cells["J"+j].Value = dfx.CorrespondantB;
                        i++;
                        j++;

                    }
                    catch (Exception)
                    {}
                }
                
                //Save your file
                excelPackage.Save();
            }
            try
            {
                dfx.Banque.FileCourierIns = i;
                dfx.Telechargements++;
                db.SaveChanges();
            }
            catch (Exception)
            { }            
            var nom = (string)Session["userName"];
            string msg = "ok";
            using (var memoryStream = new MemoryStream())
            {
                using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    try
                    {
                        //CSV
                        ziparchive.CreateEntryFromFile(exportFilePath, "DFX420_" + dateDebut.Replace("/","") + "_AU_" + dateFin.Replace("/", "") + ".csv");;
                    }
                    catch (Exception e)
                    { }
                }
                // Send Email with zip as attachment.
                string emailsender = ConfigurationManager.AppSettings["emailsender"].ToString();
                string emailSenderPassword =ConfigurationManager.AppSettings["emailSenderPassword"].ToString();

                using (MailMessage mail = new MailMessage(emailsender, User.Identity.Name))
                {
                    try
                    {
                        mail.Subject = "Extraction du DFX 420 de référence " + dfx.NumeroRef;
                        mail.Body = $"Le DFX 420 de référence {dfx.NumeroRef} a été extrait ce jour {dateNow} par l'agent {nom}";
                        try
                        {
                            emails = emails.Replace("Emails:", "");
                            foreach (var item in emails.Split(';'))
                            {
                                if (!string.IsNullOrEmpty(item))
                                    mail.CC.Add(item.Trim());
                            }
                        }
                        catch (Exception)
                        { }
                        mail.Attachments.Add(new Attachment(new MemoryStream(memoryStream.ToArray()), "Document.zip"));

                        mail.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = ConfigurationManager.AppSettings["smtpserver"].ToString();
                        smtp.EnableSsl =Convert.ToBoolean(ConfigurationManager.AppSettings["IsSSL"]);
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(emailsender, emailSenderPassword);
                        smtp.Port = Convert.ToInt16(ConfigurationManager.AppSettings["portnumber"]);
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.Send(mail);
                    }
                    catch (Exception e)
                    { msg = e.InnerException.Message; }

                }
            }

            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        private void UpdateCSVData()
        {
            var exportFilePath = this.Server.MapPath("~/dfx_recap.xlsx");
            FileInfo fi = new FileInfo(exportFilePath);
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            using (ExcelPackage excelPackage = new ExcelPackage(fi))
            {
                //Get a WorkSheet by index. Note that EPPlus indexes are base 1, not base 0!
                ExcelWorksheet firstWorksheet = excelPackage.Workbook.Worksheets[1];

                //Get a WorkSheet by name. If the worksheet doesn't exist, throw an exeption
                //ExcelWorksheet namedWorksheet = excelPackage.Workbook.Worksheets["DFX"];

                //If you don't know if a worksheet exists, you could use LINQ,
                //So it doesn't throw an exception, but return null in case it doesn't find it
                //ExcelWorksheet anotherWorksheet =
                //    excelPackage.Workbook.Worksheets.FirstOrDefault(x => x.Name == "Feuille2");

                //Get the content from cells A1 and B1 as string, in two different notations
                string valA1 = firstWorksheet.Cells["A1"].Value.ToString();

                //Save your file
                excelPackage.Save();
            }
        }

        private IEnumerable<string[]> GetAllDataFromCSVFile(string FileName)
        {
            string str;
            using (StreamReader rd = new StreamReader(FileName))
            {
                while ((str = rd.ReadLine()) != null)
                {
                    yield return str.Split(',');
                }
            }
        }

        public ActionResult OpenXAFViewer(string idsDossiers, int? idDfx = null)
        {
            DFX dFX = null;
            if(idDfx != null)
            {
                dFX=db.GetDFXs.Find(idDfx);
            }
            if(dFX == null)dFX = new DFX();
            dFX.GetIdsDossiers = idsDossiers;
            var lesId=idsDossiers.Split(';');
            foreach (var item in lesId)
            {
                try
                {
                    int id = Convert.ToInt32(item);
                    var doss = db.GetDossiers.Find(id);
                    if (doss != null)
                        dFX.Dossiers.Add(doss);
                }
                catch (Exception)
                {}
            }

            return PartialView(dFX);
        }

        private DataTable GetCSVData(string FileName)
        {
            var values = GetAllDataFromCSVFile(FileName);
            DataTable dt = new DataTable();
            int j = 0;
            foreach (var item in values.FirstOrDefault())
            {
                if (!dt.Columns.Contains(item))
                {
                    dt.Columns.Add(item.Replace("\"", ""));
                    j++;
                }
            }
            var allvalues = values.Skip(1).ToList();
            for (int i = 0; i < allvalues.Count; i++)
            {
                dt.Rows.Add("1222"+allvalues[i].Take(dt.Columns.Count).ToArray());
                //remove double quote from data
                for (int co = 0; co < j; co++)
                {
                    var d = dt.Rows[i];
                    d[2] = 150;
                    dt.Rows[i][co] ="3333"+ dt.Rows[i][co].ToString().Replace("\"", "");

                }

            }
            return dt;
        }

        public ActionResult Etats()
        {
            return View(new Dossier());
        }

        public ActionResult ActionsVew(int id)
        {
            Dossier dossier = new Dossier();
            try
            {
                dossier = db.GetDossiers.Find(id);
            }
            catch (Exception)
            {}
            return PartialView(dossier);
        }

        public ActionResult DossiersGroupes(int? idClient=null)
        {
            List<Dossier> getDossiers = new List<Dossier>();
            IEnumerable<Dossier> dd = new List<Dossier>();
            var structure = db.Structures.Find(Session["IdStructure"]);
            var idbanque = structure.BanqueId(db);

            var banque =db.GetBanques.Find(idbanque);
            if (idClient==null || idClient==0)
            {
                dd = db.GetDossiers.Where(d => d.Site.BanqueId(db) == banque.Id && d.EtapesDosier != 52 && d.ReferenceExterneId!=null).Include(d => d.Site).Include(d => d.DeviseMonetaire).Include(d => d.ReferenceExterne);//.ToList();

                //foreach (var item in db.GetDossiers.Include(d => d.Site).Include(d => d.DeviseMonetaire).Include(d => d.ReferenceExterne))
                //{
                //    if (item.IdSite == agence.Id && item.EtapesDosier != 52 && item.ReferenceExterneId != null)
                //        dd.Add(item);
                //}
            }
            else
            {
                dd = db.GetDossiers.Where(d => d.IdSite == banque.Id & d.EtapesDosier != 52 && d.ClientId==idClient).Include(d => d.Site).Include(d => d.DeviseMonetaire).Include(d => d.ReferenceExterne);
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
            if (Session == null)
                return RedirectToAction("login", "auth");
            if (id == null || id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.navigation = "rien";
            ViewBag.navigation_msg = "Details dossier ";

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
                               .Include(d => d.Fournisseur)
                           .FirstAsync(d => d.Dossier_Id == id);

                if (dossier == null)
                {
                    return RedirectToAction("Index");
                }

                //dossier.ValiderConformite = false;
                //dossier.ReferenceExterneId = null;
                //db.SaveChanges();
                Structure structure = null;
                double montantDfx = 0;
                int min = 0, banqueId = 0;
                try
                {
                    montantDfx = Convert.ToDouble(Session["MontantDFX"]);
                    banqueId = Convert.ToInt32(Session["banqueId"]);
                    dossier.BanqueToString = Session["banqueName"].ToString();
                }
                catch (Exception)
                {}
                try
                {
                    var idStruct = Session["IdStructure"];
                    structure = db.Structures.Find(idStruct);
                    min = (int)structure.NiveauDossier;
                }
                catch (Exception)
                { }
               
                //supprime la reference si dossier descendu
                if (dossier.EtapesDosier < 12 && dossier.MontantCV > montantDfx && dossier.ReferenceExterneId!=null)
                {
                    try
                    {
                        if (dossier.ReferenceExterne != null)
                        {
                            dossier.ReferenceExterne.DepotBEAC = null;
                            dossier.ReferenceExterne.Accordé = false;
                            dossier.ReferenceExterne.EnvoieBEAC = false;
                        }
                        //dossier.ReferenceExterne = null;
                        //dossier.ReferenceExterneId = null;
                        db.SaveChanges();
                    }
                    catch (Exception)
                    {}
                }
                
                //dossier reçu si le gestionnaire ou responsable du dossier ouvre celui-ci
                if (dossier.EtapesDosier==1)
                {
                    if (dossier.IdGestionnaire == User.Identity.GetUserId() || dossier.IdAgentResponsableDossier == User.Identity.GetUserId())
                    {
                        // si gestionnaire alors statut: verification encours
                       await MailFunctions.ChangeEtapeDossier(banqueId, User.Identity.GetUserId(), 3, dossier, db, structure);
                        db.SaveChanges();
                    }
                }
                if (dossier.EtapesDosier==2)
                {
                    //if (dossier.IdGestionnaire == User.Identity.GetUserId()||dossier.IdAgentResponsableDossier == User.Identity.GetUserId())
                    if (dossier.IdGestionnaire == User.Identity.GetUserId()||dossier.GetCurrenteResponsableID(min) == User.Identity.GetUserId())
                    {
                        // si gestionnaire alors statut: verification encours
                      await  MailFunctions.ChangeEtapeDossier(banqueId, User.Identity.GetUserId(), 3, dossier, db, structure);
                    }
                    db.SaveChanges();
                }
                else if (dossier.EtapesDosier == 4 && dossier.ValiderDouane)
                {
                    //documents douane
                    if (dossier.DeclarImport != null && dossier.DomicilImport != null)
                    {
                      await  MailFunctions.ChangeEtapeDossier(banqueId, User.Identity.GetUserId(), 5, dossier, db, structure);
                    }
                }
                if (dossier.EtapesDosier == 6 && structure.NiveauDossier == 6)
                {
                    //conformité: reçu
                  await  MailFunctions.ChangeEtapeDossier(banqueId, User.Identity.GetUserId(), 7, dossier, db, structure);
                }
                else if (dossier.EtapesDosier == 7 && dossier.GetCurrenteResponsableID(min) == User.Identity.GetUserId())
                {
                    //conformité: verification en cours
                  await  MailFunctions.ChangeEtapeDossier(banqueId, User.Identity.GetUserId(), 8, dossier, db, structure);
                }
                else if (dossier.EtapesDosier == 9 && structure.NiveauDossier==9)
                {
                    //service transfert: reçu
                   await MailFunctions.ChangeEtapeDossier(banqueId, User.Identity.GetUserId(), 10, dossier, db, structure);
                }
                //else if (dossier.EtapesDosier == 10 && dossier.GetCurrenteResponsableID(min) == User.Identity.GetUserId())
                //{
                //    //service transfert: verification en cours
                //   await MailFunctions.ChangeEtapeDossier(banque.Id, User.Identity.GetUserId(), 11, dossier, db, structure);
                //}

                //contrôle sur les valeurs possibles de DFX6FP6BEAC
                //if(dossier.DFX6FP6BEAC==3)
                //{
                //    if (dossier.MontantCV <= banque.MontantDFX)
                //    {
                //        dossier.DFX6FP6BEAC = 1;
                //        db.SaveChanges(); 
                //    }
                //}else 
                //if(dossier.DFX6FP6BEAC == 1)
                //{
                //    if(dossier.DeviseToString!="EUR" || dossier.MontantCV > banque.MontantDFX)
                //    {
                //        dossier.DFX6FP6BEAC = 3;
                //        db.SaveChanges();
                //    }
                //}

                if (dossier.ReferenceExterneId != null && dossier.ReferenceExterneId != 0 && unit!="ok")
                return RedirectToAction("Details", "ReferenceBanques", new { id = dossier.ReferenceExterneId });

                ViewBag.navigation = "rien";
                ViewBag.navigation_msg = "Details du dossier";

                if (Session== null) return RedirectToAction("indexbanque", "index");
                var user = db.GetCompteBanqueCommerciales.Find(Session["userId"]);

                #region A verifier
                if (false)
                {
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

                    var dd = new List<Agence>();
                    db.Agences.ToList().ForEach(c =>
                    {
                        try
                        {
                            if (c.BanqueId(db) == structure.BanqueId(db) && c.Id != user.IdStructure)
                                dd.Add(c);
                        }
                        catch (Exception)
                        { }
                    });

                    ViewBag.sites = new SelectList(dd, "Id", "Nom");
                    entitees = null;
                    dd = null;  
                }
                #endregion
                try
                {
                    var site = db.Structures.Find(user.IdStructure);
                    ViewBag.userSIteMinNiveau = site.NiveauDossier;
                    ViewBag.userSIteMaxNiveau = site.NiveauMaxDossier;
                    ViewBag.niveauSite = site.NiveauMaxDossier;
                }
                catch (Exception)
                {}
                try
                {
                    dossier.Fournisseur.RCCM = db.GetDocumentations.FirstOrDefault(d => d.Nom == "RCCM" && d.IdFournisseur == id);
                    dossier.Fournisseur.ListeGerants = db.GetDocumentations.FirstOrDefault(d => d.Nom == "ListeGerants" && d.IdFournisseur == id);
                    dossier.Fournisseur.AutresDocuments = db.GetFournisseurs.Include(f=>f.AutresDocuments).FirstOrDefault(f=>f.Id==dossier.FournisseurId).AutresDocuments;
                }
                catch (Exception)
                {}
                try
                {
                    dossier.Client.PlanLSS = db.GetDocumentations.FirstOrDefault(d => d.ClientId == dossier.ClientId && d.Nom == "PlanLSS");
                    dossier.Client.FicheKYC = db.GetDocumentations.FirstOrDefault(d => d.ClientId == dossier.ClientId && d.Nom == "FicheKYC");
                    dossier.Client.RCCM_Cl = db.GetDocumentations.FirstOrDefault(d => d.ClientId == dossier.ClientId && d.Nom == "RCCM");
                    dossier.Client.Statut = db.GetDocumentations.FirstOrDefault(d => d.ClientId == dossier.ClientId && d.Nom == "Statut");
                    dossier.Client.ProcesVerbal = db.GetDocumentations.FirstOrDefault(d => d.ClientId == dossier.ClientId && d.Nom == "ProcesVerbal");
                    dossier.Client.EtatFinanciers = db.GetDocumentations.FirstOrDefault(d => d.ClientId == dossier.ClientId && d.Nom == "EtatFinanciers");
                    dossier.Client.AtestationHinneur = db.GetDocumentations.FirstOrDefault(d => d.ClientId == dossier.ClientId && d.Nom == "AtestationHinneur");
                    dossier.Client.AutresDocuments = db.GetClients.Include(c=>c.AutresDocuments).FirstOrDefault(c=>c.Id==dossier.ClientId).AutresDocuments;
                }
                catch (Exception)
                {}
                ViewBag.pages = "dossier_bank_details";
                return View(dossier);
            }
            catch (Exception ee)
            { }

            return RedirectToAction("Index");
        }

        // GET: Dossiers1/Create
        public ActionResult Create()
        {
            if (Session == null) return RedirectToAction("Login", "Account");
            int _clientId = Convert.ToInt32(Session["clientId"]);
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
                DateCreationApp=dateNow,
                NbreJustif=0,
                RefInterne ="XXX"
            };
            return View(model);
        }

        [ActionName("Edit-douane")]
        public async Task<ActionResult> ValidationDouane(int id)
        {
            var doss=db.GetDossiers.Find(id);
            if(doss==null)
                return RedirectToAction("IndexBanque","Index");

            doss.ValiderDouane = true;
            await db.SaveChangesAsync();
            try
            {
                var structure = db.Structures.Find(Session["IdStructure"]);
                var idbanque = structure.BanqueId(db);
                await MailFunctions.ChangeEtapeDossier((int)idbanque, (string)Session["userId"], 5, doss, db, structure, msg: "", itemsRejet: "");
            }
            catch (Exception)
            { }
            //return RedirectToAction("Details", new {id=id});
            return Redirect("~/dossiers_banque/index ? "+Session["composant"]);
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
                dossier.DateDepotBank = dateNow;
                dossier.DateModif = dateNow;
                dossier.DateCreationApp = dateNow;
                try
                {
                    dossier.Client = (db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name) as CompteClient).Client;
                }
                catch (Exception e)
                {}                

                dossier=db.GetDossiers.Add(dossier);


                #region Numerisation

                //instructon
                if (ImageInstruction != null)
                {
                    if (!string.IsNullOrEmpty(ImageInstruction.FileName))
                    {
                        var imageModel = new genetrix.Models.ImageInstruction();
                        imageModel.Titre = "Instruction_" + dossier.Intitulé;

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
                        var imageModel = new genetrix.Models.ImageDocumentAttache();
                        imageModel.Titre = "Déclaration d'importation_" + dossier.Intitulé;

                        string chemin = Path.GetFileNameWithoutExtension(ImageInstruction.FileName);
                        string extension = Path.GetExtension(ImageInstruction.FileName);
                        chemin = imageModel.Titre + extension;
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
                        var imageModel = new genetrix.Models.ImageDocumentAttache();
                        imageModel.Titre = "Domiciliation d'importation_" + dossier.Intitulé;

                        string chemin = Path.GetFileNameWithoutExtension(ImageDomicilImport.FileName);
                        string extension = Path.GetExtension(ImageDomicilImport.FileName);
                        chemin = imageModel.Titre + extension;
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
                        var imageModel = new genetrix.Models.ImageDocumentAttache();
                        imageModel.Titre = "Lettre d'engagement_" + dossier.Intitulé;

                        string chemin = Path.GetFileNameWithoutExtension(ImageLettreEngage.FileName);
                        string extension = Path.GetExtension(ImageLettreEngage.FileName);
                        chemin = imageModel.Titre + extension;
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
                        var imageModel = new genetrix.Models.ImageDocumentAttache();
                        imageModel.Titre = "Quittance de paiement_" + dossier.Intitulé;

                        string chemin = Path.GetFileNameWithoutExtension(ImageQuittancePay.FileName);
                        string extension = Path.GetExtension(ImageQuittancePay.FileName);
                        chemin = imageModel.Titre + extension;
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
                        var imageModel = new genetrix.Models.ImageDocumentAttache();
                        imageModel.Titre = "Documents de transport_" + dossier.Intitulé;

                        string chemin = Path.GetFileNameWithoutExtension(ImageDocumentTransport.FileName);
                        string extension = Path.GetExtension(ImageDocumentTransport.FileName);
                        chemin = imageModel.Titre + extension;
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

            int _clientId = Convert.ToInt32(Session["clientId"]);
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
        
        private string GetClientDossierFolder(int clientId,string intituleDossier)
        {
            try
            {
                string projectPath = "~/EspaceClient/" + clientId + "/Ressources/Transferts";
                string folderName = Path.Combine(Server.MapPath(projectPath), intituleDossier);

                return folderName;
            }
            catch (Exception ee)
            { }
            return "";
        }
        
        private string GetFournisseurDocuments(int clientId,int fournisseurId)
        {
            try
            {
                string projectPath = "~/EspaceClient/" + clientId + "/Ressources/Fournisseurs";
                string folderName = Path.Combine(Server.MapPath(projectPath), ""+ fournisseurId);

                return folderName;
            }
            catch (Exception ee)
            { }
            return "";
        }
        
        private string GetClientDocuments(int clientId)
        {
            try
            {
                string projectPath = "~/EspaceClient/" + clientId + "/Ressources";
                string folderName = Path.Combine(Server.MapPath(projectPath), "DocumentsEntreprise");

                return folderName;
            }
            catch (Exception ee)
            { }
            return "";
        }

        [HttpGet]
        public ActionResult GenereCourier(int id)
        {
            Dossier dossier = db.GetDossiers.Find(id);
            return PartialView("EditCourrier", dossier);
        } 
        
        [HttpPost]
        public ActionResult ModifEtatDossier(FormCollection form)
        {
            var idDossier = int.Parse(form["IdDossier"]);
            var etat = int.Parse(form["EtapesDosier"]);
            
            return RedirectToAction("Details", new { id= idDossier });
        } 
        
        [HttpGet]
        public async Task<ActionResult> ModifEtatDossierJS(int? etat=null,int? idDossier=null,bool Estgroupe=false,int? idOpSwft=null, int? idRef=null,string message="",string date="",string itemsRejet= "")
        {
            var result = "Opération effectuée avec succes";

            if (etat==null)
            {
                return Json("Impossible d'executer cette opération: {identiant null pour l'objet representé} !", JsonRequestBehavior.AllowGet);
            }
            else if (etat<0 && string.IsNullOrEmpty(message))
            {
                return Json("Le champ motif est obligatoire !", JsonRequestBehavior.AllowGet);
            }
            if ((string)Session["userType"] == "CompteBanqueCommerciale")
            {
                try
                {
                    var structure = db.Structures.Find(Session["IdStructure"]);
                    var idbanque = structure.BanqueId(db);
                    genetrix.Models.Banque banque = new Models.Banque();
                    try
                    {
                        banque = db.GetBanques.Find(idbanque);
                    }
                    catch (Exception)
                    { }
                    if (etat > 0)
                    {
                        if (Estgroupe)
                        {
                            if (idRef > 0 && false)
                            {
                                /*
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
                                    if (re.Dossiers.FirstOrDefault().Client.Adresses.Count==0)
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
                                */
                            }
                        }
                        else if (idDossier > 0)
                        {
                            var doss = db.GetDossiers
                             .Include(d => d.Site)
                            .Include(d => d.Fournisseur)
                            .Include(d => d.Client)
                            .Include(d => d.DeviseMonetaire)
                            .FirstOrDefault(d => d.Dossier_Id == idDossier);

                            var nomclient = doss.Client.Nom;
                            if (doss.Client.Adresses.Count == 0)
                            {
                                result = "Erreur: L'adresse mail du client " + nomclient + " est nulle. Le processus s'est arreté, veillez renseigner au-moins une adresse mail du client!";
                                var d = 0;
                                var dd = 1 / d;
                            }

                            var etatmp = etat;

                            await MailFunctions.ChangeEtapeDossier((int)idbanque, (string)Session["userId"], (int)etat, doss, db, structure, msg: message,itemsRejet:itemsRejet);

                            try
                            {
                                if ((etatmp == 19 || etatmp == 20) && idOpSwft > 0)
                                {
                                    var op = db.GetOperateurSwifts.Find(idOpSwft);
                                    var details = MailFunctions.DetailsDossier(doss);

                                    var subject = $"Dossier à traiter: {doss.GetClient}; {doss.MontantStringDevise}; {doss.GetFournisseur}";
                                    var To = op.Email;
                                    var body = $"Madame, Monsieur; <br /><br /> Nous vous transmettons ci-joint le dossier du client {doss.GetClient} pour traitement ce jour via Swift selon les caracteristiques suivantes:"
                                       + "<br /> <br /> <div class=\"card\">" + details + "</ div>";

                                    //redirection vers extraction 
                                    ExtraireDossier(doss.Dossier_Id, To, false, false, subject, body);
                                }
                            }
                            catch (Exception)
                            { }
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
                                    //msg = d.GetEtapDossier()[0];
                                    count++;
                                }
                                if (count > 0)
                                {
                                    fournisseur = re.Dossiers.FirstOrDefault().Fournisseur.Nom;
                                    nomclient = re.Dossiers.FirstOrDefault().Client.Nom;
                                }
                                //if (string.IsNullOrEmpty(re.Dossiers.FirstOrDefault().Client.Email))
                                if (re.Dossiers.FirstOrDefault().Client.Adresses.Count == 0)
                                {
                                    result = "Erreur: L'adresse mail du client " + nomclient + " est nulle. Le processus s'est arreté, veillez corriger cette erreur!";
                                    return Json(result + "_" + etat, JsonRequestBehavior.AllowGet);
                                }
                                try
                                {
                                    db.SaveChanges();
                                    MailFunctions.EnvoiMail(re, fournisseur, nomclient, (int)idbanque, "Dossier rejeté ", "La référence " + re.NumeroRef + "  associée à(aux) dossier(s) suivant(s) a été rejetée suite à " + message + " depuis le " + dateNow, db, itemsRejet: itemsRejet);
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
                            if (doss.Client.Adresses.Count == 0)
                            {
                                result = "Erreur: L'adresse mail du client " + nomclient + " est nulle. Le processus s'est arreté, veillez corriger cette erreur!";
                                return Json(result + "_" + etat, JsonRequestBehavior.AllowGet);
                            }
                            if (Math.Abs((int)(doss.EtapesDosier == null ? 0 : doss.EtapesDosier)) > 27)
                            {
                                await MailFunctions.ChangeEtapeDossier((int)idbanque, (string)Session["userId"], -Math.Abs((int)(doss.EtapesDosier == null ? 0 : doss.EtapesDosier)), doss, db, structure, rejet: true, msg: message, itemsRejet: itemsRejet);
                            }
                            else if (doss.EtapesDosier > 5)
                            {
                                if (doss.EtapesDosier > 15)
                                    await MailFunctions.ChangeEtapeDossier((int)idbanque, (string)Session["userId"], -3, doss, db, structure, rejet: true, msg: message, itemsRejet: itemsRejet);
                                else
                                    await MailFunctions.ChangeEtapeDossier((int)idbanque, (string)Session["userId"], -2, doss, db, structure, rejet: true, msg: message, itemsRejet: itemsRejet);
                            }
                            else
                            {
                                if (doss.EtapesDosier == 4)
                                    await MailFunctions.ChangeEtapeDossier((int)idbanque, (string)Session["userId"], -2, doss, db, structure, rejet: true, msg: message + " - backoffice", itemsRejet: itemsRejet);
                                else
                                    await MailFunctions.ChangeEtapeDossier((int)idbanque, (string)Session["userId"], -1, doss, db, structure, rejet: true, msg: message, itemsRejet: itemsRejet);
                            }
                        }

                        //return RedirectToAction("Index");
                    }

                    db.SaveChanges();
                }
                catch (Exception)
                {
                    //result = "erreur";
                    idDossier = -1;
                } 
            }
            else if ((string)Session["userType"] == "CompteClient")
            {
                var doss = db.GetDossiers
                             .Include(d => d.Site)
                            .Include(d => d.Fournisseur)
                            .Include(d => d.Client)
                            .Include(d => d.DeviseMonetaire)
                            .FirstOrDefault(d => d.Dossier_Id == idDossier);
                await MailFunctions.ChangeEtapeDossier((int)doss.IdSite, (string)Session["userId"], (int)etat, doss, db, doss.Site, msg: message, itemsRejet: itemsRejet);
            }

            return Json(new { result, etat, idDossier }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> ModifEtatDossierNJS(int? etat = null, int? idDossier = null, bool Estgroupe = false, int? idOpSwft = null, int? idRef = null, string message = "", string date = "", string itemsRejet = "")
        {
            var result = "Opération effectuée avec succes";

            if (etat == null)
            {
                return RedirectToAction("Details", new { id=idDossier,msg= "Impossible d'executer cette opération: {identiant null pour l'objet representé} !" });
            }
            else if (etat < 0 && string.IsNullOrEmpty(message))
            {
                return RedirectToAction("Details", new { id = idDossier, msg = "Le champ motif est obligatoire !" });
            }
            if ((string)Session["userType"] == "CompteBanqueCommerciale")
            {
                try
                {
                    var structure = db.Structures.Find(Session["IdStructure"]);
                    var idbanque = structure.BanqueId(db);
                    genetrix.Models.Banque banque = new Models.Banque();
                    try
                    {
                        banque = db.GetBanques.Find(idbanque);
                    }
                    catch (Exception)
                    { }
                    if (etat > 0)
                    {
                        if (Estgroupe)
                        {
                            if (idRef > 0 && false)
                            {
                                /*
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
                                    if (re.Dossiers.FirstOrDefault().Client.Adresses.Count==0)
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
                                */
                            }
                        }
                        else if (idDossier > 0)
                        {
                            var doss = db.GetDossiers
                             .Include(d => d.Site)
                            .Include(d => d.Fournisseur)
                            .Include(d => d.Client)
                            .Include(d => d.DeviseMonetaire)
                            .FirstOrDefault(d => d.Dossier_Id == idDossier);
                            try
                            {
                                if ((dateNow - doss.DateSignInst.Value).TotalDays>=15 && doss.FormiteBEAC)
                                {
                                    return RedirectToAction("Details", new { id = idDossier, msg = "Veillez noter que l'instruction doit dater de moins de 15 jours. Ainsi nous vous invitons à insérer une version actualisée de celle-ci." });
                                }
                            }
                            catch (Exception)
                            {}
                            var nomclient = doss.Client.Nom;
                            if (doss.Client.Adresses.Count == 0)
                            {
                                result = "Erreur: L'adresse mail du client " + nomclient + " est nulle. Le processus s'est arreté, veillez renseigner au-moins une adresse mail du client!";
                                var d = 0;
                                var dd = 1 / d;
                            }

                            var etatmp = etat;

                            await MailFunctions.ChangeEtapeDossier((int)idbanque, (string)Session["userId"], (int)etat, doss, db, structure, msg: message, itemsRejet: itemsRejet);

                            try
                            {
                                if ((etatmp == 19 || etatmp == 20) && idOpSwft > 0)
                                {
                                    var op = db.GetOperateurSwifts.Find(idOpSwft);
                                    var details = MailFunctions.DetailsDossier(doss);

                                    var subject = $"Dossier à traiter: {doss.GetClient}; {doss.MontantStringDevise}; {doss.GetFournisseur}";
                                    var To = op.Email;
                                    var body = $"Madame, Monsieur; <br /><br /> Nous vous transmettons ci-joint le dossier du client {doss.GetClient} pour traitement ce jour via Swift selon les caracteristiques suivantes:"
                                       + "<br /> <br /> <div class=\"card\">" + details + "</ div>";

                                    //redirection vers extraction 
                                   await ExtraireDossier(doss.Dossier_Id, To, false, false, subject, body);
                                }
                            }
                            catch (Exception)
                            { }
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
                                    try
                                    {
                                        if ((dateNow - d.DateSignInst.Value).TotalDays >= 15 && d.FormiteBEAC)
                                        {
                                            return RedirectToAction("Details", new { id = idDossier, msg = "Veillez noter que l'instruction doit dater de moins de 15 jours. Ainsi nous vous invitons à insérer une version actualisée de celle-ci." });
                                        }
                                    }
                                    catch (Exception)
                                    { }
                                    //msg = d.GetEtapDossier()[0];
                                    count++;
                                }
                                if (count > 0)
                                {
                                    fournisseur = re.Dossiers.FirstOrDefault().Fournisseur.Nom;
                                    nomclient = re.Dossiers.FirstOrDefault().Client.Nom;
                                }
                                //if (string.IsNullOrEmpty(re.Dossiers.FirstOrDefault().Client.Email))
                                if (re.Dossiers.FirstOrDefault().Client.Adresses.Count == 0)
                                {
                                    result = "Erreur: L'adresse mail du client " + nomclient + " est nulle. Le processus s'est arreté, veillez corriger cette erreur!";
                                    return RedirectToAction("Details", new { id = idDossier, msg = result + "_" + etat });
                                }
                                try
                                {
                                    db.SaveChanges();
                                    MailFunctions.EnvoiMail(re, fournisseur, nomclient, (int)idbanque, "Dossier rejeté ", "La référence " + re.NumeroRef + "  associée à(aux) dossier(s) suivant(s) a été rejetée suite à " + message + " depuis le " + dateNow, db, itemsRejet: itemsRejet);
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
                            if (doss.Client.Adresses.Count == 0)
                            {
                                result = "Erreur: L'adresse mail du client " + nomclient + " est nulle. Le processus s'est arreté, veillez corriger cette erreur!";
                                return RedirectToAction("Details", new { id = idDossier, msg = result + "_" + etat });
                            }
                            if (Math.Abs((int)(doss.EtapesDosier == null ? 0 : doss.EtapesDosier)) > 27)
                            {
                                await MailFunctions.ChangeEtapeDossier((int)idbanque, (string)Session["userId"], -Math.Abs((int)(doss.EtapesDosier == null ? 0 : doss.EtapesDosier)), doss, db, structure, rejet: true, msg: message, itemsRejet: itemsRejet);
                            }
                            else if (doss.EtapesDosier > 5)
                            {
                                if (doss.EtapesDosier > 15)
                                    await MailFunctions.ChangeEtapeDossier((int)idbanque, (string)Session["userId"], -3, doss, db, structure, rejet: true, msg: message, itemsRejet: itemsRejet);
                                else
                                    await MailFunctions.ChangeEtapeDossier((int)idbanque, (string)Session["userId"], -2, doss, db, structure, rejet: true, msg: message, itemsRejet: itemsRejet);
                            }
                            else
                            {
                                if (doss.EtapesDosier == 4)
                                    await MailFunctions.ChangeEtapeDossier((int)idbanque, (string)Session["userId"], -2, doss, db, structure, rejet: true, msg: message + " - backoffice", itemsRejet: itemsRejet);
                                else
                                    await MailFunctions.ChangeEtapeDossier((int)idbanque, (string)Session["userId"], -1, doss, db, structure, rejet: true, msg: message, itemsRejet: itemsRejet);
                            }
                        }

                        //return RedirectToAction("Index");
                    }

                    db.SaveChanges();
                }
                catch (Exception)
                {
                    //result = "erreur";
                    idDossier = -1;
                }
            }
            else if ((string)Session["userType"] == "CompteClient")
            {
                var doss = db.GetDossiers
                             .Include(d => d.Site)
                            .Include(d => d.Fournisseur)
                            .Include(d => d.Client)
                            .Include(d => d.DeviseMonetaire)
                            .FirstOrDefault(d => d.Dossier_Id == idDossier);
                await MailFunctions.ChangeEtapeDossier((int)doss.IdSite, (string)Session["userId"], (int)etat, doss, db, doss.Site, msg: message, itemsRejet: itemsRejet);
            }

            return RedirectToAction("Details", new { id = idDossier, result=result, etat=etat });
        }

        [HttpPost]
        public async Task<ActionResult> ModifEtatDossierNJS(DossierChange model)
        {
            var result = "Opération effectuée avec succes";

            if (model.etat == null)
            {
                return RedirectToAction("Details", new { id= model.idDossier,msg= "Impossible d'executer cette opération: {identiant null pour l'objet representé} !" });
            }
            else if (model.etat < 0 && string.IsNullOrEmpty(model.message))
            {
                return RedirectToAction("Details", new { id = model.idDossier, msg = "Le champ motif est obligatoire !" });
            }
            if ((string)Session["userType"] == "CompteBanqueCommerciale")
            {
                try
                {
                    var structure = db.Structures.Find(Session["IdStructure"]);
                    var idbanque = structure.BanqueId(db);
                    genetrix.Models.Banque banque = new Models.Banque();
                    try
                    {
                        banque = db.GetBanques.Find(idbanque);
                    }
                    catch (Exception)
                    { }
                    if (model.etat > 0)
                    {
                        if (model.Estgroupe)
                        {
                            if (model.idRef > 0 && false)
                            {
                                /*
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
                                    if (re.Dossiers.FirstOrDefault().Client.Adresses.Count==0)
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
                                */
                            }
                        }
                        else if (model.idDossier > 0)
                        {
                            var doss = db.GetDossiers
                             .Include(d => d.Site)
                            .Include(d => d.Fournisseur)
                            .Include(d => d.Client)
                            .Include(d => d.DeviseMonetaire)
                            .FirstOrDefault(d => d.Dossier_Id == model.idDossier);

                            var nomclient = doss.Client.Nom;
                            if (doss.Client.Adresses.Count == 0)
                            {
                                result = "Erreur: L'adresse mail du client " + nomclient + " est nulle. Le processus s'est arreté, veillez renseigner au-moins une adresse mail du client!";
                                var d = 0;
                                var dd = 1 / d;
                            }

                            var etatmp = model.etat;

                            await MailFunctions.ChangeEtapeDossier((int)idbanque, (string)Session["userId"], (int)model.etat, doss, db, structure, msg: model.message, itemsRejet: model.itemsRejet);

                            try
                            {
                                if ((etatmp == 19 || etatmp == 20) && model.idOpSwft > 0)
                                {
                                    var op = db.GetOperateurSwifts.Find(model.idOpSwft);
                                    var details = MailFunctions.DetailsDossier(doss);

                                    var subject = $"Dossier à traiter: {doss.GetClient}; {doss.MontantStringDevise}; {doss.GetFournisseur}";
                                    var To = op.Email;
                                    var body = $"Madame, Monsieur; <br /><br /> Nous vous transmettons ci-joint le dossier du client {doss.GetClient} pour traitement ce jour via Swift selon les caracteristiques suivantes:"
                                       + "<br /> <br /> <div class=\"card\">" + details + "</ div>";

                                    //redirection vers extraction 
                                   await ExtraireDossier(doss.Dossier_Id, To, false, false, subject, body);
                                }
                            }
                            catch (Exception)
                            { }
                        }
                    }
                    else
                    {
                        if(model.Estgroupe)
                        {
                            if (model.idRef > 0)
                            {
                                string fournisseur = "", msg = "";
                                var nomclient = "";
                                int count = 0;

                                var re = db.GetReferenceBanques.Include(d => d.Dossiers).FirstOrDefault(r => r.Id == model.idRef);
                                foreach (var d in re.Dossiers)
                                {
                                    d.EtapesDosier = 2;
                                    //msg = d.GetEtapDossier()[0];
                                    count++;
                                }
                                if (count > 0)
                                {
                                    fournisseur = re.Dossiers.FirstOrDefault().Fournisseur.Nom;
                                    nomclient = re.Dossiers.FirstOrDefault().Client.Nom;
                                }
                                //if (string.IsNullOrEmpty(re.Dossiers.FirstOrDefault().Client.Email))
                                if (re.Dossiers.FirstOrDefault().Client.Adresses.Count == 0)
                                {
                                    result = "Erreur: L'adresse mail du client " + nomclient + " est nulle. Le processus s'est arreté, veillez corriger cette erreur!";
                                    return RedirectToAction("Details", new { id = model.idDossier, msg = result + "_" + model.etat });
                                }
                                try
                                {
                                    db.SaveChanges();
                                    MailFunctions.EnvoiMail(re, fournisseur, nomclient, (int)idbanque, "Dossier rejeté ", "La référence " + re.NumeroRef + "  associée à(aux) dossier(s) suivant(s) a été rejetée suite à " + model.message + " depuis le " + dateNow, db, itemsRejet: model.itemsRejet);
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
                            .FirstOrDefault(d => d.Dossier_Id == model.idDossier);
                            var nomclient = doss.Client.Nom;
                            if (doss.Client.Adresses.Count == 0)
                            {
                                result = "Erreur: L'adresse mail du client " + nomclient + " est nulle. Le processus s'est arreté, veillez corriger cette erreur!";
                                return RedirectToAction("Details", new { id = model.idDossier, msg = result + "_" + model.etat });
                            }
                            if (Math.Abs((int)(doss.EtapesDosier == null ? 0 : doss.EtapesDosier)) > 27)
                            {
                                await MailFunctions.ChangeEtapeDossier((int)idbanque, (string)Session["userId"], -Math.Abs((int)(doss.EtapesDosier == null ? 0 : doss.EtapesDosier)), doss, db, structure, rejet: true, msg: model.message, itemsRejet: model.itemsRejet);
                            }
                            else if (doss.EtapesDosier > 5)
                            {
                                if (doss.EtapesDosier > 15)
                                    await MailFunctions.ChangeEtapeDossier((int)idbanque, (string)Session["userId"], -3, doss, db, structure, rejet: true, msg: model.message, itemsRejet: model.itemsRejet);
                                else
                                    await MailFunctions.ChangeEtapeDossier((int)idbanque, (string)Session["userId"], -2, doss, db, structure, rejet: true, msg: model.message, itemsRejet: model.itemsRejet);
                            }
                            else
                            {
                                if (doss.EtapesDosier == 4)
                                    await MailFunctions.ChangeEtapeDossier((int)idbanque, (string)Session["userId"], -2, doss, db, structure, rejet: true, msg: model.message + " - backoffice", itemsRejet: model.itemsRejet);
                                else
                                    await MailFunctions.ChangeEtapeDossier((int)idbanque, (string)Session["userId"], -1, doss, db, structure, rejet: true, msg: model.message, itemsRejet: model.itemsRejet);
                            }
                        }

                        //return RedirectToAction("Index");
                    }

                    db.SaveChanges();
                }
                catch (Exception)
                {
                    //result = "erreur";
                    model.idDossier = -1;
                }
            }
            else if ((string)Session["userType"] == "CompteClient")
            {
                var doss = db.GetDossiers
                             .Include(d => d.Site)
                            .Include(d => d.Fournisseur)
                            .Include(d => d.Client)
                            .Include(d => d.DeviseMonetaire)
                            .FirstOrDefault(d => d.Dossier_Id == model.idDossier);
                await MailFunctions.ChangeEtapeDossier((int)doss.IdSite, (string)Session["userId"], (int)model.etat, doss, db, doss.Site, msg: model.message, itemsRejet: model.itemsRejet);
            }

            var url = "~/dossiers_banque/index?"+Session["composant"];
            if (model.etat == 231)
            {
                url = "~/dossiers_banque/Details?id=" + model.idDossier;
            }
            return Redirect(url);
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
                        To = emailGes,
                        Subject = "<h3 style=\"background-color:blue;color:white;padding:8px;text-align:center\"> Situation du dossier n°" + doss.Dossier_Id+"</h3>",
                        Body =doss.Traité? "<p>Le dossier n°" + doss.Dossier_Id + " a été traité le " + dateNow+"</p>": "<p>Le dossier n°" + doss.Dossier_Id + " n'a pas été traité le " + dateNow+"</p>",
                        CC= (from a in doss.Client.Adresses select a.Email).ToList()
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
        
        /// <summary>
        /// Changer le worflow du dossier
        /// </summary>
        /// <param name="doss"></param>
        /// <returns></returns>
        public JsonResult ChangeWf(int doss)
        {
            var dossier = db.GetDossiers.Find(doss);
            var estbeac = dossier.DFX6FP6BEAC;
           
        
            double montantDfx = 0;
            try
            {
                montantDfx = Convert.ToDouble(Session["MontantDFX"]);
                dossier.BanqueToString = Session["banqueName"].ToString();
            }
            catch (Exception)
            { }

            if (dossier.DFX6FP6BEAC!=2)
            {
                dossier.DFX6FP6BEAC = 2;
                //Envoyer un mail au client

            }
            else if(dossier.DFX6FP6BEAC==2)
            {
                if (dossier.DeviseToString == "EUR")
                {
                    if (dossier.MontantCV <= montantDfx)
                        dossier.DFX6FP6BEAC = 1;
                    else dossier.DFX6FP6BEAC = 3;
                }
                else
                {
                    dossier.DFX6FP6BEAC = 3;
                }
            }

            db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
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
        
        public async Task<JsonResult> GetRefCorrespondantes(int DossierId,int fournisseurId,int deviseId,int clientId)
        {
            var structure = db.Structures.Find(Session["IdStructure"]);
            var banqueId = structure.BanqueId(db);
            var banque = db.GetBanques.Find(banqueId);
            var refs = new List<ReferenceVModel>();
            try
            {
                var doss = db.GetDossiers.Find(DossierId);
                if (doss.EtapesDosier==11)
                {
                   await MailFunctions.ChangeEtapeDossier((int)banqueId, (string)Session["userId"], 12, doss, db, structure);
                }
            }
            catch (Exception)
            {}
            var dossiers = new List<Dossier>(); 
            db.GetDossiers.Include(d=>d.ReferenceExterne).Where(d=>d.EtapesDosier < 15 && d.EtapesDosier >= 13 && d.ClientId == clientId && d.FournisseurId == fournisseurId && d.DeviseMonetaireId == deviseId).ToList().ForEach(d=>
            {
                if (d.MontantCV > banque.MontantDFX)
                {
                    dossiers.Add(d);
                }
            });
            
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
            var structure = db.Structures.Find(Session["IdStructure"]);
            var banqueId = structure.BanqueId(db);
            structure = null;
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
                    To=emailGes,
                    Subject = "Transfert de dossier de " +siteOrg.Nom+" vert "+siteDes.Nom,
                    Body = "Le dossier du n°" + doss.Dossier_Id + " du client " + doss.Client.Nom + " a été transféré du site " + siteOrg.Nom + " vert le site " + siteDes.Nom,
                    CC= (from a in doss.Client.Adresses select a.Email).ToList()
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

        public JsonResult Valider(int idDossier,int? eta)
        {
            var doss = db.GetDossiers.Find(idDossier);
            doss.ValiderConformite = true;
            try
            {
                var banqueId = db.Structures.Find(doss.IdSite).BanqueId(db);
                var banque = db.GetBanques.Find(banqueId);
                if (doss.MontantCV <=banque.MontantDFX) //DFX
                {
                    doss.EtapePrecedenteDosier = doss.EtapesDosier;
                    doss.EtapesDosier = 9;
                }
                else
                {

                }
            }
            catch (Exception)
            {}
            db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult Telecharger(int idDossier)
        {
            string msg = "";
            var doss = db.GetDossiers.Include(d=>d.Client).Include(d=>d.Fournisseur).FirstOrDefault(d=>d.Dossier_Id== idDossier);
            try
            {
                //creation du dossier s'il n'existe pas
                // client
                var client = doss.Client.Nom;
                var fournisseur = doss.Fournisseur.Nom;
                string[] paths = new string[] { Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "Genetrix",client,fournisseur,doss.Dossier_Id.ToString()
                };

                var files = Directory.GetParent(GetClientDossierFolder(doss.Client.Id, doss.Intitulé));
                var dd=Directory.CreateDirectory(Path.Combine(paths));

                var spp = GetClientDossierFolder(doss.Client.Id, doss.Intitulé);
                var tpp = Path.Combine(paths);
                //Copy all the files & Replaces any files with the same name
                Copy(spp, tpp);
                doss.NbrTelechargement++;
                db.SaveChanges();
                msg = "Télécharger le dossier: le dossier a déjà été téléchargé "+doss.NbrTelechargement+" fois";
            }
            catch (Exception ee)
            {}
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public FilePathResult Download(int idDossier)
        {
            var doss = db.GetDossiers.Include(d => d.Client).Include(d => d.Fournisseur).FirstOrDefault(d => d.Dossier_Id == idDossier);

            var files = Directory.GetParent(GetClientDossierFolder(doss.Client.Id, doss.Intitulé));

            var uploads = (from u in files.GetFiles()
                           select u.FullName).FirstOrDefault();
            if (uploads != null)
            {
                string folder = Path.GetFullPath(uploads);
                return File(folder, "", "");
            }
            return null;
        }

        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            // Check if the target directory exists; if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into the new directory.
            foreach (var ff in source.GetFiles())
            {
                try
                {
                    FileInfo fi = ff as FileInfo;
                    fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
                }
                catch (Exception)
                {}
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        public ActionResult DownloadZipFile(int dossierId)
        {
            var doss = db.GetDossiers.Include(d => d.Client).Include(d => d.Fournisseur).FirstOrDefault(d => d.Dossier_Id == dossierId);

            var files = Directory.GetParent(GetClientDossierFolder(doss.Client.Id, doss.Intitulé));
            string nomClientzip = doss.GetClient + "_Dossier" +doss.Dossier_Id,
            nomFournzip=doss.GetFournisseur;
            //var uploads = (from u in files.GetFiles()
            //               select u.FullName).FirstOrDefault();
            var sourceDirectory = GetClientDossierFolder(doss.Client.Id, doss.Intitulé);
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);

            sourceDirectory = GetClientDocuments(doss.Client.Id);
            DirectoryInfo clientDocsSource = new DirectoryInfo(sourceDirectory);

            sourceDirectory = GetFournisseurDocuments(doss.Client.Id, (int)doss.FournisseurId);
            DirectoryInfo fournisseurDocsSource = new DirectoryInfo(sourceDirectory);

            nomClientzip = nomClientzip.Replace(" ", "_");
            nomFournzip = nomFournzip.Replace(" ", "_");
            //////int CurrentFileID = Convert.ToInt32(FileID);  
            using (var memoryStream = new MemoryStream())
            {
                using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    try
                    {
                        //Dossiers documents
                        foreach (var file in diSource.GetFiles())
                        {
                            try
                            {
                                ziparchive.CreateEntryFromFile(file.FullName, nomClientzip + "/"+ nomFournzip + "/"+file.Name);

                            }
                            catch (Exception e1)
                            { }
                        }
                    }
                    catch (Exception e)
                    {}
                    
                    try
                    {
                        //Client documents
                        foreach (var file in clientDocsSource.GetFiles())
                        {
                            try
                            {
                                ziparchive.CreateEntryFromFile(file.FullName, nomClientzip + "/"+ nomFournzip + "/Client_"+ nomClientzip + "/" + file.Name);
                            }
                            catch (Exception e1)
                            { }
                        }
                    }
                    catch (Exception e)
                    {}
                    
                    try
                    {
                        //Fournisseur documents
                        foreach (var file in fournisseurDocsSource.GetFiles())
                        {
                            try
                            {
                                ziparchive.CreateEntryFromFile(file.FullName, nomClientzip + "/"+ nomFournzip + "/Fournisseur_"+ nomFournzip + "/" + file.Name);

                            }
                            catch (Exception e1)
                            { }
                        }
                    }
                    catch (Exception e)
                    {}
                }
                

                
                return File(memoryStream.ToArray(), "application/zip", nomClientzip + ".zip");
            }
        }

        //public ActionResult ExtraireDossier(int dossierId,bool withFournisseur=false,bool withClient=false)
        public async Task<ActionResult> ExtraireDossier(int dossierId,string agtemail, bool includ_f=true,bool includ_cl=true, string subject = "", string body="")
        {
            var doss =await db.GetDossiers.Include(d => d.Client).Include(d => d.Fournisseur).FirstOrDefaultAsync(d => d.Dossier_Id == dossierId);

            var files = Directory.GetParent(GetClientDossierFolder(doss.Client.Id, doss.Intitulé));
            string nomClientzip = doss.GetClient + "_Dossier" +doss.Dossier_Id,
            nomFournzip=doss.GetFournisseur;
            //var uploads = (from u in files.GetFiles()
            //               select u.FullName).FirstOrDefault();
            var sourceDirectory = GetClientDossierFolder(doss.Client.Id, doss.Intitulé);
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);

            sourceDirectory = GetClientDocuments(doss.Client.Id);
            DirectoryInfo clientDocsSource = new DirectoryInfo(sourceDirectory);

            sourceDirectory = GetFournisseurDocuments(doss.Client.Id, (int)doss.FournisseurId);
            DirectoryInfo fournisseurDocsSource = new DirectoryInfo(sourceDirectory);

            nomClientzip = nomClientzip.Replace(" ", "_");
            nomFournzip = nomFournzip.Replace(" ", "_");
            var idAgent = (string)Session["userId"];
            var agentEmail = db.Users.Find(idAgent).Email;
            //////int CurrentFileID = Convert.ToInt32(FileID);  
            using (var memoryStream = new MemoryStream())
            {
                using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    try
                    {
                        //Dossiers documents
                        foreach (var file in diSource.GetFiles())
                        {
                            try
                            {
                                ziparchive.CreateEntryFromFile(file.FullName, nomClientzip + "/"+ nomFournzip + "/"+file.Name);

                            }
                            catch (Exception e1)
                            { }
                        }
                    }
                    catch (Exception e)
                    {}
                    
                    try
                    {
                        //Client documents
                        if(includ_cl)
                        foreach (var file in clientDocsSource.GetFiles())
                        {
                            try
                            {
                                ziparchive.CreateEntryFromFile(file.FullName, nomClientzip + "/"+ nomFournzip + "/Client_"+ nomClientzip + "/" + file.Name);

                            }
                            catch (Exception e1)
                            { }
                        }
                    }
                    catch (Exception e)
                    {}
                    
                    try
                    {
                        //Fournisseur documents
                        if(includ_f)
                        foreach (var file in fournisseurDocsSource.GetFiles())
                        {
                            try
                            {
                                ziparchive.CreateEntryFromFile(file.FullName, nomClientzip + "/"+ nomFournzip + "/Fournisseur_"+ nomFournzip + "/" + file.Name);

                            }
                            catch (Exception e1)
                            { }
                        }
                    }
                    catch (Exception e)
                    {}
                }
                // Send Email with zip as attachment.
                string emailsender = ConfigurationManager.AppSettings["emailsender"].ToString();
                string emailSenderPassword = ConfigurationManager.AppSettings["emailSenderPassword"].ToString();

                using (MailMessage mail = new MailMessage(emailsender, agentEmail))
                {
                    try
                    {
                        if (string.IsNullOrEmpty(subject))
                            mail.Subject = "Extraction du dossier " + doss.GetClient + $" ({doss.GetFournisseur}, {doss.MontantStringDevise})";
                        else
                            mail.Subject = subject;
                        if(string.IsNullOrEmpty(body))
                            mail.Body = "Vous avez extrait le dossier " + doss.GetClient + $" {doss.MontantStringDevise} du fournisseur {doss.GetFournisseur}";
                        else mail.Body = body;
                        if(!string.IsNullOrEmpty(agtemail))
                            try
                            {
                                foreach (var item in agtemail.Split(';'))
                                    try
                                    {
                                        if (item != agentEmail && item.Contains('@'))
                                            mail.CC.Add(item);
                                    }
                                    catch (Exception)
                                    { }
                            }
                            catch (Exception)
                            {}

                        mail.Attachments.Add(new Attachment(new MemoryStream(memoryStream.ToArray()), "Document.zip"));

                        mail.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = ConfigurationManager.AppSettings["smtpserver"].ToString();
                        smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSSL"]);
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(emailsender, emailSenderPassword);
                        smtp.Port = Convert.ToInt16(ConfigurationManager.AppSettings["portnumber"]);
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.Send(mail);
                    }
                    catch (Exception e)
                    { }

                }
                try
                {
                    doss.NbrTelechargementApurement++; 
                    await db.SaveChangesAsync();
                }
                catch (Exception)
                {}
                return Json("ok",JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Responsable du dossier
        /// </summary>
        /// <returns></returns>
        public ActionResult GetRespons(int iddoss)
        {
            var doss = db.GetDossiers.Find(iddoss);
            try
            {
                if (!string.IsNullOrEmpty(doss.IdResponsableAgence))
                {
                    var dd= db.GetCompteBanqueCommerciales.Find(doss.IdResponsableAgence);
                    doss.IdResponsableAgence = dd.NomComplet + "_" + dd.Tel1 + "_" + dd.Email;
                    dd = null;
                }
            }
            catch (Exception)
            {}
            try
            {
                if (!string.IsNullOrEmpty(doss.IdResponsableConformite))
                {
                    var dd = db.GetCompteBanqueCommerciales.Find(doss.IdResponsableConformite);
                    doss.IdResponsableConformite = dd.NomComplet + "_" + dd.Tel1 + "_" + dd.Email;
                    dd = null;
                }
            }
            catch (Exception)
            {}
            try
            {
                if (!string.IsNullOrEmpty(doss.IdResponsableTransfert))
                {
                    var dd = db.GetCompteBanqueCommerciales.Find(doss.IdResponsableTransfert);
                    doss.IdResponsableTransfert = dd.NomComplet + "_" + dd.Tel1 + "_" + dd.Email;
                    dd = null;
                }
            }
            catch (Exception)
            {}
            try
            {
                if (!string.IsNullOrEmpty(doss.IdResponsableBackOffice))
                {
                    var dd = db.GetCompteBanqueCommerciales.Find(doss.IdResponsableBackOffice);
                    doss.IdResponsableBackOffice = dd.NomComplet + "_" + dd.Tel1 + "_" + dd.Email;
                    dd = null;
                }
            }
            catch (Exception)
            {}

            return Json(new object[] { doss.IdResponsableAgence,doss.IdResponsableConformite,doss.IdResponsableTransfert,doss.IdResponsableBackOffice }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> AddDouane(FormCollection form, 
            HttpPostedFileBase ImageDeclarImport = null, HttpPostedFileBase ImageDomicilImport = null)
        {
            var idDossier = int.Parse(form["Dossier_Id"]);

            if (idDossier>0)
            {
                var dossier = db.GetDossiers.Find(idDossier);
                dossier.DateModif = dateNow;

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
                        docAttache.DateCreation = dateNow;

                        chemin = null;
                        extension = null;

                        db.GetDocumentAttaches.Add(docAttache);
                        db.GetImageDocumentAttaches.Add(imageModel);
                        try
                        {
                            db.SaveChanges();
                            ImageDeclarImport.SaveAs(imageModel.Url);
                        }
                        catch (Exception ee)
                        { }

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
                        docAttache.DateCreation = dateNow;
                        imageModel.DateCreation = dateNow;
                        imageModel.DerniereModif = dateNow;

                        chemin = null;
                        extension = null;
                        db.GetDocumentAttaches.Add(docAttache);
                        db.GetImageDocumentAttaches.Add(imageModel);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {}
                        ImageDomicilImport.SaveAs(imageModel.Url);
                    }
                }

                try
                {
                    dossier = db.GetDossiers.Include(d => d.DeclarImport).Include(d => d.DomicilImport).FirstOrDefault(d => d.Dossier_Id == dossier.Dossier_Id);
                    if (dossier.DomicilImport != null && dossier.DeclarImport != null)
                    {
                        genetrix.Models.Banque banque = null;
                        Structure structure = null;
                        double montantDfx = 0;
                        try
                        {
                            montantDfx = Convert.ToDouble(Session["MontantDFX"]);
                            dossier.BanqueToString = Session["banqueName"].ToString();
                        }
                        catch (Exception)
                        { }
                        try
                        {
                            var idStruct = Session["IdStructure"];
                            structure = db.Structures.Find(idStruct);
                        }
                        catch (Exception)
                        { }
                        await MailFunctions.ChangeEtapeDossier((int)banque.Id, (string)Session["userId"], 5, dossier, db, structure);
                        db.SaveChanges();
                        structure = null;
                        banque = null;
                    }
                }
                catch (Exception)
                {}
            }
            var param = Session["composant"];
            //return RedirectToAction("index", new { param});
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
                if (Session == null) return RedirectToAction("Login", "Account");
                int _clientId = 0;
                if (Session["Profile"].ToString()=="banque")
                    _clientId = dossier.ClientId;
                else
                    _clientId = Convert.ToInt32(Session["clientId"]);
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
                ViewBag.navigation_msg = "Edition du dossier";

                var user = db.GetCompteBanqueCommerciales.Find((string)Session["userId"]);
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
                    dossier.DateDepotBank = dateNow;
                    dossier.DateCreationApp = dateNow;
                    dossier.DateModif = dateNow;

                    db.Entry(dossier).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    #region Numerisation

                    //instructon
                    if (ImageInstruction != null)
                    {
                        if (!string.IsNullOrEmpty(ImageInstruction.FileName))
                        {
                            var imageModel = new genetrix.Models.ImageInstruction();
                            imageModel.Titre = "Instruction_" + dossier.Intitulé;

                            string chemin = Path.GetFileNameWithoutExtension(ImageInstruction.FileName);
                            string extension = Path.GetExtension(ImageInstruction.FileName);
                            chemin = imageModel.Titre + extension;
                            imageModel.Url = Path.Combine(CreateNewFolderDossier(dossier.ClientId.ToString(), dossier.Intitulé), chemin);
                            imageModel.NomCreateur = User.Identity.Name;
                            if(dossier.GetImageInstructions!=null && dossier.GetImageInstructions.Count >0)
                            {
                                //Supprime l'ancienne image
                                System.IO.File.Delete(dossier.GetImageInstructions.ToList()[0].Url);
                            }
                            else
                                dossier.GetImageInstructions = new List<genetrix.Models.ImageInstruction>();
                            
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
                            imageModel.Titre = "Déclaration d'importation_" + dossier.Intitulé;

                            string chemin = Path.GetFileNameWithoutExtension(ImageDeclarImport.FileName);
                            string extension = Path.GetExtension(ImageDeclarImport.FileName);
                            chemin = imageModel.Titre + extension;
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
                            var imageModel = new genetrix.Models.ImageDocumentAttache();
                            imageModel.Titre = "Domiciliation d'importation_" + dossier.Intitulé;

                            string chemin = Path.GetFileNameWithoutExtension(ImageDomicilImport.FileName);
                            string extension = Path.GetExtension(ImageDomicilImport.FileName);
                            chemin = imageModel.Titre + extension;
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
                            var imageModel = new genetrix.Models.ImageDocumentAttache();
                            imageModel.Titre = "Lettre d'engagement_" + dossier.Intitulé;

                            string chemin = Path.GetFileNameWithoutExtension(ImageLettreEngage.FileName);
                            string extension = Path.GetExtension(ImageLettreEngage.FileName);
                            chemin = imageModel.Titre + extension;
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
                            var imageModel = new genetrix.Models.ImageDocumentAttache();
                            imageModel.Titre = "Quittance de paiement_" + dossier.Intitulé;

                            string chemin = Path.GetFileNameWithoutExtension(ImageQuittancePay.FileName);
                            string extension = Path.GetExtension(ImageQuittancePay.FileName);
                            chemin = imageModel.Titre + extension;
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
                            var imageModel = new genetrix.Models.ImageDocumentAttache();
                            imageModel.Titre = "Documents de transport_" + dossier.Intitulé;

                            string chemin = Path.GetFileNameWithoutExtension(ImageDocumentTransport.FileName);
                            string extension = Path.GetExtension(ImageDocumentTransport.FileName);
                            chemin = imageModel.Titre + extension;
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

            int _clientId = Convert.ToInt32(Session["clientId"]);
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

            //Droit d'accès
            if (dossier.EtapesDosier != null)
                return RedirectToAction("Details", new { id = dossier.Dossier_Id, msg = "Impossible de supprimer le dossier" });

            ViewBag.navigation = "rien";
            ViewBag.navigation_msg = "suppresion du dossier";
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

                //Droit d'accès
                if (dossier.EtapesDosier != null)
                    return RedirectToAction("Details", new { id = dossier.Dossier_Id, msg = "Impossible de supprimer le dossier" });


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

        private ActionResult RetourAuth(bool sessionNull=false)
        {
            if(!sessionNull)
                return RedirectToAction("Index");
                //return RedirectToAction("IndexBanque", "Index");
            return RedirectToAction("Connexion", "Auth",new { returnUrl =""});
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (Session==null)
            {
                RetourAuth(true);
            }
            else
            {
                var url = (string)Session["urlaccueil"];
                //filterContext.Result = RedirectToAction("IndexBanque", "Index");
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
