
using e_apurement.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace e_apurement.Models
{
    [Table("CompteAdmin")]
    public class CompteAdmin : ApplicationUser
    {
        public CompteAdmin(Session session)
            : base(session)
        {
        }

        public CompteAdmin()
        {

        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }


    }
}