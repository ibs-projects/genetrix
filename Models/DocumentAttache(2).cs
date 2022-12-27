using e_apurement.Models.Fonctions;
using eApurement.Models;
using eApurement.Models.Fonctions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace e_apurement.Models
{
    [DefaultProperty("Nom")]
    [Table("DocumentAttache")]
    public class DocumentAttache: IDelateCustom
    {
        public int Id { get; set; }

        public string Nom { get; set; }

        private int nbr;
        [Display(Name="Nombre de document")]
        //[Required(ErrorMessage = "Le nombre de document doit être specifié !")]
        public int NomBreDoc { get; set; }

        public ClFichier FichierImage { get; set; }

        //[ForeignKey("ImageDocumentAttache")]
        //public int? ImageDocumentAttacheId { get; set; } = null;

        /// <summary>
        /// Une image est ajoutée
        /// </summary>
        public virtual ICollection<ImageDocumentAttache> GetImageDocumentAttaches { get; set; }

        public ImageDocumentAttache GetImageDocumentAttache()
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
            catch (Exception)
            { }
            try
            {
                db.GetDocumentAttaches.Remove(this);
                return true;
            }
            catch (Exception)
            {}
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
        AutreDoc
    }
}