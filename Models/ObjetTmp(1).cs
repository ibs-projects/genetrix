
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace e_apurement.Models
{
    [NotMapped]
    public class ObjetTmp 
    { 
      

        private int nmrJust;
        [DisplayName("Nom de justificatifs")]
        public int NombreJustificatifs
        {
            get { return nmrJust; }
            set { nmrJust = value; }
        }

        private int nbrPiece;
        [DisplayName("Nom de pages")]
        public int NombrePieces
        {
            get { return nbrPiece; }
            set { nbrPiece = value; }
        }


    }
}