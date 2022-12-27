using genetrix.Models;
using genetrix.Models.Fonctions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace genetrix.Models
{
    public class Fournisseurs : IDelateCustom
    {
        public bool Remove(ApplicationDbContext db)
        {
            try
            {
                db.GetFournisseurs.Remove(this);
                return true;
            }
            catch (Exception)
            { }
            return false;
        }

        public int Id { get; set; }

        [Display(Name ="Nom complet du bénéficiaire")]
        public string Nom { get; set; }
        public string Pays { get; set; }

        [Display(Name = "Téléphone 2")]
        public string Tel2 { get; set; }

        [Display(Name = "Téléphone")]
        public string Tel1 { get; set; }
        
        [Display(Name = "Fax/Télécopie")]
        public string Fax { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Adresse mail")]
        public string Email { get; set; }

        public string Ville { get; set; }

        public string Adresse { get; set; }
        [Display(Name = "Code Etablissement"), StringLength(5)]
        public string CodeEts { get; set; }

        [ForeignKey("Client")]
        public int ClientId { get; set; }
        public Client Client { get; set; }

        [NotMapped]
        public int IdDossier { get; set; }

        #region Documentation
        [DisplayName("Extrait RCCM ou autre document tenant lieu")]
        public virtual Documentation RCCM { get; set; }

        [DisplayName("Liste des actionnaires ou promoteurs et répartition du capital")]
        public virtual Documentation ListeGerants { get; set; } 

        //Ajout
        [DisplayName("Copie des statuts authetifiés par une autorité abilité")]
        public virtual Documentation CopieStatuts { get; set; } 
        
        [DisplayName("Liste des ayant droits finaux personnes physiques")]
        public virtual Documentation ListeAyantDroits { get; set; } 
        
        [DisplayName("Copie du procès-verbal nommant les dirigeants ou tout autre document en tenant lieu")]
        public virtual Documentation ProcesVerbalNommantDirigeants { get; set; } 
        
        [DisplayName("Copie de la carte nationale d'identité ou passeport des dirigeants")]
        public virtual Documentation CarteIdentiteDirigeants { get; set; }

        [DisplayName("Fiche ou attestation KYC (connaissance du client) ou tout document en tenant lieu établie par la banque du bénéficiaire de l'existant et de la regularité du compte au regard des dispositions relatives à la lutte contre le blanchissement des capitaux")]
        public virtual DocumentAttache FicheKYCBenefi { get; set; }

        //Personne physique
        [Display(Name ="Prise d'acte sur la déclaration du compte à la banque centrale pour les bénéficiaires resident de la CEMAC")]
        public virtual Documentation PriseActeDeclarationCompte { get; set; }

        [Display(Name = "Justificatifsn de domicile (facture d'eau ou d'électricité)")]
        public virtual DocumentAttache JustifDomicileBenefi { get; set; }

        public string Get_CopieStatuts
        {
            get
            {
                try
                {
                    if (CopieStatuts != null)
                        return CopieStatuts.GetImageDocumentAttache().GetImage();
                }
                catch (Exception)
                { }
                return "#";
            }
        }
        
        public string Get_ListeAyantDroits
        {
            get
            {
                try
                {
                    if (ListeAyantDroits != null)
                        return ListeAyantDroits.GetImageDocumentAttache().GetImage();
                }
                catch (Exception)
                { }
                return "#";
            }
        }
        
        public string Get_ProcesVerbalNommantDirigeants
        {
            get
            {
                try
                {
                    if (ProcesVerbalNommantDirigeants != null)
                        return ProcesVerbalNommantDirigeants.GetImageDocumentAttache().GetImage();
                }
                catch (Exception)
                { }
                return "#";
            }
        }
        
        public string Get_CarteIdentiteDirigeants
        {
            get
            {
                try
                {
                    if (CarteIdentiteDirigeants != null)
                        return CarteIdentiteDirigeants.GetImageDocumentAttache().GetImage();
                }
                catch (Exception)
                { }
                return "#";
            }
        }
        
        public string Get_FicheKYCBenefi
        {
            get
            {
                try
                {
                    if (FicheKYCBenefi != null)
                        return FicheKYCBenefi.GetImageDocumentAttache().GetImage();
                }
                catch (Exception)
                { }
                return "#";
            }
        }
        
        public string Get_PriseActeDeclarationCompte
        {
            get
            {
                try
                {
                    if (PriseActeDeclarationCompte != null)
                        return PriseActeDeclarationCompte.GetImageDocumentAttache().GetImage();
                }
                catch (Exception)
                { }
                return "#";
            }
        }
        
        public string Get_JustifDomicileBenefi
        {
            get
            {
                try
                {
                    if (JustifDomicileBenefi != null)
                        return JustifDomicileBenefi.GetImageDocumentAttache().GetImage();
                }
                catch (Exception)
                { }
                return "#";
            }
        }

        public int CopieStatutsId
        {
            get
            {
                try
                {
                    if (CopieStatuts != null)
                        return CopieStatuts.Id;
                }
                catch (Exception)
                { }
                return 0;
            }
        }

        public int ListeAyantDroitsId
        {
            get
            {
                try
                {
                    if (ListeAyantDroits != null)
                        return ListeAyantDroits.Id;
                }
                catch (Exception)
                { }
                return 0;
            }
        }

        public int ProcesVerbalNommantDirigeantsId
        {
            get
            {
                try
                {
                    if (ProcesVerbalNommantDirigeants != null)
                        return ProcesVerbalNommantDirigeants.Id;
                }
                catch (Exception)
                { }
                return 0;
            }
        }

        public int CarteIdentiteDirigeantsId
        {
            get
            {
                try
                {
                    if (CarteIdentiteDirigeants != null)
                        return CarteIdentiteDirigeants.Id;
                }
                catch (Exception)
                { }
                return 0;
            }
        }
        public int FicheKYCBenefiId
        {
            get
            {
                try
                {
                    if (FicheKYCBenefi != null)
                        return FicheKYCBenefi.Id;
                }
                catch (Exception)
                { }
                return 0;
            }
        }
        public int PriseActeDeclarationCompteId
        {
            get
            {
                try
                {
                    if (PriseActeDeclarationCompte != null)
                        return PriseActeDeclarationCompte.Id;
                }
                catch (Exception)
                { }
                return 0;
            }
        }
        public int JustifDomicileBenefiId
        {
            get
            {
                try
                {
                    if (JustifDomicileBenefi != null)
                        return JustifDomicileBenefi.Id;
                }
                catch (Exception)
                { }
                return 0;
            }
        }
        #endregion

        public virtual ICollection<Dossier> Dossiers { get; set; }
        public virtual ICollection<NumCompteBeneficiaire> CompteBeneficiaires { get; set; }
        [DisplayName("Autre document")]
        public virtual ICollection<DocumentAttache> AutresDocuments { get; set; } = new List<DocumentAttache>();
        public int DocumentAttachesCount
        {
            get
            {
                try
                {
                    if (AutresDocuments != null)
                    {
                        return AutresDocuments.Count;
                    }
                }
                catch (Exception)
                { }
                return 0;
            }
        }
        public int ListeGerantsId
        {
            get
            {
                try
                {
                    if (ListeGerants != null)
                        return ListeGerants.Id;
                }
                catch (Exception)
                { }
                return 0;
            }
        }
        public int RCCMId
        {
            get
            {
                try
                {
                    if (RCCM != null)
                        return RCCM.Id;
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
                    if (RCCM != null)
                        return RCCM.GetImageDocumentAttache().GetImage();
                }
                catch (Exception)
                { }
                return "#";
            }
        }
        public string Get_ListeGerants
        {
            get
            {
                try
                {
                    if (ListeGerants != null)
                        return ListeGerants.GetImageDocumentAttache().GetImage();
                }
                catch (Exception)
                { }
                return "#";
            }
        }

    }
}