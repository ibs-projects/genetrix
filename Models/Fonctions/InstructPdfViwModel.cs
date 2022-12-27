using System;

namespace genetrix.Models.Fonctions
{
    public class InstructPdfViwModel
    {
        public string logo { get; set; }
        public string client { get; set; }
        public string etbc1 { get; set; }
        public string etbc2 { get; set; }
        public string etbc3 { get; set; }
        public string etbc4 { get; set; }
        public string etbc5 { get; set; }
        public string agc1 { get; set; }
        public string agc2 { get; set; }
        public string agc3 { get; set; }
        public string agc4 { get; set; }
        public string agc5 { get; set; }
        public string ncc1 { get; set; }
        public string ncc2 { get; set; }
        public string ncc3 { get; set; }
        public string ncc4 { get; set; }
        public string ncc5 { get; set; }
        public string ncc6 { get; set; }
        public string ncc7 { get; set; }
        public string ncc8 { get; set; }
        public string ncc9 { get; set; }
        public string ncc10 { get; set; }
        public string ncc11 { get; set; }
        public string ncc12 { get; set; }
        public string clc1 { get; set; }
        public string clc2 { get; set; }
        public string devise { get; set; }
        public string montantdevise { get; set; }
        public string montantxaf { get; set; }
        public string montantlettre { get; set; }
        public string motif { get; set; }
        public string ibanf1 { get; set; }
        public string ibanf2 { get; set; }
        public string ibanf3 { get; set; }
        public string ibanf4 { get; set; }
        public string ibanf5 { get; set; }
        public string ibanf6 { get; set; }
        public string ibanf7 { get; set; }
        public string ibanf8 { get; set; }
        public string ibanf9 { get; set; }
        public string ibanf10 { get; set; }
        public string ibanf11 { get; set; }
        public string ibanf12 { get; set; }
        public string ibanf13 { get; set; }
        public string ibanf14 { get; set; }
        public string ibanf15 { get; set; }
        public string ibanf16 { get; set; }
        public string ibanf17 { get; set; }
        public string ibanf18 { get; set; }
        public string ibanf19 { get; set; }
        public string ibanf20 { get; set; }
        public string ibanf21 { get; set; }
        public string ibanf22 { get; set; }
        public string ibanf23 { get; set; }
        public string ibanf24 { get; set; }
        public string ibanf25 { get; set; }
        public string ibanf26 { get; set; }
        public string ibanf27 { get; set; }
        public string numcomf { get; set; }
        public string fourn { get; set; }
        public string addresf { get; set; }
        public string villef { get; set; }
        public string paysf { get; set; }
        public string etbf1 { get; set; }
        public string etbf2 { get; set; }
        public string etbf3 { get; set; }
        public string etbf4 { get; set; }
        public string etbf5 { get; set; }
        public string agf1 { get; set; }
        public string agf2 { get; set; }
        public string agf3 { get; set; }
        public string agf4 { get; set; }
        public string agf5 { get; set; }
        public string ncf1 { get; set; }
        public string ncf2 { get; set; }
        public string ncf3 { get; set; }
        public string ncf4 { get; set; }
        public string ncf5 { get; set; }
        public string ncf6 { get; set; }
        public string ncf7 { get; set; }
        public string ncf8 { get; set; }
        public string ncf9 { get; set; }
        public string ncf10 { get; set; }
        public string ncf11 { get; set; }
        public string ncf12 { get; set; }
        public string clf1 { get; set; }
        public string clf2 { get; set; }
        public string banquef { get; set; }
        public string addrbanquef { get; set; }
        public string viellebanquef { get; set; }
        public string date { get; set; }
        public string swiftBenf { get; internal set; }

        public string swiftBenfToString
        {
            get {
                try
                {
                    string st = "";
                    foreach (var item in swiftBenf)
                    {
                        st += item + " ";
                    }
                    return st;
                }
                catch (Exception)
                {}
                return ""; 
            }
        }

        public bool marchiseArrivee { get; internal set; }
        public string BienOuService { get; set; }

        public void InitData(Dossier dossier)
        {
            var dateNow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "W. Central Africa Standard Time");
            swiftBenf = dossier.CodeSwiftBic;
            BienOuService = dossier is Bien ? "B" : "S";
            marchiseArrivee = dossier.MarchandiseArrivee;
            addrbanquef = dossier.AdresseBanqueBenf;
            addresf = dossier.AdresseFournisseur;
            agc1 = dossier.GetCodeAgence[0] + "";
            agc2 = dossier.GetCodeAgence[1] + "";
            agc3 = dossier.GetCodeAgence[2] + "";
            agc4 = dossier.GetCodeAgence[3] + "";
            agc5 = dossier.GetCodeAgence[4] + "";

            agf1 = dossier.GetCodeAgenceBenf[0] + "";
            agf2 = dossier.GetCodeAgenceBenf[1] + "";
            agf3 = dossier.GetCodeAgenceBenf[2] + "";
            agf4 = dossier.GetCodeAgenceBenf[3] + "";
            agf5 = dossier.GetCodeAgenceBenf[4] + "";

            banquef = dossier.NomBanqueBenf;
            clc1 = dossier.GetCle[0] + "";
            clc2 = dossier.GetCle[1] + "";
            clf1 = dossier.GetCleBenf[0] + "";
            clf2 = dossier.GetCleBenf[1] + "";

            client = dossier.GetClient;
            date = dateNow.ToString("dd/MM/yyyy");
            devise = dossier.DeviseToString;

            etbc1 = dossier.GetCodeEtablissement[0] + "";
            etbc2 = dossier.GetCodeEtablissement[1] + "";
            etbc3 = dossier.GetCodeEtablissement[2] + "";
            etbc4 = dossier.GetCodeEtablissement[3] + "";
            etbc5 = dossier.GetCodeEtablissement[4] + "";

            etbf1 = dossier.GetCodeEtbBenf[0] + "";
            etbf2 = dossier.GetCodeEtbBenf[1] + "";
            etbf3 = dossier.GetCodeEtbBenf[2] + "";
            etbf4 = dossier.GetCodeEtbBenf[3] + "";
            etbf5 = dossier.GetCodeEtbBenf[4] + "";
            fourn = dossier.GetFournisseur;

            ibanf1 = dossier.GetIBANBenf[0] + "";
            ibanf2 = dossier.GetIBANBenf[1] + "";
            ibanf3 = dossier.GetIBANBenf[2] + "";
            ibanf4 = dossier.GetIBANBenf[3] + "";
            ibanf5 = dossier.GetIBANBenf[4] + "";
            ibanf6 = dossier.GetIBANBenf[5] + "";
            ibanf7 = dossier.GetIBANBenf[6] + "";
            ibanf8 = dossier.GetIBANBenf[7] + "";
            ibanf9 = dossier.GetIBANBenf[8] + "";
            ibanf10 = dossier.GetIBANBenf[9] + "";
            ibanf11 = dossier.GetIBANBenf[10] + "";
            ibanf12 = dossier.GetIBANBenf[11] + "";
            ibanf13 = dossier.GetIBANBenf[12] + "";
            ibanf14 = dossier.GetIBANBenf[13] + "";
            ibanf15 = dossier.GetIBANBenf[14] + "";
            ibanf16 = dossier.GetIBANBenf[15] + "";
            ibanf17 = dossier.GetIBANBenf[16] + "";
            ibanf18 = dossier.GetIBANBenf[17] + "";
            ibanf19 = dossier.GetIBANBenf[18] + "";
            ibanf20 = dossier.GetIBANBenf[19] + "";
            ibanf21 = dossier.GetIBANBenf[20] + "";
            ibanf22 = dossier.GetIBANBenf[21] + "";
            ibanf23 = dossier.GetIBANBenf[22] + "";
            ibanf24 = dossier.GetIBANBenf[23] + "";
            ibanf25 = dossier.GetIBANBenf[24] + "";
            ibanf26 = dossier.GetIBANBenf[25] + "";
            ibanf27 = dossier.GetIBANBenf[26] + "";

            montantdevise = dossier.MontantString;
            montantlettre = dossier.MontantEnLettre;
            montantxaf = dossier.MontantCVstring;
            motif = dossier.Motif;
            ncc1 = dossier.GetNumCompteClient[0] + "";
            ncc2 = dossier.GetNumCompteClient[1] + "";
            ncc3 = dossier.GetNumCompteClient[2] + "";
            ncc4 = dossier.GetNumCompteClient[3] + "";
            ncc5 = dossier.GetNumCompteClient[4] + "";
            ncc6 = dossier.GetNumCompteClient[5] + "";
            ncc7 = dossier.GetNumCompteClient[6] + "";
            ncc8 = dossier.GetNumCompteClient[7] + "";
            ncc9 = dossier.GetNumCompteClient[8] + "";
            ncc10 = dossier.GetNumCompteClient[9] + "";
            ncc11 = dossier.GetNumCompteClient[10] + "";

            ncf1 = dossier.GetNumCompteBenf[0] + "";
            ncf2 = dossier.GetNumCompteBenf[1] + "";
            ncf3 = dossier.GetNumCompteBenf[2] + "";
            ncf4 = dossier.GetNumCompteBenf[3] + "";
            ncf5 = dossier.GetNumCompteBenf[4] + "";
            ncf6 = dossier.GetNumCompteBenf[5] + "";
            ncf7 = dossier.GetNumCompteBenf[6] + "";
            ncf8 = dossier.GetNumCompteBenf[7] + "";
            ncf9 = dossier.GetNumCompteBenf[8] + "";
            ncf10 = dossier.GetNumCompteBenf[9] + "";
            ncf11 = dossier.GetNumCompteBenf[10] + "";
            paysf = dossier.PaysBanqueBenf;
            viellebanquef = dossier.VilleBanqueBenf;
            villef = dossier.GetVilleBenf;
        }
    }
}