using genetrix.Models;
using genetrix.Models.Fonctions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Web;

namespace genetrix.Models
{
    public abstract class UneImage : IDelateCustom
    {
        public bool Remove(ApplicationDbContext db)
        {
            try
            {
                db.GetAllImages.Remove(this);
                System.IO.File.Delete(Url);
                return true;
            }
            catch (Exception)
            { }
            return false;
        }

        public int Id { get; set; }
        public string Url { get; set; } = "#";
        public DateTime DateCreation { get; set; } = DateTime.Now;
        public DateTime DerniereModif { get; set; } = DateTime.Now;
        public string Titre { get; set; }
        public string NomCreateur { get; set; }

        public bool EstPdf
        {
            get {
                try
                {
                    if (Url.Contains(".pdf"))
                        return true;
                }
                catch (Exception)
                {}
                return false;
            }
        }


        public string GetImage()
        {
            string imgDataURL = "";
            //string imgPath = Server.MapPath("~/Content/UserImages/Originals/" + file);
            try
            {
                byte[] byteData = System.IO.File.ReadAllBytes(Url);
                string imreBase64Data = Convert.ToBase64String(byteData);
                if (Url.Contains(".pdf"))
                {
                    imgDataURL = string.Format("data:application/pd;base64,{0}", imreBase64Data);
                }
                else
                {
                    imgDataURL = string.Format("data:image/jpg;base64,{0}", imreBase64Data);
                }
            }
            catch (Exception ee)
            {/* imgDataURL = Url;*/ }

            return !string.IsNullOrEmpty(imgDataURL)? imgDataURL:"#";
        }
    }
}