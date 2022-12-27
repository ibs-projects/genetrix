
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace e_apurement.Models
{
    [Table("RoleCentralBanque")]
    public class RoleCentralBanque : IdentityUserRole
    { 
        public RoleCentralBanque(Session session)
        {

        }
        public void AfterConstruction()
        {

        }

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