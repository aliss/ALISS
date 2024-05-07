namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrganisationClaimUser : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrganisationClaimUsers",
                c => new
                    {
                        OrganisationClaimUserId = c.Guid(nullable: false),
                        OrganisationId = c.Guid(),
                        ClaimedUserId = c.Int(),
                        ClaimId = c.Guid(),
                        IsLeadClaimant = c.Boolean(nullable: false),
                        ApprovedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.OrganisationClaimUserId)
                .ForeignKey("dbo.Claims", t => t.ClaimId)
                .ForeignKey("dbo.UserProfiles", t => t.ClaimedUserId)
                .ForeignKey("dbo.Organisations", t => t.OrganisationId)
                .Index(t => t.OrganisationId)
                .Index(t => t.ClaimedUserId)
                .Index(t => t.ClaimId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrganisationClaimUsers", "OrganisationId", "dbo.Organisations");
            DropForeignKey("dbo.OrganisationClaimUsers", "ClaimedUserId", "dbo.UserProfiles");
            DropForeignKey("dbo.OrganisationClaimUsers", "ClaimId", "dbo.Claims");
            DropIndex("dbo.OrganisationClaimUsers", new[] { "ClaimId" });
            DropIndex("dbo.OrganisationClaimUsers", new[] { "ClaimedUserId" });
            DropIndex("dbo.OrganisationClaimUsers", new[] { "OrganisationId" });
            DropTable("dbo.OrganisationClaimUsers");
        }
    }
}
