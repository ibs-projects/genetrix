namespace genetrix.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FichierMails", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FichierMails", "Name");
        }
    }
}
