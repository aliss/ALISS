namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServicesSuggestedFlag : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Services", "Suggested", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Services", "Suggested");
        }
    }
}
