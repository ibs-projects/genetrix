using eApurement.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace e_apurement.Models
{
    [Table("RoleCommercialeBanque")]
    public class RoleCommercialeBanque :XtraRole //IdentityUserRole
    { 
        public RoleCommercialeBanque()
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
                    if (SecuritySystem.CurrentUser is CompteBanqueCommerciale)
                        return true;
                }
                catch (Exception)
                { }
                return memeEntreprise;
            }
        }
    }
}