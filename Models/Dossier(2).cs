using e_apurement.Models.Fonctions;
using eApurement.Models;
using eApurement.Models.Fonctions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using DisplayNameAttribute = System.ComponentModel.DisplayNameAttribute;

namespace e_apurement.Models
{
    [DefaultProperty("RefInterne")]
    [Table("Dossier")]
    public class Dossier : IDelateCustom
    {
        public bool Remove(ApplicationDbContext db)
        {
            try
            {
                if (this.DeclarImport != null)
                    this.DeclarImport.Remove(db);
                if (this.DomicilImport != null)
                    this.DomicilImport.Remove(db);
                if (this.LettreEngage != null)
                    this.LettreEngage.Remove(db);
                if (this.QuittancePay != null)
                    this.QuittancePay.Remove(db);
                if (this.DocumentTransport != null)
                    this.DocumentTransport.Remove(db);
                try
                {
                    foreach (var j in this.Justificatifs)
                    {
                        j.Remove(db);
                    }
                }
                catch (Exception)
                { }

                try
                {
                    foreach (var d in this.DocumentAttaches)
                    {
                        d.Remove(db);
                    }
                }
                catch (Exception)
                { }
                db.SaveChanges();

                return true;
            }
            catch (Exception)
            { }
            return false;
        }


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Dossier_Id { get; set; }

        private int num;
        [DisplayName("Index")]
        [Browsable(false)]
        public int NumeroDossier
        {
            get { return num; }
            set { num = value; }
        }

        public string Intitulé
        {
            get
            {
                return $"Dossier_{ClientId}_{Dossier_Id}";
            }
        }

        [Display(Name ="Nature de l'importation")]
        [Required]
        public NatureOperation NatureOperation { get; set; }


        /// <summary>
        /// Pour tout le workflow - Client -Banque -STF
        /// ******* Les succes du workflow *****
        ///  0 = dossier au brouillon. 1 = dossier soumis est au niveau de l'angence (reçu, mais pas en cours de traitement)
        ///  2= dossier en cours de traitement agence. 3= dossier transferé au service de transfert (reçu, mais pas encours de traitement)
        ///  4= dossier en cours de traitement service transfert. 5 dossier envoyé BEAC. 6 dossier accordé. 7 juste apres etre accordé, en attente pendant 3 jours
        ///  8= dossier à envoyer BEAC 
        /// 
        /// ****** Les erreurs du workflow ******
        /// 20 = dossier erroné au niveau du client. 21 = erreur au niveau de l'agence, retour au client. 
        /// 22 = dossier erroné chef d'agence, retour gestionnaire. 23 = dossier erroné service transfert, retour agence
        /// 
        /// 40 = dossier echus
        /// 
        /// 50 = dossier à apurer. 51 = dossier apuré. 52=archivé. 53 = supprimer
        /// </summary>
        [DisplayName("Statut du dossier")]
        public int? EtapesDosier { get; set; }

        public string[] GetEtapDossier(int? eta=null)
        {
            var tmp = eta!=null?eta:EtapesDosier;

            switch (tmp)
            {
                case null: return new string[] { "Rappelé", "cornflowerblue", "dossier au brouillon.", "" };
                case 0: return new string[] { "En attente d'envoie à l'agence", "cornflowerblue", "dossier au brouillon.", "" };
                case 1: return new string[] { "Dossiers reçus", "cornflowerblue", "dossier soumis est au niveau de l'angence ", "En attente référence douane" };
                case 2: return new string[] { "Dossiers en cours de traitement agence", "darkgoldenrod", "dossier en cours de traitement agence.", "" };
                case 3: return new string[] { "Dossier envoyé à la conformité", "#ffd800", "Dossiers envoyé à la conformité", "Envoyé à la conformité" };
                case 4: return new string[] { "Dossier en cours d'analyse conformité", "cornflowerblue", "dossier transferé à la conformité", "En cours d'analyse conformité" };
                case 5: return new string[] { "Dossier envoyé au service transfert", "#ffd800", "dossier envoyé au service transfert", "Envoyé au service transfert" };
                case 6: return new string[] { "Dossier en cours de traitement service transfert", "blue", "dossier en attante référence banque.", "En cours de traitement service de transfert" };
                case 7: return new string[] { "Dossier à envoyer à la BEAC", "#ffd800", "dossier à envoyer à la BEAC.", "A envoyer à la BEAC" };
                case 8: return new string[] { "Dossiers en cours de traitement BEAC", "forestgreen", "dossiers en cours de traitement BEAC.", "En cours de traitement BEAC" };
                case 9: return new string[] { "Saisie en cours", "green", "saisie en cours.", "" };
                case 10: return new string[] { "Dossiers accordés", "green", "Executé", "" };
                case 11: return new string[] { "à apurer", "crimson", "dossier à apurer" };
                case 12: return new string[] { "apuré", "saddlebrown", "dossier apuré" };
                case 13: return new string[] { "echu", "khaki", "dossier echu" };
                case 14: return new string[] { "Archivé", "saddlebrown", "dossier archivé" };
                case -2: 
                case -3: 
                case -1:
                    return new string[] { "Rejet", "red", "dossier rejeté" };
                case 15: return new string[] { "Supprimé", "black", "dossier supprimer" };
                default:
                    return new string[] { "", "white", "" };
            }
        }
        public ICollection<StatutDossier> StatusDossiers { get; set; }

        [Display(Name ="Intitulé")]
        public string Intitule { get; set; }

        [Display(Name = "Numéro du dossier")]
        //[Required]
        public string RefInterne { get; set; }

        [Display(Name = "Date reception")]
        public DateTime? DateCreaBanque {
            get {
                if (ReferenceExterne != null)
                    return ReferenceExterne.DateReception;
                else
                    return DateCreationApp;
            }
        }

        [Display(Name = "Dernière modif")]
        public DateTime DateModif { get; set; }

        [DisplayName("Nombre de factures")]
        public int NbreJustif { get; set; }

        #region Instruction

        [DisplayName("Site")]
        [ForeignKey("Site")]
        public int IdSite{ get; set; }
        public virtual Agence Site { get; set; }

        public string GestionnaireId
        {
            get {
                try
                {
                    return this.Client.Banques.FirstOrDefault().IdGestionnaire;
                }
                catch (Exception)
                {}
                return ""; 
            }
        }
        public string EstGestionnaire(ApplicationDbContext db,string idGestionnaire)
        {
            try
            {
                return this.Client.Banques.FirstOrDefault().IdGestionnaire;
            }
            catch (Exception)
            { }
            return "";
        }

        private double monatant;
        //[DataType(DataType.Currency)]
        public double Montant
        {
            get { return monatant; }
            set { monatant = value; }
        }

        [ForeignKey("Fournisseur")]
        public int? FournisseurId { get; set; }
        public virtual Fournisseurs Fournisseur { get; set; }

        private DateTime dateDepot;
        [Display(Name="Date de dépot à la banque")]
        public DateTime DateDepotBank
        {
            get { return dateDepot; }
            set { dateDepot = value; }
        }

        private DateTime datecreaApp;

        [DisplayName("Date de la numérisation")]
        public DateTime DateCreationApp
        {
            get { return datecreaApp; }
            set { datecreaApp = value; }
        }

        private string utilisateur;
        [DisplayName("Numérisée par")]
        public string ApplicationUser
        {
            get {   return utilisateur; }
            set { utilisateur = value; }
        }

        #endregion

        [Required(ErrorMessage = "Le champs dévise est obligatoire !")]
        [ForeignKey("DeviseMonetaire")]
        public int  DeviseMonetaireId { get; set; }
        public bool Traité { get; set; }

        [Display(Name ="Marchandise arrivée")]
        public bool MarchandiseArrivee { get; set; }

        public virtual DeviseMonetaire DeviseMonetaire { get; set; }

        [NotMapped]
        public int CompteurJustif { get; set; }

        private string oidClient;
        public string OidEntreprise
        {
            get {
                try
                {
                    if (Client!=null)
                    {
                        oidClient = Client.Id.ToString(); 
                    }
                }
                catch (Exception)
                {}
                return oidClient;
            }
        }

        [DisplayName("Reference banque")]
        [ForeignKey("ReferenceExterne")]
        public int? ReferenceExterneId { get; set; } = null;
        public virtual ReferenceBanque ReferenceExterne { get; set; }

        public virtual ICollection<Justificatif> Justificatifs { get; set; } = new List<Justificatif>();

        #region Documents obligatoires

        //[DisplayName("Document de la dounane")]
        //public DocumentAttache DocDouane { get; set; }

        private DocumentAttache declarImport;
        [DisplayName("Déclaration d'importation")]
        public DocumentAttache DeclarImport
        {
            get { return declarImport; }
            set {
                try
                {
                    if (value!=null)
                    {
                        value.TypeDocumentAttaché = TypeDocumentAttaché.DeclarImportBien;
                    }
                }
                catch (Exception)
                {}
                declarImport = value; 
            }
        }

        private DocumentAttache domicilImport;
        [DisplayName("Domiciliation d'importation")]
        public DocumentAttache DomicilImport
        {
            get { return domicilImport; }
            set {
                try
                {
                    if (value != null)
                    {
                        value.TypeDocumentAttaché = TypeDocumentAttaché.DomicilImport;
                    }
                }
                catch (Exception)
                { }
                domicilImport = value; 
            }
        }

        private DocumentAttache lettreEngag;
        [DisplayName("Lettre d'engagement")]
        public DocumentAttache LettreEngage
        {
            get { return lettreEngag; }
            set {
                try
                {
                    if (value != null)
                    {
                        value.TypeDocumentAttaché = TypeDocumentAttaché.LettreEngagement;
                    }
                }
                catch (Exception)
                { }
                lettreEngag = value; 
            }
        }

        private DocumentAttache quittancePay;
        [DisplayName("Quittance de paiement")]
        public DocumentAttache QuittancePay
        {
            get { return quittancePay; }
            set {
                try
                {
                    if (value != null)
                    {
                        value.TypeDocumentAttaché = TypeDocumentAttaché.QuittancePayement;
                    }
                }
                catch (Exception)
                { }
                quittancePay = value; 
            }
        }

        private DocumentAttache documentTransport;
        [DisplayName("Quittance de paiement")]
        public DocumentAttache DocumentTransport
        {
            get { return documentTransport; }
            set
            {
                try
                {
                    if (value != null)
                    {
                        value.TypeDocumentAttaché = TypeDocumentAttaché.DocumentTransport;
                    }
                }
                catch (Exception)
                { }
                documentTransport = value;
            }
        }


        #endregion

        [DisplayName("Autre document")]
        public ICollection<DocumentAttache> DocumentAttaches { get; set; } = new List<DocumentAttache>();

        private int nbpiece;
        [DisplayName("Nombre de pièces jointes")]
        public int NbPiece
        {
            get
            {
                try
                {
                    nbpiece = 0;
                    Justificatifs.ToList().ForEach(j =>
                    {
                        nbpiece += j.NbrePieces;
                    });
                }
                catch (Exception)
                { }
                return nbpiece;
            }
        }

        private int taille;

        public int TailleFiles
        {
            get { return taille; }
        }
       
        [Required(ErrorMessage = "Le champs entreprise est obligatoire !")]
        [ForeignKey("Client")]
        public int ClientId { get; set; }
        public virtual Client Client { get; set; }

        /// <summary>
        /// Une seule image est prise en charge
        /// </summary>
        public virtual ICollection<ImageInstruction> GetImageInstructions { get; set; }

        public string GetImageInstruction()
        {
            try
            {
                if (GetImageInstructions.Count > 0)
                    return GetImageInstructions.ToList()[0].GetImage();
            }
            catch (Exception)
            { }

            return "#";
        }


        private Dossier_StatutDossier statuDossier;
        [DisplayName("Statut du dossier")]
        public Dossier_StatutDossier StatutDossier
        {
            get {
                try
                {
                    if (GetStatutDossiers !=null && GetStatutDossiers.Count>0)
                    {
                        statuDossier = GetStatutDossiers.OrderByDescending(s => s.Date).ToList()[0];
                    }
                }
                catch (Exception)
                {}
                if (statuDossier == null)
                    return new Dossier_StatutDossier()
                    {
                        Date=DateTime.Now,
                        Dossier=this
                    };
                return statuDossier; 
            }
        }

        public virtual ICollection<Dossier_StatutDossier> GetStatutDossiers { get; set; }


        private char terminé;
        /// <summary>
        /// Vérifie si le processus de l'enregistrement du dossier de transfert est terminer
        /// L'utilisateur dois Appuyer sur le button terminer pour valider la transaction
        /// </summary>
        /// 
        public char TerminerEnregistrement
        {
            get { return terminé; }
            set { terminé = value; }
        }

        public virtual ICollection<DossierNotification> Notifications { get; set; }

        private int percentage;
        [Browsable(false)]
        public int Percentage
        {
            get {
                try
                {
                    percentage = 0;
                    if (Justificatifs != null && Justificatifs.Count > 0)
                    {
                        foreach (var item in Justificatifs)
                        {
                            try
                            {
                                percentage += (int)item.Percentage;

                            }
                            catch (Exception)
                            {}                        
                        }
                        try
                        {
                            percentage = (int)(percentage / Justificatifs.Count);

                        }
                        catch (Exception)
                        {}                    }
                    if (DeclarImport != null)
                    {
                        try
                        {
                            percentage += (int)DeclarImport.Percentage;

                        }
                        catch (Exception)
                        {}                    
                    }
                    if (this.DomicilImport != null)
                    {
                        try
                        {
                            percentage += (int)DomicilImport.Percentage;

                        }
                        catch (Exception)
                        {}                    }
                    if (this.LettreEngage != null)
                    {
                        try
                        {
                            percentage += (int)LettreEngage.Percentage;

                        }
                        catch (Exception)
                        {}                    
                    }
                    if (this.QuittancePay != null)
                    {
                        try
                        {
                            percentage += (int)QuittancePay.Percentage;

                        }
                        catch (Exception)
                        {}                    
                    }
                    if (this.GetImageInstruction() != "#")
                    {
                        percentage += 100;
                    }
                    if (this.Get_DocumentTransport != "#")
                    {
                        try
                        {
                            percentage += (int)DocumentTransport.Percentage;
                        }
                        catch (Exception)
                        {}
                    }
                    percentage = percentage / 7;
                }
                catch (Exception)
                {}
                if(percentage ==0)
                return percentage+1;
                if (percentage >= 100) return 100;
                else
                    return percentage; 

            }
        }

        string infomesage;
        [Browsable(false)]
        public string InfoPercentage {
            get
            {
                infomesage = "";
                try
                {
                    short s = 1;
                    if (Justificatifs != null && Justificatifs.Count > 0)
                    {
                        foreach (var item in Justificatifs)
                        {
                            if (!string.IsNullOrEmpty(item.InfoPercentage))
                            {
                                infomesage += s + "- " + item.InfoPercentage + "; ";
                                s++;
                            }
                        }
                    }
                    else { infomesage += s+"- Manque de factures; "; s++; }
                    if (DeclarImport != null)
                    {
                        if (!string.IsNullOrEmpty(DeclarImport.InfoPercentage))
                        {
                            infomesage += s + "- " + DeclarImport.InfoPercentage + "; "; s++;
                        }
                    }
                    else
                    {
                        infomesage += s + "- Manque de declaration d'importation; ";s++;
                    }
                    if (this.DomicilImport != null)
                    {
                        if (!string.IsNullOrEmpty(DomicilImport.InfoPercentage))
                        {
                            infomesage += s + "- " + DomicilImport.InfoPercentage + "; "; s++;

                        }                    }
                    else
                    {
                        infomesage += s + "- Manque de domiciliation d'importation; "; s++;
                    }
                    if (this.LettreEngage != null)
                    {
                        if (!string.IsNullOrEmpty(LettreEngage.InfoPercentage))
                        {
                            infomesage += s + "- " + LettreEngage.InfoPercentage + "; "; s++;

                        }                    
                    }
                    else
                    {
                        infomesage += s + "- Manque de la lettre d'engagement; "; s++;
                    }
                    if (this.QuittancePay != null)
                    {
                        if (!string.IsNullOrEmpty(QuittancePay.InfoPercentage))
                        {
                            infomesage += s + "- " + QuittancePay.InfoPercentage + "; "; s++;

                        }                    
                    }
                    else
                    { infomesage += s + "- Manque de la quittance; "; s++; }
                    if (this.GetImageInstruction() == null)
                    {
                        infomesage +=  s+"- Manque du fichier d'instruction; " ;s++;
                    }
                }
                catch (Exception)
                { }
                //infomesage += "<ol />";
                return infomesage;
            }
        }

        [DisplayName("Pourcentage")]
        public string PercentageToString
        {
            get { return Percentage.ToString()+"%"; }
        }


        private byte niveauEnreg;
        [Browsable(false)]
        public byte NiveauEnregistrement
        {
            get {
                if (Justificatifs != null && Justificatifs.Count > 0)
                {
                    niveauEnreg = 1;
                }
                if (DeclarImport != null)
                {
                    niveauEnreg = 2;
                }
                if (this.DomicilImport != null)
                {
                    niveauEnreg = 3;
                }
                if (this.LettreEngage != null)
                {
                    niveauEnreg = 4;
                }
                if (this.QuittancePay != null)
                {
                    niveauEnreg =5;
                }
                return niveauEnreg; 
            }

        }
        
        [NotMapped]
        public IDictionary<string, bool> Permissions { get; set; }
        public bool GetPermissions(string type)
        {
            try
            {
                return Permissions[type];
            }
            catch (Exception)
            { }

            return false;
        }


        [Browsable(false)]
        public bool EstComplet
        {
            get {
                if (Percentage >= 100)
                    return true;
                else return false;
            }
        }


        #region Les etsts
        public bool Apure
        {
            get {
                //try
                //{
                //    if (StatutDossier.StatutDossier.EtatDossier == EtatDossier.Apuré)
                //        return true;
                //}
                //catch (Exception)
                //{}
                return false; 
            }
        }

        public bool Echu
        {
            get
            {
                //try
                //{
                //    if (StatutDossier.StatutDossier.EtatDossier == EtatDossier.Echus)
                //        return true;
                //}
                //catch (Exception)
                //{ }
                return false;
            }
        }


        public bool Archive
        {
            get
            {
                //try
                //{
                //    if (StatutDossier.StatutDossier.EtatDossier == EtatDossier.Archivé)
                //        return true;
                //}
                //catch (Exception)
                //{ }
                return false;
            }
        }

        public bool Encours
        {
            get
            {
                //try
                //{
                //    if (StatutDossier.StatutDossier.EtatDossier == EtatDossier.Encours)
                //        return true;
                //}
                //catch (Exception)
                //{ }
                return false;
            }
        }

        public bool Groupe
        {
            get
            {
                try
                {
                    if (ReferenceExterne.Dossiers.Count > 1)
                        return true;
                }
                catch (Exception)
                { }
                return false;
            }
        }

        #endregion

        #region Get Numeriseds files
        public string Get_DeclarImport
        {
            get {
                try
                {
                    if (DeclarImport != null)
                        return DeclarImport.GetImageDocumentAttache().GetImage();
                }
                catch (Exception)
                { }
                return "#"; 
            }
        }

        public string Get_DomicilImport
        {
            get
            {
                try
                {
                    if (DomicilImport != null)
                        return DomicilImport.GetImageDocumentAttache().GetImage();
                }
                catch (Exception)
                { }
                return "#";
            }
        }

        //public string Get_DocDouane
        //{
        //    get
        //    {
        //        try
        //        {
        //            //if (DomicilImport != null)
        //            //    return DocDouane.GetImageDocumentAttache().GetImage();
        //        }
        //        catch (Exception)
        //        { }
        //        return "#";
        //    }
        //}

        public string Get_LettreEngage
        {
            get
            {
                try
                {
                    if (LettreEngage != null)
                        return LettreEngage.GetImageDocumentAttache().GetImage();
                }
                catch (Exception)
                { }
                return "#";
            }
        }

        public string Get_QuittancePay
        {
            get
            {
                try
                {
                    if (QuittancePay != null)
                        return QuittancePay.GetImageDocumentAttache().GetImage();
                }
                catch (Exception)
                { }
                return "#";
            }
        }

        public string Get_DocumentTransport
        {
            get
            {
                try
                {
                    if (DocumentTransport != null)
                        return DocumentTransport.GetImageDocumentAttache().GetImage();
                }
                catch (Exception)
                { }
                return "#";
            }
        }


        #endregion

        protected void OnSaving()
        {
            try
            {
                try
                {
                    if (Client == null)
                    {
                       // var entrePriseUse = (SecuritySystem.CurrentUser as CompteClient).Client;
                       // Client = Session.Query<Client>().FirstOrDefault(e => e.Equals(entrePriseUse)) as Client;
                    }
                }
                catch (System.Exception)
                { }

                if (NumeroDossier == 0)
                {
                    int num = 1;

                    try
                    {
                        Dossier person = null;
                        try
                        {
                            //person = Session.Query<Dossier>().OrderByDescending(p => (p as Dossier).NumeroDossier).FirstOrDefault() as Dossier;

                        }
                        catch (Exception e)
                        { }

                        if (person != null)
                        {
                            num = person.NumeroDossier;
                            try
                            {
                                num++;
                            }
                            catch (Exception)
                            { }
                        }
                    }
                    catch (Exception)
                    {
                    }
                    finally
                    {
                        NumeroDossier = num;
                    }
                }
            }
            catch (Exception)
            {}
        }

    }

    public enum NatureOperation
    {
        Bien,
        Service
    }

    public enum EtatDossier
    {

        [Display(Name ="Dossier encours")]
        Encours,
        [Display(Name = "Dossiers à soumettre")]
        ASoumettre,
        [Display(Name = "Dossier soumis")]
        Soumis,
        [Display(Name = "Dossier apuré")]
        Apuré,
        [Display(Name = "Dossier à apurer")]
        AApurer,
        [Display(Name = "Dossier échu")]
        Echus,
        [Display(Name = "Archivé")]
        Archivé,
        [Display(Name = "Dossier supprimé")]
        Supprimé,
        Brouillon
    }

    
}