namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MediaGalleryReference : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MediaGalleries", "GalleryReference", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MediaGalleries", "GalleryReference");
        }
    }
}
