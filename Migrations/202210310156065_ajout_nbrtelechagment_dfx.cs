namespace genetrix.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ajout_nbrtelechagment_dfx : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.References", "NbrTelechargement1", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.References", "NbrTelechargement1");
        }
    }
}
