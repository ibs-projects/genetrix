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
    [Table("RoleClient")]
    public class RoleClient :XtraRole// IdentityUserRole
    {
        public RoleClient(Session session)
        {

        }

        public RoleClient()
        {

        }

        public void AfterConstruction()
        {

            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        private bool memeEntreprise;
        public bool EstDansEntreprise
        {
            get
            {
                try
                {
                    if (SecuritySystem.CurrentUser is CompteClient)
                        return true;
                }
                catch (Exception)
                { }
                return memeEntreprise;
            }
        }

    }
}