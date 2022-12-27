using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using DisplayNameAttribute = System.ComponentModel.DisplayNameAttribute;

namespace genetrix.Models
{
    [Table("DocumentAttacheDossier")]
    public class DocumentAttacheDossier : ClFichier
    {
        //public string Intitulé
        //{
        //    get
        //    {
        //        return $"Document attaché: {documentAttache}-{Dossier}";
        //    }
        //}

        //DocumentAttache documentAttache;
        //[Association]
        //public DocumentAttache DocumentAttache
        //{
        //    get { return documentAttache; }
        //    set { SetPropertyValue(nameof(DocumentAttache), ref documentAttache, value); }
        //}

        //Dossier dossier;
        //[Association]
        //public Dossier Dossier
        //{
        //    get { return dossier; }
        //    set { SetPropertyValue(nameof(Dossier), ref dossier, value); }
        //}
    }
}