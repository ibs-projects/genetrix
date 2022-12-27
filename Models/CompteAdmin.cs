
using genetrix.Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace genetrix.Models
{
    [Table("CompteAdmin")]
    [Serializable()]
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
        public override bool SuitChat { get => base.SuitChat; set => base.SuitChat = value; }
    }
}