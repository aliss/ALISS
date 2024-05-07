namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServiceReviewTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ServiceReviews",
                c => new
                    {
                        ReviewId = c.Guid(nullable: false),
                        ServiceId = c.Guid(nullable: false),
                        LastReviewedDate = c.DateTime(),
                        LastReviewedUserId = c.Int(),
                        ReviewEmailState = c.Int(),
                        ReviewEmailDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.ReviewId)
                .ForeignKey("dbo.UserProfiles", t => t.LastReviewedUserId)
                .ForeignKey("dbo.Services", t => t.ServiceId, cascadeDelete: true)
                .Index(t => t.ServiceId)
                .Index(t => t.LastReviewedUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ServiceReviews", "ServiceId", "dbo.Services");
            DropForeignKey("dbo.ServiceReviews", "LastReviewedUserId", "dbo.UserProfiles");
            DropIndex("dbo.ServiceReviews", new[] { "LastReviewedUserId" });
            DropIndex("dbo.ServiceReviews", new[] { "ServiceId" });
            DropTable("dbo.ServiceReviews");
        }
    }
}
