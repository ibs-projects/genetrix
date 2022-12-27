using eApurement.Models;
using genetrix;
using genetrix.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace e_apurement.Models
{
    [Table("CompteCentraleBanque")]
    public class CompteCentraleBanque : ApplicationUser
    {
        public string Id { get; set; }

        private bool memeEntreprise;
        public bool EstDansEntreprise
        {
            get
            {
                try
                {
                    if (SecuritySystem.CurrentUser is CompteCentraleBanque)
                        return true;
                }
                catch (Exception)
                { }
                return memeEntreprise;
            }
        }


    }
}