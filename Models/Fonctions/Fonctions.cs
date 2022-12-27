using DocumentFormat.OpenXml.Packaging;
using GroupDocs.Editor;
using GroupDocs.Editor.Formats;
using GroupDocs.Editor.HtmlCss.Resources;
using GroupDocs.Editor.Options;
using OpenXmlPowerTools;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;

namespace genetrix.Models.Fonctions
{
    public class Fonctions
    {
        public static bool DroitSurDossier(ApplicationDbContext db,Dossier d,Structure site, XtraRole role, int banqueID, string agentId)
        {
            var verifié = false;
            var name = site.GetType().Name;
            if (site is DirectionMetier)
            {
                if (d.Site.IdDirectionMetier == site.Id)
                    verifié = true;
            }
            else if (!(site is Agence))
                verifié = true;
            if (d.IdSite == site.Id || d.IdSite == banqueID || verifié) // site
            {
                if (d.Site.BanqueId(db) == banqueID)/*Banque permissions*/
                {
                    //if (!site.VoirDossiersAutres && site.IdResponsable != agentId)//permission structure
                    if (site.IdResponsable != agentId)
                    {
                        if (d.IdSite == site.Id || site.VoirDossiersAutres)//permission structure
                        {
                            if (role.VoirDossiersAutres || d.GestionnaireId == agentId || d.IdAgentResponsableDossier == agentId)//permission de role
                            {
                                return true;
                            }
                        }
                        else if (verifié)
                        {
                            if (d.IdAgentResponsableDossier == agentId)
                                return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void CreateNewClientFile(string path)
        {
            File.Create(path);
        }

        public static string DecodingJS(string _s)
        {
            try
            {
                var r1 = "_v1";
                var r5 = "_v5";
                var r6 = "_v6";

                _s = _s.Replace(r1, "&");
                _s = _s.Replace(r5, "<");
                _s = _s.Replace(r6, ">");
            }
            catch (Exception)
            { }
            return _s;
        }

        public static string ConvertMontantEnChaine(double? Montant=null,int apresVirgul=2)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            try
            {
                nfi = new NumberFormatInfo { NumberGroupSeparator = " ", NumberDecimalDigits = apresVirgul };
            }
            catch (Exception)
            { }
            return ((double)Montant).ToString("n", nfi);
        }

        public static async void Histiriser(ApplicationDbContext db,Historisation hist)
        {
            try
            {
                foreach (var dd in db.GetHistorisations.Where(t =>t.DateFin==null && t.TypeHistorique == hist.TypeHistorique && hist.Cible == hist.Cible))
                {
                    dd.DateFin = DateTime.Now;
                }
                //db.GetHistorisations.FirstOrDefault(t => t.TypeHistorique == hist.TypeHistorique && hist.Cible == hist.Cible).DateFin = DateTime.Now;
            }
            catch (Exception)
            {}            
            db.GetHistorisations.Add(hist);
            db.SaveChanges();
            //await db.SaveChangesAsync();
        }

        public static string PubBuilder(PubItem pubItem,Theme theme)
        {
            string tmp = "";
            try
            {
                if (!string.IsNullOrEmpty(theme.Html))
                {
                    tmp = theme.Html.Replace("[titre]", pubItem.Titre);
                    tmp = tmp.Replace("[titrecolor]", pubItem.TitreColor);
                    tmp = tmp.Replace("[titrecolor]", pubItem.TitreColor);
                    tmp = tmp.Replace("[desc]", pubItem.Description);
                    tmp = tmp.Replace("[desccolor]", pubItem.DescriptionColor);
                    tmp = tmp.Replace("[image]", pubItem.Image); 
                    tmp = tmp.Replace("col-lg-6", "col-lg-12"); 
                    tmp = tmp.Replace("col-lg-6", "col-lg-12"); 
                }
            }
            catch (Exception)
            {}
            return tmp;
        }

        public static string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        // Generate a random password of a given length (optional)  
        public static string RandomPassword(int size = 0)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(RandomString(4, true));
            builder.Append(RandomNumber(1000, 9999));
            builder.Append(RandomString(2, false));
            return builder.ToString();
        }

        // Generate a random number between two numbers    
        public static int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        public static string ImageBase64ImgSrc(string fileNameandPath)
        {
            string base64 = "";
            try
            {
                byte[] byteArray = File.ReadAllBytes(fileNameandPath);
                base64 = Convert.ToBase64String(byteArray);
                if (base64.Contains("video/"))
                    return string.Format("data:video/mp4;base64,{0}", base64);
            }
            catch (Exception)
            {}
            return string.Format("data:image/gif;base64,{0}", base64);
        }


        public static void EditMiseEnDemeure(string inputFilePath,string destination,Dictionary<string,string> texts)
        {
            try
            {
                using (FileStream fs = File.OpenRead(inputFilePath))
                {
                    GroupDocs.Editor.Options.WordProcessingLoadOptions loadOptions = new WordProcessingLoadOptions();
                    //loadOptions.Password = "password-if-any";
                    //Edit the Word document
                    using (Editor editor = new Editor(delegate { return fs; }, delegate { return loadOptions; }))
                    {
                        GroupDocs.Editor.Options.WordProcessingEditOptions editOptions = new WordProcessingEditOptions();
                        editOptions.FontExtraction = FontExtractionOptions.ExtractEmbeddedWithoutSystem;
                        editOptions.EnableLanguageInformation = true;
                        editOptions.EnablePagination = true;
                        using (EditableDocument beforeEdit = editor.Edit(editOptions))
                        {
                            string originalContent = beforeEdit.GetContent();
                            List<IHtmlResource> allResources = beforeEdit.AllResources;
                            string editedContent = "";
                            foreach (var item in texts.Keys)
                            {
                                editedContent += originalContent.Replace(item, texts[item]);
                            }
                            //Save the Edited Word document with Options
                            using (EditableDocument afterEdit = EditableDocument.FromMarkup(editedContent, allResources))
                            {
                                GroupDocs.Editor.Options.WordProcessingSaveOptions saveOptions = new WordProcessingSaveOptions(WordProcessingFormats.Docx);
                                saveOptions.EnablePagination = true;
                                saveOptions.Locale = System.Globalization.CultureInfo.GetCultureInfo("en-US");
                                saveOptions.OptimizeMemoryUsage = true;
                                //saveOptions.Password = "password";
                                //saveOptions.Protection = new WordProcessingProtection(WordProcessingProtectionType.ReadOnly, "write_password");
                                using (FileStream outputStream = File.Create(destination))
                                {
                                    editor.Save(afterEdit, outputStream, saveOptions);
                                }
                            }
                        }
                    } 

                }
            }
            catch (Exception e)
            { }
        }

        public static Byte[] PdfSharpConvert(String html)
        {
            Byte[] res = null;
            using (MemoryStream ms = new MemoryStream())
            {
                //var pdf = TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerator.GeneratePdf(html, PdfSharp.PageSize.A4);
                //pdf.Save(ms);
                //res = ms.ToArray();
            }
            return res;
        }

        public static string HtmlToPdf(InstructPdfViwModel model)
        {
            string mararriveeO = model.marchiseArrivee ? "checked='checked'" : "";
            string mararriveeN = !model.marchiseArrivee ? "checked='checked'" : "";
            var html = @"
                    <!DOCTYPE html>
<html>
<head>
    <meta http-equiv='Content-Type' content='text/html; charset=iso-8859-1' />
    <style>
        #entete{display:flex;flex-direction:row-reverse;}
        #entete>table {
            border: none;
        }
        td{width:50%}
        #titre{
            text-align:right
        }
        #corp{
            display:flex;
            flex-direction:column;
            border:2px solid #449cb0;
            padding:0px;
        }
        h2 {
            background-color: #b8edea;
            height: 30px;
            margin-top: 0px;
            margin-bottom: 0px;
            line-height: 30px;
            text-align: center;
            text-transform:uppercase;
        }
        .donnee {
            border-bottom: 2px dotted #b8edea;
            padding-bottom: 3px;
            text-align: left;
            padding-left:20px;
            font-weight:bold;
            -ms-flex: 1; /* IE 10 */
            flex: 1;
            font-size:16px;
            position:relative;margin-top:5px;
            margin-left: 10px;
        }
        .donnee1{
            font-size:16px;
            position:relative;
        }
        /*.p-elt>.elt{
            display: inline-block;
        }*/
        .p-elt{
            display:flex;
            flex-direction:row;
        }
        input[type='checkbox'] {
            width: 20px;
            height: 20px;
            box-sizing: border-box;
            border: 2px solid #b8edea;
            -webkit-transition: 0.5s;
            transition: 0.5s;
            outline: none;
        }
        input[type='text'] {
            height: 40px;
            text-align: center;
            -ms-flex: 1; /* IE 10 */
            flex: 1;
            margin-left:15px;
            margin-top:-5px;
            /*            box-shadow: 0 0 3px #719ECE;
*/
        }
        #num-data {
            display: flex;
            flex-direction: row;
        }
            #num-data div {
                border: 1px solid #449cb0;
                width: 25px;
                height: 15px;
                margin-right: 1px;
                line-height: 15px;
                border-top:none
            }
        table.a {
            table-layout: auto;
        }
        th, td {
            padding-right:10px;
        }
        hr{
            border-color:transparent;
        }
        .sign {
            height: 200px;
            -ms-flex: 1; /* IE 10 */
            flex: 1;
        }
        .sign2 {
            border-left: 1px solid #449cb0;
        }
        #logo-banque {
            text-align: center;
            height: 120px;
            width: 70%;
            background-color: #b6d2f5;
            line-height: 120px;
            color:white;
            font-weight:bold;
        }
        input {
            box-sizing: border-box;
            border: 2px solid #b8edea;
            -webkit-transition: 0.5s;
            transition: 0.5s;
            outline: none;
        }
        label,.elt {
            font-weight:bold;
        }
        #date {
            text-align: center;
            line-height: 150px;
            font-size:1.3em;
            font-weight:bold;
        }
        .cont01 {
            padding: 15px;
        }
    </style>
</head>
                ";

            var body = $@"
<body>

    <div id='content-inner-left'>
        <article class='actus-details'>
            <div id='lettre' style=''>
                <div id='entete'>
                    <table style='width:100%' border='0'>
                        <tr>
                            <td>
                                <div id='logo-banque'>
                                    <img src='{model.logo}' />
                                    Logo de la banque
                                </div>
                            </td>
                            <td id='titre'>
                                <h1 style='font-weight: bolder; font-size: 2.1em; color: #5aa39f'>ORDRE DE VIREMENT</h1>
                                <p style='color: #5aa39f; font-size: 1.2em; '></p>
                            </td>
                        </tr>
                    </table>
                </div>
                <div id='corp' style='border:1px solid #dcdcdc;font-family:Arial;font-size:12px'>
                    <h2>DONNEUR D'ORDRE</h2>
                    <div class='cont01'>
                        <div class='p-elt'>
                            <p class='elt' style=''>Nom et Prénom ou Raison sociale : </p>
                            <p class='donnee'>{model.client}</p>
                        </div>
                        <div>

                        </div>
                        <div class='p-elt'>
                            <table class='a' style='width: 100%; margin-left: -3px'>
                                <tr>
                                    <th style='text-align:left'>
                                        <p class='elt' style=''>Compte à débiter : </p>
                                        <p></p>
                                        <hr />
                                    </th>
                                    <th style='text-align:left;display:none;'>
                                        <input type='checkbox' id='deviseeuro' name='deviseeuro' value='en euro' />
                                        <label for='deviseeuro'> en euro</label>
                                        <p></p>
                                        <hr />
                                    </th>
                                    <th style='text-align:left;display:none;'>
                                        <input type='checkbox' id='autredevise' name='autredevise' value='en euro' />
                                        <label for='autredevise'> en autre devise</label>
                                        <p></p>
                                        <hr />
                                    </th>
                                    <th style='width:60px'>
                                        <hr />
                                        <div id='num-data' style='margin-bottom:0px;'>
                                            <div class='donnee1'>{model.etbc1}</div>
                                            <div class='donnee1'>{model.etbc2}</div>
                                            <div class='donnee1'>{model.etbc3}</div>
                                            <div class='donnee1'>{model.etbc4}</div>
                                            <div class='donnee1'>{model.etbc5}</div>
                                        </div>
                                        <p style='margin-top:3px;'>Code Etablissement </p>
                                    </th>
                                    <th style='width:50px'>
                                        <hr />
                                        <div id='num-data' style='margin-bottom:0px;'>
                                            <div class='donnee1'>{model.agc1}</div>
                                            <div class='donnee1'>{model.agc2}</div>
                                            <div class='donnee1'>{model.agc3}</div>
                                            <div class='donnee1'>{model.agc4}</div>
                                            <div class='donnee1'>{model.agc5}</div>
                                        </div>
                                        <p style='margin-top:3px;'>Code agence </p>
                                    </th>
                                    <th style='width:111px'>
                                        <hr />
                                        <div id='num-data' style='margin-bottom:0px;'>
                                            <div class='donnee1'>{model.ncc1}</div>
                                            <div class='donnee1'>{model.ncc2}</div>
                                            <div class='donnee1'>{model.ncc3}</div>
                                            <div class='donnee1'>{model.ncc4}</div>
                                            <div class='donnee1'>{model.ncc5}</div>
                                            <div class='donnee1'>{model.ncc6}</div>
                                            <div class='donnee1'>{model.ncc7}</div>
                                            <div class='donnee1'>{model.ncc8}</div>
                                            <div class='donnee1'>{model.ncc9}</div>
                                            <div class='donnee1'>{model.ncc10}</div>
                                            <div class='donnee1'>{model.ncc11}</div>
                                        </div>
                                        <p style='margin-top:3px;'>Numéro de compte</p>
                                    </th>
                                    <th style='width:25px'>
                                        <hr />
                                        <div id='num-data' style='margin-bottom:0px;'>
                                            <div class='donnee1'>{model.clc1}</div>
                                            <div class='donnee1'>{model.clc2}</div>
                                        </div>
                                        <p style='margin-top:3px;'>Clé</p>
                                    </th>
                                </tr>
                            </table>
                        </div>

                    </div>

                    <h2>Ordre de virement</h2>
                    <div class='cont01'>
                        {(model.BienOuService=="B"?$@"<table class='a' style='width: 100%;margin-left:-6px'>
                            <tr>
                                <th style='text-align:left'>
                                    <label for='deviseeuro'>
                                        Marchandise arrvée
                                    </label>
                                    <p></p>
                                    <hr />
                                </th>
                                <th style='text-align:left'>
                                    <input type='checkbox' id='autredevise' {mararriveeO} name='autredevise' value='en euro' />
                                    <label for='autredevise'>Oui</label>
                                    <p></p>
                                    <hr />
                                </th>
                                <th style='text-align:left'>
                                    <input type='checkbox' id='autredevise' {mararriveeN} name='autredevise' value='en euro' />
                                    <label for='autredevise'>Non</label>
                                    <p></p>
                                    <hr />
                                </th>
                                <th style='text-align:left;'>
                                    <p></p>
                                    <hr />
                                </th>
                                
                            </tr>
                        </table>":"")}
                        <table ble class='a' style='width: 100%; margin-left: -3px'>
                            <tr>
                                <th style='text-align:left;width:20%'>
                                    <div class='p-elt'>
                                        <p class='elt'>Devise : </p>
                                        <p class='donnee' type='text' id='devise' name='devise'>{model.devise}</p>
                                        <p></p>
                                        <hr />
                                    </div>
                                </th>
                                <th style='text-align:left;width:40%'>
                                    <div class='p-elt'>
                                        <p class='elt'> Montant en devise (en chiffres) : </p>
                                        <input type='text' class='donnee1' id='montantdevise' name='montantdevise' value='{model.montantdevise}' />
                                        <p></p>
                                        <hr />
                                    </div>
                                </th>
                                <th style='text-align: left; width: 40%'>
                                    <div class='p-elt'>
                                        <p class='elt'> Equivalent en CFA : </p>
                                        <input type='text' class='donnee1' id='montantdevise' name='montantdevise' value='{model.montantxaf}' />
                                        <p></p>
                                        <hr />
                                    </div>
                                </th>
                            </tr>
                        </table>
                        <div class='pan'>
                            <div class='p-elt'>
                                <p class='elt'>Montant en lettres : </p>
                                <p class='donnee' type='text' id='montantlettre'>{model.montantlettre}</p>
                                <p></p>
                                <hr />
                            </div>
                            <div class='p-elt'>
                                <p class='elt'>Motif de l'opération' : </p>
                                <p class='donnee' type='text' id='motif'>{model.motif}</p>
                                <p></p>
                                <hr />
                            </div>
                            <div class='p-elt' style='margin-top:-10px'>
                                <p style='color:white'>Motif de l'opération' : </p>
                                <p class='donnee' type='text' id='devise' name='devise' value='devise'></p>
                                <p></p>
                                <hr />
                            </div>
                        </div>
                        <table class='a' style='width: 100%; margin-left: -3px;display:none'>
                            <tr>
                                <th style='text-align: left; width: 160px '>
                                    <label for='deviseeuro'>
                                        Répartition des frais :
                                    </label>
                                    <p></p>
                                    <hr />
                                </th>
                                <th style='text-align:left;'>
                                    <input type='checkbox' id='autredevise' name='autredevise' value='en euro' />
                                    <label for='autredevise'>Frais partagés (par défaut)</label>
                                    <p></p>
                                    <hr />
                                </th>
                                <th style='text-align:left'>
                                    <input type='checkbox' id='autredevise' name='autredevise' value='en euro' />
                                    <label for='autredevise'>Frais à la charge du donneur d'ordre</label>
                                    <p></p>
                                    <hr />
                                </th>
                                <th style='text-align:left'>
                                    <input type='checkbox' id='autredevise' name='autredevise' value='en euro' />
                                    <label for='autredevise'>Frais à la charge du Bénéficiaire</label>
                                    <p></p>
                                    <hr />
                                </th>
                            </tr>
                        </table>
                    </div>
                    <h2>bénéficiaire</h2>
                    <div class='cont01'>
                       
                        <div class='p-elt'>
                            <p class='elt'>ou à defaut le numéro de compte complet : </p>
                            <p style='' class='donnee' type='text' id='devise' name='devise' value='devise'>{model.numcomf}</p>
                            <p></p>
                        </div>
                        <div class='p-elt'>
                            <p class='elt'>Nom et prénom ou Raison sociale : </p>
                            <p class='donnee' type='text' id='devise' name='devise' value='devise'>{model.fourn}</p>
                        </div>
                        <div class='p-elt'>
                            <p class='elt'>Adresse : </p>
                            <p class='donnee' type='text' id='devise' name='devise' value='devise'>{model.addresf}</p>
                            <p></p>
                        </div>
                        <div class='p-elt'>
                            <table style='width: 100%; margin-left: -2px'>
                                <tr>
                                    <th>
                                        <div class='p-elt' style='text-align:left'>
                                            <p class='elt'>Ville : </p>
                                            <p class='donnee' type='text' id='devise' name='devise' value='devise'>{model.villef}</p>
                                            <p></p>
                                        </div>
                                    </th>
                                    <th>
                                        <div class='p-elt' style='text-align:left'>
                                            <p class='elt'>Pays : </p>
                                            <p class='donnee' type='text' id='devise' name='devise' value='devise'>{model.paysf}</p>
                                            <p></p>
                                        </div>
                                    </th>
                                </tr>
                            </table>
                        </div>
                    </div>
                    <h2>Banque du bénéficiaire</h2>
                    <div class='cont01'>
                    <div class='p-elt'>
                            <table class='a' style='width: 100%; margin-left: -2px'>
                                <tr>
                                    <th style='text-align:left;width:25px;'>
                                        <hr />
                                        IBAN :
                                        <p></p>
                                    </th>
                                    <th style='width:95%'>
                                        <hr />
                                        <div id='num-data' style='margin-bottom:0px;'>
                                            <div class='donnee1'></div>
                                            <div class='donnee1'></div>
                                            <div class='donnee1'></div>
                                            <div class='donnee1'></div>
                                            <div class='donnee1'></div>
                                            <div class='donnee1'></div>
                                            <div class='donnee1'></div>
                                            <div class='donnee1'></div>
                                            <div class='donnee1'></div>
                                            <div class='donnee1'></div>
                                            <div class='donnee1'></div>
                                            <div class='donnee1'></div>
                                            <div class='donnee1'></div>
                                            <div class='donnee1'></div>
                                            <div class='donnee1'></div>
                                            <div class='donnee1'></div>
                                            <div class='donnee1'></div>
                                            <div class='donnee1'></div>
                                            <div class='donnee1'></div>
                                            <div class='donnee1'></div>
                                            <div class='donnee1'></div>
                                            <div class='donnee1'></div>
                                            <div class='donnee1'></div>
                                            <div class='donnee1'></div>
                                            <div class='donnee1'></div>
                                            <div class='donnee1'></div>
                                            <div class='donnee1'></div>
                                        </div>
                                    </th>
                                </tr>
                            </table>
                        </div>
                        <div class='p-elt'>
                                    <p class='elt'>Code Swift : </p>
                                    <p class='donnee' type='text' id='swiftBenf'>{model.swiftBenfToString}</p>
                                </div>
                        <div class='p-elt'>
                            <p class='elt' style='padding-top:5px;'>RIB : </p>
                            <table class='a' style='margin-left:15px'>
                                <tr>
                                    <th style='width:60px'>
                                        <hr />
                                        <div id='num-data' style='margin-bottom:0px;'>
                                            <div class='donnee1'>{model.etbf1}</div>
                                            <div class='donnee1'>{model.etbf2}</div>
                                            <div class='donnee1'>{model.etbf3}</div>
                                            <div class='donnee1'>{model.etbf4}</div>
                                            <div class='donnee1'>{model.etbf5}</div>
                                        </div>
                                        <p style='margin-top:3px;'>Code Etablissement </p>
                                    </th>
                                    <th style='width:60px'>
                                        <hr />
                                        <div id='num-data' style='margin-bottom:0px;'>
                                            <div class='donnee1'>{model.agf1}</div>
                                            <div class='donnee1'>{model.agf2}</div>
                                            <div class='donnee1'>{model.agf3}</div>
                                            <div class='donnee1'>{model.agf4}</div>
                                            <div class='donnee1'>{model.agf5}</div>
                                        </div>
                                        <p style='margin-top:3px;'>Code agence </p>
                                    </th>
                                    <th style='width:121px'>
                                        <hr />
                                        <div id='num-data' style='margin-bottom:0px;'>
                                            <div class='donnee1'>{model.ncf1}</div>
                                            <div class='donnee1'>{model.ncf2}</div>
                                            <div class='donnee1'>{model.ncf3}</div>
                                            <div class='donnee1'>{model.ncf4}</div>
                                            <div class='donnee1'>{model.ncf5}</div>
                                            <div class='donnee1'>{model.ncf6}</div>
                                            <div class='donnee1'>{model.ncf7}</div>
                                            <div class='donnee1'>{model.ncf8}</div>
                                            <div class='donnee1'>{model.ncf9}</div>
                                            <div class='donnee1'>{model.ncf10}</div>
                                            <div class='donnee1'>{model.ncf11}</div>
                                        </div>
                                        <p style='margin-top:3px;'>Numéro de compte</p>
                                    </th>
                                    <th style='width:30px'>
                                        <hr />
                                        <div id='num-data' style='margin-bottom:0px;'>
                                            <div class='donnee1'>{model.clf1}</div>
                                            <div class='donnee1'>{model.clf2}</div>
                                        </div>
                                        <p style='margin-top:3px;'>Clé</p>
                                    </th>
                                </tr>
                            </table>
                            
                        </div>

                    </div>
                    <div class='cont01' style='margin-top:-30px;'>
                        <div class='p-elt'>
                            <p class='elt'>Nom de la banque : </p>
                            <p class='donnee' type='text' id='devise' name='devise' value='devise'>{model.banquef}</p>
                        </div>
                        <div class='p-elt'>
                            <p class='elt'>Adresse de la banque : </p>
                            <p class='donnee' type='text' id='devise' name='devise' value='devise'>{model.addrbanquef}</p>
                            <p></p>
                        </div>
                        <div class='p-elt'>
                            <p class='elt'>Ville ou Agence : </p>
                            <p class='donnee' type='text' id='devise' name='devise' value='devise'>{model.viellebanquef}</p>
                            <p></p>
                        </div>
                    </div>

                    <div class='h2'>
                        <div class='p-elt'>
                            <h2 style='flex:1'>DATE</h2>
                            <h2 style='flex:1'>SIGNATURE DU CLIENT</h2>
                        </div>
                    </div>
                    <div class='cont01' style='margin-top:0px;padding:0px'>
                        <div class='p-elt'>
                            <div class='sign'><p id='date'>{model.date}</p></div>
                            <div class='sign sign2'></div>
                        </div>
                    </div>
                </div>
                <div class='footer' style='display:none'>
                    <ol>
                        <li>Une commission est additionnelle est prélévée au donneur d'ordre dès l'emmission, si la  case est cochée</li>
                        <li>Sans frais pour le donneur d'ordre, à l'exception des frais de charge, de service d'urgent ou en cas des commission additionnelle</li>
                    </ol>
                </div>
            </div>
        </article>
    </div>
</body>
</html>
                ";

            return html+body;
        }


        public static string HtmlToPdf(Dictionary<string, string> texts,string destinantion)
        {
            string gesName = texts["gesName"], agenceName = texts["agenceName"], agenceAdresse = texts["agenceAdresse"], 
                gesTel = texts["gesTel"], gesMail = texts["gesMail"], gesVille = texts["gesVille"], 
                clientName = texts["clientName"], clientAdresse = texts["clientAdresse"],  
                clientMail = texts["clientMail"], clientTel = texts["clientTel"], 
                clientPays = texts["clientPays"], mntTrans = texts["mntTrans"], fourniTrans = texts["fourniTrans"], 
                deviseTrans = texts["deviseTrans"], dateNow=DateTime.Now.ToString("dd/MM/yyyy");

            var html = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8' />
    <title></title>
    <style>
        span{
            margin-top:10px
        }
        table {
            width:100%
        }
    </style>
</head>";

            string body = $@"
<body style='padding:20px'>
    <table>
        <tr>
            <td style='text-align:left'>
                <span style='margin-bottom:5px'>{gesName}</span><br />
                <span style='margin-bottom:5px'>{agenceName}</span><br />
                <span style='margin-bottom:5px'>{agenceAdresse}</span><br />
                <span style='margin-bottom:5px'>{gesTel}</span><br />
                <span style='margin-bottom:5px'>{gesMail}</span><br />
            </td>
            <td style='text-align:right'>
                <span style='margin-bottom:5px'>{clientName}</span><br />
                <span style='margin-bottom:5px'>{clientTel}</span><br />
                <span style='margin-bottom:5px'>{clientMail}</span><br />
                <span style='margin-bottom:5px'>{clientAdresse}</span><br />
                <span style='margin-bottom:5px'>{clientPays}</span><br />
            </td>
        </tr>
        <tr>
            <td></td>
            <td style='text-align:right'>
                <br />
                <span>Fait à {gesVille}, le {dateNow}.</span><br />
            </td>
        </tr>
    </table>
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <p>Lettre recommandée avec accusé de réception</p>
    <p>Objet : Mise en demeure</p>
    <br />
    <p>(Madame, Monsieur),</p>
    <br />
    <br />
    <p>
        En dépit de plusieurs relances restées jusqu’à ce jour infructueuses, je constate qu’à la date de la présente, à savoir le {dateNow} votre transfert {mntTrans} {deviseTrans} du fournisseur {fourniTrans} va bientôt dépasser le délai d’apurement.
    </p>
    <br />
    <p>
        Cette situation peu confortable m’impose la rédaction de la présente afin de vous signifier votre mise en demeure de me régler ladite somme. Vous disposez pour cela d’un délai de dix jours à compter de la date de ce courrier, soit jusqu’au (date de la lettre + dix jours) pour procéder au paiement des sommes qui me sont dues. Dans le cas contraire, je m’autorise une action en justice afin de faire valoir mes droits.
    </p>
    <br />
    <p>
        Dans le but de vous éviter des poursuites judiciaires, je vous informe de mon consentement à bien vouloir étudier toute résolution de cette mise en demeure par un accord à l’amiable ou une négociation qui inclurait un dédommagement et des intérêts conséquents aux préjudices subis par cette situation. Cet effort de ma part constitue ma dernière proposition avant le recours aux tribunaux compétents. Je vous incite donc à agir en conséquence.
    </p>
    <br />
    <p>
        Dans l’attente de votre retour, je vous prie d’agréer, (Madame, Monsieur), l’expression de mes sentiments les meilleurs.
    </p>
    <br />
    <br />
    <br />
    <table>
        <tr><td>Nom Prénom</td>
        <td style='text-align:right'>Signature</td>
        </tr>
    </table>
</body>
</html>";


            //string html2 = "";
            //foreach (var item in texts.Keys)
            //{
            //    html2 += html.Replace(item, texts[item]);
            //}
            //var htmlLoadOptions = new HtmlLoadOptions();
            //using (var htmlStream = new MemoryStream(htmlLoadOptions.Encoding.GetBytes(html2)))
            //{
            //    // Load input HTML text as stream.
            //    var document = DocumentModel.Load(htmlStream, htmlLoadOptions);
            //    // Save output PDF file.
            //    document.Save(destinantion);
            //}
            return html+body;
        }


        public static string ParseDOCX(string fullName)
        {
            try
            {
                byte[] byteArray = File.ReadAllBytes(fullName);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    memoryStream.Write(byteArray, 0, byteArray.Length);
                    using (WordprocessingDocument wDoc =
                                              WordprocessingDocument.Open(memoryStream, true))
                    {
                        int imageCounter = 0;
                        var pageTitle = fullName;
                        var part = wDoc.CoreFilePropertiesPart;
                        if (part != null)
                            pageTitle = (string)part.GetXDocument()
                                                    .Descendants(DC.title)
                                                    .FirstOrDefault() ?? fullName;

                        WmlToHtmlConverterSettings settings = new WmlToHtmlConverterSettings()
                        {
                            AdditionalCss = "body { margin: 1cm auto; max-width: 20cm; padding: 0; }",
                            PageTitle = pageTitle,
                            FabricateCssClasses = true,
                            CssClassPrefix = "pt-",
                            RestrictToSupportedLanguages = false,
                            RestrictToSupportedNumberingFormats = false,
                            ImageHandler = imageInfo =>
                            {
                                ++imageCounter;
                                string extension = imageInfo.ContentType.Split('/')[1].ToLower();
                                ImageFormat imageFormat = null;
                                if (extension == "png") imageFormat = ImageFormat.Png;
                                else if (extension == "gif") imageFormat = ImageFormat.Gif;
                                else if (extension == "bmp") imageFormat = ImageFormat.Bmp;
                                else if (extension == "jpeg") imageFormat = ImageFormat.Jpeg;
                                else if (extension == "tiff")
                                {
                                    extension = "gif";
                                    imageFormat = ImageFormat.Gif;
                                }
                                else if (extension == "x-wmf")
                                {
                                    extension = "wmf";
                                    imageFormat = ImageFormat.Wmf;
                                }

                                if (imageFormat == null) return null;

                                string base64 = null;
                                try
                                {
                                    using (MemoryStream ms = new MemoryStream())
                                    {
                                        imageInfo.Bitmap.Save(ms, imageFormat);
                                        var ba = ms.ToArray();
                                        base64 = System.Convert.ToBase64String(ba);
                                    }
                                }
                                catch (System.Runtime.InteropServices.ExternalException)
                                { return null; }

                                ImageFormat format = imageInfo.Bitmap.RawFormat;
                                ImageCodecInfo codec = ImageCodecInfo.GetImageDecoders()
                                                          .First(c => c.FormatID == format.Guid);
                                string mimeType = codec.MimeType;

                                string imageSource =
                                       string.Format("data:{0};base64,{1}", mimeType, base64);

                                XElement img = new XElement(Xhtml.img,
                                      new XAttribute(NoNamespace.src, imageSource),
                                      imageInfo.ImgStyleAttribute,
                                      imageInfo.AltText != null ?
                                           new XAttribute(NoNamespace.alt, imageInfo.AltText) : null);
                                return img;
                            }
                        };

                        XElement htmlElement = WmlToHtmlConverter.ConvertToHtml(wDoc, settings);
                        var html = new XDocument(new XDocumentType("html", null, null, null),
                                                                                    htmlElement);
                        var htmlString = html.ToString(SaveOptions.DisableFormatting);
                        return htmlString;
                    }
                }
            }
            catch
            {
                return "File contains corrupt data";
            }
        }


    }
}