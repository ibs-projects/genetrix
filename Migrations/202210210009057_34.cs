namespace genetrix.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _34 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ReferenceBanque", "Echus");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ReferenceBanque", "Echus", c => c.Boolean(nullable: false));
        }
    }
}
