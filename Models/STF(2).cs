using e_apurement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eApurement.Models
{
    public class STF: Agence
    {
        [DisplayName("Non Service transfert")]
        public override string Nom { get; set; }
    }
}