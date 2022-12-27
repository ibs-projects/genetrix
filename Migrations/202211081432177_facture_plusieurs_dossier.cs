namespace genetrix.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class facture_plusieurs_dossier : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Justificatif", "DossierId", "dbo.Dossier");
            AddColumn("dbo.Dossier", "Justificatif_Id", c => c.Int());
            AddColumn("dbo.Justificatif", "Dossier_Dossier_Id", c => c.Int());
            CreateIndex("dbo.Dossier", "Justificatif_Id");
            CreateIndex("dbo.Justificatif", "Dossier_Dossier_Id");
            AddForeignKey("dbo.Dossier", "Justificatif_Id", "dbo.Justificatif", "Id");
            AddForeignKey("dbo.Justificatif", "Dossier_Dossier_Id", "dbo.Dossier", "Dossier_Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Justificatif", "Dossier_Dossier_Id", "dbo.Dossier");
            DropForeignKey("dbo.Dossier", "Justificatif_Id", "dbo.Justificatif");
            DropIndex("dbo.Justificatif", new[] { "Dossier_Dossier_Id" });
            DropIndex("dbo.Dossier", new[] { "Justificatif_Id" });
            DropColumn("dbo.Justificatif", "Dossier_Dossier_Id");
            DropColumn("dbo.Dossier", "Justificatif_Id");
            AddForeignKey("dbo.Justificatif", "DossierId", "dbo.Dossier", "Dossier_Id", cascadeDelete: true);
        }
    }
}
