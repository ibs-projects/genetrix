using genetrix.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;

namespace genetrix.Models
{
    [DefaultProperty("EtatDossier")]
    [Table("StatutDossier")]
    public class StatutDossier
    {
        [Key]
        [Column(Order =1)]
        [ForeignKey("Statut")]
        public int? IdStatut { get; set; }
        public virtual Statut Statut { get; set; }
        [ForeignKey("Dossier")]
        [Key]
        [Column(Order = 2)]
        public int? IdDossier { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column(Order = 3)]
        public int Id { get; set; }
        public virtual Dossier Dossier { get; set; }
        public int? Etat { get; set; }
        public DateTime Date { get; set; }

        public string DateToString
        {
            get
            {
                try
                {
                    return Date.ToString("dd/MM/yyyy");
                }
                catch (Exception)
                { }
                return "";
            }
        }

        public int? IdStructure { get; set; }
        public string IdAgent { get; set; }
        //en rapport avec le client
        public string Objet { get; set; }
        //en rapport avec le responsable du dossier
        public string Objet2 { get; set; }
        //Message du client
        public string Message { get; set; }
        //Message du responsable du dossier aupres de la banque: ex. gestionnaire
        public string Message2 { get; set; }

        //Message personnalisé de l'utilisateur lors de l'envoie du dossier
        public string Motif { get; set; }
        public bool EstConformite { get; set; }

        public string Couleur
        {
            get {
                try
                {
                   return Statut.CouleurSt;
                }
                catch (Exception)
                {}
                return ""; 
            }
        }

        public string Statut1 { get; set; }
       
        public override string ToString()
        {
            return $"{Statut1}, {Message}<br><br>{Date}";
        }
    }
}