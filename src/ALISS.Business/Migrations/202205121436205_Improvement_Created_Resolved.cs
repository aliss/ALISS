namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Improvement_Created_Resolved : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Improvements", "CreatedOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.Improvements", "Resolved", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Improvements", "Resolved");
            DropColumn("dbo.Improvements", "CreatedOn");
        }
    }
}
