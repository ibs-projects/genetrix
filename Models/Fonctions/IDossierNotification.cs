using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace genetrix.Models.Fonctions
{
    public interface IDossierNotification
    {
        Type GetType(Guid oid);

        IEnumerable GetMessages(List<Dossier> dossiers =null, Client client = null, IdentityUserRole role = null, ApplicationUser user = null, EtatDossier etat = 0, DateTime dateTime = default, string RefInterne = "",
                    DateTime DateCreaBanque = default, DateTime DateModif = default, Guid oidBanque = default, double Montant = 0, string Fournisseur = "",
                    Guid oidReferenceExterne = default, Guid oidClient = default);
    }
}
