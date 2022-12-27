using eApurement.Models;
using eApurement.Models.Fonctions;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace e_apurement.Models
{
    [DefaultProperty("Nom")]
    [Table("Client")]
    public class Client : IDelateCustom
    {
        public bool Remove(ApplicationDbContext db)
        {
            try
            {
                db.GetClients.Remove(this);
                return true;
            }
            catch (Exception)
            { }
            return false;
        }

        public int Id { get; set; }
        [Required]
        public string Nom { get; set; }

        [DisplayName("Email associé à l'entreprise")]
        //[Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public virtual ICollection<Adresse> Adresses { get; set; }

        public virtual ICollection<BanqueClient> Banques { get; set; }

        public virtual ICollection<CompteClient> Utilisateurs { get; set; }
        public virtual ICollection<Fournisseurs> Fournisseurs { get; set; }

        public byte[] Logo { get; set; }

        //public string CompteId { get; set; }
        //public virtual CompteClient Compte { get; set; }

        private bool IsActiveClient;
        public bool EstClientActive
        {
            get {
                try
                {
                    //if (SecuritySystem.CurrentUser is CompteClient &&
                    //    (SecuritySystem.CurrentUser as CompteClient).Client.Id == this.Id)
                    //{
                    //    IsActiveClient = true;
                    //}
                    //else if (SecuritySystem.CurrentUser is CompteBanqueCommerciale )
                    //{
                    //    foreach (var b in this.Banques)
                    //        if (b.Banque.Id == (SecuritySystem.CurrentUser as CompteBanqueCommerciale).Banque.Id)
                    //            return true;
                    //}
                }
                catch (System.Exception)
                {}
                return IsActiveClient; 
            }
        }

        public virtual ICollection<Dossier> Dossiers { get; set; }

        public virtual ICollection<ClFichier> Fichiers { get; set; }

        public virtual ICollection<DocumentAttache> DocumentAttaches { get; set; }

        //public virtual ICollection<Justificatif> Factures { get; set; }

        public virtual ICollection<ReferenceBanque> ReferenceBanques { get; set; }
        public string Adresse { get;  set; }
        public string Telephone { get;  set; }

        #region Documents obligatoires
        [DisplayName("Fiche KYC")]
        public DocumentAttache FicheKYC { get; set; }
        
        [DisplayName("Plan localisation du siège social")]
        public DocumentAttache PlanLSS { get; set; }

        [DisplayName("Extrait RCCM ou autre document tenant lieu")]
        public DocumentAttache RCCM { get; set; }

        [DisplayName("Copie des statuts")]
        public DocumentAttache Statut { get; set; }

        [DisplayName("Proces-verbal nommant les dirigeants")]
        public DocumentAttache ProcesVerbal { get; set; }

        [DisplayName("Les états financiers des deux derniers exercices")]
        public DocumentAttache EtatFinanciers { get; set; }

        [DisplayName("Attestation sur honneur")]
        public DocumentAttache AtestationHinneur { get; set; }
        #endregion

        public string Get_AtestationHinneur
        {
            get
            {
                try
                {
                    if (AtestationHinneur != null)
                        return AtestationHinneur.GetImageDocumentAttache().GetImage();
                }
                catch (Exception)
                { }
                return "#";
            }
        }
        public string Get_EtatFinanciers
        {
            get
            {
                try
                {
                    if (EtatFinanciers != null)
                        return EtatFinanciers.GetImageDocumentAttache().GetImage();
                }
                catch (Exception)
                { }
                return "#";
            }
        }
        
        public string Get_ProcesVerbal
        {
            get
            {
                try
                {
                    if (ProcesVerbal != null)
                        return ProcesVerbal.GetImageDocumentAttache().GetImage();
                }
                catch (Exception)
                { }
                return "#";
            }
        }
         
        public string Get_Statut
        {
            get
            {
                try
                {
                    if (Statut != null)
                        return Statut.GetImageDocumentAttache().GetImage();
                }
                catch (Exception)
                { }
                return "#";
            }
        }
        
        public string Get_RCCM
        {
            get
            {
                try
                {
                    if (RCCM != null)
                        return RCCM.GetImageDocumentAttache().GetImage();
                }
                catch (Exception)
                { }
                return "#";
            }
        }
        
        public string Get_PlanLSS
        {
            get
            {
                try
                {
                    if (PlanLSS != null)
                        return PlanLSS.GetImageDocumentAttache().GetImage();
                }
                catch (Exception)
                { }
                return "#";
            }
        }
        public string Get_FicheKYC
        {
            get
            {
                try
                {
                    if (FicheKYC != null)
                        return FicheKYC.GetImageDocumentAttache().GetImage();
                }
                catch (Exception)
                { }
                return "#";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idBanque">Id site (agence)</param>
        /// <returns></returns>
        public IList<ClientDossierVM> GetClientDossierVM(int? idSite)
        {
            int nb_dossiers = 0, valeur = 0, device = 0;
            var list=new List<ClientDossierVM>();
            var dico = new Dictionary<string, ClientDossierVM>();

            try
            {
                //dossiers
                var _devise = "";
                Dossiers.Where(d => !d.Apure && !d.Archive && d.IdSite== idSite).ToList().ForEach(d =>
                {
                    _devise = d.DeviseMonetaire.Nom;
                    if(dico.Keys.Count==0 || !dico.Keys.Contains(_devise))
                    {
                        dico.Add(_devise, new ClientDossierVM()
                        {
                            Client=this.Nom,
                            Devise=_devise,
                            Id=this.Id,
                            NbrDossiers=1,
                            Valeur=d.Montant
                        });
                    }
                    else
                    {
                        dico[_devise].NbrDossiers++;
                        dico[_devise].Valeur +=d.Montant;
                    }
                });
            }
            catch (Exception)
            {}
            return dico.Values.ToList();
        }
    }
}