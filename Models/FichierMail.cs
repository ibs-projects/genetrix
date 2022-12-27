using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace genetrix.Models
{
    public class FichierMail
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        [ForeignKey("Mail")]
        public int IdMail { get; set; }
        public virtual Mail Mail { get; set; }
        public bool EstPdf
        {
            get
            {
                try
                {
                    if (Url.Contains(".pdf"))
                        return true;
                }
                catch (Exception)
                { }
                return false;
            }
        }
    }
}