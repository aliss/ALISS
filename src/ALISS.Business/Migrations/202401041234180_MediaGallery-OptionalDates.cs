namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MediaGalleryOptionalDates : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.MediaGalleries", "ApprovedDate", c => c.DateTime());
            AlterColumn("dbo.MediaGalleries", "UploadDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.MediaGalleries", "UploadDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.MediaGalleries", "ApprovedDate", c => c.DateTime(nullable: false));
        }
    }
}
