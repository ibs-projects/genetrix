using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eApurement.Models
{
    public class Composant
    {
        public int Id { get; set; }
        public string Nom {
            get
            {
                return "numero" + Numero;
            }
        }
        public int Numero { get; set; }
        public bool EstActif { get; set; } = true;
        public virtual ICollection<IHM> Composants { get; set; }
        public virtual ICollection<IHMStructure> IHMStructures { get; set; }
        public string Description { get; set; }


        [Display(Name = "Niveau discontinu (separer deux niveau par un ;)")]
        public string NumDiscontinue { get; set; }

        private string discont;

        public List<int> DiscontinusListe
        {
            get
            {
                List<int> tab = new List<int>();
                try
                {
                    
                    for (int i = NumeroMin; i <=NumeroMax; i++)
                    {
                        tab.Add(i);
                    }
                    foreach (var item in NumDiscontinue.Split(';'))
                    {
                        try
                        {
                            tab.Add(Convert.ToInt32(item));
                        }
                        catch (Exception)
                        {}
                    }
                }
                catch (Exception)
                { }
                return tab;
            }
        }

        [Display(Name ="Appercu des niveaux")]
       public string DiscontinusString
        {
            get
            {
                string st = "";
                try
                {
                    
                    for (int i = NumeroMin; i <=NumeroMax; i++)
                    {
                        st += i + ";";
                    }
                    foreach (var item in NumDiscontinue.Split(';'))
                    {
                        try
                        {
                            st += item + ";";
                        }
                        catch (Exception)
                        {}
                    }
                }
                catch (Exception)
                { }
                return st;
            }
        }

        public Localistion Localistion { get; set; }
        public Type Type { get; set; }

        [ForeignKey("Groupe")]
        //[Required]
        public int? IdGroupe { get; set; }
        public GroupeComposant Groupe { get; set; }

        [ForeignKey("Action")]
        public int? IdAction { get; set; }
        public ActionIHM Action { get; set; }

        [ForeignKey("GroupeDisposionAccueil")]
        public int? IdGroupeDisposionAccueil { get; set; }
        public virtual GroupeDisposionAccueil GroupeDisposionAccueil { get; set; }
        public string Recherche { get; set; }
        [Display(Name ="Niveau maxi")]
        public int NumeroMax { get; set; }
        [Display(Name = "Niveau mini")]
        public int NumeroMin { get; set; }
        public string Info { get; internal set; }

        public string GetUrl()
        {
            return Action == null ? "#" : Action.Url;
        }
        
        public string GetIconPath()
        {
            return Action == null ? "#" : string.IsNullOrEmpty(Action.IconName) ? "db" : Action.IconName;// System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "images", "IU", Action.IconName);
        }

        internal static string GetDescrip(int i)
        {
            switch (i)
            {
                case 0:return "Dossiers reçus (accueil)";
                case 1:return "Dossiers en cours de traitement (accueil)";
                case 2:return "Dossiers en attente d'apurement (accueil)";
                case 3:return "Dossiers échus (accueil)";
                case 4:return "Dossiers à envoyer BEAC (accueil)";
                case 5:return "Dossiers envoyes BEAC (accueil)";
                case 6:return "Dossiers accordés  (accueil)";
                case 7:return "Dossiers en cours STF (accueil)";
                case 8:return "Liste des clients actifs (ayant des dossiers)";
                case 9:return "Liste des agences (accueil)";
                case 10:return "Liste de tous les agents de la banque (accueil)";
                case 11:return "Liste de tous les dossiers (accueil)";
                case 12:return "Liste de tous les dossiers par agence (accueil)";
                case 13:return "Saisie en cours (accueil)";
                case 40:return "menu principal (navigation)";
                case 41:return "tableau de bord (navigation)";
                case 42:return "messagerie (navigation)";
                case 43:return "mails (navigation)";
                case 44:return "Chat (navigation)";
                case 45:return "Courriers (navigation)";
                case 46:return "Nouveau transfert (navigation)";
                case 47:return "Gestion dossiers  (navigation)";
                case 48:return "Dossiers reçu (navigation)";
                case 49:return "Dossiers en cours (navigation)";
                case 50:return "Dossiers à envoyer (navigation)";
                case 51:return "Dossiers envoyés (navigation)";
                case 52:return "Dossiers accordés (navigation)";
                case 53:return "Dossiers en attente devise (navigation)";
                case 54:return "Dossiers  à apurer banque (navigation)";
                case 55:return "Dossiers apurés (navigation)";
                case 56:return "Dossiers échus (navigation)";
                case 57:return "Dossiers rappelés (navigation)";
                case 58:return "Dossiers archivés (navigation)";
                case 59:return "Gestion des tiers (navigation)";
                case 60:return "Agences (navigation)";
                case 61:return "Clients (navigation)";
                case 62:return "Edition des références (navigation)";
                case 63:return "Liste références (navigation)";
                case 64:return "Anciennes références (navigation)";
                case 65:return "Configurations référence (navigation)";
                case 66:return "Parametrage (navigation)";
                case 67:return "Parametre de la plateforme (navigation)";
                case 68:return "Conformité (navigation)";
                case 69:return "Service transfert  (navigation)";
                case 70:return "all structures  (navigation)";
                case 71:return "Dossiers traités et non traités  (navigation)";
                case 72:return "Mon portefeuille  (navigation)";
                case 73:return "Dossiers groupés  (navigation)";
                case 74:return "Attacher référence  (navigation)";
                case 75:return "Devises  (navigation)";
                case 76:return "Les états d'un dossier (navigation)";

                case 100:return "Parametre de la plateforme (navigation)";
                case 101:return "Parametre de la plateforme (navigation)";
                case 102:return "Invalider transfert vers conformité(Edition)";
                case 103:return "Valider transfert vers conformité (Edition)";
                case 104:return "Invalider transfert vers stf (Edition)";
                case 105:return "Valider transfert vers stf (Edition)";
                case 106:return "???";
                case 107:return "Envoyer vers une autre site (Edition)";
                case 108:return "Envoyer à la BEAC (Edition)";
                case 109:return "Apuré le dossier (Edition)";
                default:
                    return "";
            }
        }
    }

    public enum Localistion
    {
        accueil,
        navigationDroite,
        piedPage,
        entetePage,
        navigationGauche,
        dossier
    }

    public enum Type
    {
        lien,
        liste,
        graphe,
        image,
        bouton,
        lien_bouton
    }

    public class GroupeComposant
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public byte Priorité { get; set; }
        public virtual ICollection<Composant> GetComposants { get; set; }
    }

    public class GroupeDisposionAccueil
    {
        public int Id { get; set; }

        /// <summary>
        /// Classe parent: row 
        /// Classe fille, ex: col-lg-6
        /// </summary>
        public string ClasseFille { get; set; }
        public string BorderColor { get; set; }
        public string BgColor { get; set; }
        public string Card { get; set; }
        public string Titre { get; set; }
        public string BG_Header { get; set; }
        public string Info_pied { get; set; }
        public string BG_Footer { get; set; }
    }
}