using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using e_apurement.Models;
using System.Linq;
using DisplayNameAttribute = System.ComponentModel.DisplayNameAttribute;
using eApurement.Models;
using eApurement.Models.Fonctions;

namespace e_apurement.Models
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

        [DisplayName("Banque")]
        public string BanqueJustif { get; set; }

        [DisplayName("Montant")]
        public double MontantJustif { get; set; }

        [DisplayName("N° justification")]
        [Required]
        public string NumeroJustif { get; set; }

        [DisplayName("Date d'émission")]
        public DateTime DateEmissioJustif { get; set; }

        [DisplayName("Date numérisation")]
        public DateTime DateCreaAppJustif { get; set; }

        [DisplayName("Dernière modif")]
        public DateTime DateModifJustif { get; set; }

        [DisplayName("Nombre pièces jointes")]
        //[Required]
        public int NbrePieces { get; set; }

        [NotMapped]
        public int CompteurPieces { get; set; }
        
        [NotMapped]
        public int CompteurFactures { get; set; }

        public virtual ICollection<ImageJustificatif> GetImages{ get; set; }

        public string UtilisateurId { get; set; }

        [Required]
        [DisplayName("Devise")]
        public string DeviseJustif { get; set; }
        public int IdDevise { get; set; }

        [ForeignKey("Dossier")]
        [Required]
        public int DossierId { get; set; }
        public virtual Dossier Dossier { get; set; }

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


        protected  void OnSaving()
        {

        }
    }
}