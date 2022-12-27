using genetrix.Models;
using genetrix.Models.Fonctions;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;

namespace genetrix.Models
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

        //ICollection<ElementResumeTransfert>
        public virtual Object[] TransfertResume(int[] etat = null, double? Montant1 = null
            , double? Montant2 = null, string Devise = "", string Fourniseur = "", int? Delai1 = null, int? Delai2 = null, int? JourDepot1 = null, int? MoisDepot1 = null
            , int? AnneeDepot1 = null, int? JourDepot2 = null, int? MoisDepot2 = null, int? AnneeDepot2 = null
            , int? JourTraitement1 = null, int? MoisTraitement1 = null, int? EtatDuDossier = null
            , int? AnneeTraitement1 = null, int? JourTraitement2 = null, int? MoisTraitement2 = null, int? AnneeTraitement2 = null
            , string Cat = "", bool DFX = false, bool FP = false, bool Ref = false)
        {
            List<ElementResumeTransfert> elts = new List<ElementResumeTransfert>();
            List<Dossier> _dossiers = new List<Dossier>();
            string[] mois = new string[] { "Jeanvier", "Févier", "Mars", "Avril", "Mai", "Juin", "Juillet", "Aoùt", "Septembre", "Octobre", "Novembre", "Decembre" };
            string serachString = "";
            _dossiers.AddRange(this.Dossiers);
            if (Montant1 > 0 && Montant2 == null)
            {
                try
                {
                    serachString += $";Montant1:{Montant1}";
                    _dossiers = _dossiers.Where(d => d.Montant == Montant1).ToList();
                }
                catch (Exception)
                { }
            }
            if (Montant2 > 0)
            {
                try
                {
                    serachString += $";Montant2:{Montant2}";
                    if (Montant1 != null)
                    {
                        serachString += $";Montant1:{Montant1}";
                        _dossiers = _dossiers.Where(d => d.Montant >= Montant1 && d.Montant <= Montant2).ToList();
                    }
                    else
                        _dossiers = _dossiers.Where(d => d.Montant <= Montant2).ToList();
                }
                catch (Exception)
                { }
            }
            if (!string.IsNullOrEmpty(Devise))
            {
                try
                {
                    serachString += $";Devise:{Devise}";
                    _dossiers = _dossiers.Where(d => d.DeviseMonetaire.Libelle == Devise).ToList();
                }
                catch (Exception)
                { }
            }
            if (!string.IsNullOrEmpty(Fourniseur))
            {
                try
                {
                    serachString += $";Fourniseur:{Fourniseur}";
                    _dossiers = _dossiers.Where(d => d.Fournisseur.Nom == Fourniseur).ToList();
                }
                catch (Exception)
                { }
            }
            if (Delai1 > 0 && Delai2 == null)
            {
                serachString += $";Delai1:{Delai1}";
                try
                {
                    var dossiers1 = _dossiers.ToList();
                    _dossiers.Clear();
                    dossiers1.Where(d => d.Date_Etape22 != null).ToList().ForEach(d =>
                    {
                        try
                        {
                            if ((d.Date_Etape22 - d.DateDepotBank).Value.TotalDays == Delai1)
                                _dossiers.Add(d);
                        }
                        catch (Exception)
                        { }
                    });
                    dossiers1 = null;
                }
                catch (Exception)
                { }
            }
            else if (Delai2 > 0)
            {
                serachString += $";Delai2:{Delai2}";
                var dossiers1 = _dossiers.ToList();
                _dossiers.Clear();
                if (Delai1 == null)
                {
                    try
                    {
                        dossiers1.Where(d => d.Date_Etape22 != null).ToList().ForEach(d =>
                        {
                            try
                            {
                                if ((d.Date_Etape22 - d.DateDepotBank).Value.TotalDays <= Delai2)
                                    _dossiers.Add(d);
                            }
                            catch (Exception)
                            { }
                        });
                    }
                    catch (Exception)
                    { }
                }
                else
                {
                    serachString += $";Delai1:{Delai1}";
                    var _delai = 0.0;
                    try
                    {
                        dossiers1.Where(d => d.Date_Etape22 != null).ToList().ForEach(d =>
                        {
                            try
                            {
                                _delai = (d.Date_Etape22 - d.DateDepotBank).Value.TotalDays;
                                if (_delai >= Delai1 && _delai <= Delai1)
                                    _dossiers.Add(d);
                            }
                            catch (Exception)
                            { }
                        });
                    }
                    catch (Exception)
                    { }
                }
                dossiers1 = null;
            }
            if (JourDepot1 > 0 && JourDepot2 == null)
            {
                try
                {
                    _dossiers = _dossiers.Where(d => d.DateDepotBank.Value.Day == JourDepot1).ToList();
                    serachString += $";JourDepot1:{JourDepot1}";
                }
                catch (Exception)
                { }
            }
            else if (JourDepot2 > 0)
            {
                serachString += $";JourDepot2:{JourDepot2}";
                if (JourDepot1 > 0)
                {
                    try
                    {
                        _dossiers = _dossiers.Where(d => d.DateDepotBank.Value.Day >= JourDepot1 && d.DateDepotBank.Value.Day <= JourDepot2).ToList();
                        serachString += $";JourDepot1:{JourDepot1}";
                    }
                    catch (Exception)
                    { }
                }
                else
                    try
                    {
                        _dossiers = _dossiers.Where(d => d.DateDepotBank.Value.Day <= JourDepot2).ToList();
                    }
                    catch (Exception)
                    { }
            }
            if (MoisDepot1 > 0 && MoisDepot2 == null)
            {
                try
                {
                    _dossiers = _dossiers.Where(d => d.DateDepotBank.Value.Month == MoisDepot1).ToList();
                    serachString += $";MoisDepot1:{mois[(int)MoisDepot1]}";
                }
                catch (Exception)
                { }
            }
            else if (MoisDepot2 > 0)
            {
                serachString += $";MoisDepot2:{mois[(int)MoisDepot2]}";
                if (MoisDepot1 > 0)
                {
                    try
                    {
                        _dossiers = _dossiers.Where(d => d.DateDepotBank.Value.Month >= MoisDepot1 && d.DateDepotBank.Value.Month <= MoisDepot2).ToList();
                        serachString += $";MoisDepot1:{mois[(int)MoisDepot1]}";
                    }
                    catch (Exception)
                    { }
                }
                else
                    try
                    {
                        _dossiers = _dossiers.Where(d => d.DateDepotBank.Value.Month <= MoisDepot2).ToList();
                    }
                    catch (Exception)
                    { }
            }
            if (AnneeDepot1 > 0 && AnneeDepot2 == null)
            {
                try
                {
                    _dossiers = _dossiers.Where(d => d.DateDepotBank.Value.Year == AnneeDepot1).ToList();
                    serachString += $";AnneeDepot1:{AnneeDepot1}";
                }
                catch (Exception)
                { }
            }
            else if (AnneeDepot2 > 0)
            {
                try
                {
                    serachString += $";AnneeDepot2:{AnneeDepot2}";
                    if (AnneeDepot2 > 0)
                    {
                        _dossiers = _dossiers.Where(d => d.DateDepotBank.Value.Year >= AnneeDepot1 && d.DateDepotBank.Value.Year <= AnneeDepot2).ToList();
                        serachString += $";AnneeDepot1:{AnneeDepot1}";
                    }
                    else
                        _dossiers = _dossiers.Where(d => d.DateDepotBank.Value.Month <= AnneeDepot2).ToList();
                }
                catch (Exception)
                { }
            }

            if (JourTraitement1 > 0 && JourTraitement2 == null)
            {
                try
                {
                    _dossiers = _dossiers.Where(d => d.Date_Etape22.Value.Day == JourTraitement1).ToList();
                    serachString += $";JourTraitement1:{JourTraitement1}";
                }
                catch (Exception)
                { }
            }
            else if (JourTraitement2 > 0)
            {
                serachString += $";JourTraitement2:{JourTraitement2}";
                if (JourTraitement1 > 0)
                {
                    _dossiers = _dossiers.Where(d => d.Date_Etape22.Value.Day >= JourTraitement1 && d.Date_Etape22.Value.Day <= JourTraitement2).ToList();
                    serachString += $";JourTraitement1:{JourTraitement1}";
                }
                else
                    _dossiers = _dossiers.Where(d => d.Date_Etape22.Value.Day <= JourTraitement2).ToList();
            }
            if (MoisTraitement1 > 0 && MoisTraitement2 == null)
            {
                try
                {
                    _dossiers = _dossiers.Where(d => d.Date_Etape22.Value.Month == MoisTraitement1).ToList();
                    serachString += $";MoisTraitement1:{mois[(int)MoisTraitement1]}";
                }
                catch (Exception)
                { }
            }
            else if (MoisTraitement2 > 0)
            {
                try
                {
                    serachString += $";MoisTraitement2:{mois[(int)MoisTraitement2]}";
                    if (MoisTraitement1 > 0)
                    {
                        try
                        {
                            _dossiers = _dossiers.Where(d => d.Date_Etape22.Value.Month >= MoisTraitement1 && d.Date_Etape22.Value.Month <= MoisTraitement2).ToList();
                            serachString += $";MoisTraitement1:{mois[(int)MoisTraitement1]}";
                        }
                        catch (Exception)
                        { }
                    }
                    else
                        try
                        {
                            _dossiers = _dossiers.Where(d => d.Date_Etape22.Value.Month <= MoisTraitement2).ToList();
                        }
                        catch (Exception)
                        { }
                }
                catch (Exception)
                { }
            }
            if (AnneeTraitement1 > 0 && AnneeTraitement2 == null)
            {
                try
                {
                    _dossiers = _dossiers.Where(d => d.Date_Etape22.Value.Year == AnneeTraitement1).ToList();
                    serachString += $";AnneeTraitement1:{AnneeTraitement1}";
                }
                catch (Exception)
                { }
            }
            else if (AnneeTraitement2 > 0)
            {
                try
                {
                    serachString += $";AnneeTraitement2:{AnneeTraitement2}";
                    if (AnneeTraitement2 > 0)
                    {
                        _dossiers = _dossiers.Where(d => d.Date_Etape22.Value.Year >= AnneeTraitement1 && d.Date_Etape22.Value.Year <= AnneeTraitement2).ToList();
                        serachString += $";AnneeTraitement1:{AnneeTraitement1}";
                    }
                    else
                        _dossiers = _dossiers.Where(d => d.Date_Etape22.Value.Month <= AnneeTraitement2).ToList();
                }
                catch (Exception)
                { }
            }

            switch (Cat)
            {
                case "aapurer":
                    _dossiers = _dossiers.Where(d => d.EtapesDosier == 23 || d.EtapesDosier == -231 || d.EtapesDosier == -232 || d.EtapesDosier == -230 || d.EtapesDosier == 231 || d.EtapesDosier == 230 || d.EtapesDosier == 232).ToList();
                    serachString += $";Cat:{Cat}";
                    break;
                case "echu":
                    _dossiers = _dossiers.Where(d => d.EtapesDosier == 25 || d.EtapesDosier == 250 || d.EtapesDosier == -250).ToList();
                    serachString += $";Cat:{Cat}";
                    break;
                case "apure":
                    _dossiers = _dossiers.Where(d => d.EtapesDosier == 24).ToList();
                    serachString += $";Cat:{Cat}";
                    break;
                default:
                    break;
            }

            if (DFX)
            {
                try
                {
                    _dossiers = _dossiers.Where(d => d.DFX6FP6BEAC == 1).ToList();
                    serachString += $";DFX:{DFX}";
                }
                catch (Exception)
                { }
            }
            if (FP)
            {
                try
                {
                    _dossiers = _dossiers.Where(d => d.DFX6FP6BEAC == 2).ToList();
                    serachString += $";FP:{FP}";
                }
                catch (Exception)
                { }
            }
            if (Ref)
            {
                try
                {
                    _dossiers = _dossiers.Where(d => d.DFX6FP6BEAC == 3).ToList();
                    serachString += $";Ref:{Ref}";
                }
                catch (Exception)
                { }
            }

            Dictionary<string, RapportMontantDevise> rapportMontantDevises = new Dictionary<string, RapportMontantDevise>();
            double totalXaf = 0.0, totalEnDevise = 0; string cle = "";
            if (etat == null)
            {
                foreach (var item in _dossiers)
                {
                    try
                    {
                        cle = item.DeviseLib + item._GroupeWFDossier;
                        totalXaf += item.MontantCV;
                        totalEnDevise += item.Montant;
                        if (!rapportMontantDevises.ContainsKey(cle))
                            rapportMontantDevises.Add(cle, new RapportMontantDevise()
                            {
                                Devise = item.DeviseLib,
                                Etat = item.EtapesDosierToString,
                                TotalXaf = item.MontantCV,
                                MontantTotal = item.Montant
                            });
                        else
                        {
                            rapportMontantDevises[cle].MontantTotal += item.Montant;
                            rapportMontantDevises[cle].TotalXaf += item.MontantCV;
                        }

                        elts.Add(new ElementResumeTransfert()
                        {
                            DateCreation = item.DateCreationAppToString,
                            DateModif = item.DateModifToString,
                            DateDepotBanque = item.DateDepotBankToString,
                            DateTraitement = item.Date_Etape22ToString,
                            Nbrfichier = item.TotalFichiers,
                            Montant = item.Montant,
                            MontantSting = item.MontantString,
                            MontantCv = item.MontantCV,
                            MontantCvString = item.MontantCVstring,
                            GetCategorie = item.DFX6FP6BEAC + "",
                            GetAgenceName = item.GetAgenceName,
                            GetReference = item.GetReference,
                            GetGestionnaire = item.GestionnaireName,
                            Nom = $"{item.GetClient} ",
                            Client = $"{this.Nom}",
                            Beneficiaire = $"{item.GetFournisseur}",
                            Devise = $"{item.DeviseToString}",
                            //Description = $"<p>- Montant en devise: {item.MontantStringDevise}. Bénéficiaire: {item.GetFournisseur}. <p/>"
                            //            + $"<p>- Date de banque: {item.DateDepotBankToString}. Date traité: {item.Date_Etape22ToString}"
                            //            + $"<p>- Gestionnaire: {item.GestionnaireName}. Statut: {item.GetMotifCurrenteStatus}"
                        }); ;
                    }
                    catch (Exception)
                    { }
                }
            }
            else
            {
                foreach (var item in _dossiers)
                {
                    try
                    {
                        cle = item.DeviseLib + item._GroupeWFDossier;
                        totalXaf += item.MontantCV;
                        totalEnDevise += item.Montant;
                        if (!rapportMontantDevises.ContainsKey(cle))
                            rapportMontantDevises.Add(cle, new RapportMontantDevise()
                            {
                                Devise = item.DeviseLib,
                                Etat = item.EtapesDosierToString,
                                TotalXaf = item.MontantCV,
                                MontantTotal = item.Montant
                            });
                        else
                        {
                            rapportMontantDevises[cle].MontantTotal += item.Montant;
                            rapportMontantDevises[cle].TotalXaf += item.MontantCV;
                        }

                        elts.Add(new ElementResumeTransfert()
                        {
                            DateCreation = item.DateCreationAppToString,
                            DateModif = item.DateModifToString,
                            Nbrfichier = item.TotalFichiers,
                            DateDepotBanque = item.DateDepotBankToString,
                            DateTraitement = item.Date_Etape22ToString,
                            Montant = item.Montant,
                            MontantSting = item.MontantString,
                            MontantCv = item.MontantCV,
                            MontantCvString = item.MontantCVstring,
                            GetCategorie = item.DFX6FP6BEAC + "",
                            GetAgenceName = item.GetAgenceName,
                            GetGestionnaire = item.GestionnaireName,
                            GetReference = item.GetReference,
                            Nom = $"{item.GetClient} ",
                            Client = $"{this.Nom}",
                            Beneficiaire = $"{item.GetFournisseur}",
                            Devise = $"{item.DeviseToString}",
                            Description = $"<p>- Montant en devise: {item.MontantStringDevise}. Bénéficiaire: {item.GetFournisseur}. <p/>"
                                        + $"<p>- Date de banque: {item.DateDepotBankToString}. Date traité: {item.Date_Etape22ToString}"
                                        + $"<p>- Gestionnaire: {item.GestionnaireName}. Statut: {item.GetMotifCurrenteStatus}"
                        });
                    }
                    catch (Exception)
                    { }
                }
            }


            return new Object[] { elts, serachString, rapportMontantDevises, totalXaf, totalEnDevise };
        }

        internal IEnumerable<Contact> GetDefaultContacts()
        {
            var getContacts = new List<Contact>();
            //Ajout des collegues comme contacts
            try
            {
                this.Utilisateurs.ToList().ForEach(u =>
                {
                    try
                    {
                        getContacts.Add(new Contact()
                        {
                            Email = u.Email,
                            Groupe = Groupe.Collegue,
                            NomComplet = u.Nom,
                            Telephone = u.Tel1
                        });
                    }
                    catch (Exception)
                    { }
                });
            }
            catch (Exception)
            { }
            //Ajouter le gestionnaire du client comme contacts
            try
            {
                getContacts.Add(new Contact()
                {
                    Email = this.GestionnaireEmail,
                    Groupe = Groupe.Gestionnaire,
                    NomComplet = this.GetGestionnaire,
                    Telephone = this.Telephone
                });
            }
            catch (Exception)
            { }
            //Ajouter les fournisseurs du client comme contacts
            try
            {
                if (this.Fournisseurs != null)
                    this.Fournisseurs.ToList().ForEach(f =>
                    {
                        try
                        {
                            getContacts.Add(new Contact()
                            {
                                Email = f.Email,
                                Groupe = Groupe.Fournisseur,
                                NomComplet = f.Nom,
                                Telephone = f.Tel1,
                                Pays = f.Pays,
                                Ville = f.Ville
                            });
                        }
                        catch (Exception)
                        { }
                    });
            }
            catch (Exception)
            { }
            if (getContacts == null) getContacts = new List<Contact>();
            return getContacts;
        }

        public int Id { get; set; }
        [Required]
        public string Nom { get; set; }

        [DisplayName("Email associé à l'entreprise")]
        //[Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Code établissement"), MaxLength(5)]
        public string CodeEtablissement { get; set; }

        public string Profession { get; set; }
        public string Pays { get; set; }

        string email;
        public string GetEmail
        {
            get
            {
                try
                {
                    if (string.IsNullOrEmpty(Email)) return Email;
                    foreach (var a in Adresses)
                    {
                        if (!string.IsNullOrEmpty(a.Email))
                            return a.Email;
                    }
                }
                catch (Exception)
                { }
                return "";
            }
        }

        public virtual ICollection<Adresse> Adresses { get; set; }

        public virtual ICollection<BanqueClient> Banques { get; set; }


        public string GetBanque(ApplicationDbContext db)
        {
            try
            {
                if (Banques != null && Banques.Count > 0)
                    return Banques.FirstOrDefault().Site.BanqueName(db);
            }
            catch (Exception)
            { }
            return "";
        }


        [Display(Name = "Autres banque du client")]
        public virtual ICollection<BanqueTierClient> BanqueTierClients { get; set; }

        public virtual ICollection<CompteClient> Utilisateurs { get; set; }
        public virtual ICollection<Fournisseurs> Fournisseurs { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; }

        public ICollection<NumCompte> GetNumComptes()
        {
            try
            {
                if (this.Banques.Count > 0)
                    return Banques.FirstOrDefault().NumComptes;
            }
            catch (Exception)
            { }
            return new List<NumCompte>();
        }

        public string Logo { get; set; }

        private bool IsActiveClient;
        public bool EstClientActive
        {
            get
            {
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
                { }
                return IsActiveClient;
            }
        }

        public virtual ICollection<Dossier> Dossiers { get; set; }

        public virtual ICollection<ClFichier> Fichiers { get; set; }

        public virtual ICollection<DocumentAttache> DocumentAttaches { get; set; }

        //public virtual ICollection<Justificatif> Factures { get; set; }

        public virtual ICollection<ReferenceBanque> ReferenceBanques { get; set; }
        public string Adresse { get; set; }
        public string Ville { get; set; }
        public string Telephone { get; set; }

        #region Documents obligatoires
        [DisplayName("Fiche KYC")]
        public virtual DocumentAttache FicheKYC { get; set; }

        [DisplayName("Plan localisation du siège social")]
        public virtual DocumentAttache PlanLSS { get; set; }

        [DisplayName("Extrait RCCM ou autre document tenant lieu")]
        public virtual DocumentAttache RCCM_Cl { get; set; }

        [DisplayName("Copie des statuts")]
        public virtual DocumentAttache Statut { get; set; }

        [DisplayName("Proces-verbal nommant les dirigeants")]
        public virtual DocumentAttache ProcesVerbal { get; set; }

        [DisplayName("Les états financiers des deux derniers exercices")]
        public virtual DocumentAttache EtatFinanciers { get; set; }

        [DisplayName("Attestation sur honneur")]
        public virtual DocumentAttache AtestationHinneur { get; set; }
        public virtual ICollection<DocumentAttache> AutresDocuments { get; set; }
        #endregion

        #region Documents Personne physique
        [DisplayName("Plan de localisation du domicile")]
        public virtual DocumentAttache PlanLOcalisationDomi { get; set; }

        [Display(Name = "Justificatifsn de domicile (facture d'eau ou d'électricité)")]
        public virtual DocumentAttache JustifDomicile { get; set; }

        [Display(Name = "Copie de la carte d'identité ou du passeport")]
        public virtual DocumentAttache CarteIdentie { get; set; }


        public string Get_CarteIdentie
        {
            get
            {
                try
                {
                    if (CarteIdentie != null)
                        return CarteIdentie.GetImageDocumentAttache().GetImage();
                }
                catch (Exception)
                { }
                return "#";
            }
        }


        public int Get_CarteIdentieId
        {
            get
            {
                try
                {
                    if (CarteIdentie != null)
                        return CarteIdentie.Id;
                }
                catch (Exception)
                { }
                return 0;
            }
        }


        public string Get_JustifDomicile
        {
            get
            {
                try
                {
                    if (JustifDomicile != null)
                        return JustifDomicile.GetImageDocumentAttache().GetImage();
                }
                catch (Exception)
                { }
                return "#";
            }
        }


        public int Get_JustifDomicileId
        {
            get
            {
                try
                {
                    if (JustifDomicile != null)
                        return JustifDomicile.Id;
                }
                catch (Exception)
                { }
                return 0;
            }
        }

        public string Get_PlanLOcalisationDomi
        {
            get
            {
                try
                {
                    if (PlanLOcalisationDomi != null)
                        return PlanLOcalisationDomi.GetImageDocumentAttache().GetImage();
                }
                catch (Exception)
                { }
                return "#";
            }
        }

        public int Get_PlanLOcalisationDomiId
        {
            get
            {
                try
                {
                    if (PlanLOcalisationDomi != null)
                        return PlanLOcalisationDomi.Id;
                }
                catch (Exception)
                { }
                return 0;
            }
        }

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

        public string CodeAgence
        {
            get
            {
                try
                {
                    if (Banques != null)
                        return Banques.FirstOrDefault().CodeEts;
                }
                catch (Exception)
                { }
                return "";
            }
        }

        public string AgenceName
        {
            get
            {
                try
                {
                    if (Banques != null)
                        return Banques.FirstOrDefault().AgenceNAme;
                }
                catch (Exception)
                { }
                return "";
            }
        }

        public string AgenceAdresse
        {
            get
            {
                try
                {
                    if (Banques != null)
                        return Banques.FirstOrDefault().AgenceAdresse;
                }
                catch (Exception)
                { }
                return "";
            }
        }

        public string GetGestionnaire
        {
            get
            {
                try
                {
                    if (Banques != null)
                        return Banques.FirstOrDefault().GestionnaireName;
                }
                catch (Exception)
                { }
                return "";
            }
        }
        
        public string GetGestionnaireId
        {
            get
            {
                try
                {
                    if (Banques != null)
                        return Banques.FirstOrDefault().IdGestionnaire;
                }
                catch (Exception)
                { }
                return "";
            }
        }

        public string GestionnaireTel
        {
            get
            {
                try
                {
                    if (Banques != null)
                        return Banques.FirstOrDefault().GestionnaireTel;
                }
                catch (Exception)
                { }
                return "";
            }
        }

        public string GestionnaireEmail
        {
            get
            {
                try
                {
                    if (Banques != null)
                        return Banques.FirstOrDefault().GestionnaireEmail;
                }
                catch (Exception)
                { }
                return "";
            }
        }

        public string RIB(string numCompte, string cle)
        {
            try
            {
                return $"{CodeEtablissement}_{CodeAgence}_{numCompte}_{cle}";
            }
            catch (Exception)
            { }
            return "_|_|___|_";
        }
        public int Get_AtestationHinneurId
        {
            get
            {
                try
                {
                    if (AtestationHinneur != null)
                        return AtestationHinneur.Id;
                }
                catch (Exception)
                { }
                return 0;
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
        public int Get_EtatFinanciersId
        {
            get
            {
                try
                {
                    if (EtatFinanciers != null)
                        return EtatFinanciers.Id;
                }
                catch (Exception)
                { }
                return 0;
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
        public int Get_ProcesVerbalId
        {
            get
            {
                try
                {
                    if (ProcesVerbal != null)
                        return ProcesVerbal.Id;
                }
                catch (Exception)
                { }
                return 0;
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
        public int Get_StatutId
        {
            get
            {
                try
                {
                    if (Statut != null)
                        return Statut.Id;
                }
                catch (Exception)
                { }
                return 0;
            }
        }

        public string Get_RCCM
        {
            get
            {
                try
                {
                    if (RCCM_Cl != null)
                        return RCCM_Cl.GetImageDocumentAttache().GetImage();
                }
                catch (Exception)
                { }
                return "#";
            }
        }
        public int Get_RCCMId
        {
            get
            {
                try
                {
                    if (RCCM_Cl != null)
                        return RCCM_Cl.Id;
                }
                catch (Exception)
                { }
                return 0;
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

        public int Get_PlanLSSId
        {
            get
            {
                try
                {
                    if (PlanLSS != null)
                        return PlanLSS.Id;
                }
                catch (Exception)
                { }
                return 0;
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
        public int Get_FicheKYCId
        {
            get
            {
                try
                {
                    if (FicheKYC != null)
                        return FicheKYC.Id;
                }
                catch (Exception)
                { }
                return 0;
            }
        }

        public DocumentAttache AttestationSurHonneur
        {
            get
            {
                try
                {
                    foreach (var d in DocumentAttaches.Where(d => d.AttestSurHonneur).OrderByDescending(d => d.DateSignature).ToList())
                    {
                        try
                        {
                            if (d.DateCreation != null && (DateTime.Now - d.DateSignature.Value).TotalDays < 30)
                            {
                                return d;
                            }
                        }
                        catch (Exception e)
                        { }
                    }
                }
                catch (Exception)
                { }
                return null;
            }
        }

        public int? Get_AttestationSurHonneurId
        {
            get
            {
                try
                {
                    if(AttestationSurHonneur!=null)
                    return AttestationSurHonneur.Id;
                }
                catch (Exception)
                { }
                return null;
            }
        }

        public bool AttestNonDefautValide
        {
            get
            {
                try
                {
                    if (BanqueTierClients == null || BanqueTierClients.Count == 0) return false;
                    foreach (var d in BanqueTierClients.ToList())
                    {
                        try
                        {
                            if (d.AttestationValid==null)
                            {
                                return false;
                            }
                        }
                        catch (Exception e)
                        { }
                    }
                }
                catch (Exception)
                { return false; }
                return true;
            }
        }


        public DocumentAttache Get_AttestationNonApurement2
        {
            get
            {
                try
                {
                    foreach (var d in BanqueTierClients.FirstOrDefault(b => b.IdClient == Id).AttestationNonDefautAp.OrderByDescending(d => d.DateSignature).ToList())
                    {
                        try
                        {
                            if (d.DateCreation != null && (DateTime.Now - d.DateSignature.Value).TotalDays < 30)
                            {
                                return d;
                            }
                        }
                        catch (Exception e)
                        { }
                    }
                }
                catch (Exception)
                { }
                return null;
            }
        }

        //[Display(Name = "Attestation de non defaut d'apurement")]
        //public virtual ICollection<DocumentAttache> AttestationNonDefautAp { get; set; }

        public DateTime? DateCreation { get; set; }
        [Display(Name ="Mode restraint (activer cette fonctionalité si vous souhaitez gener les rôles utilisateurs)")]
        public bool ModeRestraint { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idBanque">Id site (agence)</param>
        /// <returns></returns>
        public IList<ClientDossierVM> GetClientDossierVM(int? idSite)
        {
            int nb_dossiers = 0, valeur = 0, device = 0;
            var list = new List<ClientDossierVM>();
            var dico = new Dictionary<string, ClientDossierVM>();

            try
            {
                //dossiers
                var _devise = "";
                Dossiers.Where(d => d.EtapesDosier != 24 && !d.Archive && d.IdSite == idSite).ToList().ForEach(d =>
                {
                    _devise = d.DeviseMonetaire.Nom;
                    if (dico.Keys.Count == 0 || !dico.Keys.Contains(_devise))
                    {
                        dico.Add(_devise, new ClientDossierVM()
                        {
                            Client = this.Nom,
                            Devise = _devise,
                            Id = this.Id,
                            NbrDossiers = 1,
                            Valeur = d.Montant
                        });
                    }
                    else
                    {
                        dico[_devise].NbrDossiers++;
                        dico[_devise].Valeur += d.Montant;
                    }
                });
            }
            catch (Exception)
            { }
            return dico.Values.ToList();
        }
    }
}