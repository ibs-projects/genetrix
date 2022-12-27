using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace genetrix.Models
{
    [DefaultProperty("Intitulé")]
    [Table("DocumentAttendus")]
    public class DocumentAttendus 
    { 
        public int Id { get; set; }

        private TypeDocumentAttaché typeDocumentAttaché;

        public TypeDocumentAttaché Intitulé
        {
            get { return typeDocumentAttaché; }
            set { typeDocumentAttaché = value; }
        }

        public string ReferenceBanqueId { get; set; }

        public ReferenceBanque Reference { get; set; }

    }
}