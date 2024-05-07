namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServiceClaimTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ServiceClaims",
                c => new
                    {
                        ClaimId = c.Guid(nullable: false),
                        ServiceId = c.Guid(),
                        RepresentativeRole = c.String(),
                        RepresentativeName = c.String(maxLength: 50),
                        RepresentativePhone = c.String(maxLength: 30),
                        RequestLeadClaimant = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                        ClaimedUserId = c.Int(),
                        ReviewedByUserId = c.Int(),
                        ReviewedOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.ClaimId)
                .ForeignKey("dbo.UserProfiles", t => t.ClaimedUserId)
                .ForeignKey("dbo.UserProfiles", t => t.ReviewedByUserId)
                .ForeignKey("dbo.Services", t => t.ServiceId)
                .Index(t => t.ServiceId)
                .Index(t => t.ClaimedUserId)
                .Index(t => t.ReviewedByUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ServiceClaims", "ServiceId", "dbo.Services");
            DropForeignKey("dbo.ServiceClaims", "ReviewedByUserId", "dbo.UserProfiles");
            DropForeignKey("dbo.ServiceClaims", "ClaimedUserId", "dbo.UserProfiles");
            DropIndex("dbo.ServiceClaims", new[] { "ReviewedByUserId" });
            DropIndex("dbo.ServiceClaims", new[] { "ClaimedUserId" });
            DropIndex("dbo.ServiceClaims", new[] { "ServiceId" });
            DropTable("dbo.ServiceClaims");
        }
    }
}
