using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace genetrix.Models
{
    public class UsersExterne
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Nom complet")]
        public string NomComplet { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }

        [ForeignKey("Banque")]
        public int BanqueId { get; set; }
        public virtual Banque Banque { get; set; }
    }

    public class OperateurSwift : UsersExterne
    {

    }

    public enum Grade
    {
        A,
        B,
        C,
        D
    }

    public class Signataire : UsersExterne
    {
        public Grade Rang { get; set; }
        public string Fonction { get; set; }
    }

}