using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace genetrix.Models
{
    public class DossierChange
    {
       public int? etat { get; set; }
        public int? idDossier { get; set; }
        public bool Estgroupe { get; set; }
        public int? idOpSwft { get; set; }
        public int? idRef { get; set; }
        public string message { get; set; }
        public string date { get; set; }
        public string itemsRejet { get; set; }
    }
    public class ViewsModel
    {

    }
    
    public class RapportMontantDevise
    {
        public double MontantTotal { get; set; }
        public double TotalXaf { get; set; }
        public string Devise { get; set; }
        public int? Etat { get; set; }
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
        public string MontantString { get; set; }
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

    public class UserVM
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string NomComplet { get; set; }
        public int SiteId { get; set; }
        public Groupe Groupe { get; set; }
    }
    public class ChatVM
    {
        public int Id { get; set; }
        public string Sujet { get; set; }
        public int ClientId { get; set; }
        public StatutChat StatutChat { get; set; }
        public Situation Situation { get; set; }
        public string StatutName { get; set; }
        public string StatutClass { get; set; }
        public string SituationName { get; set; }
        public string SituationClass { get; set; }
    }

    public class AgentVM
    {
        public string Id { get; set; }
        public string NomComplet { get; set; }
    }
    
    public class ClientVM
    {
        public int Id { get; set; }
        public string Nom { get; set; }
    }
}