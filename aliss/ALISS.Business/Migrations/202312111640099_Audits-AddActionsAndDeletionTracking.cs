namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AuditsAddActionsAndDeletionTracking : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrganisationAudits", "OrganisationName", c => c.String());
            AddColumn("dbo.OrganisationAudits", "UserProfileName", c => c.String());
            AddColumn("dbo.OrganisationAudits", "Action", c => c.String());
            AddColumn("dbo.ServiceAudits", "ServiceName", c => c.String());
            AddColumn("dbo.ServiceAudits", "OrganisationId", c => c.Guid(nullable: false));
            AddColumn("dbo.ServiceAudits", "OrganisationName", c => c.String());
            AddColumn("dbo.ServiceAudits", "UserProfileName", c => c.String());
            AddColumn("dbo.ServiceAudits", "Action", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceAudits", "Action");
            DropColumn("dbo.ServiceAudits", "UserProfileName");
            DropColumn("dbo.ServiceAudits", "OrganisationName");
            DropColumn("dbo.ServiceAudits", "OrganisationId");
            DropColumn("dbo.ServiceAudits", "ServiceName");
            DropColumn("dbo.OrganisationAudits", "Action");
            DropColumn("dbo.OrganisationAudits", "UserProfileName");
            DropColumn("dbo.OrganisationAudits", "OrganisationName");
        }
    }
}
