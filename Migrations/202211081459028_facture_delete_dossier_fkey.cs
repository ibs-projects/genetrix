namespace genetrix.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class facture_delete_dossier_fkey : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Justificatif", "DossierId", "dbo.Dossier");
            DropForeignKey("dbo.Dossier", "Justificatif_Id", "dbo.Justificatif");
            DropForeignKey("dbo.Justificatif", "Dossier_Dossier_Id", "dbo.Dossier");
            DropIndex("dbo.Dossier", new[] { "Justificatif_Id" });
            DropIndex("dbo.Justificatif", new[] { "DossierId" });
            DropIndex("dbo.Justificatif", new[] { "Dossier_Dossier_Id" });
            CreateTable(
                "dbo.JustificatifDossiers",
                c => new
                    {
                        Justificatif_Id = c.Int(nullable: false),
                        Dossier_Dossier_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Justificatif_Id, t.Dossier_Dossier_Id })
                .ForeignKey("dbo.Justificatif", t => t.Justificatif_Id, cascadeDelete: true)
                .ForeignKey("dbo.Dossier", t => t.Dossier_Dossier_Id, cascadeDelete: true)
                .Index(t => t.Justificatif_Id)
                .Index(t => t.Dossier_Dossier_Id);
            
            DropColumn("dbo.Dossier", "Justificatif_Id");
            DropColumn("dbo.Justificatif", "DossierId");
            DropColumn("dbo.Justificatif", "Dossier_Dossier_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Justificatif", "Dossier_Dossier_Id", c => c.Int());
            AddColumn("dbo.Justificatif", "DossierId", c => c.Int(nullable: false));
            AddColumn("dbo.Dossier", "Justificatif_Id", c => c.Int());
            DropForeignKey("dbo.JustificatifDossiers", "Dossier_Dossier_Id", "dbo.Dossier");
            DropForeignKey("dbo.JustificatifDossiers", "Justificatif_Id", "dbo.Justificatif");
            DropIndex("dbo.JustificatifDossiers", new[] { "Dossier_Dossier_Id" });
            DropIndex("dbo.JustificatifDossiers", new[] { "Justificatif_Id" });
            DropTable("dbo.JustificatifDossiers");
            CreateIndex("dbo.Justificatif", "Dossier_Dossier_Id");
            CreateIndex("dbo.Justificatif", "DossierId");
            CreateIndex("dbo.Dossier", "Justificatif_Id");
            AddForeignKey("dbo.Justificatif", "Dossier_Dossier_Id", "dbo.Dossier", "Dossier_Id");
            AddForeignKey("dbo.Dossier", "Justificatif_Id", "dbo.Justificatif", "Id");
            AddForeignKey("dbo.Justificatif", "DossierId", "dbo.Dossier", "Dossier_Id", cascadeDelete: true);
        }
    }
}
