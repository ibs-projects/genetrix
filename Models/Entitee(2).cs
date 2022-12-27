using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eApurement.Models
{
    public class Entitee
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public virtual ICollection<Entitee_Role> GetEntitee_Roles { get; set; }
        public byte Niveau { get; set; }
    }
}