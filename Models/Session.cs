using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace genetrix.Models
{
    [Table("Session")]
    public class Session
    {
        public int Id { get; set; }
        public IEnumerable<T> Query<T>()
        {
            throw new NotImplementedException();
        }

        //internal CompteClient GetObjectByKey(Type type, Guid currentUserId)
        //{
        //    throw new NotImplementedException();
        //}
    }
}