namespace ALISS.Business.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class audits : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrganisationAudits",
                c => new
                {
                    OrganisationAuditId = c.Guid(nullable: false),
                    OrganisationId = c.Guid(nullable: false),
                    UserProfileId = c.Int(nullable: false),
                    DateOfAction = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.OrganisationAuditId)
                .ForeignKey("dbo.Organisations", t => t.OrganisationId)
                .ForeignKey("dbo.UserProfiles", t => t.UserProfileId)
                .Index(t => t.OrganisationId)
                .Index(t => t.UserProfileId);

            CreateTable(
                "dbo.ServiceAudits",
                c => new
                {
                    ServiceAuditId = c.Guid(nullable: false),
                    ServiceId = c.Guid(nullable: false),
                    UserProfileId = c.Int(nullable: false),
                    DateOfAction = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ServiceAuditId)
                .ForeignKey("dbo.Services", t => t.ServiceId)
                .ForeignKey("dbo.UserProfiles", t => t.UserProfileId)
                .Index(t => t.ServiceId)
                .Index(t => t.UserProfileId);

            AddColumn("dbo.UserProfiles", "Username", c => c.String(maxLength: 254));
            AddColumn("dbo.UserProfiles", "LastLoggedInDate", c => c.DateTime());
        }

        public override void Down()
        {
            DropForeignKey("dbo.ServiceAudits", "UserProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.ServiceAudits", "ServiceId", "dbo.Services");
            DropForeignKey("dbo.OrganisationAudits", "UserProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.OrganisationAudits", "OrganisationId", "dbo.Organisations");
            DropIndex("dbo.ServiceAudits", new[] { "UserProfileId" });
            DropIndex("dbo.ServiceAudits", new[] { "ServiceId" });
            DropIndex("dbo.OrganisationAudits", new[] { "UserProfileId" });
            DropIndex("dbo.OrganisationAudits", new[] { "OrganisationId" });
            DropColumn("dbo.UserProfiles", "LastLoggedInDate");
            DropColumn("dbo.UserProfiles", "Username");
            DropTable("dbo.ServiceAudits");
            DropTable("dbo.OrganisationAudits");
        }
    }
}
