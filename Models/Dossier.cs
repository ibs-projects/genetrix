using genetrix.Models.Fonctions;
using genetrix.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using DisplayNameAttribute = System.ComponentModel.DisplayNameAttribute;
using System.Globalization;

namespace genetrix.Models
{
    [DefaultProperty("RefInterne")]
    [Table("Dossier")]
    public class Dossier : IDelateCustom
    {
        DateTime dateNow;
        public Dossier()
        {
            dateNow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "W. Central Africa Standard Time");
        }
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
            catch (Exception e)
            { }
            return false;
        }

        #region Numerisation
        public virtual string PrintInstruction()
        {
            return @" <style>

        table td {
            width: 50%;
            text-align: left;
            vertical-align: top;
            padding: 10px;
        }
        table {
            border-collapse: collapse;
        } 

        #pan-nature {
            border: 0.3px solid rgb(187, 187, 187, 0.30);
            padding: 15px;
            padding-left: 6em;
        }

        input[type='radio'], label[class='form-check-label'] {
            font-size: 1.5em;
        }

        .separation {
            margin-top: 1em;
            margin-bottom: 1em;
        }

        #table-domici {
            width: 100%;
        }

        .item {
            font-weight: bold;
            text-align: center;
            text-transform: uppercase;
            color: #2689a6;
        }

        .td-data {
            padding: 10px;
            overflow: hidden;
        }

        #div-je-soussi{
            display:flex;
        }
        #div-je-soussi > div{
            height:55px;
            text-align:right;
            margin-top:20px;
        }
    </style>
";
        }
        public virtual string PrintDomiciliation()
        {
            return @"
                    <style>
        table {
            width: 100%;
            border-collapse: collapse;
        }
        table td {
            width: 50%;
            text-align: left;
            vertical-align: top;
            padding: 10px;
        }

        #pan-nature {
            border: 0.3px solid rgb(187, 187, 187, 0.30);
            padding: 15px;
            padding-left: 6em;
        }

        input[type='radio'], label[class='form-check-label'] {
            font-size: 1.5em;
        }

        .item {
            font-weight: bold;
            text-align: center;
            text-transform: uppercase;
            color: #2689a6;
        }

        .td-data {
            padding: 10px;
            overflow: hidden;
        }

            input {
                width: 80%;
                height: 40px;
                margin: 0px;
                padding-left: 10px;
                border-radius:15px;
                margin-bottom:5px;
            }

        .row{
            display:flex;
        }
        .col-6{
            flex:1;
        }
        table th{
            width:25%;
        }
        table th >p{
            margin-top:-70px;
            text-align:center;
        }
        .div-je-soussi{
                display:flex;
            }
            .div-je-soussi > div{
                text-align:right;
            }
    </style>

            ";
        }
        public virtual string PrintDeclaration()
        {
            return @"
            <style>

        table td {
            width: 50%;
            text-align: left;
            vertical-align: top;
            padding: 10px;
        }

        table {
            border-collapse: collapse;
            width:100%;
        }
        .row{
            display:flex;
        }
        .col-6{
            flex:1;
        }
        .col-4{
            flex:1;
            margin-right:3px;
        }
        .col-3{
            flex:1;
            margin-right:3px;
        }
        #pan-nature {
            border: 0.3px solid rgb(187, 187, 187, 0.30);
            padding: 15px;
            padding-left: 6em;
        }
        input{
            width:90%;
            border-radius:15px;
            margin-bottom:8px;
            padding:6px;
            height:30px;
            font-weight:bold;
        }

        .item {
            font-weight: bold;
            text-align: center;
            text-transform: uppercase;
            color: #2689a6;
        }

        .td-data {
            padding: 10px;
            overflow: hidden;
        }

        #div-je-soussi {
            display: flex;
        }

            #div-je-soussi > div {
                height: 55px;
                text-align: right;
                margin-top: 20px;
            }
            th > p{
                margin-top:-70px;
                text-align:center;
            }
            table th{
                 width:25%;
            }
    </style>
            ";
        }
        public virtual string PrintLettreEngagement(string genre,string NonComplet,
            string fonction,string entreprise,string banque,string vilePays_fournisseur,
            string dateLivraison,string ville,string dateJour)
        {
            return "";
        }
        #endregion

        #region Info domiciliation
        public virtual string Description { get; set; }

        [Display(Name = "Nomenclature douanière:")]
        public string NomenClatureDouane { get; set; }

        [Display(Name = "Echéance fixée pour le paiement:")]
        public string EcheancePaiement { get; set; }
        string _importaVille;
        [NotMapped]
        [Display(Name = "Ville Importateur")]
        public string ImportaVille
        {
            get {
                try
                {
                    if (Importateur != null)
                        return Importateur.Ville;
                }
                catch (Exception)
                {}
                return ""; 
            }
            set { _importaVille = value; }
        }
        
        string _importaPays;
        [NotMapped]
        [Display(Name = "Pays Importateur")]
        public string ImportaPays
        {
            get {
                try
                {
                    if (Importateur != null)
                        return Importateur.Pays;
                }
                catch (Exception)
                {}
                return ""; 
            }
            set { _importaPays = value; }
        }
        
        string _importaNom;
        [NotMapped]
        [Display(Name = "Nom ou raison sociale")]
        public string ImportaNom
        {
            get {
                try
                {
                    if (Importateur != null)
                        return Importateur.NomComplet;
                }
                catch (Exception)
                {}
                return ""; 
            }
            set { _importaNom = value; }
        }

        public string ImportaNomAdress { get => ImportaNom + "/" + ImportaAdresse; }

        string _importaNumInscri;
        [NotMapped]
        [Display(Name = "Numéro d'inscription au registre de commerce")]
        public string ImportaNumInscri
        {
            get
            {
                try
                {
                    if (Importateur != null)
                        return Importateur.NumInscri;
                }
                catch (Exception)
                { }
                return "";
            }
            set { _importaNumInscri = value; }
        }
        
        string _importaProfession;
        [NotMapped]
        [Display(Name = "Profession de l'importateur")]
        public string ImportaProfession
        {
            get
            {
                try
                {
                    if (Importateur != null)
                        return Importateur.Profession;
                }
                catch (Exception)
                { }
                return "";
            }
            set { _importaProfession = value; }
        }
        
        string _importaImmatri;
        [NotMapped]
        [Display(Name = "Immatriculation statistique de l'importateur")]
        public string ImportaImmatri
        {
            get
            {
                try
                {
                    if (Importateur != null)
                        return Importateur.Immatriculation;
                }
                catch (Exception)
                { }
                return "";
            }
            set { _importaImmatri = value; }
        }
        
        string _importaAdresse;
        [NotMapped]
        [Display(Name = "Adresse de l'importateur")]
        public string ImportaAdresse
        {
            get
            {
                try
                {
                    if (Importateur != null)
                        return Importateur.Adresse;
                }
                catch (Exception)
                { }
                return "";
            }
            set { _importaAdresse = value; }
        }
        
        string _importateurCodeAgr;
        [NotMapped]
        [Display(Name = "Code d'agrément de l'importateur")]
        public string ImportateurCodeAgr
        {
            get
            {
                try
                {
                    if (Importateur != null)
                        return Importateur.CodeAgrement;
                }
                catch (Exception)
                { }
                return "";
            }
            set { _importateurCodeAgr = value; }
        }
        
        string _importateurPhone;
        [NotMapped]
        [Display(Name = "Téléphone de l'importateur")]
        public string ImportateurPhone
        {
            get
            {
                try
                {
                    if (Importateur != null)
                        return Importateur.Telephone;
                }
                catch (Exception)
                { }
                return "";
            }
            set { _importateurPhone = value; }
        }
        
        string _importateurMail;
        [NotMapped]
        [Display(Name = "Email de l'importateur")]
        public string ImportateurMail
        {
            get
            {
                try
                {
                    if (Importateur != null)
                        return Importateur.Email;
                }
                catch (Exception)
                { }
                return "";
            }
            set { _importateurMail = value; }
        }
        
        string _vendeurNom;
        [NotMapped]
        [Display(Name = "Nom du vendeur")]
        public string VendeurNom
        {
            get
            {
                try
                {
                    if (Fournisseur != null)
                        return Fournisseur.Nom;
                }
                catch (Exception)
                { }
                return "";
            }
            set { _vendeurNom = value; }
        }
        
        string _vendeurAdresse;
        [NotMapped]
        [Display(Name = "Adresse du vendeur")]
        public string VendeurAdresse
        {
            get
            {
                try
                {
                    if (Fournisseur != null)
                        return Fournisseur.Adresse;
                }
                catch (Exception)
                { }
                return "";
            }
            set { _vendeurAdresse = value; }
        }

        public string VendeurNomAdress { get => VendeurNom + "/" + VendeurAdresse; }

        string _vendeurVille;
        [NotMapped]
        [Display(Name = "Ville du vendeur")]
        public string VendeurVille
        {
            get
            {
                try
                {
                    if (Fournisseur != null)
                        return Fournisseur.Ville;
                }
                catch (Exception)
                { }
                return "";
            }
            set { _vendeurVille = value; }
        }
        
        string _vendeurPhone;
        [NotMapped]
        [Display(Name = "Téléphone du vendeur")]
        public string VendeurPhone
        {
            get
            {
                try
                {
                    if (Fournisseur != null)
                        return Fournisseur.Tel1;
                }
                catch (Exception)
                { }
                return "";
            }
            set { _vendeurPhone = value; }
        }
        
        string _vendeurFax;
        [NotMapped]
        [Display(Name = "Télécopie/Fax du vendeur")]
        public string VendeurFax
        {
            get
            {
                try
                {
                    if (Fournisseur != null)
                        return Fournisseur.Fax;
                }
                catch (Exception)
                { }
                return "";
            }
            set { _vendeurFax = value; }
        }
        
        string _vendeurPaysHorsCemac;
        [NotMapped]
        [Display(Name = "Pays Hors zone CEMAC du vendeur")]
        public string VendeurPaysHorsCemac
        {
            get
            {
                try
                {
                    if (Fournisseur != null)
                        return Fournisseur.Pays;
                }
                catch (Exception)
                { }
                return "";
            }
            set { _vendeurPaysHorsCemac = value; }
        }

        [Display(Name = "Pays d'origine")]
        public string VendeurPaysOrigin { get; set; }
        
        [Display(Name = "Pays de provenance")]
        public string VendeurPaysProvenance { get; set; }

        [Display(Name = "Lieu de dédouagnement")]
        public string LieuDedouagnement { get; set; }
        [Display(Name = "Pays de provenance")]
        public string PaysProv { get; set; }
        [Display(Name = "Pays d'origine")]
        public string PaysOrig { get; set; }

        public string RefDomiciliation { get; set; }
        public DateTime? DateDomiciliation { get; set; }

        public string RefDateDomici
        {
            get { 
                return RefDomiciliation+" /"+DateDomiciliation!=null?DateDomiciliation.Value.ToString("dd/MM/yyyy"):""; 
            }
        }


        [Display(Name = "Banque domiciliataire")]
        public string BanqueDomi { get; set; }

        [Display(Name = "N° Facture pro forma")]
        public string NumFacturePro { get; set; }
        [Display(Name = "Date Facture pro forma")]
        public DateTime? DateFacturePro { get; set; }
        [Display(Name = "Modalité règlement /")]
        public string ModalReglement { get; set; }
        [Display(Name = "Terme de vente")]
        public string TermeVente { get; set; }
        [Display(Name = "Valeur FOB (devises)")]
        public string ValeurFOB { get; set; } 
        
        [Display(Name = "Taux de change")]
        public decimal TauxChange { get; set; }
        
        [Display(Name = "Valeur en CFA")]
        public decimal ValeurCFA { get; set; }

        [Display(Name = "Valeur en devise")]
        public decimal ValeurDevise { get; set; }

        [Display(Name = "Pos. Tarifaire /")]
        public decimal PosTarif { get; set; }

        [Display(Name = "FOB en devise")]
        public decimal FOBDevise { get; set; }

        [Display(Name = "Taxe d'inspection")]
        public decimal TaxeInsp { get; set; }

        [Display(Name = "Chèque n° / virement : ")]
        public string ChequeNum { get; set; }

        [Display(Name = "Du")]
        public DateTime? ChequeDate { get; set; }

        [Display(Name = "Banque")]
        public string ChequeBanque { get; set; }

        [Display(Name = "Montant CFA")]
        public decimal ChequeMontantCFA { get; set; }
        #endregion

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Dossier_Id { get; set; }

        internal IDictionary<string,string> GetItemGenerationInstruction()
        {
            Dictionary<string, string> items = new Dictionary<string, string>();
            items.Add("raisonsociale", this.GetClient);
            items.Add("telcl", this.TelClient);
            items.Add("profcl", this.ProfessionClient);
            items.Add("payscl", this.PaysClient);
            items.Add("motifop", this.Motif);
            items.Add("devisemo", this.DeviseToString);
            items.Add("montantdevise", this.MontantString);
            items.Add("montantxaf", this.MontantCVstring);
            items.Add("montantlettre", this.MontantEnLettre);
            items.Add("_benef", this.GetFournisseur);
            items.Add("adressebenef", this.AdresseFournisseur);
            items.Add("banquebenef", this.NomBanqueBenf);
            items.Add("adressebanquebenf", this.AdresseBanqueBenf);
            //Code Etablissement - client
            if (string.IsNullOrEmpty(CodeEtablissement))
                CodeEtablissement = "XXXXX";
            if (CodeEtablissement.Length < 5) CodeEtablissement=CodeEtablissement.PadRight(5, 'X');
            try
            {
                items.Add("_@1", this.CodeEtsClient[0].ToString()) ;
                items.Add("_@2", this.CodeEtsClient[1].ToString()) ;
                items.Add("_@3", this.CodeEtsClient[2].ToString()) ;
                items.Add("_@4", this.CodeEtsClient[3].ToString()) ;
                items.Add("_@5", this.CodeEtsClient[4].ToString()) ;
            }
            catch (Exception)
            {}

            //Code Agence - client
            string _CodeElt = CodeAgence;
            if (string.IsNullOrEmpty(CodeAgence))
                _CodeElt = "XXXXX";
            if (_CodeElt.Length < 5)
            {
                _CodeElt=_CodeElt.PadRight(5, 'X');
            }
            try
            {
                items.Add("_@6", _CodeElt[0].ToString()) ;
                items.Add("_@7", _CodeElt[1].ToString()) ;
                items.Add("_@8", _CodeElt[2].ToString()) ;
                items.Add("_@9", _CodeElt[3].ToString()) ;
                items.Add("_@a", _CodeElt[4].ToString()) ;
            }
            catch (Exception)
            {}

            //Numero de compte - client
            _CodeElt = NumCompteClient;
            if (string.IsNullOrEmpty(_CodeElt))
                _CodeElt = "XXXXXXXXXXX";
            if (_CodeElt.Length < 11) 
                _CodeElt=_CodeElt.PadRight(11, 'X');
            try
            {
                items.Add("_@b", _CodeElt[0].ToString()) ;
                items.Add("_@c", _CodeElt[1].ToString()) ;
                items.Add("_@d", _CodeElt[2].ToString()) ;
                items.Add("_@e", _CodeElt[3].ToString()) ;
                items.Add("_@f", _CodeElt[4].ToString()) ;
                items.Add("_@g", _CodeElt[5].ToString()) ;
                items.Add("_@h", _CodeElt[6].ToString()) ;
                items.Add("_@i", _CodeElt[7].ToString()) ;
                items.Add("_@j", _CodeElt[8].ToString()) ;
                items.Add("_@k",_CodeElt[9].ToString()) ;
                items.Add("_@l", _CodeElt[10].ToString()) ;
            }
            catch (Exception)
            {}

            //Cle de compte - client
            _CodeElt = Cle;
            if (string.IsNullOrEmpty(_CodeElt))
                _CodeElt = "XX";
            if (_CodeElt.Length < 2) _CodeElt=_CodeElt.PadRight(2, 'X');
            try
            {
                items.Add("_@m", _CodeElt[0].ToString()) ;
                items.Add("_@n", _CodeElt[1].ToString()) ;
            }
            catch (Exception)
            {}

            //Code Etablissement - Beneficiaire
            _CodeElt = CodeEtsBenf;
            if (string.IsNullOrEmpty(_CodeElt))
                _CodeElt = "XXXXX";
            if (_CodeElt.Length < 5) _CodeElt=_CodeElt.PadRight(5, 'X');
            try
            {
                items.Add("_]1", _CodeElt[0].ToString()) ;
                items.Add("_]2", _CodeElt[1].ToString()) ;
                items.Add("_]3", _CodeElt[2].ToString()) ;
                items.Add("_]4", _CodeElt[3].ToString()) ;
                items.Add("_]5", _CodeElt[4].ToString()) ;
            }
            catch (Exception)
            {}

            //Code agence - Beneficiaire
            _CodeElt = CodeAgenceBenf;
            if (string.IsNullOrEmpty(_CodeElt))
                _CodeElt = "XXXXX";
            if (_CodeElt.Length < 5) _CodeElt=_CodeElt.PadRight(5, 'X');
            try
            {
                items.Add("_]6", _CodeElt[0].ToString()) ;
                items.Add("_]7", _CodeElt[1].ToString()) ;
                items.Add("_]8", _CodeElt[2].ToString()) ;
                items.Add("_]9", _CodeElt[3].ToString()) ;
                items.Add("_]0", _CodeElt[4].ToString()) ;
            }
            catch (Exception)
            {}

            //Numero de compte - Beneficiaire
            _CodeElt = NumCompteBenef;
            if (string.IsNullOrEmpty(_CodeElt))
                _CodeElt = "XXXXXXXXXXX";
            if (_CodeElt.Length < 11) _CodeElt=_CodeElt.PadRight(11, 'X');
            try
            {
                items.Add("_]a", _CodeElt[0].ToString());
                items.Add("_]b", _CodeElt[1].ToString());
                items.Add("_]c", _CodeElt[2].ToString());
                items.Add("_]d", _CodeElt[3].ToString());
                items.Add("_]e", _CodeElt[4].ToString());
                items.Add("_]f", _CodeElt[5].ToString());
                items.Add("_]g", _CodeElt[6].ToString());
                items.Add("_]h", _CodeElt[7].ToString());
                items.Add("_]i", _CodeElt[8].ToString());
                items.Add("_]j", _CodeElt[9].ToString());
                items.Add("_]k", _CodeElt[10].ToString());
            }
            catch (Exception)
            {}
            //Cle de compte - client
            _CodeElt = Cle;
            if (string.IsNullOrEmpty(_CodeElt))
                _CodeElt = "XX";
            if (_CodeElt.Length < 5) _CodeElt=_CodeElt.PadRight(5, 'X');
            try
            {
                items.Add("_]l", _CodeElt[0].ToString());
                items.Add("_]m", _CodeElt[1].ToString());
            }
            catch (Exception)
            { }
            //Code swift (Bic)
            _CodeElt = CodeSwiftBic;
            if (string.IsNullOrEmpty(_CodeElt))
                _CodeElt = "XXXXXXXXXXXXXXX";
            if (_CodeElt.Length < 15) _CodeElt = _CodeElt.PadRight(15, 'X');
            try
            {
                items.Add("_$1", _CodeElt[0].ToString());
                items.Add("_$2", _CodeElt[1].ToString());
                items.Add("_$3", _CodeElt[2].ToString());
                items.Add("_$4", _CodeElt[3].ToString());
                items.Add("_$5", _CodeElt[4].ToString());
                items.Add("_$6", _CodeElt[5].ToString());
                items.Add("_$7", _CodeElt[6].ToString());
                items.Add("_$8", _CodeElt[7].ToString());
                items.Add("_$9", _CodeElt[8].ToString());
                items.Add("_$0", _CodeElt[9].ToString());
                items.Add("_$a", _CodeElt[10].ToString());
                items.Add("_$b", _CodeElt[11].ToString());
                items.Add("_$c", _CodeElt[12].ToString());
                items.Add("_$d", _CodeElt[13].ToString());
                items.Add("_$e", _CodeElt[14].ToString());
            }
            catch (Exception)
            { }
            return items;
        }

        public virtual ICollection<ElementResumeTransfert> TransfertResume()
        {
            List<ElementResumeTransfert> elts = new List<ElementResumeTransfert>();
            //Instruction
            try
            {
                elts.Add(new ElementResumeTransfert()
                {
                    Nom = "Instruction",
                    DateCreation = this.DateCreationAppToString,
                    Nbrfichier = "1",
                    Description =   $"<p>- Modif de l'opération : {Motif}.<p> Nature opération: {NatureOperation}.</p>"
                                   + $"<p>- Devise : {this.DeviseToString}.\t Monatnt en devise: {MontantString}. Equivalent XAF: {MontantCVstring}</p>"
                                   + $"<p>- Montant en lettre : {this.MontantEnLettre}. </p>"
                                   + $"<p>- Etat de la marchadise : {this.MarchandiseArriveeToString}.  </p> \t<p> Date signature de l'instruction: {DateSignInstToString}</p>"
                                   + "<hr />"
                });
            }
            catch (Exception)
            {}

            //Factures
            try
            {
                foreach (var item in Justificatifs)
                {
                    try
                    {
                        elts.Add(new ElementResumeTransfert()
                        {
                            Nom = $"Facture - {item.NumeroJustif}",
                            DateCreation = item.DateCreaAppJustif.ToString("dd/MM/yyyy"),
                            DateModif = item.DateModifJustif.ToString("dd/MM/yyyy"),
                            Nbrfichier = item.NbrePieces + "",
                            Description = $"<p>- Montant: {MontantString}.\t Payé:  {item.MontantPayeString}.\t Reste à payer:  {item.MontantRestantString}<p />"
                        });
                    }
                    catch (Exception)
                    { }

                }
            }
            catch (Exception)
            { }

            //Lettre d'engagement
            try
            {
                elts.Add(new ElementResumeTransfert()
                {
                    Nom = $"Lettre d'engagement",
                    DateCreation = LettreEngage.DateCreationToString,
                    Nbrfichier = LettreEngage.GetImageDocumentAttache().Url != "#" ? "1" : "0",
                    Description = $"<p>- Document numérisé le {LettreEngage.DateCreationToString}<p />"
                });
            }
            catch (Exception)
            {}

            //Quittance de payement
            try
            {
                elts.Add(new ElementResumeTransfert()
                {
                    Nom = $"Quittance de paiement",
                    DateCreation = QuittancePay.DateCreationToString,
                    Nbrfichier = QuittancePay.GetImageDocumentAttache().Url != "#" ? "1" : "0",
                    Description = $"<p>- Document numérisé le {QuittancePay.DateCreationToString}<p />"
                });
            }
            catch (Exception)
            {}

            //Declaration d'importation
            try
            {
                elts.Add(new ElementResumeTransfert()
                {
                    Nom = $"Déclaration d'importation",
                    DateCreation = DeclarImport.DateCreationToString,
                    Nbrfichier = DeclarImport.GetImageDocumentAttache().Url != "#" ? "1" : "0",
                    Description = $"<p>- Document numérisé le {DeclarImport.DateCreationToString}<p />"
                });
            }
            catch (Exception)
            {}

            //Domiciliation d'importation
            try
            {
                elts.Add(new ElementResumeTransfert()
                {
                    Nom = $"Domiciliation d'importation",
                    DateCreation = DomicilImport.DateCreationToString,
                    Nbrfichier = DomicilImport.GetImageDocumentAttache().Url != "#" ? "1" : "0",
                    Description = $"<p>- Document numérisé le {DomicilImport.DateCreationToString}<p />"
                });
            }
            catch (Exception)
            { }

            //Documents de transport
            try
            {
                foreach (var item in DocumentsTransport)
                {
                    try
                    {
                        elts.Add(new ElementResumeTransfert()
                        {
                            Nom = $"Document de trandport - {item.Nom}",
                            DateCreation = item.DateCreationToString,
                            Nbrfichier = item.GetImageDocumentAttache().Url != "#" ? "1" : "0",
                            Description = $"<p>- Document numérisé le {item.DateCreationToString}<p />"
                        });
                    }
                    catch (Exception)
                    { }

                }
            }
            catch (Exception)
            {}

            //Autres documents
            try
            {
                foreach (var item in DocumentAttaches)
                {
                    try
                    {
                        elts.Add(new ElementResumeTransfert()
                        {
                            Nom = $"{item.Nom}",
                            DateCreation = item.DateCreationToString,
                            Nbrfichier = item.GetImageDocumentAttache().Url != "#" ? "1" : "0",
                            Description = $"<p>- Document numérisé le {item.DateCreationToString}<p />"
                        });
                    }
                    catch (Exception)
                    { }
                }
            }
            catch (Exception)
            {}

            //Statuts
            try
            {
                string status = "";DateTime? datetmp=null;
                foreach (var item in StatusDossiers.OrderByDescending(s=>s.Date))
                {
                    try
                    {
                        if (datetmp == null)
                            status += $"<h6>{item.DateToString}: {NbreJourEntreDates(datetmp, item.Date)}</h6>";
                        else
                            status += $"<h6>{datetmp.Value.ToString("dd/MM/yyyy")} - {item.DateToString}: {NbreJourEntreDates(datetmp, item.Date)}</h6>";
                        status += $"<p>- {item.Statut1}</p> <hr />";
                        datetmp = item.Date;
                    }
                    catch (Exception)
                    {}
                }
                elts.Add(new ElementResumeTransfert()
                {
                    Nom = "Statuts",
                    DateCreation = this.DateCreationAppToString,
                    Description = status
                });
            }
            catch (Exception)
            { }

            return elts;
        }

        private string NbreJourEntreDates(DateTime? datetmp, DateTime date)
        {
            if (datetmp==null) return "Actuellement";
            if (date==null) return "";
            return (int)(datetmp.Value.Date-date).TotalDays+" jours";
        }

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
                return $"{ClientId}_{FournisseurId}_{Dossier_Id}";
            }
        }
        public string Intitulé2
        {
            get
            {
                try
                {
                    return $"{MontantStringDevise}";

                }
                catch (Exception)
                {}                
                return $"{ClientId}_{Dossier_Id}_{FournisseurId}";
            }
        }

        public string ToString(byte cl_bq,string motcle)
        {
            try
            {

                if (!string.IsNullOrEmpty(motcle))
                    if (motcle.Contains("client-"))
                        motcle = "clients";
                    else if (motcle.Contains("dev-"))
                        motcle = "devises";
                    else if (motcle.Contains("fou-"))
                        motcle = "fournisseurs";
                
                if (cl_bq == 1)//Client
                {
                    switch (motcle)
                    {
                        case "fournisseurs":
                            return $"{MontantStringDevise}";
                        case "devises":
                            return $"{GetFournisseur.Substring(0,5) +".."} - {MontantString}";
                        default:
                            return $"{GetFournisseur.Substring(0,4) + "."} - {MontantStringDevise}";
                    }
                }
                else //Banque
                {
                    switch (motcle)
                    {
                        case "fournisseurs":
                            return $"{GetClient.Substring(0,4) + "."} - {MontantStringDevise}";
                        case "devises":
                            return $"{GetClient.Substring(0,4) + "."} - {GetFournisseur.Substring(0,4) +"."} - {MontantString}";
                        case "clients":
                            return $"{GetFournisseur.Substring(0,4) + "."} - {MontantStringDevise}";
                        default:
                            return $"{GetClient.Substring(0,4) + "."} - {GetFournisseur.Substring(0,4) + "."} - {MontantStringDevise}";
                    }
                }
            }
            catch (Exception)
            {}
            return Intitulé;
        }

        [Display(Name ="Nature de l'importation")]
        [Required]
        public virtual NatureOperation NatureOperation { get; set; }

        #region Suppression des archives
        public bool SupprimeClient { get; set; }
        public bool SupprimeBanque { get; set; }
        #endregion

        /// <summary>
        /// Indique si le dossier est pour la BEAC
        /// </summary>
        //public bool EstBAEC { get; set; }
        //public bool EstFondPropre { get; set; }
        //public bool EstDFX { get; set; }

        //dfx=1;fp=2;beac=3
        public byte DFX6FP6BEAC { get; set; }

        public bool ValiderDouane { get; set; }
        [Display(Name ="Code établissement"),MaxLength(5)]
        public string CodeEtablissement { get; set; }
        [Display(Name = "Code agence"), MaxLength(5)]
        public string CodeAgence { get; set; }

        [Display(Name = "Ville banque bénéficiaire"), MaxLength(5)]
        public string VilleBanqueBenf { get; set; }

        public char[] GetCodeAgence
        {
            get {
                try
                {
                    if (string.IsNullOrWhiteSpace(CodeAgence)) return new char[5];
                    if (CodeAgence.Length < 5)
                    {
                        CodeAgence = CodeAgence.PadRight(5, ' ');
                    }
                    return CodeAgence.ToArray();
                }
                catch (Exception)
                {}
                return new char[5]; ; 
            }
        }
        
        public char[] GetCodeAgenceBenf
        {
            get {
                try
                {
                    if (string.IsNullOrWhiteSpace(CodeAgenceBenf)) return new char[5];
                    if (CodeAgenceBenf.Length < 5)
                    {
                        CodeAgenceBenf = CodeAgenceBenf.PadRight(5, ' ');
                    }
                    return CodeAgenceBenf.ToArray();
                }
                catch (Exception)
                {}
                return new char[5]; ; 
            }
        }
        
        public char[] GetCodeEtablissement
        {
            get {
                try
                {
                    if (string.IsNullOrWhiteSpace(Client.CodeEtablissement)) return new char[5];
                    if (Client.CodeEtablissement.Length < 5)
                    {
                        return Client.CodeEtablissement.PadRight(5, ' ').ToArray();
                    }
                    return Client.CodeEtablissement.ToArray();
                }
                catch (Exception)
                {}
                return new char[5];
            }
        }
        
        public char[] GetNumCompteClient
        {
            get {
                try
                {
                    if (string.IsNullOrWhiteSpace(NumCompteClient)) return new char[11];
                    if (NumCompteClient.Length < 11)
                    {
                        NumCompteClient = NumCompteClient.PadRight(11, ' ');
                    }
                    return NumCompteClient.ToArray();
                }
                catch (Exception)
                {}
                return new char[11]; 
            }
        }
        
        public char[] GetRibBanqueBenf
        {
            get {
                try
                {
                    if (string.IsNullOrWhiteSpace(RibBanqueBenf)) return new char[23];
                    if (RibBanqueBenf.Length < 23)
                    {
                        RibBanqueBenf = RibBanqueBenf.PadRight(22, ' ');
                    }
                    return RibBanqueBenf.ToArray();
                }
                catch (Exception)
                {}
                return new char[23]; 
            }
        }

         public char[] GetCodeEtbBenf
        {
            get {
                try
                {
                    if (string.IsNullOrWhiteSpace(CodeEtsBenf)) return new char[5];
                    if (CodeEtsBenf.Length < 5)
                    {
                        CodeEtsBenf = CodeEtsBenf.PadRight(5, ' ');
                    }
                    return CodeEtsBenf.ToArray();
                }
                catch (Exception)
                {}
                return new char[5]; 
            }
        }

        
         public char[] GetNumCompteBenf
        {
            get {
                try
                {
                    if (string.IsNullOrWhiteSpace(NumCompteBenef)) return new char[11];
                    if (NumCompteBenef.Length < 11)
                    {
                        NumCompteBenef = NumCompteBenef.PadRight(11, ' ');
                    }
                    return NumCompteBenef.ToArray();
                }
                catch (Exception)
                {}
                return new char[11]; 
            }
        }


        
        public char[] GetIBANBenf
        {
            get {
                try
                {
                    if (string.IsNullOrWhiteSpace(RibBanqueBenf)) return new char[27];
                    if (RibBanqueBenf.Length < 27)
                    {
                        RibBanqueBenf = RibBanqueBenf.PadRight(27, ' ');
                    }
                    return RibBanqueBenf.ToArray();
                }
                catch (Exception)
                {}
                return new char[27]; 
            }
        }
        
        public char[] GetCle
        {
            get {
                try
                {
                    if (string.IsNullOrWhiteSpace(Cle)) return new char[2];
                    if (Cle.Length < 2)
                    {
                        Cle = Cle.PadRight(2, ' ');
                    }
                    return Cle.ToArray();
                }
                catch (Exception)
                {}
                return new char[2]; 
            }
        }
        
        public char[] GetCleBenf
        {
            get {
                try
                {
                    if (string.IsNullOrWhiteSpace(CleCompteBenf)) return new char[2];
                    if (CleCompteBenf.Length < 2)
                    {
                        CleCompteBenf = CleCompteBenf.PadRight(2, ' ');
                    }
                    return CleCompteBenf.ToArray();
                }
                catch (Exception)
                {}
                return new char[2]; 
            }
        }

        [Display(Name = "Date de signature de l'instruction")]
        public DateTime? DateSignInst { get; set; }

        public byte EtapeValidationClient { get; set; } = 0;
        public string DateSignInstToString
        {
            get
            {
                try
                {
                    return DateSignInst.Value.ToString("dd/MM/yyyy");
                }
                catch (Exception)
                { }
                return "";
            }
        }

        [NotMapped]
        public string LabelColor { get; set; }

        public string GetColorLab(string lab)
        {
            if (lab=="devise")
            {
                if (this.DeviseToString == "USD") return "gold";
                if (this.DeviseToString == "EUR") return "gold";
            }

            return "";
        }

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
        /// <summary>
        /// Quand le dossier est rejeté: EtapesDosier<0
        /// </summary>
        public int? EtapePrecedenteDosier { get; set; }

        [NotMapped]
        public int Transmis { get; set; }

        public StatutDossier GetStatusString()
        {
            StatutDossier statut =null;
            try
            {
                statut = StatusDossiers.OrderByDescending(s=>s.Date).FirstOrDefault(s=>s.Etat==EtapesDosier);
            }
            catch (Exception e)
            {}
            if (statut == null) statut = new StatutDossier();
            return statut;
        }
        
        public string GetStatusStringA(bool isClient=false,int? minSite=null,int? maxSite=null)
        {
            if (isClient) return !string.IsNullOrEmpty(GetEtapDossier()[3])? GetEtapDossier()[3]: GetEtapDossier(EtapesDosier-1)[3];
            if (minSite <= EtapesDosier && EtapesDosier < maxSite)
                return GetEtapDossier()[0];
            var ch= GetEtapDossier()[2];
            return ch;
        }

        #region Client Info
        [Display(Name = "N° compte du client"), MaxLength(11)]
        public string NumCompteClient { get; set; }

        private string paysclient;
        [Display(Name = "Pays")]
        public string PaysClient
        {
            get {
                try
                {
                    if (string.IsNullOrEmpty(paysclient) && Client != null)
                        return Client.Pays;
                }
                catch (Exception)
                {}
                return paysclient; 
            }
            set { paysclient = value; }
        }


        [Display(Name = "Clé compte du client"), MaxLength(2)]
        public string Cle { get; set; }

        #endregion
        [Display(Name = "Compte du bénéficiaire"),MaxLength(11)]
        public string NumCompteBenef { get; set; }
        [Display(Name = "Clé"),MaxLength(2)]
        public string CleCompteBenf { get; set; }
        public int? IdBanqueBenef { get; set; }
        public string PaysBanqueBenf { get; set; }
        public string RibBanqueBenf { get; set; }
        [Display(Name = "Banque du bénéficiaire")]
        public string NomBanqueBenf { get; set; }
        [Display(Name = "Adresse de la Banque du bénéficiaire")]
        public string AdresseBanqueBenf { get; set; }
        [Display(Name = "Code agence")]
        public string CodeAgenceBenf { get; set; }
        
        string _codeEtsBenf;
        [NotMapped]
        [Display(Name = "Code d'établissement")]
        public string CodeEtsBenf
        {
            get {
                try
                {
                    if(Fournisseur!=null)
                    return Fournisseur.CodeEts;
                }
                catch (Exception)
                {}
                return ""; 
            }
            set { _codeEtsBenf = value; }
        }


        public string NumBanqueBenf { get; set; }

        public StatutDossier GetStatutEncours
        {
            get {
                StatutDossier stat = null;
                try
                {
                    stat = this.StatusDossiers.OrderByDescending(s => s.Date).FirstOrDefault();
                }
                catch (Exception)
                {}
                if(stat==null) return new StatutDossier();
                return stat;
            }
        }

        public virtual bool EtapeNumerisation
        {
            get {
                try
                {
                    if (FournisseurId > 0 && ImportateurId > 0 && !string.IsNullOrEmpty(Description)
                        && DeviseMonetaireId >0 && Montant >0)
                        return true;
                }
                catch (Exception)
                {}
                return false;
            }
        }

        public bool InEtapeNum { get; set; }

        public string GetMotifCurrenteStatus
        {
            get
            {
                string motif = "";
                try
                {
                    motif = this.GetStatutEncours.Motif;
                }
                catch (Exception)
                { }
                return motif;
            }
        }

        public bool EstEditable
        {
            get {
                try
                {
                    if (this.EtapesDosier == null || this.EtapesDosier == -1 || this.EtapesDosier == 0 || this.EtapesDosier == 22 
                        || this.EtapesDosier == 23 || this.EtapesDosier == -230 || this.EtapesDosier == -231
                        || this.EtapesDosier == -232 || this.EtapesDosier == 25 || this.EtapesDosier == -250)
                    {
                        return true;
                    }
                }
                catch (Exception)
                {}
                return false;
            }
        }

        public string Message { get; set; }

        [Display(Name = "Motif du transfert")]
        public string Motif { get; set; }

        [Display(Name = "BIC Correspondant bancaire")]
        [NotMapped]
        public string BicCorrespondant { get; set; }

        public bool EstPasséConformite { get; set; }

        [NotMapped]
        public string MessageTmp { get; set; }
        [NotMapped]
        public string ItemsRaison { get; set; }

        [NotMapped]
        public string _NatureOp { get; set; }

        //public Dictionary<string,string> GetVariables(CompteBanqueCommerciale ges, string banque,DateTime date,string structure,string agent)
        public Dictionary<string,string> GetVariables(string banque,string structure,string agent)
        {
            Dictionary<string, string> dir = new Dictionary<string, string>();
            //dir.Add("[NOM_FOURNISSEUR]", this.Fournisseur.Nom);
            //dir.Add("[MONTANT]", this.MontantString);
            //dir.Add("[DEVISE]", this.DeviseMonetaire.Nom);
            //dir.Add("[GESTIONNAIRE]", ges.NomComplet);
            //dir.Add("[TEL_GESTIONNAIRE]", ges.Tel1);
            //dir.Add("[EMAIL_GESTIONNAIRE]", ges.Email);
            //dir.Add("[DATE]", date.ToString("dd/MM/yyyy"));
            //dir.Add("[REFERENCE]", this.GetNumRefBEAC);
            //dir.Add("[CLIENT]", this.GetClient);
            //dir.Add("[STRUCTURE]", structure);
            dir.Add("[BANQUE]", banque);
            dir.Add("[AGENT]", agent);
            dir.Add("[SITE]", structure);
            return dir;
        }

        private string EncoursColor = "#ffd800";
        private string redColor = "#f00020";

        public string[] GetEtapDossier(int? eta=null)
        {
            var tmp = eta!=null?eta:EtapesDosier;
            //0: message; 1,2,3,...,n: status
            switch (tmp)
            {
                //Client
                case null: return new string[] {"Brouillon", EncoursColor, "", ""};
                case 0: return new string[] {"En attente d'envoie", EncoursColor, "", ""};
                case 1:
                    var msg1 = Message1("[BANQUE]");
                    var msg2 = Message2("[BANQUE]");
                    return new string[] {"Dossier envoyé", EncoursColor, msg1, msg2};
                //Banque agence
                case 2: return new string[] { "[SITE] : Dossier reçu", EncoursColor, "", ""};
                case 3: return new string[] {"[SITE] : vérification en cours", EncoursColor, "",""};
                case 4:
                    var msgDocDouaneRecu = MsgDocDouaneRecu("[BANQUE]");
                    return new string[] {"[SITE] : Attente référence douanes", EncoursColor, "", msgDocDouaneRecu };
                case 5:
                    var msgDouane = MsgDouane("[BANQUE]");
                    return new string[] {"[SITE] : En attente de validation", EncoursColor, "", msgDouane };
                case 6:
                    var messagerecuConformite = MessagerecuConformite("[BANQUE]");
                    return new string[] {"[SITE] : Envoyé au Service Conformité", EncoursColor, "", messagerecuConformite };
                // Conformité
                case 7: return new string[] { "CONFORMITE : Dossier reçu", EncoursColor,  "","" };
                case 8: return new string[] {"CONFORMITE : Vérification en cours", EncoursColor, "","" };
                case 9: return new string[] {"[SITE] : Dossier validé", EncoursColor, "","" };
                //Service transfert
                case 10: return new string[] {"Service Transfert : Dossier reçu", EncoursColor, "",""}; // position 0:DFX; 2: refin
                case 11: return new string[] {"Service Transfert : Vérification en cours", EncoursColor, "","" }; // position 0:DFX; 2: refin
                case 12: return new string[] { "Service Transfert : Référencement du dossier en cours", EncoursColor, "","" }; //refin
                case 13: return new string[] {"Service Transfert : Dossier référencé", EncoursColor, "","" }; //refin
                case 14: return new string[] {"Service Transfert : Formalité BEAC en cours", EncoursColor, "",""}; // refin
                case 15:
                    var msgbeacEnv = MessageBEACEnvoie("[BANQUE]");
                    var msgbeacEnv2 = MessageBEACEnvoie2("[BANQUE]");
                    return new string[] {"Service Transfert : Dossier envoyé à la BEAC", EncoursColor, msgbeacEnv, msgbeacEnv2};// Refin
                case 16:
                    var msgbeacAcc = MessageBEACAccord("[BANQUE]");
                    var msgbeacAcc2 = MessageBEACAccord2("[BANQUE]");
                    return new string[] {"BEAC: Dossier accordé", "#01d758", msgbeacAcc, msgbeacAcc2};// Refin
                case 17: return new string[] { "Service Transfert : Couverture devise en cours", "#4adc0c", "","" };//FP et DFX ;
                case 18: return new string[] { "BEAC : Couverture devise en cours", "#01d758", "","" };// Refin; DFX
                case 19:
                case 20:
                    var msgbeacDeviseRecu = MessageBEACDeviseRecu("[BANQUE]");
                    var msgbeacDeviseRecu2 = MessageBEACDeviseRecu2("[BANQUE]");
                    if(DFX6FP6BEAC==2)
                        return new string[] {"Service Transfert : Dossier accordé", "#01d758", msgbeacDeviseRecu, msgbeacDeviseRecu2 };// Refin; DFX
                    else
                        return new string[] {"Service Transfert : Devise reçue", "#01d758", msgbeacDeviseRecu, msgbeacDeviseRecu2 };// Refin; DFX
                //case 20:
                //    var msgDFXDeviseRecu = MessageDFXDeviseRecu("[NOM_FOURNISSEUR]", "[MONTANT]", "[DEVISE]", "[REFERENCE]", "[BANQUE]");
                //    var msgDFXDeviseRecu2 = MessageDFXDeviseRecu2("[CLIENT]", "[MONTANT]", "[DEVISE]", "[NOM_FOURNISSEUR]", "[BANQUE]", "[REFERENCE]");
                //    return new string[] {"Service Transfert : Devise reçu", "forestgreen",msgDFXDeviseRecu, msgDFXDeviseRecu2 };// Refin; BEAC

                case 21: return new string[] { "Service Transfert : Traitement en cours", EncoursColor, "","" };// Refin; DFX
                case 22:
                    var msgbeacTraite = MessageBEACTraite("[BANQUE]");
                    var msgbeacTraite2 = MessageBEACTraite2("[BANQUE]", "[AGENT]", "[SITE]");
                    return new string[] {"Service Transfert : Transfert traité", "#01d758", msgbeacTraite, msgbeacTraite2 }; // 0: DFX; 2: refin
                case 23: return new string[] {"Dossier à apurer", EncoursColor, "","" };
                case 230: return new string[] { "En cours de vérification à l'agence", EncoursColor, "","" };
                case 231:
                    return new string[] { "[SITE] : Apurement validé", EncoursColor,"",""};
                case 232:
                    var msgbeacEnvAp = MessageBEACEnvoieApurement("[BANQUE]");
                    var msgbeacEnv2Ap = MessageBEACEnvoie2Apurement("[BANQUE]");
                    return new string[] { "En attente apurement BEAC", EncoursColor, msgbeacEnvAp, msgbeacEnv2Ap };
                case -230: 
                case -232: 
                case -231: 
                    return new string[] { "Dossier à apurer rejeté", redColor, "","" };
                case 24:
                    var msgApure = MessageApure("[BANQUE]");
                    var msgApure2 = MessageApure2("[BANQUE]");
                    return new string[] {"Dossier apuré", "#01d758", msgApure,msgApure2};
                case 25: return new string[] {"Dossier échu", redColor, "", ""};
                case 250: return new string[] { "Dossier échu transmis à la banque", redColor, "","" };
                case -250: return new string[] { "Dossier échu rejeté", redColor, "","" };
                case 26: return new string[] {"Dossier archivé", "saddlebrown", "","" };
                case 27: return new string[] {"Dossier supprimé", "black", "","" };
                case -3:
                    var msgerejetBeac = MessageRejetBEAC("[BANQUE]", "[MOTIF]");
                    var msgerejetBeac2 = MessageRejetBEAC2("[BANQUE]", "[MOTIF]");
                    return new string[] { "Dossier rejeté", redColor, msgerejetBeac, msgerejetBeac2 };
                case -2: 
                case -1:
                    var msgerr = MessageErr("[BANQUE]", "[MOTIF]");
                    var msgerr2 = MessageErr2("[BANQUE]", "[AGENT]", "[SITE]", "[MOTIF]");
                    return new string[] {"Dossier rejeté", redColor, msgerr , msgerr2};
                default:
                    return new string[] { "", "black", "","" };
            }
        }

        public string[] GetParametre(Dictionary<string, string> parms)
        {
            string[] t = new string[5];
            string ch = "", mGes = "", stat = "",_message="",_message2="";
            try
            {
                var tab = GetEtapDossier(this.EtapesDosier);
                _message = tab[2];
                _message2 = tab[3];
                ch = _message;
                mGes = _message2;
                stat = tab[0];
                foreach (var p in parms)
                {
                    try
                    {
                        ch = ch.Replace(p.Key, p.Value);
                    }
                    catch (Exception)
                    { }
                    try
                    {
                        mGes = mGes.Replace(p.Key, p.Value);
                    }
                    catch (Exception)
                    { }
                    try
                    {
                        stat = stat.Replace(p.Key, p.Value);
                    }
                    catch (Exception)
                    { }
                }

                if (!string.IsNullOrEmpty(_message))
                {

                    t[0] = ch.Split('&')[0];
                    t[1] = ch.Split('&')[1];
                }
                if (!string.IsNullOrEmpty(_message2))
                {
                    t[2] = mGes.Split('&')[0];
                    t[3] = mGes.Split('&')[1];
                }
                //else
                //{
                //    t[0] = ch;
                //}
                t[4] = stat;
            }
            catch (Exception)
            { }
            return t;
        }

        #region Champs acces rapide au données
        public virtual DateTime? ObtDevise { get; set; } = default;
        public virtual DateTime? Date_Etape2 { get; set; } = default;
        public virtual DateTime? Date_Etape3 { get; set; } = default;
        public virtual DateTime? Date_Etape4 { get; set; } = default;
        public virtual DateTime? Date_Etape5 { get; set; } = default;
        public virtual DateTime? Date_Etape6 { get; set; } = default;
        public virtual DateTime? Date_Etape7 { get; set; } = default;
        public virtual DateTime? Date_Etape8 { get; set; } = default;
        public virtual DateTime? Date_Etape9 { get; set; } = default;
        public virtual DateTime? Date_Etape10 { get; set; } = default;
        public virtual DateTime? Date_Etape11 { get; set; } = default;
        public virtual DateTime? Date_Etape12 { get; set; } = default;
        public virtual DateTime? Date_Etape13 { get; set; } = default;
        public virtual DateTime? Date_Etape14 { get; set; } = default;
        public virtual DateTime? Date_Etape15 { get; set; } = default;
        public virtual DateTime? Date_Etape16 { get; set; } = default;
        public virtual DateTime? Date_Etape17 { get; set; } = default;
        public virtual DateTime? Date_Etape18 { get; set; } = default;
        public virtual DateTime? Date_Etape19 { get; set; } = default;
        public virtual DateTime? Date_Etape20 { get; set; } = default;
        public virtual DateTime? Date_Etape21 { get; set; } = default;
        public virtual DateTime? Date_Etape22 { get; set; } = default;

        public string Date_Etape22ToString
        {
            get
            {
                try
                {
                    return Date_Etape22.Value.ToString("dd/MM/yyyy");
                }
                catch (Exception)
                { }
                return "";
            }
        }

        public virtual DateTime? Date_Etape23 { get; set; } = default;
        public virtual DateTime? Date_Etape24 { get; set; } = default;
        public virtual DateTime? Date_Etape25 { get; set; } = default;

        #endregion


        #region Variables de tmp
        //public string Message1(params string[] t)
        public string Message1(string BanqueName)
        {
            try
            {
                //return $"Dossier envoyé ({GetFournisseur}; {MontantStringDevise})&Madame; Monsieur <br /> <br />Le transfert de {MontantStringDevise}  du fournisseur {t[0]} a été transmis avec succès à votre Gestionnaire {GetClientEmail} {t[3]}. Vous pourrez le contacter aux coordonnées suivantes <br/>: Téléphone :{t[4]}; Adresse mail: {t[5]}.<br /><br /> {t[6]} vous remercie pour votre confiance.";
                return $"Dossier envoyé ({GetFournisseur}; {MontantString} {DeviseToString})&Madame; Monsieur <br /> <br />Le transfert de {MontantString} {DeviseToString}  du fournisseur {GetFournisseur} a été transmis avec succès à votre Gestionnaire {Get_CiviliteGestionnaire} {GestionnaireName}. Vous pourrez le contacter aux coordonnées suivantes <br/>: Téléphone :{GestionnairePhone}<br/> Adresse mail: {GestionnaireEmail}.<br /><br /> {BanqueName} vous remercie pour votre confiance.";
            }
            catch (Exception)
            {}
            return "";
        } 
        
        public string Message2(string BanqueName)
        {
            try
            {
                return $"Dossier {GetClient} reçu ({GetFournisseur}; {MontantStringDevise})&Le transfert du client <b>{GetClient}<b/> de {MontantStringDevise} du fournisseur {GetFournisseur} a été envoyé sur votre compte Genetrix. Vueillez vous connecter pour le traiter.<br /><br /> {BanqueName}";
            }
            catch (Exception)
            {}
            return "";
        } 

        public string MessagerecuConformite(string banque)
        {
            try
            {
                return $"Dossier {GetClient} reçu ({GetFournisseur}; {MontantStringDevise})&Le transfert du client <b>{GetClient}<b/> de {MontantStringDevise} du fournisseur {GetFournisseur} vient d'être reçu à votre service pour son traitement. Vueillez vous connecter sur Genetrix pour proceder à son traitement.<br /><br /> {banque}";
            }
            catch (Exception)
            {}
            return "";
        } 
        
        public string MsgDocDouaneRecu(string banque)
        {
            try
            {
                return $"Dossier reçu ({GetFournisseur}; {MontantStringDevise})&Madame; Monsieur <br /> <br />Le transfert du client {GetClient} de {MontantStringDevise}  du fournisseur {GetFournisseur} est en attente des documents de douanes. Veuillez-vous connecter sur Genetrix pour proceder à son traitement. <br /><br /> {banque}.";
            }
            catch (Exception)
            {}
            return "";
        }

        public string MsgDouane(string banque)
        {
            try
            {
                return $"Dossier en attente de validation ({GetFournisseur}; {MontantStringDevise})&Madame; Monsieur <br /> <br />Les documents douanes du transfert {GetClient} de {MontantStringDevise}  du fournisseur {GetFournisseur} ont été ajoutés avec succès. <br /><br /> {banque}";
            }
            catch (Exception)
            {}
            return "";
        }

        public string MessageBEACEnvoie(string banque)
        {
            try
            {
                if (this.DFX6FP6BEAC==3)
                {
                    return $"Dossier BEAC envoyé ({GetFournisseur}; {Get_ReferenceMontantDeviseString})&Madame; Monsieur <br /><br />Le transfert de {Get_ReferenceMontantDeviseString}  du fournisseur {GetFournisseur} a été envoyé à la BEAC le {dateNow.ToString("dd/MM/yyyy")} sous la référence suivante : {GetReference}." +
                               $" Pour plus d’informations, veuillez contacter votre gestionnaire aux coordonnées suivantes :<br/> Téléphone :{GestionnairePhone}; Adresse mail: {GestionnaireEmail}.<br /><br /> {banque} vous remercie pour votre confiance.";
                }
                else if(DFX6FP6BEAC==1)
                {
                    return $"Dossier BEAC envoyé ({GetFournisseur}; {MontantStringDevise})&Madame; Monsieur <br /><br />Nous vous informons de la prise en compte du dossier de transfert de {MontantString} Euros du fournisseur {GetFournisseur}, dans l’enveloppe DFX transmise ce jour à la BEAC pour allocation de devises." +
                              $"<br/><br/>Nous restons dans l’attente de la couverture effective, et ne manquerons pas de vous tenir informés sur les prochaines étapes de ce dossier." +
                              $"<br/><br/>Votre gestionnaire {Get_CiviliteGestionnaire} {GestionnaireName} se tient à votre disposition pour tout complément d’information, aux coordonnées suivantes :<br/>"+
                              $"Téléphone :{GestionnairePhone}<br/><br/> Mail: {GestionnaireEmail}.<br /><br /> {banque} vous remercie pour votre confiance.";
                }
            }
            catch (Exception)
            {}
            return "";
        }
        public string MessageBEACEnvoieApurement(string banque)
        {
            try
            {
                return $"Dossier BEAC envoyé ({GetFournisseur}; {MontantStringDevise})&Madame; Monsieur <br /><br />Le transfert de {MontantStringDevise}  du fournisseur {GetFournisseur} a été envoyé à la BEAC le {dateNow.ToString("dd/MM/yyyy")} pour son apurement."+
                   $" Pour plus d’informations, veuillez contacter votre gestionnaire aux coordonnées suivantes :<br/> Téléphone :{GestionnairePhone}<br/>  Adresse mail: {GestionnaireEmail}.<br /><br /> {banque} vous remercie pour votre confiance."; 
            }
            catch (Exception)
            {}
            return "";
        }
        
        public string MessageApure(string banque)
        {
            try
            {
                return $"Dossier BEAC envoyé ({GetFournisseur}; {MontantStringDevise})&Madame; Monsieur <br /><br />Le transfert de {MontantStringDevise}  du fournisseur {GetFournisseur} a été apuré, vous pouvez maintenant le retrouver dans le module archivage de l'application GENETRIX."+
                   $" Pour plus d’informations, veuillez contacter votre gestionnaire aux coordonnées suivantes :<br/> Téléphone :{GestionnairePhone}; Adresse mail: {GestionnaireEmail}.<br /><br /> {banque} vous remercie pour votre confiance."; 
            }
            catch (Exception)
            {}
            return "";
        }

        public string MessageBEACEnvoie2(string banque)
        {
            try
            {
                return $"Dossier {GetClient} envoyé à la BEAC ({GetFournisseur}; {MontantStringDevise})&Le dossier du client <b>{GetClient}<b/> de {MontantStringDevise}  du fournisseur {GetFournisseur} a été envoyé à la BEAC le {dateNow.ToString("dd/MM/yyyy")} sous la référence suivante: <b>{GetReference} .<br /><br /> {banque}";
            }
            catch (Exception)
            {}
            return "";
        }
        
        public string MessageBEACEnvoie2Apurement(string banque)
        {
            try
            {
                return $"Dossier {GetClient} envoyé à la BEAC ({GetFournisseur}; {MontantStringDevise})&Le dossier du client <b>{GetClient}<b/> de {MontantStringDevise}  du fournisseur {GetFournisseur} a été envoyé à la BEAC le {dateNow.ToString("dd/MM/yyyy")} pour son apurement.<br /><br /> {banque}";
            }
            catch (Exception)
            {}
            return "";
        }
        
        public string MessageApure2(string banque)
        {
            try
            {
                return $"Dossier {GetClient} apuré ({GetFournisseur}; {MontantStringDevise})&Le dossier du client <b>{GetClient}<b/> de {MontantStringDevise}  du fournisseur {GetFournisseur} a été apuré le {dateNow.ToString("dd/MM/yyyy")}.<br /><br /> {banque}";
            }
            catch (Exception)
            {}
            return "";
        }

        public string MessageBEACAccord(string banque)
        {
            try
            {
                return $"Dossier BEAC accordé ({GetFournisseur}; {Get_ReferenceMontantDeviseString})&Madame; Monsieur <br /><br />Nous avons le plaisir de vous notifier l’accord BEAC du dossier de {Get_ReferenceMontantDeviseString}  du fournisseur {GetFournisseur} enregistré sous la référence : {GetReference}.<br/> <br/>"+
                   $" Vous serez à nouveau notifiez une fois la couverture en devise reçue.<br /><br /> {banque} vous remercie pour votre confiance."; 

            }
            catch (Exception)
            {}
            return "";
        }
        public string MessageBEACAccord2(string banque)
        {
            try
            {
                return $"Dossier {GetClient} accordé ({GetFournisseur}; {Get_ReferenceMontantDeviseString})&Le dossier BEAC du client <b>{GetClient}<b/> de {Get_ReferenceMontantDeviseString}  du fournisseur {GetFournisseur}  enregistré sous la référence {GetReference} a été accordé le {dateNow.ToString("dd/MM/yyyy")}.<br /><br /> {banque}";
            }
            catch (Exception)
            {}
            return "";
        }
        
        public string MessageBEACDeviseRecu(string banque)
        {
            try
            {
                if (DFX6FP6BEAC==2)
                {
                    return $"Dossier accordé ({GetFournisseur}; {MontantStringDevise})&Madame; Monsieur <br /><br />Nous avons le plaisir de vous notifier que le dossier de {MontantStringDevise}  du fournisseur {GetFournisseur}, traité sous fonds propres a été accordé ce jour.<br/> <br/>" +
                               $"Nous procédons à son traitement sous 24h maximum. Nous vous rappelons à toutes fins utiles que les délais d'apurement sont de 1 mois pour les services et de trois mois pour les biens. Nous vous invitons dès lors à préparer la documentation y relative pour son apurement.<br /><br /> {banque} vous remercie pour votre confiance.";  
                }if (DFX6FP6BEAC==3)
                {
                    return $"Refinancement, devise reçue ({GetFournisseur}; {Get_ReferenceMontantDeviseString})&Madame; Monsieur <br /><br />Nous avons le plaisir de vous notifier la couverture BEAC en {DeviseToString} du dossier de {Get_ReferenceMontantString}  du fournisseur {GetFournisseur}, enregistré sous la référence : {GetReference}.<br/> <br/>" +
                               $"Nous procédons à son traitement sous 24h maximum. Nous vous rappelons à toutes fins utiles que les délais d'apurement sont de un mois pour les services et de trois mois pour les biens. Nous vous invitons dès lors à préparer la documentation y relative pour son apurement.<br /><br /> {banque} vous remercie pour votre confiance.";  
                }else if (DFX6FP6BEAC==1)
                {
                    return $"DFX, devise reçue ({GetFournisseur}; {MontantStringDevise})&Madame; Monsieur <br /><br />Nous avons le plaisir de vous informer que le dossier DFX de {MontantString} Euros du fournisseur {GetFournisseur} a fait l’objet d’allocation de devise par la BEAC.<br/> <br/>" +
                               $"Nous procédons à son traitement sous 24h maximum. Nous vous rappelons à toutes fins utiles que les délais d'apurement sont de 1 mois pour les services et de trois mois pour les biens. Nous vous invitons dès lors à préparer la documentation y relative pour son apurement.<br /><br /> <b>{banque}<b/> vous remercie pour votre confiance.";  
                }
            }
            catch (Exception)
            {}
            return "";
        }
        public string MessageBEACDeviseRecu2(string banque)
        {
            try
            {
                if(DFX6FP6BEAC==2)
                    return $"Dossier accordé ({GetFournisseur}; {MontantStringDevise})&Le dossier du client <b>{GetClient}<b/> de {MontantStringDevise}  du fournisseur {GetFournisseur}, traité sur fonds propres a été accordé ce jour {dateNow.ToString("dd/MM/yyyy")}.<br /><br /> {banque}";
                return $"Dossier {GetClient}, devise recue ({GetFournisseur}; {MontantStringDevise})&Le dossier BEAC du client <b>{GetClient}<b/> de {MontantStringDevise}  du fournisseur {GetFournisseur},  enregistré sous la référence {GetReference} a reçu la couverture BEAC en {devise} le {dateNow.ToString("dd/MM/yyyy")}.<br /><br /> {banque}";
            }
            catch (Exception)
            {}
            return "";
        }

        public string MessageDFXDeviseRecu(string banque)
        {
            try
            {
                if (DFX6FP6BEAC==3)
                {
                    return $"Dossier BEAC, devise reçue ({GetFournisseur}; {MontantStringDevise}):&Madame; Monsieur<br /><br />Nous avons le plaisir de vous notifier la couverture en {DeviseToString} du dossier de {MontantStringDevise}  du fournisseur {GetFournisseur}.<br/> <br/>" +
                               $"Nous procédons à son traitement sous 24h maximum.<br /><br /> {banque} vous remercie pour votre confiance.";
                }
                else if(DFX6FP6BEAC==1)
                {
                    return $"Dossier DFX, devise reçue ({GetFournisseur}; {MontantStringDevise}):&Madame; Monsieur<br /><br />Nous avons le plaisir de vous notifier la couverture en {DeviseToString} du dossier de {MontantStringDevise}  du fournisseur {GetFournisseur}.<br/> <br/>" +
                               $"Nous procédons à son traitement sous 24h maximum.<br /><br /> {banque} vous remercie pour votre confiance.";
                }

            }
            catch (Exception)
            {}
            return "";
        }

        public string MessageDFXDeviseRecu2(string banque)
        {
            try
            {
                return $"Dossier DFX {GetClient}, devise recue ({GetFournisseur}; {MontantStringDevise})&Le dossier DFX du client <b>{GetClient}<b/> de {MontantStringDevise}  du fournisseur {GetFournisseur} a reçu la couverture en {devise} le {dateNow.ToString("dd/MM/yyyy")}.<br /><br /> {banque}";
            }
            catch (Exception)
            { }
            return "";
        }

        public string MessageBEACTraite(string banque)
        {
            try
            {
                if (DFX6FP6BEAC == 3)
                {
                    return $"Dossier BEAC traité ({GetFournisseur}; {Get_ReferenceMontantDeviseString})&Madame; Monsieur <br /><br /> Nous vous confirmons le traitement du dossier de {Get_ReferenceMontantDeviseString}  du fournisseur {GetFournisseur}, enregistré sous la référence : {GetReference}.<br/> <br/>" +
                    $"Nous vous invitons dès lors à suivre l’apurement de ce dossier dans le module dédié  de notre application GENETRIX.<br /><br /> " +
                    $"Pour plus d’informations, veuillez contacter votre gestionnaire aux coordonnées suivantes :<br/> Téléphone :{GestionnairePhone}; Adresse mail: {GestionnaireEmail}.<br /><br /> {banque} vous remercie pour votre confiance.";

                }
                else if (DFX6FP6BEAC == 1)
                {
                    return $"Dossier DFX traité ({GetFournisseur}; {MontantStringDevise})&Madame; Monsieur <br /><br /> Nous vous confirmons le traitement du dossier DFX de {MontantStringDevise}  du fournisseur {GetFournisseur}, enregistré sous la référence : {GetReference}.<br/> <br/>" +
                     $"Nous vous invitons dès lors à suivre l’apurement de ce dossier dans le module dédié  de notre application GENETRIX.<br /><br /> " +
                     $"Pour plus d’informations, veuillez contacter votre gestionnaire aux coordonnées suivantes :<br/> Téléphone :{GestionnairePhone}; Adresse mail: {GestionnaireEmail}.<br /><br /> {banque} vous remercie pour votre confiance.";
                }
                else
                {
                    return $"Dossier fonds propres traité ({GetFournisseur}; {MontantStringDevise})&Madame; Monsieur <br /><br /> Nous vous confirmons le traitement du dossier (fonds propres) de {MontantStringDevise}  du fournisseur {GetFournisseur}.<br/> <br/>" +
                   $"Nous vous invitons dès lors à suivre l’apurement de ce dossier dans le module dédié  de notre application GENETRIX.<br /><br /> " +
                   $"Pour plus d’informations, veuillez contacter votre gestionnaire aux coordonnées suivantes :<br/> Téléphone :{GestionnairePhone}; Adresse mail: {GestionnaireEmail}.<br /><br /> {banque} vous remercie pour votre confiance.";

                }

            }
            catch (Exception)
            { }
            return "";
        }

        public string MessageBEACTraite2(string banque,string agent,string site)
        {
            try
            {
                if (DFX6FP6BEAC == 3)
                {
                    return $"Dossier refinancement {GetClient} traité ({GetFournisseur}; {Get_ReferenceMontantDeviseString})&Madame; Monsieur <br /><br /> Vous avez terminé  le traitement du dossier BEAC du client {GetClient}, de {Get_ReferenceMontantDeviseString}  du fournisseur {GetFournisseur}, enregistré sous la référence : {GetReference}.<br/> <br/>" +
                    $"<br /><br /> {banque}";

                }
                else if (DFX6FP6BEAC == 1)
                {
                    return $"Dossier DFX {GetClient} traité ({GetFournisseur}; {MontantStringDevise})&Madame; Monsieur <br /><br /> Vous avez terminé  le traitement du dossier du client {GetClient}, de {MontantStringDevise}  du fournisseur {GetFournisseur}, enregistré sous la référence : {GetReference}.<br/> <br/>" +
                    $"<br /><br /> {banque}";
                }
                else
                {
                    return $"Dossier fonds propres {GetClient} traité ({GetFournisseur}; {MontantStringDevise})&Madame; Monsieur <br /><br /> Vous avez terminé le traitement du dossier du client {GetClient}, de {MontantStringDevise}  du fournisseur {GetFournisseur}, enregistré sous la référence : {GetReference}.<br/> <br/>" +
                    $"<br /><br /> {banque}";

                }
            }
            catch (Exception)
            { }
            return "";
        }

        public string MessageErr(string banque,string motif)
        {
            try
            {
                return $"Rejet du dossier ({GetFournisseur}; {MontantStringDevise})&Madame; Monsieur <br /><br />Le transfert de {MontantStringDevise}  du fournisseur {GetFournisseur} a été rejeté pour le motif suivant :<mark> {motif} </mark>. Pour plus d’informations, veuillez contacter votre gestionnaire aux coordonnées suivantes :<br/> Téléphone :{GestionnairePhone}; Adresse mail: {GestionnaireEmail}.<br /><br /> {banque} vous remercie pour votre confiance.";
            }
            catch (Exception)
            { }
            return "";
        }

        public string MessageErr2(string banque, string agent, string site, string motif)
        {
            try
            {
                return $"Rejet du dossier {GetClient} ({GetFournisseur}; {MontantStringDevise})&Le transfert du client <b>{GetClient}<b/> de {MontantStringDevise}  du fournisseur {GetFournisseur} a été rejeté par {agent} ({site}), le {dateNow.ToString("dd/MM/yyyy")}  pour le motif suivant: <mark> {motif} <mark/>.<br /><br /> {banque}";
            }
            catch (Exception)
            { }
            return "";
        }


        public string MessageEchu(string banque)
        {
            try
            {
                return $"Délai d'apurement à echéance ({GetFournisseur}; {MontantStringDevise})&Madame; Monsieur <br /><br />Le dossier de {MontantStringDevise}  du fournisseur {GetFournisseur} arrive bientôt dans {GetDelai} jours. Veuillez vous connecter sur le module apurement de l'application Genetrix pour apurer le dossier.<br /><br /> {banque} vous remercie pour votre confiance.";
            }
            catch (Exception)
            { }
            return "";
        }

        public string MessageEchu2(string banque)
        {
            try
            {
                return $"Délai d'apurement à echéance ({GetClient}; {MontantStringDevise})&Le dossier <b>{GetClient}<b/> de {MontantStringDevise}  du fournisseur {GetFournisseur} arrive bientôt dans {GetDelai} jours. Veuillez contacter le client pour apurer le dossier.<br /><br /> {banque}.";
            }
            catch (Exception)
            { }
            return "";
        }


        public string MessageRejetBEAC(string banque, string motif)
        {
            try
            {
                return $"Dossier BEAC rejeté ({GetFournisseur}; {MontantStringDevise})&Madame; Monsieur <br /><br />Nous avons le regret de vous informer du rejet BEAC du dossier  de {MontantStringDevise}  du fournisseur {GetFournisseur}, enregistré  sous la référence  <b> {GetReference}<b/> .<br /><br /> <span style=\"color:{redColor}\"> Motif suivant : {motif} </span>.<br /><br /> Pour plus d’informations, veuillez contacter votre gestionnaire aux coordonnées suivantes : Téléphone :{GestionnairePhone}; Adresse mail: {GestionnaireEmail}.<br /><br /> {banque} vous remercie pour votre confiance.";
            }
            catch (Exception)
            { }
            return "";
        }

        public string MessageRejetBEAC2(string banque, string motif)
        {
            try
            {
                return $"Dossier BEAC {GetClient} rejeté ({GetFournisseur}; {MontantStringDevise})&Le transfert du client <b>{GetClient}<b/> de {MontantStringDevise}  du fournisseur {GetFournisseur}, enregistré  sous la référence {GetReference} a été rejeté le {dateNow.ToString("dd/MM/yyyy")} pour au motif suivant: {motif}.<br /><br /> {banque}";
            }
            catch (Exception)
            { }
            return "";
        }

        #endregion

        public string MontantString
        {
            get {
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
                    NumberFormatInfo nfi = new NumberFormatInfo { NumberGroupSeparator = " ", NumberDecimalDigits = _NumberDecimalDigits};
                    return Montant.ToString("n", nfi);
                }
                catch (Exception)
                { }
                return "0"; 
            }
        }
        
        public string MontantStringDevise
        {
            get {
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
                    NumberFormatInfo nfi = new NumberFormatInfo { NumberGroupSeparator = " ", NumberDecimalDigits = _NumberDecimalDigits };
                    return Montant.ToString("n", nfi)+" "+DeviseToString;
                }
                catch (Exception)
                { }
                return "0"; 
            }
        }

        public double MontantCV
        {
            get {
                try
                {
                    return Montant*DeviseMonetaire.ParitéXAF;
                }
                catch (Exception)
                { }
                return 0; 
            }
        }

        public string MontantCVToString
        {
            get {
                try
                {
                    // A ajouter ???
                    // le cours vous sera communiqué par votre gestionnaire
                    int _NumberDecimalDigits = 0;
                    try
                    {
                        if (Montant % 1 > 0)
                        {
                            _NumberDecimalDigits = Montant.ToString().Split(',')[1].Length;
                            _NumberDecimalDigits = 2; 
                        }
                    }
                    catch (Exception)
                    { }
                    NumberFormatInfo nfi = new NumberFormatInfo { NumberGroupSeparator = " ", NumberDecimalDigits = _NumberDecimalDigits };
                    return (Montant*DeviseMonetaire.ParitéXAF).ToString("n", nfi)+" XAF";
                }
                catch (Exception)
                { }
                return "0"; 
            }
        }

        public string MontantCVstring
        {
            get {
                try
                { 
                    NumberFormatInfo nfi = new NumberFormatInfo { NumberGroupSeparator = " ", NumberDecimalDigits = 0 };
                    return (Montant*DeviseMonetaire.ParitéXAF).ToString("n", nfi);
                }
                catch (Exception)
                { }
                return "0"; 
            }
        }


        public DateTime? DateNouvelleEtape { get; set; } = DateTime.Now;
        //public virtual ICollection<DossierAgent> DossierAgents { get; set; }

        public virtual ICollection<StatutDossier> StatusDossiers { get; set; } = new List<StatutDossier>();
        public virtual ICollection<DossierStructure> GetStructures { get; set; }

        public int NbrTelechargement { get; set; }
        public int NbrTelechargementApurement { get; set; }

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

        /// <summary>
        /// 0: DFX; 1: Refinancement
        /// </summary>
        [Display(Name = "Cateorie")]
        public byte? Categorie{ get; set; } = null;
        public CategorieDossier CategorieDossier { get; set; }=default;

        public string GetCategorieDossier()
        {
            switch (Categorie)
            {
                case 0:return "DFX";
                case 1:return "REfinancement";
                default: return "";
            }
        }

        /// <summary>
        /// Agent respossable du dossier dans une structure autre que l'agence
        /// </summary>
        [ForeignKey("AgentResponsableDossier")]
        [Display(Name = "Responsable du dossier")]
        public string IdAgentResponsableDossier { get; set; } = null;
        public virtual CompteBanqueCommerciale AgentResponsableDossier{ get; set; }

        public string GetCurrenteResponsable(int eta)
        {
            try
            {
                switch (eta)
                {
                    case 1:
                        return this.AgentResponsableAgence.NomComplet;
                    case 4:
                        return this.AgentResponsableBackOffice.NomComplet;
                    case 6:
                        return this.AgentResponsableConformite.NomComplet;
                    case 9:
                        return this.AgentResponsableTransfert.NomComplet;
                    default:
                        break;
                }
            }
            catch (Exception)
            {}
            return "";
        }
        
        public string GetCurrenteResponsableID(int eta)
        {
            try
            {
                switch (eta)
                {
                    case 1:
                        return this.IdResponsableAgence;
                    case 4:
                        return this.IdResponsableBackOffice;
                    case 6:
                        return this.IdResponsableConformite;
                    case 9:
                        return this.IdResponsableTransfert;
                    default:
                        break;
                }
            }
            catch (Exception)
            {}
            return "";
        }

        public string IdPrecedentResponsable { get; set; }

        [ForeignKey("AgentResponsableAgence")]
        [Display(Name = "Responsable agence")]
        public string IdResponsableAgence { get; set; } = null;
        public virtual CompteBanqueCommerciale AgentResponsableAgence{ get; set; }

        [ForeignKey("AgentResponsableConformite")]
        [Display(Name = "Responsable conformité")]
        public string IdResponsableConformite { get; set; } = null;
        public virtual CompteBanqueCommerciale AgentResponsableConformite { get; set; }
        
        [ForeignKey("AgentResponsableBackOffice")]
        [Display(Name = "Responsable back-office")]
        public string IdResponsableBackOffice { get; set; } = null;
        public virtual CompteBanqueCommerciale AgentResponsableBackOffice{ get; set; }

        [ForeignKey("AgentResponsableTransfert")]
        [Display(Name = "Responsable transfert")]
        public string IdResponsableTransfert { get; set; } = null;
        public virtual CompteBanqueCommerciale AgentResponsableTransfert{ get; set; }

        [Display(Name = "Dernière modification")]
        public DateTime? DateModif { get; set; }

        public string DateModifToString
        {
            get
            {
                try
                {
                    return DateModif.Value.ToString("dd/MM/yyyy");
                }
                catch (Exception)
                { }
                return "";
            }
        }
        
        public string TotalFichiers
        {
            get
            {
                int nbr = 0;
                try
                {
                    //Instruction
                    nbr = GetImageInstruction() != "#" ? 1 : 0;
                }
                catch (Exception)
                { }
                
                try
                {
                    //Facture
                    foreach (var item in Justificatifs)
                    {
                        try
                        {
                            nbr += item.NbrePieces;
                        }
                        catch (Exception)
                        {}
                    }
                }
                catch (Exception)
                { }

                try
                {
                    //Lettre d'engagement
                    nbr += Get_LettreEngage != "#" ? 1 : 0;
                }
                catch (Exception)
                { }

                try
                {
                    //Quittance de paiement
                    nbr += Get_QuittancePay != "#" ? 1 : 0;
                }
                catch (Exception)
                { }

                try
                {
                    //Declaration d'importation
                    nbr += Get_DeclarImport != "#" ? 1 : 0;
                }
                catch (Exception)
                { }

                try
                {
                    //Domiciliation d'importation
                    nbr += Get_DomicilImport != "#" ? 1 : 0;
                }
                catch (Exception)
                { }

                try
                {
                    //Trandport
                    foreach (var item in DocumentsTransport)
                    {
                        try
                        {
                            nbr += item.GetImageDocumentAttache().Url!="#"?1:0;
                        }
                        catch (Exception)
                        { }
                    }
                }
                catch (Exception)
                { }
                
                try
                {
                    //Autres
                    foreach (var item in DocumentAttaches)
                    {
                        try
                        {
                            nbr += item.GetImageDocumentAttache().Url!="#"?1:0;
                        }
                        catch (Exception)
                        { }
                    }
                }
                catch (Exception)
                { }

                return nbr+"";
            }
        }

        int nbrJust;
        [DisplayName("Nombre de factures")]
        public int NbreJustif {
            get {
                try
                {
                    nbrJust=Justificatifs.Count;
                }
                catch (Exception)
                { }
                return nbrJust; 
            }
            set { nbrJust = value; }
        }

        double montantP;
        public string MontantPassif
        {
            get {
                try
                {
                    montantP = Montant;
                   Justificatifs.ToList().ForEach(j=>
                   {
                       montantP -= j.MontantJustif;
                   });
                }
                catch (Exception)
                { }
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
                NumberFormatInfo nfi = new NumberFormatInfo { NumberGroupSeparator = " ", NumberDecimalDigits = _NumberDecimalDigits};
                return montantP.ToString("n", nfi);
            }
        }

        private double montant;

        [DisplayFormat(DataFormatString = "{0:#,##0.00#}", ApplyFormatInEditMode = true)]
        public double MontantTpmp
        {
            get {
                try
                {
                    if (Justificatifs.Count()>0)
                    {
                        montant = 0;
                        Justificatifs.ToList().ForEach(j =>
                        {
                            montant += j.MontantJustif;
                        });
                        return montant - Montant;
                    }
                }
                catch (Exception)
                { }
                return Montant;
            }
        }

        public string MontantTmpString
        {
            get {
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
                NumberFormatInfo nfi = new NumberFormatInfo { NumberGroupSeparator = " ", NumberDecimalDigits = _NumberDecimalDigits };
                return MontantTpmp.ToString("n", nfi);
            }
        }



        public bool FiniFormalitéBEAC
        {
            get {
                try
                {
                    if (courrier != null && courrier.Signature1 && courrier.Signature2 && NbrTelechargement > 0)
                       return true;
                }
                catch (Exception)
                {}
                return false; 
            }
        }


        #region Instruction

        [DisplayName("Site")]
        [ForeignKey("Site")]
        public int IdSite{ get; set; }
        public virtual Agence Site { get; set; }

        public string IdGestionnaire { get; set; }

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
        
        public string GestionnaireName
        {
            get {
                try
                {
                    return this.Client.Banques.FirstOrDefault().Gestionnaire.NomComplet;
                }
                catch (Exception)
                {}
                return ""; 
            }
        }
        
        public string GestionnaireEmail
        {
            get {
                try
                {
                    return this.Client.Banques.FirstOrDefault().Gestionnaire.Email;
                }
                catch (Exception)
                {}
                return ""; 
            }
        }
         
        public string GestionnaireVille
        {
            get {
                try
                {
                    return this.Client.Banques.FirstOrDefault().Ville;
                }
                catch (Exception)
                {}
                return ""; 
            }
        }
         
        public string GetVilleBenf
        {
            get {
                try
                {
                    return this.Fournisseur.Ville;
                }
                catch (Exception)
                {}
                return ""; 
            }
        }
        
        public string GetVilleBanqueBenf
        {
            get {
                try
                {
                    return this.VilleBanqueBenf;
                }
                catch (Exception)
                {}
                return ""; 
            }
        }
        
        public string GestionnairePhone
        {
            get {
                try
                {
                    return this.Client.Banques.FirstOrDefault().Gestionnaire.Tel1;
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
        [Display(Name ="Montant")]
        [Required(ErrorMessage = "Le champs montant est obligatoire!")]
        //[DisplayFormat(DataFormatString = "{0:#.##0.00#}", ApplyFormatInEditMode = true)]
        [DisplayFormat(DataFormatString = "{0:0.##}", ApplyFormatInEditMode = true)]
        public double Montant
        {
            get {
                try
                {
                    //if (TotalResteFactures > 0)
                    //    monatant = TotalPayeFactures;
                    if (DeviseMonetaire != null)
                    {
                        int _NumberDecimalDigits = 0;
                        try
                        {
                            if (monatant % 1 > 0)
                            {
                                _NumberDecimalDigits = 2;// monatant.ToString().Split('.')[1].Length; 
                            }
                        }
                        catch (Exception)
                        { }
                        NumberFormatInfo nfi = new NumberFormatInfo { NumberGroupSeparator = " ", NumberDecimalDigits = _NumberDecimalDigits,NumberDecimalSeparator="." };
                        ContreValeurXAF = (float)(Math.Round(monatant * DeviseMonetaire.ParitéXAF,2));
                        ((float)ContreValeurXAF).ToString("n", nfi);

                    }
                }
                catch (Exception)
                { }
                return Math.Round(monatant,2); 
            }
            set { monatant = value; }
        }

        [Display(Name ="C/V XAF")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}", ApplyFormatInEditMode = true)]
        public float? ContreValeurXAF { get; set; }

        [ForeignKey("Fournisseur")]
        [Display(Name ="Bénéficiaire")]
        public int? FournisseurId { get; set; }
        public virtual Fournisseurs Fournisseur { get; set; }

        [ForeignKey("Importateur")]
        [Display(Name ="Importateur")]
        public int? ImportateurId { get; set; }
        public virtual Importateur Importateur { get; set; }

        [Display(Name ="Adresse du bénéficiaire")]
        public string AdresseFournisseur { get; set; }

        string _getFournisseur;
        [NotMapped]
        public string GetFournisseur
        {
            get {
                try
                {
                    return Fournisseur.Nom;
                }
                catch (Exception)
                { }
                return ""; 
            }
            set { _getFournisseur=value; } 
        }

        public string GetImportateur
        {
            get {
                try
                {
                    return Importateur.NomComplet;
                }
                catch (Exception)
                { }
                return ""; 
            }
        }

        public string PaysBenifiaire
        {
            get {
                try
                {
                    return Fournisseur.Pays;
                }
                catch (Exception)
                { }
                return ""; 
            }
        }
        public string TelFournisseur
        {
            get {
                try
                {
                    return Fournisseur.Tel1;
                }
                catch (Exception)
                { }
                return ""; 
            }
        }
        
        public string EmailFournisseur
        {
            get {
                try
                {
                    return Fournisseur.Email;
                }
                catch (Exception)
                { }
                return ""; 
            }
        }

        string _paysFournisseur;
        [NotMapped]
        public string PaysFournisseur
        {
            get {
                try
                {
                    return Fournisseur.Pays;
                }
                catch (Exception)
                { }
                return "";
            }
            set { _paysFournisseur = value; }
        }

        string _getAgenceName;
        [NotMapped]
        public string GetAgenceName
        {
            get {
                try
                {
                    return Site.Nom;
                }
                catch (Exception)
                { }
                return "";
            }
            set { _getAgenceName = value; }
        }
        
        public string GetAgenceAdresse
        {
            get {
                try
                {
                    return Site.Adresse;
                }
                catch (Exception)
                { }
                return ""; 
            }
        }

        public string GetCategorie
        {
            get {
                try
                {
                    switch (DFX6FP6BEAC)
                    {
                        case 1: return "DFX";
                        case 2: return "FP";
                        case 3: return "Ref";
                        default:
                            break;
                    }
                }
                catch (Exception)
                { }
                return ""; 
            }
        }

        public Reference GetReferenceObject
        {
            get {
                try
                {
                    switch (DFX6FP6BEAC)
                    {
                        case 1: 
                            return DFX;
                        case 3:
                                return ReferenceExterne;
                        default:
                            break;
                    }
                }
                catch (Exception)
                { }
                return null; 
            }
        }
        public int GetReferenceId
        {
            get {
                try
                {
                    switch (DFX6FP6BEAC)
                    {
                        case 1: 
                            if(DfxId != null && DfxId !=0)
                            return DFX.Id;
                            return 0;
                        case 3:
                            if (ReferenceExterne != null)
                                return ReferenceExterne.Id;
                            return 0;
                        default:
                            break;
                    }
                }
                catch (Exception)
                { }
                return 0; 
            }
        }

         public string GetReference
        {
            get {
                try
                {
                    switch (DFX6FP6BEAC)
                    {
                        case 1: 
                            if(DfxId != null && DfxId !=0)
                            return DFX.NumeroRef;
                            return "";
                        case 3: 
                            return ReferenceExterne.NumeroRef;
                        default:
                            break;
                    }
                }
                catch (Exception)
                { }
                return ""; 
            }
        }

        public string GetClient
        {
            get {
                try
                {
                    return Client.Nom;
                }
                catch (Exception e)
                { }
                return ""; 
            }
        }

        public string AdresseClient
        {
            get {
                try
                {
                    return Client.Adresse;
                }
                catch (Exception)
                { }
                return ""; 
            }
        }
        public string VilleClient
        {
            get {
                try
                {
                    return Client.Ville;
                }
                catch (Exception)
                { }
                return ""; 
            }
        }

        public string RibClient
        {
            get {
                try
                {
                    return Client.RIB(NumCompteClient,Cle);
                }
                catch (Exception)
                { }
                return ""; 
            }
        }
        public string CodeEtsClient
        {
            get {
                try
                {
                    return Client.CodeEtablissement;
                }
                catch (Exception)
                { }
                return ""; 
            }
        }
     

        public string TelClient
        {
            get {
                try
                {
                    return Client.Telephone;
                }
                catch (Exception)
                { }
                return ""; 
            }
        }

        public string ProfessionClient
        {
            get {
                try
                {
                    return Client.Profession;
                }
                catch (Exception)
                { }
                return ""; 
            }
        }


        [Display(Name="Date de dépot à la banque")]
        public DateTime? DateDepotBank { get; set; }

        public string DateDepotBankToString
        {
            get
            {
                try
                {
                    return DateDepotBank.Value.ToString("dd/MM/yyyy");
                }
                catch (Exception)
                { }
                return "";
            }
        }

        [Display(Name = "Date d'envoi à la BEAC")]
        public DateTime? DateEnvoiBeac { get; set; }
        
        public string DateEnvoiBeacToString
        {
            get
            {
                try
                {
                    return DateEnvoiBeac.Value.ToString("dd/MM/yyyy");
                }
                catch (Exception)
                { }
                return "";
            }
        }

        public DateTime? DateArchivage { get; set; }

        public string DateArchivageToString
        {
            get
            {
                try
                {
                    return DateArchivage.Value.ToString("dd/MM/yyyy");
                }
                catch (Exception)
                { }
                return "";
            }
        }

        [DisplayName("Debut numérisation")]
        public DateTime? DateCreationApp { get; set; }

        public string DateCreationAppToString
        {
            get {
                try
                {
                    return DateCreationApp.Value.ToString("dd/MM/yyyy");
                }
                catch (Exception)
                { }
                return ""; 
            }
        }


        private string utilisateur;
        [DisplayName("Numérisée par")]
        public string ApplicationUser
        {
            get {   return utilisateur; }
            set { utilisateur = value; }
        }

        #endregion

        //[Required(ErrorMessage = "Le champs dévise est obligatoire !")]
        [ForeignKey("DeviseMonetaire")]
        public int?  DeviseMonetaireId { get; set; }
        public bool Traité { get; set; }
        //public bool DeviseRecu { get; set; }
        //public DateTime DateDeviseRecu { get; set; }
        //public bool DebitCompteClient { get; set; }
        //public DateTime DateDebitCompteClient { get; set; }

        [Display(Name ="Marchandise arrivée")]
        public virtual bool MarchandiseArrivee { get; set; }

        public string MarchandiseArriveeToString
        {
            get {
                if (MarchandiseArrivee) return "Arrivée";
                return "Non arrivée"; 
            }
        }


        [Display(Name ="Valider conformité")]
        public bool ValiderConformite { get; set; }

        private DeviseMonetaire devise;

        public virtual DeviseMonetaire DeviseMonetaire
        {
            get {
                try
                {
                    int _NumberDecimalDigits = 0;
                    try
                    {
                        if (monatant % 1> 0)
                        {
                            _NumberDecimalDigits = monatant.ToString().Split('.')[1].Length; 
                        }
                    }
                    catch (Exception)
                    { }
                    NumberFormatInfo nfi = new NumberFormatInfo { NumberGroupSeparator = " ", NumberDecimalDigits = _NumberDecimalDigits};
                    if (devise!=null)
                    {
                        ContreValeurXAF = (float)(monatant * devise.ParitéXAF);
                        ((float)ContreValeurXAF).ToString("n", nfi); 
                    }
                }
                catch (Exception)
                { }
                return devise; 
            }
            set { devise = value; }
        }


        public string DeviseToString
        {
            get {
                try
                {
                   return DeviseMonetaire.Nom;
                }
                catch (Exception)
                {}
                return "Non renseignée"; 
            }
        }

        public string DeviseToString2Caracters
        {
            get {
                try
                {
                   return DeviseMonetaire.Nom.ToLower().Substring(0,1);
                }
                catch (Exception)
                {}
                return "Non renseignée"; 
            }
        }

        public string DeviseLib
        {
            get {
                try
                {
                   return DeviseMonetaire.Libelle;
                }
                catch (Exception)
                {}
                return "Non renseignée"; 
            }
        }

        [NotMapped]
        private string myVar;

        public string BanqueToString { get; set; }


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

        private string clientemail;
        public string GetClientEmail
        {
            get {
                try
                {
                    clientemail = Client.Email;

                    if (string.IsNullOrEmpty(clientemail))
                    {
                        clientemail = Client.Adresses.FirstOrDefault().Email;
                    }
                }
                catch (Exception)
                {}
                return clientemail;
            }
        }

       public string Get_CiviliteGestionnaire
        {
            get {
                try
                {

                    if (Client.Banques.FirstOrDefault().Gestionnaire != null)
                    {
                        return Client.Banques.FirstOrDefault().Gestionnaire.Sexe.ToString();
                    }
                }
                catch (Exception)
                {}
                return "";
            }
        }
        
        public List<string> GetAllClientEmail
        {
            get {
                List<string> mails = new List<string>();
                try
                {
                    mails.Add(Client.Email);

                    if (string.IsNullOrEmpty(clientemail))
                    {
                        mails = (from m in Client.Adresses select m.Email).ToList();
                    }
                }
                catch (Exception)
                {}
                return mails;
            }
        }
        #region Info Reference Banque

        [DisplayName("Reference banque")]
        [ForeignKey("ReferenceExterne")]
        public int? ReferenceExterneId { get; set; } = null;
        public virtual ReferenceBanque ReferenceExterne { get; set; }

        public double Get_ReferenceMontant
        {
            get {
                try
                {
                    if(ReferenceExterne!=null)
                    return ReferenceExterne.Montant;
                }
                catch (Exception)
                {}
                return 0;
            }
        }
        
        public double Get_ReferenceMontantCV
        {
            get {
                try
                {
                    if(ReferenceExterne!=null)
                    return ReferenceExterne.MontantCV;
                }
                catch (Exception)
                {}
                return 0;
            }
        }
        
        public string Get_ReferenceMontantString
        {
            get {
                try
                {
                    if(ReferenceExterne!=null)
                    return ReferenceExterne.MontantString;
                }
                catch (Exception)
                {}
                return "0";
            }
        }
        
        public string Get_ReferenceMontantDeviseString
        {
            get {
                try
                {
                    if(ReferenceExterne!=null)
                    return ReferenceExterne.MontantString;
                }
                catch (Exception)
                {}
                return "0";
            }
        }
        
        public string Get_ReferenceMontantCVString
        {
            get {
                try
                {
                    if(ReferenceExterne!=null)
                    return ReferenceExterne.MontantCVString;
                }
                catch (Exception)
                {}
                return "0";
            }
        }

        #endregion

        [ForeignKey("DFX")]
        public int? DfxId { get; set; } = null;
        public virtual DFX DFX { get; set; }

        [Display(Name ="Numéro DFX")]
        public string NumDFX { get; set; }

        [Display(Name = "Référence DFX")]
        public string RefDFX { get; set; }
        public string GetNumRefBEAC
        {
            get {
                try
                {
                    if (ReferenceExterne != null) return ReferenceExterne.NumeroRef;
                    return "";
                }
                catch (Exception)
                {}
                return "";
            }
        }

        public virtual ICollection<Justificatif> Justificatifs { get; set; } = new List<Justificatif>();

        public double ResteAPayerFacture
        {
            get {
                try
                {
                    if (Justificatifs != null && Justificatifs.Count > 0)
                        return Justificatifs.FirstOrDefault().TotalRestePayer;
                }
                catch (Exception)
                {}
                return 0; 
            }
        }
        public bool EstReglementPartiel
        {
            get {
                try
                {
                    if (Justificatifs != null)
                        foreach (var j in Justificatifs)
                            if (j.MontantJustif > 0 && j.MontantJustif > Montant)
                                return true;
                }
                catch (Exception)
                {}
                return false; 
            }
        }


        public int? TotalPageFactures
        {
            get {
                try
                {
                    int nbr = 0;
                    Justificatifs.ToList().ForEach(it =>
                    {
                        nbr += it.NbrePieces;
                    });
                    return nbr;
                }
                catch (Exception)
                {}
                return null; 
            }
        }

        [DisplayFormat(DataFormatString = "{0:#,##0.00#}", ApplyFormatInEditMode = true)]
        public double TotalResteFactures
        {
            get {
                try
                {
                    double nbr = 0;
                    Justificatifs.ToList().ForEach(it =>
                    {
                        nbr += it.MontantRestant;
                    });
                    return nbr;
                }
                catch (Exception)
                {}
                return 0; 
            }
        }
        public string TotalResteFacturesString
        {
            get {
                int _NumberDecimalDigits = 0;
                try
                {
                    if (Montant % 1 > 0)
                    {
                        _NumberDecimalDigits = 2; 
                    }
                }
                catch (Exception)
                { }
                try
                {
                    NumberFormatInfo nfi = new NumberFormatInfo { NumberGroupSeparator = " ", NumberDecimalDigits = _NumberDecimalDigits };
                    return TotalResteFactures.ToString("n", nfi);
                }
                catch (Exception)
                { }
                return "";
            }
        }

        [DisplayFormat(DataFormatString = "{0:#,##0.00#}", ApplyFormatInEditMode = true)]
        public double TotalPayeFactures
        {
            get {
                try
                {
                    double nbr = 0;
                    Justificatifs.ToList().ForEach(it =>
                    {
                        nbr += it.MontantPartiel;
                    });
                    return nbr;
                }
                catch (Exception)
                {}
                return 0; 
            }
        }
        public string TotalPayeFacturesString
        {
            get {
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
                    NumberFormatInfo nfi = new NumberFormatInfo { NumberGroupSeparator = " ", NumberDecimalDigits = _NumberDecimalDigits};
                    return TotalPayeFactures.ToString("n", nfi);
                }
                catch (Exception)
                { }
                return "";
            }
        }

        [DisplayFormat(DataFormatString = "{0:#,##0.00#}", ApplyFormatInEditMode = true)]
        public double TotalMontantFacture
        {
            get {
                try
                {
                    return Justificatifs.Sum(j=>j.MontantJustif);
                }
                catch (Exception)
                {}
                return 0; 
            }
        }

        public string TotalMontantFactureString
        {
            get {
                int _NumberDecimalDigits = 0;
                try
                {
                    if (Montant % 1 > 0)
                    {
                        _NumberDecimalDigits = 2; 
                    }
                }
                catch (Exception)
                { }
                try
                {
                    NumberFormatInfo nfi = new NumberFormatInfo { NumberGroupSeparator = " ", NumberDecimalDigits = _NumberDecimalDigits };
                    return TotalMontantFacture.ToString("n", nfi);
                }
                catch (Exception)
                { }
                return ""; 
            }
        }


        #region Documents obligatoires
        [DisplayName("N° déclaration")]
        public string NumDeclaration { get; set; }
        [DisplayName("Date déclaration")]
        public DateTime? DateDeclaration { get; set; }
        [DisplayName("Date imp.")]
        public DateTime? DateImport { get; set; }

        private DocumentAttache declarImport=null;
        [DisplayName("Déclaration d'importation")]
        public virtual DocumentAttache DeclarImport
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

        private DocumentAttache domicilImport=null;
        [DisplayName("Domiciliation d'importation")]
        public virtual DocumentAttache DomicilImport
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

        private DocumentAttache lettreEngag=null;
        [DisplayName("Lettre d'engagement")]
        public virtual DocumentAttache LettreEngage
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

        private DocumentAttache quittancePay=null;
        [DisplayName("Quittance de paiement")]
        public virtual DocumentAttache QuittancePay
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
        [DisplayName("Document de transport")]
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

        private DocumentAttache mt;
        public virtual DocumentAttache MT
        {
            get { return mt; }
            set
            {
                try
                {
                    if (value != null)
                    {
                        value.TypeDocumentAttaché = TypeDocumentAttaché.MT;
                    }
                }
                catch (Exception)
                { }
                mt = value;
            }
        }
        
        private DocumentAttache courrier;
        public virtual DocumentAttache Courrier
        {
            get { return courrier; }
            set
            {
                try
                {
                    if (value != null)
                    {
                        value.TypeDocumentAttaché = TypeDocumentAttaché.Courrier;
                    }
                }
                catch (Exception)
                { }
                courrier = value;
            }
        }

        private DocumentAttache endemeure=null;
        public virtual DocumentAttache EnDemeure
        {
            get { return endemeure; }
            set
            {
                try
                {
                    if (value != null)
                    {
                        value.TypeDocumentAttaché = TypeDocumentAttaché.EnDemeure;
                    }
                }
                catch (Exception)
                { }
                endemeure = value;
            }
        }

        private DocumentAttache endemeure2=null;
        public virtual DocumentAttache EnDemeure2
        {
            get { return endemeure2; }
            set
            {
                try
                {
                    if (value != null)
                    {
                        value.TypeDocumentAttaché = TypeDocumentAttaché.EnDemeure;
                    }
                }
                catch (Exception)
                { }
                endemeure2 = value;
            }
        }

        public virtual int EditeAuto
        {
            get
            {
                int aut = 7;
                if (Get_DomicilImport == "#")
                    aut= 6;
                if (Get_DeclarImport == "#")
                    aut = 5;
                if (MarchandiseArrivee)
                {
                    if (DocumentsTransportCount == 0)
                        aut = 4;
                    if (Get_QuittancePay == "#")
                        aut = 3; 
                }
                else if (Get_LettreEngage == "#")
                    aut = 2;
                if (TotalPageFactures == 0)
                    aut = 1;
                if (GetImageInstruction() == "#")
                    return 0;
                return aut; 
            }
        }

        public int EditeAuto2
        {
            get
            {
                if (Get_DomicilImport != "#")
                    return 7;
                if (Get_DeclarImport != "#")
                    return 6;
                if (DocumentsTransportCount >0)
                    return 5;
                if (Get_QuittancePay != "#")
                    return 4;
                if (Get_LettreEngage != "#")
                {
                    if (MarchandiseArrivee) return 5;
                    return 3;
                }
                if (TotalPageFactures > 0)
                {
                    if (MarchandiseArrivee) return 3;
                    return 2;
                }
                //if (GetImageInstruction() != "#")
                //    return 1;
                return 0; 
            }
        }


        #endregion

        [DisplayName("Autre document")]
        public virtual ICollection<DocumentAttache> DocumentAttaches { get; set; } = new List<DocumentAttache>();
        public int DocumentAttachesCount
        {
            get
            {
                try
                {
                    if (DocumentAttaches != null)
                    {
                        return DocumentAttaches.Count;
                    }
                }
                catch (Exception)
                { }
                return 0;
            }
        }

        [DisplayName("Documents de transport")]
        public virtual ICollection<DocumentAttache> DocumentsTransport { get; set; }

        public int DocumentsTransportCount
        {
            get {
                try
                {
                    if (DocumentsTransport!=null)
                    {
                        return DocumentsTransport.Count;
                    }
                }
                catch (Exception)
                {}
                return 0; 
            }
        }

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

        public int TailleFiles { get; set; }

        [Required(ErrorMessage = "Le champs entreprise est obligatoire !")]
        [ForeignKey("Client")]
        public int ClientId { get; set; }
        public virtual Client Client { get; set; }

        /// <summary>
        /// Une seule image est prise en charge
        /// </summary>
        public virtual ICollection<ImageInstruction> GetImageInstructions { get; set; }


        public bool EstPdf
        {
            get {
                try
                {
                    if(GetImageInstructions.ToList().Count>0)
                   return GetImageInstructions.ToList()[0].EstPdf;
                }
                catch (Exception)
                { }
                return false; 
            }
        }


        public string GetImageInstruction()
        {
            try
            {
                if (GetImageInstructions!=null && GetImageInstructions.Count > 0)
                    return GetImageInstructions.FirstOrDefault(i=>i.Url !="#").GetImage();
            }
            catch (Exception)
            { }

            return "#";
        }

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
                    try
                    {
                        if (this.DocumentsTransport.Count >0)
                        {
                            try
                            {
                                percentage += 100;
                                //percentage += (int)DocumentTransport.Percentage;
                            }
                            catch (Exception)
                            { }
                        }
                    }
                    catch (Exception)
                    { }
                    percentage = percentage / 7;
                }
                catch (Exception)
                {}
                if(percentage ==0)
                return percentage;
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
                infomesage = "Documents manquants: ";
                try
                {
                    short? s = null;
                    if (Justificatifs != null && Justificatifs.Count > 0)
                    {
                        foreach (var item in Justificatifs)
                        {
                            if (!string.IsNullOrEmpty(item.InfoPercentage))
                            {
                                s++;
                                infomesage += s + " - " + item.InfoPercentage + " ";
                            }
                        }
                    }
                    else { infomesage += s+" Factures "; s++; }
                    if (DeclarImport != null)
                    {
                        if (!string.IsNullOrEmpty(DeclarImport.InfoPercentage))
                        {
                            s++;
                            infomesage += s + " - " + DeclarImport.InfoPercentage + " ";
                        }
                    }
                    else
                    {
                        s++;
                        infomesage += s + " - Déclaration d'importation";
                    }
                    if (this.DomicilImport != null)
                    {
                        if (!string.IsNullOrEmpty(DomicilImport.InfoPercentage))
                        {
                            s++;
                            infomesage += s + " - " + DomicilImport.InfoPercentage + " ";

                        }                    
                    }
                    else
                    {
                        s++;
                        infomesage += s + " - Domiciliation d'importation ";
                    }
                    if (this.LettreEngage != null)
                    {
                        if (!string.IsNullOrEmpty(LettreEngage.InfoPercentage))
                        {
                            s++;
                            infomesage += s + " - " + LettreEngage.InfoPercentage + " ";

                        }                    
                    }
                    else if(!MarchandiseArrivee)
                    {
                        s++;
                        infomesage += s + " - Lettre d'engagement ";
                    }
                    if (this.QuittancePay != null)
                    {
                        if (!string.IsNullOrEmpty(QuittancePay.InfoPercentage))
                        {
                            s++;
                            infomesage += s + " - " + QuittancePay.InfoPercentage + "";

                        }                    
                    }
                    else if(MarchandiseArrivee || Apurement)
                    { s++; infomesage += s + " - Quittance "; }
                    if (this.GetImageInstruction() == null)
                    {
                        s++;
                        infomesage +=  s+" - Instruction " ;
                    }
                    if (this is Bien)
                    {
                        if (DocumentsTransport != null && DocumentsTransport.Count > 0)
                        {
                            try
                            {
                                s++;
                                infomesage += s + " - " + DocumentsTransport.FirstOrDefault().InfoPercentage;
                            }
                            catch (Exception)
                            { }
                        }
                        else if (MarchandiseArrivee || Apurement) { s++; infomesage += s + " - Documents de transport"; } 
                    }
                }
                catch (Exception)
                { }
                if (infomesage == "Documents manquants:  - " || infomesage== "Documents manquants: ") return "";
                return infomesage;
            }
        }

        public string GetDocsMonquants
        {
            get {
                try
                {
                   return !string.IsNullOrEmpty(this.InfoPercentage) ? this.InfoPercentage.Replace("Documents manquants: ", "") : "Aucun";
                }
                catch
                { }
                return "Aucun"; 
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

        [NotMapped]
        public bool Actions_content_visible { get; set; }

        public string GetColor
        {
            get { 
                if(this.Apurement)
                return GetCouleurApurement;
                return this.GetEtapDossier()[1];
            }
        }


        public string GetCouleurApurement
        {
            get {
                try
                {
                    if (this.EtapesDosier == 24)
                        return "#01d758";
                    if (this.EtapesDosier == 25)
                        return redColor;
                    if (this.EtapesDosier<0)
                    {
                        return redColor;
                    }
                    if (this.NatureOperation==NatureOperation.Service)
                    {
                        if (GetDelai > 14) return "#01d758";
                        else if (GetDelai > 7 && GetDelai <= 14)
                        {
                            return "orange";
                        }
                        else return redColor;
                    }
                    else
                    {
                        if (GetDelai >= 60) return "#01d758";
                        else if (GetDelai >= 30 && GetDelai < 60)
                        {
                            return "orange";
                        }
                        else return redColor;
                    }
                }
                catch (Exception)
                {}
                return ""; 
            }
        }

        public int GetDelai
        {
            get {
                try
                {
                    if (ObtDevise!=null)
                    {
                        if (this.NatureOperation == NatureOperation.Service)
                        {
                            return 30 - (dateNow - (DateTime)ObtDevise).Days;
                        }
                        else
                        {
                            return 90 - (dateNow - (DateTime)ObtDevise).Days;
                        } 
                    }
                }
                catch (Exception)
                { }
                return 0; 
            }
        }

        public IEnumerable<IGrouping<int?,StatutDossier>> GetStatuDossierBySite()
        {
            IEnumerable<IGrouping<int?, StatutDossier>> st = null;
            try
            {
                st = this.StatusDossiers.OrderBy(s=>s.Date).GroupBy(s => s.IdStructure);
            }
            catch (Exception)
            {}
            return st;
        }


        public string[] GetStatuDossierBySiteToString(int? etat_min,int etat_max)
        {
            string msg = "",msg2="",tmp="";
            var dateNow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "W. Central Africa Standard Time");
            if (etat_min == 7 && !this.EstPasséConformite)
                return new string[] { msg2, tmp };
            try
            {
                List<StatutDossier> strs = this.StatusDossiers.Where(s => etat_min <= s.Etat && etat_max >= s.Etat && s.Etat<=EtapesDosier).OrderBy(s => s.Date).ToList();
                int i = 0, duree = 0; DateTime df = DateTime.UtcNow, dd = dateNow;
                string temps = "";
                if (strs!=null && strs.Count>0)
                {
                    if (strs.First() != null)
                        msg2 = strs.First().ToString();

                    foreach (var item in strs)
                    {
                        try
                        {
                            msg = "";
                            if (etat_min == 0 || etat_min == 1)
                                msg = item.Statut1;
                            else
                                msg = item.Statut1.Split(':')[1];
                            if (etat_min == 2 || etat_min == 4)
                            {
                                //if (EstPasséConformite)
                                //    continue;
                            }
                            else if (etat_min == 7 && item.Etat == 9 && etat_max!=9 && !EstPasséConformite)
                                continue;
                            if (i == 0)
                            {
                                dd = item.Date;
                                df = dateNow;
                                duree = (df - item.Date).Days;
                            }
                            else
                            {
                                duree = Math.Abs((df - item.Date).Days);
                            }
                            if (duree == 0)
                            {
                                duree = Math.Abs((df - item.Date).Hours);
                                temps = duree + " heures";
                                if (duree == 0)
                                {
                                    duree = Math.Abs((df - item.Date).Minutes);
                                    temps = duree + " minutes";
                                    if (duree == 0)
                                    {
                                        duree = Math.Abs((df - item.Date).Seconds);
                                        temps = duree + " secondes";
                                    }
                                }
                            }
                            else
                            {
                                temps = duree + " jours";
                            }
                            i++;
                            tmp += $"<li class=\"menu-item\">"
                                   + "<a href = \"#\" class=\"menu-btn\" style=\"text-align:left\">"
                                   //+ "<i class=\"fa fa-folder-open\"></i>"
                                   + $"<span class=\"menu-text float-left\">{msg} : </span>"
                                   + $"<span class=\"menu-text float-end\">{item.Date.ToString("dd/MM/yyyy")}</span>"
                                     + "</a>"
                                + "</li>";
                            df = item.Date;
                        }
                        catch (Exception ee)
                        { }
                    } 
                }
                if (!string.IsNullOrEmpty(tmp))
                {
                    tmp = "<li class=\"menu-item menu-item-disabled\">"
                                          + "<button type = \"button\" class=\"menu-btn\">"
                                        + $"<span class=\"menu-text\" style=\"display:inline-block\" >Temps passé: {temps}</span>"
                                       + $"<span class=\"menu-text\" >{dd.Date.ToString("dd/MM/yyyy")} - {df.ToString("dd/MM/yyyy")}</span>"
                                   + "</button>"
                               + "</li>"
                                + tmp
                               + "<li class=\"menu-separator\"></li>";
                }
                else
                {
                    msg2 = null;
                }
            }
            catch (Exception)
            {}
            return new string[]{ msg2,tmp };
        }

        public string[] GetStatuDossierBySiteToStringApurement(List<int> etats)
        {
            string msg = "",msg2="",tmp="";
            var dateNow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "W. Central Africa Standard Time");

            try
            {
                List<StatutDossier> strs = new List<StatutDossier>();
                this.StatusDossiers.OrderBy(s => s.Date).ToList().ForEach(d =>
                {
                    if(etats.Contains((int)d.Etat))
                        strs.Add(d);
                });
                int i = 0, duree = 0; DateTime df = DateTime.UtcNow, dd = dateNow;
                string temps = "";
                if (strs!=null && strs.Count>0)
                {
                    if (strs.First() != null)
                        msg2 = strs.First().ToString();

                    foreach (var item in strs)
                    {
                        try
                        {
                            msg = "";
                            if (!item.Statut1.Contains(':'))
                                msg = item.Statut1;
                            else
                                msg = item.Statut1.Split(':')[1];

                            if (i == 0)
                            {
                                dd = item.Date;
                                df = dateNow;
                                duree = (df - item.Date).Days;
                            }
                            else
                            {
                                duree = Math.Abs((df - item.Date).Days);
                            }
                            if (duree == 0)
                            {
                                duree = Math.Abs((df - item.Date).Hours);
                                temps = duree + " heures";
                                if (duree == 0)
                                {
                                    duree = Math.Abs((df - item.Date).Minutes);
                                    temps = duree + " minutes";
                                    if (duree == 0)
                                    {
                                        duree = Math.Abs((df - item.Date).Seconds);
                                        temps = duree + " secondes";
                                    }
                                }
                            }
                            else
                            {
                                temps = duree + " jours";
                            }
                            i++;
                            tmp += $"<li class=\"menu-item\">"
                                   + "<a href = \"#\" class=\"menu-btn\" style=\"text-align:left\">"
                                   //+ "<i class=\"fa fa-folder-open\"></i>"
                                   + $"<span class=\"menu-text float-left\">{msg} : </span>"
                                   + $"<span class=\"menu-text float-end\">{item.Date.ToString("dd/MM/yyyy")}</span>"
                                     + "</a>"
                                + "</li>";
                            df = item.Date;
                        }
                        catch (Exception ee)
                        { }
                    } 
                }
                if (!string.IsNullOrEmpty(tmp))
                {
                    tmp = "<li class=\"menu-item menu-item-disabled\">"
                                          + "<button type = \"button\" class=\"menu-btn\">"
                                        + $"<span class=\"menu-text\" style=\"display:inline-block\" >Temps passé: {temps}</span>"
                                       + $"<span class=\"menu-text\" >{dd.Date.ToString("dd/MM/yyyy")} - {df.ToString("dd/MM/yyyy")}</span>"
                                   + "</button>"
                               + "</li>"
                                + tmp
                               + "<li class=\"menu-separator\"></li>";
                }
                else
                {
                    msg2 = null;
                }
            }
            catch (Exception)
            {}
            return new string[]{ msg2,tmp };
        }

        public int CurrentePosition()
        {
            if (!Apurement)
            {
                if (this.EtapesDosier >= 1 && this.EtapesDosier < 6 || EtapesDosier == -2)//AGENCE
                    return 1;
                if (this.EtapesDosier >= 6 && this.EtapesDosier < 9)//CONFORMITE
                    return 2;
                if (this.DFX6FP6BEAC == 1 && this.EtapesDosier >= 9 && (this.EtapesDosier < 19 && (DfxId == null || DfxId == 0)) || //TRANSFERT-DFX
                    this.DFX6FP6BEAC == 2 ||
                    this.DFX6FP6BEAC == 3 && this.EtapesDosier >= 9 && this.EtapesDosier < 15)//TRANSFERT-REF
                    return 3;
                if ((DFX6FP6BEAC == 3 || DFX6FP6BEAC == 1 && DfxId > 0) && this.EtapesDosier >= 15 && this.EtapesDosier < 19)//BEAC
                    return 4;
                if (EtapesDosier == 19 || EtapesDosier == 20 || EtapesDosier == 21)//Swft
                    return 5;
                if (EtapesDosier == 22 || EtapesDosier == 23)//Swft
                    return 6;
                return 0;
            }
            else
            {
                switch (EtapesDosier)
                {
                    case 22:
                    case 23:
                    case -230:
                    case -232:
                    case 25:
                    case -231://client
                        return 0;
                    case 230://Agence
                    case 231:
                    case 250:
                        return 1; 
                    case 232://Beac
                        return 2;
                    case 24://Apuré
                        return 3;
                    default:
                        break;
                }
                return 0;
            }
        }

        public string CurrenteStatut()
        {
            string st = "";
            try
            {
                var str = this.StatusDossiers.OrderByDescending(s => s.Date).First();
                return str.ToString();
            }
            catch (Exception)
            {}
            return st;
        }

        #region Les etsts
        public bool Apurement
        {
            get {
                try
                {
                    switch (this.EtapesDosier)
                    {
                        case 23:
                        case 24:
                        case 230:
                        case 231:
                        case 232:
                        case 250:
                        case -230:
                        case -231:
                        case -232:
                        case -250:
                            return true;
                        default:
                            break;
                    }
                }
                catch (Exception)
                { }
                return false; 
            }
        }

        public bool IsRejete
        {
            get {
                if (EtapesDosier < 0)
                    return true;
                return false; 
            }
        }


        public bool AApurer
        {
            get {
                try
                {
                    switch (this.EtapesDosier)
                    {
                        case 23:
                        case 230:
                        case 231:
                        case 232:
                        case 250:
                        case -230:
                        case -231:
                        case -232:
                        case -250:
                            return true;
                        default:
                            break;
                    }
                }
                catch (Exception)
                { }
                return false; 
            }
        }

        /// <summary>
        /// Apurer et à completer
        /// </summary>
        public bool AApurer_Ac
        {
            get {
                try
                {
                    switch (this.EtapesDosier)
                    {
                        case 23:
                        case -231:
                        case -232:
                        case -250:
                            return true;
                        default:
                            break;
                    }
                }
                catch (Exception)
                { }
                return false; 
            }
        }

        public bool AApurer_Av
        {
            get {
                try
                {
                    switch (this.EtapesDosier)
                    {
                        case 230:
                        case 231:
                        case 250:
                            return true;
                        default:
                            break;
                    }
                }
                catch (Exception)
                { }
                return false; 
            }
        }

        public bool Echu
        {
            get
            {
                try
                {
                    switch (this.EtapesDosier)
                    {
                        case 250:
                        case 25:
                        case -250:
                            return true;
                        default:
                            break;
                    }
                }
                catch (Exception)
                { }
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

        public GroupeWFDossier _GroupeWFDossier
        {
            get
            {
                try
                {
                    if (EtapesDosier == 26 || EtapesDosier == 27)
                    {
                        if ((dateNow - DateArchivage.Value).TotalDays > 365)
                            return GroupeWFDossier.Archive2;
                        return GroupeWFDossier.Archive;
                    }
                    if (!Apurement)
                        return GroupeWFDossier.Transfert;
                    if (AApurer)
                        return GroupeWFDossier.AApurer;
                    if (EtapesDosier == 24)
                        return GroupeWFDossier.Apurer;
                    if (Echu)
                        return GroupeWFDossier.Echus;
                }
                catch (Exception)
                {}
                return default;
            }
        }

        public int? EtapesDosierToString
        {
            get {
                try
                {
                    switch (_GroupeWFDossier)
                    {
                        case GroupeWFDossier.Null:
                            return null;
                        case GroupeWFDossier.Transfert:
                            return 1;
                        case GroupeWFDossier.AApurer:
                            return 2;
                        case GroupeWFDossier.Echus:
                            return 3;
                        case GroupeWFDossier.Apurer:
                            return 4;
                        case GroupeWFDossier.Archive:
                            return 5;
                        case GroupeWFDossier.Archive2://Si archive plus d'une annee
                            return 6;
                        default:
                            break;
                    }
                }
                catch (Exception)
                {}
                return null; 
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
        public string Get_EnDemeure
        {
            get
            {
                try
                {
                    if (EnDemeure != null)
                        return EnDemeure.GetImageDocumentAttache().GetImage();
                }
                catch (Exception)
                { }
                return "#";
            }
        }

        public string Get_EnDemeure2
        {
            get
            {
                try
                {
                    if (EnDemeure2 != null)
                        return EnDemeure2.GetImageDocumentAttache().GetImage();
                }
                catch (Exception)
                { }
                return "#";
            }
        }

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

        [Display(Name ="Montant en lettre")]
        public string MontantEnLettre { get;  set; }
        [Display(Name = "Code Swift")]
        public string CodeSwiftBic { get;  set; }

        public bool FormiteBEAC
        {
            get {
                try
                {
                    if (DFX6FP6BEAC != 2 && EtapesDosier < 15)
                        return true;
                }
                catch (Exception)
                { }
                return false; 
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
        Nulle,
        Bien,
        Service
    }
    
    /// <summary>
    /// DFX ou Refinancement
    /// </summary>
    public enum CategorieDossier
    {
        All,
        DFX,
        Refinanacement
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