namespace genetrix.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _32 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Dossier", "Montant", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Dossier", "Montant", c => c.Single(nullable: false));
        }
    }
}
