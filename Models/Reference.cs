using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;

namespace genetrix.Models
{
    public class Reference
    {
        public Reference()
        {
            }

        public void InitData()
        {
            if (FirstDossier == null)
                FirstDossier = Dossiers.OrderByDescending(d => d.EtapesDosier).FirstOrDefault();
            if (LastDossier == null)
                LastDossier = Dossiers.OrderByDescending(d => d.EtapesDosier).LastOrDefault();
        }


        [NotMapped]
        public Dossier FirstDossier { get; set; }
        private Dossier LastDossier;

        public int Id { get; set; }

        [DisplayName("Référence")]
        public string NumeroRef { get; set; }

        public int FournisseurId
        {
            get
            {
                try
                {
                    if (Dossiers != null && Dossiers.Count > 0)
                    {
                        return (int)Dossiers.FirstOrDefault().FournisseurId;
                    }
                }
                catch (Exception)
                { }
                return 0;
            }
        }

        public int ClientId
        {
            get
            {
                try
                {
                    if (Dossiers != null && Dossiers.Count > 0)
                    {
                        return (int)Dossiers.FirstOrDefault().ClientId;
                    }
                }
                catch (Exception)
                { }
                return 0;
            }
        }
        public string Client
        {
            get
            {
                try
                {
                    if (Dossiers != null && Dossiers.Count > 0)
                    {
                        return Dossiers.FirstOrDefault().Client.Nom;
                    }
                }
                catch (Exception)
                { }
                return "";
            }
        }


        [Display(Name = "DATE DE RECEPTION")]
        [DataType(DataType.Date)]
        public DateTime? DateReception { get; set; }
        private double montant;
        [Display(Name = "MONTANT")]
        public double Montant
        {
            get
            {
                try
                {
                    montant = 0;
                    Dossiers.ToList().ForEach(d =>
                    {
                        montant += d.Montant;
                    });
                }
                catch (Exception)
                { }
                return montant;
            }
        }

        public byte DFX6FP6BEAC {
            get
            {
                try
                {
                    InitData();
                    if (LastDossier.EtapesDosier == 24 && FirstDossier.EtapesDosier != 24) return FirstDossier.DFX6FP6BEAC;
                    return FirstDossier.EtapesDosier > LastDossier.EtapesDosier ? LastDossier.DFX6FP6BEAC : FirstDossier.DFX6FP6BEAC;
                }
                catch (Exception)
                { }
                return 0;
            }
        }

        private double montantcv;
        public double MontantCV
        {
            get
            {
                try
                {
                    montantcv = 0;
                    Dossiers.ToList().ForEach(d =>
                    {
                        montantcv += d.Montant * Devise.ParitéXAF;
                    });
                }
                catch (Exception)
                { }
                return montantcv;
            }
        }

        public int GetDelai
        {
            get
            {
                try
                {
                    InitData();
                    if (LastDossier.EtapesDosier == 24 && FirstDossier.EtapesDosier != 24) return FirstDossier.GetDelai;
                    return FirstDossier.EtapesDosier > LastDossier.EtapesDosier ? LastDossier.GetDelai : FirstDossier.GetDelai;
                }
                catch (Exception)
                { }
                return 0;
            }
        }
        
        public string _GetEtapDossier
        {
            get
            {
                try
                {
                    InitData();
                    if (LastDossier.EtapesDosier == 24 && FirstDossier.EtapesDosier != 24) return FirstDossier.GetEtapDossier()[0];
                    return FirstDossier.EtapesDosier > LastDossier.EtapesDosier ? LastDossier.GetEtapDossier()[0] : FirstDossier.GetEtapDossier()[0];
                }
                catch (Exception)
                { }
                return "";
            }
        }

        public bool UserAsPermition(ApplicationDbContext db,Structure site, XtraRole role, int banqueID, string agentId)
        {
            foreach (var d in Dossiers)
            {
                try
                {
                    if (Fonctions.Fonctions.DroitSurDossier(db, d, site, role, banqueID, agentId))
                        return true;
                }
                catch (Exception)
                { }
            }
            return false;
        }

        [NotMapped]
        public List<Dossier> DossiersPermi { get; set; }

        public void DossiersPermitions(ApplicationDbContext db,Structure site, XtraRole role, int banqueID, string agentId)
        {
            DossiersPermi = new List<Dossier>();
            foreach (var d in Dossiers)
            {
                try
                {
                    if (Fonctions.Fonctions.DroitSurDossier(db, d, site, role, banqueID, agentId))
                        DossiersPermi.Add(d);
                }
                catch (Exception)
                { }
            }
        }

        public string GetFournisseur
        {
            get
            {
                try
                {
                    return Dossiers.FirstOrDefault().GetFournisseur;
                }
                catch (Exception)
                { }
                return "";
            }
        }

        public string GetClient
        {
            get
            {
                try
                {
                    return Dossiers.FirstOrDefault().GetClient;
                }
                catch (Exception)
                { }
                return "";
            }
        }

        public NatureOperation NatureOperation
        {
            get
            {
                try
                {
                    return Dossiers.FirstOrDefault().NatureOperation;
                }
                catch (Exception)
                { }
                return default;
            }
        }

        private DeviseMonetaire devise;
        public DeviseMonetaire Devise
        {
            get
            {
                if (Dossiers != null && Dossiers.Count > 0)
                {
                    devise = Dossiers.FirstOrDefault().DeviseMonetaire;
                }
                return devise;
            }
        }

        public StatutDossier GetStatusString()
        {
            StatutDossier statut = null;
            try
            {
                InitData();
                if (LastDossier.EtapesDosier == 24 && FirstDossier.EtapesDosier != 24) 
                    statut= FirstDossier.GetStatusString();
                else 
                   statut= FirstDossier.EtapesDosier > LastDossier.EtapesDosier ? LastDossier.GetStatusString() : FirstDossier.GetStatusString();
            }
            catch (Exception)
            { }
            if (statut == null) statut = new StatutDossier();
            return statut;
        }

        public virtual ICollection<Dossier> Dossiers { get; set; } = new List<Dossier>();

        public int BanqueId { get; set; }

        public virtual Banque Banque { get; set; }

        public int? EtapesDosier
        {
            get
            {
                try
                {
                    InitData();
                    var etb= FirstDossier.EtapesDosier;
                    var etend= LastDossier.EtapesDosier;
                    if (etend == 24 && etb != 24) return etb;
                    return etb>etend?etend:etb;
                }
                catch (Exception ee)
                { }
                return null;
            }
        }

         public int NbrDossiers
        {
            get
            {
                try
                {
                    return Dossiers.Count();
                }
                catch (Exception ee)
                { }
                return 0;
            }
        }

        public bool IsEtapeComplete
        {
            get
            {
                try
                {
                    var ep = Dossiers.ToList()[0].EtapesDosier;
                    foreach (var d in Dossiers)
                        if (d.EtapesDosier != ep)
                            return false;
                    return true;
                }
                catch (Exception e)
                { }
                return false;
            }
        }

        public string MontantString
        {
            get
            {
                int _NumberDecimalDigits = 0;
                try
                {
                    if (Montant % 1 > 0)
                    {
                        //_NumberDecimalDigits = Montant.ToString().Split(',')[1].Length;
                        _NumberDecimalDigits = 2;
                    }
                }
                catch (Exception)
                { }
                try
                {
                    NumberFormatInfo nfi = new NumberFormatInfo { NumberGroupSeparator = " ", NumberDecimalDigits = 2 };
                    return Montant.ToString("n", nfi);
                }
                catch (Exception)
                { }
                return "0";
            }
        }

        public string MontantCVString
        {
            get
            {
                try
                {
                    NumberFormatInfo nfi = new NumberFormatInfo { NumberGroupSeparator = " ", NumberDecimalDigits = 0 };
                    return MontantCV.ToString("n", nfi);
                }
                catch (Exception)
                { }
                return "0";
            }
        }

        public string MontantStringDevise
        {
            get
            {
                int _NumberDecimalDigits = 0;
                try
                {
                    if (Montant % 1 > 0)
                    {
                        _NumberDecimalDigits = Montant.ToString().Split(',')[1].Length;
                    }
                }
                catch (Exception)
                { }
                try
                {
                    NumberFormatInfo nfi = new NumberFormatInfo { NumberGroupSeparator = " ", NumberDecimalDigits = _NumberDecimalDigits };
                    return Montant.ToString("n", nfi) + " " + Dossiers.FirstOrDefault().DeviseToString;
                }
                catch (Exception)
                { }
                return "0";
            }
        }

        public string DeviseToString
        {
            get
            {
                try
                {
                    return Dossiers.FirstOrDefault().DeviseMonetaire.Nom;
                }
                catch (Exception)
                { }
                return "Non renseignée";
            }
        }

        public string DeviseToString2Caracters
        {
            get
            {
                try
                {
                    return Dossiers.FirstOrDefault().DeviseMonetaire.Nom.ToLower().Substring(0,1);
                }
                catch (Exception)
                { }
                return "Non renseignée";
            }
        }
        
        public string[] GetEtapDossier()
        {
            try
            {
                InitData();
                if (LastDossier.EtapesDosier == 24 && FirstDossier.EtapesDosier != 24) return FirstDossier.GetEtapDossier();
                return FirstDossier.EtapesDosier > LastDossier.EtapesDosier ? LastDossier.GetEtapDossier() : FirstDossier.GetEtapDossier();
            }
            catch (Exception e)
            { }
            return new string[2];
        }

        public string GetColor
        {
            get
            {
                try
                {
                    InitData();
                    if (LastDossier.EtapesDosier == 24 && FirstDossier.EtapesDosier != 24) return FirstDossier.GetColor;
                    return FirstDossier.EtapesDosier > LastDossier.EtapesDosier ? LastDossier.GetColor : FirstDossier.GetColor;
                }
                catch (Exception)
                { }
                return "";
            }
        }

        #region Données courriers
        public string CompteBEACEditer { get; set; }
        public string BanqueDomiciliaire { get; set; }
        public string Pays { get; set; }
        public string Ville { get; set; }
        public string CodeSwift { get; set; }
        [NotMapped]
        public string CompteanqueACrediter { get; set; }
        public int IdCompteAcrediter { get; set; }

        private string sign1;

        public string Signataire1
        {
            get { return string.IsNullOrEmpty(sign1) ? "Signataire" : sign1; }
            set { sign1 = value; }
        }

        private string sign2;
        public string Signataire2
        {
            get { return string.IsNullOrEmpty(sign2) ? "Signataire" : sign2; }
            set { sign2 = value; }
        }

        private string sign3;
        public string Signataire3
        {
            get { return string.IsNullOrEmpty(sign3) ? "Signataire" : sign3; }
            set { sign3 = value; }
        }

        private string sign4;
        public string Signataire4
        {
            get { return string.IsNullOrEmpty(sign4) ? "Signataire" : sign4; }
            set { sign4 = value; }
        }

        public NOTIFICATION? NOTIFICATION { get; set; }

        [NotMapped]
        public string fonction3 { get; set; }
        [NotMapped]
        public string fonction4 { get; set; }
        [NotMapped]
        public string GetBanque { get; set; }

        [NotMapped]
        public NumCompteBeneficiaire BanqueBeneficiaire { get; set; }

        #endregion

        public virtual DocumentAttache Courrier { get; set; }
        public virtual DocumentAttache RecapTransfert { get; set; }
        public virtual DocumentAttache MT { get; set; }
        [Display(Name ="Courrier de mise en demeure")]
        public virtual DocumentAttache MiseEnDemeure { get; set; }

        public int Get_IdBanqueBen
        {
            get
            {
                try
                {
                    if (Courrier != null)
                        return (int)this.Dossiers.FirstOrDefault().IdBanqueBenef;
                }
                catch (Exception)
                { }
                return 0;
            }
        }

        public string Get_Courrier
        {
            get
            {
                try
                {
                    if (Courrier != null)
                        return Courrier.GetImageDocumentAttache().GetImage();
                }
                catch (Exception)
                { }
                return "#";
            }
        }

        public string Get_Recap
        {
            get
            {
                try
                {
                    if (Courrier != null)
                        return RecapTransfert.GetImageDocumentAttache().GetImage();
                }
                catch (Exception)
                { }
                return "#";
            }
        }
        public string Get_MT
        {
            get
            {
                try
                {
                    if (MT != null)
                        return MT.GetImageDocumentAttache().GetImage();
                }
                catch (Exception)
                { }
                return "#";
            }
        }
        public bool Apurement
        {
            get
            {
                try
                {
                    if (Dossiers != null)
                        foreach (var d in Dossiers)
                            if (d.Apurement)
                                return true;
                    return false;
                }
                catch (Exception)
                { }
                return false;
            }
        }

        public bool Traité
        {
            get
            {
                try
                {
                    if (Dossiers != null)
                        foreach (var d in Dossiers)
                            if (!d.Traité)
                                return false;
                    return true;
                }
                catch (Exception)
                { }
                return false;
            }
        }
        
        public bool IsRejete
        {
            get
            {
                try
                {
                    if (Dossiers != null)
                        foreach (var d in Dossiers)
                            if (d.IsRejete)
                                return true;
                    return false;
                }
                catch (Exception)
                { }
                return false;
            }
        }
        public bool EstPasséConformite
        {
            get
            {
                try
                {
                    if (Dossiers != null)
                        foreach (var d in Dossiers)
                            if (!d.EstPasséConformite)
                                return false;
                    return true;
                }
                catch (Exception)
                { }
                return false;
            }
        }

        public bool ClientExist(int clientID)
        {
            try
            {
                if (Dossiers != null)
                    return Dossiers.FirstOrDefault(d=>d.ClientId==clientID)!=null?true:false;
            }
            catch (Exception)
            { }
            return false;
        }

        public bool AApurer
        {
            get
            {
                try
                {
                    bool entre = false;
                    foreach (var item in DossiersPermi)
                    {
                        entre = true;//Dossiers contient aumoins un elt
                        if (item.AApurer)
                            return true;
                    }
                    if (entre)
                        return false;
                }
                catch (Exception)
                { }
                return false;
            }
        }

        public bool AApurer_Ac
        {
            get
            {
                try
                {
                    bool entre = false;
                    foreach (var item in DossiersPermi)
                    {
                        entre = true;//Dossiers contient aumoins un elt
                        if (item.AApurer_Ac)
                            return true;
                    }
                    if (entre)
                        return false;
                }
                catch (Exception)
                { }
                return false;
            }
        }

        public bool AApurer_Av
        {
            get
            {
                try
                {
                    bool entre = false;
                    foreach (var item in DossiersPermi)
                    {
                        entre = true;//Dossiers contient aumoins un elt
                        if (!item.AApurer_Av)
                            return false;
                    }
                    if (entre)
                        return true;
                }
                catch (Exception)
                { }
                return false;
            }
        }

        public bool Echus
        {
            get
            {
                try
                {
                    bool entre = false;
                    foreach (var item in DossiersPermi)
                    {
                        entre = true;//Dossiers contient aumoins un elt
                        if (item.Echu)
                            return true;
                    }
                    if (entre)
                        return false;
                }
                catch (Exception)
                { }
                return false;
            }
        }

    }
}