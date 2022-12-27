using genetrix.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace genetrix.Models
{
    public class Statut
    {
        public int Id { get; set; }
        [DisplayName("Status 1")]
        public string Status1 { get; set; }
        [DisplayName("Message 2")]
        public string Message2 { get; set; }

        [DisplayName("Couleur")]
        public string CouleurSt { get; set; }
        public int? Etape { get; set; }

        [DisplayName("Message")]
        public string Message { get; set; }

        // [NotMapped]
        public Color Couleur { get; set; }

        [DisplayName("Couleur")]
        public Int32 Argb
        {
            get
            {
                return Couleur.ToArgb();
            }
            set
            {
                Couleur = Color.FromArgb(value);
            }
        }

        public virtual ICollection<StatutDossier> StatusDossiers { get; set; }

        public string[] GetParametre(Dictionary<string,string> parms)
        {
            string[] t = new string[5];
            string ch = "",mGes="", stat = Status1;//, tmp= !string.IsNullOrEmpty(Message)?Message:Statut1;
            try
            {
                ch = Message;
                mGes = Message2;
                stat = Status1;
                foreach (var p in parms)
                {
                    try
                    {
                        ch = ch.Replace(p.Key, p.Value);
                    }
                    catch (Exception)
                    { }
                    try
                    {
                        mGes = mGes.Replace(p.Key, p.Value);
                    }
                    catch (Exception)
                    { }
                    try
                    {
                        stat=stat.Replace(p.Key, p.Value);
                    }
                    catch (Exception)
                    { }
                }

                if (!string.IsNullOrEmpty(Message))
                {
                    
                    t[0] = ch.Split('&')[0];
                    t[1] = ch.Split('&')[1];
                }
                if (!string.IsNullOrEmpty(Message2))
                {
                    t[2] = mGes.Split('&')[0];
                    t[3] = mGes.Split('&')[1];
                }
                //else
                //{
                //    t[0] = ch;
                //}
                t[4] = stat;
            }
            catch (Exception)
            {}
            return t;
        }
    }
}