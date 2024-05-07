namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServiceClaimUsersTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ServiceClaimUsers",
                c => new
                    {
                        ServiceClaimUserId = c.Guid(nullable: false),
                        ServiceId = c.Guid(),
                        ClaimedUserId = c.Int(),
                        ServiceClaimId = c.Guid(),
                        IsLeadClaimant = c.Boolean(nullable: false),
                        ApprovedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ServiceClaimUserId)
                .ForeignKey("dbo.ServiceClaims", t => t.ServiceClaimId)
                .ForeignKey("dbo.UserProfiles", t => t.ClaimedUserId)
                .ForeignKey("dbo.Services", t => t.ServiceId)
                .Index(t => t.ServiceId)
                .Index(t => t.ClaimedUserId)
                .Index(t => t.ServiceClaimId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ServiceClaimUsers", "ServiceId", "dbo.Services");
            DropForeignKey("dbo.ServiceClaimUsers", "ClaimedUserId", "dbo.UserProfiles");
            DropForeignKey("dbo.ServiceClaimUsers", "ServiceClaimId", "dbo.ServiceClaims");
            DropIndex("dbo.ServiceClaimUsers", new[] { "ServiceClaimId" });
            DropIndex("dbo.ServiceClaimUsers", new[] { "ClaimedUserId" });
            DropIndex("dbo.ServiceClaimUsers", new[] { "ServiceId" });
            DropTable("dbo.ServiceClaimUsers");
        }
    }
}
