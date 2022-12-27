
using System.ComponentModel.DataAnnotations.Schema;

namespace genetrix.Models
{
    [Table("FormatRefInderne")]
    public class FormatRefInderne 
    {
        public int Id { get; set; }

        private string _matricule;

        public string CodeFormat
        {
            get { return _matricule; }
            set { _matricule = value; }
        }

        private int taille;
        public int CodeFormatTaile
        {
            get { return taille; }
            set { taille = value; }
        }

        [NotMapped]
        public string Format
        {
            get
            {
                short _base = 0;
                var nn = _base.ToString("D" + CodeFormatTaile);
                return CodeFormat + "" + nn;
            }
        }
    }
}