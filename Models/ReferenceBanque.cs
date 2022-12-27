using System;
using System.Linq;
using System.ComponentModel;
using DisplayNameAttribute = System.ComponentModel.DisplayNameAttribute;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using genetrix.Models.Fonctions;
using System.Globalization;

namespace genetrix.Models
{
    [DefaultProperty("NumeroRef")]
    [Table("ReferenceBanque")]
    public class ReferenceBanque :Reference
    {
        public ReferenceBanque():base()
        {

        }

        [Display(Name = "Depot BEAC")]
        [DataType(DataType.Date)]
        public DateTime? DepotBEAC { get; set; }

        [Display(Name = "Délait T")]
        public int DélaitT { get; set; }

        [DisplayName("Date notification")]
        [DataType(DataType.Date)]
        public DateTime? DateNotif { get; set; }

        [Display(Name ="MT 298")]
        public string MT99 { get; set; }

        [DisplayName("DATEMT 99")]
        [DataType(DataType.Date)]
        public DateTime? DateMT99 { get; set; }

        [DisplayName("DATE MT 202")]
        [DataType(DataType.Date)]
        public DateTime? DateMT202 { get; set; }

        [DisplayName("Date CREDIT CRPDT")]
        [DataType(DataType.Date)]
        public DateTime? DateCredit { get; set; }

        [DisplayName("traité")]
        [DataType(DataType.Date)]
        public DateTime? DateTraité{ get; set; }

        [Display(Name = "obs transmission")]
        public string ObsTransmission { get; set; }

        [Display(Name = "obs BEAC")]
        public string ObsBEAC { get; set; }

        public string NATURE { get; set; }

        [Display(Name ="CV EURO")]
        public double CvEURO { get; set; }

        private string entrepriseclient;

        [DisplayName("DONNEUR D'ORDRE")]
        public string ClientEntre
        {
            get {
                try
                {
                    if (Dossiers != null && Dossiers.Count > 0)
                    {
                        entrepriseclient = Dossiers.FirstOrDefault().Client.Nom + " (" + Dossiers.Count + " DOSSIERS)";
                    }
                }
                catch (Exception)
                { }
                return entrepriseclient; 
            }
        }

        Fournisseurs fournisseur;
        public Fournisseurs Fournisseur
        {
            get {
                try
                {
                    if (Dossiers != null && Dossiers.Count > 0)
                    {
                        fournisseur = Dossiers.FirstOrDefault().Fournisseur;
                    }
                }
                catch (Exception)
                {}
                return fournisseur; 
            }
        }
       
        internal Color GetCouleur(EtatDossier statutReference)
        {
            Color color = default;
            //Session.Query<StatutReference>().ToList().ForEach(s =>
            //{
            //    if((s as StatutReference).EtatDossier ==statutReference)
            //        color=(s as StatutReference).Couleur;
            //});
            return color;
        }


        internal byte[] GetImage(EtatDossier statutReference)
        {
            byte[] image = null;
            ////Session.Query<StatutReference>().ToList().ForEach(s =>
            ////{
            ////    if ((s as StatutReference).EtatDossier == statutReference)
            ////        image = (s as StatutReference).Image;
            ////});
            return image;
        }

        internal string GetTitre(EtatDossier statutReference)
        {
            string titre = null;
            //Session.Query<StatutReference>().ToList().ForEach(s =>
            //{
            //    if ((s as StatutReference).EtatDossier == statutReference)
            //        titre = (s as StatutReference).Titre;
            //});
            return titre;
        }

        internal string GetMessage(EtatDossier statutReference)
        {
            string message = null;
            //Session.Query<StatutReference>().ToList().ForEach(s =>
            //{
            //    if ((s as StatutReference).EtatDossier == statutReference)
            //        message = (s as StatutReference).Message;
            //});
            return message;
        }

        public virtual ICollection<ReferenceNotification> Notifications { get; set; }

        public EtatDossier GetStatutReference
        {
            get {
                try
                {
                    //if (Dossiers == null || Dossiers.Count == 0)
                    //    return EtatDossier.Soumis;
                    //if (Dossiers.FirstOrDefault(d => d.StatutDossier.StatutDossier.EtatDossier == EtatDossier.Echus) != null)
                    //    return EtatDossier.Echus;
                    //if (Dossiers.FirstOrDefault(d => d.StatutDossier.StatutDossier.EtatDossier == EtatDossier.Encours) != null)
                    //    return EtatDossier.Encours;
                    //if (Dossiers.Count(d => d.StatutDossier.StatutDossier.EtatDossier == EtatDossier.Apuré) != Dossiers.Count)
                    //    return EtatDossier.Apuré;
                    //if (Dossiers.Count(d => d.StatutDossier.StatutDossier.EtatDossier == EtatDossier.Archivé) != Dossiers.Count)
                    //    return EtatDossier.Archivé;

                }
                catch (Exception)
                {}
                return EtatDossier.Soumis; 
            }
        }

        public bool Apuré { get; set; }
        public bool Accordé { get; set; }

        public bool EnvoieBEAC { get;  set; }

        public string IdGestionnaire
        {
            get {
                try
                {
                    return Dossiers.FirstOrDefault().IdGestionnaire;
                }
                catch (Exception)
                {}
                return ""; 
            }
        }
        public string IdAgentResponsableDossier
        {
            get
            {
                try
                {
                    return Dossiers.FirstOrDefault().IdAgentResponsableDossier;
                }
                catch (Exception)
                { }
                return "";
            }
        }

        public string GetCurrenteResponsableID(int eta)
        {
            try
            {
                switch (eta)
                {
                    case 1:
                        return Dossiers.FirstOrDefault().IdResponsableAgence;
                    case 6:
                        return Dossiers.FirstOrDefault().IdResponsableConformite;
                    case 9:
                        return Dossiers.FirstOrDefault().IdResponsableTransfert;
                    default:
                        break;
                }
            }
            catch (Exception)
            { }
            return "";
        }

        public string IdPrecedentResponsable
        {
            get
            {
                try
                {
                    return Dossiers.FirstOrDefault().IdPrecedentResponsable;
                }
                catch (Exception)
                { }
                return "";
            }
        }

        public string GetFournisseur
        {
            get
            {
                try
                {
                    return Dossiers.FirstOrDefault().GetFournisseur;
                }
                catch (Exception)
                { }
                return "";
            }
        }
        public bool FiniFormalitéBEAC
        {
            get
            {
                try
                {
                    return Dossiers.FirstOrDefault().FiniFormalitéBEAC;
                }
                catch (Exception)
                { }
                return false;
            }
        }

        private string clientemail;
        public string GetClientEmail
        {
            get
            {
                try
                {
                    clientemail = Dossiers.FirstOrDefault().GetClientEmail;
                }
                catch (Exception)
                { }
                return clientemail;
            }
        }


        public string GetClient
        {
            get
            {
                try
                {
                    string nbr = Dossiers.Count == 1 ? "1 dossier" : Dossiers.Count + " dossiers";
                    return Dossiers.FirstOrDefault().GetClient+" ("+ nbr + ")";
                }
                catch (Exception)
                { }
                return "";
            }
        }

        public int NbrTelechargement { get; set; }
        public string Message { get; set; }
       
        public void SetStatutReference(int etat,ApplicationDbContext db)
        {
            try
            {
                if (Dossiers!=null && Dossiers.Count>0)
                {
                    Dossiers.ToList().ForEach(d =>
                    {
                        d.EtapesDosier = etat;
                    });

                    db.SaveChanges();
                    var dossier = Dossiers.FirstOrDefault();
                    var v1 = dossier.GetEtapDossier(etat)[2].Replace("dossier", "");
                    var v2 = dossier.DeviseMonetaire != null ? dossier.DeviseMonetaire.Nom : "null";

                    var body = "<h3 style=\"background-color:blue;color:white;padding:8px;text-align:center\">" + dossier.GetEtapDossier(etat)[2] + "</h3>"
                       + "<p> Le dossier de référence de n°" + dossier.Dossier_Id + " a été "
                       + v1 + "</p>"
                       + "<hr />"
                       + "<div>"
                       + "<h4>Details du dossiers</h"
                       + "<dl class=\"dl-horizontal\">"
                       + "    <dt>                  "
                       + "        Numero du référence "
                       + "    </dt>                 "
                       + "    <dd>                  "
                       + NumeroRef
                       + "    </dd>                 " 
                       + "    <dt>                  "
                       + "        Nombre de dossiers "
                       + "    </dt>                 "
                       + "    <dd>                  "
                       + Dossiers.Count
                       + "    </dd>                 "
                       + "    <dt>                  "
                       + "        Banque            "
                       + "    </dt>                 "
                       + "    <dd>                  "
                       + dossier.Site.DirectionMetier.Banque.Nom
                       + "    </dd>                 "
                        + "    <dt>                  "
                       + "        Agence            "
                       + "    </dt>                 "
                       + "    <dd>                  "
                       + dossier.Site.Nom
                       + "    </dd>                 "
                       + "    <dt>                  "
                       + "        Montant           "
                       + "    </dt>                 "
                       + "    <dd>                  "
                       + Montant
                       + "    </dd>                 "
                       + "    <dt>                  "
                       + "        Dévise            "
                       + "    </dt>                 "
                       + "    <dd>"
                       + v2
                       + "     </dd>                         "
                       + "    <dt>                           "
                       + "        Fournisseur                "
                       + "    </dt>                          "
                       + "    <dd>                           "
                       + dossier.Fournisseur.Nom
                       + "    </dd>                          "
                       + "</dl>"
                       + "</div>";

                    MailFunctions.SendMail(new genetrix.Models.MailModel()
                    {
                        To=Dossiers.FirstOrDefault().Client.Email,
                        Subject="Situation du dossier de référence "+NumeroRef,
                        Body=body
                    },db);
                }
            }
            catch (Exception)
            {}
        }

    }

    public enum NOTIFICATION{
        Accordé,
        Rejet
    }
}