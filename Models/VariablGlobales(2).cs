using e_apurement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace eApurement.Models
{
    public class VariablGlobales
    {
        private ApplicationDbContext db;
        public ApplicationUser _User { get; set; }
        public int TotalDossiers;
        public List<BanqueClient> BanqueClients = new List<BanqueClient>();
        public List<Dossier> Dossiers = new List<Dossier>();
        public List<Fournisseurs> Fournisseurs = new List<Fournisseurs>();
        public List<Client> Clients = new List<Client>();
        public List<GestionnaireTmp> Gestionnaires = new List<GestionnaireTmp>();
        public VariablGlobales(ApplicationDbContext _db, ApplicationUser _user)
        {
            db = _db;
            _User = _user;
        }

        public static Dictionary<string, bool> Access(CompteBanqueCommerciale user, string type)
        {
            Dictionary<string, bool> resul = new Dictionary<string, bool>();
            
            try
            {
                if(user.EstAdmin)
                    return new Dictionary<string, bool>()
                    {
                        { "lire",true }, { "ecrire",true }, { "créer",true }, { "supprimer",true }, { "lire_pour_tout",true  }
                    };

                var banque = user.Structure;
                var entitee = user.XRole.GetEntitee_Roles.FirstOrDefault(e1 => e1.Entitee.Type == type);

                return new Dictionary<string, bool>()
                {
                    { "lire",entitee.Lire }, { "ecrire",entitee.Ecrire }, { "créer",entitee.Créer }, { "supprimer",entitee.Supprimer  }
                    , { "lire_pour_tout",entitee.Lire_Pour_Tout  }
                };
            }
            catch (Exception)
            { }
            return new Dictionary<string, bool>()
            {
                { "lire",false }, { "ecrire",false }, { "créer",false }, { "supprimer",false }, { "lire_pour_tout",false  }
            };
        }

        List<Dossier> dossiers = null;
        List<ReferenceBanque> referenceBanques = null;
        public IDictionary<int?,InfoDocAcueil> infoDoc(string id = null)
        {
            Dictionary<int?,InfoDocAcueil> doc = new Dictionary<int?,InfoDocAcueil>();
            try
            {
                if(!string.IsNullOrEmpty(id))
                {
                    _User = db.Users.Find(id);
                }

                if (_User != null)
                {
                    var clientId = (_User as CompteClient).ClientId;
                    dossiers = db.GetDossiers.Where(d => d.Client.Id == clientId).ToList();
                    this.TotalDossiers = dossiers.Count;
                    this.BanqueClients.AddRange((_User as CompteClient).Client.Banques);
                    this.Dossiers.AddRange((_User as CompteClient).Client.Dossiers);
                    this.Fournisseurs.AddRange((_User as CompteClient).Client.Fournisseurs);
                    var banques = (_User as CompteClient).Client.Banques.ToList();
                    this.Gestionnaires = new List<GestionnaireTmp>();
                    foreach (var item in banques)
                    {
                        this.Gestionnaires.Add(new GestionnaireTmp(item.Gestionnaire.NomComplet,item.Gestionnaire.Tel1,item.Gestionnaire.Email,item.Site.Nom));
                    }
                }
                else
                    dossiers = db.GetDossiers.ToList();
            }
            catch (Exception)
            { }

            try
            {
                var count = dossiers.Count;
                foreach (var item in dossiers)
                {
                    try
                    {
                        if (!doc.Keys.Contains((int)item.EtapesDosier))
                        {
                            doc[(int)item.EtapesDosier] = new InfoDocAcueil()
                            {
                                Date = item.DateCreationApp,
                                Nbr = 1,
                                EtapeDossier=(int)item.EtapesDosier,
                               // EtatDossier = item.StatutDossier.StatutDossier.EtatDossier,
                                Percentage = 1 * 100 / count
                            };
                        }
                        else
                        {
                            doc[(int)item.EtapesDosier].SetDurée(item.DateCreationApp);
                            doc[(int)item.EtapesDosier].Nbr++;
                            doc[(int)item.EtapesDosier].Percentage = doc[(int)item.EtapesDosier].Nbr * 100 / count;
                        }
                    }
                    catch (Exception e)
                    {}
                }
            }
            catch (Exception)
            {}

            return doc;
        }

        internal static List<DirectionMetier> GetDirectionMetierByBanque(int banqueId, ApplicationDbContext db)
        {
            List<DirectionMetier> directionMetiers = new List<DirectionMetier>();
            var dd = db.DirectionMetiers.ToList();
            foreach (var u in dd)
            {
                if (u.BanqueId(db) == banqueId)
                {
                    directionMetiers.Add(u);
                }
            }
            return directionMetiers;
        }

        internal static List<Structure> GetStructureByBanque(int banqueId, ApplicationDbContext db)
        {
            List<Structure> structures = new List<Structure>();
            foreach (var u in db.Structures.ToList())
            {
                if (u.BanqueId(db)== banqueId)
                {
                    structures.Add(u);
                }
            }
            return structures;
        }

        internal static List<Agence> GetAgenceByBanque(int banqueId, ApplicationDbContext db)
        {
            List<Agence> directionMetiers = new List<Agence>();
            var dd = db.Agences.Include("Responsable").Include("DirectionMetier").ToList();
            foreach (var u in dd)
            {
                if (u.BanqueId(db)== banqueId)
                {
                    directionMetiers.Add(u);
                }
            }
            dd = null;
            return directionMetiers;
        }

        internal static List<CompteBanqueCommerciale> GetUsersByBanque(int banqueId, ApplicationDbContext db)
        {
            List<CompteBanqueCommerciale> users = new List<CompteBanqueCommerciale>();
            foreach (var u in db.GetCompteBanqueCommerciales.ToList())
            {
                if (u.Structure.BanqueId(db) == banqueId)
                {
                    users.Add(u);
                }
            }
            return users;
        }

        public static Image BytesToImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            MemoryStream ms = new MemoryStream(imageData);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        public static byte[] ImageToBytes(string imageName)
        {
            try
            {
                Image image = Image.FromFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "images","IU", imageName));
                ImageConverter imageConverter = new ImageConverter();
                return (Byte[])imageConverter.ConvertTo(image, typeof(Byte[]));
            }
            catch (Exception)
            { }

            return null;
        }

        public static byte[] ImageToBytes(Image image)
        {
            try
            {
                ImageConverter imageConverter = new ImageConverter();
                return (Byte[])imageConverter.ConvertTo(image, typeof(Byte[]));
            }
            catch (Exception)
            { }

            return null;
        }

        public IDictionary<EtatDossier, InfoDocAcueil> infoDocT(string id = null)
        {
            Dictionary<EtatDossier, InfoDocAcueil> doc = new Dictionary<EtatDossier, InfoDocAcueil>();
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    _User = db.Users.Find(id);
                }

                if (_User != null)
                {
                    var clientId = (_User as CompteClient).ClientId;
                    dossiers = db.GetDossiers.Where(d => d.Client.Id == clientId).ToList();
                    this.TotalDossiers = dossiers.Count;
                    this.BanqueClients.AddRange((_User as CompteClient).Client.Banques);
                    this.Dossiers.AddRange((_User as CompteClient).Client.Dossiers);
                    this.Fournisseurs.AddRange((_User as CompteClient).Client.Fournisseurs);
                    var banques = (_User as CompteClient).Client.Banques.ToList();
                    this.Gestionnaires = new List<GestionnaireTmp>();
                    foreach (var item in banques)
                    {
                        this.Gestionnaires.Add(new GestionnaireTmp(item.Gestionnaire.NomComplet, item.Gestionnaire.Tel1, item.Gestionnaire.Email, item.Site.Nom));
                    }
                }
                else
                    dossiers = db.GetDossiers.ToList();
            }
            catch (Exception)
            { }

            try
            {
                var count = dossiers.Count;
                foreach (var item in dossiers)
                {
                    try
                    {
                        switch (item.StatutDossier.StatutDossier.EtatDossier)
                        {
                            case EtatDossier.Encours:
                                if (!doc.Keys.Contains(item.StatutDossier.StatutDossier.EtatDossier))
                                {
                                    doc[EtatDossier.Encours] = new InfoDocAcueil()
                                    {
                                        Date = item.DateCreationApp,
                                        Nbr = 1,
                                        EtatDossier = item.StatutDossier.StatutDossier.EtatDossier,
                                        Percentage = 1 * 100 / count
                                    };
                                }
                                else
                                {
                                    doc[EtatDossier.Encours].SetDurée(item.DateCreationApp);
                                    doc[EtatDossier.Encours].Nbr++;
                                    doc[EtatDossier.Encours].Percentage = doc[EtatDossier.Encours].Nbr * 100 / count;
                                }
                                break;
                            case EtatDossier.Soumis:
                                if (!doc.Keys.Contains(item.StatutDossier.StatutDossier.EtatDossier))
                                {
                                    doc[EtatDossier.Soumis] = new InfoDocAcueil()
                                    {
                                        Date = item.DateCreationApp,
                                        Nbr = 1,
                                        EtatDossier = item.StatutDossier.StatutDossier.EtatDossier,
                                        Percentage = 1 * 100 / count
                                    };
                                }
                                else
                                {
                                    doc[EtatDossier.Encours].SetDurée(item.DateCreationApp);
                                    doc[EtatDossier.Encours].Nbr++;
                                    doc[EtatDossier.Encours].Percentage = doc[EtatDossier.Encours].Nbr * 100 / count;
                                }
                                break;
                            case EtatDossier.Apuré:
                                if (!doc.Keys.Contains(item.StatutDossier.StatutDossier.EtatDossier))
                                {
                                    doc[item.StatutDossier.StatutDossier.EtatDossier] = new InfoDocAcueil()
                                    {
                                        Date = item.DateCreationApp,
                                        Nbr = 1,
                                        EtatDossier = item.StatutDossier.StatutDossier.EtatDossier,
                                        Percentage = 1 * 100 / count
                                    };
                                }
                                else
                                {
                                    doc[item.StatutDossier.StatutDossier.EtatDossier].SetDurée(item.DateCreationApp);
                                    doc[item.StatutDossier.StatutDossier.EtatDossier].Nbr++;
                                    doc[item.StatutDossier.StatutDossier.EtatDossier].Percentage = doc[EtatDossier.Encours].Nbr * 100 / count;
                                }
                                break;
                            case EtatDossier.AApurer:
                                if (!doc.Keys.Contains(item.StatutDossier.StatutDossier.EtatDossier))
                                {
                                    doc[item.StatutDossier.StatutDossier.EtatDossier] = new InfoDocAcueil()
                                    {
                                        Date = item.DateCreationApp,
                                        Nbr = 1,
                                        EtatDossier = item.StatutDossier.StatutDossier.EtatDossier,
                                        Percentage = 1 * 100 / count
                                    };
                                }
                                else
                                {
                                    doc[item.StatutDossier.StatutDossier.EtatDossier].SetDurée(item.DateCreationApp);
                                    doc[item.StatutDossier.StatutDossier.EtatDossier].Nbr++;
                                    doc[item.StatutDossier.StatutDossier.EtatDossier].Percentage = doc[EtatDossier.Encours].Nbr * 100 / count;
                                }
                                break;
                            case EtatDossier.Echus:
                                if (!doc.Keys.Contains(item.StatutDossier.StatutDossier.EtatDossier))
                                {
                                    doc[item.StatutDossier.StatutDossier.EtatDossier] = new InfoDocAcueil()
                                    {
                                        Date = item.DateCreationApp,
                                        Nbr = 1,
                                        EtatDossier = item.StatutDossier.StatutDossier.EtatDossier,
                                        Percentage = 1 * 100 / count
                                    };
                                }
                                else
                                {
                                    doc[item.StatutDossier.StatutDossier.EtatDossier].SetDurée(item.DateCreationApp);
                                    doc[item.StatutDossier.StatutDossier.EtatDossier].Nbr++;
                                    doc[item.StatutDossier.StatutDossier.EtatDossier].Percentage = doc[EtatDossier.Encours].Nbr * 100 / count;
                                }
                                break;
                            case EtatDossier.Archivé:
                                if (!doc.Keys.Contains(item.StatutDossier.StatutDossier.EtatDossier))
                                {
                                    doc[item.StatutDossier.StatutDossier.EtatDossier] = new InfoDocAcueil()
                                    {
                                        Date = item.DateCreationApp,
                                        Nbr = 1,
                                        EtatDossier = item.StatutDossier.StatutDossier.EtatDossier,
                                        Percentage = 1 * 100 / count
                                    };
                                }
                                else
                                {
                                    doc[item.StatutDossier.StatutDossier.EtatDossier].SetDurée(item.DateCreationApp);
                                    doc[item.StatutDossier.StatutDossier.EtatDossier].Nbr++;
                                    doc[item.StatutDossier.StatutDossier.EtatDossier].Percentage = doc[EtatDossier.Encours].Nbr * 100 / count;
                                }
                                break;
                            case EtatDossier.Brouillon:
                                if (!doc.Keys.Contains(item.StatutDossier.StatutDossier.EtatDossier))
                                {
                                    doc[item.StatutDossier.StatutDossier.EtatDossier] = new InfoDocAcueil()
                                    {
                                        Date = item.DateCreationApp,
                                        Nbr = 1,
                                        EtatDossier = item.StatutDossier.StatutDossier.EtatDossier,
                                        Percentage = 1 * 100 / count
                                    };
                                }
                                else
                                {
                                    doc[item.StatutDossier.StatutDossier.EtatDossier].SetDurée(item.DateCreationApp);
                                    doc[item.StatutDossier.StatutDossier.EtatDossier].Nbr++;
                                    doc[item.StatutDossier.StatutDossier.EtatDossier].Percentage = doc[EtatDossier.Encours].Nbr * 100 / count;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    catch (Exception e)
                    { }
                }
            }
            catch (Exception)
            { }

            return doc;
        }

        public IEnumerable<Dossier> GetDossiers()
        {
            if (dossiers != null)
            {
                try
                {
                    var clientId = (_User as CompteClient).ClientId;
                    dossiers= db.GetDossiers.Where(d => d.Client.Id == clientId).ToList();
                }
                catch (Exception e)
                {}
            }

            return dossiers;
        }

        public IEnumerable<ClientDossierVM> GetClientDossiers_Banque()
        {
            List<ClientDossierVM> listDossiers = null;
            if (_User is CompteBanqueCommerciale)
            {
                listDossiers = new List<ClientDossierVM>();
                int? idBanque = (_User as CompteBanqueCommerciale).IdStructure;

                //var bk = db.Sites.Find(idBanque);
                var bk = db.Structures.Find(idBanque);
                if (bk != null)
                {
                    try
                    {
                        foreach (var c in bk.Clients)
                        {
                            listDossiers.AddRange(c.Client.GetClientDossierVM(idBanque));
                        }
                    }
                    catch (Exception e)
                    { }
                } 
            }

            return listDossiers;
        } 
        
        public IEnumerable<Structure> GetAgences()
        {
            List<Agence> agences = null;
            if (_User is CompteBanqueCommerciale)
            {
                agences = new List<Agence>();
                int? idBanque = (_User as CompteBanqueCommerciale).IdStructure;

                var bk = db.GetBanques.Find(idBanque);
                if (bk != null)
                {
                    try
                    {
                        foreach(var s in bk.DirectionMetiers)
                            if(s is Agence)
                            agences.AddRange( s.Agences);
                    }
                    catch (Exception e)
                    { }
                } 
            }

            return agences;
        }
        
        public IEnumerable<CompteBanqueCommerciale> GetAllAgents()
        {
            List<CompteBanqueCommerciale> agents = null;
            if (_User is CompteBanqueCommerciale)
            {
                agents = new List<CompteBanqueCommerciale>();
                int? idBanque = (_User as CompteBanqueCommerciale).IdStructure;

                var bk = db.Structures.Find(idBanque);
                if (bk != null)
                {
                    try
                    {
                        agents.AddRange(bk.Agents);
                    }
                    catch (Exception e)
                    { }
                } 
            }

            return agents;
        }
        
        public IEnumerable<Dossier> GetDossiersBanque(int? etat=null,int? agenceId=null)
        {
            List<Dossier> dossiers = null;
            if (_User is CompteBanqueCommerciale)
            {
                dossiers = new List<Dossier>();
                int? idBanque = (_User as CompteBanqueCommerciale).IdStructure;

                var bk = db.Agences.Find(idBanque);
                if (bk != null)
                {
                    try
                    {
                        if(etat!=null && agenceId!=null)
                            dossiers.AddRange(bk.Dossiers.Where(d=>d.EtapesDosier==etat && d.IdSite==agenceId));
                        else if(agenceId !=null)
                            dossiers.AddRange(bk.Dossiers.Where(d=>d.IdSite==agenceId));
                        else if(etat !=null)
                            dossiers.AddRange(bk.Dossiers.Where(d=>d.EtapesDosier==etat));
                        else
                            dossiers.AddRange(bk.Dossiers);
                    }
                    catch (Exception e)
                    { }
                } 
            }

            return dossiers;
        }

        /// <summary>
        /// Dossiers par agence
        /// </summary>
        /// <param name="etat"></param>
        /// <param name="agenceId"></param>
        /// <returns></returns>
        public IEnumerable<DossierParAgence> GetDossiersParAgence(int? etat = null, int? agenceId = null)
        {
            List<DossierParAgence> dossiers = null;
            if (_User is CompteBanqueCommerciale)
            {
                dossiers = new List<DossierParAgence>();
                int? idBanque = (_User as CompteBanqueCommerciale).IdStructure;
                var nomAgence = "";
                if (agenceId != null)
                {
                    try
                    {
                        nomAgence = db.Structures.FirstOrDefault(a => a.Id == (int)agenceId).Nom;
                    }
                    catch (Exception)
                    {}
                }

                var bk = db.Structures.Find(idBanque);
                if (bk != null)
                {
                    try
                    {
                        if (etat != null && agenceId != null)
                            dossiers.Add(new DossierParAgence()
                            {
                                IdAgence = (int)agenceId,
                                NomAgence = nomAgence,
                                EtapeDosiier = etat,
                                NbrDossiers = bk.Dossiers.Where(d => d.IdSite == agenceId && d.EtapesDosier == etat).Count()
                            });
                        else if (agenceId != null)
                            foreach (var item in bk.Dossiers.Where(d => d.IdSite == agenceId).GroupBy(d => d.EtapesDosier))
                            {
                                dossiers.Add(new DossierParAgence()
                                {
                                    IdAgence = (int)agenceId,
                                    NomAgence = nomAgence,
                                    EtapeDosiier = item.Key,
                                    NbrDossiers = item.Count()
                                });
                            }
                        else if (etat != null)
                            dossiers.Add(new DossierParAgence()
                            {
                                EtapeDosiier = etat,
                                NbrDossiers = bk.Dossiers.Where(d => d.EtapesDosier == etat).Count()
                            });
                        else
                        {
                            Dictionary<int, string> agenceName = new Dictionary<int, string>();
                            foreach (var item in db.Structures.ToList())
                            {
                                agenceName.Add(item.Id, item.Nom);
                            }

                            foreach (var item in bk.Dossiers.Where(d => d.IdSite == agenceId).GroupBy(d => d.IdSite))
                            {
                                try
                                {
                                    dossiers.Add(new DossierParAgence()
                                    {
                                        IdAgence = item.Key,
                                        NomAgence = agenceName[item.Key],
                                        NbrDossiers = item.Count()
                                    });
                                }
                                catch (Exception)
                                {}
                            }
                            agenceName = null;
                        }
                    }
                    catch (Exception e)
                    { }
                }
            }

            return dossiers;
        }

        public List<string> GetDevise()
        {
            try
            {
                return (from d in db.GetDeviseMonetaires
                        select d.Nom).ToList();
            }
            catch (Exception)
            {}

            return new List<string>();
        }

        #region Partie banque


        public List<PercentageColor> GetBanquesClient()
        {
            try
            {
                var c_dossiers = GetDossiers().ToList();
                var clientId = (_User as CompteClient).Client.Id;
                var pcs = new List<PercentageColor>();
                var bb = db.GetBanqueClients;
                foreach (var d in db.GetBanqueClients.Where(b => b.Client.Id == clientId).ToList())
                {
                    var b = d.GetColorCssClass(c_dossiers);
                    b.Banque = d.Site.BanqueName(db);
                    pcs.Add(b);
                }
                return pcs;
            }
            catch (Exception ee)
            { }

            return new List<PercentageColor>();
        }

        public static List<Dossier> UserBanqueDossiers(ApplicationDbContext db,Structure site,XtraRole role,int banqueID,string agentId)
        {
            List<Dossier> dd = new List<Dossier>();
            bool verifié = false;
            //db.GetDossiers.Include(d => d.Site).Include(d => d.DeviseMonetaire).Include(d => d.ReferenceExterne).Where(d =>
            db.GetDossiers.Include("Site").Include("StatusDossiers").Include("DeviseMonetaire").Include("ReferenceExterne").Where(d =>
            /*Dossier permissions*/d.EtapesDosier != 13
            /*Role permissions*/ // && d.EtapesDosier >= role.NiveauDossier
            /*Structure permisions*/ && d.EtapesDosier >= site.NiveauDossier && d.EtapesDosier <= site.NiveauMaxDossier 
                                    ).ToList().ForEach(d =>
                                    {
                                        ///Provisoir
                                        ///
                                        try
                                        {
                                            verifié = false;
                                            var name = site.GetType().Name;
                                            if (site is DirectionMetier)
                                            {
                                                if (d.Site.IdDirectionMetier == site.Id)
                                                    verifié = true;
                                            }
                                            else if(!(site is Agence))
                                                verifié = true;

                                            if (d.IdSite == site.Id || d.IdSite == banqueID || verifié) // site
                                            {
                                                if (d.Site.BanqueId(db) == banqueID)/*Banque permissions*/
                                                {
                                                    if (!site.VoirDossiersAutres)//permission structure
                                                    {
                                                        if (d.IdSite == site.Id)
                                                        {
                                                            if (!role.VoirDossiersAutres && d.GestionnaireId == agentId)//permission de role
                                                            {
                                                                dd.Add(d);
                                                            }
                                                        }else if (verifié)
                                                        {
                                                            dd.Add(d);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dd.Add(d);
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {}

                                    });

            role = null;
            site = null;

            return dd;
        }
        
        public static List<ReferenceBanque> UserBanqueReferences(ApplicationDbContext db,Structure site,XtraRole role,int banqueID,string agentId)
        {
            List<Dossier> dd = new List<Dossier>();
            List<ReferenceBanque> rr = new List<ReferenceBanque>();

            if (!site.LireTouteReference)
            {
                db.GetDossiers.Include("Site").Include("DeviseMonetaire").Include("ReferenceExterne").Where(d =>
                   /*Dossier permissions*/d.EtapesDosier != 13
                   /*Role permissions*/ // && d.EtapesDosier >= role.NiveauDossier
                   /*Structure permisions*/ && d.EtapesDosier >= site.NiveauDossier && d.EtapesDosier <= site.NiveauMaxDossier && (d.IdSite == site.Id || d.IdSite == banqueID)
                                           ).ToList().ForEach(d =>
                                           {
                                               if (d.Site.BanqueId(db) == banqueID)/*Banque permissions*/
                                               {
                                                   if (!site.VoirClientAutres)//permission structure
                                                   {
                                                       if (d.IdSite == site.Id)
                                                       {
                                                           if (!role.VoirDossiersAutres && d.GestionnaireId == agentId)//permission de role
                                                           {
                                                               //dd.Add(d);
                                                               try
                                                               {
                                                                   if (rr.Count == 0)
                                                                   {
                                                                       rr.Add(d.ReferenceExterne);
                                                                   }
                                                                   else
                                                                   {
                                                                       if (rr.FirstOrDefault(r => r.Id == d.ReferenceExterneId) == null)
                                                                           rr.Add(d.ReferenceExterne);
                                                                   }
                                                               }
                                                               catch (Exception)
                                                               { }
                                                           }
                                                       }
                                                   }
                                                   else
                                                   {
                                                       //dd.Add(d);
                                                       try
                                                       {
                                                           if (rr.Count == 0)
                                                           {
                                                               rr.Add(d.ReferenceExterne);
                                                           }
                                                           else
                                                           {
                                                               if (rr.FirstOrDefault(r => r.Id == d.ReferenceExterneId) == null)
                                                                   rr.Add(d.ReferenceExterne);
                                                           }
                                                       }
                                                       catch (Exception)
                                                       { }
                                                   }
                                               }
                                           });
            }
            else
            {
                rr = db.GetReferenceBanques.Include("Dossiers").Include("Banque").ToList();
            }

            role = null;
            site = null;

            return rr;
        }

        /// <summary>
        /// ClientId
        /// </summary>
        /// <param name="id"></param>
        /// <param name="etapAttachementRef">L'etape du dossier lors de l'attachement de reference</param>
        /// <returns></returns>
        public IDictionary<int, InfoDocAcueil> infoDocBanque(int? etapAttachementRef,int? id = null)
        {
            if (_User == null || !(_User is CompteBanqueCommerciale)) return null;

            Dictionary<int, InfoDocAcueil> doc = new Dictionary<int, InfoDocAcueil>();
            Client client = null;
            var agenceId = (_User as CompteBanqueCommerciale).IdStructure;
            var roleID = (_User as CompteBanqueCommerciale).IdXRole;
            var banqueId = (_User as CompteBanqueCommerciale).Structure.BanqueId(db);
            var role = db.XtraRoles.Find(roleID);

            try
            {
                if (id !=null)
                {
                    client = db.GetClients.Find(id);
                }

                if (client != null)
                {
                    dossiers = db.GetDossiers.Where(d => d.IdSite == agenceId && d.EtapesDosier > 1 && d.ClientId == id).ToList();
                    this.TotalDossiers = dossiers.Count;
                    this.Dossiers.AddRange((_User as CompteBanqueCommerciale).Structure.Dossiers.Where(d => d.EtapesDosier > 0));
                    this.Clients.AddRange((from c in (_User as CompteBanqueCommerciale).Structure.Clients select c.Client).ToList());
                }
                else
                {
                    if (!(_User as CompteBanqueCommerciale).EstAdmin)
                    {
                        dossiers = new List<Dossier>();
                        var estAgence = (_User as CompteBanqueCommerciale).Structure.EstAgence;

                        #region Provisoire
                        var roleId = (_User as CompteBanqueCommerciale).IdXRole;
                        var site = db.Structures.Find(agenceId);
                        dossiers = UserBanqueDossiers(db, site, role, banqueId, _User.Id);
                        if (false)
                        {
                            //Agence
                            if (role.Nom == "Gestionnaire")
                            {
                                db.GetDossiers.Include("Client").ToList().ForEach(d =>
                                {
                                    if (d.IdSite == agenceId && d.EtapesDosier > 0 && d.GestionnaireId == _User.Id)
                                    {
                                        dossiers.Add(d);
                                    }
                                });
                            }
                            else if (role.Nom == "Chef agence")
                            {
                                dossiers = db.GetDossiers.Where(d => d.IdSite == agenceId && d.EtapesDosier > 0).ToList();
                            }
                            else if (role.Nom == " Back office")
                            {
                                dossiers = db.GetDossiers.Where(d => d.IdSite == agenceId && d.EtapesDosier > 0 && d.EtapesDosier < 3).ToList();
                            }
                            //Fin agence

                            //Direction metiers
                            if (role.Nom == "Direction metier")
                            {
                                db.GetDossiers.Include("Site").ToList().ForEach(d =>
                                {
                                    if (d.Site.IdDirectionMetier == agenceId && d.EtapesDosier > 0)
                                    {
                                        dossiers.Add(d);
                                    }
                                });
                            }
                            //fin direction metier

                            //conformité
                            if (role.Nom == "Conformité")
                            {
                                dossiers = db.GetDossiers.Where(d => d.EtapesDosier == 3).ToList();
                            }
                            //fin conformite

                            //service transfert
                            if (role.Nom == "Service transfert")
                            {
                                dossiers = db.GetDossiers.Where(d => d.EtapesDosier >= 4).ToList();
                            }
                            //fin service transfert 
                        }
                        #endregion

                    }
                    else
                    {
                        dossiers = db.GetDossiers.Where(d => d.Site.BanqueId(db) == banqueId && d.EtapesDosier > 0).ToList();
                    }
                }
            }
            catch (Exception)
            { }

            ///Dossiers non attachés de reference banque
            try
            {
                //if (etapAttachementRef<6)
                {
                    var count = dossiers.Count;
                    foreach (var item in dossiers)
                    {
                        try
                        {
                            if ((int)item.EtapesDosier < etapAttachementRef)
                            {
                                if (!doc.Keys.Contains((int)item.EtapesDosier))
                                {
                                    doc.Add((int)item.EtapesDosier, new InfoDocAcueil()
                                    {
                                        Date = item.DateCreationApp,
                                        Nbr = 1,
                                        EtapeDossier = (int)item.EtapesDosier,
                                        Percentage = 1 * 100 / count
                                    });
                                }
                                else
                                {
                                    doc[(int)item.EtapesDosier].SetDurée(item.DateCreationApp);
                                    doc[(int)item.EtapesDosier].Nbr++;
                                    doc[(int)item.EtapesDosier].Percentage += doc[2].Nbr * 100 / count;
                                } 
                            }
                        }
                        catch (Exception e)
                        { }
                    } 
                }
            }
            catch (Exception)
            { }
           
            ///Dossiers attachés de reference banque 
            try
            {
                //if (etapAttachementRef>=6)
                {
                    referenceBanques = db.GetReferenceBanques.Where(r => !r.Apuré && !r.Echus && r.BanqueId == banqueId).ToList();
                    var count = referenceBanques.Count;
                    foreach (ReferenceBanque item in referenceBanques)
                    {
                        try
                        {
                            if ((int)item.EtapesDosier>=etapAttachementRef)
                            {
                                if (!doc.Keys.Contains((int)item.EtapesDosier))
                                {
                                    doc.Add((int)item.EtapesDosier, new InfoDocAcueil()
                                    {
                                        Date =DateTime.Now,// (DateTime)item.DateCredit,
                                        Nbr = 1,
                                        EtapeDossier = (int)item.EtapesDosier,
                                        Percentage = 1 * 100 / count
                                    });
                                }
                                else
                                {
                                    doc[(int)item.EtapesDosier].SetDurée((DateTime)item.DateCredit);
                                    doc[(int)item.EtapesDosier].Nbr++;
                                    doc[(int)item.EtapesDosier].Percentage = doc[2].Nbr * 100 / count;
                                } 
                            }
                        }
                        catch (Exception e)
                        { }
                    } 
                }
            }
            catch (Exception)
            { }

            return doc;
        }

        #endregion

    }

    [NotMapped]
    public class InfoDocAcueil
    {
        public int Nbr { get; set; }
        public int EtapeDossier { get; set; }

        public double Percentage;
        public string Durée;
        public DateTime Date;
        public EtatDossier EtatDossier;

        internal void SetDurée(DateTime dateCreationApp)
        {
            if (dateCreationApp > Date)
                Date = dateCreationApp;
        }

        //public DateTime GetDate()
        //{

        //}

        public string GetDuree()
        {
            var dif = (DateTime.Now - Date).TotalDays;

            if (Date !=new DateTime())
            {
                if (dif >= 90)
                    return "Depuis 3 mois";
                if (dif >= 60)
                    return "Depuis 2 mois";
                if (dif >= 30)
                    return "Depuis 1 mois";
                if (dif >= 14)
                    return "Depuis 2 semaines";
                if (dif >= 7)
                    return "Depuis 1 semaine";
                else
                    return "Recemment"; 
            }
            return "Pour l'instant";
        }

        public double GetPercentage()
        {
            return Math.Round((double)Percentage, 2);
        }
    }

    public class GestionnaireTmp
    {
        public string NomComplet { get; set; }
        public string Tel { get; set; }
        public string Email { get; set; }
        public string Banque { get; set; }

        public GestionnaireTmp(string nom,string tel,string email,string banque)
        {
            NomComplet = nom;Tel = tel;Email = email; Banque= banque;
        }
    }

    public class DossierParAgence
    {
        [DisplayName("Nom agence")]
        public string NomAgence { get; set; }
        public int NbrDossiers { get; set; }
        public int? EtapeDosiier { get; set; }
        public int IdAgence { get; set; }
    }
    
    public class DossierParReference
    {
        [DisplayName("Nom agence")]
        public string NumeroRef { get; set; }
        public int NbrDossiers { get; set; }
        public int? EtapeDosiier { get; set; }
        public int IdClient { get; set; }
        public string NomClient { get; set; }
        public int IdAgence { get; set; }
        public string NomAgence { get; set; }
    }
}