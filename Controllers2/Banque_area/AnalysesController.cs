using genetrix.Models;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace genetrix.Controllers.Banque_area
{
    public class AnalysesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        DateTime dateNow;

        public AnalysesController()
        {
            dateNow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "W. Central Africa Standard Time");
        }
        // GET: Analyses
        public async Task<ActionResult> Anomalies()
        {
            return View(await db.Chats.Include(c=>c.Emetteur).Where(c=>c.Statut==StatutChat.Fermé).ToListAsync());
        } 
        
        public async Task<ActionResult> Transferts()
        {
            List<int> annees = new List<int>();
            var idSite = 0;
            try
            {
                int banqueId = Convert.ToInt32(Session["banqueId"]);
                var bq = db.GetBanques.Find(banqueId);
                var ann = DateTime.UtcNow.Year - 1;
                if (bq != null && bq.DateCreation != null) ann = bq.DateCreation.Value.Year;
                for (int i = ann; i <= DateTime.UtcNow.Year; i++)
                    annees.Add(i);
            }
            catch (Exception e)
            { }
            List<AgentVM> agentVMs = new List<AgentVM>();
            try
            {
                idSite = Convert.ToInt32(Session["IdStructure"]);
                try
                {
                    foreach (var item in db.GetCompteBanqueCommerciales.Where(d => d.IdStructure == idSite))
                        agentVMs.Add(new AgentVM() { Id = item.Id, NomComplet = item.NomComplet });
                }
                catch (Exception)
                { }
            }
            catch (Exception)
            {}
            List<ClientVM> clientVMs = new List<ClientVM>();
            try
            {
                foreach (var item in db.GetBanqueClients.Where(c => c.IdSite == idSite))
                    try
                    {
                        clientVMs.Add(new ClientVM() { Id = item.ClientId, Nom = item.Client.Nom });
                    }
                    catch (Exception)
                    { }
            }
            catch (Exception)
            {}
            ViewBag.DevisesMonetaire = db.GetDeviseMonetaires.ToList();
            ViewBag.clients = clientVMs.ToList();
            ViewBag.agents = agentVMs.ToList();
            ViewBag.annees = annees;
            agentVMs = null;
            clientVMs = null;
            annees = null;
            return View(await db.Chats.Include(c=>c.Emetteur).Where(c=>c.Statut==StatutChat.Fermé).ToListAsync());
        }


        public async Task<ActionResult> RecapTransfertBanque(string Structure=null,string Gestionnaire=null,string Client=null, bool pdf = false, int? print = 0, int[] etat = null, double? Montant1 = null
            , double? Montant2 = null, string Devise = "", string Fourniseur = "", int? Delai1 = null, int? Delai2 = null
            , int? JourDepot1 = null, int? MoisDepot1 = null
            , int? AnneeDepot1 = null, int? JourDepot2 = null, int? MoisDepot2 = null, int? AnneeDepot2 = null
            , int? JourTraitement1 = null, int? MoisTraitement1 = null
            , int? AnneeTraitement1 = null, int? JourTraitement2 = null, int? MoisTraitement2 = null, int? AnneeTraitement2 = null
            , int? EtatDuDossier = null, string Cat = "", bool DFX = false, bool FP = false, bool Ref = false,string TypeTransfert="")
        {
            if (!pdf && false)
            {
                var exportFilePath = this.Server.MapPath("~/instruction.docx");
            }
            List<int> annees = new List<int>();
            genetrix.Models.Banque banque = null;
            try
            {
                int banqueId = Convert.ToInt32(Session["banqueId"]);
                banque = db.GetBanques.Find(banqueId);
                var ann = DateTime.UtcNow.Year - 1;
                if (banque != null && banque.DateCreation != null) ann = banque.DateCreation.Value.Year;
                for (int i = ann; i <= DateTime.UtcNow.Year; i++)
                    annees.Add(i);
            }
            catch (Exception e)
            { }
            ViewBag.DevisesMonetaire = db.GetDeviseMonetaires.Select(d=>d.Nom);
            List<string> tmp = new List<string>();
            db.GetCompteBanqueCommerciales.ToList().ForEach(g=>
            {
                tmp.Add(g.NomComplet);
            });
            ViewBag.Gestionnaire = tmp.ToList();
            tmp = null;
            ViewBag.Structure = db.Structures.Select(d=>d.Nom);
            ViewBag.Client = db.GetClients.Select(d=>d.Nom);
            ViewBag.annees = annees;
            annees = null;
            ViewBag.datejour = dateNow;
            ViewBag.impression = print;
            var donnees = banque.ResumeTransfertBanque(await db.GetDossiers.ToListAsync(),Structure,Gestionnaire,Client,etat, Montant1, Montant2, Devise, Fourniseur, JourDepot1, JourDepot2, MoisDepot1, MoisDepot2, AnneeDepot1
                , AnneeDepot2, JourTraitement1, JourTraitement2, MoisTraitement1, MoisTraitement2, AnneeTraitement1, AnneeTraitement2, EtatDuDossier, Delai1, Delai2, TypeTransfert);
            ViewBag.serachString = donnees[1];
            ViewBag.montantDevise = donnees[2];
            ViewBag.totalXaf = donnees[3];
            ViewBag.totalEnDevise = donnees[4];
            return View(donnees[0]);
        }

        public async Task<ActionResult> RecapTransfert(int id, bool pdf = false, int? print = 0, int[] etat = null, double? Montant1 = null
            , double? Montant2 = null, string Devise = "", string Fourniseur = "", int? Delai1 = null, int? Delai2 = null
            , int? JourDepot1 = null, int? MoisDepot1 = null
            , int? AnneeDepot1 = null, int? JourDepot2 = null, int? MoisDepot2 = null, int? AnneeDepot2 = null
            , int? JourTraitement1 = null, int? MoisTraitement1 = null
            , int? AnneeTraitement1 = null, int? JourTraitement2 = null, int? MoisTraitement2 = null, int? AnneeTraitement2 = null
            , int? EtatDuDossier = null, string Cat = "", bool DFX = false, bool FP = false, bool Ref = false)
        {
            if (!pdf && false)
            {
                var exportFilePath = this.Server.MapPath("~/instruction.docx");
            }
            var _client = await db.GetClients.FindAsync(id);
            List<int> annees = new List<int>();
            try
            {
                int banqueId = Convert.ToInt32(Session["banqueId"]);
                var bq = db.GetBanques.Find(banqueId);
                var ann = DateTime.UtcNow.Year - 1;
                if (bq != null && bq.DateCreation != null) ann = bq.DateCreation.Value.Year;
                for (int i = ann; i <= DateTime.UtcNow.Year; i++)
                    annees.Add(i);
            }
            catch (Exception e)
            { }
            try
            {
                ViewBag.Fournisseurs = _client.Fournisseurs.Select(f => f.Nom);
            }
            catch (Exception)
            { }
            ViewBag.DevisesMonetaire = db.GetDeviseMonetaires.Select(d=>d.Nom);
            ViewBag.annees = annees;
            annees = null;
            ViewBag.client = _client;
            ViewBag.clientId = id;
            ViewBag.datejour = dateNow;
            ViewBag.impression = print;
            var donnees = _client.TransfertResume(etat, Montant1, Montant2, Devise, Fourniseur, JourDepot1, JourDepot2, MoisDepot1, MoisDepot2, AnneeDepot1
                , AnneeDepot2, JourTraitement1, JourTraitement2, MoisTraitement1, MoisTraitement2, AnneeTraitement1, AnneeTraitement2, EtatDuDossier, Delai1, Delai2);
            ViewBag.serachString = donnees[1];
            ViewBag.montantDevise = donnees[2];
            ViewBag.totalXaf = donnees[3];
            ViewBag.totalEnDevise = donnees[4];
            return View(donnees[0]);
        }


    }
}