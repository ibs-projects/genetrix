using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace genetrix.Models.Fonctions
{
    public interface IAccueilCommandes
    {
        IList<AbsNotification> DossiersNotifications { get;}
        void SetDossierNotification(Dossier dossier);
        IList<AbsNotification> ReferencesNotifications { get;}
        void SetReferenceNotification(ReferenceBanque reference);
        IList<AbsNotification> UsersNotifications { get;}
        void SetUserNotification(ApplicationUser utilisateur);



    }
}
