using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using genetrix.Models;
using System.Linq;
using DisplayNameAttribute = System.ComponentModel.DisplayNameAttribute;
using genetrix.Models.Fonctions;
using System.Globalization;

namespace genetrix.Models
{
    [DefaultProperty("Intitulé")]
    [Table("Justificatif")]
    public class Justificatif : IDelateCustom
    {
        public bool Remove(ApplicationDbContext db)
        {
            try
            {
                if(this.GetImages!=null)
                db.GetAllImages.Remove(this.GetImages.ToList()[0]);
                db.GetJustificatifs.Remove(this);
                return true;
            }
            catch (Exception)
            { }
            return false;
        }

        public int Id { get; set; }

        [NotMapped]
        public byte EstAncienneFactMoins6mois { get; set; }

        public bool EstPartielle { get; set; }

        [ForeignKey("AncienneFacture")]
        public int? IdAncienneFacture { get; set; }

        public virtual Justificatif AncienneFacture { get; set; }

        public virtual ICollection<FacturePiece> FacturePieces { get; set; }
        public virtual ICollection<Justificatif> FacturesPartielles { get; set; }

        public double TotalResteFacturesPartielles
        {
            get {
                try
                {
                   return AncienneFacture.FacturesPartielles.Sum(f => f.MontantRestant);
                }
                catch (Exception)
                { }
                return 0; 
            }
        }

        public double TotalRestePayer
        {
            get {
                try
                {
                    return DossiersReglParts.Sum(d => d.Montant);
                }
                catch (Exception)
                { }
                return 0; 
            }
        }


        public string Intitulé
        {
            get
            {
                return $"{NumeroJustif}";
            }
        }

        private string lib;

        public string Libellé
        {
            get { return lib; }
            set { lib = value; }
        }

        [DisplayName("Fournisseur")]
        public string FournisseurJustif { get; set; }

        private int fournId;
        [NotMapped]
        public int FournisseurId
        {
            get {
                try
                {
                    fournId=(int)Dossier.FournisseurId;
                    return fournId;
                }
                catch (Exception)
                {}
                return fournId; 
            }
            set { fournId = value; }
        }
   
        public string Devise
        {
            get {
                try
                {
                    return Dossier.DeviseToString;
                }
                catch (Exception)
                {}
                return ""; 
            }
        }

        [DisplayName("Banque")]
        public string BanqueJustif { get; set; }

        [DisplayName("Montant total")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}", ApplyFormatInEditMode = true)]
        public double MontantJustif { get; set; }

        [DisplayName("Montant payé")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}", ApplyFormatInEditMode = true)]
        public double MontantPartiel { get; set; }

        private double _reste;

        [DisplayName("Montant restant")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}", ApplyFormatInEditMode = true)]
        public double MontantRestant
        {
            get {
                double reste = 0;
                try
                {
                    reste=MontantJustif - MontantPaye;
                }
                catch (Exception)
                { }
                return Math.Round(reste,2); 
            }
            set { _reste = value; }
        }

        [DisplayName("Montant restant")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}", ApplyFormatInEditMode = true)]
        public double MontantRestant2
        {
            get {
                try
                {
                    if (this.AncienneFacture != null)
                    {
                        return AncienneFacture.MontantRestant;
                    }
                    else
                    {
                        return this.MontantRestant;
                    }
                }
                catch (Exception)
                { }
                return MontantRestant; 
            }
        }

        public string MontantString
        {
            get
            {
                try
                {
                    int _NumberDecimalDigits = 0;
                    try
                    {
                        if (Dossier.Montant % 1 > 0)
                        {
                            //_NumberDecimalDigits = Dossier.Montant.ToString().Split(',')[1].Length;
                            _NumberDecimalDigits = 2;
                        }
                    }
                    catch (Exception)
                    { }
                    NumberFormatInfo nfi = new NumberFormatInfo { NumberGroupSeparator = " ", NumberDecimalDigits = _NumberDecimalDigits   }; 
                    return MontantJustif.ToString("n", nfi);
                }
                catch (Exception)
                { }
                return "0";
            }
        }
         
        public string MontantPartielString
        {
            get
            {
                try
                {
                    int _NumberDecimalDigits = 0;
                    try
                    {
                        if (Dossier.Montant % 1 > 0)
                        {
                            // _NumberDecimalDigits = Dossier.Montant.ToString().Split(',')[1].Length;
                            _NumberDecimalDigits = 2;
                        }
                    }
                    catch (Exception)
                    { }
                    NumberFormatInfo nfi = new NumberFormatInfo { NumberGroupSeparator = " ", NumberDecimalDigits =_NumberDecimalDigits   }; 
                    return MontantPartiel.ToString("n", nfi);
                }
                catch (Exception)
                { }
                return "0";
            }
        }

        public string MontantRestantString
        {
            get
            {
                try
                {
                    int _NumberDecimalDigits = 0;
                    try
                    {
                        if (MontantRestant % 1 > 0)
                        {
                            _NumberDecimalDigits = 2;
                            //_NumberDecimalDigits = MontantRestant.ToString().Split(',')[1].Length;
                        }
                    }
                    catch (Exception)
                    { }
                    NumberFormatInfo nfi = new NumberFormatInfo { NumberGroupSeparator = " ", NumberDecimalDigits = _NumberDecimalDigits   }; 
                    //return MontantRestant.ToString("n", nfi).Replace('.',',');
                    return MontantRestant.ToString("n", nfi);
                }
                catch (Exception)
                { }
                return "0";
            }
        }

        public double MontantPaye
        {
            get {
                double mtp = 0;
                try
                {
                    if (DossiersReglParts != null)
                        DossiersReglParts.ToList().ForEach(d =>
                        {
                            mtp += d.Montant;
                        });
                }
                catch (Exception)
                {}
                return Math.Round(mtp, 2); 
            }
        }


        public string MontantPayeString
        {
            get
            {
                try
                {
                    int _NumberDecimalDigits = 0;
                    try
                    {
                        if (MontantPaye % 1 > 0)
                        {
                            _NumberDecimalDigits = 2;
                            //_NumberDecimalDigits = Dossier.Montant.ToString().Split(',')[1].Length;
                        }
                    }
                    catch (Exception)
                    { }
                    NumberFormatInfo nfi = new NumberFormatInfo { NumberGroupSeparator = " ", NumberDecimalDigits = _NumberDecimalDigits   }; 
                    return MontantPaye.ToString("n", nfi);
                    //return MontantRestant.ToString("n", nfi);
                }
                catch (Exception)
                { }
                return "0";
            }
        }

        [DisplayName("N° justification")]
        [Required]
        public string NumeroJustif { get; set; }

        [DisplayName("Date d'émission")]
        public DateTime DateEmissioJustif { get; set; }

        [DisplayName("Date numérisation")]
        public DateTime DateCreaAppJustif { get; set; }

        [DisplayName("Dernière modification")]
        public DateTime DateModifJustif { get; set; }

        [DisplayName("Nombre pièces jointes")]
        //[Required]
        //public int NbrePieces { get; set; }

        public int NbrePieces
        {
            get {
                try
                {
                    if (AncienneFacture == null)
                        return GetImages.Count;
                    else return AncienneFacture.NbrePieces;
                }
                catch (Exception)
                { }
                return 0; 
            }
        }


        [NotMapped]
        public int CompteurPieces { get; set; }
        
        [NotMapped]
        public int CompteurFactures { get; set; }

         [NotMapped]
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}", ApplyFormatInEditMode = true)]
        public double MontantTotalDossier { get; set; }

        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}", ApplyFormatInEditMode = true)]
        public string MontantTotalDossierString2 { get; set; }

        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:#,##0.00#}", ApplyFormatInEditMode = true)]
        public string MontantTotalDossierString
        {
            get
            {
                try
                {
                    int _NumberDecimalDigits = 0;
                    string mm = "";
                    try
                    {
                        if (Dossier.Montant % 1 > 0)
                        {
                            _NumberDecimalDigits = Dossier.Montant.ToString().Split(',')[1].Length;
                        }
                    }
                    catch (Exception)
                    { }
                    NumberFormatInfo nfi = new NumberFormatInfo { NumberGroupSeparator = " ", NumberDecimalDigits = _NumberDecimalDigits   };
                    try
                    {
                        mm= MontantTotalDossier.ToString("n", nfi);
                        mm = mm.Replace('.', ',');
                    }
                    catch (Exception)
                    {}
                    return mm;
                }
                catch (Exception)
                { }
                return "0";
            }
            set { }
        }

        //public virtual ICollection<ImageJustificatif> GetImages{ get; set; }

        ICollection<ImageJustificatif> images;
        public virtual ICollection<ImageJustificatif> GetImages
        {
            get { 
               
                return images; 
            }
            set { images = value; }
        }

        [NotMapped]
        public virtual ICollection<ImageJustificatif> Fichiers
        {
            get {
                if (EstPartielle || this.MontantRestant > 0)
                {
                    try
                    {
                        if (AncienneFacture != null)
                        {
                            return AncienneFacture.GetImages;
                        }
                    }
                    catch (Exception ee)
                    { }
                }
                else
                {
                    return this.GetImages;
                }
                return new List<ImageJustificatif>(); 
            }
        }


        public string UtilisateurId { get; set; }

        public bool EstPdf
        {
            get
            {
                try
                {
                    return GetImages.ToList()[0].EstPdf;
                }
                catch (Exception)
                { }
                return false;
            }
        }

        [Required]
        [DisplayName("Devise")]
        public string DeviseJustif { get; set; }
        public int IdDevise { get; set; }

        //[ForeignKey("Dossier")]
        //[Required]
        [NotMapped]
        public int DossierId { get; set; }
        //public virtual Dossier Dossier { get; set; }

        public Dossier Dossier
        {
            get {
                try
                {
                    if (DossiersReglParts != null)
                       return DossiersReglParts.FirstOrDefault();
                }
                catch (Exception)
                {}
                return null; 
            }
        }

        public virtual ICollection<Dossier> DossiersReglParts { get; set; }

        public virtual ICollection<PieceJointe> PieceJointes { get; set; }

        double percentage;
        [Browsable(false)]
        public double Percentage
        {
            get
            {
                try
                {
                    percentage = 0;
                    if (GetImages == null || GetImages.Count == 0)
                        percentage = 50;
                    else
                    {
                        if (NbrePieces > GetImages.Count)
                            percentage = Math.Round((double)((NbrePieces - GetImages.Count) * 5 / 10),2) + 50;
                        else
                            percentage = 100;
                    }

                }
                catch (Exception)
                { }
                return percentage;
            }
        }

        string infoPercentage;
        [Browsable(false)]
        public string InfoPercentage
        {
            get {
                try
                {
                    infoPercentage = "";
                    if (GetImages == null || GetImages.Count == 0)
                        infoPercentage = "> Facture - " + NumeroJustif + ": manque de pièces jointes";
                    else
                    {
                        if (NbrePieces > GetImages.Count)
                            infoPercentage = "> Facture - " + NumeroJustif + ": manque " + (NbrePieces - GetImages.Count) + " pièces jointes";
                    }

                }
                catch (Exception)
                { }
                return infoPercentage;
            }
        }

        [NotMapped]
        public IDictionary<string, bool> Permissions
        {
            get { return Dossier.Permissions; }
        }

        public int? IdClient { get; set; }

        protected  void OnSaving()
        {

        }
    }
}