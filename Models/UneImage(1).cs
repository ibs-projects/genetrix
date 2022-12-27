using e_apurement.Models;
using eApurement.Models.Fonctions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace eApurement.Models
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

        public string GetImage()
        {
            string imgDataURL = "";
            //string imgPath = Server.MapPath("~/Content/UserImages/Originals/" + file);
            try
            {
                byte[] byteData = System.IO.File.ReadAllBytes(Url);
                string imreBase64Data = Convert.ToBase64String(byteData);
                imgDataURL = string.Format("data:image/jpg;base64,{0}", imreBase64Data);
                //imgDataURL = string.Format("data:application/pd;base64,{0}", imreBase64Data);
            }
            catch (Exception ee)
            { imgDataURL = Url; }

            return !string.IsNullOrEmpty(imgDataURL)? imgDataURL:"#";
        }
    }
}