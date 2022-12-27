using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace genetrix.Models
{
    public class ActionIHM
    {
        public int Id { get; set; }
        public string Intitule { get; set; }
        public string Url { get; set; }
        public byte[] Icon { get; set; }
        public virtual ICollection<Composant> GetComposants { get; set; }
        public string Recherche { get; internal set; }
        public string IconName { get; set; }
        public TypeObjet TypeObjet { get; set; }

        public string GetUrl(string id)
        {
            return Url + (!string.IsNullOrEmpty(id)?"/id=" + id:"");
        }
    }

    public enum TypeObjet
    {
        dossier,
        client,
        users,
        agence,
        directionMetier,
        structure,
        notification
    }
}