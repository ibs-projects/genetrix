using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace genetrix.Models.Fonctions
{ 
    public class FileItemModel {
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Mime Type")]
        public string MimeType { get; set; }

        [Display(Name = "Path")]
        public string Path { get; set; }

        [Display(Name = "Is This a Folder?")]
        public bool IsFolder { get; set; }

        [Display(Name = "Archivage Date")]
        public DateTime DateArchivage { get; set; }

        public int AnneeArchivage { get; set; }
        public int MoisArchivage { get; set; }
        public int JourArchivage { get; set; }

        [Display(Name = "Depot banque Date")]
        public DateTime? DateDepot { get; set; }
        public DateTime? Date_Etape4 { get; set; }
        public DateTime? Date_Etape6 { get; set; }
        public DateTime? Date_Etape9 { get; set; }
        public DateTime? Date_Etape15 { get; set; }
        public DateTime? Date_Etape22 { get; set; }
        public DateTime? Date_Etape24 { get; set; }
        public DateTime? ObtDevise { get; set; }

        public int AnneeDepot { get; set; }
        public int MoisDepot{ get; set; }
        public int MoisTraite{ get; set; }
        public int JourDepot { get; set; }
        public string MontantString { get; set; }
        public string GetCategorie { get; set; }
        public string GetReference { get; set; }
        public string GetAgenceName { get; set; }

        [Display(Name = "Modification Date")]
        public DateTime MDate { get; set; }

        public int FileId { get; set; }
        public DirectoryInfo[] SousRepertoires { get; internal set; }
        public FileInfo[] Fichiers { get; internal set; }
        public string ParentPath { get; internal set; }
        public long Taille { get; set; }

        public string GetTaille
        {
            get {
                try
                {
                    return FormatSize(Taille);
                }
                catch (Exception)
                {}
                return "";
            }
        }

        public string GetTailleToString
        {
            get {
                try
                {
                    if(!IsFolder)
                    return $"Taile: { GetTaille};";
                }
                catch (Exception)
                {}
                return "";
            }
        }

        public string ClientName { get;  set; }
        public string FournisseurName { get;  set; }
        public string Parent { get;  set; }
        public int ClientId { get;  set; }
        public int? FournisseurId { get;  set; }
        public int SiteId { get;  set; }
        public string Devise { get;  set; }
        public int IdDevise { get;  set; }
        public int? DfxId { get;  set; }
        public byte DFX6FP6BEAC { get;  set; }
        public double Montant { get;  set; }
        public string Annee_Mois { get;  set; }
        public string GestionnaireId { get; set; }
        public string SiteName { get; set; }
        public string GestionnaireName { get; set; }
        public string GetResponsables { get; set; }

        public override string ToString()
        {
            return $" Donneur d'ordre: {this.ClientName};Bénéficiaire: {this.FournisseurName}; {this.GetTailleToString} {DateDepotTostring} {DateTraiteTostring}";
        }

        public string DateDepotTostring
        {
            get { 
                if(DateDepot!=null)
                    return "Date depôt banque : "+ this.DateDepot.Value.ToString("dd/MM/yyyy")+";";
                return ""; 
            }
        }

        public string DateTraiteTostring
        {
            get { 
                if(Date_Etape22!=null)
                    return "Date traitement: "+ this.Date_Etape22.Value.ToString("dd/MM/yyyy");
                return ""; 
            }
        }


        #region Data size
        // Load all suffixes in an array  
        static readonly string[] suffixes =
        { 
            "Bytes", "KB", "MB", "GB", "TB", "PB" 
        };
        public static string FormatSize(Int64 bytes)
        {
            int counter = 0;
            decimal number = (decimal)bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number = number / 1024;
                counter++;
            }
            return string.Format("{0:n1}{1}", number, suffixes[counter]);
        }
    }
 
    #endregion
    public class CreateFileItemModel {
        [Display(Name = "Id")]
        public int? Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Path")]
        public string Path { get; set; }

        [Display(Name = "Is This a Folder?")]
        public bool IsFolder { get; set; }
    }

    public class UploadFileItemModel {
        [Required]
        [Display(Name = "Path")]
        public string Path { get; set; }

        [Required]
        [Display(Name = "File")]
        public HttpPostedFileBase PostedFile { get; set; }
    }

    public class EditFileItemModel {
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Path")]
        public string Path { get; set; }

        [Display(Name = "Content")]
        public string Content { get; set; }

        [Display(Name = "Is This a Folder?")]
        public bool IsFolder { get; set; }

        [Display(Name = "Creation Date")]
        public DateTime CDate { get; set; }

        [Display(Name = "Modification Date")]
        public DateTime MDate { get; set; }
    }

    public class OperationResult {
        public OperationStats Status { get; set; }
        public string Message { get; set; }
        public List<ModelErrorCollection> Errors { get; set; }
        public List<FileItemModel> Items { get; set; }
    }

    public enum OperationStats {
        Error = 0,
        Success = 1
    }

    public enum MimeType
    {
        folder,
        png,
        jpg,
        jpeg,
        gif,
        image,
        pdf
    }

    public static class StringResources {
        public static string SuccessfullyUploaded = "Successfully Uploaded.";
        public static string ItemSuccessfullyUploaded = "{0} Successfully Uploaded.";
        public static string UnknownErrorOccurred = "Unknown Error Occurred!";
        public static string SuccessfullyCreated = "Successfully Created.";
        public static string ItemAlreadyExists = "{0} Already Exists!";
        public static string NotFoundInDatabase = "File or Directory Not Found in Datebase!";
        public static string NotFoundInFileSystem = "File or Directory Not Found in File System!";
        public static string NameChanged = "Name Changed.";
        public static string SuccessfullyDeleted = "Successfully Deleted.";
    }
}