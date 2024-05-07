namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MediaGalleryUserIdsNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.MediaGalleries", "ApprovedByUserId", c => c.Int());
            AlterColumn("dbo.MediaGalleries", "UploadUserId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.MediaGalleries", "UploadUserId", c => c.Int(nullable: false));
            AlterColumn("dbo.MediaGalleries", "ApprovedByUserId", c => c.Int(nullable: false));
        }
    }
}
