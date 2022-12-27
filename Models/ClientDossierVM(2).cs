using System.ComponentModel;

namespace eApurement.Models
{
    public class ClientDossierVM
    {
        public string Client { get; set; }
        [DisplayName("Nbre de dossiers")]
        public int NbrDossiers { get; set; }
        public double Valeur { get; set; }
        public string Devise { get; set; }
        public int Id { get; set; }
    }
}