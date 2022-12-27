using genetrix.Models;
using genetrix.Models.Fonctions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace genetrix.Models
{
    public class VariablGlobales
    {
        private ApplicationDbContext db;
        public ApplicationUser _User { get; set; }
        public Banque Banque { get; internal set; }

        public static IDictionary<string,List<UserInfo>> UserInfos { get; set; }

        public int TotalDossiers;
        public List<BanqueClient> BanqueClients = new List<BanqueClient>();
        public List<Dossier> Dossiers = new List<Dossier>();
        public List<ReferenceBanque> DossiersBEAC { get; set; }

        public List<Fournisseurs> Fournisseurs = new List<Fournisseurs>();
        public List<Client> Clients = new List<Client>();
        public List<GestionnaireTmp> Gestionnaires = new List<GestionnaireTmp>();

        #region attribut par numero
        public Dictionary<int?, InfoDocAcueil2> Datas =null;
        public Dictionary<int?, InfoDocAcueil3> DatasUnique = null;
        public List<InfoDocAcueil3> DonneeApurement = null;
        public List<InfoDocAcueil2> DossiersEtat = new List<InfoDocAcueil2>();
        public Dictionary<int?, InfoDocAcueil2> DatasDFX = new Dictionary<int?, InfoDocAcueil2>();
        public InfoDocAcueil2 GetDataByEtape(int? eta)
        {
            InfoDocAcueil2 info = new InfoDocAcueil2();
            try
            {
                info = Datas[eta];
            }
            catch (Exception)
            { }
            return info;
        }

        public InfoDocAcueil2 GetDataByEtapes(int?[] etas,DosType dosType=default)
        {
            InfoDocAcueil2 info = new InfoDocAcueil2();
            try
            {
                IDictionary<int?, InfoDocAcueil2> _datas = null;
                var tg = ((byte)dosType);
                if (dosType!=default)
                    _datas = Datas.Values.Where(d => d.DFX6FP6BEAC == ((byte)dosType)).ToDictionary(d => (int?)d.EtapeDossier);
                else
                    _datas = Datas;
                foreach (var eta in etas)
                {
                    try
                    {
                        info.Nbr += _datas[eta].Nbr;
                        info.NbrValides += _datas[eta].NbrTraite;
                        info.NbrRejeter += _datas[eta].NbrRejeter;

                        if (info.DateNouvelleEtape> _datas[eta].DateNouvelleEtape)
                        {
                            info.DateNouvelleEtape = _datas[eta].DateNouvelleEtape;
                        }
                    }
                    catch (Exception ee)
                    { }
                }
                var jours = info.GetDureeNbr();
                _datas.Values.ToList().ForEach(d =>
                {
                    if((DateTime.Now -d.DateNouvelleEtape).Value.Days >= jours)
                        info.NbrAnciens++;
                });
            }
            catch (Exception e)
            { }
            return info;
        }
        
        public InfoDocAcueil2 GetDataByEtapesApurement(int?[] etas,DosType dosType=default,bool IsComplet=false)
        {
            InfoDocAcueil2 info = new InfoDocAcueil2();
            try
            {
                IDictionary<int?, InfoDocAcueil2> _datas = null;
                if (dosType!=default)
                    _datas = Datas.Values.Where(d => d.DFX6FP6BEAC == ((byte)dosType) && d.IsEtapeComplete==IsComplet).ToDictionary(d => (int?)d.EtapeDossier);
                else
                    _datas = Datas;
                foreach (var eta in etas)
                {
                    try
                    {
                        info.Nbr += _datas[eta].Nbr;
                        info.NbrValides += _datas[eta].NbrTraite;
                        info.NbrRejeter += _datas[eta].NbrRejeter;

                        if (info.DateNouvelleEtape> _datas[eta].DateNouvelleEtape)
                        {
                            info.DateNouvelleEtape = _datas[eta].DateNouvelleEtape;
                        }
                    }
                    catch (Exception ee)
                    { }
                }
                var jours = info.GetDureeNbr();
                _datas.Values.ToList().ForEach(d =>
                {
                    if((DateTime.Now -d.DateNouvelleEtape).Value.Days >= jours)
                        info.NbrAnciens++;
                });
            }
            catch (Exception e)
            { }
            return info;
        }
        
        public List<InfoDocAcueil3> GetDataByEtapesModel(int?[] etas,byte? DFREF=null)
        {
            var infos = new List<InfoDocAcueil3>();
            try
            {
                IEnumerable<KeyValuePair<int?, InfoDocAcueil3>> _datas = null;
                if (DFREF==null)
                {
                    _datas = from d in DatasUnique
                             where etas.Contains(d.Key)
                             select d;
                }
                else
                {
                    _datas = from d in DatasUnique
                             where d.Value.DFX6FP6BEAC==DFREF && etas.Contains(d.Key)
                             select d;
                }
                
                var info = new InfoDocAcueil3();
                foreach (var eta in _datas)
                {
                    info = new InfoDocAcueil3();
                    try
                    {
                        info.Apurement = eta.Value.Apurement;
                        info.DFX6FP6BEAC = eta.Value.DFX6FP6BEAC;
                        info.Devise = eta.Value.Devise;
                        info.DfxId = eta.Value.DfxId;
                        info.EstConformite = eta.Value.EstConformite;
                        info.EtapeDossier = eta.Value.EtapeDossier;
                        info.IsEtapeComplete = eta.Value.IsEtapeComplete;
                        info.IsRejeter = eta.Value.IsRejeter;
                        info.IsTraite = eta.Value.IsTraite;
                        info.IsValide = eta.Value.IsValide;
                        info.RefId = eta.Value.RefId;
                        
                        infos.Add(info);
                    }
                    catch (Exception ee)
                    { }
                }
                
            }
            catch (Exception e)
            { }
            return infos;
        }

        public IEnumerable<InfoDocAcueil3> GetApurementDataByEtapesModel(int?[] etas,byte? DFREF=null)
        {
            List<InfoDocAcueil3> infos = new List<InfoDocAcueil3>();
            try
            {
                List<InfoDocAcueil3> _datas = null;
                if (DFREF==null)
                {
                    _datas = (from d in DonneeApurement
                             where etas.Contains(d.EtapeDossier)
                             select d).ToList();
                }
                else
                {
                    _datas = (from d in DonneeApurement
                             where d.DFX6FP6BEAC==DFREF && etas.Contains(d.EtapeDossier)
                             select d).ToList();
                }
                
                var info = new InfoDocAcueil3();
                foreach (var eta in _datas)
                {
                    info = new InfoDocAcueil3();
                    try
                    {
                        info.Id = eta.Id;
                        info.Apurement = eta.Apurement;
                        info.DFX6FP6BEAC = eta.DFX6FP6BEAC;
                        info.Devise = eta.Devise;
                        info.DfxId = eta.DfxId;
                        info.EstConformite = eta.EstConformite;
                        info.EtapeDossier = eta.EtapeDossier;
                        info.IsEtapeComplete = eta.IsEtapeComplete;
                        info.IsRejeter = eta.IsRejeter;
                        info.IsTraite = eta.IsTraite;
                        info.IsValide = eta.IsValide;
                        info.RefId = eta.RefId;
                        info.MontantString = eta.MontantString;
                        info.NombreDossiers = eta.NombreDossiers;
                        info.Delai = eta.Delai;
                        info.Fournisseur = eta.Fournisseur;
                        info.Client = eta.Client;
                        info.GetColor = eta.GetColor;
                        info.GetEtapDossier = eta.GetEtapDossier;
                        infos.Add(info);
                    }
                    catch (Exception ee)
                    { }
                }
                
            }
            catch (Exception e)
            { }
            return infos;
        }
        
        public InfoDocAcueil2 GetDataBankByEtapes(int?[] etas)
        {
            InfoDocAcueil2 info = new InfoDocAcueil2();
            try
            {
                foreach (var eta in etas)
                {
                    try
                    {
                        info.Nbr += Datas[eta].Nbr;
                        if (info.DateNouvelleEtape>Datas[eta].DateNouvelleEtape)
                        {
                            info.DateNouvelleEtape = Datas[eta].DateNouvelleEtape;
                        }
                    }
                    catch (Exception)
                    { }
                }
                var jours = info.GetDureeNbr();
                Datas.Values.ToList().ForEach(d =>
                {
                    if((DateTime.Now -d.DateNouvelleEtape).Value.Days >= jours)
                        info.NbrAnciens++;
                });
            }
            catch (Exception)
            { }
            return info;
        }
        
        /// <summary>
        ///
        /// </summary>
        /// <param name="etas"></param>
        /// <param name="dfx_ref">0=tous; 1=dfx; 2=refinancement</param>
        /// <returns></returns>
        public InfoDocAcueil2 GetAllDataBankByEtapes(int?[] etas,byte dfx_ref)
        {
            InfoDocAcueil2 info = new InfoDocAcueil2();
            try
            {
                foreach (var eta in etas)
                {
                    if (dfx_ref == 0)
                    {
                        try
                        {
                            info.Nbr += Datas[eta].Nbr;
                            info.NbrRef += Datas[eta].Nbr;
                            if (info.DateNouvelleEtape > Datas[eta].DateNouvelleEtape)
                            {
                                info.DateNouvelleEtape = Datas[eta].DateNouvelleEtape;
                            }
                        }
                        catch (Exception)
                        { } 
                        try
                        {
                            info.Nbr += DatasDFX[eta].Nbr;
                            info.NbrDFX += DatasDFX[eta].Nbr;
                            if (info.DateNouvelleEtape > DatasDFX[eta].DateNouvelleEtape)
                            {
                                info.DateNouvelleEtape = DatasDFX[eta].DateNouvelleEtape;
                            }
                        }
                        catch (Exception)
                        { } 
                    }
                    if (dfx_ref == 2)
                    {
                        try
                        {
                            info.Nbr += Datas[eta].Nbr;
                            info.NbrRef += Datas[eta].Nbr;
                            if (info.DateNouvelleEtape > Datas[eta].DateNouvelleEtape)
                            {
                                info.DateNouvelleEtape = Datas[eta].DateNouvelleEtape;
                            }
                        }
                        catch (Exception)
                        { } 
                    }
                    if (dfx_ref == 1)
                    {
                        try
                        {
                            info.Nbr += DatasDFX[eta].Nbr;
                            info.NbrDFX += DatasDFX[eta].Nbr;
                            if (info.DateNouvelleEtape > DatasDFX[eta].DateNouvelleEtape)
                            {
                                info.DateNouvelleEtape = DatasDFX[eta].DateNouvelleEtape;
                            }
                        }
                        catch (Exception ee)
                        { } 
                    }
                }
                var jours = info.GetDureeNbr();
                if (dfx_ref == 2 || dfx_ref == 0)
                {
                    try
                    {
                        Datas.Values.ToList().ForEach(d =>
                        {
                            if (etas.Contains(d.EtapeDossier))
                            {
                                if ((DateTime.Now - d.DateNouvelleEtape).Value.Days >= jours)
                                    info.NbrAnciens++; 
                            }
                        });
                    }
                    catch (Exception)
                    { }  
                }
                if (dfx_ref == 1 || dfx_ref == 0)
                {
                    try
                    {
                        DatasDFX.Values.ToList().ForEach(d =>
                        {
                            if (etas.Contains(d.EtapeDossier))
                            {
                                if ((DateTime.Now - d.DateNouvelleEtape).Value.Days >= jours)
                                    info.NbrAnciens++; 
                            }
                        });
                    }
                    catch (Exception)
                    { } 
                }
            }
            catch (Exception)
            { }
            return info;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="etas"></param>
        /// <param name="devise">0: toutes; 1: eur; 2: usd;3: autres</param>
        /// <returns></returns>
        public InfoDocAcueil2 GetAllDataBankByEtapesDevise(int?[] etas,byte devise,bool estBeac)
        {
            InfoDocAcueil2 info = new InfoDocAcueil2();
            try
            {
                foreach (var eta in etas)
                {
                    try
                    {
                        var beac = Datas[eta].DFX6FP6BEAC;
                        if (devise == 0)
                        {
                            try
                            {
                                info.Nbr += Datas[eta].Nbr;
                                info.NbrAutreDevise += Datas[eta].NbrAutreDevise;
                                info.NbrUSD += Datas[eta].NbrUSD;
                                info.NbrEUR += Datas[eta].NbrEUR;
                                if (info.DateNouvelleEtape > Datas[eta].DateNouvelleEtape)
                                {
                                    info.DateNouvelleEtape = Datas[eta].DateNouvelleEtape;
                                }
                            }
                            catch (Exception)
                            { }
                        }
                        else if (devise == 1)
                        {
                            try
                            {
                                info.Nbr += Datas[eta].Nbr;
                                info.NbrEUR += Datas[eta].NbrEUR;
                                if (info.DateNouvelleEtape > DatasDFX[eta].DateNouvelleEtape)
                                {
                                    info.DateNouvelleEtape = DatasDFX[eta].DateNouvelleEtape;
                                }
                            }
                            catch (Exception ee)
                            { }
                        }
                        else if (devise == 2)
                        {
                            try
                            {
                                info.Nbr += Datas[eta].Nbr;
                                info.NbrEUR += Datas[eta].NbrUSD;
                                if (info.DateNouvelleEtape > DatasDFX[eta].DateNouvelleEtape)
                                {
                                    info.DateNouvelleEtape = DatasDFX[eta].DateNouvelleEtape;
                                }
                            }
                            catch (Exception ee)
                            { }
                        }
                    }
                    catch (Exception)
                    {}
                }
            }
            catch (Exception)
            { }
            return info;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="etas"></param>
        /// <param name="devise">0:all; 1: euro; 2: autre</param>
        /// <param name="estBeac"></param>
        /// <param name="all_beac_non"></param>
        /// <param name="estPasseConformite">0:all; 1:true; 2: false</param>
        /// <returns></returns>
        public List<InfoDocAcueil2> GetAllDataBankByEtapesDeviseBeac(int?[] etas,byte devise,byte estBeac,bool all_beac_non=false,byte estPasseConformite=0)
        {
            List<InfoDocAcueil2> infos = new List<InfoDocAcueil2>();
            try
            {
                InfoDocAcueil2 info = new InfoDocAcueil2();
                bool _estCOnformiteF = false;
                bool _estCOnformiteV = true;
                if (estPasseConformite==1)
                {
                    _estCOnformiteF = true;
                }
                else if(estPasseConformite==2)
                {
                    _estCOnformiteV = false;
                }

                foreach (var eta in etas)
                {
                    info = new InfoDocAcueil2();
                    try
                    {
                        if (!all_beac_non)
                        {
                            //var beac = Datas[eta].EstBEAC;
                            if (devise == 0)
                            {
                                try
                                {
                                    //infos.AddRange(DossiersEtat.Where(d => d.EtapeDossier == eta && d.DFX6FP6BEAC == estBeac && (!_estCOnformiteF&& !d.EstConformite || _estCOnformiteV && d.EstConformite)));
                                    infos.AddRange(DossiersEtat.Where(d => d.EtapeDossier == eta && (!_estCOnformiteF&& !d.EstConformite || _estCOnformiteV && d.EstConformite)));
                                }
                                catch (Exception)
                                { }
                            }
                            else if (devise == 1)
                            {
                                try
                                {
                                    //infos.AddRange(DossiersEtat.Where(d => d.EtapeDossier == eta && d.DFX6FP6BEAC == estBeac && d.NbrEUR == 1 && (!_estCOnformiteF&& !d.EstConformite || _estCOnformiteV && d.EstConformite)));
                                    infos.AddRange(DossiersEtat.Where(d => d.EtapeDossier == eta && d.NbrEUR == 1 && (!_estCOnformiteF&& !d.EstConformite || _estCOnformiteV && d.EstConformite)));
                                }
                                catch (Exception ee)
                                { }
                            }
                            else if (devise == 2)
                            {
                                try
                                {
                                    //infos.AddRange(DossiersEtat.Where(d => d.EtapeDossier == eta && d.DFX6FP6BEAC == estBeac && (d.NbrUSD == 1 || d.NbrAutreDevise == 1 && (!_estCOnformiteF&& !d.EstConformite || _estCOnformiteV && d.EstConformite))));
                                    infos.AddRange(DossiersEtat.Where(d => d.EtapeDossier == eta && (d.NbrUSD == 1 || d.NbrAutreDevise == 1 && (!_estCOnformiteF&& !d.EstConformite || _estCOnformiteV && d.EstConformite))));
                                }
                                catch (Exception ee)
                                { }
                            }
                        }
                        else
                        {
                            try
                            {
                                if (devise == 0)
                                {
                                    try
                                    {
                                        infos.AddRange(DossiersEtat.Where(d => d.EtapeDossier == eta && (!_estCOnformiteF&& !d.EstConformite || _estCOnformiteV && d.EstConformite)));
                                    }
                                    catch (Exception)
                                    { }
                                    var bb = (_estCOnformiteF && false || _estCOnformiteV & false);
                                }
                                else if (devise == 1)
                                {
                                    try
                                    {
                                        infos.AddRange(DossiersEtat.Where(d => d.EtapeDossier == eta && d.NbrEUR == 1 && (!_estCOnformiteF&& !d.EstConformite || _estCOnformiteV && d.EstConformite)));
                                    }
                                    catch (Exception ee)
                                    { }
                                }
                                else if (devise == 2)
                                {
                                    try
                                    {
                                        infos.AddRange(DossiersEtat.Where(d => d.EtapeDossier == eta && (d.NbrUSD == 1 || d.NbrAutreDevise == 1) && (!_estCOnformiteF&& !d.EstConformite || _estCOnformiteV && d.EstConformite)));
                                    }
                                    catch (Exception ee)
                                    { }
                                }
                            }
                            catch (Exception ee)
                            { }
                        }
                    }
                    catch (Exception e)
                    { }
                }
            }
            catch (Exception)
            { }
            return infos;
        }
        
        public InfoDocAcueil2 GetRejetDataBank(int etaRejet,int etaAncien,byte dfx_ref)
        {
            int eta = Math.Abs(etaRejet*10) + etaAncien;
            InfoDocAcueil2 info = new InfoDocAcueil2();
            if (etaRejet >= 0) return info;
            try
            {
                if (dfx_ref == 2 || dfx_ref == 0)
                {
                    try
                    {
                        info.Nbr += Datas[eta].Nbr;
                        info.NbrRef += Datas[eta].Nbr;
                    }
                    catch (Exception)
                    { }
                }
                if (dfx_ref == 1 || dfx_ref == 0)
                {
                    try
                    {
                        info.Nbr += DatasDFX[eta].Nbr;
                        info.NbrDFX += DatasDFX[eta].Nbr;
                    }
                    catch (Exception)
                    { }
                }
               
            }
            catch (Exception)
            { }
            return info;
        }
                
        #endregion

        public VariablGlobales(ApplicationDbContext _db, ApplicationUser _user)
        {
            db = _db;
            _User = _user;
            Datas = new Dictionary<int?, InfoDocAcueil2>();
            DatasUnique = new Dictionary<int?, InfoDocAcueil3>();
            DonneeApurement = new List<InfoDocAcueil3>();
        }

        public static Dictionary<string, bool> Access(CompteBanqueCommerciale user, string type)
        {
            Dictionary<string, bool> resul = new Dictionary<string, bool>();

            try
            {
                if (user.EstAdmin)
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
        public IDictionary<int?, InfoDocAcueil> infoDoc(string id = null)
        {
            Dictionary<int?, InfoDocAcueil> doc = new Dictionary<int?, InfoDocAcueil>();
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
                int traite = 0, traite_dfx = 0, traite_ref = 0,
                    rejet = 0, rejet_dfx = 0, rejet_ref = 0, encours = 0, dfx = 0, dfx_recu = 0,
                    refinancement = 0, refinancement_recu = 0;
                foreach (var item in dossiers)
                {
                    try
                    {
                        if (!doc.Keys.Contains((int)item.EtapesDosier))
                        {
                            if (item.EtapesDosier == 4 || item.EtapesDosier == 6)
                                traite = 1;
                            else if (item.EtapesDosier < 0)
                                rejet = 1;
                            else encours = 1;

                            if (item.Montant <= 50000000)
                            {
                                if (item.EtapesDosier == 5) dfx_recu = 1;
                                if (item.EtapesDosier == 10) traite_dfx = 1;
                                if (item.EtapesDosier < 0) rejet_dfx = 1;
                                dfx = 1;
                            }
                            else
                            {
                                if (item.EtapesDosier == 5) refinancement_recu = 1;
                                if (item.EtapesDosier == 10) traite_ref = 1;
                                if (item.EtapesDosier < 0) rejet_ref = 1;
                                refinancement = 1;
                            }

                            doc[(int)item.EtapesDosier] = new InfoDocAcueil()
                            {
                                Date = item.DateCreationApp,
                                Nbr = 1,
                                NbrRejets = rejet,
                                NbrRejets_DFX = rejet_dfx,
                                NbrRejets_Refinancement = rejet_ref,
                                NbrValidés = traite,
                                NbrValidés_DFX = traite_dfx,
                                NbrValidés_Refinancement = traite_ref,
                                EtapeDossier = (int)item.EtapesDosier,
                                NbrDFX = dfx,
                                NbrDFX_recu = dfx_recu,
                                NbrRefinanacement = refinancement,
                                NbrRefinanacement_recu = refinancement_recu,
                                Percentage = 1// 1 * 100 / count
                            };
                        }
                        else
                        {
                            doc[(int)item.EtapesDosier].SetDurée(item.DateCreationApp);
                            doc[(int)item.EtapesDosier].Nbr++;
                            if (item.EtapesDosier < 0)
                                doc[(int)item.EtapesDosier].NbrRejets++;
                            else if (item.EtapesDosier == 4 || item.EtapesDosier == 6)
                                doc[(int)item.EtapesDosier].NbrValidés++;
                            else doc[(int)item.EtapesDosier].NbrEncours++;

                            if (item.Montant <= 50000000)
                            {
                                if (item.EtapesDosier == 5)
                                    doc[(int)item.EtapesDosier].NbrDFX_recu++;
                                if (item.EtapesDosier == 10)
                                    doc[(int)item.EtapesDosier].NbrValidés_DFX++;
                                if (item.EtapesDosier < 0)
                                    doc[(int)item.EtapesDosier].NbrRejets_DFX++;
                                doc[(int)item.EtapesDosier].NbrDFX++;
                            }
                            else
                            {
                                if (item.EtapesDosier == 5)
                                    doc[(int)item.EtapesDosier].NbrRefinanacement_recu++;
                                if (item.EtapesDosier == 10)
                                    doc[(int)item.EtapesDosier].NbrValidés_Refinancement++;
                                if (item.EtapesDosier < 0)
                                    doc[(int)item.EtapesDosier].NbrRejets_Refinancement++;
                                doc[(int)item.EtapesDosier].NbrRefinanacement++;
                            }

                            doc[(int)item.EtapesDosier].Percentage = doc[(int)item.EtapesDosier].Nbr * 100 / count;
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

        public void infoDoc2(string id = null,bool apurement=false)
        {
            Datas = new Dictionary<int?, InfoDocAcueil2>();
            var references = new List<Reference>();
            int? etat = 0;
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    _User = db.Users.Find(id);
                }

                if (_User != null)
                {
                    var clientId = (_User as CompteClient).ClientId;
                    
                    if (apurement)
                    {
                        Dossier item = null;
                        bool estComplet = false;
                        //DFX
                        foreach (var d in db.GetDossiers.OrderByDescending(d=>d.EtapesDosier).Where(d =>d.DfxId>0 && d.Client.Id == clientId && d.DFX6FP6BEAC == 1).GroupBy(d => d.DfxId))
                            try
                            {
                                item = d.FirstOrDefault();
                                etat = item.EtapesDosier == null ? 1000 : item.EtapesDosier;
                                if (d.Count() == 1) estComplet = true;
                                else if (d.Count() > 1 && d.Where(_d => _d.EtapesDosier == etat).Count() == d.Count())
                                    estComplet=true;
                                if (!Datas.Keys.Contains((int)etat))
                                {
                                    Datas.Add(etat, new InfoDocAcueil2()
                                    {
                                        EtapeDossier = item.EtapesDosier,
                                        Nbr = 1,
                                        IsEtapeComplete=estComplet,
                                        DFX6FP6BEAC = item.DFX6FP6BEAC,
                                        NbrDFX=1,
                                        NbrEUR=1,
                                        EstConformite=item.EstPasséConformite?true:false,
                                        NbrTraite=1,
                                        Apurement=item.Apurement
                                    });
                                }
                                else
                                {
                                    Datas[etat].NbrDFX++;
                                    Datas[etat].NbrEUR++;
                                    Datas[etat].EstConformite = item.EstPasséConformite ? true : false;
                                    Datas[etat].NbrTraite++;
                                    Datas[etat].Nbr++;
                                }
                            }
                            catch (Exception e)
                            { }
                        //REF
                        foreach (var d in db.GetDossiers.OrderByDescending(d => d.EtapesDosier).Where(d => d.ReferenceExterneId > 0 && d.Client.Id == clientId && d.DFX6FP6BEAC == 3).GroupBy(d => d.ReferenceExterneId))
                            try
                            {
                                item = d.FirstOrDefault();
                                etat = item.EtapesDosier == null ? 1000 : item.EtapesDosier;
                                if (!Datas.Keys.Contains((int)etat))
                                {
                                    Datas.Add(etat, new InfoDocAcueil2()
                                    {
                                        Nbr = 1,
                                        IsEtapeComplete = estComplet,
                                        DFX6FP6BEAC = item.DFX6FP6BEAC,
                                        NbrRef = 1,
                                        NbrEUR = item.DeviseToString2Caracters=="eu"?1:0,
                                        NbrUSD = item.DeviseToString2Caracters=="us"?1:0,
                                        EstConformite = item.EstPasséConformite ? true : false,
                                        NbrTraite = 1,
                                        Apurement = item.Apurement
                                    });
                                }
                                else
                                {
                                    Datas[etat].NbrDFX++;
                                    Datas[etat].NbrEUR += item.DeviseToString2Caracters == "eu" ? 1 : 0;
                                    Datas[etat].NbrUSD += item.DeviseToString2Caracters == "us" ? 1 : 0;
                                    Datas[etat].EstConformite = item.EstPasséConformite ? true : false;
                                    Datas[etat].NbrTraite++;
                                    Datas[etat].Nbr++;
                                }
                            }
                            catch (Exception e)
                            { }
                        dossiers = db.GetDossiers.Where(d => d.Client.Id == clientId && d.Apurement).ToList();
                    }
                    else
                        dossiers = db.GetDossiers.Where(d => d.Client.Id == clientId).ToList();
                    this.TotalDossiers = dossiers.Count;
                    this.BanqueClients.AddRange((_User as CompteClient).Client.Banques);
                    this.Dossiers.AddRange((_User as CompteClient).Client.Dossiers.Where(d => d.EtapesDosier <= 22));
                    this.Fournisseurs.AddRange((_User as CompteClient).Client.Fournisseurs);
                    var banques = (_User as CompteClient).Client.Banques.ToList();
                    this.Gestionnaires = new List<GestionnaireTmp>();
                    foreach (var item in banques)
                    {
                        this.Gestionnaires.Add(new GestionnaireTmp(item.Gestionnaire.NomComplet, item.Gestionnaire.Tel1, item.Gestionnaire.Email, item.Site.Nom));
                    }
                }
                else
                {
                    if (apurement)
                    {
                        try
                        {
                            foreach (var item in db.GetReferences.Where(d => d.DFX6FP6BEAC != 2))
                            {
                                etat = item.EtapesDosier == null ? 1000 : item.EtapesDosier;
                                if (!Datas.Keys.Contains((int)etat))
                                {
                                    Datas.Add(etat, new InfoDocAcueil2()
                                    {
                                        EtapeDossier = item.EtapesDosier,
                                        Nbr = 1,
                                        IsEtapeComplete = item.IsEtapeComplete,
                                        DFX6FP6BEAC = item.DFX6FP6BEAC,
                                        Apurement = true,
                                        NbrDFX = item.DFX6FP6BEAC==1?1:0,
                                        NbrEUR = item.DeviseToString2Caracters == "eu" ? 1 : 0,
                                        NbrUSD = item.DeviseToString2Caracters == "us" ? 1 : 0,
                                        NbrAutreDevise = item.DeviseToString2Caracters != "us" && item.DeviseToString2Caracters != "eu" ? 1 : 0,
                                        NbrTraite = 1,
                                    });
                                }
                                else
                                {
                                    Datas[etat].Nbr++;
                                }
                            }
                        }
                        catch (Exception e)
                        { }
                    }
                    else
                        dossiers = db.GetDossiers.ToList();
                }
            }
            catch (Exception)
            { }
            try
            {
                var count = dossiers!=null?dossiers.Count:0;
                if (apurement && false)
                {
                    
                    foreach (var item in references)
                    {
                        try
                        {
                            etat = item.EtapesDosier == null ? 1000 : item.EtapesDosier;
                            if (!Datas.Keys.Contains((int)etat))
                            {
                                Datas.Add(etat, new InfoDocAcueil2()
                                {
                                    EtapeDossier = item.EtapesDosier,
                                    Nbr = 1,
                                    IsEtapeComplete = item.IsEtapeComplete,
                                    DFX6FP6BEAC = item.DFX6FP6BEAC
                                });
                            }
                            else
                            {
                                Datas[etat].Nbr++;
                                //if (Datas[etat].DateNouvelleEtape > item.Key.DateNouvelleEtape)
                                //{
                                //    Datas[etat].DateNouvelleEtape = item.DateNouvelleEtape;
                                //    Datas[etat].DFX6FP6BEAC = item.DFX6FP6BEAC;
                                //}
                            }
                        }
                        catch (Exception e)
                        { }
                    }
                
                }
                else
                {
                    foreach (var item in dossiers)
                    {
                        try
                        {
                            etat = item.EtapesDosier == null ? 1000 : item.EtapesDosier;
                            if (!Datas.Keys.Contains((int)etat))
                            {
                                Datas.Add(etat, new InfoDocAcueil2()
                                {
                                    DateNouvelleEtape = item.DateNouvelleEtape,
                                    EtapeDossier = item.EtapesDosier,
                                    Nbr = 1,
                                    DFX6FP6BEAC = item.DFX6FP6BEAC,
                                    EstConformite = item.EstPasséConformite,
                                    NbrAutreDevise = item.DeviseToString2Caracters != "us" && item.DeviseToString2Caracters != "eu" ? 1 : 0,
                                    NbrDFX = item.DFX6FP6BEAC == 1 ? 1 : 0,
                                    NbrRef = item.DFX6FP6BEAC != 1 ? 1 : 0,
                                    NbrEUR = item.DeviseToString2Caracters != "eu" ? 1 : 0,
                                    NbrUSD = item.DeviseToString2Caracters != "us" ? 1 : 0,
                                    NbrTraite = item.Traité ? 1 : 0,
                                    DfxId = item.DfxId,
                                    IsEtapeComplete = item.EstComplet
                                });
                            }
                            else
                            {
                                Datas[etat].Nbr++;
                                Datas[etat].DFX6FP6BEAC = item.DFX6FP6BEAC;
                                Datas[etat].EstConformite = item.EstPasséConformite;
                                Datas[etat].NbrAutreDevise = item.DeviseToString2Caracters != "us" && item.DeviseToString2Caracters != "eu" ? 1 : 0;
                                Datas[etat].NbrDFX += item.DFX6FP6BEAC == 1 ? 1 : 0;
                                Datas[etat].NbrRef += item.DFX6FP6BEAC != 1 ? 1 : 0;
                                Datas[etat].NbrEUR += item.DeviseToString2Caracters != "eu" ? 1 : 0;
                                Datas[etat].NbrUSD += item.DeviseToString2Caracters != "us" ? 1 : 0;
                                Datas[etat].NbrTraite += item.Traité ? 1 : 0;
                                Datas[etat].IsEtapeComplete = item.EstComplet;

                                if (Datas[etat].DateNouvelleEtape > item.DateNouvelleEtape)
                                {
                                    Datas[etat].DateNouvelleEtape = item.DateNouvelleEtape;
                                    Datas[etat].DFX6FP6BEAC = item.DFX6FP6BEAC;
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

        }

        public void infoDoc3(string id = null,bool apurement=false)
        {
            DatasUnique = new Dictionary<int?, InfoDocAcueil3>();
            var references = new List<Reference>();
            int? etat = 0;
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    _User = db.Users.Find(id);
                }

                if (_User != null)
                {
                    var clientId = (_User as CompteClient).ClientId;
                    
                    if (apurement)
                    {
                        Dossier item = null;
                        bool estComplet = false;
                        //DFX
                        foreach (var d in db.GetDossiers.OrderByDescending(d=>d.EtapesDosier).Where(d =>d.DfxId>0 && d.Client.Id == clientId && d.DFX6FP6BEAC == 1).GroupBy(d => d.DfxId))
                            try
                            {
                                item = d.FirstOrDefault();
                                etat = item.EtapesDosier == null ? 1000 : item.EtapesDosier;
                                if (d.Count() == 1) estComplet = true;
                                else if (d.Count() > 1 && d.Where(_d => _d.EtapesDosier == etat).Count() == d.Count())
                                    estComplet=true;
                                DatasUnique.Add(etat, new InfoDocAcueil3()
                                {
                                    EtapeDossier = item.EtapesDosier,
                                    Nbr = 1,
                                    IsEtapeComplete = estComplet,
                                    DFX6FP6BEAC = item.DFX6FP6BEAC,
                                    EstConformite = item.EstPasséConformite ? true : false,
                                    Apurement = item.Apurement,
                                    Devise=item.DeviseToString,
                                    DfxId=item.DfxId,
                                    IsRejeter=item.IsRejete,
                                    IsTraite=item.Traité,
                                    RefId=item.ReferenceExterneId,
                                    Delai=item.GetDelai,
                                    Fournisseur=item.GetFournisseur,
                                    MontantString=item.MontantString,
                                    NatureOperation=item.NatureOperation,
                                    GetColor=item.GetColor,
                                    GetEtapDossier=item.GetEtapDossier()[0],
                                    Client=item.GetClient,
                                    Benefi=item.GetFournisseur
                                });

                            }
                            catch (Exception e)
                            { }
                        //REF
                        foreach (var d in db.GetDossiers.OrderByDescending(d => d.EtapesDosier).Where(d => d.ReferenceExterneId > 0 && d.Client.Id == clientId && d.DFX6FP6BEAC == 3).GroupBy(d => d.ReferenceExterneId))
                            try
                            {
                                item = d.FirstOrDefault();
                                //etat = item.EtapesDosier == null ? 1000 : item.EtapesDosier;
                                DatasUnique.Add(etat, new InfoDocAcueil3()
                                {
                                    EtapeDossier = item.EtapesDosier,
                                    Nbr = 1,
                                    IsEtapeComplete = estComplet,
                                    DFX6FP6BEAC = item.DFX6FP6BEAC,
                                    EstConformite = item.EstPasséConformite ? true : false,
                                    Apurement = item.Apurement,
                                    Devise = item.DeviseToString,
                                    DfxId = item.DfxId,
                                    IsRejeter = item.IsRejete,
                                    IsTraite = item.Traité,
                                    RefId = item.ReferenceExterneId,
                                    Delai = item.GetDelai,
                                    Fournisseur = item.GetFournisseur,
                                    MontantString = item.MontantString,
                                    NatureOperation = item.NatureOperation,
                                    GetColor = item.GetColor,
                                    GetEtapDossier = item.GetEtapDossier()[0],
                                    Client = item.GetClient,
                                    Benefi = item.GetFournisseur
                                });
                            }
                            catch (Exception e)
                            { }

                    }
                    else
                        dossiers = db.GetDossiers.Where(d => d.Client.Id == clientId).ToList();
                    this.TotalDossiers = dossiers.Count;
                    this.BanqueClients.AddRange((_User as CompteClient).Client.Banques);
                    this.Dossiers.AddRange((_User as CompteClient).Client.Dossiers.Where(d => d.EtapesDosier <= 22));
                    this.Fournisseurs.AddRange((_User as CompteClient).Client.Fournisseurs);
                    var banques = (_User as CompteClient).Client.Banques.ToList();
                    this.Gestionnaires = new List<GestionnaireTmp>();
                    foreach (var item in banques)
                    {
                        this.Gestionnaires.Add(new GestionnaireTmp(item.Gestionnaire.NomComplet, item.Gestionnaire.Tel1, item.Gestionnaire.Email, item.Site.Nom));
                    }
                }
                else
                {
                    if (apurement)
                    {
                        try
                        {
                            foreach (var item in db.GetReferences.Where(d => d.DFX6FP6BEAC != 2))
                            {
                                DatasUnique.Add(etat, new InfoDocAcueil3()
                                {
                                    EtapeDossier = item.EtapesDosier,
                                    Nbr = 1,
                                    IsEtapeComplete = item.IsEtapeComplete,
                                    DFX6FP6BEAC = item.DFX6FP6BEAC,
                                    EstConformite = item.EstPasséConformite ? true : false,
                                    Apurement = item.Apurement,
                                    Devise = item.DeviseToString,
                                    DfxId = item.DFX6FP6BEAC==1?1:0,
                                    IsRejeter = item.IsRejete,
                                    IsTraite = item.Traité,
                                    RefId = item.DFX6FP6BEAC != 1 ? 1 : 0,
                                    Delai = item.GetDelai,
                                    Fournisseur = item.GetFournisseur,
                                    MontantString = item.MontantString,
                                    Statut=item.GetStatusString(),
                                    GetColor = item.GetColor,
                                    GetEtapDossier = item.GetEtapDossier()[0],
                                    Client = item.GetClient,
                                    Benefi = item.GetFournisseur,
                                    NombreDossiers=item.NbrDossiers

                                });
                            }
                        }
                        catch (Exception e)
                        { }
                    }
                    else
                        dossiers = db.GetDossiers.ToList();
                }
            }
            catch (Exception)
            { }
            try
            {
                var count = dossiers!=null?dossiers.Count:0;
                if (apurement && false)
                {
                    
                    foreach (var item in references)
                    {
                        try
                        {
                            DatasUnique.Add(etat, new InfoDocAcueil3()
                            {
                                EtapeDossier = item.EtapesDosier,
                                Nbr = 1,
                                IsEtapeComplete = item.IsEtapeComplete,
                                DFX6FP6BEAC = item.DFX6FP6BEAC,
                                EstConformite = item.EstPasséConformite ? true : false,
                                Apurement = item.Apurement,
                                Devise = item.DeviseToString,
                                DfxId = item.DFX6FP6BEAC == 1 ? 1 : 0,
                                IsRejeter = item.IsRejete,
                                IsTraite = item.Traité,
                                RefId = item.DFX6FP6BEAC != 1 ? 1 : 0,
                            });
                        }
                        catch (Exception e)
                        { }
                    }
                
                }
                else
                    foreach (var item in dossiers)
                    {
                        try
                        {
                            DatasUnique.Add(etat, new InfoDocAcueil3()
                            {
                                EtapeDossier = item.EtapesDosier,
                                Nbr = 1,
                                IsEtapeComplete = item.EstComplet,
                                DFX6FP6BEAC = item.DFX6FP6BEAC,
                                EstConformite = item.EstPasséConformite ? true : false,
                                Apurement = item.Apurement,
                                Devise = item.DeviseToString,
                                DfxId = item.DFX6FP6BEAC == 1 ? 1 : 0,
                                IsRejeter = item.IsRejete,
                                IsTraite = item.Traité,
                                RefId = item.DFX6FP6BEAC != 1 ? 1 : 0,
                                Delai = item.GetDelai,
                                Fournisseur = item.GetFournisseur,
                                MontantString = item.MontantString,
                                NatureOperation = item.NatureOperation,
                                GetColor = item.GetColor,
                                GetEtapDossier = item.GetEtapDossier()[0],
                                Client = item.GetClient,
                                Benefi = item.GetFournisseur
                            });
                        }
                        catch (Exception e)
                        { }
                    }
            
            }
            catch (Exception)
            { }

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
                if (u.BanqueId(db) == banqueId)
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
                if (u.BanqueId(db) == banqueId)
                {
                    directionMetiers.Add(u);
                }
            }
            dd = null;
            return directionMetiers;
        }

        internal static List<CompteBanqueCommerciale> GetUsersByBanque(int banqueId, ApplicationDbContext db,int idStricture, string estAdmin)
        {
            List<CompteBanqueCommerciale> users = new List<CompteBanqueCommerciale>();
            foreach (var u in db.GetCompteBanqueCommerciales.ToList())
            {
                try
                {
                    if (u.Structure.BanqueId(db) == banqueId)
                    {
                        if (estAdmin == "Admin")
                            users.Add(u);
                        else
                        {
                            if (u.IdStructure == idStricture)
                                users.Add(u);
                        }
                    }
                }
                catch (Exception)
                {}
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
                Image image = Image.FromFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "images", "IU", imageName));
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
                //var count = dossiers.Count;
                //foreach (var item in dossiers)
                //{
                //    try
                //    {
                //        switch (item.StatutDossier.StatutDossier.EtatDossier)
                //        {
                //            case EtatDossier.Encours:
                //                if (!doc.Keys.Contains(item.StatutDossier.StatutDossier.EtatDossier))
                //                {
                //                    doc[EtatDossier.Encours] = new InfoDocAcueil()
                //                    {
                //                        Date = item.DateCreationApp,
                //                        Nbr = 1,
                //                        EtatDossier = item.StatutDossier.StatutDossier.EtatDossier,
                //                        Percentage = 1 * 100 / count
                //                    };
                //                }
                //                else
                //                {
                //                    doc[EtatDossier.Encours].SetDurée(item.DateCreationApp);
                //                    doc[EtatDossier.Encours].Nbr++;
                //                    doc[EtatDossier.Encours].Percentage = doc[EtatDossier.Encours].Nbr * 100 / count;
                //                }
                //                break;
                //            case EtatDossier.Soumis:
                //                if (!doc.Keys.Contains(item.StatutDossier.StatutDossier.EtatDossier))
                //                {
                //                    doc[EtatDossier.Soumis] = new InfoDocAcueil()
                //                    {
                //                        Date = item.DateCreationApp,
                //                        Nbr = 1,
                //                        EtatDossier = item.StatutDossier.StatutDossier.EtatDossier,
                //                        Percentage = 1 * 100 / count
                //                    };
                //                }
                //                else
                //                {
                //                    doc[EtatDossier.Encours].SetDurée(item.DateCreationApp);
                //                    doc[EtatDossier.Encours].Nbr++;
                //                    doc[EtatDossier.Encours].Percentage = doc[EtatDossier.Encours].Nbr * 100 / count;
                //                }
                //                break;
                //            case EtatDossier.Apuré:
                //                if (!doc.Keys.Contains(item.StatutDossier.StatutDossier.EtatDossier))
                //                {
                //                    doc[item.StatutDossier.StatutDossier.EtatDossier] = new InfoDocAcueil()
                //                    {
                //                        Date = item.DateCreationApp,
                //                        Nbr = 1,
                //                        EtatDossier = item.StatutDossier.StatutDossier.EtatDossier,
                //                        Percentage = 1 * 100 / count
                //                    };
                //                }
                //                else
                //                {
                //                    doc[item.StatutDossier.StatutDossier.EtatDossier].SetDurée(item.DateCreationApp);
                //                    doc[item.StatutDossier.StatutDossier.EtatDossier].Nbr++;
                //                    doc[item.StatutDossier.StatutDossier.EtatDossier].Percentage = doc[EtatDossier.Encours].Nbr * 100 / count;
                //                }
                //                break;
                //            case EtatDossier.AApurer:
                //                if (!doc.Keys.Contains(item.StatutDossier.StatutDossier.EtatDossier))
                //                {
                //                    doc[item.StatutDossier.StatutDossier.EtatDossier] = new InfoDocAcueil()
                //                    {
                //                        Date = item.DateCreationApp,
                //                        Nbr = 1,
                //                        EtatDossier = item.StatutDossier.StatutDossier.EtatDossier,
                //                        Percentage = 1 * 100 / count
                //                    };
                //                }
                //                else
                //                {
                //                    doc[item.StatutDossier.StatutDossier.EtatDossier].SetDurée(item.DateCreationApp);
                //                    doc[item.StatutDossier.StatutDossier.EtatDossier].Nbr++;
                //                    doc[item.StatutDossier.StatutDossier.EtatDossier].Percentage = doc[EtatDossier.Encours].Nbr * 100 / count;
                //                }
                //                break;
                //            case EtatDossier.Echus:
                //                if (!doc.Keys.Contains(item.StatutDossier.StatutDossier.EtatDossier))
                //                {
                //                    doc[item.StatutDossier.StatutDossier.EtatDossier] = new InfoDocAcueil()
                //                    {
                //                        Date = item.DateCreationApp,
                //                        Nbr = 1,
                //                        EtatDossier = item.StatutDossier.StatutDossier.EtatDossier,
                //                        Percentage = 1 * 100 / count
                //                    };
                //                }
                //                else
                //                {
                //                    doc[item.StatutDossier.StatutDossier.EtatDossier].SetDurée(item.DateCreationApp);
                //                    doc[item.StatutDossier.StatutDossier.EtatDossier].Nbr++;
                //                    doc[item.StatutDossier.StatutDossier.EtatDossier].Percentage = doc[EtatDossier.Encours].Nbr * 100 / count;
                //                }
                //                break;
                //            case EtatDossier.Archivé:
                //                if (!doc.Keys.Contains(item.StatutDossier.StatutDossier.EtatDossier))
                //                {
                //                    doc[item.StatutDossier.StatutDossier.EtatDossier] = new InfoDocAcueil()
                //                    {
                //                        Date = item.DateCreationApp,
                //                        Nbr = 1,
                //                        EtatDossier = item.StatutDossier.StatutDossier.EtatDossier,
                //                        Percentage = 1 * 100 / count
                //                    };
                //                }
                //                else
                //                {
                //                    doc[item.StatutDossier.StatutDossier.EtatDossier].SetDurée(item.DateCreationApp);
                //                    doc[item.StatutDossier.StatutDossier.EtatDossier].Nbr++;
                //                    doc[item.StatutDossier.StatutDossier.EtatDossier].Percentage = doc[EtatDossier.Encours].Nbr * 100 / count;
                //                }
                //                break;
                //            case EtatDossier.Brouillon:
                //                if (!doc.Keys.Contains(item.StatutDossier.StatutDossier.EtatDossier))
                //                {
                //                    doc[item.StatutDossier.StatutDossier.EtatDossier] = new InfoDocAcueil()
                //                    {
                //                        Date = item.DateCreationApp,
                //                        Nbr = 1,
                //                        EtatDossier = item.StatutDossier.StatutDossier.EtatDossier,
                //                        Percentage = 1 * 100 / count
                //                    };
                //                }
                //                else
                //                {
                //                    doc[item.StatutDossier.StatutDossier.EtatDossier].SetDurée(item.DateCreationApp);
                //                    doc[item.StatutDossier.StatutDossier.EtatDossier].Nbr++;
                //                    doc[item.StatutDossier.StatutDossier.EtatDossier].Percentage = doc[EtatDossier.Encours].Nbr * 100 / count;
                //                }
                //                break;
                //            default:
                //                break;
                //        }
                //    }
                //    catch (Exception e)
                //    { }
                //}
            }
            catch (Exception)
            { }

            return doc;
        }

        public IEnumerable<Dossier> GetDossiers(bool estApurement=false)
        {
            if (dossiers != null)
            {
                try
                {
                    var clientId = (_User as CompteClient).ClientId;
                    if(!estApurement)
                        dossiers = db.GetDossiers.Where(d => d.Client.Id == clientId && d.EtapesDosier!=26 && d.EtapesDosier!=27).OrderByDescending(d=>d.DateCreationApp).ToList();
                        //dossiers = db.GetDossiers.Where(d => d.Client.Id == clientId && d.EtapesDosier>=-1 && d.EtapesDosier<22).ToList();
                    else
                        dossiers = db.GetDossiers.Where(d => d.Client.Id == clientId &&( d.EtapesDosier== -230 ||d.EtapesDosier== -231 || d.EtapesDosier== -232 || d.EtapesDosier== -250 || d.EtapesDosier== 250 || d.EtapesDosier== 232 || d.EtapesDosier== 231 || d.EtapesDosier== 230 || d.EtapesDosier== 23 || d.EtapesDosier== 24)).OrderByDescending(d => d.DateCreationApp).ToList();
                }
                catch (Exception e)
                { }
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
                        foreach (var s in bk.DirectionMetiers)
                            if (s is Agence)
                                agences.AddRange(s.Agences);
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

        public IEnumerable<Dossier> GetDossiersStructure(int? etat = null, int? structureId = null)
        {
            List<Dossier> dossiers = null;
            if (_User is CompteBanqueCommerciale)
            {
                dossiers = new List<Dossier>();

                if (structureId != null)
                {
                    var bk = db.Structures.Include("GetDossiers").FirstOrDefault(d => d.Id == structureId);
                    try
                    {
                        bk.GetDossiers.ToList().ForEach(d => {
                            if (d.Dossier.EtapesDosier == etat)
                            {
                                dossiers.Add(d.Dossier);
                            }
                        });
                    }
                    catch (Exception e)
                    { }
                }
                else
                {
                    foreach (var bk in db.Structures.Include("GetDossiers"))
                    {
                        try
                        {
                            bk.GetDossiers.ToList().ForEach(d => {
                                if (d.Dossier.EtapesDosier == etat)
                                {
                                    dossiers.Add(d.Dossier);
                                }
                            });
                        }
                        catch (Exception e)
                        { }
                    }
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
            { }

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="site"></param>
        /// <param name="role"></param>
        /// <param name="banqueID"></param>
        /// <param name="agentId"></param>
        /// <param name="dfx_ref">0=tous; 1=dfx; 2=refinancement</param>
        /// <returns></returns>
        public static List<Dossier> UserBanqueDossiers(ApplicationDbContext db,List<Dossier> dossiers, Structure site, XtraRole role, string agentId,double montantDfx=0, int banqueID = 0, byte dfx_ref=0, bool estBackoffice = false)
        {
            List<Dossier> dd = new List<Dossier>();
            bool verifié = false, rentre=false;
            //var ddg = db.GetDossiers.ToList();
            //db.GetDossiers.Include(d => d.Site).Include(d => d.DeviseMonetaire).Include(d => d.ReferenceExterne).Where(d =>
            dossiers.Where(d =>(d.EtapesDosier >= site.NiveauDossier && d.EtapesDosier <= site.NiveauMaxDossier || d.EtapesDosier <0 ||( d.EtapesDosier >= 15))
                           && d.EtapesDosier != 26 && d.EtapesDosier != 27)
                          .ToList().ForEach(d =>
                                    {
                                        if (dfx_ref == 0) rentre = true;
                                        else if (dfx_ref == 1 && d.MontantCV<=montantDfx) rentre = true;
                                        else if (dfx_ref == 2 && d.MontantCV>montantDfx) rentre = true;

                                        if (rentre)
                                        {
                                            ///Provisoir
                                            try
                                            {
                                                verifié = false;
                                                var name = site.GetType().Name;
                                                if (site is DirectionMetier)
                                                {
                                                    if (d.Site.IdDirectionMetier == site.Id)
                                                        verifié = true;
                                                }
                                                else if (!(site is Agence))
                                                    verifié = true;

                                                if (d.IdSite == site.Id || d.IdSite == banqueID || verifié) // site
                                                {
                                                    if (d.Site.BanqueId(db) == banqueID)/*Banque permissions*/
                                                    {
                                                        //if (!site.VoirDossiersAutres && site.IdResponsable != agentId)//permission structure
                                                        if (site.IdResponsable != agentId)
                                                        {
                                                            if (d.IdSite == site.Id || site.VoirDossiersAutres)//permission structure
                                                            {
                                                                if (role.VoirDossiersAutres || d.GestionnaireId == agentId || d.IdAgentResponsableDossier == agentId || (estBackoffice && (d.EtapesDosier == 4 || d.EtapesDosier == 5)) )//permission de role
                                                                {
                                                                    dd.Add(d);
                                                                }
                                                            }
                                                            else if (verifié)
                                                            {
                                                                if (d.IdAgentResponsableDossier == agentId || (estBackoffice && (d.EtapesDosier == 4 || d.EtapesDosier == 5)))
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
                                            { }

                                        }
                                    });

            role = null;
            site = null;

            return dd;
        }

        public static List<ReferenceBanque> UserBanqueReferences(ApplicationDbContext db, Structure site, XtraRole role, int banqueID, string agentId)
        {
            List<Dossier> dd = new List<Dossier>();
            List<ReferenceBanque> rr = new List<ReferenceBanque>();

            if (!site.LireTouteReference)
            {
                db.GetDossiers.Include("Site").Include("DeviseMonetaire").Include("ReferenceExterne").Where(d =>
                   /*Dossier permissions*/d.EtapesDosier != 13
                   /*Role permissions*/ // && d.EtapesDosier >= role.NiveauDossier
                   /*Structure permisions*/ //&& d.EtapesDosier >= site.NiveauDossier && d.EtapesDosier <= site.NiveauMaxDossier && (d.IdSite == site.Id || d.IdSite == banqueID)
                   /*Structure permisions*/ && d.EtapesDosier >= site.NiveauDossier && (d.IdSite == site.Id || d.IdSite == banqueID)
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
                                                                   {if(d.ReferenceExterne!=null)
                                                                       rr.Add(d.ReferenceExterne);
                                                                   }
                                                                   else
                                                                   {
                                                                       if (rr.FirstOrDefault(r => r.Id == d.ReferenceExterneId) == null)
                                                                           if(d.ReferenceExterne!=null)
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
                // rr = db.GetReferenceBanques.Include("Dossiers").Include("Banque").ToList();
                rr = db.GetReferenceBanques.Where(r => !r.Apuré && !r.Echus && r.BanqueId == banqueID).ToList();

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
        public IDictionary<int, InfoDocAcueil> infoDocBanque(CompteBanqueCommerciale userOfData,List<Dossier> _dossiers, int? etapAttachementRef, int? id = null, bool getDossiersList = false, int? getDossiersListNum = null)
        {
            if (userOfData == null) return null;

            Dictionary<int, InfoDocAcueil> doc = new Dictionary<int, InfoDocAcueil>();
            Client client = null;
            var agenceId = userOfData.IdStructure;
            var roleID = userOfData.IdXRole;
            var banqueId = userOfData.Structure.BanqueId(db);
            var role = db.XtraRoles.Find(roleID);

            try
            {
                if (id != null)
                {
                    client = db.GetClients.Find(id);
                }

                if (client != null)
                {
                    dossiers = db.GetDossiers.Where(d => d.IdSite == agenceId && d.EtapesDosier > 1 && d.ClientId == id).ToList();
                    this.TotalDossiers = dossiers.Count;
                    userOfData.Structure.GetDossiers.ToList().ForEach(d => {
                        if (d.Dossier.EtapesDosier > 0)
                        {
                            this.Dossiers.Add(d.Dossier);
                        }
                    });
                    //this.Dossiers.AddRange(userOfData.Structure.Dossiers.Where(d => d.EtapesDosier > 0));
                    this.Clients.AddRange((from c in userOfData.Structure.Clients select c.Client).ToList());
                }
                else
                {
                    if (!(_User as CompteBanqueCommerciale).EstAdmin)
                    {
                        dossiers = new List<Dossier>();
                        var estAgence = userOfData.Structure.EstAgence;

                        #region Provisoire
                        var roleId = userOfData.IdXRole;
                        var site = db.Structures.Find(agenceId);
                        //dossiers = UserBanqueDossiers(db, site, role, banqueId, _User.Id);
                        dossiers = UserBanqueDossiers(db,_dossiers, site: site, role: role, banqueID: banqueId,agentId: userOfData.Id, montantDfx: 50000000, dfx_ref: 0) ;
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
                if (getDossiersList)
                    Dossiers = new List<Dossier>();
                //if (etapAttachementRef<6)
                {
                    var count = dossiers.Count;
                    int traite = 0, traite_dfx = 0, traite_ref = 0,
                    rejet = 0, rejet_dfx = 0, rejet_ref = 0, encours = 0, dfx = 0, dfx_recu = 0,
                    refinancement = 0, refinancement_recu = 0, accorde = 0, accorDfx = 0, accordRef = 0;
                    foreach (var item in dossiers)
                    {
                        try
                        {
                            if (getDossiersList)
                                if (item.NumeroDossier >= getDossiersListNum)
                                    Dossiers.Add(item);
                            if (/*(int)item.EtapesDosier < etapAttachementRef ||*/ item.Montant <= 50000000)
                            {
                                if (!doc.Keys.Contains((int)item.EtapesDosier))
                                {
                                    if (item.EtapesDosier == 4 || item.EtapesDosier == 6)
                                        traite = 1;
                                    else if (item.EtapesDosier < 0)
                                        rejet = 1;
                                    else encours = 1;

                                    if (item.Montant <= 50000000)
                                    {
                                        if (item.EtapesDosier == 5)
                                            dfx_recu = 1;
                                        else if (item.EtapesDosier == 9)
                                            traite_dfx = 1;
                                        else if (item.EtapesDosier == 10)
                                        {
                                            accorDfx = 1;
                                            accorde = 1;
                                        }
                                        else if (item.EtapesDosier < 0)
                                            rejet_dfx = 1;
                                        dfx = 1;
                                    }
                                    else
                                    {
                                        if (item.EtapesDosier == 5) refinancement_recu = 1;
                                        else if (item.EtapesDosier == 9)
                                        {
                                            traite_ref = 1;
                                        }
                                        else if (item.EtapesDosier == 10)
                                        {
                                            accordRef = 1;
                                            accorde = 1;
                                        }
                                        else if (item.EtapesDosier < 0)
                                        {
                                            rejet_ref = 1;
                                            rejet = 1;
                                        }
                                        refinancement = 1;
                                    }

                                    doc[(int)item.EtapesDosier] = new InfoDocAcueil()
                                    {
                                        Date = item.DateCreationApp,
                                        Nbr = 1,
                                        NbrRejets = rejet,
                                        NbrRejets_DFX = rejet_dfx,
                                        NbrRejets_Refinancement = rejet_ref,
                                        NbrValidés = traite,
                                        NbrValidés_DFX = traite_dfx,
                                        NbrValidés_Refinancement = traite_ref,
                                        EtapeDossier = (int)item.EtapesDosier,
                                        NbrDFX = dfx,
                                        NbrDFX_recu = dfx_recu,
                                        NbrRefinanacement = refinancement,
                                        NbrRefinanacement_recu = refinancement_recu,
                                        NbrAccordés = accorde,
                                        NbrAccordés_DFX = accorDfx,
                                        NbrAccordéé_Refinancement = accordRef,
                                        Percentage = 1// 1 * 100 / count
                                    };

                                    //doc.Add((int)item.EtapesDosier, new InfoDocAcueil()
                                    //{
                                    //    Date = item.DateCreationApp,
                                    //    Nbr = 1,
                                    //    EtapeDossier = (int)item.EtapesDosier,
                                    //    Percentage = 1 * 100 / count
                                    //});
                                }
                                else
                                {
                                    doc[(int)item.EtapesDosier].SetDurée(item.DateCreationApp.Value);
                                    doc[(int)item.EtapesDosier].Nbr++;
                                    if (item.EtapesDosier < 0)
                                        doc[(int)item.EtapesDosier].NbrRejets++;
                                    else if (item.EtapesDosier == 4 || item.EtapesDosier == 6)
                                        doc[(int)item.EtapesDosier].NbrValidés++;
                                    else doc[(int)item.EtapesDosier].NbrEncours++;

                                    if (item.Montant <= 50000000)
                                    {
                                        if (item.EtapesDosier == 5)
                                            doc[(int)item.EtapesDosier].NbrDFX_recu++;
                                        else if (item.EtapesDosier == 9)
                                            doc[(int)item.EtapesDosier].NbrValidés_DFX++;
                                        else if (item.EtapesDosier == 10)
                                        {
                                            doc[(int)item.EtapesDosier].NbrAccordés_DFX++;
                                            doc[(int)item.EtapesDosier].NbrAccordés++;
                                        }
                                        else if (item.EtapesDosier < 0)
                                            doc[(int)item.EtapesDosier].NbrRejets_DFX++;
                                        doc[(int)item.EtapesDosier].NbrDFX++;
                                    }
                                    else if (item.ReferenceExterneId == null)
                                    {
                                        if (item.EtapesDosier == 5)
                                            doc[(int)item.EtapesDosier].NbrRefinanacement_recu++;
                                        else if (item.EtapesDosier == 9)
                                            doc[(int)item.EtapesDosier].NbrValidés_Refinancement++;
                                        else if (item.EtapesDosier == 10)
                                        {
                                            doc[(int)item.EtapesDosier].NbrAccordéé_Refinancement++;
                                            doc[(int)item.EtapesDosier].NbrAccordés++;
                                        }
                                        else if (item.EtapesDosier < 0)
                                            doc[(int)item.EtapesDosier].NbrRejets_Refinancement++;
                                        doc[(int)item.EtapesDosier].NbrRefinanacement++;
                                    }
                                    doc[(int)item.EtapesDosier].Percentage = doc[(int)item.EtapesDosier].Nbr * 100 / count;

                                    doc[(int)item.EtapesDosier].SetDurée(item.DateCreationApp);
                                    //doc[(int)item.EtapesDosier].Nbr++;
                                    //doc[(int)item.EtapesDosier].Percentage += doc[2].Nbr * 100 / count;
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
                            if ((int)item.Montant > 50000000 && (int)item.EtapesDosier >= etapAttachementRef)
                            {
                                if (!doc.Keys.Contains((int)item.EtapesDosier))
                                {
                                    doc.Add((int)item.EtapesDosier, new InfoDocAcueil()
                                    {
                                        Date = DateTime.Now,// (DateTime)item.DateCredit,
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
        public void infoDocBanque2(CompteBanqueCommerciale userOfData,List<Dossier> _dossiers,Banque banque, int? id = null)
        {

            Datas = new Dictionary<int?, InfoDocAcueil2>();
            DossiersEtat = new List<InfoDocAcueil2>();
            DatasDFX = new Dictionary<int?, InfoDocAcueil2>();
            Client client = null;
            var agenceId = userOfData.IdStructure;
            var roleID = userOfData.IdXRole;
            var banqueId = banque.Id;
            var role = db.XtraRoles.Find(roleID);
            var structure = db.Structures.Find(agenceId);

            try
            {
                if (id != null)
                {
                    client = db.GetClients.Find(id);
                }

                if (client != null)
                {
                    dossiers = db.GetDossiers.Where(d => d.IdSite == agenceId && d.EtapesDosier > 1 && d.ClientId == id).ToList();
                    this.TotalDossiers = dossiers.Count;
                    userOfData.Structure.GetDossiers.ToList().ForEach(d => {
                        if (d.Dossier.EtapesDosier > 0)
                        {
                            this.Dossiers.Add(d.Dossier);
                        }
                    });
                    //this.Dossiers.AddRange(userOfData.Structure.Dossiers.Where(d => d.EtapesDosier > 0));
                    this.Clients.AddRange((from c in userOfData.Structure.Clients select c.Client).ToList());
                }
                else
                {
                    if (!(_User as CompteBanqueCommerciale).EstAdmin)
                    {
                        dossiers = new List<Dossier>();
                        var estAgence = structure.EstAgence;

                        #region Provisoire
                        var roleId = userOfData.IdXRole;
                        var site = db.Structures.Find(agenceId);
                        //dossiers = UserBanqueDossiers(db, site, role, banqueId, _User.Id);
                        dossiers = UserBanqueDossiers(db,_dossiers, site: site, role: role, banqueID: banqueId,agentId: userOfData.Id, montantDfx: banque.MontantDFX,dfx_ref: 0, estBackoffice: userOfData.EstBackOff) ;
                        try
                        {
                            ////Dossiers BEAC
                            //this.Dossiers = new List<Dossier>();
                            //this.Dossiers.AddRange(dossiers.Where(d => d.EtapesDosier >= 15 && d.EtapesDosier <= 18 || d.EtapesDosier == -230 || d.EtapesDosier == -250 
                            //|| d.EtapesDosier == 230|| d.EtapesDosier == 250|| d.EtapesDosier == 22|| d.EtapesDosier == 23 && (d.EtapesDosier !=24  &&d.EtapesDosier !=27 )));
                        }
                        catch (Exception)
                        {}
                        #endregion
                    }
                    else
                    {
                        //dossiers = db.GetDossiers.Include("DomicilImport").Include("DeclarImport").Where(d => d.Site.BanqueId(db) == banqueId && d.EtapesDosier > 0).ToList();
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
                    int? eta = 0;
                    //foreach (var item in dossiers.Where(d=>d.ReferenceExterneId==null))
                    foreach (var item in dossiers)
                    {
                        try
                        {
                            this.Dossiers.Add(item);
                            eta = item.EtapesDosier;//>=0?(int)item.EtapesDosier:Math.Abs((int)item.EtapesDosier)*10+item.EtapePrecedenteDosier;

                            if (item.DeviseToString.ToLower()=="usd")
                            {
                                DossiersEtat.Add(new InfoDocAcueil2()
                                {
                                    DateNouvelleEtape = item.DateNouvelleEtape,
                                    EtapeDossier = item.EtapesDosier,
                                    EtapAvantRejet = eta,
                                    DFX6FP6BEAC=item.DFX6FP6BEAC,
                                    DfxId = item.DfxId,
                                    NbrUSD = 1,
                                    EstConformite=item.EstPasséConformite,
                                });
                            }
                            else if (item.DeviseToString.ToLower()=="eur")
                            {
                                DossiersEtat.Add(new InfoDocAcueil2()
                                {
                                    DateNouvelleEtape = item.DateNouvelleEtape,
                                    EtapeDossier = item.EtapesDosier,
                                    EtapAvantRejet = eta,
                                    DFX6FP6BEAC = item.DFX6FP6BEAC,
                                    DfxId = item.DfxId,
                                    NbrEUR = 1,
                                    EstConformite = item.EstPasséConformite,
                                });
                            }
                            else
                            {
                                DossiersEtat.Add(new InfoDocAcueil2()
                                {
                                    DateNouvelleEtape = item.DateNouvelleEtape,
                                    EtapeDossier = item.EtapesDosier,
                                    EtapAvantRejet = eta,
                                    DFX6FP6BEAC = item.DFX6FP6BEAC,
                                    DfxId = item.DfxId,
                                    NbrAutreDevise = 1,
                                    EstConformite = item.EstPasséConformite,
                                });
                            }

                            if (item.MontantCV <= banque.MontantDFX)
                            {
                                if (!DatasDFX.Keys.Contains(eta))
                                {
                                    DatasDFX.Add(eta, new InfoDocAcueil2()
                                    {
                                        DateNouvelleEtape = item.DateNouvelleEtape,
                                        EtapeDossier = item.EtapesDosier,
                                        DfxId = item.DfxId,
                                        EtapAvantRejet = eta,
                                        DFX6FP6BEAC = item.DFX6FP6BEAC,
                                        Nbr = 1,
                                    });
                                }
                                else
                                {
                                    DatasDFX[eta].DFX6FP6BEAC = item.DFX6FP6BEAC;
                                    DatasDFX[eta].Nbr++;
                                    if (DatasDFX[eta].DateNouvelleEtape > item.DateNouvelleEtape)
                                    {
                                        DatasDFX[eta].DateNouvelleEtape = item.DateNouvelleEtape;
                                    }
                                }
                            }
                            else
                            {
                                if (!Datas.Keys.Contains(eta))
                                {
                                    Datas.Add(eta, new InfoDocAcueil2()
                                    {
                                        DateNouvelleEtape = item.DateNouvelleEtape,
                                        EtapeDossier = item.EtapesDosier,
                                        EtapAvantRejet = eta,
                                        DFX6FP6BEAC = item.DFX6FP6BEAC,
                                        Nbr = 1,
                                    });
                                }
                                else
                                {
                                    Datas[item.EtapesDosier].DFX6FP6BEAC = item.DFX6FP6BEAC;
                                    Datas[item.EtapesDosier].Nbr++;
                                    if (Datas[item.EtapesDosier].DateNouvelleEtape > item.DateNouvelleEtape)
                                    {
                                        Datas[item.EtapesDosier].DateNouvelleEtape = item.DateNouvelleEtape;
                                    }
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
                if (false)
                {
                    //referenceBanques = db.GetReferenceBanques.Where(r => !r.Apuré && !r.Echus && r.BanqueId == banqueId).ToList();
                    referenceBanques = (from d in dossiers select d.ReferenceExterne).ToList();
                    var count = referenceBanques.Count;
                    int? eta = 0;
                    foreach (ReferenceBanque item in referenceBanques)
                    {
                        try
                        {
                            var doss = item.Dossiers.FirstOrDefault();
                            eta = doss.EtapesDosier;
                            if (doss.DeviseToString.ToLower() == "usd")
                            {
                                if (!Datas.Keys.Contains(eta))
                                {
                                    Datas.Add(eta, new InfoDocAcueil2()
                                    {
                                        DateNouvelleEtape = doss.DateNouvelleEtape,
                                        EtapeDossier = doss.EtapesDosier,
                                        EtapAvantRejet = eta,
                                        DFX6FP6BEAC = item.DFX6FP6BEAC,
                                        Nbr = 1,
                                        NbrUSD = 1
                                    }); ;
                                }
                                else
                                {
                                    DatasDFX[eta].NbrUSD++;
                                }
                            }
                            else if (item.DeviseToString.ToLower() == "eur")
                            {
                                if (!Datas.Keys.Contains(eta))
                                {
                                    Datas.Add(eta, new InfoDocAcueil2()
                                    {
                                        DateNouvelleEtape = doss.DateNouvelleEtape,
                                        EtapeDossier = doss.EtapesDosier,
                                        EtapAvantRejet = eta,
                                        Nbr = 1,
                                        NbrEUR = 1,
                                        DFX6FP6BEAC = item.DFX6FP6BEAC
                                    }); ;
                                }
                                else
                                {
                                    DatasDFX[eta].NbrEUR++;
                                }
                            }
                            else
                            {
                                if (!Datas.Keys.Contains(eta))
                                {
                                    Datas.Add(eta, new InfoDocAcueil2()
                                    {
                                        DateNouvelleEtape = doss.DateNouvelleEtape,
                                        EtapeDossier = doss.EtapesDosier,
                                        EtapAvantRejet = eta,
                                        Nbr = 1,
                                        NbrAutreDevise = 1,
                                        DFX6FP6BEAC = item.DFX6FP6BEAC
                                    }); ;
                                }
                                else
                                {
                                    DatasDFX[eta].NbrAutreDevise++;
                                }
                            }

                            if (doss!=null)
                            {
                                if (!Datas.Keys.Contains((int)doss.EtapesDosier))
                                {
                                    Datas.Add(doss.EtapesDosier, new InfoDocAcueil2()
                                    {
                                        DateNouvelleEtape = doss.DateNouvelleEtape,
                                        EtapeDossier = doss.EtapesDosier,
                                        Nbr = 1,
                                        DFX6FP6BEAC = item.DFX6FP6BEAC
                                    });
                                }
                                else
                                {
                                    Datas[item.EtapesDosier].Nbr++;
                                    if (Datas[doss.EtapesDosier].DateNouvelleEtape > doss.DateNouvelleEtape)
                                    {
                                        Datas[doss.EtapesDosier].DateNouvelleEtape = doss.DateNouvelleEtape;
                                        Datas[doss.EtapesDosier].DFX6FP6BEAC = doss.DFX6FP6BEAC;
                                    }
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

        }

        public void infoDocBanque3(CompteBanqueCommerciale userOfData,List<Dossier> _dossiers,Banque banque, int? id = null,bool apurement=true)
        {

            Client client = null;
            var agenceId = userOfData.IdStructure;
            var roleID = userOfData.IdXRole;
            var banqueId = banque.Id;
            var role = db.XtraRoles.Find(roleID);
            var structure = db.Structures.Find(agenceId);

            try
            {
                if (id != null)
                {
                    client = db.GetClients.Find(id);
                }

                if (client != null)
                {
                    dossiers = db.GetDossiers.Where(d => d.IdSite == agenceId && d.EtapesDosier > 1 && d.ClientId == id).ToList();
                    this.TotalDossiers = dossiers.Count;
                    userOfData.Structure.GetDossiers.ToList().ForEach(d => {
                        if (d.Dossier.EtapesDosier > 0)
                        {
                            this.Dossiers.Add(d.Dossier);
                        }
                    });
                    this.Clients.AddRange((from c in userOfData.Structure.Clients select c.Client).ToList());
                    foreach (var d in dossiers.GroupBy(d=>d.DFX ))
                    {
                        var item = d.Single();
                        DonneeApurement.Add(new InfoDocAcueil3()
                        {
                            EtapeDossier = item.EtapesDosier,
                            Nbr = 1,
                            IsEtapeComplete = d.Key.IsEtapeComplete,
                            DFX6FP6BEAC = item.DFX6FP6BEAC,
                            EstConformite = item.EstPasséConformite ? true : false,
                            Apurement = item.Apurement,
                            Devise = item.DeviseToString,
                            DfxId = item.DfxId,
                            IsRejeter = item.IsRejete,
                            IsTraite = item.Traité,
                            RefId = item.ReferenceExterneId,
                            Delai = item.GetDelai,
                            Fournisseur = item.GetFournisseur,
                            MontantString = d.Key.MontantString,
                            Statut = item.GetStatusString(),
                            GetColor = item.GetColor,
                            GetEtapDossier = item.GetEtapDossier()[0],
                            Client = item.GetClient,
                            Benefi = item.GetFournisseur,
                            NombreDossiers = d.Key.NbrDossiers

                        });
                    }
                }
                else
                {
                    if (!(_User as CompteBanqueCommerciale).EstAdmin)
                    {
                        dossiers = new List<Dossier>();
                        var estAgence = structure.EstAgence;

                        #region Provisoire
                        var roleId = userOfData.IdXRole;
                        var site = db.Structures.Find(agenceId);
                        //dossiers = UserBanqueDossiers(db, site, role, banqueId, _User.Id);
                        dossiers = UserBanqueDossiers(db,_dossiers, site, role:role,banqueID: banqueId,agentId: userOfData.Id, montantDfx: banque.MontantDFX,dfx_ref:0,estBackoffice:userOfData.EstBackOff);
                        //DFX
                        Dossier item = null;
                        foreach (var d in dossiers.Where(d=>d.DFX!=null).OrderByDescending(d => d.EtapesDosier).GroupBy(d => d.DFX))
                        {
                            try
                            {
                                var first = d.FirstOrDefault();
                                var end = d.LastOrDefault();
                                if (end.EtapesDosier == 24 && first.EtapesDosier != 24) 
                                    item = first;
                                else
                                    item = first.EtapesDosier > end.EtapesDosier ? end : first;
                                var dd = item.GetEtapDossier()[0];
                                var str = item.GetEtapDossier()[0].Contains("[SITE]") ? item.GetEtapDossier()[0].Replace("[SITE] :", "") : item.GetEtapDossier()[0];
                                DonneeApurement.Add(new InfoDocAcueil3()
                                {
                                    EtapeDossier = item.EtapesDosier,
                                    Nbr = 1,
                                    IsEtapeComplete = d.Key.IsEtapeComplete,
                                    DFX6FP6BEAC = item.DFX6FP6BEAC,
                                    EstConformite = item.EstPasséConformite ? true : false,
                                    Apurement = item.Apurement,
                                    Devise = item.DeviseToString,
                                    DfxId = item.DfxId,
                                    IsRejeter = item.IsRejete,
                                    IsTraite = item.Traité,
                                    RefId = item.ReferenceExterneId,
                                    Delai = item.GetDelai,
                                    Fournisseur = item.GetFournisseur,
                                    MontantString = d.Key.MontantString,
                                    Statut = item.GetStatusString(),
                                    GetColor = item.GetColor,
                                    GetEtapDossier = str,
                                    Client = item.GetClient,
                                    Benefi = item.GetFournisseur,
                                    NombreDossiers = d.Key.NbrDossiers,
                                    Id=item.Dossier_Id

                                });
                            }
                            catch (Exception)
                            {}
                        }
                        //Ref
                        item = null;
                        foreach (var d in dossiers.Where(d=>d.ReferenceExterne!=null).GroupBy(d => d.ReferenceExterne))
                        {
                            var first = d.FirstOrDefault();
                            var end = d.LastOrDefault();
                            if (end.EtapesDosier == 24 && first.EtapesDosier != 24) item = first;
                            else
                                item = first.EtapesDosier > end.EtapesDosier ? end : first;
                            var dd = item.GetEtapDossier()[0];
                            var str = item.GetEtapDossier()[0].Contains("[SITE]") ? item.GetEtapDossier()[0].Replace("[SITE] :", "") : item.GetEtapDossier()[0];
                            DonneeApurement.Add(new InfoDocAcueil3()
                            {
                                EtapeDossier = item.EtapesDosier,
                                Nbr = 1,
                                IsEtapeComplete = d.Key.IsEtapeComplete,
                                DFX6FP6BEAC = item.DFX6FP6BEAC,
                                EstConformite = item.EstPasséConformite ? true : false,
                                Apurement = item.Apurement,
                                Devise = item.DeviseToString,
                                DfxId = item.DfxId,
                                IsRejeter = item.IsRejete,
                                IsTraite = item.Traité,
                                RefId = item.ReferenceExterneId,
                                Delai = item.GetDelai,
                                Fournisseur = item.GetFournisseur,
                                MontantString = d.Key.MontantString,
                                Statut = item.GetStatusString(),
                                GetColor = item.GetColor,
                                GetEtapDossier = str,
                                Client = item.GetClient,
                                Benefi = item.GetFournisseur,
                                NombreDossiers = d.Key.NbrDossiers,
                                Id=item.Dossier_Id

                            });
                        }
                        item = null;
                        //Fonds propres
                        foreach (var d in dossiers.Where(d=>d.DFX6FP6BEAC==2))
                        {
                            DonneeApurement.Add(new InfoDocAcueil3()
                            {
                                EtapeDossier = d.EtapesDosier,
                                Nbr = 1,
                                DFX6FP6BEAC = d.DFX6FP6BEAC,
                                EstConformite = d.EstPasséConformite ? true : false,
                                Apurement = d.Apurement,
                                Devise = d.DeviseToString,
                                IsRejeter = d.IsRejete,
                                IsTraite = d.Traité,
                                RefId = d.ReferenceExterneId,
                                Delai = d.GetDelai,
                                Fournisseur = d.GetFournisseur,
                                MontantString = d.MontantString,
                                Statut = d.GetStatusString(),
                                GetColor = d.GetColor,
                                GetEtapDossier = d.GetEtapDossier()[0],
                                Client = d.GetClient,
                                Benefi = d.GetFournisseur,
                                Id=d.Dossier_Id
                            });
                        }
                        item = null;
                        #endregion
                    }
                }
            }
            catch (Exception e)
            { }

        }

        #endregion

        /// <summary>
        /// Affiche les actions de modification d'etape du dossier
        /// </summary>
        /// <param name="etapeDossier">Etape du dosier que l'action doit être affichée</param>
        /// <param name="min">le niveau le bas de la structure pour intervenir du dossier</param>
        /// <param name="max">le niveau le maximul de la structure pour intervenir du dossier</param>
        /// <param name="etapeMaxi">si > etapeDossier alors affichaAction dans l'intervalle [etapeDossier,etapeMaxi] </param>
        /// <returns></returns>
        public static bool AfficheAction(string gestionnaireId, string responsableID, string precedentResponsableId,string userId,int? etapeDossier, int min, int max, int etapeMaxi = 0)
        {
            bool ecrire = false;
            try
            {
                if (gestionnaireId==userId || responsableID==userId)
                {
                    if (etapeDossier < 0) return true;
                    if (etapeDossier >= etapeMaxi)
                    {
                        if (min==1)
                        {
                            if (etapeDossier >= min && (etapeDossier <6 || etapeDossier >=19 )&& responsableID == userId)
                            {
                                return true;
                            }
                        }
                        else if (etapeDossier >= min && etapeDossier <= max)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        for (int i = (int)etapeDossier; i <= etapeMaxi; i++)
                        {
                            if (i >= min && i < max)
                            {
                                return true;
                                //break;
                            }
                        }
                    } 
                }

            }
            catch (Exception)
            { }
            return ecrire;
        }
    }

    [NotMapped]
    public class InfoDocAcueil3
    {
        public int Nbr { get; set; }
        public bool IsValide { get; set; }
        public bool IsRejeter { get; set; }
        public bool IsTraite { get; set; }
        public string Devise{ get; set; }
        public int? EtapeDossier { get; set; }
        public int? DfxId { get; set; }
        public int? RefId { get; set; }
        public bool EstConformite { get; set; }
        public byte DFX6FP6BEAC { get; set; }
        public bool IsEtapeComplete { get; set; }
        public bool Apurement { get; set; }
        public string Fournisseur { get; set; }
        public string MontantString { get; set; }
        public NatureOperation NatureOperation { get; set; }
        public double Montant { get; set; }
        public int Delai { get; set; }
        public StatutDossier Statut { get;  set; }
        public string GetColor { get; set; }
        public string GetEtapDossier { get;  set; }
        [DisplayName("Donneur d'ordre")]
        public string Client { get;  set; }
        [DisplayName("Bénéficiaire")]
        public string Benefi { get;  set; }
        public int NombreDossiers { get;  set; }
        public int Id { get;  set; }
    }

     [NotMapped]
    public class InfoDocAcueil2
    {
        public int Nbr { get; set; }
        public int NbrValides { get; set; }
        public int NbrRejeter { get; set; }
        public int NbrTraite { get; set; }
        public int NbrDFX { get; set; }
        public int NbrRef { get; set; }
        public int NbrEUR{ get; set; }
        public int NbrUSD{ get; set; }
        public int NbrAutreDevise { get; set; }
        public int NbrAnciens { get; set; }
        public int NbrAvantRejet{ get; set; }
        public int? EtapeDossier { get; set; }
        public int? DfxId { get; set; }
        public DateTime? DateNouvelleEtape { get; set; } = DateTime.Now;
        public int? EtapAvantRejet { get; internal set; }
        //public bool EstDfx{ get; set; }
        //public bool EstBEAC { get; set; }
        public bool EstConformite { get; set; }
        //public bool EstDFX { get; internal set; }
        //public bool EstFondPropre { get; internal set; }
        public byte DFX6FP6BEAC { get; set; }
        public bool IsEtapeComplete { get; internal set; }
        public bool Apurement { get; set; }

        public string GetDureeString()
        {
            if (DateNouvelleEtape != new DateTime())
            {
                var dif = (DateTime.Now - (DateTime)DateNouvelleEtape).TotalDays;
                if (dif >= 90)
                    return "Il y a 3 mois";
                else if (dif >= 60)
                    return "Il y a 2 mois";
                else if (dif >= 30)
                    return "Il y a  1 mois";
                else if (dif >= 14)
                    return "Il y a  2 semaines";
                else if (dif >= 7)
                    return "Il y a 1 semaine";
                else if (dif > 1)
                    return "Il y a " + (int)dif + " jour(s)";
                else
                    return "à l'instant";
            }
            return "";
        }
        
        public int GetDureeNbr()
        {
            if (DateNouvelleEtape != new DateTime())
            {
                var dd = DateNouvelleEtape == default ? DateTime.Now : DateNouvelleEtape;
                var dif = (DateTime.Now - (DateTime)dd).TotalDays;
                if (dif >= 90)
                    return 90;
                else if (dif >= 60)
                    return 60;
                else if (dif >= 30)
                    return 30;
                else if (dif >= 14)
                    return 14;
                else if (dif >= 7)
                    return 7;
                else if (dif > 1)
                    return (int)dif;
            }
            return 0;
        }
    }

    [NotMapped]
    public class InfoDocAcueil
    {
        public int Nbr { get; set; }
        public int EtapeDossier { get; set; }
        public int NbrRejets { get; set; }
        public int NbrRejets_DFX { get; set; }
        public int NbrRejets_Refinancement { get; set; }
        public int NbrValidés { get; set; }
        public int NbrValidés_DFX { get; set; }
        public int NbrValidés_Refinancement { get; set; }
        public int NbrEncours { get; set; }
        public int NbrDFX_recu { get; set; }
        public int NbrDFX { get; set; }
        public int NbrRefinanacement { get; set; }
        public int NbrRefinanacement_recu { get; set; }
        public int NbrAccordés { get; set; }
        public int NbrAccordéé_Refinancement { get; set; }
        public int NbrAccordés_DFX { get; set; }

        public double Percentage;
        public string Durée;
        public DateTime? Date;
        public EtatDossier EtatDossier;
        public List<Dossier> Dossiers { get; set; }

        public int GetAbsolute_Nbr(CategorieDossier categorie)
        {
            if (categorie == CategorieDossier.Refinanacement) return NbrRefinanacement;
            else if (categorie == CategorieDossier.DFX) return NbrDFX;
            else return Nbr;
        }

        internal void SetDurée(DateTime? dateCreationApp)
        {
            if (dateCreationApp > Date)
                Date = dateCreationApp;
        }

        //public DateTime GetDate()
        //{

        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeInfo">null ou 0=date; 1=rejet; 2=validé ou traité; 3=en cours;4=DFX;5=Refinancement</param>
        /// <returns></returns>
        public string GetDuree(byte? typeInfo = null)
        {
            if (typeInfo == null || typeInfo == 0)
            {
                var dif = (DateTime.Now - Date).Value.TotalDays;

                if (Date != new DateTime())
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
            else if (typeInfo == 1)
            {
                return "rejetés";
            }
            else if (typeInfo == 2)
            {
                return "validés";
            }
            else if (typeInfo == 3)
                return "En cours";
            else if (typeInfo == 4)
                return "DFX";
            else if (typeInfo == 5)
                return "Refinancement";

            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeDonnee">null =date;0=tous; 1=rejet; 2=traité;3=en cours; 4=DFX;5=Refinancement</param>
        /// <param name="etatDossier">-3: rejet DFX; -4 rejet Refinancement; 5 reçu DFX ou reçu refinancement; 9 traité DFX; 10 traité refinancement; 11 all DFX;12 all refinancement</param>
        /// <returns></returns>
        public double GetPercentage(int? typeDonnee = null, int? etatDossier = null)
        {
            if (typeDonnee == null)
                return Math.Round((double)Percentage, 2);
            if (typeDonnee == 0)
                return Nbr;
            else if (typeDonnee == 1)
            {
                if (etatDossier == -3) return NbrRejets_DFX;
                if (etatDossier == -4) return NbrRejets_Refinancement;
                return NbrRejets;
            }
            else if (typeDonnee == 2)
            {
                if (etatDossier == 9) return NbrValidés_DFX;
                if (etatDossier == 10) return NbrValidés_Refinancement;
                return NbrValidés;
            }
            else if (typeDonnee == 3) return NbrEncours;
            else if (typeDonnee == 4)
            {
                if (etatDossier == 5) return NbrDFX_recu;
                return NbrDFX;
            }
            else if (typeDonnee == 5)
            {
                if (etatDossier == 5) return NbrRefinanacement_recu;
                return NbrRefinanacement;
            }
            else if (typeDonnee == 6)
            {
                if (etatDossier == 9) return NbrAccordés_DFX;
                if (etatDossier == 10) return NbrAccordéé_Refinancement;
                return NbrAccordés;
            }
            else if (typeDonnee == 11)
            {
                return NbrDFX;
            }
            else if (typeDonnee == 12)
            {
                return NbrRefinanacement;
            }
            else return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeDonnee">null =date;0=tous; 1=rejet; 2=traité;3=en cours; 4=DFX;5=Refinancement</param>
        /// <param name="etatDossier">-3: rejet DFX; -4 rejet Refinancement; 5 reçu DFX ou reçu refinancement; 9 traité DFX; 10 traité refinancement</param>
        /// <returns></returns>
        public string GetLink(int? typeDonnee = null, int? etatDossier = null)
        {
            string st = "", filtre = "";
            if (typeDonnee == null)
                st = "#";
            if (typeDonnee == 0)
                st = "all";
            else if (typeDonnee == 1) st = "rejet";
            else if (typeDonnee == 2) st = "validé";
            else if (typeDonnee == 2) st = "encours";
            else if (typeDonnee == 4) st = "dfx";
            else if (typeDonnee == 5) st = "refinancement";
            else return "#";

            if (etatDossier == -4) filtre = "rejet_ref";
            if (etatDossier == -3) filtre = "rejet_dfx";
            if (etatDossier == 5) filtre = "recu_dfx";
            if (etatDossier == 9) filtre = "traite";

            if (st == "#") return st;
            return "~/dossiers_banque/index/st=" + st + "&fil=" + filtre;
        }
    }

    public class GestionnaireTmp
    {
        public string NomComplet { get; set; }
        public string Tel { get; set; }
        public string Email { get; set; }
        public string Banque { get; set; }

        public GestionnaireTmp(string nom, string tel, string email, string banque)
        {
            NomComplet = nom; Tel = tel; Email = email; Banque = banque;
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

    public class UserInfo
    {
        [DisplayName("Type")]
        public string Type { get; set; }
        [DisplayName("Donnée")]
        public Object Donnée { get; set; }
        public string UserId { get; set; }
        //0:add; 1:delete; 2:update
        public byte Opération { get; set; }
        public string ComponentId { get; set; } = string.Empty;
        public string ComponentType { get; set; } = string.Empty;
    }

    public enum ViewComponent
    {
        Create,
        Update,
        Delete,
        Details,
        List,
        Other
    }
}