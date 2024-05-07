namespace ALISS.Business.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class claims : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Claims",
                c => new
                {
                    ClaimId = c.Guid(nullable: false),
                    OrganisationId = c.Guid(nullable: false),
                    RepresentativeRole = c.String(),
                    RepresentativeName = c.String(maxLength: 50),
                    RepresentativePhone = c.String(maxLength: 30),
                    CreatedOn = c.DateTime(nullable: false),
                    Status = c.Int(nullable: false),
                    ClaimedUserId = c.Int(nullable: false),
                    ReviewedByUserId = c.Int(),
                    ReviewedOn = c.DateTime(),
                })
                .PrimaryKey(t => t.ClaimId)
                .ForeignKey("dbo.UserProfiles", t => t.ClaimedUserId)
                .ForeignKey("dbo.Organisations", t => t.OrganisationId)
                .ForeignKey("dbo.UserProfiles", t => t.ReviewedByUserId)
                .Index(t => t.OrganisationId)
                .Index(t => t.ClaimedUserId)
                .Index(t => t.ReviewedByUserId);

        }

        public override void Down()
        {
            DropForeignKey("dbo.Claims", "ReviewedByUserId", "dbo.UserProfiles");
            DropForeignKey("dbo.Claims", "OrganisationId", "dbo.Organisations");
            DropForeignKey("dbo.Claims", "ClaimedUserId", "dbo.UserProfiles");
            DropIndex("dbo.Claims", new[] { "ReviewedByUserId" });
            DropIndex("dbo.Claims", new[] { "ClaimedUserId" });
            DropIndex("dbo.Claims", new[] { "OrganisationId" });
            DropTable("dbo.Claims");
        }
    }
}
