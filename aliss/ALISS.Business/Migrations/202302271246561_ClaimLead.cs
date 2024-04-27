namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClaimLead : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Claims", "RequestLeadClaimant", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Claims", "RequestLeadClaimant");
        }
    }
}
