using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace genetrix.Models.Fonctions
{
    public enum DosType
    {
        Null,
        DFX,
        FP,
        REF
    }

    public enum GroupeWFDossier
    {
        Null,
        Transfert,
        AApurer,
        Echus,
        Apurer,
        Archive,
        Archive2
    }

    public enum JourSemaine
    {
        Lundi,
        Mardi,
        Mercredi,
        Jeudi,
        Vendre,
        Samedi,
        Dimanche
    }

    public enum Sexe
    {
        [Display(Name = "Non défini")]
        Null,
        [Display(Name = "Madame")]
        Mme,
        [Display(Name = "Monsieur")]
        Mr
    }

}