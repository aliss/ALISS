namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MediaGalleryUploadApproved : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MediaGalleries", "ApprovedByUserId", c => c.Int(nullable: false));
            AddColumn("dbo.MediaGalleries", "ApprovedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.MediaGalleries", "UploadDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MediaGalleries", "UploadDate");
            DropColumn("dbo.MediaGalleries", "ApprovedDate");
            DropColumn("dbo.MediaGalleries", "ApprovedByUserId");
        }
    }
}
