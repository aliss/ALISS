namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AuditMakeIdsNullable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OrganisationAudits", "OrganisationId", "dbo.Organisations");
            DropForeignKey("dbo.OrganisationAudits", "UserProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.ServiceAudits", "ServiceId", "dbo.Services");
            DropForeignKey("dbo.ServiceAudits", "UserProfileId", "dbo.UserProfiles");
            DropIndex("dbo.OrganisationAudits", new[] { "OrganisationId" });
            DropIndex("dbo.OrganisationAudits", new[] { "UserProfileId" });
            DropIndex("dbo.ServiceAudits", new[] { "ServiceId" });
            DropIndex("dbo.ServiceAudits", new[] { "UserProfileId" });
            AlterColumn("dbo.OrganisationAudits", "OrganisationId", c => c.Guid());
            AlterColumn("dbo.OrganisationAudits", "UserProfileId", c => c.Int());
            AlterColumn("dbo.ServiceAudits", "ServiceId", c => c.Guid());
            AlterColumn("dbo.ServiceAudits", "OrganisationId", c => c.Guid());
            AlterColumn("dbo.ServiceAudits", "UserProfileId", c => c.Int());
            CreateIndex("dbo.OrganisationAudits", "OrganisationId");
            CreateIndex("dbo.OrganisationAudits", "UserProfileId");
            CreateIndex("dbo.ServiceAudits", "ServiceId");
            CreateIndex("dbo.ServiceAudits", "OrganisationId");
            CreateIndex("dbo.ServiceAudits", "UserProfileId");
            AddForeignKey("dbo.ServiceAudits", "OrganisationId", "dbo.Organisations", "OrganisationId");
            AddForeignKey("dbo.OrganisationAudits", "OrganisationId", "dbo.Organisations", "OrganisationId");
            AddForeignKey("dbo.OrganisationAudits", "UserProfileId", "dbo.UserProfiles", "UserProfileId");
            AddForeignKey("dbo.ServiceAudits", "ServiceId", "dbo.Services", "ServiceId");
            AddForeignKey("dbo.ServiceAudits", "UserProfileId", "dbo.UserProfiles", "UserProfileId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ServiceAudits", "UserProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.ServiceAudits", "ServiceId", "dbo.Services");
            DropForeignKey("dbo.OrganisationAudits", "UserProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.OrganisationAudits", "OrganisationId", "dbo.Organisations");
            DropForeignKey("dbo.ServiceAudits", "OrganisationId", "dbo.Organisations");
            DropIndex("dbo.ServiceAudits", new[] { "UserProfileId" });
            DropIndex("dbo.ServiceAudits", new[] { "OrganisationId" });
            DropIndex("dbo.ServiceAudits", new[] { "ServiceId" });
            DropIndex("dbo.OrganisationAudits", new[] { "UserProfileId" });
            DropIndex("dbo.OrganisationAudits", new[] { "OrganisationId" });
            AlterColumn("dbo.ServiceAudits", "UserProfileId", c => c.Int(nullable: false));
            AlterColumn("dbo.ServiceAudits", "OrganisationId", c => c.Guid(nullable: false));
            AlterColumn("dbo.ServiceAudits", "ServiceId", c => c.Guid(nullable: false));
            AlterColumn("dbo.OrganisationAudits", "UserProfileId", c => c.Int(nullable: false));
            AlterColumn("dbo.OrganisationAudits", "OrganisationId", c => c.Guid(nullable: false));
            CreateIndex("dbo.ServiceAudits", "UserProfileId");
            CreateIndex("dbo.ServiceAudits", "ServiceId");
            CreateIndex("dbo.OrganisationAudits", "UserProfileId");
            CreateIndex("dbo.OrganisationAudits", "OrganisationId");
            AddForeignKey("dbo.ServiceAudits", "UserProfileId", "dbo.UserProfiles", "UserProfileId", cascadeDelete: true);
            AddForeignKey("dbo.ServiceAudits", "ServiceId", "dbo.Services", "ServiceId", cascadeDelete: true);
            AddForeignKey("dbo.OrganisationAudits", "UserProfileId", "dbo.UserProfiles", "UserProfileId", cascadeDelete: true);
            AddForeignKey("dbo.OrganisationAudits", "OrganisationId", "dbo.Organisations", "OrganisationId", cascadeDelete: true);
        }
    }
}
