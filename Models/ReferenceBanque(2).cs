using System;
using System.Linq;
using System.ComponentModel;
using DisplayNameAttribute = System.ComponentModel.DisplayNameAttribute;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using eApurement.Models.Fonctions;

namespace e_apurement.Models
{
    [DefaultProperty("NumeroRef")]
    [Table("ReferenceBanque")]
    public class ReferenceBanque 
    {
        public int Id { get; set; }

        [DisplayName("REFERENCE")]
        public string NumeroRef { get; set; }

        [Display(Name = "RECEPTION")]
        [DataType(DataType.Date)]
        public DateTime? DateReception { get; set; }

        [Display(Name = "Depot BEAC")]
        [DataType(DataType.Date)]
        public DateTime? DepotBEAC { get; set; }

        [Display(Name = "Délait T")]
        public int DélaitT { get; set; }

        public NOTIFICATION? NOTIFICATION { get; set; }

        [DisplayName("Date notification")]
        [DataType(DataType.Date)]
        public DateTime? DateNotif { get; set; }

        [Display(Name ="MT 99")]
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

        private double montant;
        [Display(Name ="MONTANT XAF")]
        public double Montant
        {
            get {
                try
                {
                    montant = 0;
                    Dossiers.ToList().ForEach(d=>
                    {
                        montant += d.Montant;
                    });
                }
                catch (Exception)
                {}
                return montant; 
            }
        }

        [Display(Name ="CV EURO")]
        public double CvEURO { get; set; }

        private string entrepriseclient;

        [DisplayName("DONNEUR D'ORDRE")]
        public string ClientEntre
        {
            get {
                if (Dossiers != null && Dossiers.Count > 0)
                {
                    entrepriseclient = Dossiers.FirstOrDefault().Client.Nom+" ("+ Dossiers.Count + " DOSSIERS)";
                }
                return entrepriseclient; 
            }
        }

        private DeviseMonetaire devise;
        public DeviseMonetaire Devise
        {
            get {
                if (Dossiers!=null && Dossiers.Count> 0)
                {
                    devise = Dossiers.FirstOrDefault().DeviseMonetaire;
                }
                return devise; 
            }
        }

        public ICollection<Dossier> Dossiers { get; set; } = new List<Dossier>();

        public int BanqueId { get; set; }

        public virtual Banque Banque { get; set; }

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
                    if (Dossiers == null || Dossiers.Count == 0)
                        return EtatDossier.Soumis;
                    if (Dossiers.FirstOrDefault(d => d.StatutDossier.StatutDossier.EtatDossier == EtatDossier.Echus) != null)
                        return EtatDossier.Echus;
                    if (Dossiers.FirstOrDefault(d => d.StatutDossier.StatutDossier.EtatDossier == EtatDossier.Encours) != null)
                        return EtatDossier.Encours;
                    if (Dossiers.Count(d => d.StatutDossier.StatutDossier.EtatDossier == EtatDossier.Apuré) != Dossiers.Count)
                        return EtatDossier.Apuré;
                    if (Dossiers.Count(d => d.StatutDossier.StatutDossier.EtatDossier == EtatDossier.Archivé) != Dossiers.Count)
                        return EtatDossier.Archivé;

                }
                catch (Exception)
                {}
                return EtatDossier.Soumis; 
            }
        }

        public bool Apuré { get; set; }
        public bool Accordé { get; set; }

        public int? EtapesDosier
        {
            get {
                try
                {
                    return Dossiers.FirstOrDefault().EtapesDosier;
                }
                catch (Exception)
                {}
                return null; 
            }
        }

        public bool Echus { get; set; }
        public bool EnvoieBEAC { get;  set; }

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

                    MailFunctions.SendMail(new eApurement.Models.MailModel()
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