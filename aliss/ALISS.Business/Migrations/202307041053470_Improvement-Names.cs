namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ImprovementNames : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Improvements", "ServiceName", c => c.String(maxLength: 100));
            AddColumn("dbo.Improvements", "OrganisationName", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Improvements", "OrganisationName");
            DropColumn("dbo.Improvements", "ServiceName");
        }
    }
}
