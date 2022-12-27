using genetrix.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace genetrix.Models
{
    public class Structure
    {
        public int Id { get; set; }
        [DisplayName("Nom de la structure")]
        public virtual string Nom { get; set; }
        public string Adresse { get; set; }
        public string Ville { get; set; }
        public string Pays { get; set; }
        public string Telephone { get; set; }
        public string Telephone2 { get; set; }
        [Display(Name = "Code établissement"), MaxLength(5)]
        public string CodeEtablissement { get; set; }

        #region Permissions
        [Display(Name = "Niveau min L/E dossier")]
        public int? NiveauDossier { get; set; }
        [Display(Name = "Niveau max L/E dossier")]
        public int? NiveauMaxManager{ get; set; }

        [Display(Name = "Niveau max lecture dossier")]
        public int? NiveauMaxDossier { get; set; }

        [Display(Name = "Voir des dossiers de toute structure")]
        public bool VoirDossiersAutres { get; set; }

        [Display(Name = "Voir des utilisateurs de toute structure")]
        public bool VoirUsersAutres { get; set; }
        [Display(Name = "Voir des clients de toute structure")]
        public bool VoirClientAutres { get; set; } 
        #endregion

        [DisplayName("Type *")]
        [ForeignKey("TypeStructure")]
        //[Required]
        public int? IdTypeStructure { get; set; }
        public virtual TypeStructure TypeStructure { get; set; }
        [Display(Name = "Est agence")]
        public bool EstAgence { get; set; }
        [Display(Name = "Suivi chat")]
        public bool SuitChat { get; set; }

        [DisplayName("Niveau hiérarchique")]
        public int NiveauH { get; set; }

        [Display(Name = "Chef")]
        [ForeignKey("Responsable")]
        public string IdResponsable { get; set; } = null;
        public virtual CompteBanqueCommerciale Responsable { get; set; }

        public string GetResponsableEmail
        {
            get {
                try
                {
                    if (Responsable != null)
                        return Responsable.Email;
                }
                catch (Exception)
                {}
                return ""; 
            }
        }


        public virtual ICollection<CompteBanqueCommerciale> Agents { get; set; }
        public virtual ICollection<IHMStructure> Composants { get; set; }
        public virtual ICollection<BanqueClient> Clients { get; set; }
        public virtual ICollection<DossierStructure> GetDossiers { get; set; } = null;
        public virtual ICollection<XtraRole> Roles { get; set; }
        public virtual ICollection<Structure> Structures { get; set; }
        [Display(Name ="Lire toutes les références banque")]
        public bool LireTouteReference { get; set; }

        public virtual int BanqueId(ApplicationDbContext db)
        {
            return 0;
        }

        public virtual string BanqueName(ApplicationDbContext db)
        {
            return "";
        }


        public string GetTypeElt()
        {
            return (this.GetType().Name == "Structure" ? TypeStructure != null ? TypeStructure.Intitule : GetType().Name : GetType().Name);
        }

        public virtual Object[] ResumeTransfertBanque(List<Dossier> _dossiers, string Structure = null, string Gestionnaire = null, string Client = null, int[] etat = null, double? Montant1 = null
           , double? Montant2 = null, string Devise = "", string Fourniseur = "", int? Delai1 = null, int? Delai2 = null, int? JourDepot1 = null, int? MoisDepot1 = null
           , int? AnneeDepot1 = null, int? JourDepot2 = null, int? MoisDepot2 = null, int? AnneeDepot2 = null
           , int? JourTraitement1 = null, int? MoisTraitement1 = null, int? EtatDuDossier = null
           , int? AnneeTraitement1 = null, int? JourTraitement2 = null, int? MoisTraitement2 = null, int? AnneeTraitement2 = null
           , string Cat = "", bool DFX = false, bool FP = false, bool Ref = false,string TypeTransfert="")
        {
            List<ElementResumeTransfert> elts = new List<ElementResumeTransfert>();
            string[] mois = new string[] { "Jeanvier", "Févier", "Mars", "Avril", "Mai", "Juin", "Juillet", "Aoùt", "Septembre", "Octobre", "Novembre", "Decembre" };
            string serachString = "";
            if (!string.IsNullOrEmpty(Structure))
            {
                try
                {
                    serachString += $";Structure:{Structure}";
                    _dossiers = _dossiers.Where(d => d.Site.Nom == Structure).ToList();
                }
                catch (Exception)
                { }
            }
            if (!string.IsNullOrEmpty(Gestionnaire))
            {
                try
                {
                    serachString += $";Gestionnaire:{Gestionnaire}";
                    List<Dossier> tmp = new List<Dossier>();
                    _dossiers.ForEach(d =>
                    {
                        if(d.GestionnaireName == Gestionnaire)
                            tmp.Add(d);
                    });
                    _dossiers = tmp.ToList();
                    tmp = null;
                }
                catch (Exception)
                { }
            }
            if (!string.IsNullOrEmpty(Client))
            {
                try
                {
                    serachString += $";Client:{Client}";
                    List<Dossier> tmp = new List<Dossier>();
                    _dossiers.ForEach(d =>
                    {
                        if (d.GetClient == Client)
                            tmp.Add(d);
                    });
                    _dossiers = tmp.ToList();
                    tmp = null;
                }
                catch (Exception)
                { }
            }
            if (!string.IsNullOrEmpty(TypeTransfert))
            {
                try
                {
                    serachString += $";TypeTransfert:{TypeTransfert}";
                    List<Dossier> tmp = new List<Dossier>();
                    _dossiers.ForEach(d =>
                    {
                        if (!d.Apurement)
                            tmp.Add(d);
                    });
                    _dossiers = tmp.ToList();
                    tmp = null;
                }
                catch (Exception)
                { }
            }
            
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

    }
}