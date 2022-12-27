using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eApurement.Models
{
    public class ViewsModel
    {

    }
    
    public class ReferenceVModel
    {
        public int Id { get; set; }
        public string Numero { get; set; }
    }
    
    public class DossierVModel
    {
        public int Id { get; set; }
        public string Fournisseur { get; set; }
        public string Devise { get; set; }
        public double Montant { get; set; }
    }

    public class JustificatifVM
    {
        public int Id { get; set; }

        public string Fournisseur { get; set; }
        public double Montant { get; set; }
        public string Numero { get; set; }
        public int NbrePieces { get; set; }

        public string UtilisateurId { get; set; }
        public int ClientId { get; set; }

        public int DeviseId { get; set; }

        public bool EstDansEntreprise { get; set; }

        private string DossierId { get; set; }
    }
}