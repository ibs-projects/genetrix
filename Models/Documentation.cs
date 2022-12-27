using genetrix.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace genetrix.Models
{
    public class Documentation:DocumentAttache
    {

        [ForeignKey("Client")]
        public int? ClientId { get; set; } = null;

        public virtual Client Client { get; set; } = null;

        public int? IdFournisseur { get; set; } = null;
    }
}