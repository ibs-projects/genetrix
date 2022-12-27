using genetrix.Models.Fonctions;
using genetrix.Models;
using genetrix.Models.Fonctions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace genetrix.Models
{
    [DefaultProperty("Nom")]
    [Table("DocumentAttache")]
    public class DocumentAttache: IDelateCustom
    {
        public int Id { get; set; }

        public string Nom { get; set; }

        [NotMapped]
        public int typeDocument { get; set; }

        public DateTime? DateCreation { get; set; } = DateTime.Now;
        public bool EstAttestation { get; set; }
        public DateTime? DateSignature { get; set; }

        [Display(Name ="Date d'expiration du document")]
        public DateTime? DateExpiration
        {
            get {
                try
                {
                    return DateSignature.Value.AddDays(29);
                }
                catch (Exception)
                { }
                return default; 
            }
        }

        public int? IdBanqueTierce { get; set; }

        public string DateExpirationToString
        {
            get
            {
                try
                {
                    return DateExpiration.Value.ToString("dd/MM/yyyy");
                }
                catch (Exception)
                { }
                return "";
            }
        }
        
        public string DateSignatureToString
        {
            get
            {
                try
                {
                    return DateSignature.Value.ToString("dd/MM/yyyy");
                }
                catch (Exception)
                { }
                return "";
            }
        }
        
        public string DateCreationToString
        {
            get
            {
                try
                {
                    return DateCreation.Value.ToString("dd/MM/yyyy");
                }
                catch (Exception)
                { }
                return "";
            }
        }

        private int nbr;
        [Display(Name="Nombre de document")]
        //[Required(ErrorMessage = "Le nombre de document doit être specifié !")]
        public int NomBreDoc { get; set; }

        public ClFichier FichierImage { get; set; }

        [Display(Name = "Signature 1")]
        public bool Signature1 { get; set; }

        [Display(Name = "Signature 2")]
        public bool Signature2 { get; set; }

        public bool EstPdf
        {
            get
            {
                try
                {
                    return GetImageDocumentAttaches.ToList()[0].EstPdf;
                }
                catch (Exception)
                { }
                return false;
            }
        }
        public bool AttestSurHonneur { get; set; }

        /// <summary>
        /// Une image est ajoutée
        /// </summary>
        public virtual ICollection<ImageDocumentAttache> GetImageDocumentAttaches { get; set; }

        public virtual ImageDocumentAttache GetImageDocumentAttache()
        {
            try
            {
                if (GetImageDocumentAttaches.Count > 0)
                    return GetImageDocumentAttaches.ToList()[0];
            }
            catch (Exception)
            { }

            return new ImageDocumentAttache();
        }

        public bool Remove(ApplicationDbContext db)
        {
            try
            {
                if (GetImageDocumentAttaches != null)
                    GetImageDocumentAttaches.ToList()[0].Remove(db);
            }
            catch (Exception e)
            { }
            //try
            //{
            //    db.GetDocumentAttaches.Remove(this);
            //    return true;
            //}
            //catch (Exception)
            //{}
            return false;
        }

        private bool memeEntreprise;
        public bool EstDansEntreprise
        {
            get
            {
                try
                {
                    //if (SecuritySystem.CurrentUser is CompteClient && (SecuritySystem.CurrentUser as CompteClient).Client.Id == Client.Id)
                    //    return true;
                    //else if (SecuritySystem.CurrentUser is CompteBanqueCommerciale)
                    //    foreach (var b in Client.Banques)
                    //        if(b.Banque.Id==(SecuritySystem.CurrentUser as CompteBanqueCommerciale).Banque.Id)
                    //            return true;

                }
                catch (Exception)
                { }
                return memeEntreprise;
            }
        }

        [ForeignKey("Dossier")]
        public int? DossierId { get; set; } = null;
        public virtual Dossier Dossier { get; set; }
        
        /// <summary>
        /// Pour l'ajout des documents concernant la reference banque: courrier, MT, etc..
        /// </summary>
        public int? IdReference { get; set; } = null;
        [Display(Name ="Type du document")]
        private TypeDocumentAttaché typeDocumentAttaché;

        public TypeDocumentAttaché TypeDocumentAttaché
        {
            get { return typeDocumentAttaché; }
            set { typeDocumentAttaché = value; }
        }

        double percentage;
        [Browsable(false)]
        public double Percentage {
            get {
                try
                {
                    percentage = 0;
                    if (GetImageDocumentAttache() == null)
                    {
                        percentage = 50;
                    }
                    else
                    {
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
                    if (GetImageDocumentAttache() == null)
                        infoPercentage = "> " + Nom + " - manque de la pièce jointe";
                }
                catch (Exception)
                { }
                return infoPercentage; 
            }
        }

        [NotMapped]
        public int IdClient { get; set; }
    }

    public enum TypeDocumentAttaché
    {
        [Display(Name="Déclaration d'importation")]
        DeclarImportBien,
        [Display(Name = "Domiciliation d'importation")]
        DomicilImport,
        [Display(Name = "Lettre d'engagement")]
        LettreEngagement,
        [Display(Name = "Quittance de paiement")]
        QuittancePayement,
        [Display(Name = "Document de transport")]
        DocumentTransport,
        [Display(Name = "Autre document")]
        AutreDoc,
        MT,
        Courrier,
        EnDemeure,
        AttestationNonDefautApurement
    }
}