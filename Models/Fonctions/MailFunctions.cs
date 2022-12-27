using genetrix.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace genetrix.Models.Fonctions
{
    public class MailFunctions
    {
        public static bool SendMail(Models.MailModel objModelMail,ApplicationDbContext db=null,int? idDossier=null,bool persiste=true)
        {
            string emailsender = ConfigurationManager.AppSettings["emailsender"].ToString();
            string emailSenderPassword = ConfigurationManager.AppSettings["emailSenderPassword"].ToString();
            using (MailMessage mail = new MailMessage(emailsender, objModelMail.To))
            {
                try
                {
                    mail.Subject = objModelMail.Subject;
                    mail.Body = objModelMail.Body;

                    //copy
                    try
                    {
                        foreach (var item in objModelMail.CC)
                        {
                            try
                            {
                                MailAddress copy = new MailAddress(item);
                                mail.CC.Add(copy);
                            }
                            catch (Exception)
                            {}
                        }
                    }
                    catch (Exception ee)
                    {}

                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = ConfigurationManager.AppSettings["smtpserver"].ToString();
                    smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSSL"]);
                    //smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(emailsender, emailSenderPassword);
                    smtp.Port = Convert.ToInt16(ConfigurationManager.AppSettings["portnumber"]);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Send(mail);

                    if (persiste)
                    {
                        var dateNow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "W. Central Africa Standard Time");
                        var mailObjt = new Mail()
                        {
                            Date = dateNow,
                            AdresseDest = objModelMail.To,
                            AdresseEmeteur = emailsender,//from,
                            Objet = objModelMail.Subject,
                            Message = objModelMail.Body,
                            DossierId = idDossier
                        };
                        db.GetMails.Add(mailObjt);
                        db.SaveChanges(); 
                    }
                    return true;
                }
                catch (Exception e)
                {}

                return false;
            }
        }

        public static void EnvoiMail(ReferenceBanque re, string fournisseur, string nomclient, int idbanque, string subject, string body1, ApplicationDbContext db, string itemsRejet="")
        {
            Dossier dossier = re.Dossiers.FirstOrDefault();
            fournisseur = dossier != null ? dossier.Fournisseur != null ? dossier.Fournisseur.Nom : "" : "";
            nomclient = dossier != null ? dossier.Client != null ? dossier.Client.Nom : "" : "";
            var txt = TabReference(re, fournisseur, re.Montant, (re.Devise != null ? re.Devise.Nom : ""), nomclient,idbanque,db);
            if (!string.IsNullOrEmpty(itemsRejet))
            {
                string eltsmodif = "";
                try
                {
                    foreach (var item in itemsRejet.Split(';'))
                    {
                        eltsmodif += $"<li> {item}</li>";
                    }
                }
                catch (Exception)
                { }
                body1 += $"<h6 style=\"margin-top:15px\">Eléments consernés</h6>"
                                + $"<ul>{eltsmodif}</ul>";
            }

            var body = "La référence " + re.NumeroRef + " " + body1
                      + "<hr />"
                      + txt;
            var client = re.Dossiers.ToList()[0].Client;
            var ges = client.Banques.FirstOrDefault(b => b.Site.BanqueId(db) == idbanque).Gestionnaire;
            try
            {
                MailFunctions.SendMail(new Models.MailModel()
                {
                    To = ges != null ? ges.Email : "",
                    CC = (from a in client.Adresses select a.Email).ToList(),
                    Subject = subject,
                    Body = body
                }, db);
            }
            catch (Exception)
            {}
            ges = null;
        }

        public static string TabReference(ReferenceBanque item, string Fournisseur, double montant, string devise, string client,int idbanque, ApplicationDbContext db)
        {
            var banque = db.GetBanques.Find(idbanque);
            var bank = banque != null ? banque.Nom : "";
            banque = null;
            var txt = "<div class=\"table-responsive\">"
                    + "<table id=\"datatable-buttons\" class=\"table table-striped table-bordered dt-responsive nowrap table-responsive\">"
                    + "<tbody>"
                    + "<tr>"
                    + "<th>"
                    + "Référence "
                    + "</th> "
                    + "<th>"
                    + "Date reception"
                    + "</th> "
                    + "<th>"
                    + "NATURE"
                    + "</th> "
                    + ""
                    + "<th>"
                    + "CvEURO"
                    + "</th> "
                    + "<th>"
                    + "Banque"
                    + "</th> "
                    + "<th>"
                    + "Fournisseur "
                    + "</th> "
                    + "<th>"
                    + "Montant "
                    + "</th> "
                    + "<th>"
                    + "Devise"
                    + "</th> "
                    + "<th>"
                    + "Client"
                    + "</th>"
                    + "<th></th>"
                    + "</tr>"
                    + "</tbody>"
                    + "<tfoot>"
                    + "<tr>"
                    + "<td>"
                    + item.NumeroRef
                    + "</td> "
                    + "<td>"
                    + item.DateReception
                    + "</td> "
                    + "<td>"
                    + item.NATURE
                    + "</td> "
                    + "<td>"
                    + item.CvEURO
                    + "</td> "
                    + "<td>"
                    + bank
                    + "</td> "
                    + "<td>"
                    + Fournisseur
                    + "</td> "
                    + "<td>"
                    + montant
                    + "</td> "
                    + "<td>"
                    + devise
                    + "</td> "
                    + "<td>"
                    + client
                    + "</td> "
                    + "</tr> "
                    + "</tfoot>"
                    + "</table>"
                    + "</div>";
            return txt;
        }

        public static string TabDFX(List<Dossier> dossiers)
        {
            string tmp = "";
            foreach (var item in dossiers)
            {
                tmp += $"<tr style=\"border:1px solid darkblue\"><td>{item.MontantString}</td>"
                       +$"<td>{item.DeviseToString}</td>"
                       + $"<td>{item.GetFournisseur}</td></tr>";
            }
            var txt = "<div class=\"table-responsive\">"
                    + "<table class=\"table\">"
                    + "<thead style=\"background-color:darkblue;color:white; border:1px solid darkblue;\">"
                    + "<tr>"
                    + "<th>"
                    + "Montant"
                    + "</th> "
                    + "<th>"
                    + "Devise"
                    + "</th> "
                    + "<th>"
                    + "Fournisseur"
                    + "</th> "
                    + "</tr>"
                    + "</thead>"
                    + "<tbody>"
                    +tmp
                    + "</tbody>"
                    + "</table>"
                    + "</div>";
            return txt;
        }


        public static async Task<bool> ChangeEtapeDossier(int banqueId,string agentId,int eta,Dossier dossier,ApplicationDbContext db,Structure site, bool rejet=false,string msg="",bool estGroupe=false, string itemsRejet = null)
        {
            try
            {
                if ((dossier.DFX6FP6BEAC !=3 || dossier.DfxId==null || dossier.DfxId==0) && eta==18)
                    eta= 17;

                var dateNow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "W. Central Africa Standard Time");

                var ges = db.GetBanqueClients.FirstOrDefault(d => d.IdSite == dossier.IdSite && d.ClientId == dossier.ClientId).Gestionnaire;
                var agent = db.Users.FirstOrDefault(d => d.Id ==agentId );


                //var v1 = dossier.GetEtapDossier(eta)[2].Replace("dossier", "");
                //var v2 =rejet?"rejeté suite au motif suivant: ": dossier.DeviseMonetaire != null ? dossier.DeviseMonetaire.Nom : "null";
                var banque = db.GetBanques.Find(dossier.Site.BanqueId(db));
                var fournisseur = dossier.Fournisseur != null ? dossier.Fournisseur.Nom : "";

                var status = db.GetStatuts.FirstOrDefault(s => s.Etape == eta);

                dossier = db.GetDossiers.Find(dossier.Dossier_Id);
                if (eta == 4)
                {
                    dossier.ValiderDouane = false;
                }

                if (eta < 0)
                {
                    dossier.EtapePrecedenteDosier = dossier.EtapesDosier;
                }else if (dossier.EtapePrecedenteDosier < eta)
                {
                    dossier.EtapePrecedenteDosier = dossier.EtapesDosier;
                }
                string eltsmodif = "";
                if (!string.IsNullOrEmpty(msg))
                {
                    dossier.Message = "<h5>" + agent.NomComplet +$" ({site.Nom}) {dateNow} </h5>"+ msg+"<hr/>"+ dossier.Message;
                    if (!string.IsNullOrEmpty(itemsRejet))
                    {
                        try
                        {
                            foreach (var item in itemsRejet.Split(';'))
                            {
                                if(!string.IsNullOrEmpty(item)) 
                                    eltsmodif += $"<li> {item}</li>";
                            }
                        }
                        catch (Exception)
                        {}
                        eltsmodif = $"<h4 style=\"margin-top:15px\">Eléménts affectés</h4>"
                                        +$"<ol>{eltsmodif}</ol>"; 
                        dossier.Message += eltsmodif;
                    }

                }
                if (eta == 22) 
                    eta = 23;

                dossier.EtapesDosier = eta;

                dossier.IdPrecedentResponsable = dossier.IdAgentResponsableDossier;
                //if(eta==6 || eta==9 || eta <0)
                //    dossier.IdAgentResponsableDossier = null;
                dossier.DateNouvelleEtape = dateNow;
                if (eta<0)
                {
                    dossier.ValiderConformite = false;
                    //dossier.ReferenceExterneId = null;
                }

                if (dossier.EtapesDosier==15)
                {
                    dossier.DateEnvoiBeac = dateNow;
                }

                if (dossier.EtapesDosier==19 || dossier.EtapesDosier==20)
                {
                    dossier.ObtDevise = dateNow;
                }
             
                if (eta==26)
                {
                    dossier.DateArchivage = dateNow;
                }
                if (eta==6)
                {
                    dossier.EstPasséConformite = true;
                    //// Le chef de la conformité reçoit le mail
                    ///
                    if (false)
                    {
                        try
                        {
                            var chef = db.Structures.Include("Responsable").FirstOrDefault(s => s.NiveauDossier == 6).GetResponsableEmail;
                            MailFunctions.SendMail(new MailModel()
                            {
                                To = chef,
                                Subject = "Reception du dossier " + dossier.GetClient + $" ({dossier.MontantStringDevise} de {dossier.GetFournisseur})",
                                Body = $"Le dossier ({dossier.MontantStringDevise}, fournisseur {dossier.GetFournisseur}) du client {dossier.GetClient}  a été transmis par l'agent {agent.NomComplet} ce jour dans votre service pour vérification."
                            });
                        }
                        catch (Exception)
                        { } 
                    }
                }

                if (dossier.EtapesDosier==9)
                {
                    if (dossier.DeviseToString == "EUR" && dossier.MontantCV <= banque.MontantDFX)
                        dossier.DFX6FP6BEAC = 1;//DFX
                    else dossier.DFX6FP6BEAC= 3;
                }

                if (eta<0)
                {
                    dossier.EstPasséConformite = false;
                    dossier.ValiderConformite = false;
                }

                //remplissage champ acces rapide
                switch (eta)
                {
                    case 2:dossier.Date_Etape2=dateNow;break;
                    case 3:dossier.Date_Etape3 = dateNow;break;
                    case 4:dossier.Date_Etape4 = dateNow;break;
                    case 5:dossier.Date_Etape5 = dateNow;break;
                    case 6:dossier.Date_Etape6 = dateNow;break;
                    case 7:dossier.Date_Etape7 = dateNow;break;
                    case 8:dossier.Date_Etape8 = dateNow;break;
                    case 9:dossier.Date_Etape9 = dateNow;break;
                    case 10:dossier.Date_Etape10 = dateNow;break;
                    case 11:dossier.Date_Etape11 = dateNow;break;
                    case 12:dossier.Date_Etape12 = dateNow;break;
                    case 13:dossier.Date_Etape13 = dateNow;break;
                    case 14:dossier.Date_Etape14 = dateNow;break;
                    case 15:dossier.Date_Etape15 = dateNow;break;
                    case 16:dossier.Date_Etape16 = dateNow;break;
                    case 17:dossier.Date_Etape17 = dateNow;break;
                    case 18:dossier.Date_Etape18 = dateNow;break;
                    case 19:dossier.Date_Etape19 = dateNow;break;
                    case 20:dossier.Date_Etape20 = dateNow;break;
                    case 21:dossier.Date_Etape21 = dateNow;break;
                    case 22:
                    case 23:
                        dossier.Date_Etape22 = dateNow;
                        dossier.Date_Etape23 = dateNow;
                        break;
                    case 24:dossier.Date_Etape24 = dateNow;break;
                    case 25:dossier.Date_Etape25 = dateNow;break;
                    default:
                        break;
                }
                db.SaveChanges();
                //await db.SaveChangesAsync();
                StatutDossier stat = new StatutDossier();
                string[] pp = new string[] { };

                try
                {
                    var tab = dossier.GetVariables(banque.Nom,site.Nom, agent.NomComplet);
                    if (!string.IsNullOrEmpty(msg))
                        tab.Add("[MOTIF]", msg);
                    if(false)
                        pp = status.GetParametre(tab);
                    else
                        pp = dossier.GetParametre(tab);

                    //create statut dossier
                    stat = db.GetStatutDossiers.Add(new StatutDossier()
                    {
                        IdDossier = dossier.Dossier_Id,
                        Date = dateNow,
                        Etat=dossier.EtapesDosier,
                        IdStatut = status.Id,
                        IdStructure = site.Id,
                        IdAgent=agentId,
                        //Objet=pp[0],
                        //Message=pp[1],
                        //Objet2 = pp[2],
                        //Message2 = pp[3],
                        Statut1 = pp[4],
                        Motif=msg,
                        EstConformite=dossier.EstPasséConformite
                    });
                    db.SaveChanges();
                }
                catch (Exception ee)
                {}
                if (!estGroupe)
                {
                    List<string> clientEmails = new List<string>();
                    foreach (var item in dossier.Client.Adresses.ToList())
                    {
                        clientEmails.Add(item.Email);
                    }
                    var emailGes = "";
                    if (ges != null)
                        try
                        {
                            emailGes = ges.Email;
                        }
                        catch (Exception)
                        { }

                    //mail cleint
                    var firstEmail = clientEmails[0];
                    clientEmails.Remove(firstEmail);
                    //if (!string.IsNullOrEmpty(stat.Message))
                    if (!string.IsNullOrEmpty(pp[1]))
                    {
                        SendMail(new MailModel()
                        {
                            Subject = pp[0],// rejet ? "Dossier rejeté" : dossier.GetEtapDossier(eta)[2],
                            To = firstEmail,
                            CC = clientEmails,
                            //Body = stat.Message+"<br/>"+ eltsmodif,
                            Body = pp[1]+ "<br/>"+ eltsmodif,
                        }, db, dossier.Dossier_Id);
                    }
                    clientEmails = new List<string>();
                    //mail gestionnaire et le responsable de l'agence
                    var responsableEmail = "";
                    if (!string.IsNullOrEmpty(dossier.IdAgentResponsableDossier))
                    {
                        try
                        {
                            responsableEmail = db.GetCompteBanqueCommerciales.Find(dossier.IdAgentResponsableDossier).Email;
                            if (responsableEmail != emailGes) clientEmails.Add(responsableEmail);
                            try
                            {
                                if (responsableEmail != site.GetResponsableEmail && emailGes != site.GetResponsableEmail) clientEmails.Add(site.GetResponsableEmail);
                            }
                            catch (Exception)
                            {}                        
                        }
                        catch (Exception)
                        { }
                    }
                    //if (!string.IsNullOrEmpty(stat.Message2))
                    if (!string.IsNullOrEmpty(pp[3]))
                    {
                        SendMail(new MailModel()
                        {
                            Subject = pp[2],
                            To = emailGes,
                            CC = clientEmails,
                            //Body = stat.Message2+"<br />"+ eltsmodif,
                            Body = pp[3] + "<br />"+ eltsmodif,
                        }, db, dossier.Dossier_Id);
                    }
                    clientEmails = null; 
                }
                return true;
            }
            catch (Exception ee)
            { }
            return false;
        }
        
        public static async Task<bool> ChangeEtapeDossier2(int banqueId,string agentId,int eta,ReferenceBanque reference,ApplicationDbContext db,Structure site, bool rejet=false,string msg="", int? idOpSwft = null)
        {
            try
            {
                var dateNow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "W. Central Africa Standard Time");

                var doss = reference.Dossiers.First();
                var ges = db.GetBanqueClients.FirstOrDefault(d => d.IdSite == doss.IdSite && d.ClientId == doss.ClientId).Gestionnaire;
                var agent = db.GetCompteBanqueCommerciales.Find(agentId);

                var banque = db.GetBanques.Find(doss.Site.BanqueId(db));
                var fournisseur = doss.Fournisseur != null ? doss.Fournisseur.Nom : "";

                var status = db.GetStatuts.FirstOrDefault(s => s.Etape == eta);
                if (!string.IsNullOrEmpty(msg))
                {
                    reference.Message = "<h5>" + agent.NomComplet + $" ({site.Nom}) {dateNow} </h5>" + msg + "<hr/>" + reference.Message;
                }

                foreach (var dossier in reference.Dossiers)
                {
                    try
                    {
                        if (eta < 0)
                            dossier.EtapePrecedenteDosier = dossier.EtapesDosier;

                       await ChangeEtapeDossier(banqueId,agentId,eta,dossier,db,site,msg:msg);

                        dossier.IdPrecedentResponsable = dossier.IdAgentResponsableDossier;
                        dossier.DateNouvelleEtape = dateNow;
                    }
                    catch (Exception)
                    {}

                }
                db.SaveChanges();

                StatutDossier stat = new StatutDossier();
                stat.Etat = doss.EtapesDosier;
                Dossier dosstmp = doss;
                List<string> clientEmails = new List<string>();
                foreach (var item in doss.Client.Adresses.ToList())
                {
                    clientEmails.Add(item.Email);
                }
                var emailGes = "";
                if (ges != null)
                    try
                    {
                        emailGes = ges.Email;
                    }
                    catch (Exception)
                    { }

                //mail cleint
                var firstEmail = clientEmails[0];
                clientEmails.Remove(firstEmail);
                if (!string.IsNullOrEmpty(stat.Message))
                {
                    SendMail(new MailModel()
                    {
                        Subject = stat.Objet,// rejet ? "Dossier rejeté" : dossier.GetEtapDossier(eta)[2],
                        To = firstEmail,
                        CC = clientEmails,
                        Body = stat.Message,
                    }, db, doss.Dossier_Id);
                }
                clientEmails = new List<string>();
                //mail gestionnaire
                var responsableEmail = "";
                if (!string.IsNullOrEmpty(doss.IdAgentResponsableDossier))
                {
                    try
                    {
                        responsableEmail = db.GetCompteBanqueCommerciales.Find(doss.IdAgentResponsableDossier).Email;
                        if (responsableEmail != emailGes) clientEmails.Add(responsableEmail);
                    }
                    catch (Exception)
                    { }
                }
                if (!string.IsNullOrEmpty(stat.Message2))
                {
                    SendMail(new MailModel()
                    {
                        Subject = stat.Objet2,
                        To = emailGes,
                        CC = clientEmails,
                        Body = stat.Message2,
                    }, db, doss.Dossier_Id);
                }
                clientEmails = null;
            }
            catch (Exception ee)
            { }
            return false;
        }

        public static async Task<bool> ChangeEtapeDossierDFX(int banqueId,string agentId,int eta,DFX dfx,ApplicationDbContext db,Structure site, bool rejet=false,string msg="", int? idOpSwft = null)
        {
            try
            {
                var dateNow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "W. Central Africa Standard Time");

                var doss = dfx.Dossiers.First();
                var agent = db.GetCompteBanqueCommerciales.Find(agentId);

                var banque = db.GetBanques.Find(doss.Site.BanqueId(db));
                var status = db.GetStatuts.FirstOrDefault(s => s.Etape == eta);
                
                Dossier dostmp = null;
                foreach (var dosgp in dfx.Dossiers.GroupBy(d=>d.ClientId))
                {
                    try
                    {
                        foreach (var dossier in dosgp)
                        {
                            try
                            {
                                if (eta < 0)
                                    dossier.EtapePrecedenteDosier = dossier.EtapesDosier;

                                await ChangeEtapeDossier(banqueId, agentId, eta, dossier, db, site, msg: msg, estGroupe: true);
                                dossier.IdPrecedentResponsable = dossier.IdAgentResponsableDossier;
                                dossier.DateNouvelleEtape = dateNow;
                            }
                            catch (Exception)
                            {}
                        }
                        dostmp = dosgp.FirstOrDefault();
                        //Envoie mail unique
                        var tab = TabDFX(dfx.Dossiers.Where(d=>d.ClientId==dosgp.Key).ToList());
                        if (eta==15)
                        {
                            SendMail(
                                new MailModel()
                                {
                                    Subject = "Dossiers envoyés à la BEAC",
                                    To = dostmp.GetClientEmail,
                                    CC = dostmp.GetAllClientEmail,
                                    Body = "Madame, Monsieur, <br /> <br />"
                                        + $"Nous vous informons de la prise en compte des dossiers des transferts ci-après, dans l’enveloppe DFX transmise ce jour à la BEAC pour allocation des devises :"
                                        + $"{tab}"
                                        + "<br />Nous restons dans l’attente de la couverture effective en devises, et ne manquerons pas de vous tenir informés sur les prochaines étapes de ces dossiers."
                                        + $"<br /> <br />Votre Gestionnaire {dostmp.Get_CiviliteGestionnaire} {dostmp.GestionnaireName} se tient à votre disposition pour tout complément d’information, aux coordonnées suivantes :"
                                        + "<br /> <br />   &emsp;Mail : " + dostmp.GestionnaireEmail
                                        + "<br />   &emsp;Téléphone : " + dostmp.GestionnairePhone
                                        + $"<br /> <br /><b>{banque.Nom}</b> vous remercie pour votre confiance"
                                }
                            );  
                        }
                        else if (eta==19 || eta==20)
                        {
                            SendMail(
                                new MailModel()
                                {
                                    Subject = "Reception devise",
                                    To = dostmp.GetClientEmail,
                                    CC = dostmp.GetAllClientEmail,
                                    Body = "Madame, Monsieur, <br /> <br />"
                                        + "Nous avons le plaisir de vous informer que les dossiers DFX ci-après ont fait l’objet d’allocation de devise par la BEAC. Nous procédons à leur traitement sous 24h maximum."
                                        + $"{tab}"
                                        + "<br /> <br />   &emsp;Mail :" + dostmp.GestionnaireEmail
                                        + "<br />   &emsp;Téléphone :" + dostmp.GestionnairePhone
                                        + $"<br /> <br /><b>{banque.Nom}<b/> vous remercie pour votre confiance"
                                }
                            );  
                        }
                    }
                    catch (Exception)
                    {}

                }
                db.SaveChanges();
                return true;
            }
            catch (Exception ee)
            { }
            return false;
        }
    
        public static string DetailsReference(ReferenceBanque reference)
        {
            var content = "";
            content = "<fieldset> "
                + "<dt> "
                + "Donneur d'ordre"
                + "</dt>"
                + "<dd> "
                + reference.ClientEntre
                + "</dd>"
                + "<dt> "
                + "Fournisseur"
                + "</dt>"
                + "<dd> "
                + reference.GetFournisseur
                + "</dd>"
                + "<dt> "
                + "Montant"
                + "</dt>"
                + "<dd> "
                + reference.MontantString
                + "</dd>"
                + "<dt> "
                + "Devise"
                + "</dt>"
                + "<dd> "
                + reference.DeviseToString
                + "</dd>"
                + "<dt> "
                + "NATURE"
                + "</dt>"
                + "<dd> "
                + reference.NATURE
                + "</dd>"
                + "</fieldset>";
            return content;
        }
        public static string DetailsDossier(Dossier dossier)
        {
            var content = "";
            content = "<fieldset> "
                + "<dt> "
                + "Donneur d'ordre"
                + "</dt>"
                + "<dd> "
                + dossier.GetClient
                + "</dd>"
                + "<dt> "
                + "Fournisseur"
                + "</dt>"
                + "<dd> "
                + dossier.GetFournisseur
                + "</dd>"
                + "<dt> "
                + "Montant"
                + "</dt>"
                + "<dd> "
                + dossier.MontantString
                + "</dd>"
                + "<dt> "
                + "Devise"
                + "</dt>"
                + "<dd> "
                + dossier.DeviseToString
                + "</dd>"
                + "<dt> "
                + "NATURE"
                + "</dt>"
                + "<dd> "
                + dossier.NatureOperation
                + "</dd>"
                + "</fieldset>";
            return content;
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
    }
}