namespace genetrix.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deviseId_no_required : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Dossier", "DeviseMonetaireId", "dbo.DeviseMonetaire");
            DropIndex("dbo.Dossier", new[] { "DeviseMonetaireId" });
            AlterColumn("dbo.Dossier", "DeviseMonetaireId", c => c.Int());
            CreateIndex("dbo.Dossier", "DeviseMonetaireId");
            AddForeignKey("dbo.Dossier", "DeviseMonetaireId", "dbo.DeviseMonetaire", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Dossier", "DeviseMonetaireId", "dbo.DeviseMonetaire");
            DropIndex("dbo.Dossier", new[] { "DeviseMonetaireId" });
            AlterColumn("dbo.Dossier", "DeviseMonetaireId", c => c.Int(nullable: false));
            CreateIndex("dbo.Dossier", "DeviseMonetaireId");
            AddForeignKey("dbo.Dossier", "DeviseMonetaireId", "dbo.DeviseMonetaire", "Id", cascadeDelete: true);
        }
    }
}
