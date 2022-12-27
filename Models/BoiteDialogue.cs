using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace genetrix.Models
{
    [NotMapped]
    public class BoiteDialogue 
    {
        private string text;

        public string Test
        {
            get { return text; }
            set { text = value; }
        }

    }
}