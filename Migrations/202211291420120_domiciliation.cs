namespace genetrix.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class domiciliation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Dossier", "Description", c => c.String());
            AddColumn("dbo.Dossier", "ImportaVille", c => c.String());
            AddColumn("dbo.Dossier", "ImportaPays", c => c.String());
            AddColumn("dbo.Dossier", "ImportaNom", c => c.String());
            AddColumn("dbo.Dossier", "ImportaNumInscri", c => c.String());
            AddColumn("dbo.Dossier", "ImportaProfession", c => c.String());
            AddColumn("dbo.Dossier", "ImportaImmatri", c => c.String());
            AddColumn("dbo.Dossier", "ImportaAdresse", c => c.String());
            AddColumn("dbo.Dossier", "ImportateurCodeAgr", c => c.String());
            AddColumn("dbo.Dossier", "ImportateurPhone", c => c.String());
            AddColumn("dbo.Dossier", "ImportateurMail", c => c.String());
            AddColumn("dbo.Dossier", "VendeurNom", c => c.String());
            AddColumn("dbo.Dossier", "VendeurAdresse", c => c.String());
            AddColumn("dbo.Dossier", "VendeurVille", c => c.String());
            AddColumn("dbo.Dossier", "VendeurPhone", c => c.String());
            AddColumn("dbo.Dossier", "VendeurFax", c => c.String());
            AddColumn("dbo.Dossier", "VendeurPaysHorsCemac", c => c.String());
            AddColumn("dbo.Dossier", "LieuDeroulement", c => c.String());
            AddColumn("dbo.Dossier", "PaysProv", c => c.String());
            AddColumn("dbo.Dossier", "PaysOrig", c => c.String());
            AddColumn("dbo.Dossier", "RefDomiciliation", c => c.String());
            AddColumn("dbo.Dossier", "DateDomiciliation", c => c.DateTime());
            AddColumn("dbo.Dossier", "BanqueDomi", c => c.String());
            AddColumn("dbo.Dossier", "NumFacturePro", c => c.String());
            AddColumn("dbo.Dossier", "DateFacturePro", c => c.DateTime());
            AddColumn("dbo.Dossier", "ModalReglement", c => c.String());
            AddColumn("dbo.Dossier", "TermeVente", c => c.String());
            AddColumn("dbo.Dossier", "ValeurFOB", c => c.String());
            AddColumn("dbo.Dossier", "TauxChange", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Dossier", "ValeurCFA", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Dossier", "ValeurDevise", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Dossier", "PosTarif", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Dossier", "FOBDevise", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Dossier", "TaxeInsp", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Dossier", "ChequeNum", c => c.String());
            AddColumn("dbo.Dossier", "ChequeDate", c => c.DateTime());
            AddColumn("dbo.Dossier", "ChequeBanque", c => c.String());
            AddColumn("dbo.Dossier", "ChequeMontantCFA", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Dossier", "ModeTransport", c => c.String());
            AddColumn("dbo.Dossier", "TypeExpedition", c => c.String());
            AddColumn("dbo.Dossier", "Qte", c => c.Int());
            AddColumn("dbo.Dossier", "Unite", c => c.String());
            AddColumn("dbo.Dossier", "Obtention", c => c.String());
            AddColumn("dbo.Dossier", "Peremption", c => c.String());
            AddColumn("dbo.Dossier", "ImportateurCode", c => c.String());
            AddColumn("dbo.Dossier", "ServiceConnexe", c => c.String());
            AddColumn("dbo.Dossier", "Discriminator", c => c.String(nullable: false, maxLength: 128));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Dossier", "Discriminator");
            DropColumn("dbo.Dossier", "ServiceConnexe");
            DropColumn("dbo.Dossier", "ImportateurCode");
            DropColumn("dbo.Dossier", "Peremption");
            DropColumn("dbo.Dossier", "Obtention");
            DropColumn("dbo.Dossier", "Unite");
            DropColumn("dbo.Dossier", "Qte");
            DropColumn("dbo.Dossier", "TypeExpedition");
            DropColumn("dbo.Dossier", "ModeTransport");
            DropColumn("dbo.Dossier", "ChequeMontantCFA");
            DropColumn("dbo.Dossier", "ChequeBanque");
            DropColumn("dbo.Dossier", "ChequeDate");
            DropColumn("dbo.Dossier", "ChequeNum");
            DropColumn("dbo.Dossier", "TaxeInsp");
            DropColumn("dbo.Dossier", "FOBDevise");
            DropColumn("dbo.Dossier", "PosTarif");
            DropColumn("dbo.Dossier", "ValeurDevise");
            DropColumn("dbo.Dossier", "ValeurCFA");
            DropColumn("dbo.Dossier", "TauxChange");
            DropColumn("dbo.Dossier", "ValeurFOB");
            DropColumn("dbo.Dossier", "TermeVente");
            DropColumn("dbo.Dossier", "ModalReglement");
            DropColumn("dbo.Dossier", "DateFacturePro");
            DropColumn("dbo.Dossier", "NumFacturePro");
            DropColumn("dbo.Dossier", "BanqueDomi");
            DropColumn("dbo.Dossier", "DateDomiciliation");
            DropColumn("dbo.Dossier", "RefDomiciliation");
            DropColumn("dbo.Dossier", "PaysOrig");
            DropColumn("dbo.Dossier", "PaysProv");
            DropColumn("dbo.Dossier", "LieuDeroulement");
            DropColumn("dbo.Dossier", "VendeurPaysHorsCemac");
            DropColumn("dbo.Dossier", "VendeurFax");
            DropColumn("dbo.Dossier", "VendeurPhone");
            DropColumn("dbo.Dossier", "VendeurVille");
            DropColumn("dbo.Dossier", "VendeurAdresse");
            DropColumn("dbo.Dossier", "VendeurNom");
            DropColumn("dbo.Dossier", "ImportateurMail");
            DropColumn("dbo.Dossier", "ImportateurPhone");
            DropColumn("dbo.Dossier", "ImportateurCodeAgr");
            DropColumn("dbo.Dossier", "ImportaAdresse");
            DropColumn("dbo.Dossier", "ImportaImmatri");
            DropColumn("dbo.Dossier", "ImportaProfession");
            DropColumn("dbo.Dossier", "ImportaNumInscri");
            DropColumn("dbo.Dossier", "ImportaNom");
            DropColumn("dbo.Dossier", "ImportaPays");
            DropColumn("dbo.Dossier", "ImportaVille");
            DropColumn("dbo.Dossier", "Description");
        }
    }
}
