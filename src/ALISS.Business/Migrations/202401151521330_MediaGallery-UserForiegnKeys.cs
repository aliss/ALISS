namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MediaGalleryUserForiegnKeys : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.MediaGalleries", "ApprovedByUserId");
            CreateIndex("dbo.MediaGalleries", "UploadUserId");
            AddForeignKey("dbo.MediaGalleries", "ApprovedByUserId", "dbo.UserProfiles", "UserProfileId");
            AddForeignKey("dbo.MediaGalleries", "UploadUserId", "dbo.UserProfiles", "UserProfileId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MediaGalleries", "UploadUserId", "dbo.UserProfiles");
            DropForeignKey("dbo.MediaGalleries", "ApprovedByUserId", "dbo.UserProfiles");
            DropIndex("dbo.MediaGalleries", new[] { "UploadUserId" });
            DropIndex("dbo.MediaGalleries", new[] { "ApprovedByUserId" });
        }
    }
}
