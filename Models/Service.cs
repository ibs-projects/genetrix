using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace genetrix.Models
{
    public class Service:Dossier
    {
        public Service()
        {
            MarchandiseArrivee = true;
        }

        #region Info domiciliation
        public override NatureOperation NatureOperation { get => NatureOperation.Service; }

        public override bool MarchandiseArrivee { get=>true; }

        public override bool EtapeNumerisation
        {
            get
            {
                try
                {
                    if (base.EtapeNumerisation && !string.IsNullOrEmpty(DescriptionService))
                        return true;
                }
                catch (Exception)
                { }
                return false;
            }
        }

        [Display(Name = "Description du service")]
        public string DescriptionService { get; set; } 
        public string Chapitre { get; set; }

        public override int EditeAuto
        {
            get
            {
                int aut = 6;
                if (Get_DomicilImport == "#")
                    aut = 5;
                if (Get_DeclarImport == "#")
                    aut = 4;
                if (TotalPageFactures == 0)
                    aut = 1;
                if (GetImageInstruction() == "#")
                    return 0;
                return aut;
            }
        }
        #endregion

        #region Numerisation
        public override string PrintDomiciliation()
        {
            return $@"
<!DOCTYPE html>

<html lang='en' xmlns='http://www.w3.org/1999/xhtml'>
<head>
    <meta charset='utf-8' />
    <title></title>
    {base.PrintDomiciliation()}
</head>
<body style='font-family:""Times New Roman"", Times, serif; font-size:1.1em; font-weight:bold;margin-left:8mm;margin-right:8mm;'>
    <div class='table-responsive'>
        <h3 style='text-align:right;text-transform:uppercase;margin-bottom:0;padding-right:20px'> Republic <label style='min-width:100px;text-decoration:dotted,underline'><span style='padding:3px 20px 1px 20px;border-bottom:1px dotted black'>&emsp;{PaysClient}&emsp;</span></label></h3>
        <h2 style='padding:10px;margin-bottom:3px;margin-top:3px;text-align:center;text-transform:uppercase;border:4px solid black'> Domiciliation d'importation (services)</h2>
        <table id='table-domici' border='1'>
            <tr>
                <td colspan='2' class='item'>
                    Importatteur
                </td>
            </tr>
            <tr>
                <td>Nom ou raison sociale:</td>
                <td class='td-data'>
                    <span>
                        {this.ImportaNom}
                    </span>
                </td>
            </tr>
            <tr>
                <td>Numéro d'inscription au registre de commerce:</td>
                <td class='td-data'>
                    <span>{this.ImportaNumInscri}</span>
                </td>
            </tr>
            <tr>
                <td>Adresse complète:</td>
                <td class='td-data'>
                    <span>{this.ImportaAdresse}</span>
                </td>
            </tr>
            <tr>
                <td>Profession</td>
                <td class='td-data'>
                    <span>{this.ImportaProfession}</span>
                </td>
            </tr>
            <tr>
                <td>Immatriculation statique: </td>
                <td class='td-data'>
                    <span>{this.ImportaImmatri}</span>
                </td>
            </tr>
            <tr>
                <td colspan='2' class='item'>
                    Fournisseur
                </td>
            </tr>
            <tr>
                <td>Nom ou raison sociale: </td>
                <td class='td-data'>
                    <span>{this.GetFournisseur}</span>
                </td>
            </tr>
            <tr>
                <td>Pays d'origine:</td>
                <td class='td-data'>
                    <span>{this.PaysFournisseur}</span>
                </td>
            </tr>
            <tr>
                <td>Adresse dans le pays d'origine:</td>
                <td class='td-data'>
                    <span>{this.PaysFournisseur}</span>
                </td>
            </tr>
            <tr>
                <td colspan='2' class='item'>
                    Services
                </td>
            </tr>
            <tr>
                <td>Designation commerciale:</td>
                <td class='td-data'>
                    <span>{this.Description}</span>
                </td>
            </tr>
            <tr>
                <td>Description du service :</td>
                <td class='td-data'>
                    <span>{this.DescriptionService}</span>
                </td>
            </tr>
            <tr>
                <td>Chapitre:</td>
                <td class='td-data'>
                    <span>{this.Chapitre}</span>
                </td>
            </tr>

            <tr>
                <td>FOB:</td>
                <td class='td-data'>
                    <span>{this.FOBDevise}</span>
                </td>
            </tr>

            <tr>
                <td>Nomenclature douanière:</td>
                <td class='td-data'>
                    <span>{this.NomenClatureDouane}</span>
                </td>
            </tr>

            <tr>
                <td>Reference de la facture:</td>
                <td class='td-data'>
                    <span>{this.NumFacturePro}</span>
                </td>
            </tr>

            <tr>
                <td>Montant en devise:</td>
                <td class='td-data'>
                    <span>{this.Montant}</span>
                </td>
            </tr>

            <tr>
                <td>Montant en CFA:</td>
                <td class='td-data'>
                    <span>{this.MontantCV}</span>
                </td>
            </tr>

            <tr>
                <td>Echéance fixée pour le paiement:</td>
                <td class='td-data'>
                    <span>{this.EcheancePaiement}</span>
                </td>
            </tr>

            <tr>
                <td colspan='2'>
                    <p> Je soussigné, certifie sincères et véritables, les énonciations sur la présente formule.</p>
                    <div class='div-je-soussi'>
                        <div style='flex:1'> A </div>
                        <div style='flex:1'>le</div>
                        <div style='flex:1.5;padding-top:50px'> Signature et cachet de la banque</div>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan='2'>
                    <p> Banque domiciliataire :</p>
                    <p>Nom & adresse:</p>
                    <p>Numéro du dossier de domiciliation:</p>
                    <div class='div-je-soussi'>
                        <div style='flex:1'> A </div>
                        <div style='flex:1'>le</div>
                        <div style='flex:1.5;padding-top:50px'> Signature et cachet de la banque</div>
                    </div>
                </td>
            </tr>

        </table>
        <p style='margin-top:0'>Annexe à l'instruction N°007/GR/2019 du 10 juin 2019</p>
    </div>

</body>
</html>
";   
        }
        public override string PrintDeclaration()
        {
            return $@"
                
<!DOCTYPE html>

<html lang='en' xmlns='http://www.w3.org/1999/xhtml'>
<head>
    <meta charset='utf-8' />
    <title></title>
    {base.PrintDeclaration()}
</head>
<body>
    <div class='table-responsive' style='margin-left:8mm;margin-right:8mm;'>
        <h4 style='text-align:right;text-transform:uppercase;margin-bottom:0;padding-right:20px'>Republic <span style='padding:3px 20px 1px 20px;border-bottom:1px dotted black'>&emsp;{PaysClient}&emsp;</span></h4>
        <h2 style='padding:10px;margin-bottom:3px;margin-top:3px;text-align:center;text-transform:uppercase;border:3px solid black'>Déclaration d'importation de services</h2>
        <h5 style='text-align:right;margin-top:7px;margin-bottom:0px;'>
            <span>D I N° : <span style='padding:3px 20px 1px 20px;border-bottom:1px dotted black'>&emsp;{NumDeclaration}&emsp;</span></span><br />
            <span>DU :  <span style='padding:3px 20px 1px 20px;border-bottom:1px dotted black'>&emsp;{(DateDeclaration!=null?DateDeclaration.Value.ToString("dd/MM/yyyy"):"")}&emsp;</span></span><br />
            <span>Date imp. : <span style='padding:3px 20px 1px 20px;border-bottom:1px dotted black'>&emsp;{(DateImport != null ? DateImport.Value.ToString("dd/MM/yyyy") : "")}&emsp;</span></span>
        </h5>
        <p style='text-align:left;margin-top:0px;margin-bottom:0px;'>Soumise au contrôle douanier</p>
        <table border='1'>
            <tr>
                <td colspan='2'>
                    <div class='form-group mb-3'>
                        <div class='col-12'>
                            <label class='control-label mt-2'>Importateur (nom/adresse)</label>
                            <div class='input-group'>
                                <input type='text' class='form-control input-sel' name='ImportaNom' id='ImportaNom' value='{this.ImportaNomAdress}' />
                            </div>
                        </div>
                        <div class='row'>
                            <div class='col-6'>
                                <label class='control-label mt-2'>Ville</label>
                                <div class='input-group'>
                                    <input type='text' class='form-control input-sel' name='ImportaVille' id='ImportaVille' value='{this.ImportaVille}' />
                                </div>
                            </div>
                            <div class='col-6'>
                                <label class='control-label mt-2'>Pays</label>
                                <div class='input-group'>
                                    <input type='text' class='form-control input-sel' name='ImportaPays' id='ImportaPays' value='{this.ImportaPays}' />
                                </div>
                            </div>
                        </div>
                        <div class='row'>
                            <div class='col-6'>
                                <label class='control-label mt-2'>Code d'agrément</label>
                                <div class='input-group'>
                                    <input type='text' class='form-control input-sel' name='ImportateurCodeAgr' id='ImportateurCodeAgr' value='{this.ImportateurCodeAgr}' />
                                </div>
                            </div>
                            <div class='col-6'>
                                <label class='control-label mt-2'>Téléphone</label>
                                <div class='input-group'>
                                    <input type='text' class='form-control input-sel' name='ImportateurPhone' id='ImportateurPhone' value='{this.ImportateurPhone}' />
                                </div>
                            </div>
                        </div>
                        <div class='col-12'>
                            <div class='col-12'>
                                <label class='control-label mt-2'>E-mail:</label>
                                <div class='input-group'>
                                    <input type='text' class='form-control input-sel' name='ImportateurMail' id='ImportateurMail' value='{this.ImportateurMail}' />
                                </div>
                            </div>
                        </div>
                    </div>
                </td>
                <td colspan='2'>
                    <div class='form-group mb-3'>
                        <div class='col-12'>
                            <label class='control-label mt-2'>Vendeur (nom/adresse)</label>
                            <div class='input-group'>
                                <input type='text' class='form-control input-sel' name='VendeurNomAdress' id='VendeurNomAdress' value='{this.VendeurNomAdress}' />
                            </div>
                        </div>
                        <div class='row'>
                            <div class='col-6'>
                                <label class='control-label mt-2'>Ville</label>
                                <div class='input-group'>
                                    <input type='text' class='form-control input-sel' name='VendeurVille' id='VendeurVille' value='{this.VendeurVille}' />
                                </div>
                            </div>
                            <div class='col-6'>
                                <label class='control-label mt-2'>Pays Hors zone CEMAC </label>
                                <div class='input-group'>
                                    <input type='text' class='form-control input-sel' name='VendeurPaysHorsCemac' id='VendeurPaysHorsCemac' value='{this.VendeurPaysHorsCemac}' />
                                </div>
                            </div>
                        </div>
                        <div class='row'>
                            <div class='col-6'>
                                <label class='control-label mt-2'>Téléphone</label>
                                <div class='input-group'>
                                    <input type='text' class='form-control input-sel' name='VendeurPhone' id='VendeurPhone' value='{this.VendeurPhone}' />
                                </div>
                            </div>
                            <div class='col-6'>
                                <label class='control-label mt-2'>Télécopie/Fax</label>
                                <div class='input-group'>
                                    <input type='text' class='form-control input-sel' name='VendeurFax' id='VendeurFax' value='{this.VendeurFax}' />
                                </div>
                            </div>
                        </div>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan='2'>
                    <div class='form-group mb-3'>
                        <div class=''>
                            <label class='control-label mt-2'>Lieu de dédouanement</label>
                            <div class='input-group'>
                                <input type='text' class='form-control input-sel' name='LieuDeroulement' id='LieuDeroulement' value='{this.LieuDedouagnement}' />
                            </div>
                        </div>
                    </div>
                </td>
                <td colspan='2'>
                    <div class='form-group mb-3'>
                        <div class='row'>
                            <div class='col-6'>
                                <label class='control-label mt-2'>Pays de provenance</label>
                                <div class='input-group'>
                                    <input type='text' class='form-control input-sel' name='PaysProv' id='PaysProv' value='{this.PaysProv}' />
                                </div>
                            </div>
                            <div class='col-6'>
                                <label class='control-label mt-2'>Pays d'origine</label>
                                <input type='text' class='form-control input-sel' name='PaysOrig' id='PaysOrig' value='{this.PaysOrig}' />
                            </div>
                        </div>

                    </div>
                </td>
            </tr>
            <tr>
                <td colspan='2'>
                    <div class='form-group mb-3'>
                        <div class='col-12'>
                            <label class='control-label mt-2'>Reference et date domiciliation</label>
                            <input type='text' placeholder='Date' class='form-control input-sel' name='DateDomiciliation' id='DateDomiciliation' value='{this.RefDateDomici}' />
                        </div>
                    </div>
                </td>
                <td colspan='2'>
                    <div class='form-group mb-3'>
                        <div class='col-12'>
                            <label class='control-label mt-2'>Banque domiciliataire</label>
                            <input type='text' class='form-control input-sel' name='BanqueDomi' id='BanqueDomi' value='{this.BanqueDomi}' />
                        </div>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan='2'>
                    <div class='form-group mb-3'>
                        <div class='row'>
                            <div class='col-6'>
                                <label class='control-label mt-2'>Devise</label>
                                <input type='text' class='form-control input-sel' name='DeviseMonetaireId' id='DeviseMonetaireId' value='{this.DeviseToString}' />
                            </div>
                            <div class='col-6'>
                                <label class='control-label mt-2'>Valeur totale (devises)</label>
                                <input type='text' class='form-control input-sel' name='ValeurDevise' id='ValeurDevise' value='{this.ValeurDevise}' />
                            </div>
                        </div>

                    </div>
                </td>
                <td colspan='2'>
                    <div class='form-group mb-3'>
                        <div class='row'>
                            <div class='col-4'>
                                <label class='control-label mt-2'>N° Facture pro forma</label>
                                <input type='text' style='flex:1' class='form-control input-sel' name='NumFacturePro' id='NumFacturePro' value='{this.NumFacturePro}' />
                            </div>
                            <div class='col-4'>
                                <label class='control-label mt-2'>Date</label>
                                <input type='text' class='form-control input-sel' name='DateFacturePro' id='DateFacturePro' value='{(this.DateFacturePro != null ? this.DateFacturePro.Value.ToString("dd/MM/yyyy") : "")}' />
                            </div>
                            <div class='col-4'>
                                <label class='control-label mt-2'>Modalité règlement /</label>
                                <input type='text' class='form-control input-sel' name='ModalReglement' id='ModalReglement' value='{this.ModalReglement}' />
                            </div>
                        </div>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan='2'>
                    <div class='form-group mb-3'>
                        <div class='row'>
                            <div class='col-5'>
                                <label class='control-label mt-2'>Terme de vente:</label>
                                <input type='text' class='form-control input-sel' name='TermeVente' id='TermeVente' value='{this.TermeVente}' />
                            </div>
                            <div class='col-7'>
                                <label class='control-label mt-2'>Valeur FOB (devises)</label>
                                <input type='text' class='form-control input-sel' name='ValeurFOB' id='ValeurFOB' value='{this.ValeurFOB}' />
                            </div>

                        </div>
                    </div>
                </td>
                <td colspan='2'>
                    <div class='form-group mb-3'>
                        <div class='row'>
                            <div class='col-4'>
                                <label class='control-label mt-2'>Taux de change</label>
                                <input type='text' class='form-control input-sel' name='TauxChange' id='TauxChange' value='{this.TauxChange}' />
                            </div>
                            <div class='col-4'>
                                <label class='control-label mt-2'>Valeur en CFA</label>
                                <input type='text' class='form-control input-sel' name='ValeurCFA' id='ValeurCFA' value='{this.ValeurCFA}' />
                            </div>
                            <div class='col-4'>
                                <label class='control-label mt-2'>Valeur en devise</label>
                                <input type='text' class='form-control input-sel' name='ValeurDevise' id='ValeurDevise' value='{this.ValeurDevise}' />
                            </div>
                        </div>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan='2'>
                    <div class='form-group mb-3'>
                        <div class='col-12'>
                            <label class='control-label mt-2'>Description du service</label>
                            <input type='text' class='form-control input-sel' name='TermeVente' id='Description' value='{this.Description}' />
                        </div>
                    </div>
                </td>
                <td colspan='2'>
                    <div class='form-group mb-3'>
                        <div style='display:flex'>
                            <div style='flex:1;padding-right:3px;'>
                                <label class='control-label mt-2'>Pos. Tarifaire </label>
                                <input type='text' class='form-control input-sel' style='width:80%' name='PosTarif' id='PosTarif' value='{this.PosTarif}' />
                            </div>
                            <div style='flex:1.4;padding-right:3px;'>
                                <label class='control-label mt-2'>FOB en devise</label>
                                <input type='text' class='form-control input-sel' style='width:80%' name='FOBDevise' id='FOBDevise' value='{this.FOBDevise}' />
                            </div>

                        </div>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan='4'>
                    <div class='row'>
                        <div class='col-8'>
                            <div class='input-group'>
                                <label class='control-label mt-2'>Taxe d'inspection :</label>
                                <input type='text' class='form-control input-sel' name='TypeExpedition' id='TypeExpedition' value='{this.TaxeInsp}' />
                            </div>
                        </div>
                    </div>
                    <div class='row'>
                        <div class='col-4'>
                            <div class='control-group'>
                                <label class='control-label mt-2'>Chèque N° / Virement :</label>
                                <input type='text' class='form-control input-sel' name='ChequeNum' id='ChequeNum' value='{this.ChequeNum}' />
                            </div>
                        </div>
                        <div class='col-2'>
                            <label class='control-label mt-2'>Du</label>
                            <input type='text' class='form-control input-sel' name='ChequeDate' id='ChequeDate' value='{(this.ChequeDate!=null?this.ChequeDate.Value.ToString("dd/MM/yyyy"):"")}' />
                        </div>
                        <div class='col-3'>
                            <label class='control-label mt-2'>Banque</label>
                            <input type='text' class='form-control input-sel' name='ChequeBanque' id='ChequeBanque' value='{this.ChequeBanque}' />
                        </div>
                        <div class='col-3'>
                            <label class='control-label mt-2'>Montant CFA</label>
                            <input type='text' class='form-control input-sel' name='ChequeMontantCFA' id='ChequeMontantCFA' value='{this.ChequeMontantCFA}' />
                        </div>
                    </div>
                </td>
            </tr>
            <tr style='height:160px'>
                <th><p>IMPORTATEUR</p></th>
                <th>
                    <p style='margin-top:-55px'>
                        BANQUE
                        <br />
                        Atteste de la domiciliation de la présente importation
                        <br />
                        (Signature et cachet)
                    </p>
                </th>
                <th>
                    <p>
                        AUTORITE TECHNIQUE
                        <br />
                        (Reference et date)
                        <br />
                    </p>
                </th>
                <th>
                    <p>DOUANE</p>
                </th>
            </tr>
        </table>
        <p style='margin-top:0px'>Annexe à l'instruction N°007/GR/2019 du 10 juin 2019</p>
    </div>
</body>
</html>

            "; 
        }

        public override string PrintLettreEngagement(string genre, string NonComplet, string fonction, string entreprise, string banque, string vilePays_fournisseur, string dateLivraison, string ville, string dateJour)
        {
            string li = "";
            if (!string.IsNullOrEmpty(InfoPercentage))
                foreach (var item in InfoPercentage.Split('-'))
                {
                    if (!item.Contains("Documents manquants"))
                        li += "<li>" + item + "</li>";
                }
            string style = @"
                <style>
        .page {
            padding-left:20mm ;
            padding-right:20mm ;
            padding-top:15mm ;
            padding-bottom:15mm ;
            background: white;
        }

        p,ul,li {
            text-indent: 50px;
            line-height: 2;
            word-spacing: 7px;
            font-size:17pt;
            text-align:justify;
       }
    </style>

            ";
            return $@"
    <!DOCTYPE html>

<html lang='en' xmlns='http://www.w3.org/1999/xhtml'>
<head>
    <meta charset='utf-8' />
    <title></title>
    {style}
</head>
<body style=''>
    <div class='page'>
     <p>
        Je soussigné {genre} {NonComplet} agissant en qualité de {fonction} de la société {entreprise}, m'engage conformément
        à la réglementation des changes en vigueur en zone CEMAC, à fournir à {banque} dès reception, toute la documentation
        requise pour l'exécution de mon transfert d'un montant de {MontantStringDevise} en faveur de mon fournisseur {GetFournisseur}
        localisé à {vilePays_fournisseur} dans le cadre de l'importation de {Description} dont la date de livraison est
        prévue le {dateLivraison}.
    </p>
    <p>
        Il s'agit précisement de :
    </p>
    <p>
        <ul>
            {li}
        </ul>
    </p>
    <p style='text-align:right'>
        Fait à {ville}, le {dateJour}
    </p>
    </div>
</body>
</html>
";

        }

        #endregion


    }
}