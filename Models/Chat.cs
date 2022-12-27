using genetrix;
using genetrix.Models.Fonctions;
using genetrix.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace genetrix.Models
{
    [DefaultProperty("Message")]
    [Table("Chat")]
    public class Chat :IDelateCustom
    {
        public bool Remove(ApplicationDbContext db)
        {
            try
            {
                db.Chats.Remove(this);
                return true;
            }
            catch (Exception)
            { }
            return false;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ChatId { get; set; }

        [Display(Name ="Numéro du topic")]
        public string NumeroTopic { get; set; }

        public string Sujet { get; set; }

        private DateTime? dateTime;

        public DateTime? DateHeure
        {
            get { return dateTime; }
            set { dateTime = value; }
        }

        public DateTime? DateFermeture { get; set; }

        public Guid Token { get; set; }

        public StatutChat Statut { get; set; }
        public Situation Situation { get; set; }
        public int? EntrepriseId { get; set; }

        [Display(Name ="Durée de traitement")]
        public string DelaiTraitement
        {
            get {
                try
                {
                   string delai = "";
                   var j= (DateFermeture - DateHeure).Value.TotalDays;
                    if (j / 365 >= 1)
                    {
                        j = j / 365;
                        delai = (int)j + " année(s)";
                        j = (j - (int)j)*365;
                    }
                    if (j / 30 >= 1)
                    {
                        j = j / 30;
                        delai += (int)j + " mois";
                        j = (j - (int)j) * 30;
                    }
                    if (j > 1)
                        delai += j + " jours";
                    else
                    {
                        j = (DateFermeture - DateHeure).Value.TotalHours;
                        if (j<1 && string.IsNullOrEmpty(delai))
                        {
                            j = (DateFermeture - DateHeure).Value.TotalMinutes;
                            if(j>1)
                                delai += (int)j + " minutes";
                            else
                            {
                                j = (DateFermeture - DateHeure).Value.Seconds;
                                delai += (int)j + " secondes";
                            }
                        }
                    }
                   return delai;
                }
                catch (Exception)
                {}
                return ""; 
            }
        }


        [ForeignKey("Emetteur")]
        public string EmetteurId { get; set; }
        public virtual ApplicationUser Emetteur { get; set; }
        public virtual ICollection<ImageChat> ImageChats { get; set; }
        /// <summary>
        /// Permet d'identifier entre le client et l'agent du service technique qui a écrit le dernier: 1: client et 2: agent
        /// </summary>
        public byte DernierEcrit { get; set; }

        public string[] GetStatutColor
        {
            get {
                try
                {
                    switch (this.Statut)
                    {
                        case StatutChat.Attente:
                            return new string[] {"En attente","warning" };
                        case StatutChat.Encours:
                            return new string[] { "En cours", "info" };
                        case StatutChat.Fermé:
                            return new string[] { "Fermé", "danger" };
                        default:
                            break;
                    }
                }
                catch (Exception)
                {}
                return new string[2]; 
            }
        }

        public string[] GetSituationColor
        {
            get {
                try
                {
                    switch (this.Situation)
                    {
                        case Situation.Resolu:
                            return new string[] { "Resolu", "success" };
                        case Situation.NonResolu:
                            return new string[] { "Non resolu", "warning" };
                        case Situation.AucuneReponse:
                            return new string[] { "Aucune reponse", "dark" };
                        default:
                            break;
                    }
                }
                catch (Exception)
                {}
                return new string[2]; 
            }
        }

        public string GetEmetteur
        {
            get {
                try
                {
                    if (Emetteur != null)
                        return Emetteur.NomComplet;
                }
                catch (Exception)
                { }
                return ""; 
            }
        }

        [Display(Name ="Emetteur")]
        public string GetEmetteurFirme
        {
            get {
                try
                {
                    if (Emetteur != null)
                    {
                        if (Emetteur is CompteClient)
                            return (Emetteur as CompteClient).ClientName;
                        else if(Emetteur is CompteBanqueCommerciale)
                            return (Emetteur as CompteBanqueCommerciale).StructureName;
                    }
                }
                catch (Exception)
                { }
                return ""; 
            }
        }


        public virtual ICollection<ApplicationUser> Destinataires { get; set; }       
        public virtual ICollection<MessageItem> Contenu { get; set; }       
    }

    public enum StatutChat
    {
        Attente,
        [Display(Name ="En cours")]
        Encours,
        Fermé
    }

    public enum Situation
    {
        Resolu, 
        [Display(Name = "Non resolu")]
        NonResolu,
        [Display(Name = "Aucune reponse")]
        AucuneReponse,
    }

    public class MessageItem
    {
        [Key]
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public string EmetteurName{ get; set; }
        public bool Lu { get; set; }
        public string LienImage { get; set; } = string.Empty;
        [ForeignKey("Chat")]
        public int ChatId { get; set; }
        public virtual Chat Chat{ get; set; }
    }
}