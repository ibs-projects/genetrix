using genetrix.Models.Fonctions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace genetrix.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Courrier électronique")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Mémoriser ce navigateur ?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Courrier électronique")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Courrier électronique")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Nom d'utilisateur")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; }

        [Display(Name = "Mémoriser le mot de passe ?")]
        public bool RememberMe { get; set; }
        [Required]
        public string NumeroEntreprise { get; set; }
        public int IdAgence { get; set; }
        public int IdRole { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Courrier électronique")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Nom d'utilisateur")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "La chaîne {0} doit comporter au moins {2} caractères.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; }

        public string Prenom { get; set; }
        [Display(Name ="Civilité")]
        public Sexe Sexe { get; set; }
        public string Nom { get; set; }

        [Display(Name = "Est back-office")]
        public bool EstBackOff { get; set; }

        public List<XtraRole> Roles { get; set; }
        public List<Structure> Sites { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "Confirmer le mot de passe ")]
        [Compare("Password", ErrorMessage = "Le mot de passe et le mot de passe de confirmation ne correspondent pas.")]
        public string ConfirmPassword { get; set; }

        #region Entreprise
        [Required]
        public string NomEntreprise { get; set; }
        public string Adresse { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string Telephone { get; set; }

        public string NIF { get; set; }
        [Display(Name = "Est administrateur")]
        public bool EstAdmin { get; set; }
        public int? IdAgence { get; set; }
        public int? IdRole { get; set; }
        public bool EstGestionnaire { get; set; }
        #endregion

        #region Client attributs
        public int? IdClientRole { get; set; }

        [Display(Name = "Peut créer un dossier")]
        public bool CreerDossier { get; set; }
        [Display(Name = "Peut soumettre un dossier")]
        public bool SoumettreDossier { get; set; }
        [Display(Name = "Peut créer un utilisateur")]
        public bool CreerUser { get; set; }
        [Display(Name = "Peut supprimer un utilisateur")]
        public bool SuppUser { get; set; }
        [Display(Name = "Peut modifier un utilisateur")]
        public bool ModifUser { get; set; }
        [Display(Name = "Peut créer un fournisseur")]
        public bool CreerBenef { get; set; }
        [Display(Name = "Peut supprimer un fournisseur")]
        public bool SuppBenef { get; set; }
        [Display(Name = "Peut modifier un fournisseur")]
        public bool ModifBenef { get; set; }
        #endregion
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Courrier électronique")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "La chaîne {0} doit comporter au moins {2} caractères.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmer le mot de passe")]
        [Compare("Password", ErrorMessage = "Le nouveau mot de passe et le mot de passe de confirmation ne correspondent pas.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; }
    }
}
