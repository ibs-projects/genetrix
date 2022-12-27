using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace e_apurement.Models
{
    [Table("StatutReference")]
    public class StatutReference 
    {
        public int Id { get; set; }

        private EtatDossier etat;

        public EtatDossier EtatDossier
        {
            get { return etat; }
            set { etat = value; }
        }

        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        private string titre;

        public string Titre
        {
            get { return titre; }
            set { titre = value; }
        }

        public byte[] Image { get; set; }

        private Color color;

        public Color Couleur
        {
            get { return color; }
            set { color = value; }
        }


    }
}